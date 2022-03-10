using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvoiceNew.DataAccess;
using InvoiceNew.Models;
using Newtonsoft.Json.Linq;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace InvoiceNew.Controllers
{
  public class VendorController : IQinvoiceController
    {
      /// <summary>
      /// Description:Get Method to return Vendor view
      /// parameters:null
      /// created by : Bharath
      /// </summary>
      /// <returns></returns>
        public ActionResult VendorView()
        {
            return View();
        }

        /// <summary>
        /// Description: Method to fetch all Vendor details to JQGrid
        /// parameters:null
        /// created by : Bharath
        /// </summary>
        /// <returns></returns>
        public JsonResult GetVendorDetails()
        {
          try
          {
            VendorRepository vendor = new VendorRepository();
            
            IEnumerable<Vendor> vendorlist = vendor.getVendorDetails();

            int rowCounter = 0;
            var jgridData = new
            {

              total = 1,
              page = 0,
              records = 1,

              rows = (from m in vendorlist select new { id = m.VendorID, slno = ++rowCounter, vendorname = m.VendorName, vendordescription = m.VendorAddress, vendoremail=m.VendorEmail, vendorgst=m.VendorGST,vendorphone=m.VendorPhone,vendordisabled = (m.Disabled.GetValueOrDefault()) ? "Yes" : "No" })

            };
            return Json(jgridData, JsonRequestBehavior.AllowGet);
          }
          catch (Exception ex)
          {
              Logger.Write(ex.Message);
            return null;
          }


        }


        /// <summary>
        /// Description: Method to save,edit and delete the Vendor details in JQGrid
        /// parameters: string id, string vendorname, string vendordescription, string vendordisabled, string oper
        /// created by : Bharath
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveVendorDetails(string id, string vendorname, string vendordescription,string vendoremail, string vendorgst,string vendorphone, string vendordisabled, string oper)
        {
          try
          {
            VendorRepository vendor = new VendorRepository();
            if (vendorname == "" )
            {
              JsonResult jr = new JsonResult();
              jr.Data = new { msgid = 9, msg = "Vendor Name  is Required" };
              return jr;
            }

            else if (oper == "add")
            {

              Vendor newvendor = new Vendor();
              int count = vendor.getVendorDetails().Where(v => v.VendorName == vendorname).Count();
              if(count>0)
              {
                  JsonResult jr = new JsonResult();
                  jr.Data = new { msgid = 9, msg = "Vendor Name Already Exist" };
                  return jr;
              }
            newvendor.VendorName = vendorname;
            newvendor.VendorAddress = vendordescription;
            newvendor.VendorEmail = vendoremail;
            newvendor.VendorGST = vendorgst;
            newvendor.VendorPhone = vendorphone;

            newvendor.VendorID = 0;

              if (vendordisabled == "Yes")
              {
                newvendor.Disabled = true;
              }
              else
              {
                newvendor.Disabled = false;
              }
              newvendor.CreatedBy = 1;
                newvendor.ModifiedBy = 1;
                    //newvendor.CreatedDate = System.DateTime.Now;

                    vendor.AddUpdateVendors(newvendor);

            }
            else
            {
              int vendorid = Convert.ToInt32(id);
              Vendor editvendor = vendor.getVendorbyid(vendorid);
              int count = vendor.getVendorDetails().Where(v => v.VendorName == vendorname && v.VendorID != vendorid).Count();
              if (oper == "edit")
              {
                        //if(count>0)
                        //{
                        //    JsonResult jr = new JsonResult();
                        //    jr.Data = new { msgid = 9, msg = "Vendor Name Already Exist" };
                        //    return jr;
                        //}
                        editvendor.VendorID = vendorid;
                editvendor.VendorName = vendorname;
                editvendor.VendorAddress = vendordescription;
                if (vendordisabled == "Yes")
                {
                  editvendor.Disabled = true;
                }
                else
                {
                  editvendor.Disabled = false;
                }
                        editvendor.VendorEmail = vendoremail;
                        editvendor.VendorGST = vendorgst;
                        editvendor.VendorPhone = vendorphone;
                        editvendor.CreatedBy = 1;
                        editvendor.ModifiedBy = 1;
                editvendor.ModifiedDate = System.DateTime.Now;
                vendor.AddUpdateVendors(editvendor);

              }
              else if (oper == "del")
              {
                vendor.DeleteVendor(vendorid);
              }
            }
          }
          catch (Exception ex)
          {
              Logger.Write(ex.Message);
          }
          return RedirectToAction("GetVendorDetails");
        }

    }
}