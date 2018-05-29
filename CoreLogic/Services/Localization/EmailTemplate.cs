﻿namespace Goldmint.CoreLogic.Services.Localization {

	public sealed class EmailTemplate {

		public const string EmailConfirmation = "EmailConfirmation";
		public const string PasswordChanged = "PasswordChanged";
		public const string PasswordRestoration = "PasswordRestoration";
		public const string SignedIn = "SignedIn";
		public const string TfaEnabled = "TfaEnabled";
		public const string TfaDisabled = "TfaDisabled";
		public const string ProofOfResidenceApproved = "ProofOfResidenceApproved";
		public const string ProofOfResidenceRejected = "ProofOfResidenceRejected";
		public const string SwiftDepositInvoice = "SwiftDepositInvoice";
		public const string ExchangeGoldIssued = "ExchangeGoldIssued";

		public string Subject { get; set; }
		public string Body { get; set; }
	}
}
