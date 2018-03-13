﻿using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Goldmint.WebApplication.Models.API.v1.User.SwiftModels {

	public class DepositModel : BaseValidableModel {

		/// <summary>
		/// USD amount
		/// </summary>
		[Required]
		public double Amount { get; set; }

		protected override FluentValidation.Results.ValidationResult ValidateFields() {
			var v = new InlineValidator<DepositModel>();
			v.CascadeMode = CascadeMode.Continue;

			v.RuleFor(_ => _.Amount)
				.GreaterThanOrEqualTo(1)
				.WithMessage("Invalid amount")
			;

			return v.Validate(this);
		}
	}

	public class DepositView {

		/// <summary>
		/// Beneficiary name
		/// </summary>
		[Required]
		public string BenName { get; set; }

		/// <summary>
		/// Beneficiary address
		/// </summary>
		[Required]
		public string BenAddress { get; set; }

		/// <summary>
		/// Beneficiary IBAN
		/// </summary>
		[Required]
		public string BenIban { get; set; }

		/// <summary>
		/// Beneficiary bank name
		/// </summary>
		[Required]
		public string BenBankName { get; set; }

		/// <summary>
		/// Beneficiary bank addr
		/// </summary>
		[Required]
		public string BenBankAddress { get; set; }

		/// <summary>
		/// Beneficiary bank SWIFT
		/// </summary>
		[Required]
		public string BenSwift { get; set; }

		/// <summary>
		/// Payment ref
		/// </summary>
		[Required]
		public string Reference { get; set; }
	}

	// ---

	public class WithdrawModel : BaseValidableModel {

		/// <summary>
		/// USD amount
		/// </summary>
		[Required]
		public double Amount { get; set; }

		/// <summary>
		/// Account holder's name, .{1,256}
		/// </summary>
		[Required]
		public string Holder { get; set; }

		/// <summary>
		/// IBAN, account number, .{1,256}
		/// </summary>
		[Required]
		public string Iban { get; set; }

		/// <summary>
		/// Bank name, .{1,256}
		/// </summary>
		[Required]
		public string Bank { get; set; }

		/// <summary>
		/// BIC / SWIFT, .{1,128}
		/// </summary>
		[Required]
		public string Bic { get; set; }

		/// <summary>
		/// Details, .{1-1024}
		/// </summary>
		[Required]
		public string Details { get; set; }

		protected override FluentValidation.Results.ValidationResult ValidateFields() {
			var v = new InlineValidator<WithdrawModel>();
			v.CascadeMode = CascadeMode.Continue;

			v.RuleFor(_ => _.Amount)
				.GreaterThanOrEqualTo(1)
				.WithMessage("Invalid amount")
			;

			v.RuleFor(_ => _.Holder)
				.NotEmpty()
				.MaximumLength(256)
				.WithMessage("Invalid holder name")
			;

			v.RuleFor(_ => _.Iban)
				.NotEmpty()
				.MaximumLength(256)
				.WithMessage("Invalid IBAN")
			;

			v.RuleFor(_ => _.Bank)
				.NotEmpty()
				.MaximumLength(256)
				.WithMessage("Invalid bank name")
			;

			v.RuleFor(_ => _.Bic)
				.NotEmpty()
				.MaximumLength(128)
				.WithMessage("Invalid BIC/SWIFT")
			;

			v.RuleFor(_ => _.Details)
				.NotEmpty()
				.MaximumLength(1024)
				.WithMessage("Invalid details")
			;

			return v.Validate(this);

		}
	}

	public class WithdrawView {

		[Required]
		public string Reference { get; set; }
	}
}
