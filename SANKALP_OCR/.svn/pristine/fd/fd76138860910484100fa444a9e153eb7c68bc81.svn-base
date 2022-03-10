using InvoiceNew.DataAccess;
using InvoiceNew.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Drawing;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;

namespace InvoiceNew.Controllers
{
    public class ReportController : Controller
    {
        static string Exeptionfrom = string.Empty;
        static string constr = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBconnection"].ConnectionString;
        static SqlConnection con = new SqlConnection(constr);
        // GET: Report
        public ActionResult Index()
        {
            ViewBag.DropDown = "Disabled";
            if (GetBranchDropDown().BranchDropDown.Count > 2)
            {
                ViewBag.DropDown = "Enabled";
            }
            return View(GetBranchDropDown());
        }

        public ActionResult GSTReturns()
        {
            ViewBag.DropDown = "Disabled";
            if (GetBranchDropDown().BranchDropDown.Count > 2)
            {
                ViewBag.DropDown = "Enabled";
            }
            return View(GetBranchDropDown());
        }

        public ActionResult Category()
        {
            ViewBag.DropDown = "Disabled";
            if(GetBranchDropDown().BranchDropDown.Count > 2)
            {
                ViewBag.DropDown = "Enabled";
            }
            
            return View(GetBranchDropDown());
        }
        private BranchDrpModel GetBranchDropDown()
        {
            BranchDrpModel smodel = new BranchDrpModel();
            smodel.BranchDropDown = new List<SelectListItem>();
            smodel.BranchID = 0;
            smodel.BranchDropDown.Add(new SelectListItem { Text = "All", Value = "0" });
            List<int> branchList = (List<int>)Session["BranchList"];
            List<BranchSelectModel> bl = new List<BranchSelectModel>();
            con.Open();
            SqlCommand cmd = new SqlCommand("getBranchDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {

                    //smodel.BranchID = int.Parse(sdr["BranchID"].ToString());
                    if(branchList.Exists( x=> x.ToString() == sdr["BranchID"].ToString()))
                        smodel.BranchDropDown.Add(new SelectListItem { Text = sdr["BranchName"].ToString(), Value = sdr["BranchID"].ToString() });
                }
            }
            con.Close();
            return smodel;
        }
        public ActionResult Purchase()
        {
            ViewBag.DropDown = "Disabled";
            if (GetBranchDropDown().BranchDropDown.Count > 2)
            {
                ViewBag.DropDown = "Enabled";
            }
            return View(GetBranchDropDown());
        }
        public ActionResult Journal()
        {
            ViewBag.DropDown = "Disabled";
            if (GetBranchDropDown().BranchDropDown.Count > 2)
            {
                ViewBag.DropDown = "Enabled";
            }
            return View(GetBranchDropDown());
        }


        public ActionResult ExportToExcel(String StartDate,string EndDate, int BranchID)
        {
            InvoiceReport ir = new InvoiceReport();
            DataTable dt = ir.GetInvoiceReports(StartDate, EndDate,BranchID);
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string FileName = "Invoice" + DateTime.Now.ToString("ddMMyyyy") +".xlsx";
          //  Response.AddHeader("content-disposition", "attachment;filename=" + HttpUtility.UrlEncode(FileName, System.Text.Encoding.UTF8));
            byte[] result = null;
            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("ItemWise");
                ws.Column(1).Style.Numberformat.Format = "dd-mmm-yy";
                ws.Column(4).Style.Numberformat.Format = "dd-mmm-yy";
                ws.Column(10).Style.Numberformat.Format = "0.00";
                ws.Column(12).Style.Numberformat.Format = "0.00";
                ws.Column(14).Style.Numberformat.Format = "0.00";
                ws.Column(16).Style.Numberformat.Format = "0.00";
                ws.Column(17).Style.Numberformat.Format = "0.00";
                ws.Cells["A1"].LoadFromDataTable(dt, true);
               
                result = pck.GetAsByteArray();
                // ws = pck.Workbook.Worksheets.Add("OrderItemDetails");
                ////  ws.Cells["A1"].LoadFromDataTable(ds.Tables[1], true);
                //var ms = new System.IO.MemoryStream();
                //pck.SaveAs(ms);
                //ms.WriteTo(Response.OutputStream);
            }
            return File(result, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
        }

