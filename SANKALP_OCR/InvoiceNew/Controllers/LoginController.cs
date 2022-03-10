using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using InvoiceNew.D
using System.Data;
using IQinvoiceNew.DataAccess;
using System.Globalization;
using System.Data.SqlClient;

namespace InvoiceNew.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        static string constr = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBconnection"].ConnectionString;
        static SqlConnection con = new SqlConnection(constr);
        public ActionResult Login(int? id)
        {            
            ViewBag.msg = TempData["Reset"];
            Session["UserID"] = "";
            Session["UserName"] = "";
            Session["IsAdmin"] = "0";
            Session["UserPic"] = "";
            Session["DepartmentID"] = "";
            Session["UserTypeID"] = "";
            Session["IsFinance"] = "";
            Session["IsVendor"] = "";
            Session["IsPaymentApproval"] = "";
            Session["ExcelExport"] = "";
            Session["MyDashboard"] = "MyDashboard";

            Session["DateFrom"] = "";
            Session["DateTo"] = "";
            Session["Status"] = "";                        

            ViewBag.InvoiceId = id;
            return View();
        }

        [HttpPost]
        public ActionResult Submit(FormCollection collection)
        {
            string uname = collection.Get("email");
            string pwd = collection.Get("password");
            string InvoiceID = collection.Get("hdnInvoiceID");
            string productKey = System.Configuration.ConfigurationManager.AppSettings["ProductKey"].ToString();

            LoginCredentials objLogin = new LoginCredentials();
            DataRow objUserLogedIn = objLogin.GetLogin(uname, pwd);
            if(Convert.ToInt32(objUserLogedIn["IsValid"]) != 0)
            {
                con.Open();
                List<int> branchIDs = new List<int>();
                SqlCommand cmm = new SqlCommand("getBranchSession", con);
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.Parameters.AddWithValue("@UserName",uname);
                using (SqlDataReader sdr = cmm.ExecuteReader())
                {
                    while(sdr.Read())
                    {
                       branchIDs.Add(Convert.ToInt32(sdr["BranchID"].ToString()));
                    }
                }
                con.Close();
                Session["BranchList"] = branchIDs;
            }
            //CompanyProfileDetails objLoginInfo = new CompanyProfileDetails();
            ViewBag.msg = "";

            string expiryDetail = new Crypto("!QInvoice!q$$2017").Decrypt(productKey);
            DateTime expiryDate = DateTime.ParseExact(expiryDetail, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            bool productExpired = false;


            if (expiryDate == null)
            { productExpired = true; }
            else
            {
                TimeSpan ts = expiryDate.Subtract(DateTime.Now);
                double elapsedDays = ts.TotalDays;
                if (elapsedDays < 0)
                    productExpired = true;
            }
            if (productExpired)
            {
                ViewBag.msg = "The evaluation period has expired. Please contact IQSS.";
                return View("Login");
            }
            if (objUserLogedIn == null)
            {
                ViewBag.msg = "The username or password is missing or incorrect.";
                return View("Login");
            }
            else
            {
                if (Convert.ToInt16(objUserLogedIn["IsValid"]) == 0)
                {
                    ViewBag.msg = "The username or password is missing or incorrect."; //new StrategicPlannerEntities().CustomMessagesAdditionalLanguages.Where(s => s.MessageID == 2 && s.LanguageID == lid).FirstOrDefault().CustomMessage;
                    return View("Login");
                }
                else
                {
                    ViewBag.LogedInUser = objUserLogedIn["FirstName"] + " " + objUserLogedIn["MiddleName"] + " " + objUserLogedIn["LastName"];
                    //if ((bool)(objUserLogedIn.Disabled) & (bool)(objUserLogedIn.IsAdmin))
                    //{

                    //  ViewBag.LogedInUserIsAdmin = "1";
                    //  //objLoginInfo.IsActive = "1";
                    //  //objLoginInfo.IsAdmin = "1";
                    //  Session["IsAdmin"] = "1";
                    //}

                    Session["IsAdmin"] = objUserLogedIn["IsAdmin"];

                    if (objUserLogedIn["IsVendor"] == null)
                    {
                        objUserLogedIn["IsVendor"] = false;
                    }
                    Session["IsVendor"] = objUserLogedIn["IsVendor"];

                    if (objUserLogedIn["UserImage"] != null)
                    {
                        Session["UserPic"] = objUserLogedIn["UserImage"].ToString();
                    }
                    else
                    {
                        Session["UserPic"] = "male.png";
                    }
                    Session["UserName"] = objUserLogedIn["FirstName"] + " " + objUserLogedIn["MiddleName"] + " " + objUserLogedIn["LastName"];
                    Session["UserID"] = objUserLogedIn["UserID"];

                    Session["UserTypeID"] = objUserLogedIn["UserTypeID"];

                    Session["IsFinance"] = objUserLogedIn["IsFinance"];
                    Session["IsPaymentApproval"] = objUserLogedIn["IsPaymentApproval"];

                  Session["DepartmentID"] = objUserLogedIn["DepartmentID"];
                    // Session["UserProfile"] = objUserLogedIn;
                    if (!string.IsNullOrEmpty(InvoiceID))
                    {
                        Session["InvoiceID"] = InvoiceID;
                        return RedirectToAction("InvoiceView", "Invoice", new { id = InvoiceID });
                    }
                    else
                    {
                        Session["InvoiceID"] = null;
                        return RedirectToAction("index", "Dashboard");
                    }
                }
            }
        }

        public ActionResult Index()
        {
            return View();
        }
    }

}