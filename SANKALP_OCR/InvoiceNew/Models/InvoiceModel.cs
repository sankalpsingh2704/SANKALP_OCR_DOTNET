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

        [Required(ErrorMessage = "Gate entry number is required")]
        [DisplayName("Gate Entry Number")]
        public string GateEntryNumber { get; set; }
        [Required(ErrorMessage = "Vehicle number is required")]
        [DisplayName("Vehicle Number")]
        public string VehicleNumber { get; set; }

        [Required]
        //[DisplayFormat(ApplyFormatInEditMode = true,DataFormatString = "{0:dd/MM/yyyy}")]
        public string Date { get; set; }
        [Required]
        public string ledgerlist { get; set; }
        [Required]
        public string Time { get; set; }

        [Required(ErrorMessage = "Invoice number is required")]
        [DisplayName("Invoice Number")]
        public string InvoiceNumber { get; set; }
        [Required(ErrorMessage = "Invoice amount is required")]
        [RegularExpression("^[1-9]\\d*(\\.\\d+)?$", ErrorMessage = "Must be Numeric")]
        //[RegularExpression("[0-9]+([\\.,][0-9]+)?", ErrorMessage = "Must be Numeric")]
        [DisplayName("Invoice Amount")]
        public string InvoiceAmount { get; set; }
        public string PONumber { get; set; }
        //[Required(ErrorMessage = "GSTIN is required")]
        public string GSTIN { get; set; }
        public string GRNNO { get; set; }
        //[Required]
        //public string PlaceOfSupply { get; set; }
        //[Required]
        //public bool ReverseCharges { get; set; }

        ////[Required]
        //public string EcommerceGSTIN { get; set; }

        //[Required]
        ////[MaxLength(12)]
        ////[MinLength(1)]
        //[RegularExpression("^[0-9]\\d*(\\.\\d+)?$", ErrorMessage = "Must be Numeric")]
        //public int TaxPercent { get; set; }

        //[Required]
        //[RegularExpression("^[0-9]\\d*(\\.\\d+)?$", ErrorMessage = "Must be Numeric")]
        //public float TaxableValue { get; set; }

        //[Required]
        //[RegularExpression("^[0-9]\\d*(\\.\\d+)?$", ErrorMessage = "Must be Numeric")]
        //public int CessAmount { get; set; }

        public string PANNumber { get; set; }
        [Required(ErrorMessage = "Vendor name is required")]
        [DisplayName("Vendor Name")]
        public string VendorName { get; set; }
        public string VendorAddress { get; set; }
        public int VendorID { get; set; }
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Required]
        [DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode =true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? InvoiceDate { get; set; }
        [DataType(DataType.Date)]
        //[DisplayFormat(ApplyFormatInEditMode = true,DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Invoice Due Date")]
        [Required]
        public DateTime? InvoiceDueDate { get; set; }

        [DisplayName("Invoice Received Date")]
        [DataType(DataType.Date)]
        [Required]
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
        public string URating { get; set; }
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
        [Required]
        public string VoucherType { get; set; }

        public IEnumerable<SelectListItem> DocType { get; set; }

        [DisplayName("Document Type")]
        public string SelectedType { get; set; }

        public IEnumerable<SelectListItem> SecurityUsers { get; set; }
        [DisplayName("Checked By")]
        public int SelectedSecurity { get; set; }
        [DisplayName("Checked By")]
        public string SecurityName { get; set; }
        public int branchid { get; set; }
		
        public List<CreditDebitNotes> CreditDebitNotes { get; set; }
        public List<VendorRatings> VendorRatings { get; set; }
		public List<SelectListItem> LocationList { get; set; }
		[Required][DisplayName("Branch Location")]
		public int LocationId { get; set; }


	}
	public class VendorRatings
    {
        public int Ratings { get; set; }
        public string URatings { get; set; }
    }
    public class CreditDebitNotes
    {
        public int InvoiceID { get; set; }
        public int CrDrNoteID { get; set; }
        public string NoteType { get; set; }
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
        public string CategoryId { get; set; }
        public string HSN { get; set; }
        public string UOM { get; set; }
        public string Price { get; set; }
        public string IAmount { get; set; }
        public string SAmount { get; set; }
        public string CAmount { get; set; }
        public string Total { get; set; }
        public string GRN { get; set; }

        public string GRNPrice { get; set; }
        public string GRNQty { get; set; }
        public string GRNAmount { get; set; }
        public string GRNSGST { get; set; }
        public string GRNCGST { get; set; }
        public string GRNIGST { get; set; }
        public string GRNSAmount { get; set; }
        public string GRNCAmount { get; set; }
        public string GRNIAmount { get; set; }
        public string GRNTotal { get; set; }

        public string Ledgerlist { get; set; }
    }
    public class GRNModel
    {
        public string GRNPrice { get; set; }
        public string GRNQty { get; set; }
        public string GRNAmount { get; set; }
        public string GRNSGST { get; set; }
        public string GRNCGST { get; set; }
        public string GRNIGST { get; set; }
        public string GRNSAmount { get; set; }
        public string GRNCAmount { get; set; }
        public string GRNIAmount { get; set; }
        public string GRNTotal { get; set; }

        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string GSTIN { get; set; }
        public string GRNDate { get; set; }
        public string PartNumber { get; set; }
        public string GoodName { get; set; }
        public string HSN { get; set; }
        public string UOM { get; set; }
    }
    public class GRNExtra
    {
        public string SupplierName { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string PONumber { get; set; }
        public string GSTIN { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? InvoiceRecieveDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public float TotalTaxable { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public float TotalCGST { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public float TotalIGST { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public float TotalSGST { get; set; }

        public float TotalQty { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public float TaxTotal { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public float FinalAmount { get; set; }
        public string GRNNO { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime GRNDate { get; set; }
        public string Address { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string BGSTIN { get; set; }
		public string BranchName { get; set; }

    }
    public class GTAXModel
    {
        public float gst { get; set; }
        [DisplayFormat(DataFormatString = "{0:N}")]
        public float gsum { get; set; }
    }
    public class LedgerDrpModel
    {
        public int InvoiceId { get; set; }
        public int UpdateId { get; set; }
        public string LedgerName { get; set; }
    }


    
}