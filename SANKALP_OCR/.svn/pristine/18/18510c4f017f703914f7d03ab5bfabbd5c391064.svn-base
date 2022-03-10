using InvoiceNew.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
namespace InvoiceNew.Controllers
{
    public class DashboardController : IQinvoiceController
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
        public ActionResult Index()
        {
            List<string> label = new List<string>();
            List<int> InvoiceCount = new List<int>();
            string test = "";
            string lbl = "";
            List<InvoiceModel> objlist = GetSelectedItems();
            objlist = objlist.OrderByDescending(n => n.InvoiceDate).Take(5).ToList();

            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("sp_getDashBoardetails", con);
            cmd.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    ViewBag.OVERDUE = rdr["OVERDUE"].ToString();
                    ViewBag.DUETODAY = rdr["DUETODAY"].ToString();
                    ViewBag.PENDING = rdr["PENDING"].ToString();
                    ViewBag.PAID = rdr["PAID"].ToString();
                    ViewBag.OSOVERDUE = Convert.ToDouble(rdr["OSOVERDUE"]).ToString("n2");
                    ViewBag.OSTODAY = Convert.ToDouble(rdr["OSTODAY"]).ToString("n2");
                    ViewBag.OSTHISMONTH = Convert.ToDouble(rdr["OSTHISMONTH"]).ToString("n2");
                    ViewBag.TotalOS = Convert.ToDouble(rdr["TotalOS"]).ToString("n2");
                }
            }
            con.Close();
            this.GetChartValues();
            return View(objlist);
        }

        [HttpPost]
        public ActionResult GetAllProducts()
        {
            List<InvoiceModel> objlist = GetSelectedItems();
            return PartialView("_ProductDetails", objlist);
        }
        public List<InvoiceModel> GetSelectedItems()
        {

            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("sp_GetInvoiceItems", con);///GetOrderItems
            int userID = Convert.ToInt32(Session["UserID"]);
            cmd.Parameters.AddWithValue("@CurrentUserID", userID);
            cmd.CommandType = CommandType.StoredProcedure;
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(ds);
            List<InvoiceModel> objList = new List<InvoiceModel>();
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
                    objinv.Dateofpayment = Convert.ToDateTime(dr["Dateofpayment"]).Date;
                if (dr["DateofAccount"] != DBNull.Value)
                    objinv.DateofAccount = Convert.ToDateTime(dr["DateofAccount"]).Date;
                objList.Add(objinv);
            }
            return objList;
        }

        public void GetChartValues()
        {
            List<string> labels = new List<string>();
            List<int> paidInvoiceCount = new List<int>();
            List<string> labels2 = new List<string>();
            List<int> DueInvoiceCount = new List<int>();
            string test = "";
            string lbl = "";
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand com = new SqlCommand("sp_getChartDetails", con);
            com.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader rdr = com.ExecuteReader())
            {
                string mnt = string.Empty;
                int cnt = 0;
                while (rdr.Read())
                {
                    mnt = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Convert.ToInt16(rdr["monthVal"])).ToString() + " " + rdr["yearVal"].ToString();
                    cnt = Convert.ToInt32(rdr["PaidInvoices"]);
                    labels.Add(mnt);
                    paidInvoiceCount.Add(cnt);
                }
                if (rdr.NextResult())
                {
                    while (rdr.Read())
                    {
                        mnt = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Convert.ToInt16(rdr["monthVal"])).ToString() + " " + rdr["yearVal"].ToString();
                        cnt = Convert.ToInt32(rdr["DueInvoices"]);
                        labels2.Add(mnt);
                        DueInvoiceCount.Add(cnt);
                    }
                }
            }
            con.Close();
           /* var result = paidInvoiceCount.ToArray()*/;
            //for (int i = 0; i < result.Length; i++)
            //{
            //    if (i == 0)
            //    {
            //        test = test + result[i].ToString();
            //    }
            //    else
            //    {
            //        test = test + "," + result[i].ToString();
            //    }
            //}
            //////Labels
            // var lblresult = labels.ToArray();
            //for (int i = 0; i < lblresult.Length; i++)
            //{
            //    if (i == 0)
            //    {
            //        lbl = lbl + lblresult[i].ToString();
            //    }
            //    else
            //    {
            //        lbl = lbl + "," + lblresult[i].ToString();
            //    }
            //}
            var result = paidInvoiceCount.ToArray();
            var lblresult = labels.ToArray();
            List<string> grp = ComputeGraphValues(result, lblresult);
            ViewBag.PaidInvoices = grp[0];
            ViewBag.Labels = grp[1];
            ///
            result = DueInvoiceCount.ToArray();
            lblresult = labels2.ToArray();
            grp = ComputeGraphValues(result, lblresult);
            ViewBag.DueInvoices = grp[0];
            ViewBag.DueLabels = grp[1];

        }

        public List<string> ComputeGraphValues( int[] result,string[] lblresult)
        {
            string test = "";
            string lbl = "";
            List<string> grValue = new List<string>();
            //  var result = paidInvoiceCount.ToArray();
            for (int i = 0; i < result.Length; i++)
            {
                if (i == 0)
                {
                    test = test + result[i].ToString();
                }
                else
                {
                    test = test + "," + result[i].ToString();
                }
            }
            grValue.Add(test);
         //   var lblresult = labels.ToArray();
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
            grValue.Add(lbl);
            return grValue;
            //ViewBag.PaidInvoices = test;
            //ViewBag.Labels = lbl;
        }


        public ActionResult GetAllItemsForUser()
        {

            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("sp_GetAllInvoiceItemsForUser", con);///GetOrderItems
            int userID = Convert.ToInt32(Session["UserID"]);
            cmd.Parameters.AddWithValue("@UserID", userID);
            cmd.CommandType = CommandType.StoredProcedure;
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(ds);
            List<InvoiceModel> objList = new List<InvoiceModel>();
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
                    objinv.Dateofpayment = Convert.ToDateTime(dr["Dateofpayment"]).Date;
                if (dr["DateofAccount"] != DBNull.Value)
                    objinv.DateofAccount = Convert.ToDateTime(dr["DateofAccount"]).Date;

                if (dr["UserName"] != DBNull.Value)
                    objinv.CurrentUserName = (dr["UserName"]).ToString();
                objinv.CurrentStatus = "Pending Approval";
                if (dr["CurrentStatus"] != DBNull.Value)
                {
                    string curSt = (dr["CurrentStatus"]).ToString();
                    if (curSt.ToUpper() == "A")
                        objinv.CurrentStatus = "Approved";
                    else if(curSt.ToUpper() == "R")
                        objinv.CurrentStatus = "Rejected";
                    else if (curSt.ToUpper() == "REASSIGN")
                        objinv.CurrentStatus = "Re Assign";
                    else if (curSt.ToUpper() == "H")
                        objinv.CurrentStatus = "On Hold";
                }
                objList.Add(objinv);
            }
            return View("MyInvoices",objList);
        }
    }
}