        public ActionResult ExportGSTReturns(String StartDate, String EndDate , int BranchID)
        {
            Exeptionfrom = "Report/GSTReturns";
            byte[] result = null;
            string FileName = "GSTReturns_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";
            try
            {
                InvoiceReport ir = new InvoiceReport();
                DataTable dt = ir.GetGSTReturnsReport(StartDate, EndDate,BranchID);
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                
                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("GSTReturns");
                    ws.Column(2).Style.Numberformat.Format = "dd-mmm-yy";
                    ws.Column(3).Style.Numberformat.Format = "dd-mmm-yy";
                    ws.Cells["A1"].LoadFromDataTable(dt, true);

                    result = pck.GetAsByteArray();
                }
            }
            catch (Exception ex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
            }

            return File(result, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
        }

        public ActionResult ExportCategoryInvoiceItems(String StartDate, String EndDate, int BranchID)
        {
            Exeptionfrom = "Report/Category";
            byte[] result = null;
            string FileName = "Category_InvoiceItems_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";
            try
            {
                InvoiceReport ir = new InvoiceReport();
                DataTable dt = ir.GetCategoryInvoiceItemsReport(StartDate, EndDate,BranchID);
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("GSTReturns");
                    ws.Column(2).Style.Numberformat.Format = "dd-mmm-yy";
                    ws.Column(3).Style.Numberformat.Format = "dd-mmm-yy";
                    ws.Cells["A1"].LoadFromDataTable(dt, true);

                    result = pck.GetAsByteArray();
                }
            }
            catch (Exception ex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
            }

            return File(result, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
        }

