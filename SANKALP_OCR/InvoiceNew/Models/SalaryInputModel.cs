using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvoiceNew.Models
{
    public class SalaryInputModel
    {
        public string LocationId { get; set; }
        public List<SelectListItem> LocationList { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public bool Update { get; set; }
        public int UpdateId { get; set; }
    }
}