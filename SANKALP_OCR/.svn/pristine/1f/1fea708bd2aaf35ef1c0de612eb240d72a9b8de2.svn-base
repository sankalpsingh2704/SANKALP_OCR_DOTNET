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
using Newtonsoft.Json;

namespace InvoiceNew.Controllers
{
    public class InvoiceController : Controller
    {
        static string constr = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBconnection"].ConnectionString;
        static SqlConnection con = new SqlConnection(constr);
        // GET: Home
        public ActionResult Index(int id=0)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("sp_getInvoiceDetails_Model", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@InvoiceId", id);
            InvoiceModel model = new InvoiceModel();
            List<UserTypes> objUsrList = new List<UserTypes>();
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
                    model.InvoiceDate = Convert.ToDateTime(sdr["InvoiceDate"]);//.ToString("dd-MMM-yyyy");
                    model.InvoiceDueDate = Convert.ToDateTime(sdr["InvoiceDate"]);
                    model.InvoiceReceiveddate = Convert.ToDateTime(sdr["InvoiceReceiveddate"]);
                    model.Dateofpayment = Convert.ToDateTime(sdr["Dateofpayment"]);
                    model.DateofAccount = Convert.ToDateTime(sdr["DateofAccount"]);
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
                        }
                 
                        objUsrList.Add(usr);
                    }
                }
                model.UserTypes = objUsrList;
            }
            /////
      
            //////////////////////


            /////
            List<SelectListItem> items = new List<SelectListItem>();
            string query = "select usr.UserID,usr.UserName,usrt.UsertypeID from dbo.[Users] usr left join [UserTypes] usrt on usr.UserTypeID=usrt.UserTypeID";
            using (SqlCommand comm = new SqlCommand(query))
            {
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
            }
            ViewBag.Users = items;
            TempData["ReAssignUsers"] = items;
            ViewBag.ReAssignUsers = TempData["ReAssignUsers"];
            ////////
            con.Close();

            return View(model);
        }

        public ActionResult InvoiceView(int id)
        {
            InvoiceModel model = new InvoiceModel();
            con.Open();
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
                        model.Dateofpayment = Convert.ToDateTime(sdr["Dateofpayment"]).Date;
                    if (sdr["DateofAccount"] != DBNull.Value)
                        model.DateofAccount = Convert.ToDateTime(sdr["DateofAccount"]).Date;
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
                model.UserTypes = objUsrList;
            }
            //////////
            List<SelectListItem> items = new List<SelectListItem>();
            string query = "select UserID,UserName from dbo.[Users] WHERE Disabled =0";
            con.Open();
            using (SqlCommand comm = new SqlCommand(query))
            {
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
            }
            con.Close();
            ViewBag.ReAssignUsers = items;/// TempData["ReAssignUsers"];
            return View(model);
        }

        [HttpPost]
        public ActionResult SubmitInvoice(InvoiceModel model)
        {
            SqlDataAdapter da = new SqlDataAdapter();

            SqlCommand cmd = new SqlCommand("sp_UpdateInvoiceStatus", con);
            cmd.CommandType = CommandType.StoredProcedure;
            //   string vendor = Request.Form["vendor"];
            string comment = string.Empty;
            cmd.Parameters.AddWithValue("@Status", model.status);
            //if(!String.IsNullOrWhiteSpace(model.Comment))
            
            //else
                cmd.Parameters.AddWithValue("@Comment", model.Comment);
            cmd.Parameters.AddWithValue("@InvoiceID", model.InvoiceID);
            cmd.Parameters.AddWithValue("@userID", Convert.ToInt32(Session["UserID"]));
            cmd.Parameters.AddWithValue("@ReAssignUserID",model.ReAssignID);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("Index", "DashBoard");
        }


        [HttpPost]
        public ActionResult getcomments(string invoiceno)
        {
            JsonResult resp = Json(new { success = 0, commentlist = "" }, JsonRequestBehavior.AllowGet);
            con.Open();
            int invoiceid = Convert.ToInt32(invoiceno);
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
            return resp;
        }

    }
}