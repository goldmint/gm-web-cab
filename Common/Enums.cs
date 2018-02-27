﻿using System;

namespace Goldmint.Common {

	public enum LoginProvider {

		Google = 1,
	}

	public enum JwtAudience {

		/// <summary>
		/// App access
		/// </summary>
		App = 1,

		/// <summary>
		/// Dashboard access
		/// </summary>
		Dashboard,
	}

	public enum JwtArea {

		/// <summary>
		/// Authorized area
		/// </summary>
		Authorized = 1,

		/// <summary>
		/// Two factor auth area
		/// </summary>
		TFA,

		/// <summary>
		/// OAuth area
		/// </summary>
		OAuth,

		/// <summary>
		/// User registration
		/// </summary>
		Registration,

		/// <summary>
		/// User password restoration
		/// </summary>
		RestorePassword
	}

	public enum AccessRights : long {
		
		/// <summary>
		/// App client
		/// </summary>
		Client = 0x1L,

		/// <summary>
		/// Dashboard: general access, read access
		/// </summary>
		DashboardReadAccess = 0x2L,

		/// <summary>
		/// Dashboard: swift tabs write access
		/// </summary>
		SwiftWriteAccess = 0x10000000L,

		/// <summary>
		/// Dashboard: countries tab write access
		/// </summary>
		CountriesWriteAccess = 0x20000000L,

		/// <summary>
		/// Dashboard: transparency tab write access
		/// </summary>
		TransparencyWriteAccess = 0x40000000L,

		/// <summary>
		/// Critical functions access
		/// </summary>
		Owner = 0x80000000L,
	}

	public enum BlockchainTransactionStatus {

		/// <summary>
		/// Unconfirmed status, still outside of any block
		/// </summary>
		Pending = 1,

		/// <summary>
		/// Transaction confirmed
		/// </summary>
		Success,

		/// <summary>
		/// Transaction cancelled or failed
		/// </summary>
		Failed,
	}

	public enum FiatEnqueueStatus {

		/// <summary>
		/// Enqueued
		/// </summary>
		Success,

		/// <summary>
		/// Limit reached or gateway transaction failed
		/// </summary>
		Limit,

		/// <summary>
		/// Internal error
		/// </summary>
		Error,
	}

	public enum UserOpLogStatus {

		/// <summary>
		/// Operation is pending
		/// </summary>
		Pending = 1,

		/// <summary>
		/// Operation succesfully completed
		/// </summary>
		Completed,

		/// <summary>
		/// Operation is failed
		/// </summary>
		Failed,
	}

	public enum MutexEntity {
		
		/// <summary>
		/// Particular payment should be locked before it will be updated via acquirer (payment-wide)
		/// </summary>
		CardPaymentCheck = 1,

		/// <summary>
		/// Lock should be set while new deposit creation to check limits (user-wide)
		/// </summary>
		DepositEnqueue,

		/// <summary>
		/// Lock should be set while updating deposit (deposit-wide)
		/// </summary>
		DepositCheck,

		/// <summary>
		/// Lock should be set while new withdraw creation to check limits (user-wide)
		/// </summary>
		WithdrawEnqueue,

		/// <summary>
		/// Lock should be set while updating withdraw (withdraw-wide)
		/// </summary>
		WithdrawCheck,

		/// <summary>
		/// Lock should be set while sending a notification (notification-wide)
		/// </summary>
		NotificationSend,

		/// <summary>
		/// Lock should be set while changing buying request state
		/// </summary>
		EthBuyRequest,

		/// <summary>
		/// Lock should be set while changing selling request state
		/// </summary>
		EthSellRequest,

		/// <summary>
		/// Hot wallet operation initiation mutex
		/// </summary>
		HWOperation,

		/// <summary>
		/// Lock should be set while buying gold to hot wallet
		/// </summary>
		HWBuyRequest,

		/// <summary>
		/// Lock should be set while selling gold from hot wallet
		/// </summary>
		HWSellRequest,

		/// <summary>
		/// Lock should be set while transferring gold from hot wallet
		/// </summary>
		HWTransferRequest,
	}

	public enum FiatCurrency {

		USD = 1,
	}

	public enum CardState {

		/// <summary>
		/// User should provide card data for deposit operations
		/// </summary>
		InputDepositData = 1,

		/// <summary>
		/// User should provide card data for withdraw operations
		/// </summary>
		InputWithdrawData,

		/// <summary>
		/// Sending verification payment
		/// </summary>
		Payment,

		/// <summary>
		/// Awaiting for code from bank statement
		/// </summary>
		Verification,

		/// <summary>
		/// Verified
		/// </summary>
		Verified,

		/// <summary>
		/// Disabled by system
		/// </summary>
		Disabled,

		/// <summary>
		/// Deleted
		/// </summary>
		Deleted,
	}

	public enum CardPaymentType {

		/// <summary>
		/// Card verification
		/// </summary>
		Verification = 1,

		/// <summary>
		/// User's deposit payment
		/// </summary>
		Deposit,

		/// <summary>
		/// Card deposit refund on limit
		/// </summary>
		Refund,

		/// <summary>
		/// Withdraw operation
		/// </summary>
		Withdraw,

		/// <summary>
		/// Not payment actually but needed to control card addition
		/// </summary>
		CardDataInputSMS = 10,
		CardDataInputCRD,
		CardDataInputP2P,
	}

