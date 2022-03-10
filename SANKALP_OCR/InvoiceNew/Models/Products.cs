using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProductExtractTool.BI
{
   
    public class VendorDetails
    {

        public int ID { get; set; }
        public string Vendor { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public string PO { get; set; }

        [RegularExpression(@"[+]?(\d*[.])?\d+", ErrorMessage = "Please enter correct Price")]
        public string Amount { get; set; }
        public string PAN { get; set; }
        public string COMPANYVATTIN  { get; set; }
        public string    COMPANYCSTNO  { get; set; }
        public string    BUYERVATTIN  { get; set; }
        public string BUYERCSTNO { get; set; }
        public string TransportCharge { get; set; }

        public string ImageFilePath { get; set; }


        public List<Products> ProductDetails { get; set; }
    }


    public class Products
    {
        [DisplayName("Item Name")]
        [Required(ErrorMessage = "Item Name is required")]
        public string ItemName { get; set; }

        [DisplayName("Item Price")]
        [Required(ErrorMessage = "Item Price is required")]
        [RegularExpression(@"[+]?(\d*[.])?\d+", ErrorMessage = "Please enter correct Price")]
        //public double Price { get; set; }
        public string Price { get; set; }

        [DisplayName("Quantity")]
        [Required(ErrorMessage = "Quantity is required")]
        [RegularExpression(@"[+]?(\d*[.])?\d+", ErrorMessage = "Please enter correct qty")]
        public string Qty { get; set; }

        [DisplayName("Tax")]
       // [Required(ErrorMessage = "Tax is required")]
        [RegularExpression(@"[+]?(\d*[.])?\d+", ErrorMessage = "Please enter correct Tax")]
        public string Tax { get; set; }


        [DisplayName("Total")]
        [Required(ErrorMessage = "Total Should not be blank")]
        [RegularExpression(@"[+]?(\d*[.])?\d+", ErrorMessage = "Please enter correct Total Price")]
        //public Single TOTAL { get; set; }
        public string TOTAL { get; set; }

        public bool Asset { get; set; }
    }

}