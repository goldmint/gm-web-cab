﻿using Goldmint.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Goldmint.DAL.Models {

	[Table("gm_issue_gold_tx")]
	public class IssueGoldTransaction : BaseUserFinHistoryEntity, IConcurrentUpdate {

		[Column("status"), Required]
		public EthereumOperationStatus Status { get; set; }

		[Column("address"), MaxLength(FieldMaxLength.BlockchainAddress), Required]
		public string DestinationAddress { get; set; }

		[Column("amount"), MaxLength(FieldMaxLength.BlockchainCurrencyAmount), Required]
		public string Amount { get; set; }

		[Column("eth_txid"), MaxLength(FieldMaxLength.EthereumTransactionHash)]
		public string EthTransactionId { get; set; }

		[Column("desk_ticket_id"), MaxLength(FieldMaxLength.Guid), Required]
		public string DeskTicketId { get; set; }

		[Column("origin"), Required]
		public IssueGoldOrigin Origin { get; set; }

		[Column("origin_id"), Required]
		public long OriginId { get; set; }

		[Column("time_created"), Required]
		public DateTime TimeCreated { get; set; }

		[Column("time_next_check"), Required]
		public DateTime TimeNextCheck { get; set; }

		[Column("time_completed")]
		public DateTime? TimeCompleted { get; set; }

		[Column("concurrency_stamp"), MaxLength(FieldMaxLength.ConcurrencyStamp), ConcurrencyCheck]
		public string ConcurrencyStamp { get; set; }

		// ---

		public void OnConcurrencyStampRegen() {
			this.ConcurrencyStamp = ConcurrentStamp.GetGuid();
		}
	}
}