	public enum CardGatewayTransactionStatus {

		/// <summary>
		/// Processing
		/// </summary>
		Pending = 1,

		/// <summary>
		/// Finally successful
		/// </summary>
		Success,

		/// <summary>
		/// Completely failed
		/// </summary>
		Failed,

		/// <summary>
		/// Not found on GW side
		/// </summary>
		NotFound
	}

	public enum CardPaymentStatus {

		/// <summary>
		/// Initial state, just enqueued
		/// </summary>
		Pending = 1,

		/// <summary>
		/// There is an attempt to charge
		/// </summary>
		Charging,

		/// <summary>
		/// Final success, finalized
		/// </summary>
		Success,

		/// <summary>
		/// Final failure, finalized
		/// </summary>
		Failed,
	}

	public enum SwiftPaymentType {

		/// <summary>
		/// Deposit
		/// </summary>
		Deposit = 1,

		/// <summary>
		/// Withdraw
		/// </summary>
		Withdraw
	}

	public enum SwiftPaymentStatus {

		/// <summary>
		/// Awaiting payment
		/// </summary>
		Pending = 1,

		/// <summary>
		/// Completed
		/// </summary>
		Success,

		/// <summary>
		/// Cancelled by support
		/// </summary>
		Cancelled,
	}

	public enum DepositSource {

		/// <summary>
		/// Deposit comes from credit card
		/// </summary>
		CreditCard = 1,

		/// <summary>
		/// Deposit comes from bank transaction
		/// </summary>
		Swift,
	}

	public enum DepositStatus {

		/// <summary>
		/// Initially enqueued
		/// </summary>
		Initial = 1,

		/// <summary>
		/// Sending request to blockchain
		/// </summary>
		BlockchainInit,

		/// <summary>
		/// Waiting confirmation from blockchain
		/// </summary>
		BlockchainConfirm,

		/// <summary>
		/// Final success
		/// </summary>
		Success,

		/// <summary>
		/// Final failure
		/// </summary>
		Failed,
	}

	public enum WithdrawDestination {

		/// <summary>
		/// Destination is credit card
		/// </summary>
		CreditCard = 1,

		/// <summary>
		/// Destination is bank transaction
		/// </summary>
		Swift,
	}

	public enum WithdrawStatus {

		/// <summary>
		/// Initially enqueued
		/// </summary>
		Initial = 1,

		/// <summary>
		/// Sending request to blockchain
		/// </summary>
		BlockchainInit,

		/// <summary>
		/// Waiting confirmation from blockchain
		/// </summary>
		BlockchainConfirm,

		/// <summary>
		/// Payment processing (actually used by card payment to wait for payment success)
		/// </summary>
		Processing,

		/// <summary>
		/// Final success
		/// </summary>
		Success,

		/// <summary>
		/// Final failure
		/// </summary>
		Failed,
	}

	public enum ExchangeRequestType {

		/// <summary>
		/// Hot wallet queue
		/// </summary>
		HWRequest = 1,

		/// <summary>
		/// Ethereum requests queue
		/// </summary>
		EthRequest,
	}

	public enum ExchangeRequestStatus {

		/// <summary>
		/// Just created
		/// </summary>
		Initial = 1,

		/// <summary>
		/// Prepared for processing
		/// </summary>
		Processing,

		/// <summary>
		/// Sending request to blockchain
		/// </summary>
		BlockchainInit,

		/// <summary>
		/// Waiting confirmation from blockchain
		/// </summary>
		BlockchainConfirm,

		/// <summary>
		/// Final success
		/// </summary>
		Success,

		/// <summary>
		/// Cancelled
		/// </summary>
		Cancelled,
		
		/// <summary>
		/// Final failure
		/// </summary>
		Failed,
	}

	public enum NotificationType {

		Email = 1,
	}

	public enum DbSetting {

		LastExchangeIndex = 1,
	}

	public enum UserActivityType {

		/// <summary>
		/// User logged in
		/// </summary>
		Auth = 1,

		/// <summary>
		/// Password restoration / change
		/// </summary>
		Password,

		/// <summary>
		/// Some setting changed
		/// </summary>
		Settings,

		/// <summary>
		/// Gold token exchange
		/// </summary>
		Exchange,

		/// <summary>
		/// Credit card operation
		/// </summary>
		CreditCard,

		/// <summary>
		/// Swift reqeust
		/// </summary>
		Swift
	}

	public enum FinancialHistoryType {

		/// <summary>
		/// Deposit
		/// </summary>
		Deposit = 1,

		/// <summary>
		/// Completed
		/// </summary>
		Withdraw,

		/// <summary>
		/// Gold bought
		/// </summary>
		GoldBuying,

		/// <summary>
		/// Gold sold
		/// </summary>
		GoldSelling
	}

	public enum FinancialHistoryStatus {

		/// <summary>
		/// Pending
		/// </summary>
		Pending = 1,

		/// <summary>
		/// Completed
		/// </summary>
		Success,

		/// <summary>
		/// Failed or cancelled
		/// </summary>
		Cancelled
	}

	public enum SignedDocumentType {

		/// <summary>
		/// Primary agreement
		/// </summary>
		PrimaryAgreement = 1,
	}
}
