using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceNew.Models
{
    public class SavePaymentList
    {
        public List<PaymentModel> Data { get; set; }
    }
    public class PaymentModel
    {
        public string InvoiceNumber { get; set; }
		public string InvoiceAmount { get; set; }
		public string VendorName { get; set; }
		public string Days { get; set; }
		public string InvoiceDueDate { get; set; }

	}
}