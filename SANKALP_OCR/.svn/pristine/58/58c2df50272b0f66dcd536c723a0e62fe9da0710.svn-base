using InvoiceNew.Models;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvoiceNew.Controllers
{
    public class BranchController : Controller
    {
        private static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
        SqlConnection sqlcon = new SqlConnection(connectionString);
        // GET: Branch
        public ActionResult Index()
        {
            return View();
        }

        // GET: Branch/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Branch/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Branch/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Branch/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Branch/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Branch/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Branch/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult BranchManagement()
        {
            return View();
        }
        public JsonResult GetBranchDetails()
        {
            try
            {
                

                IEnumerable<BranchModel> branchlist = getBranchDetails();

                
                var jgridData = new
                {

                    total = 1,
                    page = 0,
                    records = 1,

                    rows = (from m in branchlist select new { BranchID = m.BranchID, BranchName = m.BranchName, BranchAddress = m.BranchAddress, BranchAddress1 = m.BranchAddress1, BranchAddress2 = m.BranchAddress2, BranchAddress3 = m.BranchAddress3, BranchLocation = m.BranchLocation, BranchState = m.BranchState,BranchPhone = m.BranchPhone,BranchEmail = m.BranchEmail,GENPrefix = m.GENPrefix,GRNPrefix = m.GRNPrefix, BranchTIN = m.BranchTIN, CSTNumber = m.CSTNumber,GSTIN = m.GSTIN, branchdisabled = (m.Disabled.GetValueOrDefault()) ? "Yes" : "No" })

                };
                return Json(jgridData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
                return null;
            }


        }
        private IEnumerable<BranchModel> getBranchDetails()
        {
            
            List<BranchModel> objBranch = new List<BranchModel>();
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("getBranchDetails", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    BranchModel branch = new BranchModel();
                    branch.BranchName = rdr["BranchName"].ToString();
                    //branch.BranchAddress = rdr["BranchAddress"].ToString();
                    branch.BranchAddress1 = rdr["BranchAddress1"].ToString();
                    branch.BranchAddress2 = rdr["BranchAddress2"].ToString();
                    branch.BranchAddress3 = rdr["BranchAddress3"].ToString();
                    branch.BranchLocation = rdr["BranchLocation"].ToString();
                    branch.BranchState = rdr["BranchState"].ToString();
                    branch.BranchTIN = rdr["BranchTIN"].ToString();
                    branch.CSTNumber = rdr["CSTNumber"].ToString();
                    branch.GSTIN = rdr["GSTIN"].ToString();
                    branch.BranchID = int.Parse(rdr["BranchID"].ToString());
                    branch.BranchPhone = rdr["BranchPhone"].ToString();
                    branch.BranchEmail = rdr["BranchEmail"].ToString();
                    branch.GENPrefix = rdr["GENPrefix"].ToString();
                    branch.GRNPrefix = rdr["GRNPrefix"].ToString();
                    branch.Disabled = bool.Parse(rdr["Disabled"].ToString());
                    objBranch.Add(branch);
                }
            }
            sqlcon.Close();
            return objBranch;
        }
        public ActionResult SaveBranchDetails(string id, string BranchName, string BranchAddress1, string BranchAddress2, string BranchAddress3, string BranchLocation, string BranchState,string BranchPhone,string BranchEmail,string GENPrefix,string GRNPrefix, string BranchTIN, string CSTNumber, string GSTIN,string branchdisabled,string oper)
        {
            try
            {
                
                if (BranchName == "")
                {
                    JsonResult jr = new JsonResult();
                    jr.Data = new { msgid = 9, msg = "Vendor Name  is Required" };
                    return jr;
                }

                else if(oper == "add")
                {

                    BranchModel newvendor = new BranchModel();
                    int count = getBranchDetails().Where(b => b.BranchName == BranchName).Count();
                    if (count > 0)
                    {
                        JsonResult jr = new JsonResult();
                        jr.Data = new { msgid = 9, msg = "Branch Name Already Exist" };
                        return jr;
                    }
                    newvendor.BranchName = BranchName;
                    //newvendor.BranchAddress = BranchAddress;
                    newvendor.BranchAddress2 = BranchAddress2;
                    newvendor.BranchAddress3 = BranchAddress3;
                    newvendor.BranchAddress1 = BranchAddress1;
                    newvendor.BranchLocation = BranchLocation;
                    newvendor.BranchState = BranchState;
                    newvendor.BranchEmail = BranchEmail;
                    newvendor.BranchPhone = BranchPhone;
                    newvendor.GENPrefix = GENPrefix;
                    newvendor.GRNPrefix = GRNPrefix;
                    newvendor.BranchTIN = BranchTIN;
                    newvendor.GSTIN = GSTIN;
                    newvendor.CSTNumber = CSTNumber;
                    newvendor.BranchID = 0;
                    
                    if (branchdisabled == "Yes")
                    {
                        newvendor.Disabled = true;
                    }
                    else
                    {
                        newvendor.Disabled = false;
                    }
                    //newvendor.CreatedBy = 1;
                    //newvendor.ModifiedBy = 1;
                    //newvendor.CreatedDate = System.DateTime.Now;

                    AddUpdateBranches(newvendor);

                }
                
                else
                {
                    int vendorid = Convert.ToInt32(id);
                    BranchModel editbranch = getBranchbyid(vendorid);
                    int count = getBranchDetails().Where(v => v.BranchName == BranchName && v.BranchID != vendorid).Count();
                    if (oper == "edit")
                    {
                        
                        editbranch.BranchID = vendorid;
                        editbranch.BranchName = BranchName;
                        //editbranch.BranchAddress = BranchAddress;
                        editbranch.BranchAddress1 = BranchAddress1;
                        editbranch.BranchAddress2 = BranchAddress2;
                        editbranch.BranchAddress3 = BranchAddress3;
                        editbranch.BranchLocation = BranchLocation;
                        editbranch.BranchState = BranchState;
                        editbranch.BranchPhone = BranchPhone;
                        editbranch.BranchEmail = BranchEmail;
                        editbranch.GENPrefix = GENPrefix;
                        editbranch.GRNPrefix = GRNPrefix;
                        editbranch.BranchTIN = BranchTIN;
                        editbranch.CSTNumber = CSTNumber;
                        editbranch.GSTIN = GSTIN;
                        if (branchdisabled == "Yes")
                        {
                            editbranch.Disabled = true;
                        }
                        else
                        {
                            editbranch.Disabled = false;
                        }
                        AddUpdateBranches(editbranch);

                    }
                    else if (oper == "del")
                    {
                        DeleteBranch(vendorid);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
            }
            return RedirectToAction("GetBranchDetails");
        }
        private void AddUpdateBranches(BranchModel vendor)
        {
            SqlConnection sqlcon = new SqlConnection(connectionString);

            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("AddUpdateBranches", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BranchID", vendor.BranchID);
            cmd.Parameters.AddWithValue("@BranchName", vendor.BranchName);
            //cmd.Parameters.AddWithValue("@BranchAddress", vendor.BranchAddress);
            cmd.Parameters.AddWithValue("@BranchAddress1", vendor.BranchAddress1);
            cmd.Parameters.AddWithValue("@BranchAddress2", vendor.BranchAddress2);
            cmd.Parameters.AddWithValue("@BranchAddress3", vendor.BranchAddress3);
            cmd.Parameters.AddWithValue("@BranchLocation", vendor.BranchLocation);
            cmd.Parameters.AddWithValue("@CSTNumber", vendor.CSTNumber);
            cmd.Parameters.AddWithValue("@GSTIN", vendor.GSTIN);
            cmd.Parameters.AddWithValue("@BranchState", vendor.BranchState);
            cmd.Parameters.AddWithValue("@BranchPhone", vendor.BranchPhone);
            cmd.Parameters.AddWithValue("@BranchEmail", vendor.BranchEmail);
            cmd.Parameters.AddWithValue("@GENPrefix", vendor.GENPrefix);
            cmd.Parameters.AddWithValue("@GRNPrefix", vendor.GRNPrefix);
            cmd.Parameters.AddWithValue("@BranchTIN", vendor.BranchTIN);
            cmd.Parameters.AddWithValue("@Disabled", vendor.Disabled);
            cmd.ExecuteNonQuery();
            sqlcon.Close();

        }
        private BranchModel getBranchbyid(int vendorid)
        {

            SqlConnection sqlcon = new SqlConnection(connectionString);
            BranchModel vendor = new BranchModel();
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("sp_getbranchbyid", sqlcon);
            cmd.Parameters.AddWithValue("@ID",vendorid);
            cmd.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    vendor.BranchID = Convert.ToInt32(rdr["BranchID"]);
                    vendor.BranchName = rdr["BranchName"].ToString();
                    //vendor.BranchAddress = rdr["BranchAddress"].ToString();
                    vendor.BranchAddress1 = rdr["BranchAddress1"].ToString();
                    vendor.BranchAddress2 = rdr["BranchAddress2"].ToString();
                    vendor.BranchAddress3 = rdr["BranchAddress3"].ToString();
                    vendor.BranchLocation = rdr["BranchLocation"].ToString();
                    vendor.BranchState = rdr["BranchState"].ToString();
                    vendor.BranchTIN = rdr["BranchTIN"].ToString();
                    vendor.CSTNumber = rdr["CSTNumber"].ToString();
                    vendor.GSTIN = rdr["GSTIN"].ToString();
                }
            }
            sqlcon.Close();
            return vendor;
        }
        public void DeleteBranch(int vendorid)
        {
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("delete_branch", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", vendorid);
            
            cmd.ExecuteNonQuery();
            sqlcon.Close();
        }
    }
}
