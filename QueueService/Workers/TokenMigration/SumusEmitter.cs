﻿using Goldmint.Common;
using Goldmint.Common.Extensions;
using Goldmint.CoreLogic.Services.Blockchain.Sumus;
using Goldmint.CoreLogic.Services.Blockchain.Sumus.Models;
using Goldmint.CoreLogic.Services.Localization;
using Goldmint.CoreLogic.Services.Notification;
using Goldmint.CoreLogic.Services.Notification.Impl;
using Goldmint.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Goldmint.QueueService.Workers.TokenMigration {

	public class SumusEmitter : BaseWorker {

		private readonly int _rowsPerRound;

		private ILogger _logger;
		private AppConfig _appConfig;
		private ApplicationDbContext _dbContext;
		private ISumusReader _sumusReader;
		private ISumusWriter _sumusWriter;
		private INotificationQueue _notificationQueue;
		private ITemplateProvider _templateProvider;
		private Common.Sumus.Signer _emitterSigner;

		private long _statProcessed = 0;
		private long _statFailed = 0;

		// ---

		public SumusEmitter(int rowsPerRound) {
			_rowsPerRound = Math.Max(1, rowsPerRound);
		}

		protected override async Task OnInit(IServiceProvider services) {
			_appConfig = services.GetRequiredService<AppConfig>();
			_logger = services.GetLoggerFor(this.GetType());
			_dbContext = services.GetRequiredService<ApplicationDbContext>();
			_sumusReader = services.GetRequiredService<ISumusReader>();
			_sumusWriter = services.GetRequiredService<ISumusWriter>();
			_notificationQueue = services.GetRequiredService<INotificationQueue>();
			_templateProvider = services.GetRequiredService<ITemplateProvider>();

			// emitter
			{
				if (!Common.Sumus.Pack58.Unpack(_appConfig.Services.Sumus.MigrationEmissionPk, out var pk)) {
					throw new ArgumentException("Sumus emission private key is invalid");
				}
				_emitterSigner = new Common.Sumus.Signer(pk, 0);
				var ws = await _sumusReader.GetWalletState(_emitterSigner.PublicKey);
				if (ws == null) {
					throw new Exception("Failed to get sumus emitter wallet state");
				}
				_emitterSigner.SetNonce(ws.LastNonce);
			}
		}

		protected override async Task OnUpdate() {

			_dbContext.DetachEverything();

			var nowTime = DateTime.UtcNow;

			var rows = await (
					from r in _dbContext.MigrationEthereumToSumusRequest
					where
						r.Status == MigrationRequestStatus.Emission &&
						r.TimeNextCheck <= nowTime
					select r
				)
				.Include(_ => _.User)
				.AsTracking()
				.OrderBy(_ => _.Id)
				.Take(_rowsPerRound)
				.ToArrayAsync()
			;
			if (IsCancelled()) return;

			_logger.Debug(rows.Length > 0 ? $"{rows.Length} request(s) found" : "Nothing found");

			foreach (var row in rows) {

				if (IsCancelled()) return;

				row.Status = MigrationRequestStatus.EmissionStarted;
				await _dbContext.SaveChangesAsync();

				SentTransaction sumTransaction = null;

				if (row.Amount != null && Common.Sumus.Pack58.Unpack(row.SumAddress, out var sumusAddressBytes)) {
					SumusToken stoken = 0;
					switch (row.Asset) {
						case MigrationRequestAsset.Gold: 
							stoken = SumusToken.Gold;
							break;
						case MigrationRequestAsset.Mnt: 
							stoken = SumusToken.Mnt;
							break;
					}
					sumTransaction = await _sumusWriter.TransferToken(
						_emitterSigner, 
						_emitterSigner.NextNonce(), 
						sumusAddressBytes, 
						stoken, 
						row.Amount.Value.ToSumus()
					);
				}

				if (sumTransaction != null) {
					row.TimeEmitted = DateTime.UtcNow;

					// TODO: check?
					// row.Status = MigrationRequestStatus.EmissionConfirmation;
					// row.SumTransaction = sumTransaction;
					// row.TimeNextCheck = DateTime.UtcNow.Add(nextCheckDelay);
					
					row.Status = MigrationRequestStatus.Completed;
					row.SumTransaction = sumTransaction.Digest;
					row.TimeCompleted = DateTime.UtcNow;
				}
				else {
					row.Status = MigrationRequestStatus.Failed;
					row.TimeCompleted = DateTime.UtcNow;
				}
				await _dbContext.SaveChangesAsync();

				// notify
				if (sumTransaction != null) {
					try {
						await EmailComposer
							.FromTemplate(await _templateProvider.GetEmailTemplate(EmailTemplate.ExchangeEthTransferred, Locale.En))
							.ReplaceBodyTag("REQUEST_ID", row.Id.ToString())
							.ReplaceBodyTag("TOKEN", row.Asset.ToString().ToUpperInvariant())
							.ReplaceBodyTag("LINK", _appConfig.Services.Sumus.ScannerTxView + sumTransaction.Digest)
							.ReplaceBodyTag("DETAILS_SOURCE", TextFormatter.MaskBlockchainAddress(row.EthAddress))
							.ReplaceBodyTag("DETAILS_AMOUNT", row.Amount.Value.ToString("F"))
							.ReplaceBodyTag("DETAILS_DESTINATION", TextFormatter.MaskBlockchainAddress(row.SumAddress))
							.Send(row.User.Email, row.User.UserName, _notificationQueue)
						;
					} catch { }
				}

				if (sumTransaction != null) {
					++_statProcessed;
					_logger.Info($"Request {row.Id} - emission success");
				}
				else {
					++_statFailed;
					_logger.Error($"Request {row.Id} - emission failed");
				}
			}
		}
	}
}
