using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvoiceNew.Models;
using System.Data.SqlClient;
using System.Data;

namespace InvoiceNew.Controllers
{
    public class UserRoleController : Controller
    {
      static string constr = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBconnection"].ConnectionString;
        // GET: UserRole
        public ActionResult Index()
        {
            List<UserRoleModel> objUserList = new List<UserRoleModel>();
 
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select UserTypeID,UserType from dbo.UserTypes";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            UserRoleModel objUser = new UserRoleModel();
                            objUser.ID= (Int32)rdr["UserTypeID"];
                            objUser.UserType = (String)rdr["UserType"];
                            objUser.RoleId = "2";
                            // objUser.UserRoles = PopulateDropdown();
                            //   ViewBag.Roles = PopulateDropdown();
                            objUser= PopulateDropdown(objUser);
                            objUserList.Add(objUser);
                        }
                    }
                    con.Close();
                }
            }
            return View(objUserList);
        }
        public UserRoleModel PopulateDropdown(UserRoleModel model)
        {
            string selectID = "0";
            List<SelectListItem> items = new List<SelectListItem>();
            //       string constr = ConfigurationManager.ConnectionStrings["MYDBconnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select usr.UserID,usr.UserName,usrt.UsertypeID from dbo.[Users] usr left join [UserTypes] usrt on usr.UserTypeID=usrt.UserTypeID";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                  
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new SelectListItem
                            {
                                Text = sdr["UserName"].ToString(),
                                Value = sdr["UserID"].ToString()
                            });
                            selectID = sdr["UsertypeID"].ToString();
                        }
                    }
                    con.Close();
                }
            }
            model.RoleId = selectID;
            ViewBag.Roles = items;
            return model;
        }

        public ActionResult Submit(List<UserRoleModel> lstuser)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ItemName", typeof(string));
            dt.Columns.Add("Price", typeof(string));
            foreach ( var m in lstuser)
            {
     
            }
            return RedirectToAction("Index");
        }


        public static List<SelectListItem> PopulateDropdown()
        {
            List<SelectListItem> items = new List<SelectListItem>();
     //       string constr = ConfigurationManager.ConnectionStrings["MYDBconnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select usr.UserID,usr.UserName,usrt.UsertypeID from dbo.[Users] usr left join [UserTypes] usrt on usr.UserTypeID=usrt.UserTypeID";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
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
                }
            }
            return items;
        }
    }
}