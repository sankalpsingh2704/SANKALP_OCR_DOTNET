using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvoiceNew.Models
{

    public class OrderItems
    {
        public int ID { get; set; }
        public string Vendor { get; set; }
        [DisplayName("Invoice No")]
        public string InvoiceNo { get; set; }
        [DisplayName("Invoice Date")]
        public string InvoiceDate { get; set; }
        public string PO { get; set; }

        [DisplayName("PO Date")]
        public string PODate { get; set; }

        [RegularExpression(@"[+]?(\d*[.])?\d+", ErrorMessage = "Please enter correct Price")]
        public string Amount { get; set; }
        public string PAN { get; set; }
        [DisplayName("Company VAT TIN")]
        public string COMPANYVATTIN { get; set; }
        [DisplayName("Company CST No")]
        public string COMPANYCSTNO { get; set; }
        [DisplayName("Buyer VAT TIN")]
        public string BUYERVATTIN { get; set; }
        [DisplayName("Buyer CST No")]
        public string BUYERCSTNO { get; set; }
        [DisplayName("Transport Charge")]
        public string TransportCharge { get; set; }
        public string ImageFilePath { get; set; }


        public List<OrderItemsDetails> ProductDetails { get; set; }
    }



    public class OrderItemsDetails
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Item Name is required")]
        [DisplayName("Item Name")]
        public string ItemName { get; set; }

        [DisplayName("Price")]
        [Required(ErrorMessage = "Price is required")]
        [RegularExpression(@"[+]?(\d*[.])?\d+", ErrorMessage = "Please enter correct Price")]
        public string Price { get; set; }

        [DisplayName("Quantity")]
        [Required(ErrorMessage = "Quantity is required")]
        [RegularExpression(@"[+]?(\d*[.])?\d+", ErrorMessage = "Please enter correct qty")]
        public string Qty { get; set; }

        [DisplayName("Tax%")]
        [RegularExpression(@"[+]?(\d*[.])?\d+", ErrorMessage = "Please enter correct Tax")]
        public string Tax { get; set; }
        public bool Asset { get; set; }

        [DisplayName("Total")]
        [Required(ErrorMessage = "Total Should not be blank")]
        [RegularExpression(@"[+]?(\d*[.])?\d+", ErrorMessage = "Please enter correct Total Price")]
        public string TOTAL { get; set; }

        [Display(Name = "CODES ")]
        //public List<SelectListItem> ItemCodes { get; set; }
        //public int? CodeId { get; set; }
        public string Code { get; set; }
        public int LastIndex { get; set; }

    }

    public class CodeValue
    {
        public int CodeId { get; set; }
        public string Name { get; set; }
    }

    /*
    public class OrderItems
    {
        public int ID { get; set; }
        public string Vendor { get; set; }
        [DisplayName("Invoice No")]
        public string InvoiceNo { get; set; }
        [DisplayName("Invoice Date")]
        public string InvoiceDate { get; set; }
        public string PO { get; set; }

        [DisplayName("PO Date")]
        public string PODate { get; set; }

        [RegularExpression(@"[+]?(\d*[.])?\d+", ErrorMessage = "Please enter correct Price")]
        public string Amount { get; set; }
        public string PAN { get; set; }
        [DisplayName("Company VAT TIN")]
        public string COMPANYVATTIN { get; set; }
        [DisplayName("Company CST No")]
        public string COMPANYCSTNO { get; set; }
        [DisplayName("Buyer VAT TIN")]
        public string BUYERVATTIN { get; set; }
        [DisplayName("Buyer CST No")]
        public string BUYERCSTNO { get; set; }
        [DisplayName("Transport Charge")]
        public string TransportCharge { get; set; }
        public string ImageFilePath { get; set; }

        public string Dateofpayment { get; set; }
        public string InvoiceDueDate { get; set; }

        public List<OrderItemsDetails> ProductDetails { get; set; }
    }



    public class OrderItemsDetails
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Item Name is required")]
        [DisplayName("Item Name")]
        public string ItemName { get; set; }

        [DisplayName("Price")]
        [Required(ErrorMessage = "Price is required")]
        [RegularExpression(@"[+]?(\d*[.])?\d+", ErrorMessage = "Please enter correct Price")]
        public string Price { get; set; }

        [DisplayName("Quantity")]
        [Required(ErrorMessage = "Quantity is required")]
        [RegularExpression(@"[+]?(\d*[.])?\d+", ErrorMessage = "Please enter correct qty")]
        public string Qty { get; set; }

        [DisplayName("Tax%")]
        [RegularExpression(@"[+]?(\d*[.])?\d+", ErrorMessage = "Please enter correct Tax")]
        public string Tax { get; set; }
        public bool Asset { get; set; }

        [DisplayName("Total")]
        [Required(ErrorMessage = "Total Should not be blank")]
        [RegularExpression(@"[+]?(\d*[.])?\d+", ErrorMessage = "Please enter correct Total Price")]
        public string TOTAL { get; set; }

        [Display(Name = "CODES ")]
        public List<SelectListItem> ItemCodes { get; set; }
        public int? CodeId { get; set; }
        public string Code { get; set; }
        public int LastIndex { get; set; }

    }

    public class CodeValue
    {
        public int CodeId { get; set; }
        public string Name { get; set; }
    }*/

}