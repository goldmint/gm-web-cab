﻿using Goldmint.Common;
using Goldmint.CoreLogic.Services.Blockchain.Ethereum.Models;
using System.Numerics;
using System.Threading.Tasks;
using Goldmint.CoreLogic.Services.Blockchain.Ethereum.Models.Event;

namespace Goldmint.CoreLogic.Services.Blockchain.Ethereum {

	public interface IEthereumReader {

		/// <summary>
		/// Get ether balance
		/// </summary>
		Task<BigInteger> GetEtherBalance(string address);

		/// <summary>
		/// Get latest block number on the logs provider side
		/// </summary>
		Task<BigInteger> GetLogsLatestBlockNumber();

		/// <summary>
		/// Check chain transaction by it's ID
		/// </summary>
		Task<TransactionInfo> CheckTransaction(string txid, int confirmationsRequired);

		/// <summary>
		/// Get current gas price
		/// </summary>
		Task<BigInteger> GetCurrentGasPrice();

		/// <summary>
		/// Get MNT balance
		/// </summary>
		Task<BigInteger> GetAddressMntBalance(string address);

		///// <summary>
		///// Get GOLD balance
		///// </summary>
		//Task<BigInteger> GetAddressGoldBalance(string address);

		///// <summary>
		///// Get hot wallet GOLD balance
		///// </summary>
		//Task<BigInteger> GetHotWalletGoldBalance(string userId);

		// ---

		///// <summary>
		///// Buy/sell ETH requests current count
		///// </summary>
		//Task<BigInteger> GetBuySellRequestsCount();

		///// <summary>
		///// GOLD buy/sell request base info
		///// </summary>
		//Task<BuySellRequestBaseInfo> GetBuySellRequestBaseInfo(BigInteger requestIndex);

		///// <summary>
		///// Get TokenBuyRequest events
		///// </summary>
		//Task<GatheredTokenBuyEvents> GatherTokenBuyEvents(BigInteger from, BigInteger to, BigInteger confirmationsRequired);

		///// <summary>
		///// Get TokenSellRequest events
		///// </summary>
		//Task<GatheredTokenSellEvents> GatherTokenSellEvents(BigInteger from, BigInteger to, BigInteger confirmationsRequired);

		///// <summary>
		///// Get RequestProcessed events
		///// </summary>
		//Task<GatheredRequestProcessedEvents> GatherRequestProcessedEvents(BigInteger from, BigInteger to, BigInteger confirmationsRequired);

		///// <summary>
		///// Get migration contract transfers
		///// </summary>
		//Task<GatheredLog<MigrationContractTransferEvent>> GatherMigrationContractTransfers(BigInteger from, BigInteger to, BigInteger confirmationsRequired);

		/// <summary>
		/// Get TokenBuyRequest events
		/// </summary>
		Task<GatheredPoolFreezerEvents> GatherPoolFreezerEvents(BigInteger from, BigInteger to, BigInteger confirmationsRequired);
	}
}
