using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvoiceNew.Models
{
    public class InvoiceModel
    {
        public int InvoiceID { get; set; }

        [Required]
        public string GateEntryNumber { get; set; }
        [Required]
        public string VehicleNumber { get; set; }

        [Required]
        //[DisplayFormat(ApplyFormatInEditMode = true,DataFormatString = "{0:dd/MM/yyyy}")]
        public string Date { get; set; }

        [Required]
        public string Time { get; set; }

        [Required]
        [DisplayName("Invoice Number")]
        public string InvoiceNumber { get; set; }
        [Required]
        [RegularExpression("^[1-9]\\d*(\\.\\d+)?$", ErrorMessage = "Must be Numeric")]
        //[RegularExpression("[0-9]+([\\.,][0-9]+)?", ErrorMessage = "Must be Numeric")]
        [DisplayName("Invoice Amount")]
        public string InvoiceAmount { get; set; }
        public string PONumber { get; set; }
        [Required]
        public string GSTIN { get; set; }
        [Required]
        public string PlaceOfSupply { get; set; }
        [Required]
        public bool ReverseCharges { get; set; }

        [Required]
        public string EcommerceGSTIN { get; set; }

        [Required]
        //[MaxLength(12)]
        //[MinLength(1)]
        [RegularExpression("^[0-9]\\d*(\\.\\d+)?$", ErrorMessage = "Must be Numeric")]
        public int TaxPercent { get; set; }

        [Required]
        [RegularExpression("^[0-9]\\d*(\\.\\d+)?$", ErrorMessage = "Must be Numeric")]
        public float TaxableValue { get; set; }

        [Required]
        [RegularExpression("^[0-9]\\d*(\\.\\d+)?$", ErrorMessage = "Must be Numeric")]
        public int CessAmount { get; set; }

        public string PANNumber { get; set; }
        [Required]
        [DisplayName("Vendor Name")]
        public string VendorName { get; set; }
        public int VendorID { get; set; }
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode =true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? InvoiceDate { get; set; }
        [DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true,DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? InvoiceDueDate { get; set; }

        [DisplayName("Invoice Received Date")]
        [DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true,DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? InvoiceReceiveddate { get; set; }
        [DisplayName("Date Of Payment")]
        [DataType(DataType.Date)]
        public DateTime? DateOfPayment { get; set; }

        public DateTime? DateOfAccount { get; set; }

        public string filePath { get; set; }
        [Required(ErrorMessage = "Choose an Action")]
        public string status { get; set; }
        public int VendorRatingID { get; set; }
        
        public int Rating { get; set; }
        public string VendorComment { get; set; }
        public string Comment { get; set; }
        public string Navisionvoucherno { get; set; }
        public int ReAssignID { get; set; }
        public string CurrentUserName { get; set; }
        public string CurrentStatus { get; set; }
        //  public double  AmountToCredit { get; set; }
        [DisplayName("Debit Amount")]
        public double DebitAmount { get; set; }
        public List<UserTypes> UserTypes { get; set; }
        public virtual ICollection<InVoiceFile> InVoiceFiles { get; set; }
        public List<InvoiceItem> InvoiceItems { get; set; }

        public string LedgerName { get; set; }

        

    }

    public class UserTypes
    {
        public int InvoiceID { get; set; }
        public int UserTypeID { get; set; }
        public string UserTypeName { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public IEnumerable<SelectListItem> Users { get; set; }
        public int SelectedUserId { get; set; }
    }
    public class InvoiceItem
    {
        public int Id { get; set; }
        public string ItemId { get; set; }
        public string InvoiceId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Qty { get; set; }
        public string Rate { get; set; }
        public string Amount { get; set; }
        public string IGST { get; set; }
        public string SGST { get; set; }
        public string CGST { get; set; }
        public string Ledgerlist { get; set; }
         
    }
    
}