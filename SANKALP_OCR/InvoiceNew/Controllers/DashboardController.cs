using InvoiceNew.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using InvoiceNew.DataAccess;
using System.Text.RegularExpressions;

namespace InvoiceNew.Controllers
{
    public class DashboardController : IQinvoiceController
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
        string Exeptionfrom = string.Empty;
        /// <summary>
        /// get the Invoice Items to show in DashBoard Home page
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //Logger.Write("Logs Creation");
            List<string> label = new List<string>();
            List<int> InvoiceCount = new List<int>();
            string test = "";
            string lbl = "";
            List<InvoiceModel> objlist = GetTopItems();
            // objlist = objlist.OrderByDescending(n => n.InvoiceDate).Take(5).ToList();
            List<int> blist = (List<int>)Session["BranchList"];            
            UserRepository ur = new UserRepository();
            DataTable dt = ur.getBranchList(blist);
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_getDashBoardetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BranchList", dt);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        /*
                        ViewBag.OVERDUE = rdr["OVERDUE"].ToString();
                        ViewBag.DUETODAY = rdr["DUETODAY"].ToString();
                        ViewBag.PENDING = rdr["PENDING"].ToString();
                        ViewBag.PAID = rdr["PAID"].ToString();
                        ViewBag.OSOVERDUE = Convert.ToDouble(rdr["OSOVERDUE"]).ToString("n2");
                        ViewBag.OSTODAY = Convert.ToDouble(rdr["OSTODAY"]).ToString("n2");
                        ViewBag.OSTHISMONTH = Convert.ToDouble(rdr["OSTHISMONTH"]).ToString("n2");
                        ViewBag.TotalOS = Convert.ToDouble(rdr["TotalOS"]).ToString("n2");*/
                        ViewBag.OVERDUE = rdr["OVERDUE"].ToString();
                        ViewBag.DUETODAY = rdr["DUETODAY"].ToString();
                        ViewBag.PENDING = rdr["PENDING"].ToString();
                        ViewBag.PAID = rdr["PAID"].ToString();
                        ViewBag.OSOVERDUE = Convert.ToDouble(rdr["OSOVERDUE"]).ToString("n2");
                        ViewBag.OSTODAY = Convert.ToDouble(rdr["OSTODAY"]).ToString("n2");
                        ViewBag.TOTALOUTSTANDING = Convert.ToDouble(rdr["TotalOutstanding"]).ToString("n2");
                        ViewBag.TotalPAID = Convert.ToDouble(rdr["TotalPaid"]).ToString("n2");
                    }
                }
                con.Close();
                this.GetChartValues();
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
            return View(objlist);
        }

        /// <summary>
        /// Get All Invoices
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAllProducts()
        {
            List<InvoiceModel> objlist = GetSelectedItems();
            return PartialView("_ProductDetails", objlist);
        }

        /// <summary>
        /// get all the Invoice Items for a user
        /// </summary>
        /// <returns></returns>
        public List<InvoiceModel> GetSelectedItems()
        {
            List<InvoiceModel> objList = new List<InvoiceModel>();
            SqlConnection con = new SqlConnection(connectionString);
            ViewBag.GRN = "Disabled";
            try
            {
                Exeptionfrom = "DashBoard/GetSelectedItems";
                UserRepository ur = new UserRepository();
                DataTable blist = ur.getBranchList((List<int>)Session["BranchList"]);
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_GetInvoiceItems", con);///GetOrderItems
                int userID = Convert.ToInt32(Session["UserID"]);
                cmd.Parameters.AddWithValue("@CurrentUserID", userID);
                cmd.Parameters.AddWithValue("@BranchList",blist);
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
                        if(dr["CurrentStatus"] != DBNull.Value)
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
            return objList;
        }


        /// <summary>
        /// get top 5 Items for Dashboard
        /// </summary>
        /// <returns></returns>
        public List<InvoiceModel> GetTopItems()
        {
            List<InvoiceModel> objList = new List<InvoiceModel>();
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                Exeptionfrom = "DashBoard/GetTopItems";

                con.Open();
                SqlCommand cmd = new SqlCommand("sp_GetTop5InvoiceItems", con);///GetOrderItems
                int userID = Convert.ToInt32(Session["UserID"]);
                cmd.Parameters.AddWithValue("@CurrentUserID", userID);
                UserRepository ur = new UserRepository();
                DataTable blist = ur.getBranchList((List<int>)Session["BranchList"]);
                cmd.Parameters.AddWithValue("@BranchList", blist);
                cmd.CommandType = CommandType.StoredProcedure;
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(ds);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    InvoiceModel objinv = new InvoiceModel();
                    //if (dr["CurrentStatus"].ToString() != "Pending For Payment" && dr["CurrentStatus"].ToString() != "Paid")
                    //{
                        objinv.InvoiceID = Convert.ToInt32(dr["InvoiceID"].ToString());
                        objinv.VendorName = dr["VendorName"].ToString();
                        objinv.InvoiceNumber = dr["InvoiceNumber"].ToString();
                        objinv.PONumber = dr["PONumber"].ToString();
                        objinv.InvoiceAmount = dr["InvoiceAmount"].ToString();
                        objinv.PANNumber = dr["PANNumber"].ToString();

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
                        objList.Add(objinv);
                    
                }
                con.Close();
            }
            catch (Exception ex)
            {
                //Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
                return null;
            }
            return objList;
        }

        /// <summary>
        /// get details for Chart 
        /// </summary>
        public void GetChartValues()
        {
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                Exeptionfrom = "Dashboard/GetChartValues";
                List<string> labels = new List<string>();
                List<int> paidInvoiceCount = new List<int>();
                List<string> labels2 = new List<string>();
                List<int> DueInvoiceCount = new List<int>();
                string test = "";
                string lbl = "";

                con.Open();
                SqlCommand com = new SqlCommand("sp_getChartDetails_2", con);
                com.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader rdr = com.ExecuteReader())
                {
                    string mnt = string.Empty;
                    int cntPd = 0, cntDue = 0;
                    while (rdr.Read())
                    {
                        mnt = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Convert.ToInt16(rdr["monthVal"])).ToString() + " " + rdr["yearVal"].ToString();
                        cntPd = Convert.ToInt32(rdr["PaidInvoices"]);
                        cntDue = Convert.ToInt32(rdr["DueInvoices"]);
                        labels.Add(mnt);
                        paidInvoiceCount.Add(cntPd);
                        DueInvoiceCount.Add(cntDue);

                    }
                }
                con.Close();

                ////Labels
                var lblresult = labels.ToArray();
                for (int i = 0; i < lblresult.Length; i++)
                {
                    if (i == 0)
                    {
                        lbl = lbl + lblresult[i].ToString();
                    }
                    else
                    {
                        lbl = lbl + "," + lblresult[i].ToString();
                    }
                }
                var result = paidInvoiceCount.ToArray();
                //  var lblresult = labels.ToArray();
                string grp = ComputeGraphValues(result);
                ViewBag.PaidInvoices = grp;
                ViewBag.Labels = lbl;//grp[1];
                                     ///
                result = DueInvoiceCount.ToArray();
                grp = ComputeGraphValues(result);
                ViewBag.DueInvoices = grp;
                //  ViewBag.DueLabels = grp[1];
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
        }

        /// <summary>
        /// Get Value for Graph
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public string ComputeGraphValues(int[] result)
        {
            string grValue = "";
            try
            {
                Exeptionfrom = "Dashboard/ComputeGraphValues";
                string lbl = "";

                //  var result = paidInvoiceCount.ToArray();
                for (int i = 0; i < result.Length; i++)
                {
                    if (i == 0)
                    {
                        grValue = grValue + result[i].ToString();
                    }
                    else
                    {
                        grValue = grValue + "," + result[i].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
            }
            return grValue;
        }

        /// <summary>
        /// Get All Invoice Items for Logged in User
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllItemsForUser()
        {
            SqlConnection con = new SqlConnection(connectionString);
            List<InvoiceModel> objList = new List<InvoiceModel>();
            try
            {
                Exeptionfrom = "Dashboard/GetAllItemsForUser";

                con.Open();
                SqlCommand cmd = new SqlCommand("sp_GetAllInvoiceItemsForUser", con);///GetOrderItems
                int userID = Convert.ToInt32(Session["UserID"]);
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.CommandType = CommandType.StoredProcedure;
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(ds);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    InvoiceModel objinv = new InvoiceModel();
                    objinv.InvoiceID = Convert.ToInt32(dr["InvoiceID"].ToString());
                    objinv.VendorName = dr["VendorName"].ToString();
                    objinv.InvoiceNumber = dr["InvoiceNumber"].ToString();
                    objinv.PONumber = dr["PONumber"].ToString();
                    objinv.InvoiceAmount = dr["InvoiceAmount"].ToString();
                    objinv.PANNumber = dr["PANNumber"].ToString();

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
                        else if (curSt.ToUpper() == "H")
                            objinv.CurrentStatus = "On Hold";
                        else
                            objinv.CurrentStatus = dr["CurrentStatus"].ToString();
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
            return View("MyInvoices", objList);
        }
    }
}