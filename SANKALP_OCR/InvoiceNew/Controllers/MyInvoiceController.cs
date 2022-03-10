using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Globalization;
using System.Data;
using InvoiceNew.Models;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.IO;
using InvoiceNew.DataAccess;
using System.Text.RegularExpressions;
using System.Xml;
using System.Net;

namespace InvoiceNew.Controllers
{
    public class MyInvoiceController : IQinvoiceController
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
        string Exeptionfrom = string.Empty;

        #region Editing
        List<string> imglist = new List<string>();
        [HttpPost]
        public ActionResult NewgetImageLists(dynamic formdata)
        {
            string sessionname = Guid.NewGuid().ToString();
            Session["UserHit"] = sessionname;
            Session["Hit"] = sessionname;

            if (TempData["T"] != null)
            {
                TempData["T"] = TempData["T"] + "," + sessionname;
                TempData.Keep(); // Keep TempData
            }
            else
            {
                TempData["T"] = sessionname;
                TempData.Keep(); // Keep TempData
            }

            int n = Request.Files.Count;
            string path = Server.MapPath("~/Files/Temp/") + sessionname;
            if (n != 0)
            {
                if (!(Directory.Exists(path)))
                {
                    Directory.CreateDirectory(path);
                }
            }

            for (int i = 0; i < n; i++)
            {
                HttpPostedFileBase file = Request.Files[i];
                string src;
                var fileName = Path.GetFileName(file.FileName);
                Session["fileName"] = fileName;
                Session["file"] = fileName;// extract only the fielname
                var ext = Path.GetExtension(fileName.ToLower());

                if (ext == ".pdf")
                {
                    file.SaveAs(Server.MapPath("~/Files/Temp/") + sessionname + "/" + fileName); //File will be saved in application root
                    var filepath = Server.MapPath("~/Files/Temp/") + sessionname + "/" + fileName;
                    src = Server.MapPath(("~/Files/pdf.jpg")) + "~" + filepath;

                }
                else
                {
                    file.SaveAs(Server.MapPath("~/Files/Temp/") + sessionname + "/" + fileName); //File will be saved in application root
                    var filepath = Server.MapPath("~/Files/Temp/") + sessionname + "/" + fileName;
                    src = filepath + "~" + filepath;
                }
                imglist.Add(src);

            }
            return Json(new { imglist }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        // GET: MyInvoice
        public ActionResult Index()
        {
            SqlConnection con = new SqlConnection(connectionString);
            List<InvoiceModel> objList = new List<InvoiceModel>();
            try
            {
                Exeptionfrom = "MyInvoiceController/Index";
                con.Open();
                UserRepository ur = new UserRepository();
                DataTable blist = ur.getBranchList((List<int>)Session["BranchList"]);
                SqlCommand cmd = new SqlCommand("sp_GetAllInvoiceItemsForUser", con);///GetOrderItems
                int userID = Convert.ToInt32(Session["UserID"]);
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.Parameters.AddWithValue("@BranchList",blist);
                cmd.CommandType = CommandType.StoredProcedure;
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(ds);
                ViewBag.GRN = "Disabled";
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    InvoiceModel objinv = new InvoiceModel();
                    objinv.InvoiceID = Convert.ToInt32(dr["InvoiceID"].ToString());
                    objinv.VendorName = dr["VendorName"].ToString();
                    objinv.InvoiceNumber = dr["InvoiceNumber"].ToString();
                    objinv.PONumber = dr["PONumber"].ToString();
                    objinv.InvoiceAmount = dr["InvoiceAmount"].ToString();
                    objinv.PANNumber = dr["PANNumber"].ToString();
                    objinv.GateEntryNumber = dr["GateEntryNumber"].ToString();
                    objinv.SelectedType = dr["DocType"].ToString();
                    objinv.GRNNO = dr["GRNNO"].ToString();
                    //objinv.SelectedType = dr["DocType"].ToString();

                    if (int.Parse(Regex.Replace(objinv.GRNNO, "[^0-9.]", "")) != 0)
                    {
                        ViewBag.GRN = "Enabled";
                    }
                    else
                    {
                        objinv.GRNNO = "";
                    }
                    //if(int.Parse(Regex.Replace(objinv.GateEntryNumber, "[^0-9.]", "")) != 0)
                    //{
                    //    ViewBag.GRN = "Enabled";
                    //}
                    if (dr["InvoiceDate"] != DBNull.Value)
                        objinv.InvoiceDate = Convert.ToDateTime(dr["InvoiceDate"]).Date;
                    else
                        objinv.InvoiceDate = null;
                    if (dr["InvoiceDueDate"] != DBNull.Value)
                        objinv.InvoiceDueDate = Convert.ToDateTime(dr["InvoiceDueDate"]).Date;
                    else
                        objinv.InvoiceDueDate = null;
                    if (dr["InvoiceReceiveddate"] != DBNull.Value)
                        objinv.InvoiceReceiveddate = Convert.ToDateTime(dr["InvoiceReceiveddate"]).Date;
                    if (dr["Dateofpayment"] != DBNull.Value)
                        objinv.DateOfPayment = Convert.ToDateTime(dr["Dateofpayment"]).Date;
                    if (dr["DateofAccount"] != DBNull.Value)
                        objinv.DateOfAccount = Convert.ToDateTime(dr["DateofAccount"]).Date;

                    if (dr["UserName"] != DBNull.Value)
                        objinv.CurrentUserName = (dr["UserName"]).ToString();
                    objinv.CurrentStatus = "Initial Stage";
                    if (dr["CurrentStatus"] != DBNull.Value)
                    {
                        string curSt = (dr["CurrentStatus"]).ToString();
                        if (curSt.ToUpper() == "A")
                            objinv.CurrentStatus = "Approved";
                        else if (curSt.ToUpper() == "R")
                            objinv.CurrentStatus = "Rejected";
                        else if (curSt.ToUpper() == "REASSIGN")
                            objinv.CurrentStatus = "Re Assign";
                        else if (curSt.ToUpper() == "On Hold")
                            objinv.CurrentStatus = "On Hold";
                        else
                            objinv.CurrentStatus = dr["CurrentStatus"].ToString();
                    }

                    if(objinv.CurrentStatus == "Pending For Payment")
                    {
                        //Session["UserID"];
                        cmd = new SqlCommand("sp_getPaymentBox", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@InvoiceId",objinv.InvoiceID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var userId = reader["UserId"].ToString();
                                if(userId == Session["UserID"].ToString())
                                {
                                    objinv.status = "display";
                                }
                                else
                                {
                                    objinv.status = "none";
                                }
                            }
                        }
                    }
                    objList.Add(objinv);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            return View(objList);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceno"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult getCreditDetailss(string invoiceno)
        {
            JsonResult resp = Json(new { success = 0, creditlist = "" }, JsonRequestBehavior.AllowGet);

            InvoiceRepository invoice = new InvoiceRepository();

            try
            {
                int invoiceid = Convert.ToInt32(invoiceno);
                var creditlist = invoice.getdetailscredit(int.Parse(invoiceno)).ToList();
                //var commentlist = invEF.Audits.Where(a => a.InvoiceID == invoiceno).Select(a => new { a.CreatedBy, a.AssignedTo, a.Comment.Comment1 });
                resp = Json(new { success = 1, creditlist }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { Logger.Write(ex.Message); resp = Json(new { success = 0, commentlist = "" }, JsonRequestBehavior.AllowGet); }
            Session["InvoiceID"] = null;
            return resp;
        }
        /// <summary>
        /// get Details of an Invoice for Details View
        /// </summary>
        /// <param name="id">InvoiceID</param>
        /// <returns></returns>
        public ActionResult MyInvoiceView(int id)
        {
            SqlConnection con = new SqlConnection(connectionString);
            InvoiceModel model = new InvoiceModel();
            try
            {
                Exeptionfrom = "MyInvoiceController/MyInvoiceView";

                con.Open();
                SqlCommand cmd = new SqlCommand("sp_getInvoiceDetails_Model", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@InvoiceId", id);
                TempData["InvoiceId"] = id;
                List<UserTypes> objUsrList = new List<UserTypes>();
                Session["FilePath"] = string.Empty;
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        model.InvoiceID = Convert.ToInt32(sdr["InvoiceID"]);
                        model.InvoiceNumber = sdr["InvoiceNumber"].ToString();
                        model.InvoiceAmount = sdr["InvoiceAmount"].ToString();
                        model.PONumber = sdr["PONumber"].ToString();
                        model.PANNumber = sdr["PANNumber"].ToString();
                        model.VendorName = sdr["VendorName"].ToString();
                        
                        if (sdr["InvoiceDate"] != DBNull.Value)
                            model.InvoiceDate = Convert.ToDateTime(sdr["InvoiceDate"]).Date;//.ToString("dd-MMM-yyyy");
                        if (sdr["InvoiceDueDate"] != DBNull.Value)
                            model.InvoiceDueDate = Convert.ToDateTime(sdr["InvoiceDueDate"]).Date;
                        if (sdr["InvoiceReceiveddate"] != DBNull.Value)
                            model.InvoiceReceiveddate = Convert.ToDateTime(sdr["InvoiceReceiveddate"]).Date;
                        if (sdr["Dateofpayment"] != DBNull.Value)
                            model.DateOfPayment = Convert.ToDateTime(sdr["Dateofpayment"]).Date;
                        if (sdr["DateofAccount"] != DBNull.Value)
                            model.DateOfAccount = Convert.ToDateTime(sdr["DateofAccount"]).Date;
                        if (sdr["FilePath"] != DBNull.Value)
                        {
                            model.filePath = sdr["FilePath"].ToString();
                            Session["FilePath"] = model.filePath;
                        }
                        
                        //if (sdr["PlaceOfSupply"] != DBNull.Value)
                        //    model.PlaceOfSupply = (sdr["PlaceOfSupply"]).ToString();
                       if (sdr["GSTIN"] != DBNull.Value)
                               model.GSTIN = (sdr["GSTIN"]).ToString();
                        //if (sdr["EcommerceGSTIN"] != DBNull.Value)
                        //    model.EcommerceGSTIN = (sdr["EcommerceGSTIN"]).ToString();
                        //if (sdr["TaxableValue"] != DBNull.Value)
                        //    model.TaxableValue = float.Parse(sdr["TaxableValue"].ToString());
                        //if (sdr["TaxPercent"] != DBNull.Value)
                        //    model.TaxPercent = int.Parse(sdr["TaxPercent"].ToString());
                        //if (sdr["CessAmount"] != DBNull.Value)
                        //    model.CessAmount = int.Parse(sdr["CessAmount"].ToString());
                        if (sdr["GateEntryNumber"] != DBNull.Value)
                            model.GateEntryNumber = sdr["GateEntryNumber"].ToString();
                        if (sdr["DocType"] != DBNull.Value)
                            model.SelectedType = sdr["DocType"].ToString();
                        if (sdr["VehicleNumber"] != DBNull.Value)
                            model.VehicleNumber = sdr["VehicleNumber"].ToString();
                        if (sdr["VehicleDate"] != DBNull.Value)
                            model.Date = sdr["VehicleDate"].ToString();
                        if (sdr["VehicleTime"] != DBNull.Value)
                            model.Time = sdr["VehicleTime"].ToString();
                        /*if(sdr["UserName"] != DBNull.Value)
                            model.CurrentUserName = (sdr["UserName"]).ToString();*/
                    }
                    if (sdr.NextResult())
                    {
                        while (sdr.Read())
                        {
                            UserTypes usr = new UserTypes();
                            usr.UserTypeID = Convert.ToInt32(sdr["UserTypeID"]);
                            usr.UserTypeName = sdr["UserTypeName"].ToString();
                            if (sdr["UserID"] != DBNull.Value)
                            {
                                usr.UserID = Convert.ToInt32(sdr["UserID"]);
                                usr.UserName = sdr["UserName"].ToString();
                            }

                            objUsrList.Add(usr);
                        }
                    }
                    con.Close();

                    cmd = new SqlCommand("sp_getcurrentuser", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@InvoiceID", id);
                    con.Open();
                    using (SqlDataReader sdread = cmd.ExecuteReader())
                    {
                        while (sdread.Read())
                        {
                            model.CurrentUserName = sdread["UserName"]?.ToString()??"";
                        }
                        
                    }

                    con.Close();
                    
                    //model.UserTypes = objUsrList;
                }
                model.InvoiceItems = new List<InvoiceItem>();
                con.Open();
                cmd = new SqlCommand("sp_getInvoiceItemsList", con);
                cmd.Parameters.AddWithValue("@InvoiceId", id);
                cmd.Parameters.AddWithValue("@ViewState", "View");


                cmd.CommandType = CommandType.StoredProcedure;


                using (SqlDataReader sdr = cmd.ExecuteReader())
                {

                    while (sdr.Read())
                    {
                        InvoiceItem envitems = new InvoiceItem();
                        envitems.Id = int.Parse(sdr["Id"]?.ToString() ?? "");
                        envitems.InvoiceId = sdr["InvoiceId"]?.ToString() ?? "";
                        //envitems.ItemId = sdr["ItemId"].ToString();
                        //envitems.Code = sdr["Code"].ToString();

                        envitems.Name = sdr["itemName"]?.ToString() ?? "";

                        envitems.Qty = sdr["Qty"]?.ToString() ?? "";
                        envitems.Qty = Regex.Replace(envitems.Qty, "[^0-9.]", "");
                        //envitems.Rate = sdr["Rate"].ToString();
                        envitems.Amount = sdr["Amount"]?.ToString() ?? "";
                        envitems.UOM = sdr["UOM"]?.ToString() ?? "";
                        envitems.HSN = sdr["HSN"]?.ToString() ?? "";
                        envitems.Price = sdr["Price"]?.ToString() ?? "";
                        envitems.SGST = sdr["SGST"]?.ToString() ?? "";
                        envitems.IGST = sdr["IGST"]?.ToString() ?? "";
                        envitems.CGST = sdr["CGST"]?.ToString() ?? "";
                        envitems.CAmount = sdr["CGSTAmount"]?.ToString() ?? "";
                        envitems.SAmount = sdr["SGSTAmount"]?.ToString() ?? "";
                        envitems.IAmount = sdr["IGSTAmount"]?.ToString() ?? "";
                        envitems.Total = sdr["Total"]?.ToString() ?? "";

                        envitems.CategoryId = sdr["Name"]?.ToString() ?? "";
                        envitems.GRN = sdr["GRN"]?.ToString();


                        //envitems.Ledgerlist = sdr["ledgervalue"].ToString();

                        model.InvoiceItems.Add(envitems);
                    }

                }
                con.Close();


                //////////
                List<SelectListItem> items = new List<SelectListItem>();
                //string query = "select * FROM Users USR INNER JOIN UserTypes UT ON UT.UserTypeID = USR.UserTypeID AND UT.Disabled = 0 AND Usr.UserID <> 3";/// "select UserID,UserName from dbo.[Users] WHERE Disabled =0";
                con.Open();
                SqlCommand comm = new SqlCommand("sp_getUsers", con);
                cmd.CommandType = CommandType.StoredProcedure;

                comm.Connection = con;
                using (SqlDataReader sdr = comm.ExecuteReader())
                {
                    while (sdr.Read())
                    {

                        items.Add(new SelectListItem
                        {
                            Text = sdr["UserName"].ToString(),
                            Value = sdr["UserID"].ToString()
                        });
                    }
                }

                con.Close();
                ViewBag.ReAssignUsers = items;/// TempData["ReAssignUsers"];

                con.Open();
                SqlCommand cmdNotes = new SqlCommand("usp_getCreditDebitNotes", con);
                cmdNotes.CommandType = CommandType.StoredProcedure;
                cmdNotes.Parameters.AddWithValue("@InvoiceId", model.InvoiceID);

                List<CreditDebitNotes> objNotesList = new List<CreditDebitNotes>();
                using (SqlDataReader sdr = cmdNotes.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        CreditDebitNotes crDrNotes = new CreditDebitNotes();
                        crDrNotes.CrDrNoteID = Convert.ToInt32(sdr["CrDrNoteID"]);
                        crDrNotes.InvoiceID = Convert.ToInt32(sdr["InvoiceID"]);
                        crDrNotes.NoteType = sdr["NoteType"].ToString();
                        objNotesList.Add(crDrNotes);

                    }
                    model.CreditDebitNotes = objNotesList;
                }
                con.Close();
            }
            catch (Exception ex)
            {
               Logger.Write(Exeptionfrom);
               Logger.Write(ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
            return View(model);
        }

        public ActionResult CreditView(int Id)
        {
            SqlConnection con = new SqlConnection(connectionString);
            CreditModel model = new CreditModel();

            try
            {
                model.CreditDebitNoteItems = new List<CreditDebitNoteItem>();
                model.CreditDebitNoteTaxes = new List<CreditDebitNoteTax>();
                model.CrDrNoteId = Id;
                con.Open();

                SqlCommand cmd = new SqlCommand("sp_getCrDrNoteItemsList", con);
                cmd.Parameters.AddWithValue("@CrDrNoteId", Id);
                cmd.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader sdr = cmd.ExecuteReader())
                {

                    while (sdr.Read())
                    {
                        CreditDebitNoteItem noteitem = new CreditDebitNoteItem();
                        noteitem.Name = sdr["ItemName"].ToString();

                        model.InvoiceID = Int32.Parse(sdr["InvoiceId"].ToString());
                        model.CrDrNoteDate = String.Format("{0:d-MMM-yyyy}", Convert.ToDateTime(sdr["Created"].ToString()));
                        model.DocType = sdr["DocType"].ToString();
                        noteitem.InvoiceId = sdr["InvoiceId"].ToString();
                        noteitem.NoteItemId = sdr["CrDrNoteItemId"].ToString();
                        noteitem.CrDrNoteId = Id.ToString();
                        noteitem.Name = sdr["ItemName"].ToString();
                        noteitem.Qty = sdr["Qty"].ToString();
                        noteitem.Rate = sdr["Rate"].ToString();
                        noteitem.Amount = Truncate(float.Parse(sdr["Amount"].ToString()), 2);
                        noteitem.IGST = sdr["IGST"].ToString();
                        noteitem.SGST = sdr["SGST"].ToString();
                        noteitem.CGST = sdr["CGST"].ToString();
                        //noteitem.Price = sdr["Price"].ToString();
                        noteitem.IAmount = sdr["IGSTAmount"].ToString();
                        noteitem.SAmount = sdr["SGSTAmount"].ToString();
                        noteitem.CAmount = sdr["CGSTAmount"].ToString();
                        noteitem.Total = sdr["CrDrNoteItemId"].ToString();
                        //noteitem.CrDrNoteDate = String.Format("{0:d-MMM-yyyy}", Convert.ToDateTime(sdr["Created"].ToString()));

                        model.CreditDebitNoteItems.Add(noteitem);
                    }
                    if (sdr.NextResult())
                    {
                        while (sdr.Read())
                        {
                            CreditDebitNoteTax notetaxitem = new Models.CreditDebitNoteTax();
                            notetaxitem.TaxType = sdr["TaxType"].ToString();
                            notetaxitem.Tax = Int32.Parse(sdr["Tax"].ToString());
                            notetaxitem.TaxAmount = Truncate(float.Parse(sdr["TaxAmount"].ToString()), 2);

                            model.CreditDebitNoteTaxes.Add(notetaxitem);
                        }
                    }
                    model.Total = 0;
                    foreach (var item in model.CreditDebitNoteItems)
                    {
                        model.Total = model.Total + item.Amount;
                    }

                    foreach (var taxitem in model.CreditDebitNoteTaxes)
                    {
                        model.Total = model.Total + float.Parse(taxitem.TaxAmount.ToString());
                    }
                    float TotalF = Truncate(model.Total, 0);
                    model.Total = Truncate(model.Total, 2);
                    model.TotalWords = ConvertNumbertoWords(long.Parse(TotalF.ToString()));
                }

                cmd = new SqlCommand("sp_getVendorRelInfo", con);
                cmd.Parameters.AddWithValue("@InvoiceId", model.InvoiceID);
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        model.VendorName = sdr["VendorName"].ToString();
                        model.Phone = sdr["BranchPhone"].ToString();
                        model.Email = sdr["BranchEmail"].ToString();
                        model.BranchAddress = sdr["BranchAddress1"].ToString() + "," + sdr["BranchAddress2"] + "," + sdr["BranchAddress3"];
                        model.BGSTIN = sdr["BGSTIN"].ToString();
                        model.InvoiceDate = String.Format("{0:d-MMM-yyyy}", Convert.ToDateTime(sdr["InvoiceDate"].ToString()));
                        model.InvoiceNumber = sdr["InvoiceNumber"].ToString();
                    }
                }
                con.Close();
            }
            catch (Exception ex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
            return View(model);
        }

        public static float Truncate(float value, int digits)
        {
            double mult = Math.Pow(10.0, digits);
            double result = Math.Truncate(mult * value) / mult;
            return (float)result;
        }

        public string ConvertNumbertoWords(long number)
        {
            if (number == 0) return "ZERO";
            if (number < 0) return "minus " + ConvertNumbertoWords(Math.Abs(number));
            string words = "";
            if ((number / 1000000) > 0)
            {
                words += ConvertNumbertoWords(number / 100000) + " Lakhs ";
                number %= 1000000;
            }
            if ((number / 1000) > 0)
            {
                words += ConvertNumbertoWords(number / 1000) + " Thousand ";
                number %= 1000;
            }
            if ((number / 100) > 0)
            {
                words += ConvertNumbertoWords(number / 100) + " Hundred ";
                number %= 100;
            }
            //if ((number / 10) > 0)  
            //{  
            // words += ConvertNumbertoWords(number / 10) + " RUPEES ";  
            // number %= 10;  
            //}  
            if (number > 0)
            {
                if (words != "") words += "AND ";
                var unitsMap = new[]
                {
                    "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN"
                };
                var tensMap = new[]
                {
                    "ZERO", "TEN", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY"
                };
                if (number < 20) words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0) words += " " + unitsMap[number % 10];
                }
            }
            return words;
        }

        /// <summary>
        /// Save the payment Date
        /// </summary>
        /// <param name="ID">InvoiceID</param>
        /// <param name="date">payment date</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SavePaymentDate(List<int> list,string tall)
        {//string ID, string date
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                con.Open();
				DataTable dt = new DataTable();
				dt.Columns.Add("InvoiceId");
                foreach (var item in list)
                {
                    Exeptionfrom = "MyInvoiceController/SavePaymentDate";
					/*
				  DateTime? pdate = null;

				  //pdate = DateTime.ParseExact(item.Date, "dd/MM/yyyy", null);
				 // if (DateTime.TryParse(item.Date, out dateValue))
				//  {
					  //pdate = dateValue;
					  //int invId = Convert.ToInt32(ID);
					  int invId = Convert.ToInt32(item.Id);
					  //  string query = "UPDATE InVoice set Dateofpayment ='" + pdate.ToString() + "', CurrentStatus = 'Paid' from InVoice where InvoiceID = " + invId;

					  SqlCommand comm = new SqlCommand("sp_updateDateofpayment", con);
					  comm.CommandType = CommandType.StoredProcedure;
					  comm.Parameters.AddWithValue("@Dateofpayment", pdate);
					  comm.Parameters.AddWithValue("@InvoiceID", invId);
					  comm.ExecuteNonQuery();
				  //}			*/
					dt.Rows.Add(item);
					
				}
				SqlCommand comm = new SqlCommand("getInvoicePaymentDetails", con);
				comm.CommandType = CommandType.StoredProcedure;
				comm.Parameters.AddWithValue("@InvoiceIds", dt);
				List<PaymentModel> payList = new List<PaymentModel>();
				using (SqlDataReader sdr = comm.ExecuteReader())
				{
					while (sdr.Read())
					{
						payList.Add(new PaymentModel() { InvoiceNumber =  sdr["InvoiceNumber"].ToString(),
                            VendorName = sdr["VendorName"].ToString(),
                            InvoiceAmount = sdr["InvoiceAmount"].ToString(),
                            Days = sdr["Days"].ToString(),
                            InvoiceDueDate= sdr["InvoiceDueDate"].ToString(),
                        });
					}
				}

                var res = AddtopaymentTally(payList, tall);
				con.Close();
				if (res != "")
					return Json(new { res });
				else
					return Json(new { res = true});
			}
            catch (Exception ex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
				return Json(new { res = ex.Message });
			}
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
            return Json(new { success = 1 });
        }

        public string AddtopaymentTally(List<PaymentModel> payList,string comment)
        {

            var payVendorList = payList.GroupBy(x => x.VendorName).Select(g=> new { VendorName = g.Key, GrandTotal = g.Sum(Item => Decimal.Parse(Item.InvoiceAmount)) }).ToList();

            foreach(var item in payVendorList)
            { 
                XmlDocument xdoc = new XmlDocument();
           
                xdoc.Load(Server.MapPath("~/tallyxml/PaymentInvoiceVoucher.xml"));
                XmlNode root = xdoc.DocumentElement;
                XmlNode parentNode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER");


                //VucherTypeName.InnerText = "JN-" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(LocList[0]);
                XmlNode VucherNumberNode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/DATE");
                VucherNumberNode.InnerText = "20180501";

                XmlNode EFFECTIVEDATENode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/EFFECTIVEDATE");
                EFFECTIVEDATENode.InnerText = "20180501";

                //EFFECTIVEDATENode.InnerText = DateTime.Now.ToString("yyyymmdd");
                XmlNode NARRATIONNode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/NARRATION");
                NARRATIONNode.InnerText = comment;

                XmlNode VOUCHERNUMBERNode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/INVOICEORDERLIST.LIST/BASICORDERDATE");
                VOUCHERNUMBERNode.InnerText = "20180501";


                //string monthname = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(pm.Month) + " " + pm.Year.ToString().Substring(2, 2);
                XmlNode PARTYLEDGERNAMENode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/PARTYLEDGERNAME");
                PARTYLEDGERNAMENode.InnerText = item.VendorName ;

          
                 XmlNodeList ALLLEDGERENTRIESLIST = xdoc.SelectNodes("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/ALLLEDGERENTRIES.LIST");


            Dictionary<string, decimal> banklistTotal = new Dictionary<string, decimal>();
            decimal GrandTotal = 0;

      
                 
                     
               
            
            


            XmlDocumentFragment xfrag = xdoc.CreateDocumentFragment();







           
               
                  xfrag = xdoc.CreateDocumentFragment();

                //  
                string part1 = @"<ALLLEDGERENTRIES.LIST>" +
"<OLDAUDITENTRYIDS.LIST TYPE=\"Number\">" +
"<OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>" +
"</OLDAUDITENTRYIDS.LIST>" +
"<LEDGERNAME>" + item.VendorName + "</LEDGERNAME>" +
"<GSTCLASS/>" +
"<ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>" +
"<LEDGERFROMITEM>No</LEDGERFROMITEM>" +
"<REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>" +
"<ISPARTYLEDGER>Yes</ISPARTYLEDGER>" +
"<ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE>" +
"<AMOUNT>- " + item.GrandTotal + "</AMOUNT>" +
"<VATEXPAMOUNT>- " + item.GrandTotal + "</VATEXPAMOUNT>" +
"<SERVICETAXDETAILS.LIST></SERVICETAXDETAILS.LIST>" +
"<CATEGORYALLOCATIONS.LIST>" +
"<CATEGORY>Service</CATEGORY>" +
"<ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>" +
"<COSTCENTREALLOCATIONS.LIST>" +
"<NAME>Ambattur Unit I-Service</NAME>" +
"<AMOUNT>-" + item.GrandTotal + "</AMOUNT>" +
"</COSTCENTREALLOCATIONS.LIST>" +
"</CATEGORYALLOCATIONS.LIST>" +
"<BANKALLOCATIONS.LIST></BANKALLOCATIONS.LIST>";
                string part2 = "";
foreach (var vendorm in payList)
                {

                    if (vendorm.VendorName == item.VendorName)
                    {
                        part2 += "<BILLALLOCATIONS.LIST>" +
                        "<NAME>"+ vendorm.InvoiceNumber +"</NAME>" +
                        "<BILLCREDITPERIOD  P=\""+ vendorm.Days+" Days\">" + vendorm.Days+ " Days</BILLCREDITPERIOD>" +
                        "<BILLTYPE>Agst Ref</BILLTYPE>" +
                        "<TDSDEDUCTEEISSPECIALRATE>No</TDSDEDUCTEEISSPECIALRATE>" +
                        "<AMOUNT>-"+vendorm.InvoiceAmount +"</AMOUNT>" +
                        "<INTERESTCOLLECTION.LIST></INTERESTCOLLECTION.LIST>" +
                        "<STBILLCATEGORIES.LIST></STBILLCATEGORIES.LIST>" +
                        "</BILLALLOCATIONS.LIST>";
                    }

}


                string part3 = @"<INTERESTCOLLECTION.LIST></INTERESTCOLLECTION.LIST>" +
"<OLDAUDITENTRIES.LIST></OLDAUDITENTRIES.LIST>" +
"<ACCOUNTAUDITENTRIES.LIST></ACCOUNTAUDITENTRIES.LIST>" +
"<AUDITENTRIES.LIST></AUDITENTRIES.LIST>" +
"<INPUTCRALLOCS.LIST></INPUTCRALLOCS.LIST>" +
"<DUTYHEADDETAILS.LIST></DUTYHEADDETAILS.LIST>" +
"<EXCISEDUTYHEADDETAILS.LIST></EXCISEDUTYHEADDETAILS.LIST>" +
"<RATEDETAILS.LIST></RATEDETAILS.LIST>" +
"<SUMMARYALLOCS.LIST></SUMMARYALLOCS.LIST>" +
"<STPYMTDETAILS.LIST></STPYMTDETAILS.LIST>" +
"<EXCISEPAYMENTALLOCATIONS.LIST></EXCISEPAYMENTALLOCATIONS.LIST>" +
"<TAXBILLALLOCATIONS.LIST></TAXBILLALLOCATIONS.LIST>" +
"<TAXOBJECTALLOCATIONS.LIST></TAXOBJECTALLOCATIONS.LIST>" +
"<TDSEXPENSEALLOCATIONS.LIST></TDSEXPENSEALLOCATIONS.LIST>" +
"<VATSTATUTORYDETAILS.LIST></VATSTATUTORYDETAILS.LIST>" +
"<COSTTRACKALLOCATIONS.LIST></COSTTRACKALLOCATIONS.LIST>" +
"<REFVOUCHERDETAILS.LIST></REFVOUCHERDETAILS.LIST>" +
"<INVOICEWISEDETAILS.LIST></INVOICEWISEDETAILS.LIST>" +
"<VATITCDETAILS.LIST></VATITCDETAILS.LIST>" +
"<ADVANCETAXDETAILS.LIST></ADVANCETAXDETAILS.LIST>" +
"</ALLLEDGERENTRIES.LIST>";


                xfrag.InnerXml = part1 + part2 + part3;

                parentNode.InsertAfter(xfrag, parentNode.LastChild);


     

 


                xfrag.InnerXml = @"<ALLLEDGERENTRIES.LIST>" +
"<OLDAUDITENTRYIDS.LIST TYPE=\"Number\">" +
"<OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>" +
"</OLDAUDITENTRYIDS.LIST>" +
"<LEDGERNAME>Kotak Mahindra Bank(425044005505)-AMB CC</LEDGERNAME>" +
"<GSTCLASS/>" +
"<ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>" +
"<LEDGERFROMITEM>No</LEDGERFROMITEM>" +
"<REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>" +
"<ISPARTYLEDGER>Yes</ISPARTYLEDGER>" +
"<ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE>" +
"<AMOUNT>"+item.GrandTotal +"</AMOUNT>" +
"<VATEXPAMOUNT>" + item.GrandTotal + "</VATEXPAMOUNT>" +
"<SERVICETAXDETAILS.LIST></SERVICETAXDETAILS.LIST>" +
"<CATEGORYALLOCATIONS.LIST>" +
"<CATEGORY>Service</CATEGORY>" +
"<ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>" +
"<COSTCENTREALLOCATIONS.LIST>" +
"<NAME>Ambattur Unit I-Service</NAME>" +
"<AMOUNT>" + item.GrandTotal + "</AMOUNT>" +
"</COSTCENTREALLOCATIONS.LIST>" +
"</CATEGORYALLOCATIONS.LIST>" +
//"<BANKALLOCATIONS.LIST>" +
//"<DATE>20180201</DATE>" +
//"<INSTRUMENTDATE>20180201</INSTRUMENTDATE>" +
//"<NAME></NAME>" +
//"<TRANSACTIONTYPE></TRANSACTIONTYPE>" +
//"<PAYMENTFAVOURING>" + item.VendorName + "</PAYMENTFAVOURING>" +
//"<CHEQUECROSSCOMMENT></CHEQUECROSSCOMMENT>" +
//"<INSTRUMENTNUMBER></INSTRUMENTNUMBER>" +
//"<UNIQUEREFERENCENUMBER></UNIQUEREFERENCENUMBER>" +
//"<STATUS>No</STATUS>" +
//"<PAYMENTMODE></PAYMENTMODE>" +
//"<BANKPARTYNAME>" + item.VendorName + "</BANKPARTYNAME>" +
//"<ISCONNECTEDPAYMENT>No</ISCONNECTEDPAYMENT>" +
//"<ISSPLIT>No</ISSPLIT>" +
//"<ISCONTRACTUSED>No</ISCONTRACTUSED>" +
//"<ISACCEPTEDWITHWARNING>No</ISACCEPTEDWITHWARNING>" +
//"<ISTRANSFORCED>No</ISTRANSFORCED>" +
//"<AMOUNT>" + item.GrandTotal + "</AMOUNT>" +
//"<CONTRACTDETAILS.LIST></CONTRACTDETAILS.LIST>" +
//"<BANKSTATUSINFO.LIST></BANKSTATUSINFO.LIST>" +
//"</BANKALLOCATIONS.LIST>" + 
"<BILLALLOCATIONS.LIST></BILLALLOCATIONS.LIST>" +
"<INTERESTCOLLECTION.LIST></INTERESTCOLLECTION.LIST>" +
"<OLDAUDITENTRIES.LIST></OLDAUDITENTRIES.LIST>" +
"<ACCOUNTAUDITENTRIES.LIST></ACCOUNTAUDITENTRIES.LIST>" +
"<AUDITENTRIES.LIST></AUDITENTRIES.LIST>" +
"<INPUTCRALLOCS.LIST></INPUTCRALLOCS.LIST>" +
"<DUTYHEADDETAILS.LIST></DUTYHEADDETAILS.LIST>" +
"<EXCISEDUTYHEADDETAILS.LIST></EXCISEDUTYHEADDETAILS.LIST>" +
"<RATEDETAILS.LIST></RATEDETAILS.LIST>" +
"<SUMMARYALLOCS.LIST></SUMMARYALLOCS.LIST>" +
"<STPYMTDETAILS.LIST></STPYMTDETAILS.LIST>" +
"<EXCISEPAYMENTALLOCATIONS.LIST></EXCISEPAYMENTALLOCATIONS.LIST>" +
"<TAXBILLALLOCATIONS.LIST></TAXBILLALLOCATIONS.LIST>" +
"<TAXOBJECTALLOCATIONS.LIST></TAXOBJECTALLOCATIONS.LIST>" +
"<TDSEXPENSEALLOCATIONS.LIST></TDSEXPENSEALLOCATIONS.LIST>" +
"<VATSTATUTORYDETAILS.LIST></VATSTATUTORYDETAILS.LIST>" +
"<COSTTRACKALLOCATIONS.LIST></COSTTRACKALLOCATIONS.LIST>" +
"<REFVOUCHERDETAILS.LIST></REFVOUCHERDETAILS.LIST>" +
"<INVOICEWISEDETAILS.LIST></INVOICEWISEDETAILS.LIST>" +
"<VATITCDETAILS.LIST></VATITCDETAILS.LIST>" +
"<ADVANCETAXDETAILS.LIST></ADVANCETAXDETAILS.LIST>" +
"</ALLLEDGERENTRIES.LIST>";
                parentNode.InsertAfter(xfrag, parentNode.LastChild);

 
                string m = xdoc.InnerXml.ToString();

            string result1 = "";

            string tallyurls = System.Web.Configuration.WebConfigurationManager.AppSettings["TallyURL"].ToString();
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(tallyurls);

            httpWebRequest.Method = "POST";
            httpWebRequest.ContentLength = m.Length;
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            streamWriter.Write(m);
            streamWriter.Close();

            HttpWebResponse objResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                result1 = sr.ReadToEnd();

                //Logger.Write("InvoiceID : " + model.InvoiceID);
                //Logger.Write("InvoiceNumber : " + model.InvoiceNumber);
                //Logger.Write(result1);
                XmlDocument responseDoc = new XmlDocument();
                responseDoc.LoadXml(result1);

                XmlNode TallySuccessNode = responseDoc.SelectSingleNode("RESPONSE/CREATED");

                if (TallySuccessNode != null && TallySuccessNode.InnerText == "1")
                {
                    //SqlConnection con = new SqlConnection();
                    //con.Open();
                    //SqlCommand com = new SqlCommand("UpdateTallyPaySalStatus", con);
                    //com.CommandType = CommandType.StoredProcedure;

                    ////com.Parameters.AddWithValue("@LocId", pm.Month);
                    //com.Parameters.AddWithValue("@Month", pm.Month);
                    //com.Parameters.AddWithValue("@Year", pm.Year);

                    //com.ExecuteNonQuery();
                    //con.Close();
                }
                else
                {
                    XmlNode TallyerrorNode = responseDoc.SelectSingleNode("RESPONSE/LINEERROR");
					Logger.Write(TallyerrorNode.InnerText);
                    return TallyerrorNode.InnerText;
					
                }
                sr.Close();
            }



            }




            return "";
        }
        /*
        [HttpPost]
        public ActionResult SavePaymentDate(string ID, string date)
        {
            try
            {
                Exeptionfrom = "MyInvoiceController/SavePaymentDate";
                DateTime dateValue;
                DateTime? pdate = null;
                if (DateTime.TryParse(date, out dateValue))
                {
                    pdate = dateValue;
                    int invId = Convert.ToInt32(ID);
                    
                    //  string query = "UPDATE InVoice set Dateofpayment ='" + pdate.ToString() + "', CurrentStatus = 'Paid' from InVoice where InvoiceID = " + invId;
                    SqlConnection con = new SqlConnection(connectionString);
                    con.Open();
                    SqlCommand comm = new SqlCommand("sp_updateDateofpayment", con);
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@Dateofpayment",pdate);
                    comm.Parameters.AddWithValue("@InvoiceID", invId);
                    comm.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
            }
            return Json(new { success = 1 }, JsonRequestBehavior.AllowGet);
        }
        */
                            /// <summary>
                            /// Get File for Download
                            /// </summary>
                            /// <param name="filepath"></param>
                            /// <returns></returns>
        public FileResult GetFile(string filepath)
        {
            var fileext = ".pdf";
            if ((filepath == null || filepath == string.Empty) && Session["Filepath"] != null)
            {
                filepath = Session["Filepath"].ToString();
            }
            byte[] bytes = new byte[0];
            try
            {
                Exeptionfrom = "MyInvoiceController/GetFile";
                if (!String.IsNullOrWhiteSpace(filepath))
                {
                    string fileName = System.IO.Path.GetFileName(filepath);
                    fileext = Path.GetExtension(filepath);
                    bytes = System.IO.File.ReadAllBytes(filepath);
                    // Force the pdf document to be displayed in the browser
                    Response.AppendHeader("Content-Disposition", "inline; filename=" + fileName + ";");
                }
            }
            catch (Exception ex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
            }
            if (fileext.ToLower() == ".jpg" || fileext.ToLower() == ".jpeg")
            {
                return File(bytes, "image/jpeg");
            }
            return File(bytes, "application/pdf");
        }
    }
}