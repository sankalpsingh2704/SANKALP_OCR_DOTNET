using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceNew.Models
{
    public class ProductsModel
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string ItemId { get; set; }
        public string HSNCode { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string UOM { get; set; }
        public string Price { get; set; }
        public int SGST { get; set; }
        public int CGST { get; set; }
        public int IGST { get; set; }
        public int UpdateId { get; set; }
        public bool IsActive { get; set; }


    }
    public class PageModel
    {
        public int TotalProducts { get; set; }
        public int CurrentPage { get; set; }
    }
}