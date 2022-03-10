using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceNew.Models
{
    public class CreditModel
    {
        public int InvoiceID { get; set; }
        public int CrDrNoteId { get; set; }
        public List<PerticularModel> PerticularList { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string CrDrNoteDate { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string BranchAddress { get; set; }
        public string BGSTIN { get; set; }
        public float Total { get; set; }
        public string TotalWords { get; set; }
        public string DocType { get; set; }
        public List<CreditDebitNoteItem> CreditDebitNoteItems { get; set; }
        public List<CreditDebitNoteTax> CreditDebitNoteTaxes { get; set; }
    }
    public class PerticularModel
    {
        public string Name { get; set; }
        public int Amount { get; set; }
        public int Qty { get; set; }
        public int IGST { get; set; }
        public int CGST { get; set; }
        public int SGST { get; set; }

        public int GRNAmount { get; set; }
        public int GRNQty { get; set; }
        public int GRNIGST { get; set; }
        public int GRNCGST { get; set; }
        public int GRNSGST { get; set; }

    }

    public class CreditDebitNoteItem
    {
        public int Id { get; set; }
        public string NoteItemId { get; set; }
        public string CrDrNoteId { get; set; }
        public string InvoiceId { get; set; }
        public string Name { get; set; }
        public string Qty { get; set; }
        public string Rate { get; set; }
        public float Amount { get; set; }
        public string IGST { get; set; }
        public string SGST { get; set; }
        public string CGST { get; set; }
        public string Price { get; set; }
        public string IAmount { get; set; }
        public string SAmount { get; set; }
        public string CAmount { get; set; }
        public string Total { get; set; }
        public string CrDrNoteDate { get; set; }
    }

    public class CreditDebitNoteTax
    {
        public string TaxType { get; set; }
        public decimal Tax { get; set; }
        public float TaxAmount { get; set; }
    }
}