        public ActionResult ExportPurchaseReport(String StartDate, String EndDate, int BranchID)
        {
            Exeptionfrom = "Report/PurchaseReport";
            byte[] result = null;
            string FileName = "PurchaseRegister_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";
            try
            {
                InvoiceReport ir = new InvoiceReport();
                DataTable dt = ir.PurchaseReport(StartDate, EndDate,BranchID);
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("GSTReturns");
                    if(BranchID != 0) {
                        con.Open(); 
                        SqlCommand cmd = new SqlCommand("getBranchHeaders", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@BranchID",BranchID);
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                ws.Cells["A1"].Value = sdr["BranchName"];
                                ws.Cells["A2"].Value = sdr["BranchAddress1"];
                                ws.Cells["A3"].Value = sdr["BranchAddress2"];
                                ws.Cells["A4"].Value = sdr["BranchAddress3"];
                                ws.Cells["A5"].Value = sdr["GSTIN"];
                            }
                        }
                        con.Close();
                    }
                    else { 
                        ws.Cells["A1"].Value = "Ficus Pax Pvt Ltd";
                        ws.Cells[1, 1].Style.Font.Name = "Arial";
                        ws.Cells[1, 1].Style.Font.Size = 12;
                        ws.Cells[1, 1].Style.Font.Bold = true;

                        ws.Cells["A2"].Value = "All Branches";
                        ws.Cells["A3"].Value = "-";
                        ws.Cells["A4"].Value = "-";
                        ws.Cells["A5"].Value = "GSTIN: 29AAACF5951E1ZN";
                    }
                    for (var row = 2; row <= 7; row++)
                    {
                        ws.Cells[row, 1, row, 3].Merge = true;
                        ws.Cells[row, 1].Style.Font.Name = "Arial";
                        ws.Cells[row, 1].Style.Font.Size = 9;
                    }
                    ws.Cells["A6"].Value = "Purchase Register";
                    ws.Cells[6, 1].Style.Font.Name = "Arial";
                    ws.Cells[6, 1].Style.Font.Size = 12;
                    ws.Cells[6, 1].Style.Font.Bold = true;
                    ws.Cells[6, 1].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                    ws.Cells["A7"].Value = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) 
                                            + " to " + DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture); ;

                    ws.Cells["A8"].LoadFromDataTable(dt, true);
                    ws.Row(8).Height = 36;

                    //Applies border to invoices list and other formatting
                    using (ExcelRange Rng = ws.Cells[8, 1, dt.Rows.Count + 9, dt.Columns.Count])
                    {
                        Rng.Style.Font.Name = "Arial";
                        Rng.Style.Font.Size = 9;
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    ws.Cells[6, 1, 6, 3].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[8, 2].Style.Font.Italic = true;
                    ws.Cells[8, 7].Value = "Vch No.";
                    ws.Cells[8, 1, 8, dt.Columns.Count].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    ws.Cells[8, 1, 8, dt.Columns.Count].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells[8, 1, 8, dt.Columns.Count].Style.WrapText = true;
                    ws.Column(1).Width = 10;
                    ws.Column(2).Width = 40;
                    ws.Column(3).Width = 15;
                    ws.Column(4).Width = 20;
                    ws.Column(5).Width = 11;
                    ws.Column(6).Width = 20;
                    ws.Column(8).Width = 11;

                    //Renames purchase amount column and date format in invoice list table
                    for (int c = 1; c <= ws.Cells[8, 1, 8, dt.Columns.Count].Columns; c++)
                    {
                        if (ws.Cells[8, c].Text.IndexOf("Purchase Accounts") > 0)
                            ws.Cells[8, c].Value = "Purchase Accounts";

                        ws.Column(1).Style.Numberformat.Format = "dd-mm-yyyy";
                        ws.Column(5).Style.Numberformat.Format = "dd-mm-yyyy";
                        ws.Column(2).Style.Font.Bold = true;
                        ws.Column(8).Style.Font.Bold = true;
                    }

                    //Adds total row and sets formulas to columns to calculate totals
                    string col = "";
                    for (int c = 8; c <= dt.Columns.Count; c++)
                    {
                        col = Number2String(c, true);
                        ws.Cells[dt.Rows.Count + 9, c].Formula = "SUM(" + col  + "9:" + col + (dt.Rows.Count + 8) + ")";
                    }
                    ws.Cells[dt.Rows.Count + 9, 2].Value = "Grand Total";
                    ws.Cells[dt.Rows.Count + 9, 2].Style.Font.Bold = false;
                    ws.Cells[dt.Rows.Count + 9, 2].Style.Font.Italic = true;
                    ws.Cells[dt.Rows.Count + 9, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    //ws.Cells[dt.Rows.Count + 9, 2].Value = ((char)66).ToString();

                    ws.View.FreezePanes(9, 1);
                    result = pck.GetAsByteArray();
                }
            }
            catch (Exception ex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
            }

            return File(result, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
        }
        public ActionResult ExportJournalReport(String StartDate, String EndDate, int BranchID)
        {
            Exeptionfrom = "Report/JournalReport";
            byte[] result = null;
            string FileName = "JournalRegister_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";
            try
            {
                InvoiceReport ir = new InvoiceReport();
                DataTable dt = ir.JournalReport(StartDate, EndDate,BranchID);
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("GSTReturns");
                    if (BranchID != 0)
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand("getBranchHeaders", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@BranchID", BranchID);
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                ws.Cells["A1"].Value = sdr["BranchName"];
                                ws.Cells["A2"].Value = sdr["BranchAddress1"];
                                ws.Cells["A3"].Value = sdr["BranchAddress2"];
                                ws.Cells["A4"].Value = sdr["BranchAddress3"];
                                ws.Cells["A5"].Value = sdr["GSTIN"];
                            }
                        }
                        con.Close();
                    }
                    else
                    {
                        ws.Cells["A1"].Value = "Ficus Pax Pvt Ltd";
                        ws.Cells[1, 1].Style.Font.Name = "Arial";
                        ws.Cells[1, 1].Style.Font.Size = 12;
                        ws.Cells[1, 1].Style.Font.Bold = true;

                        ws.Cells["A2"].Value = "All Branches";
                        ws.Cells["A3"].Value = "-";
                        ws.Cells["A4"].Value = "-";
                        ws.Cells["A5"].Value = "GSTIN: 29AAACF5951E1ZN";
                    }

                    for (var row = 2; row <= 7; row++)
                    {
                        ws.Cells[row, 1, row, 3].Merge = true;
                        ws.Cells[row, 1].Style.Font.Name = "Arial";
                        ws.Cells[row, 1].Style.Font.Size = 9;
                    }
                    ws.Cells["A6"].Value = "Journal Register";
                    ws.Cells[6, 1].Style.Font.Name = "Arial";
                    ws.Cells[6, 1].Style.Font.Size = 12;
                    ws.Cells[6, 1].Style.Font.Bold = true;
                    ws.Cells[6, 1].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

                    ws.Cells["A7"].Value = DateTime.ParseExact(StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture)
                                            + " to " + DateTime.ParseExact(EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture); ;

                    ws.Cells["A8"].LoadFromDataTable(dt, true);
                    ws.Row(8).Height = 36;

                    //Applies border to invoices list and other formatting
                    using (ExcelRange Rng = ws.Cells[8, 1, dt.Rows.Count + 9, dt.Columns.Count])
                    {
                        Rng.Style.Font.Name = "Arial";
                        Rng.Style.Font.Size = 9;
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    ws.Cells[6, 1, 6, 3].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[8, 2].Style.Font.Italic = true;
                    ws.Cells[8, 7].Value = "Vch No.";
                    ws.Cells[8, 1, 8, dt.Columns.Count].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    ws.Cells[8, 1, 8, dt.Columns.Count].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells[8, 1, 8, dt.Columns.Count].Style.WrapText = true;
                    ws.Column(1).Width = 10;
                    ws.Column(2).Width = 40;
                    ws.Column(4).Width = 20;
                    ws.Column(5).Width = 11;
                    ws.Column(6).Width = 20;
                    ws.Column(8).Width = 11;

                    //Renames purchase amount column and date format in invoice list table
                    for (int c = 1; c <= ws.Cells[8, 1, 8, dt.Columns.Count].Columns; c++)
                    {
                        if (ws.Cells[8, c].Text.IndexOf("Purchase Accounts") > 0)
                            ws.Cells[8, c].Value = "Purchase Accounts";

                        ws.Column(1).Style.Numberformat.Format = "dd-mm-yyyy";
                        ws.Column(5).Style.Numberformat.Format = "dd-mm-yyyy";
                        ws.Column(2).Style.Font.Bold = true;
                        ws.Column(8).Style.Font.Bold = true;
                    }

                    //Adds total row and sets formulas to columns to calculate totals
                    string col = "";
                    for (int c = 8; c <= dt.Columns.Count; c++)
                    {
                        col = Number2String(c, true);
                        ws.Cells[dt.Rows.Count + 9, c].Formula = "SUM(" + col + "9:" + col + (dt.Rows.Count + 8) + ")";
                    }
                    ws.Cells[dt.Rows.Count + 9, 2].Value = "Grand Total";
                    ws.Cells[dt.Rows.Count + 9, 2].Style.Font.Bold = false;
                    ws.Cells[dt.Rows.Count + 9, 2].Style.Font.Italic = true;
                    ws.Cells[dt.Rows.Count + 9, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    //ws.Cells[dt.Rows.Count + 9, 2].Value = ((char)66).ToString();

                    ws.View.FreezePanes(9, 1);
                    result = pck.GetAsByteArray();
                }
            }
            catch (Exception ex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
            }

            return File(result, System.Net.Mime.MediaTypeNames.Application.Octet, FileName);
        }

        private String Number2String(int number, bool isCaps)
        {
            Char c = (Char)((isCaps ? 65 : 97) + (number - 1));
            return c.ToString();
        }

        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }
}