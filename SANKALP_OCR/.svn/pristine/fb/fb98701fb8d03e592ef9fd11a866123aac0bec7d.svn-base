using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvoiceNew.Models
{
    public class PurchaseOrderModel
    {
        public int POID { get; set; }

        [Required(ErrorMessage = "Purchase date is required")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> PODate { get; set; }



        [Remote("CheckPONumber", "PurchaseOrder" ,ErrorMessage = "PO Number already Exist")]
        [Required(ErrorMessage = "Purchase order is required")]
        public string PONumber { get; set; }

        [Required(ErrorMessage = "Purchase amount is required")]
        [RegularExpression(@"^\d+.\d{0,2}$",ErrorMessage="Please enter valid amount" )]
        public decimal POAmount { get; set; }

        public int Createdby { get; set; }
        public Nullable<System.DateTime> Createddate { get; set; }
        public int ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }


        public string POFileName { get; set; }
 
    }

    //public partial class PurchaseOrder
    //{
    //    public int POID { get; set; }
    //    public Nullable<System.DateTime> PODate { get; set; }
    //    public string PONumber { get; set; }
    //    public Nullable<decimal> POAmount { get; set; }
    //    public Nullable<int> Createdby { get; set; }
    //    public Nullable<System.DateTime> Createddate { get; set; }
    //    public Nullable<int> ModifiedBy { get; set; }
    //    public Nullable<System.DateTime> ModifiedDate { get; set; }
    //    public string POFileName { get; set; }
    //}
}