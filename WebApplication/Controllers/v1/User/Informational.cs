﻿using Goldmint.Common;
using Goldmint.WebApplication.Core.Policies;
using Goldmint.WebApplication.Core.Response;
using Goldmint.WebApplication.Models.API;
using Goldmint.WebApplication.Models.API.v1.User.UserModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goldmint.WebApplication.Controllers.v1.User {

	public partial class UserController : BaseController {

		/// <summary>
		/// Fiat limits
		/// </summary>
		[RequireJWTAudience(JwtAudience.App), RequireJWTArea(JwtArea.Authorized), RequireAccessRights(AccessRights.Client)]
		[HttpGet, Route("limits")]
		[ProducesResponseType(typeof(LimitsView), 200)]
		public async Task<APIResponse> Limits() {

			var user = await GetUserFromDb();
			var userTier = CoreLogic.User.GetTier(user);

			var limits = await CoreLogic.User.GetFiatLimits(HttpContext.RequestServices, FiatCurrency.USD, userTier);

			var curDepositLimit = await CoreLogic.User.GetCurrentFiatDepositLimit(HttpContext.RequestServices, FiatCurrency.USD, user.Id, userTier);
			var curWithdrawLimit = await CoreLogic.User.GetCurrentFiatWithdrawLimit(HttpContext.RequestServices, FiatCurrency.USD, user.Id, userTier);

			return APIResponse.Success(
				new LimitsView() {

					// current user fiat limits
					Current = new LimitsView.UserLimits() {

						Deposit = new LimitsView.UserPeriodLimitItem() {
							Minimal = curDepositLimit.Minimal / 100d,
							Day = curDepositLimit.Day / 100d,
							Month = curDepositLimit.Month / 100d,
						},

						Withdraw = new LimitsView.UserPeriodLimitItem() {
							Minimal = curWithdrawLimit.Minimal / 100d,
							Day = curWithdrawLimit.Day / 100d,
							Month = curWithdrawLimit.Month / 100d,
						},
					},

					// limits by verification level and current user level
					Levels = new LimitsView.VerificationLevels() {

						Current = new LimitsView.VerificationLevelLimits() {

							Deposit = new LimitsView.PeriodLimitItem() {
								Day = limits.Current.Deposit.Day / 100d,
								Month = limits.Current.Deposit.Month / 100d,
							},
							Withdraw = new LimitsView.PeriodLimitItem() {
								Day = limits.Current.Withdraw.Day / 100d,
								Month = limits.Current.Withdraw.Month / 100d,
							}
						},

						L0 = new LimitsView.VerificationLevelLimits() {

							Deposit = new LimitsView.PeriodLimitItem() {
								Day = limits.Tier1.Deposit.Day / 100d,
								Month = limits.Tier1.Deposit.Month / 100d,
							},
							Withdraw = new LimitsView.PeriodLimitItem() {
								Day = limits.Tier1.Withdraw.Day / 100d,
								Month = limits.Tier1.Withdraw.Month / 100d,
							}
						},

						L1 = new LimitsView.VerificationLevelLimits() {

							Deposit = new LimitsView.PeriodLimitItem() {
								Day = limits.Tier2.Deposit.Day / 100d,
								Month = limits.Tier2.Deposit.Month / 100d,
							},
							Withdraw = new LimitsView.PeriodLimitItem() {
								Day = limits.Tier2.Withdraw.Day / 100d,
								Month = limits.Tier2.Withdraw.Month / 100d,
							}
						}
					},

					// limits per payment method
					PaymentMethod = new LimitsView.PaymentMethods() {

						Card = new LimitsView.PaymentMethodLimits() {
							Deposit = new LimitsView.OnetimeLimitItem() {
								Min = AppConfig.Constants.CardPaymentData.DepositMin / 100d,
								Max = AppConfig.Constants.CardPaymentData.DepositMax / 100d,
							},
							Withdraw = new LimitsView.OnetimeLimitItem() {
								Min = AppConfig.Constants.CardPaymentData.WithdrawMin / 100d,
								Max = AppConfig.Constants.CardPaymentData.WithdrawMax / 100d,
							}
						},

						Swift = new LimitsView.PaymentMethodLimits() {
							Deposit = new LimitsView.OnetimeLimitItem() {
								Min = AppConfig.Constants.SwiftData.DepositMin / 100d,
								Max = AppConfig.Constants.SwiftData.DepositMax / 100d,
							},
							Withdraw = new LimitsView.OnetimeLimitItem() {
								Min = AppConfig.Constants.SwiftData.WithdrawMin / 100d,
								Max = AppConfig.Constants.SwiftData.WithdrawMax / 100d,
							}
						}
					}
				}
			);
		}

		/// <summary>
		/// Profile info
		/// </summary>
		[RequireJWTArea(JwtArea.Authorized)]
		[HttpGet, Route("profile")]
		[ProducesResponseType(typeof(ProfileView), 200)]
		public async Task<APIResponse> Profile() {

			var user = await GetUserFromDb();
			var userTier = CoreLogic.User.GetTier(user);

			// user challenges
			// TODO: move to challenges subsystem
			var challenges = new List<string>();
			if (!user.UserOptions.InitialTFAQuest && !user.TwoFactorEnabled) challenges.Add("2fa");

			return APIResponse.Success(
				new ProfileView() {
					Id = user.UserName,
					Name = CoreLogic.User.HasFilledPersonalData(user.UserVerification) ? (user.UserVerification.FirstName + " " + user.UserVerification.LastName).Trim() : user.UserName,
					Email = user.Email ?? "",
					DpaSigned = user.UserOptions.DPADocument?.IsSigned ?? false,
					TfaEnabled = user.TwoFactorEnabled,
					VerifiedL0 = userTier >= UserTier.Tier1,
					VerifiedL1 = userTier >= UserTier.Tier2,
					Challenges = challenges.ToArray(),
				}
			);
		}

		/// <summary>
		/// User activity
		/// </summary>
		[RequireJWTAudience(JwtAudience.App), RequireJWTArea(JwtArea.Authorized), RequireAccessRights(AccessRights.Client)]
		[HttpPost, Route("activity")]
		[ProducesResponseType(typeof(ActivityView), 200)]
		public async Task<APIResponse> Activity([FromBody] ActivityModel model) {

			var sortExpression = new Dictionary<string, System.Linq.Expressions.Expression<Func<DAL.Models.UserActivity, object>>>() {
				{ "date", _ => _.TimeCreated },
			};

			// validate
			if (BasePagerModel.IsInvalid(model, sortExpression.Keys, out var errFields)) {
				return APIResponse.BadRequest(errFields);
			}

			var user = await GetUserFromDb();

			var query = (
				from a in DbContext.UserActivity
				where a.UserId == user.Id
				select a
			);

			var page = await DalExtensions.PagerAsync(query, model.Offset, model.Limit,
				sortExpression.GetValueOrDefault(model.Sort), model.Ascending
			);

			var list =
				from i in page.Selected
				select new ActivityViewItem() {
					Type = i.Type.ToLower(),
					Comment = i.Comment,
					Ip = i.Ip,
					Agent = i.Agent,
					Date = ((DateTimeOffset)i.TimeCreated).ToUnixTimeSeconds(),
				}
			;

			return APIResponse.Success(
				new ActivityView() {
					Items = list.ToArray(),
					Limit = model.Limit,
					Offset = model.Offset,
					Total = page.TotalCount,
				}
			);
		}

		/// <summary>
		/// Fiat history
		/// </summary>
		[RequireJWTAudience(JwtAudience.App), RequireJWTArea(JwtArea.Authorized), RequireAccessRights(AccessRights.Client)]
		[HttpPost, Route("fiat/history")]
		[ProducesResponseType(typeof(FiatHistoryView), 200)]
		public async Task<APIResponse> FiatHistory([FromBody] FiatHistoryModel model) {

			var sortExpression = new Dictionary<string, System.Linq.Expressions.Expression<Func<DAL.Models.FinancialHistory, object>>>() {
				{ "date",   _ => _.TimeCreated },
				{ "amount", _ => _.AmountCents },
				{ "type",   _ => _.Type },
				{ "fee",    _ => _.FeeCents },
				{ "status", _ => _.Status }
			};

			// validate
			if (BasePagerModel.IsInvalid(model, sortExpression.Keys, out var errFields)) {
				return APIResponse.BadRequest(errFields);
			}

			var user = await GetUserFromDb();

			var query = (
				from a in DbContext.FinancialHistory
				where a.UserId == user.Id
				select a
			);

			var page = await query.PagerAsync(
				model.Offset, model.Limit,
				sortExpression.GetValueOrDefault(model.Sort), model.Ascending
			);

			var list =
				from i in page.Selected
				select new FiatHistoryViewItem() {
					Type = i.Type.ToString().ToLower(),
					Status = (int)i.Status,
					Comment = i.Comment,
					EthTxId = i.RelEthTransactionId,
					Amount = i.AmountCents / 100d,
					Fee = i.FeeCents > 0 ? (i.FeeCents / 100d) : (double?)null,
					Date = ((DateTimeOffset)i.TimeCreated).ToUnixTimeSeconds(),
				}
			;

			return APIResponse.Success(
				new FiatHistoryView() {
					Items = list.ToArray(),
					Limit = model.Limit,
					Offset = model.Offset,
					Total = page.TotalCount,
				}
			);
		}
	}
}