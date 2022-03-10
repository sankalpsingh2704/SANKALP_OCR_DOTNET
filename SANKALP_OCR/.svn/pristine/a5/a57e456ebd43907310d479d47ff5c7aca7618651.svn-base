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
  public class CostCenterController : IQinvoiceController
    {
      /// <summary>
      /// Description:Get Method to return CostCenter view
      /// </summary>
      /// <returns></returns>
        public ActionResult CostcenterView()
        {
            return View();
        }

        /// <summary>
        /// Description: Method to fetch all CostCenter details to JQGrid
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCostCenterDetails()
        {
          try
          {
            CostCenterRepository costcenter = new CostCenterRepository();

            IEnumerable<CostCenter> costcenterlist = costcenter.getCostCenterDetails();

            int rowCounter = 0;
            var jgridData = new
            {

              total = 1,
              page = 0,
              records = 1,

              rows = (from m in costcenterlist select new { id = m.CostCenterID, slno = ++rowCounter, costcentername = m.CostCenterName, costcenterdescription = m.CostCenterDescription, costcenterdisabled = (m.Disabled.Value) ? "Yes" : "No" })

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
        /// Description: Method to save,edit and delete the Costcenter details in JQGrid
        /// parameters: string id, string costcentername, string costcenterdescription, string costcenterdisabled, string oper
        /// created by : Bharath
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveCostCenterDetails(string id, string costcentername, string costcenterdescription, string costcenterdisabled, string oper)
        {
          try
          {
            ICostCenter costcenter = new CostCenterRepository();
            if (costcentername == "" )
            {
              JsonResult jr = new JsonResult();
              jr.Data = new { msgid = 9, msg = "Costcenter name is required." };
              return jr;
            }
            
            else if (oper == "add")
            {

              CostCenter newcostcenter = new CostCenter();
              int costcount = costcenter.getCostCenterDetails().Where(c => c.CostCenterName == costcentername).Count();
              if (costcount > 0)
              {
                  JsonResult jr = new JsonResult();
                  jr.Data = new { msgid = 9, msg = "CostCenter Name exists" };
                  return jr;

              }
              newcostcenter.CostCenterName = costcentername;
              newcostcenter.CostCenterDescription = costcenterdescription;
              if (costcenterdisabled == "Yes")
              {
                newcostcenter.Disabled = true;
              }
              else
              {
                newcostcenter.Disabled = false;
              }
              newcostcenter.CreatedBy = 1;
              //newcostcenter.CreatedDate = System.DateTime.Now;
              costcenter.CreateCostCenter(newcostcenter);

            }
            else
            {
              int costcenterid = Convert.ToInt32(id);
              CostCenter editcostcenter = costcenter.getCostCenterbyid(costcenterid);
              int costcount = costcenter.getCostCenterDetails().Where(c => c.CostCenterName == costcentername && c.CostCenterID!=costcenterid).Count();
              if (oper == "edit")
              {
                  if (costcount>0)
                  {
                      JsonResult jr = new JsonResult();
                      jr.Data = new { msgid = 9, msg = "CostCenter Name exists" };
                      return jr;

                  }
                  else
                  {
                      editcostcenter.CostCenterName = costcentername;
                      editcostcenter.CostCenterDescription = costcenterdescription;
                      if (costcenterdisabled == "Yes")
                      {
                          editcostcenter.Disabled = true;
                      }
                      else
                      {
                          editcostcenter.Disabled = false;
                      }
                      editcostcenter.ModifiedBy = 1;
                      //editcostcenter.ModifiedDate = System.DateTime.Now;
                      costcenter.UpdateCostCenter(editcostcenter);

                  }
               
              }
              else if (oper == "del")
              {
                costcenter.DeleteCostCenter(costcenterid);
              }
            }
          }
          catch (Exception ex)
          {
              Logger.Write(ex.Message);
          }
          return RedirectToAction("GetCostCenterDetails");
        }
    }
}