using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvoiceNew.Models
{
    public class SalarySearchModel
    {
        public int LocationId { get; set; }
        public List<SelectListItem> LocationList { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public List<SearchCategory> SearchColumn { get; set; }
        public List<SelectListItem> SearchList { get; set; }
        public string SelectedCategory { get; set; }
        public string SearchValue { get; set; }
    }
    public class SearchCategory
    {
        public int Sno { get; set; }
        public string Search { get; set; }
        public bool CheckBox { get; set; } = false;
    }
}