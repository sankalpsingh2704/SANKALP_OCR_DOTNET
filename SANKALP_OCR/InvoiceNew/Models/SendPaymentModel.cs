using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceNew.Models
{
	public class SendPaymentModel
	{
		public string BankName { get; set; }
		public string Code { get; set; }
		public string Service { get; set; }
		public decimal Amount { get; set; }
	}
}