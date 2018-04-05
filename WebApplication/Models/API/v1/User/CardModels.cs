﻿using Goldmint.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;
using FluentValidation;

namespace Goldmint.WebApplication.Models.API.v1.User.CardModels {

	public class ListView {

		/// <summary>
		/// List
		/// </summary>
		[Required]
		public Item[] List { get; set; }

		// ---

		public class Item {

			/// <summary>
			/// Card ID
			/// </summary>
			[Required]
			public long CardId { get; set; }

			/// <summary>
			/// Card masked number
			/// </summary>
			[Required]
			public string Mask { get; set; }

			/// <summary>
			/// Card status: see 'status' method
			/// </summary>
			[Required]
			public string Status { get; set; }
		}
	}

	// ---

	public class AddModel : BaseValidableModel {

		/// <summary>
		/// URL to redirect user after form filling (`:cardId` will be replaced with card id)
		/// </summary>
		[Required]
		public string Redirect { get; set; }

		protected override FluentValidation.Results.ValidationResult ValidateFields() {
			var v = new InlineValidator<AddModel>() { CascadeMode = CascadeMode.StopOnFirstFailure };

			v.RuleFor(_ => _.Redirect)
				.NotEmpty().WithMessage("Empty")
			;
			
			return v.Validate(this);

		}
	}

	public class AddView {

		/// <summary>
		/// New card ID
		/// </summary>
		[Required]
		public long CardId { get; set; }

		/// <summary>
		/// Redirect to acquirer gateway
		/// </summary>
		[Required]
		public string Redirect { get; set; }

	}

	// ---

	public class ConfirmModel : BaseValidableModel {

		/// <summary>
		/// Card ID
		/// </summary>
		[Required]
		public long CardId { get; set; }

		/// <summary>
		/// URL to redirect user after form filling (`:cardId` will be replaced with card id)
		/// </summary>
		[Required]
		public string Redirect { get; set; }

		protected override FluentValidation.Results.ValidationResult ValidateFields() {
			var v = new InlineValidator<ConfirmModel>() { CascadeMode = CascadeMode.StopOnFirstFailure };

			v.RuleFor(_ => _.CardId)
				.Must(ValidationRules.BeValidId).WithMessage("Invalid id")
			;

			v.RuleFor(_ => _.Redirect)
				.NotEmpty().WithMessage("Empty")
			;

			return v.Validate(this);

		}
	}

	public class ConfirmView {

		/// <summary>
		/// Redirect to acquirer gateway
		/// </summary>
		[Required]
		public string Redirect { get; set; }

	}

	// ---

	public class VerifyModel : BaseValidableModel {

		/// <summary>
		/// Card ID
		/// </summary>
		[Required]
		public long CardId { get; set; }

		/// <summary>
		/// Card code from bank statement /.{1,10}/
		/// </summary>
		[Required]
		public string Code { get; set; }

		protected override FluentValidation.Results.ValidationResult ValidateFields() {
			var v = new InlineValidator<VerifyModel>() { CascadeMode = CascadeMode.StopOnFirstFailure };

			v.RuleFor(_ => _.CardId)
				.Must(ValidationRules.BeValidId).WithMessage("Invalid id")
			;

			v.RuleFor(_ => _.Code)
				.Length(1, 10).WithMessage("Invalid length")
			;

			return v.Validate(this);

		}
	}

	// ---

	public class StatusModel : BaseValidableModel {

		/// <summary>
		/// Card ID
		/// </summary>
		[Required]
		public long CardId { get; set; }

		protected override FluentValidation.Results.ValidationResult ValidateFields() {
			var v = new InlineValidator<StatusModel>() { CascadeMode = CascadeMode.StopOnFirstFailure };

			v.RuleFor(_ => _.CardId)
				.Must(ValidationRules.BeValidId).WithMessage("Invalid id")
			;

			return v.Validate(this);
		}
	}

	public class StatusView {

		/// <summary>
		/// Card status: initial, confirm, payment, verification, verified, disabled, failed
		/// </summary>
		[Required]
		public string Status { get; set; }
	}

	// ---

	public class DepositModel : BaseValidableModel {

		/// <summary>
		/// Card ID
		/// </summary>
		[Required]
		public long CardId { get; set; }

		/// <summary>
		/// USD amount
		/// </summary>
		[Required]
		public double Amount { get; set; }

		protected override FluentValidation.Results.ValidationResult ValidateFields() {
			var v = new InlineValidator<DepositModel>() { CascadeMode = CascadeMode.StopOnFirstFailure };

			v.RuleFor(_ => _.CardId)
				.Must(ValidationRules.BeValidId).WithMessage("Invalid id")
			;

			v.RuleFor(_ => _.Amount)
				.GreaterThanOrEqualTo(1).WithMessage("Invalid amount")
			;

			return v.Validate(this);

		}
	}

	public class DepositView {
	}

	// ---

	public class WithdrawModel : BaseValidableModel {

		/// <summary>
		/// Card ID
		/// </summary>
		[Required]
		public long CardId { get; set; }

		/// <summary>
		/// USD amount
		/// </summary>
		[Required]
		public double Amount { get; set; }

		/// <summary>
		/// TFA Code /[0-9]{6}/
		/// </summary>
		[Required]
		public string Code { get; set; }

		// ---

		protected override FluentValidation.Results.ValidationResult ValidateFields() {
			var v = new InlineValidator<WithdrawModel>() { CascadeMode = CascadeMode.StopOnFirstFailure };

			v.RuleFor(_ => _.CardId)
				.Must(ValidationRules.BeValidId).WithMessage("Invalid id")
			;

			v.RuleFor(_ => _.Amount)
				.GreaterThanOrEqualTo(1).WithMessage("Invalid amount")
			;

			v.RuleFor(_ => _.Code)
				.Must(Common.ValidationRules.BeValidTFACode).WithMessage("Invalid format")
			;

			return v.Validate(this);

		}
	}

	public class WithdrawView {
	}

	// ---

	public class RemoveModel : BaseValidableModel {

		/// <summary>
		/// Card ID
		/// </summary>
		[Required]
		public long CardId { get; set; }

		protected override FluentValidation.Results.ValidationResult ValidateFields() {
			var v = new InlineValidator<RemoveModel>() { CascadeMode = CascadeMode.StopOnFirstFailure };

			v.RuleFor(_ => _.CardId)
				.Must(ValidationRules.BeValidId).WithMessage("Invalid id")
				;

			return v.Validate(this);

		}
	}

	public class RemoveView {
	}
}
