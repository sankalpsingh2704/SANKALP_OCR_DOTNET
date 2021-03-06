using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI;

using InvoiceNew.Models;
using InvoiceNew.DataAccess;
using Newtonsoft.Json;
using System.Threading;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Xml;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Specialized;
using System.Security;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Collections;
using System.Xml.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using NPOI.HSSF.Model;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace InvoiceNew.Controllers
{
    public class InvoiceController : IQinvoiceController
    {
        static string constr = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBconnection"].ConnectionString;
        static SqlConnection con = new SqlConnection(constr);
        static InvoiceRepository invoice = new InvoiceRepository();
        static UserRepository usr = new UserRepository();
        static string Exeptionfrom = string.Empty;
        static string branchname = string.Empty;
        //static string branchid = "2";
        static bool erp = false;

        /// <summary>
        /// Returns All the Invoice Details of 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index(int id = 0)
        {
            InvoiceModel model = new InvoiceModel();
            Session["filePath"] = string.Empty;

            //Session["filePath"] = "D%3A%5CInvoiceNew%5CInvoiceNew%5CFiles%5CTemp%5Ce7708f11-e797-4021-83a3-ad0bd099b6da%5CInvoice_Bizerba.pdf";
            // Session["filePath"] = "D:\\InvoiceNew\\InvoiceNew\\Files\\Temp\\62c7c5ba-787e-4857-b675-137a16e9f424\\Invoice_Bizerba.pdf";
            Session["filePath"] = System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultFile"].ToString();
            try
            {
                con.Open();
                Exeptionfrom = "InvoiceController/Index";
                SqlCommand cmd = new SqlCommand("sp_getInvoiceDetails_Model", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@InvoiceId", id);

                List<UserTypes> objUsrList = new List<UserTypes>();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        model.InvoiceID = Convert.ToInt32(sdr["InvoiceID"]);
                        model.InvoiceNumber = sdr["InvoiceNumber"].ToString();
                        model.InvoiceAmount = sdr["InvoiceAmount"].ToString().Replace(",", "");
                        model.PONumber = sdr["PONumber"].ToString();
                        model.PANNumber = sdr["PANNumber"].ToString();
                        model.VendorName = sdr["VendorName"].ToString();
                        model.VendorAddress = sdr["Address"].ToString();
                        model.InvoiceDate = Convert.ToDateTime(sdr["InvoiceDate"]);//.ToString("dd-MMM-yyyy");
                        model.InvoiceDueDate = Convert.ToDateTime(sdr["InvoiceDate"]);
                        model.InvoiceReceiveddate = Convert.ToDateTime(sdr["InvoiceReceiveddate"]);
                        model.DateOfPayment = Convert.ToDateTime(sdr["Dateofpayment"]);
                        model.DateOfAccount = Convert.ToDateTime(sdr["DateofAccount"]);
                        model.branchid = Convert.ToInt32(sdr["BranchID"]);


                    }
                    if (sdr.NextResult())
                    {

                        while (sdr.Read())
                        {
                            UserTypes usr = new UserTypes();

                            usr.UserTypeID = Convert.ToInt32(sdr["UserTypeID"]);


                            if (usr.UserTypeID != 1)
                            {

                                usr.UserTypeName = sdr["UserTypeName"].ToString();
                                if (sdr["UserID"] != DBNull.Value)
                                {

                                    usr.UserID = Convert.ToInt32(sdr["UserID"]);

                                }

                                int bid = ((List<int>)Session["BranchList"])[0];
                                usr.SelectedUserId = int.Parse(PopulateUsers(usr.UserTypeID, bid)[0].Value);
                                usr.UserName = PopulateUsers(usr.UserTypeID, bid)[0].Text;
                                usr.Users = PopulateUsers(usr.UserTypeID, bid);
                                objUsrList.Add(usr);
                            }
                        }
                    }

                    model.Date = string.Format("{0}/{1}/{2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                    model.Time = string.Format(DateTime.Now.ToShortTimeString());
                    model.UserTypes = objUsrList;
                    TempData["UserTypes"] = objUsrList;

                }
                model.InvoiceDueDate = DateTime.Now.AddDays(90);
                List<SelectListItem> securityList = new List<SelectListItem>();
                int branchid = ((List<int>)Session["BranchList"])[0];
                SqlCommand com = new SqlCommand("getSecuritylevelUsers", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@BranchID", branchid);
                using (SqlDataReader sdr = com.ExecuteReader())
                {

                    while (sdr.Read())
                    {
                        securityList.Add(new SelectListItem
                        {
                            Text = sdr["UserName"].ToString(),
                            Value = sdr["UserID"].ToString()
                        });
                    }
                }
                model.SecurityUsers = securityList;
                List<SelectListItem> invoicetypes = new List<SelectListItem>() { new SelectListItem { Text = "Invoice", Value = "Invoice" }, new SelectListItem { Text = "DC", Value = "DC" } };
                model.DocType = invoicetypes;

                com = new SqlCommand("getGateEntry", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@BranchID", branchid);
                using (SqlDataReader sdr = com.ExecuteReader())
                {

                    while (sdr.Read())
                    {
                        //model.GateEntryNumber = "GEN" + (int.Parse(sdr["GEN"].ToString()) + 1).ToString();
                        model.GateEntryNumber = sdr["GEN"].ToString();
                    }
                }



                //// get the users for dropDown
                List<SelectListItem> items = new List<SelectListItem>();
                ////string query = "select usr.UserID,usr.UserName,usrt.UsertypeID from dbo.[Users] usr inner join[UserTypes] usrt on usr.UserTypeID = usrt.UserTypeID AND Usr.UserID <> 3";
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
				ViewBag.Users = items;
                TempData["ReAssignUsers"] = items;
                ViewBag.ReAssignUsers = TempData["ReAssignUsers"];
				LocationDrp(model);
				

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
		/// <summary>
		/// Populating Location Dropdown
		/// </summary>
		/// <param name="sim">SalaryInputModel as Parameter</param>
		private void LocationDrp(InvoiceModel sim)
		{
			con.Open();
			SqlCommand com = new SqlCommand("getBranchLocationDropDown", con);
			com.CommandType = CommandType.StoredProcedure;

			DataTable btable = new DataTable();
			var blist = ((List<int>)Session["BranchList"])[0];
			btable.Columns.Add("BranchID");
			btable.Rows.Add(blist);
			com.Parameters.AddWithValue("@BranchList", btable);

			sim.LocationList = new List<SelectListItem>();
			using (SqlDataReader sdr = com.ExecuteReader())
			{
				while (sdr.Read())
				{
					sim.LocationList.Add(new SelectListItem { Text = sdr["LocationName"].ToString(), Value = sdr["Id"].ToString() });
				}
			}
			con.Close();
			
		}
		public ActionResult ActiveInvoice()
        {
            ActiveInvoiceModel model = new ActiveInvoiceModel();
            try
            {

                model.BranchDropDown = new List<SelectListItem>();
                ViewBag.BranchDropDown = "Disabled";
                con.Open();
                SqlCommand cmd = new SqlCommand("getBranchDropDown", con);
                cmd.CommandType = CommandType.StoredProcedure;
                UserRepository ur = new UserRepository();

                DataTable blist = ur.getBranchList((List<int>)Session["BranchList"]);
                cmd.Parameters.AddWithValue("@BranchList", blist);
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        var branchId = sdr["BranchID"].ToString();
                        if (Session["SelectedBranch"] != null && Session["SelectedBranch"].ToString() == branchId)
                        {
                            model.SelectedBranch = branchId;
                        }
                        model.BranchDropDown.Add(new SelectListItem() { Text = sdr["BranchName"].ToString(), Value = branchId });
                    }
                }
                if (model.BranchDropDown.Count > 1)
                {
                    ViewBag.BranchDropDown = "Enabled";
                }
                con.Close();
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult GetAllProducts(int selectedBranch)
        {
            List<InvoiceModel> objList = new List<InvoiceModel>();
            Session["SelectedBranch"] = selectedBranch;
            ViewBag.GRN = "Disabled";
            try
            {
                Exeptionfrom = "Invoice/ActiveInvoice";
                UserRepository ur = new UserRepository();
                List<int> some = (List<int>)Session["BranchList"];
                int userID = Convert.ToInt32(Session["UserID"]);
                DataTable blist = ur.getBranchList(new List<int>() { selectedBranch });
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_GetInvoiceItems", con);///GetOrderItems

                cmd.Parameters.AddWithValue("@CurrentUserID", userID);
                cmd.Parameters.AddWithValue("@BranchList", blist);
                cmd.CommandType = CommandType.StoredProcedure;
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(ds);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {

                    if (dr["CurrentStatus"].ToString() != "Pending For Payment" && dr["CurrentStatus"].ToString() != "Paid")
                    {
                        InvoiceModel objinv = new InvoiceModel();
                        objinv.InvoiceID = Convert.ToInt32(dr["InvoiceID"].ToString());
                        objinv.GateEntryNumber = dr["GateEntryNumber"].ToString();
                        //objinv.GRNNO = int.Parse(dr["GRNNO"].ToString());
                        objinv.GRNNO = dr["GRNNO"].ToString();
                        objinv.VendorName = dr["VendorName"].ToString();
                        objinv.InvoiceNumber = dr["InvoiceNumber"].ToString();
                        objinv.PONumber = dr["PONumber"].ToString();
                        objinv.InvoiceAmount = dr["InvoiceAmount"].ToString();
                        objinv.PANNumber = dr["PANNumber"].ToString();
                        objinv.SelectedType = dr["DocType"].ToString();

                        if (int.Parse(Regex.Replace(objinv.GRNNO, "[^0-9.]", "")) != 0)
                        {
                            ViewBag.GRN = "Enabled";
                        }
                        else
                        {
                            objinv.GRNNO = "";
                        }
                        //if (int.Parse(Regex.Replace(objinv.GRNNO, "[^0-9.]", "")) != 0 )
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
                        /*if (dr["Dateofpayment"] != DBNull.Value)
                            objinv.DateOfPayment = Convert.ToDateTime(dr["Dateofpayment"]).Date;
                        if (dr["DateofAccount"] != DBNull.Value)
                            objinv.DateOfAccount = Convert.ToDateTime(dr["DateofAccount"]).Date;*/
                        if (dr["CurrentStatus"] != DBNull.Value)
                        {
                            objinv.CurrentStatus = dr["CurrentStatus"].ToString();
                        }
                        objList.Add(objinv);

                    }
                }
                con.Close();
            }
            catch (Exception ex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
                return null;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
            return PartialView("_GetAllProducts", objList);
        }

        public ActionResult ImageView()
        {
            return PartialView("_ImageView");
        }
        /// <summary>
        /// Get an Invoice details by InvoiceID
        /// it returns the Invoice details for the Invoice View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public JsonResult getAllCategoryName()
        {
            List<CategoryModel> list = new List<CategoryModel>();
            try
            {
                con.Open();
                List<TaxModel> taxlist = new List<TaxModel>();
                SqlCommand cmd = new SqlCommand("sp_getAllActivatedCategoryNames", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        CategoryModel model = new CategoryModel();
                        model.CategoryID = Convert.ToInt32(sdr["CategoryID"]);
                        //model.Description = sdr["Description"].ToString();
                        model.Name = sdr["Name"].ToString();
                        /*
                        if (sdr["IsActive"].ToString() == "1")
                        {
                            model.IsActive = true;
                        }
                        model.TaxValues = sdr["TaxIDList"].ToString();
                        */
                        list.Add(model);

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

            return Json(new { CategoryList = list }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult getProductList(string CategoryID)
        {
            List<ProductsModel> prolist = new List<ProductsModel>();
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_getProductNameList", con);
                cmd.Parameters.AddWithValue("@CategoryID", CategoryID);
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        ProductsModel model = new ProductsModel();
                        model.ProductId = Convert.ToInt32(sdr["ProductId"]);
                        //model.CategoryId = Convert.ToInt32(sdr["CategoryID"]);

                        model.ItemDescription = sdr["ItemDescription"].ToString();
                        model.ItemName = sdr["ItemName"].ToString();
                        prolist.Add(model);
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

            return Json(new { products = prolist });
        }

        [HttpPost]
        public JsonResult getInvoiceItem(string productID)
        {
            try
            {
                con.Open();
                //List<InvoiceItemModel> list = new List<InvoiceItemModel>();
                SqlCommand cmd = new SqlCommand("sp_getInvoiceItemdbList", con);
                cmd.Parameters.AddWithValue("@ProductID", productID);
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        InvoiceItemModel model = new InvoiceItemModel();
                        //model.ProductId = Convert.ToInt32(sdr["ProductId"]);
                        //model.CategoryId = Convert.ToInt32(sdr["CategoryID"]);
                        model.HSN = sdr["HSNCode"].ToString();
                        model.Description = sdr["ItemDescription"].ToString();
                        model.Name = sdr["ItemName"].ToString();
                        model.UOM = sdr["UOM"].ToString();
                        model.Price = sdr["Price"].ToString();
                        if (sdr["SGST"] != DBNull.Value)
                        {
                            model.SGST = sdr["SGST"].ToString();
                        }
                        if (sdr["CGST"] != DBNull.Value)
                        {
                            model.CGST = sdr["CGST"].ToString();
                        }
                        if (sdr["IGST"] != DBNull.Value)
                        {
                            model.IGST = sdr["IGST"].ToString();
                        }
                        con.Close();
                        return Json(new { dbitem = model });

                    }
                }
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

            return (Json(new { }));
        }

        public ActionResult InvoiceView(int id)
        {
            InvoiceModel model = new InvoiceModel();
            model.InvoiceItems = new List<InvoiceItem>();
            try
            {
                Exeptionfrom = "InvoiceController/InvoiceView";
                //con.Close();
                con.Open();
                SqlCommand cmd = new SqlCommand("getLoginUserType", con);
                int userID = Convert.ToInt32(Session["UserID"]);
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.CommandType = CommandType.StoredProcedure;
                var usertype = "";
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        usertype = sdr["UserTypeID"].ToString();
                    }

                }
				con.Close();
				LocationDrp(model);
                int finalUserType = 0;
				con.Open() ;
				cmd = new SqlCommand("GetFinalUser", con);
                cmd.CommandType = CommandType.StoredProcedure;
                Object objFinalUserType = cmd.ExecuteScalar();

                if (objFinalUserType != null)
                {
                    finalUserType = (int)objFinalUserType;
                }



                cmd = new SqlCommand("sp_getInvoiceDetails_Model", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@InvoiceId", id);
                TempData["InvoiceId"] = id;
                List<UserTypes> objUsrList = new List<UserTypes>();
                Session["FilePath"] = string.Empty;
                int blist = ((List<int>)Session["BranchList"])[0];
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
                        model.VendorAddress = sdr["Address"].ToString();
                        if (sdr["BranchID"] != DBNull.Value)
                            model.branchid = Convert.ToInt32(sdr["BranchID"]);
                        if (sdr["VendorID"] != DBNull.Value)
                            model.VendorID = Convert.ToInt32(sdr["VendorID"]);
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
                            // Session["FilePath"] = model.filePath;
                        }
                        /*
                        if (sdr["PlaceOfSupply"] != DBNull.Value)
                            model.PlaceOfSupply = (sdr["PlaceOfSupply"]).ToString();
                        if (sdr["ReverseCharges"] != DBNull.Value)
                        { 
                            var rev = int.Parse((sdr["ReverseCharges"]).ToString());
                            if(rev == 1)
                            {
                                model.ReverseCharges = true;
                            }
                        }*/
                        if (sdr["GSTIN"] != DBNull.Value)
                            model.GSTIN = (sdr["GSTIN"]).ToString();
                        /*
                        if (sdr["EcommerceGSTIN"] != DBNull.Value)
                            model.EcommerceGSTIN = (sdr["EcommerceGSTIN"]).ToString();
                        if (sdr["TaxableValue"] != DBNull.Value)
                            model.TaxableValue = float.Parse(sdr["TaxableValue"].ToString());
                        if (sdr["TaxPercent"] != DBNull.Value)
                            model.TaxPercent = int.Parse(sdr["TaxPercent"].ToString());
                        if (sdr["CessAmount"] != DBNull.Value)
                            model.CessAmount = int.Parse(sdr["CessAmount"].ToString());*/
                        if (sdr["GateEntryNumber"] != DBNull.Value)
                            model.GateEntryNumber = sdr["GateEntryNumber"].ToString();
                        if (sdr["DocType"] != DBNull.Value)
                        {
                            model.SelectedType = sdr["DocType"].ToString();
                        }
                        if (sdr["VehicleNumber"] != DBNull.Value)
                            model.VehicleNumber = sdr["VehicleNumber"].ToString();
                        if (sdr["VehicleDate"] != DBNull.Value)
                        {
                            var stringval = sdr["VehicleDate"].ToString();
                            //model.Date = Convert.ToDateTime(sdr["VehicleDate"].ToString()).ToShortDateString();
                            model.Date = stringval;
                        }
                        //model.Date = Convert.ToDateTime(sdr["VehicleDate"]).ToString("dd/MM/yyyy");
                        if (sdr["VehicleTime"] != DBNull.Value)
                            model.Time = sdr["VehicleTime"].ToString();
                        if (sdr["UserName"] != DBNull.Value)
                            model.SecurityName = sdr["UserName"].ToString();

                        if (sdr["LedgerName"] != DBNull.Value)
                        {
                            model.LedgerName = sdr["LedgerName"].ToString();
                        }
						if (sdr["LocationId"] != DBNull.Value)
						{
							model.LocationId = int.Parse(sdr["LocationId"].ToString());
						}
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
                            if (usr.UserTypeID.ToString() != usertype && Convert.ToInt32(sdr["UserTypeID"]) != 1)
                            {

                                usr.Users = PopulateUsers(usr.UserTypeID, blist);
                                usr.SelectedUserId = usr.UserID;
                                objUsrList.Add(usr);
                            }
                        }
                    }

                    model.UserTypes = objUsrList;
                }
                con.Close();
                var branchid = model.branchid.ToString();
                //branchid = model.branchid.ToString();
                GetBranch(branchid);
                if (finalUserType.ToString() == usertype)
                {
                    if (erp)
                        CheckTallyExists(branchname);
                }
                //////////
                List<SelectListItem> items = new List<SelectListItem>();
                //string query = "select USR.UserID,USR.UserName,UT.UsertypeID FROM Users USR INNER JOIN UserTypes UT ON UT.UserTypeID = USR.UserTypeID AND UT.Disabled = 0 AND USR.UserID<>3";/// "select UserID,UserName from dbo.[Users] WHERE Disabled =0";
                SqlCommand comm = new SqlCommand("sp_getUsers", con);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("@UserTypeId", int.Parse(usertype));
                con.Open();
                // comm.Connection = con;
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
                con.Open();
                ViewBag.ReAssignUsers = items;/// TempData["ReAssignUsers"];
                //cmd = new SqlCommand("getUserTypeId", con);
                /*
                cmd = new SqlCommand("getLoginUserType",con);
                int userID = Convert.ToInt32(Session["UserID"]);
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.CommandType = CommandType.StoredProcedure;
                var usertype = "";
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                         usertype = sdr["UserTypeID"].ToString();
                    }

                }*/
                cmd = new SqlCommand("getUserTypesOrdered", con);
                userID = Convert.ToInt32(Session["UserID"]);
                cmd.CommandType = CommandType.StoredProcedure;
                var utypeid = "";
                var utype = "";
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        utypeid = sdr["UserTypeID"].ToString();
                        utype = sdr["UserType"].ToString();
                    }

                }



                if (utypeid == usertype)
                {
                    utype = "";
                    ViewBag.appstate = "Edit";
                }
                else
                {
                    ViewBag.appstate = "View";
                }

                cmd = new SqlCommand("getVendorRatingDisable", con);
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.CommandType = CommandType.StoredProcedure;
                var colorder = "";
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        colorder = sdr["ColumnOrder"].ToString();

                    }

                }
                ViewBag.account = "Disabled";
                string accounts = System.Web.Configuration.WebConfigurationManager.AppSettings["accounts"].ToString();
                string manage = System.Web.Configuration.WebConfigurationManager.AppSettings["manage"].ToString();
                if (colorder == accounts)
                {
                    GetBranch(branchid);
                    ViewBag.account = "Enabled";
                    ViewBag.ERP = erp;
                    if (erp)
                    {
                        List<String> lm = new List<String>();
                        cmd = new SqlCommand("GetLedgerlist", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                lm.Add(dr["ledgername"].ToString());
                            }

                        }


                        JavaScriptSerializer serial = new JavaScriptSerializer();
                        var ledgerdata = Json(new { ledgerlist = lm });
                        ViewBag.ledgerdata = serial.Serialize(ledgerdata);
                    }
                    ViewBag.VoucherList = (IEnumerable)new List<SelectListItem>() {
                                                                                    new SelectListItem() { Text = "JOURNAL", Value = "JOURNAL" },
                                                                                    new SelectListItem() { Text = "PURCHASE", Value = "PURCHASE" } };
                }
                if (colorder == manage || colorder == accounts)
                {
                    List<VendorRatings> list = new List<VendorRatings>();
                    ViewBag.VendorRating = "View";
                    cmd = new SqlCommand("getVendorRating", con);
                    cmd.Parameters.AddWithValue("@InvoiceId", model.InvoiceID);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            VendorRatings VR = new VendorRatings();
                            VR.Ratings = int.Parse(sdr["Rating"].ToString());
                            VR.URatings = sdr["Users"].ToString();
                            list.Add(VR);
                        }
                        model.VendorRatings = list;
                    }

                }
                else
                {
                    ViewBag.VendorRating = "Edit";
                }



                var invoiceNumber = model.InvoiceNumber;

                cmd = new SqlCommand("sp_getInvoiceItemsList", con);
                cmd.Parameters.AddWithValue("@InvoiceId", id);
                cmd.Parameters.AddWithValue("@ViewState", ViewBag.appstate);
                userID = Convert.ToInt32(Session["UserID"]);

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
                        envitems.GRN = sdr["GRN"]?.ToString();
                        if (envitems.GRN == "1")
                        {
                            envitems.GRNPrice = sdr["GRNPrice"].ToString();
                            envitems.GRNQty = sdr["GRNQty"].ToString();
                            envitems.GRNAmount = sdr["GRNAmount"].ToString();
                            envitems.GRNIGST = sdr["GRNIGST"].ToString();
                            envitems.GRNSGST = sdr["GRNSGST"].ToString();
                            envitems.GRNCGST = sdr["GRNCGST"].ToString();
                            envitems.GRNIAmount = sdr["GRNIAmount"].ToString();
                            envitems.GRNSAmount = sdr["GRNSAmount"].ToString();
                            envitems.GRNCAmount = sdr["GRNCAmount"].ToString();
                            envitems.GRNTotal = sdr["GRNTotal"].ToString();
                        }
                        if (ViewBag.appstate == "View")
                        {
                            envitems.CategoryId = sdr["Name"]?.ToString() ?? "";
                            envitems.GRN = sdr["GRN"]?.ToString();
                        }

                        //envitems.Ledgerlist = sdr["ledgervalue"].ToString();

                        model.InvoiceItems.Add(envitems);
                    }

                }

                ViewBag.catdata = "no";
                //ViewBag.counter = 0;
                List<ProductsModel> categoryitemlist = new List<ProductsModel>();
                foreach (var item in model.InvoiceItems)
                {

                    ViewBag.catdata = "yes";
                    cmd = new SqlCommand("sp_getProductforitem", con);
                    cmd.Parameters.AddWithValue("@ItemName", item.Name);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {

                        while (sdr.Read())
                        {

                            ProductsModel product = new ProductsModel();
                            product.ProductId = int.Parse(sdr["ProductId"].ToString());
                            product.CategoryId = int.Parse(sdr["CategoryID"].ToString());
                            product.HSNCode = sdr["HSNCode"].ToString();
                            product.ItemName = sdr["ItemName"].ToString();
                            product.ItemDescription = sdr["ItemDescription"].ToString();
                            product.UOM = sdr["UOM"].ToString();
                            product.Price = sdr["Price"].ToString();
                            if (sdr["SGST"] != DBNull.Value)
                                product.SGST = int.Parse(sdr["SGST"]?.ToString() ?? "0");
                            if (sdr["CGST"] != DBNull.Value)
                                product.CGST = int.Parse(sdr["CGST"]?.ToString() ?? "1");
                            if (sdr["IGST"] != DBNull.Value)
                                product.IGST = int.Parse(sdr["IGST"]?.ToString() ?? "2");

                            categoryitemlist.Add(product);

                        }
                    }
                }
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var selecteddata = Json(new { category = categoryitemlist });
                ViewBag.SelectedData = serializer.Serialize(selecteddata);
                con.Close();
                bool showledger = false;
                if (usertype == utypeid)
                {
                    showledger = true;
                    GetBranch(branchid);
                    GetLedgerNames(branchname);
                    if (!erp)
                    {
                        model.ledgerlist = model.VendorName;
                    }
                    List<String> list = GetSelectedLedgerNames(id);

                    ViewBag.SelectedLedgers = list;
                }
                ViewBag.Showledger = showledger;
                //sp_getInvoiceItemsList

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
            catch (Exception ex) when (ex.Message.Equals("Unable to connect to the remote server"))
            {


                TempData["ErrorMessage"] = "ERP is not started.Please start tally for editng invoices";

                return RedirectToAction("Index", "DashBoard");
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="updatetype"></param>
        /// <returns></returns>
        public JsonResult DeleteItemData(string updatetype)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("sp_deleteItem", con);
            cmd.Parameters.AddWithValue("@UpdateId", updatetype);
            cmd.CommandType = CommandType.StoredProcedure;
            var id = cmd.ExecuteNonQuery();
            con.Close();
            return Json(new { id });
        }
        public ActionResult GrnView(int Id)
        {
            List<GRNModel> grnlist = new List<GRNModel>();
            GRNExtra Extra = new GRNExtra();
            List<List<GTAXModel>> gtaxall = new List<List<GTAXModel>>();
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("getGrnView", con);
                cmd.Parameters.AddWithValue("@ID", Id);
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        GRNModel grn = new GRNModel();
                        grn.GRNPrice = sdr["GRNPrice"]?.ToString() ?? "";
                        grn.GRNQty = sdr["GRNQty"]?.ToString() ?? "";
                        grn.GRNAmount = sdr["GRNAmount"]?.ToString() ?? "";
                        grn.GRNIGST = sdr["GRNIGST"]?.ToString() ?? "";
                        grn.GRNIAmount = sdr["GRNIAmount"]?.ToString() ?? "";
                        grn.GRNSGST = sdr["GRNSGST"]?.ToString() ?? "";
                        grn.GRNSAmount = sdr["GRNSAmount"]?.ToString() ?? "";
                        grn.GRNCGST = sdr["GRNCGST"]?.ToString() ?? "";
                        grn.GRNCAmount = sdr["GRNCAmount"]?.ToString() ?? "";
                        grn.GRNTotal = sdr["GRNTotal"]?.ToString() ?? "";
                        grn.PartNumber = sdr["ItemName"]?.ToString() ?? "";
                        grn.GoodName = sdr["ItemDesc"]?.ToString() ?? "";
                        grn.HSN = sdr["HSN"]?.ToString() ?? "";
                        grn.UOM = sdr["UOM"]?.ToString() ?? "";
                        grnlist.Add(grn);
                    }
                    while (sdr.NextResult())
                    {
                        List<GTAXModel> gtaxlist = new List<GTAXModel>();
                        while (sdr.Read())
                        {
                            GTAXModel gmodel = new GTAXModel();
                            gmodel.gst = float.Parse(sdr["GST"].ToString());

                            var gval = sdr["GSUM"].ToString();
                            gmodel.gsum = float.Parse(sdr["GSUM"].ToString());
                            gtaxlist.Add(gmodel);
                        }
                        gtaxall.Add(gtaxlist);

                    }

                }


                cmd = new SqlCommand("sp_getVendorRelInfo", con);
                cmd.Parameters.AddWithValue("@InvoiceId", Id);
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        Extra.SupplierName = sdr["VendorName"]?.ToString() ?? "";
                        Extra.InvoiceRecieveDate = DateTime.Parse(sdr["InvoiceReceivedDate"]?.ToString());
                        Extra.InvoiceNumber = sdr["InvoiceNumber"]?.ToString() ?? "";
                        Extra.InvoiceDate = DateTime.Parse(sdr["InvoiceDate"]?.ToString() ?? "");
                        Extra.GSTIN = sdr["GSTIN"]?.ToString() ?? "";
                        Extra.PONumber = sdr["PONumber"]?.ToString() ?? "";
                        Extra.GRNNO = sdr["GRNNO"]?.ToString() ?? "0";
						//Extra.BranchName = sdr["BranchName"].ToString()??"";
                        if (sdr["GRNDate"] == DBNull.Value)
                        {
                            Extra.GRNDate = DateTime.Now;
                        }
                        else
                        {
                            Extra.GRNDate = DateTime.Parse(sdr["GRNDate"]?.ToString() ?? DateTime.Now.ToString());
                        }
                        Extra.Address = sdr["Address"]?.ToString() ?? "";
                        Extra.Address1 = sdr["BranchAddress1"]?.ToString() ?? "";
                        Extra.Address2 = sdr["BranchAddress2"]?.ToString() ?? "";
                        Extra.Address3 = sdr["BranchAddress3"]?.ToString() ?? "";
                        Extra.BGSTIN = sdr["BGSTIN"]?.ToString() ?? "";
                    }

                }
                float totalqty = 0;
                float totalamount = 0;
                float taxtotal = 0;
                float finalamount = 0;
                float totalcgst = 0;
                float totalsgst = 0;
                float totaligst = 0;
                foreach (var item in grnlist)
                {
                    totalqty = totalqty + float.Parse(item.GRNQty);

                    totaligst = totaligst + float.Parse(item.GRNIAmount);
                    totalsgst = totalsgst + float.Parse(item.GRNSAmount);
                    totalcgst = totalcgst + float.Parse(item.GRNCAmount);

                    totalamount = totalamount + float.Parse(item.GRNAmount);
                    taxtotal = taxtotal + float.Parse(item.GRNIAmount) + float.Parse(item.GRNSAmount) + float.Parse(item.GRNCAmount);


                }
                finalamount = finalamount + taxtotal + totalamount;
                Extra.TaxTotal = taxtotal;

                Extra.TotalCGST = totalcgst;
                Extra.TotalIGST = totaligst;
                Extra.TotalSGST = totalsgst;

                Extra.TotalQty = totalqty;
                Extra.TotalTaxable = totalamount;
                Extra.FinalAmount = finalamount;

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

            Tuple<List<GRNModel>, GRNExtra, List<List<GTAXModel>>> tuple = new Tuple<List<GRNModel>, GRNExtra, List<List<GTAXModel>>>(grnlist, Extra, gtaxall);
            return View(tuple);
        }

        public ActionResult CreditView(int Id)
        {
            Exeptionfrom = "InvoiceController/CreditView";
            CreditModel model = new CreditModel();
            model.CreditDebitNoteItems = new List<CreditDebitNoteItem>();
            model.CreditDebitNoteTaxes = new List<CreditDebitNoteTax>();
            model.CrDrNoteId = Id;

            try
            {
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
                        noteitem.Amount = float.Parse(sdr["Amount"].ToString());
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
                            notetaxitem.Tax = Decimal.Parse(sdr["Tax"].ToString());
                            notetaxitem.TaxAmount = float.Parse(sdr["TaxAmount"].ToString());

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
                    model.Total = model.Total;
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
                        model.VendorAddress = sdr["Address"].ToString();
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




        public JsonResult getPreSelectValue()
        {

            return Json(new
            { //selectedlist = categoryitemlist 
            }, JsonRequestBehavior.AllowGet);
        }
        public List<string> GetSelectedLedgerNames(int id)
        {
            List<string> list = new List<string>();
            try
            {
                DataTable dt = new DataTable();
                string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand("GetLedgerNamesByInvoiceID", con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@InvoiceID", id);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(dt);
                list = dt.AsEnumerable()
                               .Select(r => r.Field<string>("ledgerName"))
                               .ToList();
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

            return list;
            //  
        }


        private void CheckTallyExists(string branchname)
        {
            List<LedgerModel> ledgers = new List<LedgerModel>();

            WebRequest TallyRequest;

            string xmlStr = "";
            string ponumber = "12345";
            xmlStr = xmlStr + "<ENVELOPE>" + "\r\n";
            xmlStr = xmlStr + "<HEADER>" + "\r\n";
            xmlStr = xmlStr + "<VERSION>1</VERSION>" + "\r\n";

            xmlStr = xmlStr + "<TALLYREQUEST>Export</TALLYREQUEST>" + "\r\n";

            xmlStr = xmlStr + "<TYPE>Collection</TYPE>" + "\r\n";
            xmlStr = xmlStr + "<ID>TestDrColl</ID>" + "\r\n";

            xmlStr = xmlStr + "</HEADER>" + "\r\n";
            xmlStr = xmlStr + "<BODY>" + "\r\n";
            xmlStr = xmlStr + "<DESC>" + "\r\n";
            xmlStr = xmlStr + "<STATICVARIABLES>" + "\r\n";
            xmlStr = xmlStr + "<SVCURRENTCOMPANY>" + branchname + "</SVCURRENTCOMPANY>" + "\r\n";
            xmlStr = xmlStr + "</STATICVARIABLES>" + "\r\n";
            xmlStr = xmlStr + "<TDL>" + "\r\n";
            xmlStr = xmlStr + "<TDLMESSAGE>" + "\r\n";

            xmlStr = xmlStr + "<COLLECTION NAME=\"TestDrColl\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\">" + "\r\n";
            xmlStr = xmlStr + "<TYPE>Ledger</TYPE>" + "\r\n";

            xmlStr = xmlStr + "<CHILDOF>$$ALLLEDGERENTRIES.LIST</CHILDOF>" + "\r\n";
            //   xmlStr = xmlStr + "<CHILDOF>$$BILLALLOCATIONS</CHILDOF>" + "\r\n";
            xmlStr = xmlStr + "<BELONGSTO>Yes</BELONGSTO>" + "\r\n";
            xmlStr = xmlStr + " <FETCH>Ledger</FETCH>" + "\r\n";
            //    xmlStr = xmlStr + "<NATIVEMETHOD>VOUCHERNUMBER</NATIVEMETHOD>" + "\r\n";
            // xmlStr = xmlStr + "<FILTER>VoucherNumberFilter</FILTER>" + "\r\n";
            // xmlStr = xmlStr + "<FILTER>ReferenceFilter</FILTER>" + "\r\n";
            xmlStr = xmlStr + "</COLLECTION>" + "\r\n";
            //  xmlStr = xmlStr + "<SYSTEM TYPE=\"Formulae\" NAME=\"VoucherNumberFilter\" ISMODIFY=\"No\"> $Name=\"12345\"  </SYSTEM> " + "\r\n";
            //   xmlStr = xmlStr + "<SYSTEM TYPE=\"Formulae\" NAME=\"ReferenceFilter\" ISMODIFY=\"No\"> $ORDERNO=\"" + ponumber + "\"  </SYSTEM> " + "\r\n";

            xmlStr = xmlStr + "</TDLMESSAGE>" + "\r\n";
            xmlStr = xmlStr + "</TDL>" + "\r\n";
            xmlStr = xmlStr + "</DESC>" + "\r\n";
            xmlStr = xmlStr + "</BODY>" + "\r\n";
            xmlStr = xmlStr + "</ENVELOPE>" + "\r\n";
            string tallyurl = System.Web.Configuration.WebConfigurationManager.AppSettings["TallyURL"].ToString();
            TallyRequest = WebRequest.Create(tallyurl);
            ((HttpWebRequest)TallyRequest).UserAgent = ".NET Framework Example Client";
            TallyRequest.Method = "POST";
            string postData = xmlStr;
            byte[] byteArray = new byte[1024];
            byteArray = Encoding.UTF8.GetBytes(postData);
            TallyRequest.ContentType = "application/x-www-form-urlencoded";
            TallyRequest.ContentLength = byteArray.Length;
            Stream dataStream = TallyRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse tallyresponse = TallyRequest.GetResponse();
            string Response1 = (((HttpWebResponse)tallyresponse).StatusDescription).ToString();
            dataStream = tallyresponse.GetResponseStream();
            StreamReader reader1 = new StreamReader(dataStream);
            string responseFromTallyServer = reader1.ReadToEnd();



        }

        private List<LedgerModel> GetLedgerNames(string branchname)
        {
            List<LedgerModel> ledgers = new List<LedgerModel>();
            Logger.Write("branchname" + branchname);
            if (erp)
            {
                WebRequest TallyRequest;

                string xmlStr = "";
                string ponumber = "12345";
                xmlStr = xmlStr + "<ENVELOPE>" + "\r\n";
                xmlStr = xmlStr + "<HEADER>" + "\r\n";
                xmlStr = xmlStr + "<VERSION>1</VERSION>" + "\r\n";

                xmlStr = xmlStr + "<TALLYREQUEST>Export</TALLYREQUEST>" + "\r\n";

                xmlStr = xmlStr + "<TYPE>Collection</TYPE>" + "\r\n";
                xmlStr = xmlStr + "<ID>TestDrColl</ID>" + "\r\n";

                xmlStr = xmlStr + "</HEADER>" + "\r\n";
                xmlStr = xmlStr + "<BODY>" + "\r\n";
                xmlStr = xmlStr + "<DESC>" + "\r\n";
                xmlStr = xmlStr + "<STATICVARIABLES>" + "\r\n";
                xmlStr = xmlStr + "<SVCURRENTCOMPANY>" + branchname + "</SVCURRENTCOMPANY>" + "\r\n";
                xmlStr = xmlStr + "</STATICVARIABLES>" + "\r\n";
                xmlStr = xmlStr + "<TDL>" + "\r\n";
                xmlStr = xmlStr + "<TDLMESSAGE>" + "\r\n";

                xmlStr = xmlStr + "<COLLECTION NAME=\"TestDrColl\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\">" + "\r\n";
                xmlStr = xmlStr + "<TYPE>Ledger</TYPE>" + "\r\n";

                xmlStr = xmlStr + "<CHILDOF>$$ALLLEDGERENTRIES.LIST</CHILDOF>" + "\r\n";
                //   xmlStr = xmlStr + "<CHILDOF>$$BILLALLOCATIONS</CHILDOF>" + "\r\n";
                xmlStr = xmlStr + "<BELONGSTO>Yes</BELONGSTO>" + "\r\n";
                xmlStr = xmlStr + " <FETCH>Ledger</FETCH>" + "\r\n";
                //    xmlStr = xmlStr + "<NATIVEMETHOD>VOUCHERNUMBER</NATIVEMETHOD>" + "\r\n";
                // xmlStr = xmlStr + "<FILTER>VoucherNumberFilter</FILTER>" + "\r\n";
                // xmlStr = xmlStr + "<FILTER>ReferenceFilter</FILTER>" + "\r\n";
                xmlStr = xmlStr + "</COLLECTION>" + "\r\n";
                //  xmlStr = xmlStr + "<SYSTEM TYPE=\"Formulae\" NAME=\"VoucherNumberFilter\" ISMODIFY=\"No\"> $Name=\"12345\"  </SYSTEM> " + "\r\n";
                //   xmlStr = xmlStr + "<SYSTEM TYPE=\"Formulae\" NAME=\"ReferenceFilter\" ISMODIFY=\"No\"> $ORDERNO=\"" + ponumber + "\"  </SYSTEM> " + "\r\n";

                xmlStr = xmlStr + "</TDLMESSAGE>" + "\r\n";
                xmlStr = xmlStr + "</TDL>" + "\r\n";
                xmlStr = xmlStr + "</DESC>" + "\r\n";
                xmlStr = xmlStr + "</BODY>" + "\r\n";
                xmlStr = xmlStr + "</ENVELOPE>" + "\r\n";
                string tallyurl = System.Web.Configuration.WebConfigurationManager.AppSettings["TallyURL"].ToString();
                Logger.Write("tallyurl" + tallyurl);
                TallyRequest = WebRequest.Create(tallyurl);
                ((HttpWebRequest)TallyRequest).UserAgent = ".NET Framework Example Client";
                TallyRequest.Method = "POST";
                string postData = xmlStr;
                byte[] byteArray = new byte[1024];
                byteArray = Encoding.UTF8.GetBytes(postData);
                TallyRequest.ContentType = "application/x-www-form-urlencoded";
                TallyRequest.ContentLength = byteArray.Length;
                Stream dataStream = TallyRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse tallyresponse = TallyRequest.GetResponse();
                string Response1 = (((HttpWebResponse)tallyresponse).StatusDescription).ToString();
                dataStream = tallyresponse.GetResponseStream();
                StreamReader reader1 = new StreamReader(dataStream);
                string responseFromTallyServer = reader1.ReadToEnd();
                responseFromTallyServer = CleanInvalidXmlChars(responseFromTallyServer);
                Logger.Write("responseFromTallyServer" + responseFromTallyServer);

                XmlDocument xdoc = new XmlDocument();

                using (var sr = new StringReader(responseFromTallyServer))
                using (var xtr = new XmlTextReader(sr) { Namespaces = false })
                    xdoc.Load(xtr);


                XmlNodeList LedgerName = xdoc.SelectNodes("ENVELOPE/BODY/DATA/COLLECTION/LEDGER");
                Logger.Write("LedgerName" + LedgerName);

                //List<LedgerModel> ledgers = new List<LedgerModel>();
                foreach (XmlNode m in LedgerName)
                {
                    // if (m.Attributes["NAME"].Value.ToString().StartsWith("TDS") || m.Attributes["NAME"].Value.ToString().Contains("Swacha") || m.Attributes["NAME"].Value.ToString().StartsWith("Service Tax"))

                    // if ((m.Attributes["NAME"].Value.ToString()) == ("TDS - Contracts") )
                    //if (m.Attributes["NAME"].Value.ToString().StartsWith("TDS - Contracts") || m.Attributes["NAME"].Value.ToString().Contains("Swacha Bharat Cess Payable @ 0.50%") || m.Attributes["NAME"].Value.ToString().StartsWith("Service Tax Payable Basic @ 14%"))
                    //{
                    LedgerModel lm = new LedgerModel();
                    lm.LedgerId = m.Attributes["NAME"].Value.ToString();
                    lm.LedgerName = m.Attributes["NAME"].Value.ToString();

                    ledgers.Add(lm);
                    //}
                }

                List<string> Services = new List<string>();

                JavaScriptSerializer serial = new JavaScriptSerializer();



                var LedgerNames = ledgers.Where(a => !a.LedgerName.Contains("%")).Select(a => new SelectListItem { Value = a.LedgerId, Text = a.LedgerName });

                var ledgerdata = Json(new { ledgerlist = LedgerNames });

                var serializer = new JavaScriptSerializer();

                // For simplicity just use Int32's max value.
                // You could always read the value from the config section mentioned above.
                serializer.MaxJsonLength = Int32.MaxValue;

                  /*
                var result = new ContentResult
                {
                    Content = serializer.Serialize(ledgerdata),
                    ContentType = "application/json"
                }; */
                ViewBag.LedgerNames = serializer.Serialize(ledgerdata);

                ViewBag.LedgerlistNames = ledgers.Select(a => new SelectListItem { Value = a.LedgerId, Text = a.LedgerName });
                ViewBag.LedgerServices = Services;
                ViewBag.ERP = true;
            }
            else
            {
                ViewBag.ERP = false;
            }
            //foreach (XmlNode m in LedgerName)
            //{
            //    // if (m.Attributes["NAME"].Value.ToString().StartsWith("TDS") || m.Attributes["NAME"].Value.ToString().Contains("Swacha") || m.Attributes["NAME"].Value.ToString().StartsWith("Service Tax"))

            //    if ((m.Attributes["NAME"].Value.ToString()) == ("Repair - Others") || m.Attributes["NAME"].Value.ToString().Contains("Freight") || m.Attributes["NAME"].Value.ToString().Contains("Charges"))
            //    {
            //        Services.Add(m.Attributes["NAME"].Value.ToString());
            //    }
            //}

            // ViewBag.LedgerServices = Services;


            return ledgers;
        }

        public static string CleanInvalidXmlChars(string StrInput)
        {
            //Returns same value if the value is empty.
            if (string.IsNullOrWhiteSpace(StrInput))
            {
                return StrInput;
            }
            // From xml spec valid chars:
            // #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF]    
            // any Unicode character, excluding the surrogate blocks, FFFE, and FFFF.
            string RegularExp = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            return Regex.Replace(StrInput, RegularExp, String.Empty);
        }

        private List<string> GetLedgerAddress(string vendorname, string branchname)
        {
            //List<LedgerAddress> ledgers = new List<LedgerAddress>();

            WebRequest TallyRequest;

            string xmlStr = "";
            string ponumber = "12345";
            xmlStr = xmlStr + "<ENVELOPE>" + "\r\n";
            xmlStr = xmlStr + "<HEADER>" + "\r\n";
            xmlStr = xmlStr + "<VERSION>1</VERSION>" + "\r\n";

            xmlStr = xmlStr + "<TALLYREQUEST>Export</TALLYREQUEST>" + "\r\n";

            xmlStr = xmlStr + "<TYPE>Collection</TYPE>" + "\r\n";
            xmlStr = xmlStr + "<ID>TestDrColl</ID>" + "\r\n";

            xmlStr = xmlStr + "</HEADER>" + "\r\n";
            xmlStr = xmlStr + "<BODY>" + "\r\n";
            xmlStr = xmlStr + "<DESC>" + "\r\n";
            xmlStr = xmlStr + "<STATICVARIABLES>" + "\r\n";
            xmlStr = xmlStr + "<SVCURRENTCOMPANY>" + branchname + "</SVCURRENTCOMPANY>" + "\r\n";
            xmlStr = xmlStr + "</STATICVARIABLES>" + "\r\n";
            xmlStr = xmlStr + "<TDL>" + "\r\n";
            xmlStr = xmlStr + "<TDLMESSAGE>" + "\r\n";

            xmlStr = xmlStr + "<COLLECTION NAME=\"TestDrColl\" ISMODIFY=\"No\" ISFIXED=\"No\" ISINITIALIZE=\"No\" ISOPTION=\"No\" ISINTERNAL=\"No\">" + "\r\n";
            xmlStr = xmlStr + "<TYPE>Ledger</TYPE>" + "\r\n";

            xmlStr = xmlStr + "<CHILDOF>$$ALLLEDGERENTRIES.LIST</CHILDOF>" + "\r\n";
            //   xmlStr = xmlStr + "<CHILDOF>$$BILLALLOCATIONS</CHILDOF>" + "\r\n";
            xmlStr = xmlStr + "<BELONGSTO>Yes</BELONGSTO>" + "\r\n";
            xmlStr = xmlStr + " <FETCH>Ledger,Address</FETCH>" + "\r\n";
            //    xmlStr = xmlStr + "<NATIVEMETHOD>VOUCHERNUMBER</NATIVEMETHOD>" + "\r\n";
            // xmlStr = xmlStr + "<FILTER>VoucherNumberFilter</FILTER>" + "\r\n";
            // xmlStr = xmlStr + "<FILTER>ReferenceFilter</FILTER>" + "\r\n";
            xmlStr = xmlStr + "</COLLECTION>" + "\r\n";
            //  xmlStr = xmlStr + "<SYSTEM TYPE=\"Formulae\" NAME=\"VoucherNumberFilter\" ISMODIFY=\"No\"> $Name=\"12345\"  </SYSTEM> " + "\r\n";
            //   xmlStr = xmlStr + "<SYSTEM TYPE=\"Formulae\" NAME=\"ReferenceFilter\" ISMODIFY=\"No\"> $ORDERNO=\"" + ponumber + "\"  </SYSTEM> " + "\r\n";

            xmlStr = xmlStr + "</TDLMESSAGE>" + "\r\n";
            xmlStr = xmlStr + "</TDL>" + "\r\n";
            xmlStr = xmlStr + "</DESC>" + "\r\n";
            xmlStr = xmlStr + "</BODY>" + "\r\n";
            xmlStr = xmlStr + "</ENVELOPE>" + "\r\n";
            string tallyurl = System.Web.Configuration.WebConfigurationManager.AppSettings["TallyURL"].ToString();
            TallyRequest = WebRequest.Create(tallyurl);
            ((HttpWebRequest)TallyRequest).UserAgent = ".NET Framework Example Client";
            TallyRequest.Method = "POST";
            string postData = xmlStr;
            byte[] byteArray = new byte[1024];
            byteArray = Encoding.UTF8.GetBytes(postData);
            TallyRequest.ContentType = "application/x-www-form-urlencoded";
            TallyRequest.ContentLength = byteArray.Length;
            Stream dataStream = TallyRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse tallyresponse = TallyRequest.GetResponse();
            string Response1 = (((HttpWebResponse)tallyresponse).StatusDescription).ToString();
            dataStream = tallyresponse.GetResponseStream();
            StreamReader reader1 = new StreamReader(dataStream);
            string responseFromTallyServer = reader1.ReadToEnd();

            responseFromTallyServer = CleanInvalidXmlChars(responseFromTallyServer);
            XmlDocument xdoc = new XmlDocument();

            using (var sr = new StringReader(responseFromTallyServer))
            using (var xtr = new XmlTextReader(sr) { Namespaces = false })
                xdoc.Load(xtr);

            XmlNodeList LedgerName = xdoc.SelectNodes("ENVELOPE/BODY/DATA/COLLECTION/LEDGER");

            //for (int i = 0; i < LedgerName.Count; i++)
            //{
            //    XmlNode  billallcations = LedgerName[i].SelectSingleNode("ADDRESS.LIST");
            //    for (int j = 0; j < billallcations.Count; j++)
            //    {
            //        XmlNode amountNode = billallcations[j].SelectSingleNode("Address");
            //        string s = amountNode.InnerText;
            //    }
            //}

            List<string> ledgerAddress = new List<string>();
            //List<LedgerModel> ledgers = new List<LedgerModel>();
            foreach (XmlNode m in LedgerName)
            {
                if (m != null)
                {
                    if (m.Attributes["NAME"].Value.ToString() == vendorname)
                    {

                        // if (m.Attributes["NAME"].Value.ToString().StartsWith("TDS") || m.Attributes["NAME"].Value.ToString().Contains("Swacha") || m.Attributes["NAME"].Value.ToString().StartsWith("Service Tax"))
                        XmlNodeList CatNodesList = m.SelectNodes("ADDRESS.LIST");

                        if (CatNodesList != null)
                        {
                            foreach (XmlNode category in CatNodesList)

                            {
                                if (category != null)
                                {

                                    foreach (XmlNode c in category)
                                    {

                                        if (c != null)
                                        {
                                            ledgerAddress.Add(c.InnerText);
                                        }
                                    }

                                }
                            }

                        }
                    }
                }


            }





            return ledgerAddress;
        }
        [HttpPost]
        public JsonResult UpdateLedger(List<LedgerDrpModel> led)
        {
            con.Open();
            //List<CategoryModel> list = new List<CategoryModel>();
            //CategoryModel model = new CategoryModel();
            if (led != null)
            {
                foreach (var item in led)
                {
                    SqlCommand cmd = new SqlCommand("up_ledgerName", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //foreach (var item in catlist)
                    //{
                    cmd.Parameters.AddWithValue("@Invoiceid", item?.InvoiceId ?? 0);
                    cmd.Parameters.AddWithValue("@UpdateId", item?.UpdateId ?? 0);
                    cmd.Parameters.AddWithValue("@LedgerName", item?.LedgerName ?? "");

                    var id = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                }
            }
            //}

            con.Close();
            return Json(new { led });
        }
        /// <summary>
        /// /Get Branch details for tally
        /// </summary>
        /// <param name="branchid"></param>
        public void GetBranch(string branchid)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("usp_GetBranch", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BranchID", branchid);

            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {
                    branchname = sdr["BranchName"].ToString();
                    erp = bool.Parse(sdr["ERP"].ToString());
                }
            }


            con.Close();

        }
        /// <summary>
        /// Update Invoice Status
        /// </summary>
        /// <param name="model">InvoiceModel</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateInvoice(InvoiceModel model)
        {

            int mailid = 0;
            string emailTemplate = "";
            string VoucherType = "";
            string vendor = model.VendorName;
			string branchid = model.branchid.ToString();
			
            if (model.VoucherType != null)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("InsertVoucherType", con);
                cmd.Parameters.AddWithValue("@InvoiceId", model.InvoiceID);
                cmd.Parameters.AddWithValue("@VoucherType", model.VoucherType);
                cmd.CommandType = CommandType.StoredProcedure;
                Object objFinalUserType = cmd.ExecuteNonQuery();
                con.Close();
            }


            if (erp)
            {
                try
                {


                    InvoiceRepository invoice = new InvoiceRepository();
                    //  

                    int invoiceId = model.InvoiceID;
                    Session["invoiceid"] = invoiceId;



                    //emailTemplate = MailFunctionality(Server.MapPath("~/App_Data/Templates/InvoiceNotification.html"));
                    //string Name = "Dear user";
                    //emailTemplate = emailTemplate.Replace("#Name#", Name);
                    //emailTemplate = emailTemplate.Replace("#VendorName", vendor);
                    //emailTemplate = emailTemplate.Replace("#URL#", ConfigurationManager.AppSettings["MailLink"].ToString() + "/" + invoiceId);
                    //invoice.Sendmail(int.Parse(Session["UserID"].ToString()), mailid, invoiceId, " Invoice Details", emailTemplate);
                    if (model.status != "A")
                    {
                        mailid = invoice.UpdateInvoiceStatus(model, Convert.ToInt32(Session["UserID"]));
                        if (mailid != 0)
                        {

                            var UserName = usr.getUserDetails().Where(u => u.UserID == mailid).FirstOrDefault();
                            if (model.status == "A")
                            {
                                emailTemplate = MailFunctionality(Server.MapPath("~/App_Data/Templates/InvoiceNotification.html"));
                                emailTemplate = emailTemplate.Replace("#VendorName#", vendor);
                            }
                            else if (model.status == "ReAssign")
                            {
                                emailTemplate = MailFunctionality(Server.MapPath("~/App_Data/Templates/InvoiceReassign.html"));
                                emailTemplate = emailTemplate.Replace("#VendorName#", vendor);
                            }
                            else if (model.status == "R")
                            {
                                emailTemplate = MailFunctionality(Server.MapPath("~/App_Data/Templates/InvoiceReject.html"));
                                emailTemplate = emailTemplate.Replace("#status#", "rejected");
                                emailTemplate = emailTemplate.Replace("#VendorName#", vendor);
                            }
                            else if (model.status == "H")
                            {
                                emailTemplate = MailFunctionality(Server.MapPath("~/App_Data/Templates/InvoiceReject.html"));
                                emailTemplate = emailTemplate.Replace("#status#", "hold");
                                emailTemplate = emailTemplate.Replace("#VendorName#", vendor);
                            }
                            string Name = "Dear user";
                            //Name = "Hi" + " " + UserName.UserFirstName + " " + UserName.UserLastName;
                            emailTemplate = emailTemplate.Replace("#Name#", Name);
                            emailTemplate = emailTemplate.Replace("#URL#", ConfigurationManager.AppSettings["MailLink"].ToString() + "/" + model.InvoiceID);

                            // Mails not to be sent for Finance Approval
                            invoice.Sendmail(int.Parse(Session["UserID"].ToString()), mailid, model.InvoiceID, " Invoice Details", emailTemplate);
                        }
                        return RedirectToAction("ActiveInvoice", "Invoice");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(Exeptionfrom);
                    Logger.Write(ex.Message);
                }

                int finalUserType = 0;
                var usertype = "";
                int userID = 0;
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("GetFinalUser", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    Object objFinalUserType = cmd.ExecuteScalar();

                    if (objFinalUserType != null)
                    {
                        finalUserType = (int)objFinalUserType;
                    }
                    con.Close();
                    con.Open();
                    cmd = new SqlCommand("getLoginUserType", con);
                    userID = Convert.ToInt32(Session["UserID"]);
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader sdr1 = cmd.ExecuteReader())
                    {
                        while (sdr1.Read())
                        {
                            usertype = sdr1["UserTypeID"].ToString();
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
                bool GRNEnabled = false;
                DateTime GrnDate = DateTime.Now;
                if (finalUserType.ToString() == usertype)
                {

                    List<string> lstledgerNames = GetSelectedLedgerNames(model.InvoiceID);

                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_getInvoiceDetails_Model", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@InvoiceId", model.InvoiceID);
                    List<TallyLedgerAmount> objLedgerlist = new List<TallyLedgerAmount>();
                    List<TallyTaxDetails> objTaxlist = new List<TallyTaxDetails>();
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
                            model.VendorName = sdr["LedgerName"].ToString();
                            if (sdr["VendorID"] != DBNull.Value)
                                model.VendorID = Convert.ToInt32(sdr["VendorID"]);
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
                                // Session["FilePath"] = model.filePath;
                            }
                            /*
                            if (sdr["PlaceOfSupply"] != DBNull.Value)
                                model.PlaceOfSupply = (sdr["PlaceOfSupply"]).ToString();
                            if (sdr["ReverseCharges"] != DBNull.Value)
                            {
                                var rev = int.Parse((sdr["ReverseCharges"]).ToString());
                                if (rev == 1)
                                {
                                    model.ReverseCharges = true;
                                }
                            }*/
                            if (sdr["GSTIN"] != DBNull.Value)
                                model.GSTIN = (sdr["GSTIN"]).ToString();
                            /*
                            if (sdr["EcommerceGSTIN"] != DBNull.Value)
                                model.EcommerceGSTIN = (sdr["EcommerceGSTIN"]).ToString();
                            if (sdr["TaxableValue"] != DBNull.Value)
                                model.TaxableValue = float.Parse(sdr["TaxableValue"].ToString());
                            if (sdr["TaxPercent"] != DBNull.Value)
                                model.TaxPercent = int.Parse(sdr["TaxPercent"].ToString());
                            if (sdr["CessAmount"] != DBNull.Value)
                                model.CessAmount = int.Parse(sdr["CessAmount"].ToString());
                                */
                            if (sdr["GRNDate"] != DBNull.Value)
                                GrnDate = Convert.ToDateTime(sdr["GRNDate"]).Date;

                            if (sdr["VoucherType"] != DBNull.Value)
                            {
                                VoucherType = sdr["VoucherType"].ToString();
                            }
							if (sdr["LocationId"] != DBNull.Value)
							{
								model.LocationId = int.Parse(sdr["LocationId"].ToString());
							}
							

						}
                    }
                    con.Close();

                    decimal ItemGrandTotal = 0;

                    decimal Itemtotal = 0;
                    int Iteminput = 0;
                    Dictionary<string, decimal> ledgerlistAmount = new Dictionary<string, decimal>();
                    con.Open();

                    cmd = new SqlCommand("sp_getInvoiceItemsList", con);
                    cmd.Parameters.AddWithValue("@InvoiceId", model.InvoiceID);
                    cmd.Parameters.AddWithValue("@ViewState", "View");
                    userID = Convert.ToInt32(Session["UserID"]);

                    cmd.CommandType = CommandType.StoredProcedure;


                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        ItemGrandTotal = 0;
                        while (sdr.Read())
                        {
                            InvoiceItem envitems = new InvoiceItem();
                            Itemtotal = 0;
                            envitems.Amount = sdr["Amount"].ToString();
                            envitems.Ledgerlist = sdr["ledgervalue"].ToString();

                            var ledgerlistNames = envitems.Ledgerlist.Split(',').ToList();


                            //  ItemGrandTotal  += Convert.ToDecimal(sdr["Amount"].ToString().Replace(",", ""));
                            // List<string> taxledgerlist = ledgerlistNames.Where(a => a.Contains("Tax")).ToList();

                            if (sdr["GRN"].ToString() == "1")
                            {

                                GRNEnabled = true;
                                //ItemGrandTotal +=  Decimal.Parse(sdr["GRNTotal"].ToString()) ;
                                ////  ledgerlistAmount.Add(new sdr["ledgervalue"].ToString(), Decimal.Parse(sdr["Amount"].ToString()));

                                //objLedgerlist.Add(new TallyLedgerAmount { LedgerAmount = Decimal.Parse(sdr["GRNAmount"].ToString()) .ToString(), LedgerName = sdr["LedgerName"].ToString() });

                                //objTaxlist.Add(new TallyTaxDetails { TaxName = "IGST", TaxAmount =  Decimal.Parse(sdr["GRNIAmount"].ToString()) , TaxPercentage = sdr["GRNIGST"].ToString() });
                                //objTaxlist.Add(new TallyTaxDetails { TaxName = "CGST", TaxAmount = Decimal.Parse(sdr["GRNCAmount"].ToString()) , TaxPercentage = sdr["GRNCGST"].ToString() });
                                //objTaxlist.Add(new TallyTaxDetails { TaxName = "SGST", TaxAmount =  Decimal.Parse(sdr["GRNSAmount"].ToString()) , TaxPercentage = sdr["GRNSGST"].ToString() });
                            }

                            ItemGrandTotal += Decimal.Parse(sdr["Total"].ToString());


                            //  ledgerlistAmount.Add(new sdr["ledgervalue"].ToString(), Decimal.Parse(sdr["Amount"].ToString()));

                            objLedgerlist.Add(new TallyLedgerAmount { LedgerAmount = Decimal.Parse(sdr["Amount"].ToString()).ToString(), LedgerName = sdr["LedgerName"].ToString() });

                            objTaxlist.Add(new TallyTaxDetails { TaxName = "IGST", TaxAmount = Decimal.Parse(sdr["IGSTAmount"].ToString()), TaxPercentage = sdr["IGST"].ToString() });
                            objTaxlist.Add(new TallyTaxDetails { TaxName = "CGST", TaxAmount = Decimal.Parse(sdr["CGSTAmount"].ToString()), TaxPercentage = sdr["CGST"].ToString() });
                            objTaxlist.Add(new TallyTaxDetails { TaxName = "SGST", TaxAmount = Decimal.Parse(sdr["SGSTAmount"].ToString()), TaxPercentage = sdr["SGST"].ToString() });

                        }

                    }


                    List<TaxClass> taxlist = objTaxlist.Where(mk => mk.TaxPercentage != "0").GroupBy(x => new { x.TaxName, x.TaxPercentage }).Select(a => new TaxClass { TaxName = a.Key.TaxName, TaxPercentage = a.Key.TaxPercentage, TaxAmount = a.Sum(s => s.TaxAmount) }).ToList();

                    List<TallyLedgerAmount> catlist = objLedgerlist.GroupBy(a => a.LedgerName).Select(a => new TallyLedgerAmount { LedgerAmount = a.Sum(b => Decimal.Parse(b.LedgerAmount)).ToString(), LedgerName = a.Key }).ToList();



                    con.Close();





                    //  DateTime dt1 = new DateTime(2017, 11, 02);
                    //string invoicedate = model.InvoiceDate.Value.ToString("yyyyMMdd");
                    string invoicedate = GrnDate.ToString("yyyyMMdd");
                    //  string invoicedate = dt1.ToString("yyyyMMdd");
                    string invoiceno = model.InvoiceNumber;
                    GetBranch(branchid);
                    List<LedgerModel> lstLegderlist = GetLedgerNames(branchname);

                    string vendorname = model.VendorName;

                    List<string> listAdress = GetLedgerAddress(vendorname, branchname);
                    String vendorName = SecurityElement.Escape(vendorname);
                    String responseResult = "";
					List<string> Loclist = getLocationNameById(model.LocationId);
					if (VoucherType == "PURCHASE")
                    {
                        responseResult = AddtoPurchaseXml(model, vendorName, invoicedate, taxlist, catlist, ledgerlistAmount, lstLegderlist, ItemGrandTotal, listAdress, branchname);

                        if (GRNEnabled)
                        {
                            decimal ItemCreditNoteGrandTotal = 0;
                            List<TallyLedgerAmount> objCreditNoteLedgerlist = new List<TallyLedgerAmount>();
                            List<TallyTaxDetails> objCreditNoteTaxlist = new List<TallyTaxDetails>();
                            con.Open();
                            bool CreditNote = false;
                            cmd = new SqlCommand("sp_getInvoiceDebitCreditList", con);
                            cmd.Parameters.AddWithValue("@InvoiceId", model.InvoiceID);
                            cmd.Parameters.AddWithValue("@DocType", 1);
                            cmd.CommandType = CommandType.StoredProcedure;
                            using (SqlDataReader sdr = cmd.ExecuteReader())
                            {
                                while (sdr.Read())
                                {
                                    if (sdr["InvoiceId"] != DBNull.Value)
                                    {
                                        CreditNote = true;
                                        ItemCreditNoteGrandTotal += Decimal.Parse(sdr["Total"].ToString());
                                        objCreditNoteLedgerlist.Add(new TallyLedgerAmount { LedgerAmount = Decimal.Parse(sdr["Amount"].ToString()).ToString(), LedgerName = sdr["LedgerName"].ToString() });
                                        objCreditNoteTaxlist.Add(new TallyTaxDetails { TaxName = "IGST", TaxAmount = Decimal.Parse(sdr["IGSTAmount"].ToString()), TaxPercentage = sdr["IGST"].ToString() });
                                        objCreditNoteTaxlist.Add(new TallyTaxDetails { TaxName = "CGST", TaxAmount = Decimal.Parse(sdr["CGSTAmount"].ToString()), TaxPercentage = sdr["CGST"].ToString() });
                                        objCreditNoteTaxlist.Add(new TallyTaxDetails { TaxName = "SGST", TaxAmount = Decimal.Parse(sdr["SGSTAmount"].ToString()), TaxPercentage = sdr["SGST"].ToString() });

                                    }

                                }

                                if (CreditNote)
                                {
                                    GetBranch(branchid);
                                    List<TaxClass> creditnotetaxlist = objCreditNoteTaxlist.Where(mk => mk.TaxPercentage != "0").GroupBy(x => new { x.TaxName, x.TaxPercentage }).Select(a => new TaxClass { TaxName = a.Key.TaxName, TaxPercentage = a.Key.TaxPercentage, TaxAmount = a.Sum(s => s.TaxAmount) }).ToList();

                                    List<TallyLedgerAmount> creditnotecatlist = objCreditNoteLedgerlist.GroupBy(a => a.LedgerName).Select(a => new TallyLedgerAmount { LedgerAmount = a.Sum(b => Decimal.Parse(b.LedgerAmount)).ToString(), LedgerName = a.Key }).ToList();

                                    responseResult = AddCreditNoteXml(model, vendorName, invoicedate, creditnotetaxlist, creditnotecatlist, lstLegderlist, ItemCreditNoteGrandTotal, listAdress, branchname);
                                }
                                sdr.Close();
                                con.Close();
                                CreditNote = false;
                            }
                            con.Open();
                            decimal ItemDebitNoteGrandTotal = 0;
                            bool DebitNote = false;
                            List<TallyLedgerAmount> objDebitNoteLedgerlist = new List<TallyLedgerAmount>();
                            List<TallyTaxDetails> objDebitNoteTaxlist = new List<TallyTaxDetails>();
                            cmd = new SqlCommand("sp_getInvoiceDebitCreditList", con);
                            cmd.Parameters.AddWithValue("@InvoiceId", model.InvoiceID);
                            cmd.Parameters.AddWithValue("@DocType", 2);
                            cmd.CommandType = CommandType.StoredProcedure;
                            using (SqlDataReader sdr = cmd.ExecuteReader())
                            {
                                while (sdr.Read())
                                {
                                    if (sdr["InvoiceId"] != DBNull.Value)
                                    {
                                        DebitNote = true;
                                        ItemDebitNoteGrandTotal += Decimal.Parse(sdr["Total"].ToString());
                                        objDebitNoteLedgerlist.Add(new TallyLedgerAmount { LedgerAmount = Decimal.Parse(sdr["Amount"].ToString()).ToString(), LedgerName = sdr["LedgerName"].ToString() });
                                        objDebitNoteTaxlist.Add(new TallyTaxDetails { TaxName = "IGST", TaxAmount = Decimal.Parse(sdr["IGSTAmount"].ToString()), TaxPercentage = sdr["IGST"].ToString() });
                                        objDebitNoteTaxlist.Add(new TallyTaxDetails { TaxName = "CGST", TaxAmount = Decimal.Parse(sdr["CGSTAmount"].ToString()), TaxPercentage = sdr["CGST"].ToString() });
                                        objDebitNoteTaxlist.Add(new TallyTaxDetails { TaxName = "SGST", TaxAmount = Decimal.Parse(sdr["SGSTAmount"].ToString()), TaxPercentage = sdr["SGST"].ToString() });


                                        // AddDebitNoteXml(model, vendorName, invoicedate, taxlist, catlist, ledgerlistAmount, lstLegderlist, ItemGrandTotal, listAdress);
                                    }
                                }

                                if (DebitNote)
                                {
                                    GetBranch(branchid);
                                    List<TaxClass> debitnotetaxlist = objDebitNoteTaxlist.Where(mk => mk.TaxPercentage != "0").GroupBy(x => new { x.TaxName, x.TaxPercentage }).Select(a => new TaxClass { TaxName = a.Key.TaxName, TaxPercentage = a.Key.TaxPercentage, TaxAmount = a.Sum(s => s.TaxAmount) }).ToList();

                                    List<TallyLedgerAmount> debitnotecatlist = objDebitNoteLedgerlist.GroupBy(a => a.LedgerName).Select(a => new TallyLedgerAmount { LedgerAmount = a.Sum(b => Decimal.Parse(b.LedgerAmount)).ToString(), LedgerName = a.Key }).ToList();

                                    responseResult = AddDebitNoteXml(model, vendorName, invoicedate, debitnotetaxlist, debitnotecatlist, lstLegderlist, ItemDebitNoteGrandTotal, listAdress, branchname);
                                }
                                sdr.Close();
                                con.Close();
                                DebitNote = false;
                            }
                        }

                        GRNEnabled = false;

                        if (responseResult != "")
                        {

                            XmlDocument responseDoc = new XmlDocument();
                            responseDoc.LoadXml(responseResult);

                            XmlNode LineErrorNode = responseDoc.SelectSingleNode("RESPONSE/LINEERROR");

                            if (LineErrorNode != null)
                            {
                                TempData["TallyErrorMessage"] = "Invoice Number :" + model.InvoiceNumber + "  " + LineErrorNode.InnerText;
                                return RedirectToAction("ActiveInvoice", "Invoice");
                            }
                            XmlNode TallySuccessNode = responseDoc.SelectSingleNode("RESPONSE/CREATED");

                            if (TallySuccessNode != null && TallySuccessNode.InnerText == "1")
                            {
                                mailid = invoice.UpdateInvoiceStatus(model, Convert.ToInt32(Session["UserID"]));
                                if (mailid != 0)
                                {

                                    var UserName = usr.getUserDetails().Where(u => u.UserID == mailid).FirstOrDefault();
                                    if (model.status == "A")
                                    {
                                        emailTemplate = MailFunctionality(Server.MapPath("~/App_Data/Templates/InvoiceNotification.html"));
                                        emailTemplate = emailTemplate.Replace("#VendorName#", vendor);
                                    }
                                    else if (model.status == "ReAssign")
                                    {
                                        emailTemplate = MailFunctionality(Server.MapPath("~/App_Data/Templates/InvoiceReassign.html"));
                                        emailTemplate = emailTemplate.Replace("#VendorName#", vendor);
                                    }
                                    else if (model.status == "R")
                                    {
                                        emailTemplate = MailFunctionality(Server.MapPath("~/App_Data/Templates/InvoiceReject.html"));
                                        emailTemplate = emailTemplate.Replace("#status#", "rejected");
                                        emailTemplate = emailTemplate.Replace("#VendorName#", vendor);
                                    }
                                    else if (model.status == "H")
                                    {
                                        emailTemplate = MailFunctionality(Server.MapPath("~/App_Data/Templates/InvoiceReject.html"));
                                        emailTemplate = emailTemplate.Replace("#status#", "hold");
                                        emailTemplate = emailTemplate.Replace("#VendorName#", vendor);
                                    }
                                    string Name = "Dear user";
                                    //Name = "Hi" + " " + UserName.UserFirstName + " " + UserName.UserLastName;
                                    emailTemplate = emailTemplate.Replace("#Name#", Name);
                                    emailTemplate = emailTemplate.Replace("#URL#", ConfigurationManager.AppSettings["MailLink"].ToString() + "/" + model.InvoiceID);

                                    // Mails not to be sent for Finance Approval
                                    invoice.Sendmail(int.Parse(Session["UserID"].ToString()), mailid, model.InvoiceID, " Invoice Details", emailTemplate);
                                }
                            }
                        }

                    }
                    else if (VoucherType == "JOURNAL")
                    {
                        GetBranch(branchid);
						
                        XmlDocument journaldoc = new XmlDocument();
                        journaldoc.Load(Server.MapPath("~/tallyxml/JournalVoucher.xml"));

                        XmlNode Cname = journaldoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDESC/STATICVARIABLES/SVCURRENTCOMPANY");
                        Cname.InnerText = branchname;
                        XmlNode Rname = journaldoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/COMPANY/REMOTECMPINFO.LIST/REMOTECMPNAME");
                        Rname.InnerText = branchname;

                        XmlNode journalroot = journaldoc.DocumentElement;
                        XmlNode journalparentNode = journaldoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER");

                        XmlNode journalNumberNode = journaldoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/NARRATION");
                        journalNumberNode.InnerText = model.Comment;

                        XmlNode JournalNode = journaldoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/VOUCHERNUMBER");
                        JournalNode.InnerText = model.InvoiceNumber;

                        XmlNode JournalDATENode = journaldoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/DATE");
                        JournalDATENode.InnerText = invoicedate;



                        XmlNode JournalEFFECTIVEDATENode = journaldoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/EFFECTIVEDATE");
                        JournalEFFECTIVEDATENode.InnerText = invoicedate;


                        XmlNode PARTYLEDGERNAME = journaldoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/PARTYLEDGERNAME");
                        PARTYLEDGERNAME.InnerText = catlist.FirstOrDefault().LedgerName;


                        XmlNode VOUCHERTYPENAME = journaldoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/VOUCHERTYPENAME");
                        VOUCHERTYPENAME.InnerText = "JN-"+Loclist[0];

                        //VOUCHERTYPENAME
                        //PARTYLEDGERNAME
                        XmlDocumentFragment journalfrag = journaldoc.CreateDocumentFragment();

                        foreach (var ledger in catlist)
                        {


                            journalfrag = journaldoc.CreateDocumentFragment();
                            //journalfrag.InnerXml = @"<ALLLEDGERENTRIES.LIST>" +
                            //"<OLDAUDITENTRYIDS.LIST TYPE = \"Number\">" +
                            //"<OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>" +
                            //"</OLDAUDITENTRYIDS.LIST>" +
                            //"<LEDGERNAME>" + SecurityElement.Escape(ledger.LedgerName) + "</LEDGERNAME>" +
                            //"<GSTCLASS/>" +
                            //"<ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>" +
                            //"<LEDGERFROMITEM>No</LEDGERFROMITEM>" +
                            //"<REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>" +
                            //"<ISPARTYLEDGER>No</ISPARTYLEDGER>" +
                            //"<ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE>" +
                            //"<AMOUNT>-" + ledger.LedgerAmount + "</AMOUNT>" +
                            //"<VATEXPAMOUNT>-" + ledger.LedgerAmount + "</VATEXPAMOUNT>" +
                            //"<SERVICETAXDETAILS.LIST></SERVICETAXDETAILS.LIST> " +
                            //"<BANKALLOCATIONS.LIST></BANKALLOCATIONS.LIST>" +
                            //"<BILLALLOCATIONS.LIST></BILLALLOCATIONS.LIST>" +
                            //"<INTERESTCOLLECTION.LIST></INTERESTCOLLECTION.LIST> " +
                            //"<OLDAUDITENTRIES.LIST></OLDAUDITENTRIES.LIST>" +
                            //"<ACCOUNTAUDITENTRIES.LIST></ACCOUNTAUDITENTRIES.LIST> " +
                            //"<AUDITENTRIES.LIST></AUDITENTRIES.LIST>" +
                            //"<INPUTCRALLOCS.LIST></INPUTCRALLOCS.LIST>" +
                            //"<DUTYHEADDETAILS.LIST></DUTYHEADDETAILS.LIST>" +
                            //"<EXCISEDUTYHEADDETAILS.LIST></EXCISEDUTYHEADDETAILS.LIST>" +
                            //"<RATEDETAILS.LIST></RATEDETAILS.LIST>" +
                            //"<SUMMARYALLOCS.LIST></SUMMARYALLOCS.LIST> " +
                            //"<STPYMTDETAILS.LIST></STPYMTDETAILS.LIST>" +
                            //"<EXCISEPAYMENTALLOCATIONS.LIST></EXCISEPAYMENTALLOCATIONS.LIST>" +
                            //"<TAXBILLALLOCATIONS.LIST></TAXBILLALLOCATIONS.LIST>" +
                            //"<TAXOBJECTALLOCATIONS.LIST></TAXOBJECTALLOCATIONS.LIST>" +
                            //"<TDSEXPENSEALLOCATIONS.LIST></TDSEXPENSEALLOCATIONS.LIST>" +
                            //"<VATSTATUTORYDETAILS.LIST></VATSTATUTORYDETAILS.LIST>" +
                            //"<COSTTRACKALLOCATIONS.LIST></COSTTRACKALLOCATIONS.LIST>" +
                            //"<REFVOUCHERDETAILS.LIST></REFVOUCHERDETAILS.LIST>" +
                            //"<INVOICEWISEDETAILS.LIST></INVOICEWISEDETAILS.LIST>" +
                            //"<VATITCDETAILS.LIST></VATITCDETAILS.LIST>" +
                            //"<ADVANCETAXDETAILS.LIST></ADVANCETAXDETAILS.LIST>" +
                            //"</ALLLEDGERENTRIES.LIST>";
                            ledger.LedgerAmount = "12500";
                            journalfrag.InnerXml = @"<ALLLEDGERENTRIES.LIST>" +
							"<OLDAUDITENTRYIDS.LIST TYPE=\"Number\">" +
							"<OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>" +
							"</OLDAUDITENTRYIDS.LIST>" +
							"<LEDGERNAME>" + SecurityElement.Escape(ledger.LedgerName) +"</LEDGERNAME>" +
							"<GSTCLASS/>" +
							"<ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>" +
							"<LEDGERFROMITEM>No</LEDGERFROMITEM>" +
							"<REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>" +
							"<ISPARTYLEDGER>Yes</ISPARTYLEDGER>" +
							"<ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE>" +
							"<AMOUNT>-"+ ledger.LedgerAmount +"</AMOUNT>" +
							"<VATEXPAMOUNT>-" + ledger.LedgerAmount + "</VATEXPAMOUNT>" +
							"<SERVICETAXDETAILS.LIST></SERVICETAXDETAILS.LIST>" +
							"<CATEGORYALLOCATIONS.LIST>" +
							"<CATEGORY>Service</CATEGORY>" +
							"<ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>" +
							"<COSTCENTREALLOCATIONS.LIST>" +
							"<NAME>Ambattur Unit I-Service</NAME>" +
							"<AMOUNT>-" + ledger.LedgerAmount + "</AMOUNT>" +
							"</COSTCENTREALLOCATIONS.LIST>" +
							"</CATEGORYALLOCATIONS.LIST>" +
							"<BANKALLOCATIONS.LIST></BANKALLOCATIONS.LIST>" +
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
                            journalparentNode.InsertAfter(journalfrag, journalparentNode.LastChild);
                            journalfrag = journaldoc.CreateDocumentFragment();
                        }

                        //foreach (var ledger in taxlist)
                        //{
                        //    var ledgerNamelst = lstLegderlist.FirstOrDefault(a => a.LedgerName.Contains(ledger.TaxName) && a.LedgerName.StartsWith(ledger.TaxName) && a.LedgerName.ToUpper().Contains("INPUT") && a.LedgerName.Contains(ledger.TaxPercentage));
                        //    if (ledgerNamelst != null)
                        //    {
                        //        var ledgerName = ledgerNamelst.LedgerName;
                        //        journalfrag = journaldoc.CreateDocumentFragment();
                        //        journalfrag.InnerXml = @"<ALLLEDGERENTRIES.LIST>" +
                        //        "<OLDAUDITENTRYIDS.LIST TYPE = \"Number\">" +
                        //        "<OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>" +
                        //        "</OLDAUDITENTRYIDS.LIST>" +
                        //        "<LEDGERNAME>" + SecurityElement.Escape(ledgerName) + "</LEDGERNAME>" +
                        //        "<GSTCLASS/>" +
                        //        "<ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>" +
                        //        "<LEDGERFROMITEM>No</LEDGERFROMITEM>" +
                        //        "<REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>" +
                        //        "<ISPARTYLEDGER>No</ISPARTYLEDGER>" +
                        //        "<ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE>" +
                        //        "<AMOUNT>-" + ledger.TaxAmount.ToString() + "</AMOUNT>" +
                        //         "<VATEXPAMOUNT>-" + ledger.TaxAmount.ToString() + "</VATEXPAMOUNT>" +
                        //        "<SERVICETAXDETAILS.LIST></SERVICETAXDETAILS.LIST> " +
                        //        "<BANKALLOCATIONS.LIST></BANKALLOCATIONS.LIST>" +
                        //        "<BILLALLOCATIONS.LIST></BILLALLOCATIONS.LIST>" +
                        //        "<INTERESTCOLLECTION.LIST></INTERESTCOLLECTION.LIST> " +
                        //        "<OLDAUDITENTRIES.LIST></OLDAUDITENTRIES.LIST>" +
                        //        "<ACCOUNTAUDITENTRIES.LIST></ACCOUNTAUDITENTRIES.LIST> " +
                        //        "<AUDITENTRIES.LIST></AUDITENTRIES.LIST>" +
                        //        "<INPUTCRALLOCS.LIST></INPUTCRALLOCS.LIST>" +
                        //        "<DUTYHEADDETAILS.LIST></DUTYHEADDETAILS.LIST>" +
                        //        "<EXCISEDUTYHEADDETAILS.LIST></EXCISEDUTYHEADDETAILS.LIST>" +
                        //        "<RATEDETAILS.LIST></RATEDETAILS.LIST>" +
                        //        "<SUMMARYALLOCS.LIST></SUMMARYALLOCS.LIST> " +
                        //        "<STPYMTDETAILS.LIST></STPYMTDETAILS.LIST>" +
                        //        "<EXCISEPAYMENTALLOCATIONS.LIST></EXCISEPAYMENTALLOCATIONS.LIST>" +
                        //        "<TAXBILLALLOCATIONS.LIST></TAXBILLALLOCATIONS.LIST>" +
                        //        "<TAXOBJECTALLOCATIONS.LIST></TAXOBJECTALLOCATIONS.LIST>" +
                        //        "<TDSEXPENSEALLOCATIONS.LIST></TDSEXPENSEALLOCATIONS.LIST>" +
                        //        "<VATSTATUTORYDETAILS.LIST></VATSTATUTORYDETAILS.LIST>" +
                        //        "<COSTTRACKALLOCATIONS.LIST></COSTTRACKALLOCATIONS.LIST>" +
                        //        "<REFVOUCHERDETAILS.LIST></REFVOUCHERDETAILS.LIST>" +
                        //        "<INVOICEWISEDETAILS.LIST></INVOICEWISEDETAILS.LIST>" +
                        //        "<VATITCDETAILS.LIST></VATITCDETAILS.LIST>" +
                        //        "<ADVANCETAXDETAILS.LIST></ADVANCETAXDETAILS.LIST>" +
                        //        "</ALLLEDGERENTRIES.LIST>";
                        //        journalparentNode.InsertAfter(journalfrag, journalparentNode.LastChild);
                        //        journalfrag = journaldoc.CreateDocumentFragment();
                        //    }
                        //}

                        //                    journalfrag.InnerXml = @"<ALLLEDGERENTRIES.LIST>" +
                        //"<OLDAUDITENTRYIDS.LIST TYPE =  \"Number\">" +
                        //"<OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>" +
                        //"</OLDAUDITENTRYIDS.LIST>" +
                        //"<LEDGERNAME>" + vendorName + "</LEDGERNAME>" +
                        //"<GSTCLASS/>" +
                        //"<ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>" +
                        //"<LEDGERFROMITEM>No</LEDGERFROMITEM>" +
                        //"<REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>" +
                        //"<ISPARTYLEDGER>Yes</ISPARTYLEDGER>" +
                        //"<ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE> " +
                        //"<AMOUNT>" + ItemGrandTotal.ToString() + "</AMOUNT>" +
                        //"<VATEXPAMOUNT>" + ItemGrandTotal.ToString() + "</VATEXPAMOUNT>" +
                        //"<SERVICETAXDETAILS.LIST></SERVICETAXDETAILS.LIST>" +
                        //"<BANKALLOCATIONS.LIST></BANKALLOCATIONS.LIST> " +
                        //"<BILLALLOCATIONS.LIST>" +
                        //"<NAME>" + model.InvoiceNumber + "</NAME>" +
                        //"<BILLTYPE>New Ref</BILLTYPE>" +
                        //"<TDSDEDUCTEEISSPECIALRATE>No</TDSDEDUCTEEISSPECIALRATE>" +
                        //"<AMOUNT>" + ItemGrandTotal.ToString() + "</AMOUNT>" +
                        //"<INTERESTCOLLECTION.LIST></INTERESTCOLLECTION.LIST>" +
                        //"<STBILLCATEGORIES.LIST></STBILLCATEGORIES.LIST>" +
                        //"</BILLALLOCATIONS.LIST>" +
                        //"<INTERESTCOLLECTION.LIST></INTERESTCOLLECTION.LIST>" +
                        //"<OLDAUDITENTRIES.LIST></OLDAUDITENTRIES.LIST>" +
                        //"<ACCOUNTAUDITENTRIES.LIST></ACCOUNTAUDITENTRIES.LIST>" +
                        //"<AUDITENTRIES.LIST></AUDITENTRIES.LIST>" +
                        //"<INPUTCRALLOCS.LIST></INPUTCRALLOCS.LIST>" +
                        //"<DUTYHEADDETAILS.LIST></DUTYHEADDETAILS.LIST>" +
                        //"<EXCISEDUTYHEADDETAILS.LIST></EXCISEDUTYHEADDETAILS.LIST>" +
                        //"<RATEDETAILS.LIST></RATEDETAILS.LIST>" +
                        //"<SUMMARYALLOCS.LIST></SUMMARYALLOCS.LIST>" +
                        //"<STPYMTDETAILS.LIST></STPYMTDETAILS.LIST>" +
                        //"<EXCISEPAYMENTALLOCATIONS.LIST></EXCISEPAYMENTALLOCATIONS.LIST>" +
                        //"<TAXBILLALLOCATIONS.LIST></TAXBILLALLOCATIONS.LIST>" +
                        //"<TAXOBJECTALLOCATIONS.LIST></TAXOBJECTALLOCATIONS.LIST>" +
                        //"<TDSEXPENSEALLOCATIONS.LIST></TDSEXPENSEALLOCATIONS.LIST>" +
                        //"<VATSTATUTORYDETAILS.LIST></VATSTATUTORYDETAILS.LIST>" +
                        //"<COSTTRACKALLOCATIONS.LIST></COSTTRACKALLOCATIONS.LIST>" +
                        //"<REFVOUCHERDETAILS.LIST></REFVOUCHERDETAILS.LIST>" +
                        //"<INVOICEWISEDETAILS.LIST></INVOICEWISEDETAILS.LIST>" +
                        //"<VATITCDETAILS.LIST></VATITCDETAILS.LIST>" +
                        //"<ADVANCETAXDETAILS.LIST></ADVANCETAXDETAILS.LIST>" +
                        //"</ALLLEDGERENTRIES.LIST>";

                        ItemGrandTotal = 12500;
                        journalfrag.InnerXml = @"<ALLLEDGERENTRIES.LIST>"+
						"<OLDAUDITENTRYIDS.LIST TYPE=\"Number\">" +
						"<OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>" +
						"</OLDAUDITENTRYIDS.LIST>" +
						"<LEDGERNAME>" + vendorName + "</LEDGERNAME>" +
						"<GSTCLASS/>" +
						"<ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>" +
						"<LEDGERFROMITEM>No</LEDGERFROMITEM>" +
						"<REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>" +
						"<ISPARTYLEDGER>Yes</ISPARTYLEDGER>" +
						"<ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE>" +
						"<AMOUNT>" + ItemGrandTotal.ToString() + "</AMOUNT>" +
						"<VATEXPAMOUNT>" + ItemGrandTotal.ToString() + "</VATEXPAMOUNT>" +
						"<SERVICETAXDETAILS.LIST></SERVICETAXDETAILS.LIST>" +
						"<CATEGORYALLOCATIONS.LIST>" +
						"<CATEGORY>Service</CATEGORY>" +
						"<ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>" +
						"<COSTCENTREALLOCATIONS.LIST>" +
						"<NAME>"+Loclist[2]+"</NAME>" +
						"<AMOUNT>"+ ItemGrandTotal.ToString() + "</AMOUNT>" +
						"</COSTCENTREALLOCATIONS.LIST>" +
						"</CATEGORYALLOCATIONS.LIST>" +
						"<BANKALLOCATIONS.LIST></BANKALLOCATIONS.LIST>" +
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
                        journalparentNode.InsertAfter(journalfrag, journalparentNode.LastChild);
                        string mt = journaldoc.InnerXml.ToString();


                        if (model.status == "A")
                        {
                            string tallyurls = System.Web.Configuration.WebConfigurationManager.AppSettings["TallyURL"].ToString();
                            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(tallyurls);

                            httpWebRequest.Method = "POST";
                            httpWebRequest.ContentLength = mt.Length;
                            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                            StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                            streamWriter.Write(mt);
                            streamWriter.Close();
                            string result2;
                            HttpWebResponse objResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
                            {
                                result2 = sr.ReadToEnd();

                                Logger.Write("InvoiceID : " + model.InvoiceID);
                                Logger.Write("InvoiceNumber : " + model.InvoiceNumber);
                                Logger.Write(result2);


                                XmlDocument responseDoc = new XmlDocument();
                                responseDoc.LoadXml(result2);

                                XmlNode LineErrorNode = responseDoc.SelectSingleNode("RESPONSE/LINEERROR");

                                if (LineErrorNode != null)
                                {
                                    TempData["TallyErrorMessage"] = "Invoice Number :" + model.InvoiceNumber + "  " + LineErrorNode.InnerText;
                                    return RedirectToAction("ActiveInvoice", "Invoice");
                                }
                                XmlNode TallySuccessNode = responseDoc.SelectSingleNode("RESPONSE/CREATED");

                                if (TallySuccessNode != null && TallySuccessNode.InnerText == "1")
                                {

                                    mailid = invoice.UpdateInvoiceStatus(model, Convert.ToInt32(Session["UserID"]));
                                    if (mailid != 0)
                                    {

                                        var UserName = usr.getUserDetails().Where(u => u.UserID == mailid).FirstOrDefault();
                                        if (model.status == "A")
                                        {
                                            emailTemplate = MailFunctionality(Server.MapPath("~/App_Data/Templates/InvoiceNotification.html"));
                                            emailTemplate = emailTemplate.Replace("#VendorName#", vendor);
                                        }
                                        else if (model.status == "ReAssign")
                                        {
                                            emailTemplate = MailFunctionality(Server.MapPath("~/App_Data/Templates/InvoiceReassign.html"));
                                            emailTemplate = emailTemplate.Replace("#VendorName#", vendor);
                                        }
                                        else if (model.status == "R")
                                        {
                                            emailTemplate = MailFunctionality(Server.MapPath("~/App_Data/Templates/InvoiceReject.html"));
                                            emailTemplate = emailTemplate.Replace("#status#", "rejected");
                                            emailTemplate = emailTemplate.Replace("#VendorName#", vendor);
                                        }
                                        else if (model.status == "H")
                                        {
                                            emailTemplate = MailFunctionality(Server.MapPath("~/App_Data/Templates/InvoiceReject.html"));
                                            emailTemplate = emailTemplate.Replace("#status#", "hold");
                                            emailTemplate = emailTemplate.Replace("#VendorName#", vendor);
                                        }
                                        string Name = "Dear user";
                                        //Name = "Hi" + " " + UserName.UserFirstName + " " + UserName.UserLastName;
                                        emailTemplate = emailTemplate.Replace("#Name#", Name);
                                        emailTemplate = emailTemplate.Replace("#URL#", ConfigurationManager.AppSettings["MailLink"].ToString() + "/" + model.InvoiceID);

                                        // Mails not to be sent for Finance Approval
                                        invoice.Sendmail(int.Parse(Session["UserID"].ToString()), mailid, model.InvoiceID, " Invoice Details", emailTemplate);
                                    }
                                }
                                sr.Close();
                            }
                        }
                    }



                }
                else
                {
                    mailid = invoice.UpdateInvoiceStatus(model, Convert.ToInt32(Session["UserID"]));
                    if (mailid != 0)
                    {

                        var UserName = usr.getUserDetails().Where(u => u.UserID == mailid).FirstOrDefault();
                        if (model.status == "A")
                        {
                            emailTemplate = MailFunctionality(Server.MapPath("~/App_Data/Templates/InvoiceNotification.html"));
                            emailTemplate = emailTemplate.Replace("#VendorName#", vendor);
                        }
                        else if (model.status == "ReAssign")
                        {
                            emailTemplate = MailFunctionality(Server.MapPath("~/App_Data/Templates/InvoiceReassign.html"));
                            emailTemplate = emailTemplate.Replace("#VendorName#", vendor);
                        }
                        else if (model.status == "R")
                        {
                            emailTemplate = MailFunctionality(Server.MapPath("~/App_Data/Templates/InvoiceReject.html"));
                            emailTemplate = emailTemplate.Replace("#status#", "rejected");
                            emailTemplate = emailTemplate.Replace("#VendorName#", vendor);
                        }
                        else if (model.status == "H")
                        {
                            emailTemplate = MailFunctionality(Server.MapPath("~/App_Data/Templates/InvoiceReject.html"));
                            emailTemplate = emailTemplate.Replace("#status#", "hold");
                            emailTemplate = emailTemplate.Replace("#VendorName#", vendor);
                        }
                        string Name = "Dear user";
                        //Name = "Hi" + " " + UserName.UserFirstName + " " + UserName.UserLastName;
                        emailTemplate = emailTemplate.Replace("#Name#", Name);
                        emailTemplate = emailTemplate.Replace("#URL#", ConfigurationManager.AppSettings["MailLink"].ToString() + "/" + model.InvoiceID);

                        // Mails not to be sent for Finance Approval
                        invoice.Sendmail(int.Parse(Session["UserID"].ToString()), mailid, model.InvoiceID, " Invoice Details", emailTemplate);
                    }
                }
            }
            else
            {
                mailid = invoice.UpdateInvoiceStatus(model, Convert.ToInt32(Session["UserID"]));
                if (mailid != 0)
                {

                    var UserName = usr.getUserDetails().Where(u => u.UserID == mailid).FirstOrDefault();
                    if (model.status == "A")
                    {
                        emailTemplate = MailFunctionality(Server.MapPath("~/App_Data/Templates/InvoiceNotification.html"));
                        emailTemplate = emailTemplate.Replace("#VendorName#", vendor);
                    }
                    else if (model.status == "ReAssign")
                    {
                        emailTemplate = MailFunctionality(Server.MapPath("~/App_Data/Templates/InvoiceReassign.html"));
                        emailTemplate = emailTemplate.Replace("#VendorName#", vendor);
                    }
                    else if (model.status == "R")
                    {
                        emailTemplate = MailFunctionality(Server.MapPath("~/App_Data/Templates/InvoiceReject.html"));
                        emailTemplate = emailTemplate.Replace("#status#", "rejected");
                        emailTemplate = emailTemplate.Replace("#VendorName#", vendor);
                    }
                    else if (model.status == "H")
                    {
                        emailTemplate = MailFunctionality(Server.MapPath("~/App_Data/Templates/InvoiceReject.html"));
                        emailTemplate = emailTemplate.Replace("#status#", "hold");
                        emailTemplate = emailTemplate.Replace("#VendorName#", vendor);
                    }
                    string Name = "Dear user";
                    //Name = "Hi" + " " + UserName.UserFirstName + " " + UserName.UserLastName;
                    emailTemplate = emailTemplate.Replace("#Name#", Name);
                    emailTemplate = emailTemplate.Replace("#URL#", ConfigurationManager.AppSettings["MailLink"].ToString() + "/" + model.InvoiceID);

                    // Mails not to be sent for Finance Approval
                    invoice.Sendmail(int.Parse(Session["UserID"].ToString()), mailid, model.InvoiceID, " Invoice Details", emailTemplate);
                }
            }
            return RedirectToAction("ActiveInvoice", "Invoice");

        }
		private List<string> getLocationNameById(int Id)
		{
			List<string> list = new List<string>();
			con.Open();
			SqlCommand cmd = new SqlCommand("getLocationByLocationId", con);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue("@LocationId", Id);
			using (SqlDataReader sdr = cmd.ExecuteReader())
			{
				while (sdr.Read())
				{
					list.Add(sdr["LocationName"].ToString());
					list.Add(sdr["Code"].ToString());
					list.Add(sdr["Service"].ToString());
				}
			}
			con.Close();
			return list;

		}
		public ActionResult UpdateInvoiceEdited(InvoiceModel model, FormCollection col)
        {
            var mailid = invoice.UpdateInvoiceStatus(model, Convert.ToInt32(Session["UserID"]));
            List<String> ledgerlist = new List<string>();
            //string ledgerName = "";

            // etc.



            if (col.AllKeys.Contains("ledgerlist"))
            {
                model.ledgerlist = col["ledgerlist"];
                //ledgerlist = model.ledgerlist.Split(',').ToList();
                ledgerlist.Add(model.ledgerlist);
            }
            int blist = ((List<int>)Session["BranchList"])[0];
            con.Open();
            SqlCommand cmd = new SqlCommand("GRNDATE", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@InvoiceID", model.InvoiceID);
            cmd.Parameters.AddWithValue("@BranchID", blist);
            var grn = cmd.ExecuteScalar();
            con.Close();

            String VehicleDate = ""; ;
            if (col.AllKeys.Contains("Date"))
            {
                string valuedate = col["Date"].Trim();
                // VehicleDate = DateTime.ParseExact(valuedate, "dd/MM/yyyy", null);
                VehicleDate = valuedate;
            }

            DateTime Invoicedate = DateTime.Now; ;
            if (col.AllKeys.Contains("InvoiceDate"))
            {
                string valuedate = col["InvoiceDate"].Trim();
                Invoicedate = DateTime.ParseExact(valuedate, "dd/MM/yyyy", null);

            }
            DateTime InvoiceReceiveddate = DateTime.Now; ;

            if (col.AllKeys.Contains("InvoiceReceiveddate"))
            {
                string valuedate = col["InvoiceReceiveddate"].Trim();
                InvoiceReceiveddate = DateTime.ParseExact(valuedate, "dd/MM/yyyy", null);

            }


            DateTime InvoiceDueDate = DateTime.Now; ;

            if (col.AllKeys.Contains("InvoiceDueDate"))
            {
                string valuedate = col["InvoiceDueDate"].Trim();
                InvoiceDueDate = DateTime.ParseExact(valuedate, "dd/MM/yyyy", null);

            }

            con.Open();
            cmd = new SqlCommand("GetFinalUser", con);
            cmd.CommandType = CommandType.StoredProcedure;
            Object objFinalUserType = cmd.ExecuteScalar();
            int finaluserid = 0;
            if (objFinalUserType != null)
            {
                finaluserid = (int)objFinalUserType;
            }
            con.Close();
            con.Open();
            cmd = new SqlCommand("getLoginUserType", con);
            int userID = Convert.ToInt32(Session["UserID"]);
            cmd.Parameters.AddWithValue("@UserID", userID);
            cmd.CommandType = CommandType.StoredProcedure;
            var usertype = "";
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {
                    usertype = sdr["UserTypeID"].ToString();
                }

            }
            con.Close();
            //invoice.UpdateInvoiceStatus(model, Convert.ToInt32(Session["UserID"]));
            con.Open();
            cmd = new SqlCommand("sp_UpdateIQInvoiceItem", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@InvoiceId", model.InvoiceID);
            cmd.Parameters.AddWithValue("@InvoiceNumber", model.InvoiceNumber);
            //      int userID = Convert.ToInt32(Session["UserID"]);
            //Double? amount = (String.IsNullOrEmpty(p.Amount) ? (Double?)null : Double.Parse(p.Amount));  @PANNumber

            cmd.Parameters.AddWithValue("@PANNumber", model.PANNumber);
            cmd.Parameters.AddWithValue("@PONumber", model.PONumber);
            cmd.Parameters.AddWithValue("@InvoiceDate", Invoicedate);
            cmd.Parameters.AddWithValue("@InvoiceReceiveddate", InvoiceReceiveddate);
            cmd.Parameters.AddWithValue("@InvoiceDueDate", InvoiceDueDate);
            //cmd.Parameters.AddWithValue("@Amount", Convert.ToDouble(m.InvoiceAmount));
            cmd.Parameters.AddWithValue("@InvoiceAmount", model.InvoiceAmount);
            cmd.Parameters.AddWithValue("@VendorID", model.VendorID);
            cmd.Parameters.AddWithValue("@VendorName", model.VendorName);

            cmd.Parameters.AddWithValue("@CreatedBy", userID);
            //cmd.Parameters.AddWithValue("@filePath", Session["FilePath"].ToString());
            cmd.Parameters.AddWithValue("@GSTIN", model?.GSTIN ?? "");
            //cmd.Parameters.AddWithValue("@PlaceOfSupply", model?.PlaceOfSupply ?? "");
            //cmd.Parameters.AddWithValue("@ReverseCharges", model.ReverseCharges);
            //cmd.Parameters.AddWithValue("@EcommerceGSTIN", model?.EcommerceGSTIN ?? "");
            //cmd.Parameters.AddWithValue("@TaxPercent", model.TaxPercent);
            //cmd.Parameters.AddWithValue("@TaxableValue", model.TaxableValue);
            //cmd.Parameters.AddWithValue("@CessAmount", model.CessAmount);
            cmd.Parameters.AddWithValue("@GateEntryNumber", Regex.Replace(model.GateEntryNumber, "[^0-9.]", ""));
            cmd.Parameters.AddWithValue("@VehicleNumber", model.VehicleNumber);
            // cmd.Parameters.AddWithValue("@VehicleDate", model.Date);
            cmd.Parameters.AddWithValue("@VehicleDate", VehicleDate);
            cmd.Parameters.AddWithValue("@VehicleTime", model.Time);
			cmd.Parameters.AddWithValue("@LocationId",model.LocationId);
            var currentinvoice = Convert.ToInt32(cmd.ExecuteScalar());
            cmd = new SqlCommand("sp_UpdateUserType", con);
            cmd.CommandType = CommandType.StoredProcedure;

            foreach (var usr in model.UserTypes)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@InvoiceId", model.InvoiceID);
                cmd.Parameters.AddWithValue("@UserTypeId", usr.UserTypeID);
                cmd.Parameters.AddWithValue("@SelectedUserId", usr.SelectedUserId);

                cmd.ExecuteScalar();
            }
            //var currentinvoice = 0;
            /*
            cmd = new SqlCommand("sp_DeleteInvoiceItems",con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@InvoiceNumber",model.InvoiceNumber);
            currentinvoice = Convert.ToInt32(cmd.ExecuteScalar());*/
            var jump = 0;
            for (int i = 40, j = 0; i < col.Keys.Count - 9; i += 17 + jump, j++)
            {
                var id = col.GetValue("update" + j)?.AttemptedValue ?? "";
                if (id == "")
                {
                    j++;
                    while (id == "")
                    {
                        id = col.GetValue("update" + j)?.AttemptedValue ?? "";
                        if (id == "")
                            j++;
                    }
                }


                jump = 0;
                //var ledgerValue=  col.GetValue("ledgerlistNames" + j)?.AttemptedValue ?? "";


                string updateid = col.GetValue("update" + j)?.AttemptedValue ?? "";
                var some = updateid;
                if (updateid == "insert")
                {


                    cmd = new SqlCommand("sp_UpdateInvoiceItems", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@InvoiceId", currentinvoice);
                    //cmd.Parameters.AddWithValue("@Id", col.GetValue("id" + j)?.AttemptedValue ?? "");

                    cmd.Parameters.AddWithValue("@Name", col.GetValue("ItemName" + j)?.AttemptedValue ?? "");
                    cmd.Parameters.AddWithValue("@Qty", col.GetValue("qty" + j)?.AttemptedValue ?? "");
                    //cmd.Parameters.AddWithValue("@Rate", col.GetValue("rate" + j)?.AttemptedValue ?? "");
                    cmd.Parameters.AddWithValue("@Amount", col.GetValue("amount" + j)?.AttemptedValue ?? "");
                    cmd.Parameters.AddWithValue("@IGST", col.GetValue("igst" + j)?.AttemptedValue ?? "");
                    cmd.Parameters.AddWithValue("@SGST", col.GetValue("sgst" + j)?.AttemptedValue ?? "");
                    cmd.Parameters.AddWithValue("@CGST", col.GetValue("cgst" + j)?.AttemptedValue ?? "");

                    cmd.Parameters.AddWithValue("@CategoryId", col["category" + j]);
                    cmd.Parameters.AddWithValue("@ProductId", col["namedes-id" + j]);
                    cmd.Parameters.AddWithValue("@UOM", col["uom" + j]);
                    cmd.Parameters.AddWithValue("@HSN", col["hsncode" + j]);
                    cmd.Parameters.AddWithValue("@Price", col["price" + j]);
                    cmd.Parameters.AddWithValue("@IGSTAmount", col["igstamount" + j]);
                    cmd.Parameters.AddWithValue("@SGSTAmount", col["sgstamount" + j]);
                    cmd.Parameters.AddWithValue("@CGSTAmount", col["cgstamount" + j]);
                    cmd.Parameters.AddWithValue("@Total", col["total" + j]);

                    var test = col["grnenable" + j];
                    if (col["grnenable" + j] == "true")
                    {
                        //grnenable
                        jump = 11;
                        cmd.Parameters.AddWithValue("@GRN", true);
                        cmd.Parameters.AddWithValue("@GRNPrice", col["grnprice" + j]);
                        cmd.Parameters.AddWithValue("@GRNQty", col["grnqty" + j]);
                        cmd.Parameters.AddWithValue("@GRNAmount", col["grnamount" + j]);
                        cmd.Parameters.AddWithValue("@GRNIGST", col["grnigst" + j]);
                        cmd.Parameters.AddWithValue("@GRNSGST", col["grnsgst" + j]);
                        cmd.Parameters.AddWithValue("@GRNCGST", col["grncgst" + j]);
                        cmd.Parameters.AddWithValue("@GRNIAmount", col["grnigstamount" + j]);
                        cmd.Parameters.AddWithValue("@GRNSAmount", col["grnsgstamount" + j]);
                        cmd.Parameters.AddWithValue("@GRNCAmount", col["grncgstamount" + j]);
                        cmd.Parameters.AddWithValue("@GRNTotal", col["grntotal" + j]);
                        var invoiceitem = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@GRN", false);
                        cmd.Parameters.AddWithValue("@GRNPrice", col["price" + j]);
                        cmd.Parameters.AddWithValue("@GRNQty", col["qty" + j]);
                        cmd.Parameters.AddWithValue("@GRNAmount", col["amount" + j]);
                        cmd.Parameters.AddWithValue("@GRNIGST", col["igst" + j]);
                        cmd.Parameters.AddWithValue("@GRNSGST", col["sgst" + j]);
                        cmd.Parameters.AddWithValue("@GRNCGST", col["cgst" + j]);
                        cmd.Parameters.AddWithValue("@GRNIAmount", col["igstamount" + j]);
                        cmd.Parameters.AddWithValue("@GRNSAmount", col["sgstamount" + j]);
                        cmd.Parameters.AddWithValue("@GRNCAmount", col["cgstamount" + j]);
                        cmd.Parameters.AddWithValue("@GRNTotal", col["total" + j]);
                        var invoiceitem = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                }
                else
                {
                    cmd = new SqlCommand("sp_UpdateItemsNew", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@UpdateId", updateid);
                    cmd.Parameters.AddWithValue("@Name", col["ItemName" + j]);
                    cmd.Parameters.AddWithValue("@Qty", col["qty" + j]);
                    cmd.Parameters.AddWithValue("@Amount", col["amount" + j]);
                    cmd.Parameters.AddWithValue("@SGST", col["sgst" + j]);
                    cmd.Parameters.AddWithValue("@IGST", col["igst" + j]);
                    cmd.Parameters.AddWithValue("@CGST", col["cgst" + j]);
                    cmd.Parameters.AddWithValue("@InvoiceId", currentinvoice);

                    cmd.Parameters.AddWithValue("@CategoryId", col["category" + j]);
                    cmd.Parameters.AddWithValue("@ProductId", col["namedes-id" + j]);
                    cmd.Parameters.AddWithValue("@UOM", col["uom" + j]);
                    cmd.Parameters.AddWithValue("@HSN", col["hsncode" + j]);
                    cmd.Parameters.AddWithValue("@Price", col["price" + j]);
                    cmd.Parameters.AddWithValue("@IGSTAmount", col["igstamount" + j]);
                    cmd.Parameters.AddWithValue("@SGSTAmount", col["sgstamount" + j]);
                    cmd.Parameters.AddWithValue("@CGSTAmount", col["cgstamount" + j]);
                    cmd.Parameters.AddWithValue("@Total", col["total" + j]);
                    var test = col["grnenable" + j];
                    if (col["grnenable" + j] == "true")
                    {
                        //grnenable
                        jump = 11;
                        cmd.Parameters.AddWithValue("@GRN", true);
                        cmd.Parameters.AddWithValue("@GRNPrice", col["grnprice" + j]);
                        cmd.Parameters.AddWithValue("@GRNQty", col["grnqty" + j]);
                        cmd.Parameters.AddWithValue("@GRNAmount", col["grnamount" + j]);
                        cmd.Parameters.AddWithValue("@GRNIGST", col["grnigst" + j]);
                        cmd.Parameters.AddWithValue("@GRNSGST", col["grnsgst" + j]);
                        cmd.Parameters.AddWithValue("@GRNCGST", col["grncgst" + j]);
                        cmd.Parameters.AddWithValue("@GRNIAmount", col["grnigstamount" + j]);
                        cmd.Parameters.AddWithValue("@GRNSAmount", col["grnsgstamount" + j]);
                        cmd.Parameters.AddWithValue("@GRNCAmount", col["grncgstamount" + j]);
                        cmd.Parameters.AddWithValue("@GRNTotal", col["grntotal" + j]);
                        var invoiceitem = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@GRN", false);
                        cmd.Parameters.AddWithValue("@GRNPrice", col["price" + j]);
                        cmd.Parameters.AddWithValue("@GRNQty", col["qty" + j]);
                        cmd.Parameters.AddWithValue("@GRNAmount", col["amount" + j]);
                        cmd.Parameters.AddWithValue("@GRNIGST", col["igst" + j]);
                        cmd.Parameters.AddWithValue("@GRNSGST", col["sgst" + j]);
                        cmd.Parameters.AddWithValue("@GRNCGST", col["cgst" + j]);
                        cmd.Parameters.AddWithValue("@GRNIAmount", col["igstamount" + j]);
                        cmd.Parameters.AddWithValue("@GRNSAmount", col["sgstamount" + j]);
                        cmd.Parameters.AddWithValue("@GRNCAmount", col["cgstamount" + j]);
                        cmd.Parameters.AddWithValue("@GRNTotal", col["total" + j]);
                        var invoiceitem = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    /*
                    cmd.Parameters.AddWithValue("@UpdateId", updateid);
                    cmd.Parameters.AddWithValue("@InvoiceId", currentinvoice);
                    cmd.Parameters.AddWithValue("@ItemId", col.GetValue("id" + j)?.AttemptedValue ?? "");
                    cmd.Parameters.AddWithValue("@Code", col.GetValue("code" + j)?.AttemptedValue ?? "");
                    cmd.Parameters.AddWithValue("@Name", col.GetValue("name" + j)?.AttemptedValue ?? "");
                    cmd.Parameters.AddWithValue("@Qty", col.GetValue("qty" + j)?.AttemptedValue ?? "");
                    cmd.Parameters.AddWithValue("@Rate", col.GetValue("rate" + j)?.AttemptedValue ?? "");
                    cmd.Parameters.AddWithValue("@Amount", col.GetValue("amount" + j)?.AttemptedValue ?? "");
                    cmd.Parameters.AddWithValue("@IGST", col.GetValue("igst" + j)?.AttemptedValue ?? "");
                    cmd.Parameters.AddWithValue("@SGST", col.GetValue("sgst" + j)?.AttemptedValue ?? "");
                    cmd.Parameters.AddWithValue("@CGST", col.GetValue("cgst" + j)?.AttemptedValue ?? "");
                    cmd.Parameters.AddWithValue("@ledgerValue", ledgerValue);*/
                    //var invoiceitem = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            con.Close();

            DataTable dtledger = new DataTable();
            dtledger.Columns.Add("InvoiceId", typeof(Int32));
            dtledger.Columns.Add("LedgerName", typeof(string));

            if (ledgerlist.Count > 0)
            {
                foreach (var item in ledgerlist)
                {
                    dtledger.Rows.Add(model.InvoiceID, item);
                }

                con.Open();
                SqlCommand cmdLedger = new SqlCommand("AddUpdateledgerInvoice", con);
                cmdLedger.CommandType = CommandType.StoredProcedure;

                //Pass table Valued parameter to Store Procedure
                SqlParameter sqlParam = cmdLedger.Parameters.AddWithValue("@ledger", dtledger);
                sqlParam.SqlDbType = SqlDbType.Structured;
                cmdLedger.ExecuteNonQuery();
                con.Close();
            }



            //}



            return RedirectToAction("ActiveInvoice", "Invoice");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public ActionResult GenerateCrDrNote(InvoiceModel model, FormCollection col)
        {
            JsonResult resp = Json(new { success = 0, noteslist = "" }, JsonRequestBehavior.AllowGet);
            try
            {
                List<String> ledgerlist = new List<string>();

                if (col.AllKeys.Contains("ledgerlist"))
                {
                    var value = col["ledgerlist"];

                    ledgerlist = value.Split(',').ToList();
                }

                //con.Open();
                //SqlCommand cmd = new SqlCommand("GetFinalUser", con);
                //cmd.CommandType = CommandType.StoredProcedure;
                //Object objFinalUserType = cmd.ExecuteScalar();
                //int finaluserid = 0;
                //if (objFinalUserType != null)
                //{
                //    finaluserid = (int)objFinalUserType;
                //}
                //con.Close();
                //con.Open();
                //cmd = new SqlCommand("getLoginUserType", con);
                //int userID = Convert.ToInt32(Session["UserID"]);
                //cmd.Parameters.AddWithValue("@UserID", userID);
                //cmd.CommandType = CommandType.StoredProcedure;
                //var usertype = "";
                //using (SqlDataReader sdr = cmd.ExecuteReader())
                //{
                //    while (sdr.Read())
                //    {
                //        usertype = sdr["UserTypeID"].ToString();
                //    }

                //}
                //con.Close();


                int userID = Convert.ToInt32(Session["UserID"]);


                //invoice.UpdateInvoiceStatus(model, Convert.ToInt32(Session["UserID"]));
                con.Open();
                //cmd = new SqlCommand("sp_UpdateIQInvoiceItem", con);
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@InvoiceId", model.InvoiceID);
                //cmd.Parameters.AddWithValue("@InvoiceNumber", model.InvoiceNumber);
                ////      int userID = Convert.ToInt32(Session["UserID"]);
                ////Double? amount = (String.IsNullOrEmpty(p.Amount) ? (Double?)null : Double.Parse(p.Amount));  @PANNumber

                //cmd.Parameters.AddWithValue("@PANNumber", model.PANNumber);
                //cmd.Parameters.AddWithValue("@PONumber", model.PONumber);
                //cmd.Parameters.AddWithValue("@InvoiceDate", model.InvoiceDate);
                //cmd.Parameters.AddWithValue("@InvoiceReceiveddate", model.InvoiceReceiveddate);
                //cmd.Parameters.AddWithValue("@InvoiceDueDate", model.InvoiceDueDate);
                ////cmd.Parameters.AddWithValue("@Amount", Convert.ToDouble(m.InvoiceAmount));
                //cmd.Parameters.AddWithValue("@InvoiceAmount", model.InvoiceAmount);
                //cmd.Parameters.AddWithValue("@VendorID", model.VendorID);
                //cmd.Parameters.AddWithValue("@VendorName", model.VendorName);

                //cmd.Parameters.AddWithValue("@CreatedBy", userID);
                ////cmd.Parameters.AddWithValue("@filePath", Session["FilePath"].ToString());
                //cmd.Parameters.AddWithValue("@GSTIN", model.GSTIN);
                //cmd.Parameters.AddWithValue("@PlaceOfSupply", model.PlaceOfSupply);
                //cmd.Parameters.AddWithValue("@ReverseCharges", model.ReverseCharges);
                //cmd.Parameters.AddWithValue("@EcommerceGSTIN", model.EcommerceGSTIN);
                //cmd.Parameters.AddWithValue("@TaxPercent", model.TaxPercent);
                //cmd.Parameters.AddWithValue("@TaxableValue", model.TaxableValue);
                //cmd.Parameters.AddWithValue("@CessAmount", model.CessAmount);
                //cmd.Parameters.AddWithValue("@GateEntryNumber", model.GateEntryNumber);
                //cmd.Parameters.AddWithValue("@VehicleNumber", model.VehicleNumber);
                //cmd.Parameters.AddWithValue("@VehicleDate", model.Date);
                //cmd.Parameters.AddWithValue("@VehicleTime", model.Time);
                //var currentinvoice = Convert.ToInt32(cmd.ExecuteScalar());
                //cmd = new SqlCommand("sp_UpdateUserType", con);
                //cmd.CommandType = CommandType.StoredProcedure;

                //foreach (var usr in model.UserTypes)
                //{
                //    cmd.Parameters.Clear();
                //    cmd.Parameters.AddWithValue("@InvoiceId", model.InvoiceID);
                //    cmd.Parameters.AddWithValue("@UserTypeId", usr.UserTypeID);
                //    cmd.Parameters.AddWithValue("@SelectedUserId", usr.SelectedUserId);

                //    cmd.ExecuteScalar();
                //}
                //var currentinvoice = 0;
                /*
                cmd = new SqlCommand("sp_DeleteInvoiceItems",con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@InvoiceNumber",model.InvoiceNumber);
                currentinvoice = Convert.ToInt32(cmd.ExecuteScalar());*/
                var jump = 0;
                for (int i = 39, j = 0; i < col.Keys.Count - 9; i += 17 + jump, j++)
                {
                    var id = col.GetValue("id" + j)?.AttemptedValue ?? "";
                    if (id == "")
                    {
                        //j++;
                    }


                    jump = 0;
                    //var ledgerValue=  col.GetValue("ledgerlistNames" + j)?.AttemptedValue ?? "";


                    string updateid = col.GetValue("update" + j)?.AttemptedValue ?? "";
                    var some = updateid;
                    if (updateid == "insert")
                    {


                        SqlCommand cmd = new SqlCommand("sp_UpdateInvoiceItems", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@InvoiceId", model.InvoiceID);
                        //cmd.Parameters.AddWithValue("@Id", col.GetValue("id" + j)?.AttemptedValue ?? "");

                        cmd.Parameters.AddWithValue("@Name", col.GetValue("ItemName" + j)?.AttemptedValue ?? "");
                        cmd.Parameters.AddWithValue("@Qty", col.GetValue("qty" + j)?.AttemptedValue ?? "");
                        //cmd.Parameters.AddWithValue("@Rate", col.GetValue("rate" + j)?.AttemptedValue ?? "");
                        cmd.Parameters.AddWithValue("@Amount", col.GetValue("amount" + j)?.AttemptedValue ?? "");
                        cmd.Parameters.AddWithValue("@IGST", col.GetValue("igst" + j)?.AttemptedValue ?? "");
                        cmd.Parameters.AddWithValue("@SGST", col.GetValue("sgst" + j)?.AttemptedValue ?? "");
                        cmd.Parameters.AddWithValue("@CGST", col.GetValue("cgst" + j)?.AttemptedValue ?? "");

                        cmd.Parameters.AddWithValue("@CategoryId", col["category" + j]);
                        cmd.Parameters.AddWithValue("@ProductId", col["namedes-id" + j]);
                        cmd.Parameters.AddWithValue("@UOM", col["uom" + j]);
                        cmd.Parameters.AddWithValue("@HSN", col["hsncode" + j]);
                        cmd.Parameters.AddWithValue("@Price", col["price" + j]);
                        cmd.Parameters.AddWithValue("@IGSTAmount", col["igstamount" + j]);
                        cmd.Parameters.AddWithValue("@SGSTAmount", col["sgstamount" + j]);
                        cmd.Parameters.AddWithValue("@CGSTAmount", col["cgstamount" + j]);
                        cmd.Parameters.AddWithValue("@Total", col["total" + j]);

                        var test = col["grnenable" + j];
                        if (col["grnenable" + j] == "true")
                        {
                            //grnenable
                            jump = 11;
                            cmd.Parameters.AddWithValue("@GRN", true);
                            cmd.Parameters.AddWithValue("@GRNPrice", col["grnprice" + j]);
                            cmd.Parameters.AddWithValue("@GRNQty", col["grnqty" + j]);
                            cmd.Parameters.AddWithValue("@GRNAmount", col["grnamount" + j]);
                            cmd.Parameters.AddWithValue("@GRNIGST", col["grnigst" + j]);
                            cmd.Parameters.AddWithValue("@GRNSGST", col["grnsgst" + j]);
                            cmd.Parameters.AddWithValue("@GRNCGST", col["grncgst" + j]);
                            cmd.Parameters.AddWithValue("@GRNIAmount", col["grnigstamount" + j]);
                            cmd.Parameters.AddWithValue("@GRNSAmount", col["grnsgstamount" + j]);
                            cmd.Parameters.AddWithValue("@GRNCAmount", col["grncgstamount" + j]);
                            cmd.Parameters.AddWithValue("@GRNTotal", col["grntotal" + j]);
                            var invoiceitem = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@GRN", false);
                            cmd.Parameters.AddWithValue("@GRNPrice", col["price" + j]);
                            cmd.Parameters.AddWithValue("@GRNQty", col["qty" + j]);
                            cmd.Parameters.AddWithValue("@GRNAmount", col["amount" + j]);
                            cmd.Parameters.AddWithValue("@GRNIGST", col["igst" + j]);
                            cmd.Parameters.AddWithValue("@GRNSGST", col["sgst" + j]);
                            cmd.Parameters.AddWithValue("@GRNCGST", col["cgst" + j]);
                            cmd.Parameters.AddWithValue("@GRNIAmount", col["igstamount" + j]);
                            cmd.Parameters.AddWithValue("@GRNSAmount", col["sgstamount" + j]);
                            cmd.Parameters.AddWithValue("@GRNCAmount", col["cgstamount" + j]);
                            cmd.Parameters.AddWithValue("@GRNTotal", col["total" + j]);
                            var invoiceitem = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                    }
                    else
                    {
                        SqlCommand cmd = new SqlCommand("sp_UpdateItemsNew", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@UpdateId", updateid);
                        cmd.Parameters.AddWithValue("@Name", col["ItemName" + j]);
                        cmd.Parameters.AddWithValue("@Qty", col["qty" + j]);
                        cmd.Parameters.AddWithValue("@Amount", col["amount" + j]);
                        cmd.Parameters.AddWithValue("@SGST", col["sgst" + j]);
                        cmd.Parameters.AddWithValue("@IGST", col["igst" + j]);
                        cmd.Parameters.AddWithValue("@CGST", col["cgst" + j]);
                        cmd.Parameters.AddWithValue("@InvoiceId", model.InvoiceID);

                        cmd.Parameters.AddWithValue("@CategoryId", col["category" + j]);
                        cmd.Parameters.AddWithValue("@ProductId", col["namedes-id" + j]);
                        cmd.Parameters.AddWithValue("@UOM", col["uom" + j]);
                        cmd.Parameters.AddWithValue("@HSN", col["hsncode" + j]);
                        cmd.Parameters.AddWithValue("@Price", col["price" + j]);
                        cmd.Parameters.AddWithValue("@IGSTAmount", col["igstamount" + j]);
                        cmd.Parameters.AddWithValue("@SGSTAmount", col["sgstamount" + j]);
                        cmd.Parameters.AddWithValue("@CGSTAmount", col["cgstamount" + j]);
                        cmd.Parameters.AddWithValue("@Total", col["total" + j]);
                        var test = col["grnenable" + j];
                        if (col["grnenable" + j] == "true")
                        {
                            //grnenable
                            jump = 11;
                            cmd.Parameters.AddWithValue("@GRN", true);
                            cmd.Parameters.AddWithValue("@GRNPrice", col["grnprice" + j]);
                            cmd.Parameters.AddWithValue("@GRNQty", col["grnqty" + j]);
                            cmd.Parameters.AddWithValue("@GRNAmount", col["grnamount" + j]);
                            cmd.Parameters.AddWithValue("@GRNIGST", col["grnigst" + j]);
                            cmd.Parameters.AddWithValue("@GRNSGST", col["grnsgst" + j]);
                            cmd.Parameters.AddWithValue("@GRNCGST", col["grncgst" + j]);
                            cmd.Parameters.AddWithValue("@GRNIAmount", col["grnigstamount" + j]);
                            cmd.Parameters.AddWithValue("@GRNSAmount", col["grnsgstamount" + j]);
                            cmd.Parameters.AddWithValue("@GRNCAmount", col["grncgstamount" + j]);
                            cmd.Parameters.AddWithValue("@GRNTotal", col["grntotal" + j]);
                            var invoiceitem = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@GRN", false);
                            cmd.Parameters.AddWithValue("@GRNPrice", col["price" + j]);
                            cmd.Parameters.AddWithValue("@GRNQty", col["qty" + j]);
                            cmd.Parameters.AddWithValue("@GRNAmount", col["amount" + j]);
                            cmd.Parameters.AddWithValue("@GRNIGST", col["igst" + j]);
                            cmd.Parameters.AddWithValue("@GRNSGST", col["sgst" + j]);
                            cmd.Parameters.AddWithValue("@GRNCGST", col["cgst" + j]);
                            cmd.Parameters.AddWithValue("@GRNIAmount", col["igstamount" + j]);
                            cmd.Parameters.AddWithValue("@GRNSAmount", col["sgstamount" + j]);
                            cmd.Parameters.AddWithValue("@GRNCAmount", col["cgstamount" + j]);
                            cmd.Parameters.AddWithValue("@GRNTotal", col["total" + j]);
                            var invoiceitem = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        /*
                        cmd.Parameters.AddWithValue("@UpdateId", updateid);
                        cmd.Parameters.AddWithValue("@InvoiceId", currentinvoice);
                        cmd.Parameters.AddWithValue("@ItemId", col.GetValue("id" + j)?.AttemptedValue ?? "");
                        cmd.Parameters.AddWithValue("@Code", col.GetValue("code" + j)?.AttemptedValue ?? "");
                        cmd.Parameters.AddWithValue("@Name", col.GetValue("name" + j)?.AttemptedValue ?? "");
                        cmd.Parameters.AddWithValue("@Qty", col.GetValue("qty" + j)?.AttemptedValue ?? "");
                        cmd.Parameters.AddWithValue("@Rate", col.GetValue("rate" + j)?.AttemptedValue ?? "");
                        cmd.Parameters.AddWithValue("@Amount", col.GetValue("amount" + j)?.AttemptedValue ?? "");
                        cmd.Parameters.AddWithValue("@IGST", col.GetValue("igst" + j)?.AttemptedValue ?? "");
                        cmd.Parameters.AddWithValue("@SGST", col.GetValue("sgst" + j)?.AttemptedValue ?? "");
                        cmd.Parameters.AddWithValue("@CGST", col.GetValue("cgst" + j)?.AttemptedValue ?? "");
                        cmd.Parameters.AddWithValue("@ledgerValue", ledgerValue);*/
                        //var invoiceitem = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
                con.Close();

                DataTable dtledger = new DataTable();
                dtledger.Columns.Add("InvoiceId", typeof(Int32));
                dtledger.Columns.Add("LedgerName", typeof(string));

                if (ledgerlist.Count > 0)
                {
                    foreach (var item in ledgerlist)
                    {
                        dtledger.Rows.Add(model.InvoiceID, item);
                    }

                    con.Open();
                    SqlCommand cmdLedger = new SqlCommand("AddUpdateledgerInvoice", con);
                    cmdLedger.CommandType = CommandType.StoredProcedure;

                    //Pass table Valued parameter to Store Procedure
                    SqlParameter sqlParam = cmdLedger.Parameters.AddWithValue("@ledger", dtledger);
                    sqlParam.SqlDbType = SqlDbType.Structured;
                    cmdLedger.ExecuteNonQuery();
                    con.Close();
                }

                con.Open();
                SqlCommand cmdGenerate = new SqlCommand("usp_AddCrDrNote", con);
                cmdGenerate.CommandType = CommandType.StoredProcedure;

                cmdGenerate.Parameters.AddWithValue("@InvoiceID", model.InvoiceID);
                cmdGenerate.Parameters.AddWithValue("@UserID", userID);
                ////Pass table Valued parameter to Store Procedure
                //SqlParameter sqlParamInv = cmdGenerate.Parameters.AddWithValue("@InvoiceID", model.InvoiceID);
                //sqlParamInv.SqlDbType = SqlDbType.Structured;

                //SqlParameter sqlParamUserID = cmdGenerate.Parameters.AddWithValue("@UserID", userID);
                //sqlParamUserID.SqlDbType = SqlDbType.Structured;
                cmdGenerate.ExecuteNonQuery();
                con.Close();

                con.Open();
                SqlCommand cmdNotes = new SqlCommand("usp_getCreditDebitNotes", con);
                cmdNotes.CommandType = CommandType.StoredProcedure;
                cmdNotes.Parameters.AddWithValue("@InvoiceId", model.InvoiceID);

                //List<CreditDebitNotes> objNotesList = new List<CreditDebitNotes>();
                //using (SqlDataReader sdr = cmdNotes.ExecuteReader())
                //{
                //    while (sdr.Read())
                //    {
                //        CreditDebitNotes crDrNotes = new CreditDebitNotes();
                //        crDrNotes.CrDrNoteID = Convert.ToInt32(sdr["CrDrNoteID"]);
                //        crDrNotes.InvoiceID = Convert.ToInt32(sdr["InvoiceID"]);
                //        crDrNotes.NoteType = sdr["NoteType"].ToString();
                //        objNotesList.Add(crDrNotes);

                //    }
                //    model.CreditDebitNotes = objNotesList;
                //}

                DataTable dt = new DataTable();

                dt.Load(cmdNotes.ExecuteReader());
                con.Close();


                string noteslist;
                noteslist = JsonConvert.SerializeObject(dt);
                resp = Json(new { success = 1, noteslist }, JsonRequestBehavior.AllowGet);
                //}

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
            return resp;
        }

        /// <summary>
        /// get the list of Commnets for Commnets Section
        /// </summary>
        /// <param name="invoiceno"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult getcomments(string invoiceno)
        {
            JsonResult resp = Json(new { success = 0, commentlist = "" }, JsonRequestBehavior.AllowGet);
            try
            {
                con.Open();
                int invoiceid = Convert.ToInt32(TempData["InvoiceId"]);
                ///   SqlCommand cmd = new SqlCommand("SP_GetCommentsbyInvoiceID", con);
                SqlCommand cmd = new SqlCommand("SP_GetCommentsbyInvoiceID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@InvoiceId", invoiceid);
                //        DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                dt.Load(cmd.ExecuteReader());
                con.Close();
                Session["InvoiceID"] = null;
                //   var commentlist = dt.AsEnumerable().ToList();
                string commentlist;
                commentlist = JsonConvert.SerializeObject(dt);
                resp = Json(new { success = 1, commentlist }, JsonRequestBehavior.AllowGet);
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
            return resp;
        }

        /// <summary>
        /// Get the Vendor Details
        /// </summary>
        /// <returns></returns>
        public JsonResult getvendor()
        {
            VendorRepository vendor = new VendorRepository();
            return Json(vendor.getVendorDetails()
                .Select(d => new
                {
                    id = d.VendorID,
                    text = d.VendorName
                }).Distinct().ToList(), JsonRequestBehavior.AllowGet);
        }

        List<string> imglist = new List<string>();
      
        /// <summary>
        /// Get the List of File Uploaded 
        /// </summary>
        /// <param name="formdata"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NewgetImageLists(dynamic formdata)
        {
            try
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
            }
            catch (Exception ex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
            }
            return Json(new { imglist }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Call to OCR method to read data and Bind to XML
        /// </summary>
        /// <returns></returns>
        /* public ActionResult BindPdfValue()
         {
             //Logger.Write("Reached inside OCR Method");
             System.Diagnostics.ProcessStartInfo objProcess = new System.Diagnostics.ProcessStartInfo();
             string outp = "";
             string fpath = string.Empty;
             using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
             using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
             {
                 using (Process process = new Process())
                 {
                     // preparing ProcessStartInfo
                     process.StartInfo.FileName = Server.MapPath("~/OCRExtratorApp/OCRExtractor.bat");
                     fpath = Server.MapPath("~/Files/Temp/") + Session["Hit"] + "\\" + Session["file"];

                     ///fpath = ("/Temp/") + Session["Hit"] + "\\" + Session["file"];
                     Session["FilePath"] = fpath;
                     process.StartInfo.Arguments = String.Format("{0} {1}", fpath, " XML");
                     //process.StartInfo.Arguments = String.Format("{0} {1}", Server.MapPath("~/Files/Temp/") + Session["Hit"] + "\\" + Session["file"], " XML");

                     process.StartInfo.WorkingDirectory = Server.MapPath("~/OCRExtratorApp");
                     process.StartInfo.UseShellExecute = false;
                     process.StartInfo.RedirectStandardOutput = true;
                     process.StartInfo.RedirectStandardError = true;
                     StringBuilder output = new StringBuilder();
                     StringBuilder error = new StringBuilder();
                     try
                     {
                         process.OutputDataReceived += (sender, e) =>
                         {
                             if (e.Data == null)
                             {
                                 outputWaitHandle.Set();
                             }
                             else
                             {
                                 output.AppendLine(e.Data);
                             }
                         };
                         process.ErrorDataReceived += (sender, e) =>
                         {
                             if (e.Data == null)
                             {
                                 errorWaitHandle.Set();
                             }
                             else
                             {
                                 output.AppendLine(e.Data);
                             }
                         };

                         process.Start();

                         process.BeginOutputReadLine();
                         process.BeginErrorReadLine();

                         if (process.WaitForExit(100000) &&
              outputWaitHandle.WaitOne(100000) &&
              errorWaitHandle.WaitOne(100000))

                         {
                             // Process completed. Check process.ExitCode here.
                         }
                         else
                         {
                             // Timed out.
                         }

                         outp = output.ToString().Replace("Empty page!!", "");
                         outp = outp.Remove(outp.LastIndexOf(Environment.NewLine));
                         //Logger.Write("going to write output");
                         //Logger.Write(outp);
                     }
                     finally
                     {

                     }
                 }
             }

             InvoiceModel objInvoice = new InvoiceModel();
             objInvoice = ReadXMLInvoice(outp);
             objInvoice.UserTypes = (List<UserTypes>)TempData["UserTypes"];
             ViewBag.Users = TempData["ReAssignUsers"];
             //   return View("Index", objInvoice);
             objInvoice.filePath = fpath;
             Session["Filepath"] = "~/Files/Temp/" + Session["Hit"] + "\\" + Session["file"];
             return Json(objInvoice, JsonRequestBehavior.AllowGet);
         }*/


        public ActionResult BindPdfValue()
        {
            InvoiceModel objinv = new InvoiceModel();

            #region Extractor


            string outp = "";
            string fPath = Server.MapPath("~/Files/Temp/") + Session["Hit"] + "\\" + Session["file"];
            //string url = System.Web.Configuration.WebConfigurationManager.AppSettings["WebServiceURL"].ToString();
            //    string url = "http://192.168.1.99:8080/OCRRestService/textcapture/extractinvoice/upload";
            //string url = "http://localhost:8080/fileuploader/integrated.php";
            //string url = "http://192.168.2.140/iqinvoiceapi/integrated.php";
            //string url = "http://192.168.2.140/iqinvoiceapi/final.php";
            string url = System.Web.Configuration.WebConfigurationManager.AppSettings["CaptureURL"].ToString();
            string[] fPatharray = { fPath };
            // fPatharray[0] = fPath;
            outp = UploadFilesToRemoteUrl(url, fPatharray);

            string output = outp;

            if (output.Equals("<?xml version=\"1.0\" encoding=\"utf-8\"?><INVOICE><ITEMS></ITEMS></INVOICE>"))
            {
                outp = "<INVOICE><INVOICENO></INVOICENO><AMOUNT></AMOUNT><INVOICEDATE></INVOICEDATE><PO></PO><PAN></PAN><GSTIN></GSTIN><VENDOR></VENDOR><ITEMS></ITEMS></INVOICE>";
            }
            if (output.Equals(""))
            {
                outp = "<INVOICE><INVOICENO></INVOICENO><AMOUNT></AMOUNT><INVOICEDATE></INVOICEDATE><PO></PO><PAN></PAN><GSTIN></GSTIN><VENDOR></VENDOR><ITEMS></ITEMS></INVOICE>";
            }
            if (output.Equals("Error: Cannot create object"))
            {
                outp = "<INVOICE><INVOICENO></INVOICENO><AMOUNT></AMOUNT><INVOICEDATE></INVOICEDATE><PO></PO><PAN></PAN><GSTIN></GSTIN><VENDOR></VENDOR><ITEMS></ITEMS></INVOICE>";
            }
            string op = "";
            objinv = ReadXMLInvoice(outp);

            var VendorId = "";
            var VendorName = "";
            var GSTIN = "";
            SqlCommand cmd = new SqlCommand("sp_checkvendorexist", con);
            if (objinv.VendorName != null)
            {
                cmd.Parameters.AddWithValue("@VendorName", objinv.VendorName);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        if (sdr["VendorID"] != DBNull.Value)
                            VendorId = sdr["VendorID"].ToString();
                        if (sdr["VendorName"] != DBNull.Value)
                            VendorName = sdr["VendorName"].ToString();
                        if (sdr["GSTIN_UIN"] != DBNull.Value)
                            objinv.GSTIN = sdr["GSTIN_UIN"].ToString();

                    }

                }
                con.Close();
            }
            if (VendorId == "")
            {
                objinv.VendorName = "";
            }
            else
            {
                objinv.VendorName = VendorName;
            }
            objinv.filePath = fPath;
            Session["Filepath"] = fPath;
            //     objVd.ImageFilePath = Server.MapPath("~/Files/Temp/") + Session["Hit"] + "\\" + Session["file"];
            //return Json(objVd, JsonRequestBehavior.AllowGet);

            //return Json(output, JsonRequestBehavior.AllowGet);

            return Json(objinv, JsonRequestBehavior.AllowGet);

            #endregion


        }

        public static string UploadFilesToRemoteUrl(string url, string[] files, NameValueCollection formFields = null)
        {
            try
            {
                string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                request.Method = "POST";
                request.KeepAlive = true;
                Stream memStream = new System.IO.MemoryStream();
                var boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                var endBoundaryBytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--");


                string formdataTemplate = "\r\n--" + boundary +
                                            "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";

                if (formFields != null)
                {
                    foreach (string key in formFields.Keys)
                    {
                        string formitem = string.Format(formdataTemplate, key, formFields[key]);
                        byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                        memStream.Write(formitembytes, 0, formitembytes.Length);
                    }
                }

                /*string headerTemplate =
                    "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                    "Content-Type: application/vnd.pdf\r\n\r\n";*/
                string headerTemplate =
                   "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                   "Content-Type: application/vnd.pdf\r\n\r\n";

                for (int i = 0; i < files.Length; i++)
                {
                    memStream.Write(boundarybytes, 0, boundarybytes.Length);
                    var header = string.Format(headerTemplate, "image", files[i]);
                    var headerbytes = System.Text.Encoding.UTF8.GetBytes(header);

                    memStream.Write(headerbytes, 0, headerbytes.Length);

                    using (var fileStream = new FileStream(files[i], FileMode.Open, FileAccess.Read))
                    {
                        var buffer = new byte[1024];
                        var bytesRead = 0;
                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            memStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }

                memStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
                request.ContentLength = memStream.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    memStream.Position = 0;
                    byte[] tempBuffer = new byte[memStream.Length];
                    memStream.Read(tempBuffer, 0, tempBuffer.Length);
                    memStream.Close();
                    requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                }

                // XDocument doc;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        // doc = XDocument.Load(stream);
                        using (var streamRdr = new StreamReader(stream))
                        {
                            var resp = streamRdr.ReadToEnd();

                            response.Close();

                            return resp;
                        }
                    }
                }

            }
            catch (WebException webex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(webex.Message);
                /*
                WebResponse errResp = webex.Response;
                using (Stream respStream = errResp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(respStream);
                    string text = reader.ReadToEnd();
                }*/
            }
            return "";
        }

        /// <summary>
        /// Read data from XML string and Bind to model
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public InvoiceModel ReadXMLInvoice(string xmlString)
        {
            InvoiceModel objInvoice = new InvoiceModel();
            try
            {
                Exeptionfrom = "InvoiceController/ReadXMLInvoice";
                XmlDocument xmlInputItem = new XmlDocument();
                xmlString = xmlString.Replace("&", "&amp;");
                xmlInputItem.LoadXml(xmlString);
                XmlNodeList xmlItems;
                xmlItems = xmlInputItem.SelectNodes("INVOICE/ITEMS/ITEM");
                DateTime dateValue;
                DateTime? date = DateTime.Now;
                DateTime? invRcvdate = null;
                DateTime? invDuedate = null;
                //create the DataTable that will hold the data

                XmlNode xmlVendor = xmlInputItem.SelectSingleNode("INVOICE");
                objInvoice.VendorName = xmlVendor.SelectSingleNode("VENDOR")?.InnerText.Trim() ?? "";
                objInvoice.InvoiceNumber = xmlVendor.SelectSingleNode("INVOICENO")?.InnerText.Trim() ?? "";
                var envdate = xmlVendor.SelectSingleNode("INVOICEDATE")?.InnerText.Trim().Replace("Invoice Date: ", "") ?? "";
                if (DateTime.TryParse(envdate, out dateValue))
                {
                    date = dateValue;
                }
                objInvoice.InvoiceDate = date;/// xmlVendor.SelectSingleNode("INVOICEDATE").InnerText.Trim();
                objInvoice.PONumber = xmlVendor.SelectSingleNode("PO")?.InnerText.Trim() ?? ""; ;
                ///      objInvoice.InvoiceReceiveddate = xmlVendor.SelectSingleNode("PO_DATE").InnerText.Trim(); ;
                objInvoice.InvoiceAmount = xmlVendor.SelectSingleNode("AMOUNT")?.InnerText.Trim().Replace(",", "") ?? ""; ;
                objInvoice.PANNumber = xmlVendor.SelectSingleNode("PAN")?.InnerText?.Trim() ?? ""; ;
                objInvoice.GSTIN = xmlVendor.SelectSingleNode("GSTIN")?.InnerText?.Trim().Replace("GSTIN/UIN: ", "") ?? ""; ;
                //GSTIN/UIN: 
                objInvoice.InvoiceItems = new List<InvoiceItem>();

                foreach (XmlNode x in xmlItems)
                {
                    InvoiceItem prd = new InvoiceItem();
                    //   prd.ProductId = tempId++;
                    if (x.Attributes["Id"] != null)
                    {
                        prd.ItemId = x.Attributes["Id"].InnerText.Replace(",", "").Trim();

                    }

                    else
                        prd.ItemId = string.Empty;
                    // prd.Price = double.Parse(x.Attributes["PRICE"].InnerText);
                    if (x.Attributes["Code"] != null)
                        prd.Code = x.Attributes["Code"].InnerText.Replace(",", "").Trim();
                    else
                        prd.Code = string.Empty;
                    //prd.Qty =  Int32.Parse(x.Attributes["QTY"].InnerText.Replace(",","").Replace("No","").Trim());
                    if (x.Attributes["Name"] != null)
                        prd.Name = x.Attributes["Name"].InnerText.Replace(",", "").Replace("No", "").Replace("Nos", "").Replace("sqft", "").Trim();
                    else
                        prd.Name = string.Empty;
                    //prd.TOTAL = Single.Parse(x.Attributes["TOTAL"].InnerText.Replace(",", "").Trim());
                    if (x.Attributes["Qty"] != null)
                        prd.Qty = x.Attributes["Qty"].InnerText.Replace(",", "").Trim();
                    else
                        prd.Qty = string.Empty;

                    if (x.Attributes["Rate"] != null)
                        prd.Rate = x.Attributes["Rate"].InnerText.Replace(",", "").Trim();
                    else
                        prd.Rate = string.Empty;
                    if (x.Attributes["Amount"] != null)
                        prd.Amount = x.Attributes["Amount"].InnerText.Replace(",", "").Trim();
                    else
                        prd.Amount = string.Empty;
                    if (x.Attributes["IGST"] != null)
                        prd.IGST = x.Attributes["IGST"].InnerText.Replace(",", "").Trim();
                    else
                        prd.IGST = string.Empty;
                    if (x.Attributes["SGST"] != null)
                        prd.SGST = x.Attributes["SGST"].InnerText.Replace(",", "").Trim();
                    else
                        prd.SGST = string.Empty;
                    if (x.Attributes["CGST"] != null)
                        prd.CGST = x.Attributes["CGST"].InnerText.Replace(",", "").Trim();
                    else
                        prd.CGST = string.Empty;


                    //prd.ItemCodes = PopulateDropdown();
                    //objProdList.Add(prd);
                    objInvoice.InvoiceItems.Add(prd);
                }


                objInvoice.InvoiceReceiveddate = DateTime.Now;
                return objInvoice;
            }
            catch (Exception ex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
                return objInvoice;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }


        /// <summary>
        /// Save Invoice Details Data
        /// </summary>
        /// <param name="m">InvoiceModel</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SubmitInvoices([Bind(Exclude = "status,ledgerlist")]InvoiceModel m, FormCollection col)
        {
            ModelState.Remove("status");
            ModelState.Remove("GSTIN");
            ModelState.Remove("InvoiceNumber");
            ModelState.Remove("InvoiceAmount");
            ModelState.Remove("ledgerlist");
            ModelState.Remove("InvoiceDueDate");
            ModelState.Remove("InvoiceReceiveddate");
            ModelState.Remove("InvoiceDate");
            ModelState.Remove("VoucherType");
            //Invoicedate = DateTime.ParseExact(valuedate, "dd/MM/yyyy", null);
            //List<SelectListItem> items = new List<SelectListItem>();
            //string query = "select usr.UserID,usr.UserName,usrt.UsertypeID from dbo.[Users] usr inner join[UserTypes] usrt on usr.UserTypeID = usrt.UserTypeID AND usr.UserID <> 3";
            //using (SqlCommand comm = new SqlCommand(query))
            //{
            //    con.Open();
            //    comm.Connection = con;
            //    using (SqlDataReader sdr = comm.ExecuteReader())
            //    {
            //        while (sdr.Read())
            //        {

            //            items.Add(new SelectListItem
            //            {
            //                Text = sdr["UserName"].ToString(),
            //                Value = sdr["UserID"].ToString()
            //            });
            //        }
            //    }
            //    con.Close();
            //}
            //ViewBag.Users = items;
            //m.status = "Initial";
            ViewBag.Filepath = Session["Filepath"];
            //// Get Values for DropDown base on UserType

            foreach (var item in m.UserTypes)
            {
                item.Users = PopulateUsers(item.UserTypeID, 1000);
            }
            /*
            var errors = ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .Select(x => new { x.Key, x.Value.Errors })
            .ToArray();
            var err = errors; */
            //ModelState.IsValid

            if (ModelState.IsValid)
            {
                if (ConfigurationManager.AppSettings["OutPutType"].ToString() == "DB")
                {
                    try
                    {
                        Exeptionfrom = "InvoiceController/SubmitInvoices";
                        string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
                        SqlConnection con = new SqlConnection(connectionString);
                        var invn = m?.InvoiceNumber ?? "n";
                        if (!invn.Equals("n"))
                        {


                            Boolean ifInvoiceNoExist = ifExistingInvoiceNo(m.VendorName, m.InvoiceNumber);
                            if (ifInvoiceNoExist)
                            {
                                ViewBag.ErrMsg = " Invoice already exists";
                                List<SelectListItem> securityList = new List<SelectListItem>();
                                con.Open();
                                SqlCommand com = new SqlCommand("getSecuritylevelUsers", con);
                                com.CommandType = CommandType.StoredProcedure;
                                int branchid1 = ((List<int>)Session["BranchList"])[0];
                                com.Parameters.AddWithValue("@BranchID", branchid1);
                                using (SqlDataReader sdr = com.ExecuteReader())
                                {

                                    while (sdr.Read())
                                    {
                                        securityList.Add(new SelectListItem
                                        {
                                            Text = sdr["UserName"].ToString(),
                                            Value = sdr["UserID"].ToString()
                                        });
                                    }
                                }
                                m.SecurityUsers = securityList;
                                con.Close();
                                List<SelectListItem> invoicetypes = new List<SelectListItem>() { new SelectListItem { Text = "Invoice", Value = "Invoice" }, new SelectListItem { Text = "DC", Value = "DC" } };
                                m.DocType = invoicetypes;
								LocationDrp(m);
                                return View("Index", m);
                            }
                        }
                        con.Open();
                        int branchid = ((List<int>)Session["BranchList"])[0];
                        Logger.Write("stored procedure updateGateEntry");
                        SqlCommand comm = new SqlCommand("updateGateEntry", con);
                        comm.CommandType = CommandType.StoredProcedure;
                        Logger.Write("stored procedure updateGateEntry" + branchid);
                        comm.Parameters.AddWithValue("@BranchID", branchid);
                        comm.Parameters.AddWithValue("@GateEntry", Regex.Replace(m.GateEntryNumber, "[^0-9.]", ""));
                        Logger.Write("stored procedure updateGateEntry" + Regex.Replace(m.GateEntryNumber, "[^0-9.]", ""));

                        comm.ExecuteNonQuery();
                        //sp_InsertInvoiceItems
                        SqlCommand cmd = new SqlCommand("sp_InsertIQInvoiceItem", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        DateTime dateValue;
                        DateTime? date = null;
                        DateTime? invRecdate = null;
                        DateTime? invDuedate = null;
                        var Invoicedate = DateTime.ParseExact(col["InvoiceDate"], "dd/MM/yyyy", null);
                        /*
                        if (DateTime.TryParse(m.InvoiceDate.ToString(), out dateValue))
                        {
                            date = dateValue;
                        }*/
                        var InvoiceRecievedate = DateTime.ParseExact(col["InvoiceReceiveddate"], "dd/MM/yyyy", null);
                        /*
                        if (DateTime.TryParse(m.InvoiceReceiveddate.ToString(), out dateValue))
                        {
                            invRecdate = dateValue;
                        }*/

                        var InvoiceDuedate = DateTime.ParseExact(col["InvoiceDueDate"], "dd/MM/yyyy", null);

                        /*
                        if (DateTime.TryParse(m.InvoiceDueDate.ToString(), out dateValue))
                        {
                            invDuedate = dateValue;
                        }*/

                        int blist = ((List<int>)Session["BranchList"])[0];
                        VendorRepository vendor = new VendorRepository();
                        int vendorid = vendor.getVendorDetails().Where(v => v.VendorName == m.VendorName).Select(v => v.VendorID).DefaultIfEmpty(0).FirstOrDefault();
                        int userID = Convert.ToInt32(Session["UserID"]);
                        //Double? amount = (String.IsNullOrEmpty(p.Amount) ? (Double?)null : Double.Parse(p.Amount));  @PANNumber
                        cmd.Parameters.AddWithValue("@InvoiceNumber", m?.InvoiceNumber ?? "");
                        cmd.Parameters.AddWithValue("@PANNumber", m.PANNumber);
                        cmd.Parameters.AddWithValue("@PONumber", m.PONumber);
                        cmd.Parameters.AddWithValue("@InvoiceDate", Invoicedate);
                        cmd.Parameters.AddWithValue("@InvoiceReceiveddate", InvoiceRecievedate);
                        cmd.Parameters.AddWithValue("@InvoiceDueDate", InvoiceDuedate);
                        //cmd.Parameters.AddWithValue("@Amount", Convert.ToDouble(m.InvoiceAmount));
                        cmd.Parameters.AddWithValue("@InvoiceAmount", m?.InvoiceAmount ?? "");
                        cmd.Parameters.AddWithValue("@VendorID", m.VendorID);
                        cmd.Parameters.AddWithValue("@VendorName", m.VendorName);

                        cmd.Parameters.AddWithValue("@CreatedBy", userID);
                        cmd.Parameters.AddWithValue("@filePath", Session["FilePath"].ToString());
                        cmd.Parameters.AddWithValue("@GSTIN", m?.GSTIN ?? "");
                        //cmd.Parameters.AddWithValue("@PlaceOfSupply", m?.PlaceOfSupply??"");
                        //cmd.Parameters.AddWithValue("@ReverseCharges", m.ReverseCharges);
                        //cmd.Parameters.AddWithValue("@EcommerceGSTIN", m?.EcommerceGSTIN??"");
                        //cmd.Parameters.AddWithValue("@TaxPercent", m.TaxPercent);
                        //cmd.Parameters.AddWithValue("@TaxableValue", m.TaxableValue);
                        //cmd.Parameters.AddWithValue("@CessAmount", m.CessAmount);
                        cmd.Parameters.AddWithValue("@GateEntryNumber", Regex.Replace(m.GateEntryNumber, "[^0-9.]", ""));
                        cmd.Parameters.AddWithValue("@VehicleNumber", m.VehicleNumber);
                        cmd.Parameters.AddWithValue("@VehicleDate", m.Date);
                        cmd.Parameters.AddWithValue("@VehicleTime", m.Time);
                        cmd.Parameters.AddWithValue("@SelectedSecurity", m.SelectedSecurity);
                        cmd.Parameters.AddWithValue("@DocType", m.SelectedType);
						cmd.Parameters.AddWithValue("@LocationId",m.LocationId);
                        cmd.Parameters.AddWithValue("@BranchID", blist);
                        //Pass table Valued parameter to Store Procedure
                        DataTable dt = new DataTable();
                        dt.Columns.Add("UserTypeID", typeof(int));
                        dt.Columns.Add("UserID", typeof(int));

                        foreach (var usr in m.UserTypes)
                        {
                            dt.Rows.Add(usr.UserTypeID, usr.SelectedUserId);
                        }

                        SqlParameter sqlParam = cmd.Parameters.AddWithValue("@UserTypesTbl", dt);
                        sqlParam.SqlDbType = SqlDbType.Structured;
                        //int currentinvoice = 0;
                        int currentinvoice = Convert.ToInt32(cmd.ExecuteScalar());
                        for (int i = 40, j = 0; i < col.Keys.Count; i += 10, j++)
                        {
                            var id = col.GetValue("h" + j)?.AttemptedValue ?? "";
                            if (id == "")
                            {
                                j++;
                            }

                            cmd = new SqlCommand("sp_InsertInvoiceItems", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@InvoiceId", currentinvoice);
                            cmd.Parameters.AddWithValue("@Id", col.GetValue("id" + j).AttemptedValue);
                            cmd.Parameters.AddWithValue("@Code", col.GetValue("code" + j).AttemptedValue);
                            cmd.Parameters.AddWithValue("@Name", col.GetValue("name" + j).AttemptedValue);
                            cmd.Parameters.AddWithValue("@Qty", col.GetValue("qty" + j).AttemptedValue);
                            cmd.Parameters.AddWithValue("@Rate", col.GetValue("rate" + j).AttemptedValue);
                            cmd.Parameters.AddWithValue("@Amount", col.GetValue("amount" + j).AttemptedValue);
                            cmd.Parameters.AddWithValue("@IGST", col.GetValue("igst" + j).AttemptedValue);
                            cmd.Parameters.AddWithValue("@SGST", col.GetValue("sgst" + j).AttemptedValue);
                            cmd.Parameters.AddWithValue("@CGST", col.GetValue("cgst" + j).AttemptedValue);
                            var itemid = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        con.Close();

                        /////Send Mail
                        int assignedUserID = invoice.GetAssignedToUserID(currentinvoice, userID);
                        string emailTemplate = MailFunctionality(Server.MapPath("~/App_Data/Templates/InvoiceNotification.html"));
                        emailTemplate = emailTemplate.Replace("#URL#", ConfigurationManager.AppSettings["MailLink"].ToString() + "/" + currentinvoice);
                        var UserName = usr.getUserDetails().Where(u => u.UserID == assignedUserID).FirstOrDefault();
                        // string Name = "Hi" + " " + UserName.UserFirstName + " " + UserName.UserLastName;
                        string Name = "Dear user,";
                        emailTemplate = emailTemplate.Replace("#Name#", Name);
                        emailTemplate = emailTemplate.Replace("#VendorName#", m.VendorName);
                        invoice.Sendmail(int.Parse(Session["UserID"].ToString()), assignedUserID, currentinvoice, " Invoice Created", emailTemplate);

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
                    //   
                    //     ViewBag.ShowPartial = "NO";
                    return RedirectToAction("Index", "DashBoard");
                }
            }
            ////   return PartialView("InvoiceView", m);
            return View("Index", m);
        }
        [HttpPost]
        public JsonResult getGSTIN(string vendorname)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("sp_GetGSTIN", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@VendorName", vendorname);
            var gstin = "";
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {
                    gstin = sdr["GSTIN_UIN"].ToString();
                }
            }


            con.Close();

            return Json(new { gstin = gstin });
        }

        public JsonResult GetSupportFiles(string invoiceno)
        {
            List<IFiles> files = new List<IFiles>();
            try
            {
                string path = Server.MapPath("~/Files/InvoiceImages/");
                string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_GetAllInvoiceFilesById", con);
                var invoiceid = Session["invoiceid"];
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@InVoiceId", invoiceno);
                var count = 0;
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {

                        IFiles file = new IFiles();
                        file.InvoiceId = sdr["InvoiceID"].ToString();
                        file.FileName = path + sdr["FileName"].ToString();
                        file.FileType = sdr["FileType"].ToString();
                        file.SeqNo = count.ToString();
                        files.Add(file);
                        count++;
                    }
                }
                cmd.Parameters.Clear();
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
            return Json(files);
            // return Json(new { status = "success", Qno = currentinvoice });
        }
        private async Task<int> GetSeqNo(string invoiceno)
        {
            var count = 0;
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_GetAllInvoiceFilesById", con);
                var invoiceid = Session["invoiceid"];
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@InVoiceId", invoiceno);

                using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                {
                    while (sdr.Read())
                    {
                        count++;
                    }
                }
                cmd.Parameters.Clear();
                con.Close();
            }
            catch (Exception ex)
            {
                return count;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
            return count;

        }
        public async Task<JsonResult> DeleteFileByName(string filename)
        {
            string path = Server.MapPath("~/Files/");
            var targetPath = path + @"InvoiceImages/" + filename;
            string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("sp_deleteInvoiceFileByName", con);
            var invoiceid = Session["invoiceid"];
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FileName", filename);
            var currentinvoiceid = await cmd.ExecuteScalarAsync();
            System.IO.File.Delete(targetPath);
            con.Close();

            return Json(new { status = "success", invoiceid = currentinvoiceid });

        }
        public async Task<JsonResult> SubmitFiles(IFiles files)
        {
            //sp_InsertFile
            string fileName = "test.pdf";
            string sourcePath = @"D:\IQInvoiceSvn\trunk\InvoiceNew\Files";
            string targetPath = @"D:\IQInvoiceSvn\trunk\InvoiceNew\Files\InvoiceImages";
            string path = Server.MapPath("~/Files/");
            sourcePath = path;
            targetPath = path + @"InvoiceImages";
            //fileName = System.IO.Path.GetFileName(s);
            //destFile = System.IO.Path.Combine(targetPath, fileName);
            //System.IO.File.Copy(s, destFile, true);
            var currentinvoice = 0;
            try
            {


                var seq = await GetSeqNo(files.InvoiceId);
                fileName = "INV-" + files.InvoiceId + "~" + seq + "." + files.FileType;
                sourcePath = System.IO.Path.Combine(sourcePath + files.FileName);
                var destFile = System.IO.Path.Combine(targetPath, fileName);
                System.IO.File.Copy(sourcePath, destFile, true);


                string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_InsertFile", con);
                var invoiceid = Session["invoiceid"];
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@invoiceid", files.InvoiceId);
                cmd.Parameters.AddWithValue("@filename", fileName);
                cmd.Parameters.AddWithValue("@filetype", files.FileType);
                currentinvoice = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                cmd.Parameters.Clear();
                con.Close();
                //fileName = System.IO.Path.GetFileName(s);

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
            return Json(new { status = "success", filename = fileName });
        }

        /// <summary>
        /// get the file details for Download
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
                //filepath = Server.MapPath(filepath);//  wrong implementation
                // :D:\\InvoiceNew\\InvoiceNew\\Files\\Temp\\62c7c5ba-787e-4857-b675-137a16e9f424\\Invoice_Bizerba.pdf
                //:D%3A%5CInvoiceNew%5CInvoiceNew%5CFiles%5CTemp%5Ce7708f11-e797-4021-83a3-ad0bd099b6da%5CInvoice_Bizerba.pdf
                Exeptionfrom = "InvoiceController/GetFile";
                if (!String.IsNullOrWhiteSpace(filepath))
                {
                    string fileName = Path.GetFileName(filepath);
                    fileext = Path.GetExtension(filepath);
                    bytes = System.IO.File.ReadAllBytes(filepath);
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

        public JsonResult getLedgers()
        {
            //VendorRepository vendor = new VendorRepository();
            List<string> led = new List<string>() { "Item 1", "Item 2", "Item 3" };
            return Json(led, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Checks for Duplicate Invoice Number 
        /// </summary>
        /// <param name="vendor">vendor name</param>
        /// <param name="InvoiceNo">Invoice Number</param>
        /// <returns></returns>
        public bool ifExistingInvoiceNo(string vendor, string InvoiceNo)
        {
            bool IfInvoiceNumberExists = false;
            try
            {
                Exeptionfrom = "InvoiceController/ifExistingInvoiceNo";
                Logger.Write("vendor" + vendor);
                Logger.Write("InvoiceNo" + InvoiceNo);

                IfInvoiceNumberExists = invoice.IfInvoiceNumberExists(vendor, InvoiceNo);
                Logger.Write("ifExistingInvoiceNo completed");
            }
            catch (Exception ex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
                Logger.Write(ex.StackTrace);
            }
            return IfInvoiceNumberExists;

        }

        /// <summary>
        /// get Users for DropDownList assigned to a UserType
        /// </summary>
        /// <param name="userTypeId">UserTypeID</param>
        /// <returns></returns>
        public List<SelectListItem> PopulateUsers(int userTypeId, int branchid)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                Exeptionfrom = "InvoiceController/PopulateUsers";
                UserRepository usr = new UserRepository();
                IEnumerable<UserModel> ObjUsers = (IEnumerable<UserModel>)usr.getUsersForDropdown(branchid).Where(a => a.UserTypeID == userTypeId);
                foreach (var itm in ObjUsers)
                {
                    /*
                    if(userid == itm.UserID)
                    {
                        items.Add(new SelectListItem
                        {
                            Text = itm.UserLoginName.ToString(),
                            Value = itm.UserID.ToString(),
                            Selected = true
                        });
                    }*/

                    items.Add(new SelectListItem
                    {
                        Text = itm.UserLoginName.ToString(),
                        Value = itm.UserID.ToString(),

                    });


                }
            }
            catch (Exception ex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
            }
            return items;
        }
        /*
        public class PostModel
        {
            public string Name { get; set; }
            public string Phone { get; set; }
            public List<string> Links { get; set; }
        }
        
        [HttpPost]
        public JsonResult postJson(PostModel model)
        {
            var jobj = Json(new { name = model.Name, phone = model.Phone, links = model.Links });

            return jobj;
        }*/
        /// <summary>
        /// Get CReadit Deatils
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
        [HttpPost]
        public ActionResult getCreditDetails(string invoiceno)
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
        [HttpPost]
        public ActionResult getCreditData(string invoiceno)
        {
            JsonResult resp = Json(new { success = 0, creditdata = "" }, JsonRequestBehavior.AllowGet);

            InvoiceRepository invoice = new InvoiceRepository();
            try
            {
                int invoiceid = Convert.ToInt32(invoiceno);
                var creditdata = invoice.getdetailsdata(int.Parse(invoiceno)).ToList();
                resp = Json(new { success = 1, creditdata }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { Logger.Write(ex.Message); resp = Json(new { success = 0, creditdata = "" }, JsonRequestBehavior.AllowGet); }
            Session["InvoiceID"] = null;
            return resp;
        }

        /// <summary>
        /// Updating Debit Details
        /// </summary>
        /// <param name="id">debitID</param>
        /// <param name="amount">amount to be debited</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateDebitData(string id, string amount)
        {
            try
            {
                Exeptionfrom = "InvoiceController/UpdateDebitData";
                var fileName = string.Empty;
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        HttpPostedFileBase files = Request.Files[file];
                        fileName = Path.GetFileName(files.FileName);

                        // extract only the fielname
                        var ext = Path.GetExtension(fileName.ToLower());

                        if (ext == ".pdf")
                        {
                            files.SaveAs(Server.MapPath("~/Files/DebitFiles/") + "/" + fileName);
                        }
                    }
                }
                int invNo = Convert.ToInt32(id);
                double amt = Convert.ToDouble(amount);
                InvoiceRepository inv = new DataAccess.InvoiceRepository();
                inv.UpdateDebitData(invNo, amt, fileName);
            }
            catch (Exception ex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
            }
            return Json("1", JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateCredit(string creditId, string amount)
        {
            string message = "Some error Occurss while Updating";
            try
            {
                Exeptionfrom = "InvoiceController/UpdateCredit";
                int Id = Convert.ToInt32(creditId);
                double amt = Convert.ToDouble(amount);

                InvoiceRepository inv = new DataAccess.InvoiceRepository();

                if (inv.UpdateDebitDetail(Id, amt) > 0)
                {
                    message = "Success";
                }
            }
            catch (Exception ex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Delete debit Data from invoice
        /// </summary>
        /// <param name="debitId">debitId</param>
        /// <returns></returns>
        public JsonResult DeleteCredit(string debitId)
        {
            string message = "Some error Occurss while Deleting";
            try
            {
                Exeptionfrom = "InvoiceController/DeleteCredit";
                int Id = Convert.ToInt32(debitId);
                InvoiceRepository inv = new InvoiceRepository();

                if (inv.DeleteDebitDetail(Id) > 0)
                {
                    message = "Success";
                }
            }
            catch (Exception ex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Validate PO Number and Amount
        /// </summary>
        /// <param name="ponumber">PO Number</param>
        /// <param name="poamount">Amount</param>
        /// <returns></returns>
        public ActionResult CheckInvoiceAmount(string ponumber, string poamount)
        {
            try
            {
                Exeptionfrom = "InvoiceController/CheckInvoiceAmount";
                Decimal pamount = 0;
                if (!String.IsNullOrWhiteSpace(poamount))
                {
                    pamount = Decimal.Parse(poamount);
                }
                InvoiceRepository invoice = new InvoiceRepository();
                var result = invoice.CheckInvoiceAmount(ponumber, pamount);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// Download Debit Files
        /// </summary>
        /// <param name="id">InvoiceID</param>
        /// <returns></returns>
        public ActionResult DownloadFile(string id)
        {
            try
            {
                Exeptionfrom = "InvoiceController/DownloadFile";
                int ids = Convert.ToInt32(id);
                InvoiceRepository invoice = new InvoiceRepository();
                var result = invoice.getdetailsdata(ids).SingleOrDefault();

                FileStream fs = new FileStream(Server.MapPath(@"~\Files\DebitFiles\" + result.DebitFileName + ""), FileMode.Open, FileAccess.Read);
                string pdfFilePath = Server.MapPath(@"~\Files\DebitFiles\" + result.DebitFileName + "");
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + result.DebitFileName + "");
                Response.TransmitFile(pdfFilePath);
                Response.End();
            }
            catch (Exception ex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// get the Emaail Template from File
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string MailFunctionality(string filePath)
        {
            // Read the file as one string.
            System.IO.StreamReader myFile = new System.IO.StreamReader(filePath);
            string fileContents = myFile.ReadToEnd();
            myFile.Close();
            return fileContents;
        }

        [HttpPost]
        public JsonResult Upload()
        {
            InvoiceRepository invoice = new InvoiceRepository();

            string sessionname = "";
            if (Session["UserHit"] != null)
            {
                Logger.Write("Session :" + Session["UserHit"].ToString());
                sessionname = Session["UserHit"].ToString();
            }
            string[] Words = new string[100];
            if (TempData["T"] != null)
            {
                Logger.Write("Tempdata :" + Convert.ToString(TempData["T"]));
                string Str = Convert.ToString(TempData["T"]);
                Words = Str.Split(',');
            }
            try
            {

                string NewDir = Server.MapPath("~/Files/InvoiceImages");
                Logger.Write("NewDir :" + NewDir);
                int invoiceid = int.Parse(Session["InvoiceID"].ToString());
                int i = invoice.getlastfileid(invoiceid);
                if (i == 0)
                {
                    i = 1;
                }
                else
                {
                    i = i + 1;
                }
                string requiredFileName = invoice.GetInvoiceNumber(Session["InvoiceNumber"].ToString());
                requiredFileName = requiredFileName.Replace('/', '-');
                Logger.Write("requiredFileName :" + requiredFileName);
                // path = Server.MapPath("~/Files/InvoiceImages/") + requiredFileName;
                //if (!(Directory.Exists(path)))
                //{
                //  Directory.CreateDirectory(path);
                //  NewDir = Server.MapPath("~/Files/InvoiceImages/") + requiredFileName;
                //}
                foreach (var words in Words)
                {
                    sessionname = words;
                    Logger.Write("sessionname :" + words);
                    foreach (var file in Directory.GetFiles(Server.MapPath("~/Files/Temp/" + sessionname), "*"))
                    {

                        InVoiceFile inf = new InVoiceFile();
                        string FileName = Path.GetFileName(file);
                        string ext = System.IO.Path.GetExtension(FileName);
                        Logger.Write("FileName :" + FileName);

                        //if (!System.IO.File.Exists(Server.MapPath("~/Files/InvoiceImages/") + requiredFileName+"/" + FileName))
                        if (!System.IO.File.Exists(Server.MapPath("~/Files/InvoiceImages/") + FileName))
                        {
                            System.IO.File.Move(file, Path.Combine(NewDir, requiredFileName + "-" + i + ext));
                            Logger.Write(file + "-" + NewDir + "-" + requiredFileName + "-" + i + ext);
                            Logger.Write("Successfully done Move");
                        }
                        inf.InvoiceID = invoiceid;
                        inf.FileName = requiredFileName + "-" + i + ext;
                        inf.FileType = ext;
                        invoice.AddInvoiceFiles(inf);
                        i++;
                    }
                }


            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                Session.Remove("UserHit");
            }
            finally
            {
                if (Session["UserHit"] != null)
                {
                    if (Directory.Exists(Server.MapPath("~/Files/Temp/") + sessionname))
                    {
                        Logger.Write("Deleting temp file");
                        foreach (string filename in Directory.GetFiles(Server.MapPath("~/Files/Temp/") + sessionname))
                        {
                            System.IO.File.Delete(filename);

                        }
                        Directory.Delete(Server.MapPath("~/Files/Temp/") + sessionname);
                        Logger.Write("Deleted the temp file");
                    }
                    Session.Remove("UserHit");
                    Logger.Write("Session[Userhit] deleted");
                }
                TempData["T"] = null;
                Session["InvoiceID"] = null;

            }
            return Json("Uploaded " + Request.Files.Count + " files");
        }

        public string AddDebitNoteXml(InvoiceModel model, string vendorName, string invoicedate, List<TaxClass> debitnotetaxlist, List<TallyLedgerAmount> debitnotecatlist, List<LedgerModel> lstLegderlist, decimal ItemDebitNoteGrandTotal, List<string> listAdress, string branchname)
        {
            XmlDocument xdoc = new XmlDocument();

            xdoc.Load(Server.MapPath("~/tallyxml/DebitNoteVoucher.xml"));
            XmlNode Cname = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDESC/STATICVARIABLES/SVCURRENTCOMPANY");
            Cname.InnerText = branchname;
            XmlNode Rname = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/COMPANY/REMOTECMPINFO.LIST/REMOTECMPNAME");
            Rname.InnerText = branchname;
            XmlNode root = xdoc.DocumentElement;
            XmlNode parentNode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER");

            XmlNode VucherNumberNode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/NARRATION");
            VucherNumberNode.InnerText = model.Comment;

            XmlNode VucherNode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/REFERENCE");
            VucherNode.InnerText = model.InvoiceNumber;

            XmlNode VucherDATENode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/DATE");
            VucherDATENode.InnerText = invoicedate;

            XmlNode VucherREFERENCEDATENode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/REFERENCEDATE");
            VucherREFERENCEDATENode.InnerText = invoicedate;

            XmlNode VucherEFFECTIVEDATENode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/EFFECTIVEDATE");
            VucherEFFECTIVEDATENode.InnerText = invoicedate;

            XmlNode VuchervenderNode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/PARTYLEDGERNAME");
            VuchervenderNode.InnerText = vendorName;

            XmlNode Vuchervender2Node = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/BASICBASEPARTYNAME");
            Vuchervender2Node.InnerText = vendorName;

            XmlNode Vuchervender3Node = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/BASICBUYERNAME");
            Vuchervender3Node.InnerText = vendorName;


            XmlNode menu = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/BASICBUYERADDRESS.LIST");
            foreach (var mr in listAdress)
            {
                XmlNode newSub = xdoc.CreateNode(XmlNodeType.Element, "BASICBUYERADDRESS", null);
                newSub.InnerText = mr;
                menu.AppendChild(newSub);
            }

            XmlDocumentFragment xfrag = xdoc.CreateDocumentFragment();



            xfrag.InnerXml = @"<ALLLEDGERENTRIES.LIST>" +
           "<OLDAUDITENTRYIDS.LIST TYPE=\"Number\">" +
           "<OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>" +
           "</OLDAUDITENTRYIDS.LIST>" +
           "<LEDGERNAME>" + vendorName + "</LEDGERNAME>" +
           "<GSTCLASS/>" +
           "<ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>" +
           "<LEDGERFROMITEM>No</LEDGERFROMITEM>" +
           "<REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>" +
           "<ISPARTYLEDGER>Yes</ISPARTYLEDGER>" +
           "<ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE>" +
           "<AMOUNT>-" + ItemDebitNoteGrandTotal.ToString() + "</AMOUNT>" +
           "<VATEXPAMOUNT>-" + ItemDebitNoteGrandTotal.ToString() + "</VATEXPAMOUNT>" +
           "<SERVICETAXDETAILS.LIST></SERVICETAXDETAILS.LIST>" +
           "<BANKALLOCATIONS.LIST></BANKALLOCATIONS.LIST>" +
           "<BILLALLOCATIONS.LIST>" +
           "<NAME>" + model.InvoiceNumber + "</NAME>" +
           "<BILLTYPE>Agst Ref</BILLTYPE>" +
           "<TDSDEDUCTEEISSPECIALRATE>No</TDSDEDUCTEEISSPECIALRATE>" +
           "<AMOUNT>-" + ItemDebitNoteGrandTotal.ToString() + "</AMOUNT>" +
           "<INTERESTCOLLECTION.LIST></INTERESTCOLLECTION.LIST>" +
           "<STBILLCATEGORIES.LIST></STBILLCATEGORIES.LIST>" +
           "</BILLALLOCATIONS.LIST>" +
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


            foreach (var ledger in debitnotecatlist)
            {



                if (ledger != null)
                {
                    var ledgerName = ledger.LedgerName;
                    xfrag = xdoc.CreateDocumentFragment();
                    xfrag.InnerXml = @"<ALLLEDGERENTRIES.LIST>" +
                    "<OLDAUDITENTRYIDS.LIST TYPE=\"Number\">" +
                    "<OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>" +
                    "</OLDAUDITENTRYIDS.LIST>" +
                    "<LEDGERNAME>" + SecurityElement.Escape(ledger.LedgerName) + "</LEDGERNAME>" +
                    "<GSTCLASS/>" +
                    "<ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>" +
                    "<LEDGERFROMITEM>No</LEDGERFROMITEM>" +
                    "<REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>" +
                    "<ISPARTYLEDGER>No</ISPARTYLEDGER>" +
                    "<ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE>" +
                    "<AMOUNT>" + ledger.LedgerAmount + "</AMOUNT>" +
                    "<VATEXPAMOUNT>" + ledger.LedgerAmount + "</VATEXPAMOUNT>" +
                    "<SERVICETAXDETAILS.LIST></SERVICETAXDETAILS.LIST>" +
                    "<BANKALLOCATIONS.LIST></BANKALLOCATIONS.LIST>" +
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
                    xfrag = xdoc.CreateDocumentFragment();
                }
            }

            foreach (var ledger in debitnotetaxlist)
            {

                var ledgerNamelst = lstLegderlist.FirstOrDefault(a => a.LedgerName.Contains(ledger.TaxName) && a.LedgerName.StartsWith(ledger.TaxName) && a.LedgerName.ToUpper().Contains("INPUT") && a.LedgerName.Contains(ledger.TaxPercentage));


                if (ledgerNamelst != null)
                {
                    var ledgerName = ledgerNamelst.LedgerName;
                    xfrag = xdoc.CreateDocumentFragment();
                    xfrag.InnerXml = @"<ALLLEDGERENTRIES.LIST>" +
                    "<OLDAUDITENTRYIDS.LIST TYPE = \"Number\">" +
                    "<OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>" +
                    "</OLDAUDITENTRYIDS.LIST>" +
                    "<LEDGERNAME>" + SecurityElement.Escape(ledgerName) + "</LEDGERNAME>" +
                    "<GSTCLASS/>" +
                    "<ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>" +
                    "<LEDGERFROMITEM>No</LEDGERFROMITEM>" +
                    "<REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>" +
                    "<ISPARTYLEDGER>No</ISPARTYLEDGER>" +
                    "<ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE>" +
                    "<AMOUNT>" + ledger.TaxAmount.ToString() + "</AMOUNT>" +
                     "<VATEXPAMOUNT>" + ledger.TaxAmount.ToString() + "</VATEXPAMOUNT>" +
                    "<SERVICETAXDETAILS.LIST></SERVICETAXDETAILS.LIST> " +
                    "<BANKALLOCATIONS.LIST></BANKALLOCATIONS.LIST>" +
                    "<BILLALLOCATIONS.LIST></BILLALLOCATIONS.LIST>" +
                    "<INTERESTCOLLECTION.LIST></INTERESTCOLLECTION.LIST> " +
                    "<OLDAUDITENTRIES.LIST></OLDAUDITENTRIES.LIST>" +
                    "<ACCOUNTAUDITENTRIES.LIST></ACCOUNTAUDITENTRIES.LIST> " +
                    "<AUDITENTRIES.LIST></AUDITENTRIES.LIST>" +
                    "<INPUTCRALLOCS.LIST></INPUTCRALLOCS.LIST>" +
                    "<DUTYHEADDETAILS.LIST></DUTYHEADDETAILS.LIST>" +
                    "<EXCISEDUTYHEADDETAILS.LIST></EXCISEDUTYHEADDETAILS.LIST>" +
                    "<RATEDETAILS.LIST></RATEDETAILS.LIST>" +
                    "<SUMMARYALLOCS.LIST></SUMMARYALLOCS.LIST> " +
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
                    xfrag = xdoc.CreateDocumentFragment();
                }
            }
            string m = xdoc.InnerXml.ToString();
            string result1 = "";
            if (model.status == "A")
            {
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

                    Logger.Write("InvoiceID : " + model.InvoiceID);
                    Logger.Write("InvoiceNumber : " + model.InvoiceNumber);
                    Logger.Write(result1);


                    sr.Close();
                }
            }

            return result1;
        }


        public string AddCreditNoteXml(InvoiceModel model, string vendorName, string invoicedate, List<TaxClass> creditnotetaxlist, List<TallyLedgerAmount> creditnotecatlist, List<LedgerModel> lstLegderlist, decimal ItemCreditNoteGrandTotal, List<string> listAdress, string branchname)
        {
            XmlDocument xdoc = new XmlDocument();

            xdoc.Load(Server.MapPath("~/tallyxml/CreditNoteVoucher.xml"));
            XmlNode Cname = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDESC/STATICVARIABLES/SVCURRENTCOMPANY");
            Cname.InnerText = branchname;
            XmlNode Rname = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/COMPANY/REMOTECMPINFO.LIST/REMOTECMPNAME");
            Rname.InnerText = branchname;
            XmlNode root = xdoc.DocumentElement;
            XmlNode parentNode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER");

            XmlNode VucherNumberNode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/NARRATION");
            VucherNumberNode.InnerText = model.Comment;

            XmlNode VucherNode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/REFERENCE");
            VucherNode.InnerText = model.InvoiceNumber;

            XmlNode VucherDATENode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/DATE");
            VucherDATENode.InnerText = invoicedate;

            XmlNode VucherREFERENCEDATENode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/REFERENCEDATE");
            VucherREFERENCEDATENode.InnerText = invoicedate;

            XmlNode VucherEFFECTIVEDATENode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/EFFECTIVEDATE");
            VucherEFFECTIVEDATENode.InnerText = invoicedate;

            XmlNode VuchervenderNode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/PARTYLEDGERNAME");
            VuchervenderNode.InnerText = vendorName;

            XmlNode Vuchervender2Node = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/BASICBASEPARTYNAME");
            Vuchervender2Node.InnerText = vendorName;

            XmlNode Vuchervender3Node = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/BASICBUYERNAME");
            Vuchervender3Node.InnerText = vendorName;


            XmlNode menu = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/BASICBUYERADDRESS.LIST");
            foreach (var mr in listAdress)
            {
                XmlNode newSub = xdoc.CreateNode(XmlNodeType.Element, "BASICBUYERADDRESS", null);
                newSub.InnerText = mr;
                menu.AppendChild(newSub);
            }

            XmlDocumentFragment xfrag = xdoc.CreateDocumentFragment();

            xfrag.InnerXml = @"<ALLLEDGERENTRIES.LIST>" +
           "<OLDAUDITENTRYIDS.LIST TYPE=\"Number\">" +
           "<OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>" +
           "</OLDAUDITENTRYIDS.LIST>" +
           "<LEDGERNAME>" + vendorName + "</LEDGERNAME>" +
           "<GSTCLASS/>" +
           "<ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>" +
           "<LEDGERFROMITEM>No</LEDGERFROMITEM>" +
           "<REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>" +
           "<ISPARTYLEDGER>Yes</ISPARTYLEDGER>" +
           "<ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE>" +
           "<AMOUNT>" + ItemCreditNoteGrandTotal.ToString() + "</AMOUNT>" +
           "<VATEXPAMOUNT>" + ItemCreditNoteGrandTotal.ToString() + "</VATEXPAMOUNT>" +
           "<SERVICETAXDETAILS.LIST></SERVICETAXDETAILS.LIST>" +
           "<BANKALLOCATIONS.LIST></BANKALLOCATIONS.LIST>" +
           "<BILLALLOCATIONS.LIST>" +
           "<NAME>" + model.InvoiceNumber + "</NAME>" +
           "<BILLTYPE>Agst Ref</BILLTYPE>" +
           "<TDSDEDUCTEEISSPECIALRATE>No</TDSDEDUCTEEISSPECIALRATE>" +
           "<AMOUNT>" + ItemCreditNoteGrandTotal.ToString() + "</AMOUNT>" +
           "<INTERESTCOLLECTION.LIST></INTERESTCOLLECTION.LIST>" +
           "<STBILLCATEGORIES.LIST></STBILLCATEGORIES.LIST>" +
           "</BILLALLOCATIONS.LIST>" +
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

            foreach (var ledger in creditnotecatlist)
            {



                if (ledger != null)
                {
                    var ledgerName = ledger.LedgerName;
                    xfrag = xdoc.CreateDocumentFragment();
                    xfrag.InnerXml = @"<ALLLEDGERENTRIES.LIST>" +
                    "<OLDAUDITENTRYIDS.LIST TYPE=\"Number\">" +
                    "<OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>" +
                    "</OLDAUDITENTRYIDS.LIST>" +
                    "<LEDGERNAME>" + SecurityElement.Escape(ledger.LedgerName) + "</LEDGERNAME>" +
                    "<GSTCLASS/>" +
                    "<ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>" +
                    "<LEDGERFROMITEM>No</LEDGERFROMITEM>" +
                    "<REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>" +
                    "<ISPARTYLEDGER>No</ISPARTYLEDGER>" +
                    "<ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE>" +
                    "<AMOUNT>-" + ledger.LedgerAmount + "</AMOUNT>" +
                    "<VATEXPAMOUNT>-" + ledger.LedgerAmount + "</VATEXPAMOUNT>" +
                    "<SERVICETAXDETAILS.LIST></SERVICETAXDETAILS.LIST>" +
                    "<BANKALLOCATIONS.LIST></BANKALLOCATIONS.LIST>" +
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
                    xfrag = xdoc.CreateDocumentFragment();
                }
            }

            foreach (var ledger in creditnotetaxlist)
            {

                var ledgerNamelst = lstLegderlist.FirstOrDefault(a => a.LedgerName.Contains(ledger.TaxName) && a.LedgerName.StartsWith(ledger.TaxName) && a.LedgerName.ToUpper().Contains("INPUT") && a.LedgerName.Contains(ledger.TaxPercentage));


                if (ledgerNamelst != null)
                {
                    var ledgerName = ledgerNamelst.LedgerName;
                    xfrag = xdoc.CreateDocumentFragment();
                    xfrag.InnerXml = @"<ALLLEDGERENTRIES.LIST>" +
                    "<OLDAUDITENTRYIDS.LIST TYPE = \"Number\">" +
                    "<OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>" +
                    "</OLDAUDITENTRYIDS.LIST>" +
                    "<LEDGERNAME>" + SecurityElement.Escape(ledgerName) + "</LEDGERNAME>" +
                    "<GSTCLASS/>" +
                    "<ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>" +
                    "<LEDGERFROMITEM>No</LEDGERFROMITEM>" +
                    "<REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>" +
                    "<ISPARTYLEDGER>No</ISPARTYLEDGER>" +
                    "<ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE>" +
                    "<AMOUNT>-" + ledger.TaxAmount.ToString() + "</AMOUNT>" +
                     "<VATEXPAMOUNT>-" + ledger.TaxAmount.ToString() + "</VATEXPAMOUNT>" +
                    "<SERVICETAXDETAILS.LIST></SERVICETAXDETAILS.LIST> " +
                    "<BANKALLOCATIONS.LIST></BANKALLOCATIONS.LIST>" +
                    "<BILLALLOCATIONS.LIST></BILLALLOCATIONS.LIST>" +
                    "<INTERESTCOLLECTION.LIST></INTERESTCOLLECTION.LIST> " +
                    "<OLDAUDITENTRIES.LIST></OLDAUDITENTRIES.LIST>" +
                    "<ACCOUNTAUDITENTRIES.LIST></ACCOUNTAUDITENTRIES.LIST> " +
                    "<AUDITENTRIES.LIST></AUDITENTRIES.LIST>" +
                    "<INPUTCRALLOCS.LIST></INPUTCRALLOCS.LIST>" +
                    "<DUTYHEADDETAILS.LIST></DUTYHEADDETAILS.LIST>" +
                    "<EXCISEDUTYHEADDETAILS.LIST></EXCISEDUTYHEADDETAILS.LIST>" +
                    "<RATEDETAILS.LIST></RATEDETAILS.LIST>" +
                    "<SUMMARYALLOCS.LIST></SUMMARYALLOCS.LIST> " +
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
                    xfrag = xdoc.CreateDocumentFragment();
                }
            }
            string m = xdoc.InnerXml.ToString();

            string result1 = "";
            if (model.status == "A")
            {
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

                    Logger.Write("InvoiceID : " + model.InvoiceID);
                    Logger.Write("InvoiceNumber : " + model.InvoiceNumber);
                    Logger.Write(result1);


                    sr.Close();
                }
            }

            return result1;
        }


        public string AddtoPurchaseXml(InvoiceModel model, string vendorName, string invoicedate, List<TaxClass> taxlist, List<TallyLedgerAmount> catlist, Dictionary<string, decimal> ledgerlistAmount, List<LedgerModel> lstLegderlist, decimal ItemGrandTotal, List<string> listAdress, string branchname)
        {
            XmlDocument xdoc = new XmlDocument();

            xdoc.Load(Server.MapPath("~/tallyxml/AccountingVoucher.xml"));
            XmlNode Cname = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDESC/STATICVARIABLES/SVCURRENTCOMPANY");
            Cname.InnerText = branchname;
            XmlNode Rname = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/COMPANY/REMOTECMPINFO.LIST/REMOTECMPNAME");
            Rname.InnerText = branchname;
            XmlNode root = xdoc.DocumentElement;
            XmlNode parentNode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER");

            XmlNode VucherNumberNode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/NARRATION");
            VucherNumberNode.InnerText = model.Comment;

			XmlNode VOUCHERNUMBERNode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/VOUCHERNUMBER");
			VOUCHERNUMBERNode.InnerText = model.InvoiceID.ToString();

			XmlNode PARTYLEDGERNAMENode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/PARTYLEDGERNAME");
            PARTYLEDGERNAMENode.InnerText = model.VendorName;

			XmlNode VucherNode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/REFERENCE");
			VucherNode.InnerText = model.InvoiceNumber;

			XmlNode VucherDATENode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/DATE");
            VucherDATENode.InnerText = invoicedate;

            XmlNode VucherREFERENCEDATENode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/REFERENCEDATE");
            VucherREFERENCEDATENode.InnerText = invoicedate;

            XmlNode VucherEFFECTIVEDATENode = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/EFFECTIVEDATE");
            VucherEFFECTIVEDATENode.InnerText = invoicedate;

            ///      <VOUCHERNUMBER>2018030108</VOUCHERNUMBER>
    //  < PARTYLEDGERNAME > Sam Traders </ PARTYLEDGERNAME >





               //XmlNode menu = xdoc.SelectSingleNode("ENVELOPE/BODY/IMPORTDATA/REQUESTDATA/TALLYMESSAGE/VOUCHER/ADDRESS.LIST");



               //foreach (var mr in listAdress)
               //{
               //    XmlNode newSub = xdoc.CreateNode(XmlNodeType.Element, "ADDRESS", null);
               //    newSub.InnerText = mr;
               //    menu.AppendChild(newSub);
               //}


            XmlDocumentFragment xfrag = xdoc.CreateDocumentFragment();

            //  var taxlist = lstledgerNames.Where(a => a.Contains("Tax")).ToList();


            ////  decimal total = Decimal.Parse("53423");
            //foreach (var tax in taxlist)
            //{
            //    Iteminput = Convert.ToInt32(Regex.Replace(tax, "[^0-9]+", string.Empty));
            //    total += Math.Round(GrandTotal * (input) / 100, 0);
            //}
            xfrag.InnerXml = @"<ALLLEDGERENTRIES.LIST>" +
			"<OLDAUDITENTRYIDS.LIST TYPE=\"Number\">" +
			"<OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>" +
			"</OLDAUDITENTRYIDS.LIST>" +
			"<LEDGERNAME>" + vendorName + "</LEDGERNAME>" +
			"<GSTCLASS/>" +
			"<ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>" +
			"<LEDGERFROMITEM>No</LEDGERFROMITEM>" +
			"<REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>" +
			"<ISPARTYLEDGER>Yes</ISPARTYLEDGER>" +
			"<ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE>" +
			"<AMOUNT>" + ItemGrandTotal.ToString() + "</AMOUNT>" +
			"<VATEXPAMOUNT>" + ItemGrandTotal.ToString() + "</VATEXPAMOUNT>" +
			"<SERVICETAXDETAILS.LIST>       </SERVICETAXDETAILS.LIST>" +
			"<CATEGORYALLOCATIONS.LIST>" +
			"<CATEGORY>Service</CATEGORY>" +
			"<ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>" +
			"<COSTCENTREALLOCATIONS.LIST>" +
			"<NAME>Ambattur Unit I-Service</NAME>" +
			"<AMOUNT>" + ItemGrandTotal.ToString() +"</AMOUNT>" +
			"</COSTCENTREALLOCATIONS.LIST>" +
			"</CATEGORYALLOCATIONS.LIST>" +
			"<BANKALLOCATIONS.LIST></BANKALLOCATIONS.LIST>" +
			"<BILLALLOCATIONS.LIST>" +
			"<NAME>" + model.InvoiceNumber + "</NAME>" +
			"<BILLCREDITPERIOD   P=\"75 Days\">75 Days</BILLCREDITPERIOD>" +
			"<BILLTYPE>New Ref</BILLTYPE>" +
			"<TDSDEDUCTEEISSPECIALRATE>No</TDSDEDUCTEEISSPECIALRATE>" +
			"<AMOUNT>" + ItemGrandTotal.ToString() + "</AMOUNT>" +
			"<INTERESTCOLLECTION.LIST></INTERESTCOLLECTION.LIST>" +
			"<STBILLCATEGORIES.LIST></STBILLCATEGORIES.LIST>" +
			"</BILLALLOCATIONS.LIST>" +
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
            int input = 0;
            decimal total = 0;
            var ledgerlist = ledgerlistAmount.Select(pair => new { Id = pair.Key, Name = pair.Value }).ToList();
            //foreach (var item in objLedgerlist)
            //{
            //  List<string> debitlist = item.Id.Split(',').OrderByDescending(a=>!a.Contains("Tax")).ToList();  


            foreach (var ledger in catlist)
            {
                //string taxpercentage = "";
                //if (ledger.Contains("Tax"))
                //{
                //    taxpercentage = Regex.Replace(ledger, "[^0-9]+", string.Empty);
                //}

                //if (taxpercentage != "")
                //{
                //    input = Convert.ToInt32(taxpercentage);
                //    total = Math.Round(item.Name * (input) / 100, 0);
                //}
                //else
                //{
                //    total = item.Name;
                //}


                xfrag = xdoc.CreateDocumentFragment();
                xfrag.InnerXml = @"<ALLLEDGERENTRIES.LIST>" +
				"<OLDAUDITENTRYIDS.LIST TYPE=\"Number\">" +
				"<OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>" +
				"</OLDAUDITENTRYIDS.LIST>" +
				"<LEDGERNAME>" + SecurityElement.Escape(ledger.LedgerName) +"</LEDGERNAME>" +
				"<GSTCLASS/>" +
				"<ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>" +
				"<LEDGERFROMITEM>No</LEDGERFROMITEM>" +
				"<REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>" +
				"<ISPARTYLEDGER>No</ISPARTYLEDGER>" +
				"<ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE>" +
				"<AMOUNT>-" + ledger.LedgerAmount + "</AMOUNT>" +
				"<VATEXPAMOUNT>-" + ledger.LedgerAmount + "</VATEXPAMOUNT>" +
				"<SERVICETAXDETAILS.LIST></SERVICETAXDETAILS.LIST>" +
				"<CATEGORYALLOCATIONS.LIST>" +
				"<CATEGORY>Service</CATEGORY>" +
				"<ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>" +
				"<COSTCENTREALLOCATIONS.LIST>" +
				"<NAME>"+getLocationNameById(model.LocationId)[2]+"</NAME>" +
				"<AMOUNT>-" + ledger.LedgerAmount + "</AMOUNT>" +
				"</COSTCENTREALLOCATIONS.LIST>" +
				"</CATEGORYALLOCATIONS.LIST>" +
				"<BANKALLOCATIONS.LIST></BANKALLOCATIONS.LIST>" +
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
                xfrag = xdoc.CreateDocumentFragment();
            }
            foreach (var ledger in taxlist)
            {

                var ledgerNamelst = lstLegderlist.FirstOrDefault(a => a.LedgerName.Contains(ledger.TaxName) && a.LedgerName.StartsWith("INPUT") && a.LedgerName.Contains("TN") && a.LedgerName.Contains(ledger.TaxPercentage));


                if (ledgerNamelst != null)
                {
                    var ledgerName = ledgerNamelst.LedgerName;
                    xfrag = xdoc.CreateDocumentFragment();

                    //xfrag.InnerXml = @"<ALLLEDGERENTRIES.LIST>" +
                    //"<OLDAUDITENTRYIDS.LIST TYPE = \"Number\">" +
                    //"<OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>" +
                    //"</OLDAUDITENTRYIDS.LIST>" +
                    //"<LEDGERNAME>" + SecurityElement.Escape(ledgerName) + "</LEDGERNAME>" +
                    //"<GSTCLASS/>" +
                    //"<ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>" +
                    //"<LEDGERFROMITEM>No</LEDGERFROMITEM>" +
                    //"<REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>" +
                    //"<ISPARTYLEDGER>No</ISPARTYLEDGER>" +
                    //"<ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE>" +
                    //"<AMOUNT>-" + ledger.TaxAmount.ToString() + "</AMOUNT>" +
                    // "<VATEXPAMOUNT>-" + ledger.TaxAmount.ToString() + "</VATEXPAMOUNT>" +
                    //"<SERVICETAXDETAILS.LIST></SERVICETAXDETAILS.LIST> " +
                    //"<BANKALLOCATIONS.LIST></BANKALLOCATIONS.LIST>" +
                    //"<BILLALLOCATIONS.LIST></BILLALLOCATIONS.LIST>" +
                    //"<INTERESTCOLLECTION.LIST></INTERESTCOLLECTION.LIST> " +
                    //"<OLDAUDITENTRIES.LIST></OLDAUDITENTRIES.LIST>" +
                    //"<ACCOUNTAUDITENTRIES.LIST></ACCOUNTAUDITENTRIES.LIST> " +
                    //"<AUDITENTRIES.LIST></AUDITENTRIES.LIST>" +
                    //"<INPUTCRALLOCS.LIST></INPUTCRALLOCS.LIST>" +
                    //"<DUTYHEADDETAILS.LIST></DUTYHEADDETAILS.LIST>" +
                    //"<EXCISEDUTYHEADDETAILS.LIST></EXCISEDUTYHEADDETAILS.LIST>" +
                    //"<RATEDETAILS.LIST></RATEDETAILS.LIST>" +
                    //"<SUMMARYALLOCS.LIST></SUMMARYALLOCS.LIST> " +
                    //"<STPYMTDETAILS.LIST></STPYMTDETAILS.LIST>" +
                    //"<EXCISEPAYMENTALLOCATIONS.LIST></EXCISEPAYMENTALLOCATIONS.LIST>" +
                    //"<TAXBILLALLOCATIONS.LIST></TAXBILLALLOCATIONS.LIST>" +
                    //"<TAXOBJECTALLOCATIONS.LIST></TAXOBJECTALLOCATIONS.LIST>" +
                    //"<TDSEXPENSEALLOCATIONS.LIST></TDSEXPENSEALLOCATIONS.LIST>" +
                    //"<VATSTATUTORYDETAILS.LIST></VATSTATUTORYDETAILS.LIST>" +
                    //"<COSTTRACKALLOCATIONS.LIST></COSTTRACKALLOCATIONS.LIST>" +
                    //"<REFVOUCHERDETAILS.LIST></REFVOUCHERDETAILS.LIST>" +
                    //"<INVOICEWISEDETAILS.LIST></INVOICEWISEDETAILS.LIST>" +
                    //"<VATITCDETAILS.LIST></VATITCDETAILS.LIST>" +
                    //"<ADVANCETAXDETAILS.LIST></ADVANCETAXDETAILS.LIST>" +
                    //"</ALLLEDGERENTRIES.LIST>";
                    xfrag.InnerXml =  @"<ALLLEDGERENTRIES.LIST>" +
"<OLDAUDITENTRYIDS.LIST TYPE=\"Number\">" +
"<OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>" +
"</OLDAUDITENTRYIDS.LIST>" +
"<LEDGERNAME>"+ SecurityElement.Escape(ledgerName) +"</LEDGERNAME>" +
"<GSTCLASS/>" +
"<ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>" +
"<LEDGERFROMITEM>No</LEDGERFROMITEM>" +
"<REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>" +
"<ISPARTYLEDGER>No</ISPARTYLEDGER>" +
"<ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE>" +
"<AMOUNT>-" + ledger.TaxAmount.ToString() + "</AMOUNT>" +
"<VATEXPAMOUNT>-" + ledger.TaxAmount.ToString() + "</VATEXPAMOUNT>" +
"<SERVICETAXDETAILS.LIST>       </SERVICETAXDETAILS.LIST>" +
"<CATEGORYALLOCATIONS.LIST>" +
"<CATEGORY>Service</CATEGORY>" +
"<ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>" +
"<COSTCENTREALLOCATIONS.LIST>" +
"<NAME>Ambattur Unit I-Service</NAME>" +
"<AMOUNT>-" + ledger.TaxAmount.ToString() + "</AMOUNT>" +
"</COSTCENTREALLOCATIONS.LIST>" +
"</CATEGORYALLOCATIONS.LIST>" +
"<BANKALLOCATIONS.LIST></BANKALLOCATIONS.LIST>" +
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
                    xfrag = xdoc.CreateDocumentFragment();
                }
            }
            string m = xdoc.InnerXml.ToString();

            string result1 = "";
            if (model.status == "A")
            {
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

                    Logger.Write("InvoiceID : " + model.InvoiceID);
                    Logger.Write("InvoiceNumber : " + model.InvoiceNumber);
                    Logger.Write(result1);


                    sr.Close();
                }
            }

            return result1;

        }
    }
}