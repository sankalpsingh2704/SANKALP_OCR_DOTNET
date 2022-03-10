using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvoiceNew.Models
{
	public class PaymentInputModel
	{
		
		public string PageName { get; set; }
		public List<SelectListItem> PageTypes { get; set; }
		public int Month { get; set; }
		public int Year { get; set; }
		public string TallyComment { get; set; }

	}
}