using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceNew.Models
{
    public class ExcelQueryModel
    {
        public int LocationId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public List<string> SearchList { get; set; }
        public string SelectedCategory { get; set; }
        public string SearchValue { get; set; }
    }
}