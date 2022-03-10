using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvoiceNew.DataAccess;
using Newtonsoft.Json.Linq;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using InvoiceNew.Models;

namespace InvoiceNew.Controllers
{
   
    public class DepartmentController : IQinvoiceController
    {
        static DepartmentRepository dept = new DepartmentRepository();
        /// <summary>
        /// Description:Get Method to return Department view
        /// parameters:null
        /// created by : Bharath
        /// </summary>
        /// <returns></returns>
        public ActionResult DepartmentView()
        {
            IEnumerable<Department> departmentlist = dept.getDepartmentDetails();
            return View(departmentlist);
    }

    /// <summary>
    public JsonResult GetDepartmentDetails()
    {
      try
      { 
      

        IEnumerable<Department> departmentlist = dept.getDepartmentDetails();

        int rowCounter = 0;
        var jgridData = new
        {

          total = 1,
          page = 0,
          records = 1,

          rows = (from m in departmentlist
                  select new
                  { id = m.DeptID, slno = ++rowCounter, deptname = m.DeptName, deptdescription = m.DeptDescription,
                    deptdisabled = (m.Disabled.Value) ? "Yes" : "No" }
          )

        };
        return Json(jgridData, JsonRequestBehavior.AllowGet);
      }
      catch (Exception ex)
      {
          Logger.Write(ex.Message);
        return null;
      }


    }

    [HttpPost]
    public ActionResult SaveDepartmentDetails(string id, string deptname, string deptdescription, string deptdisabled)////,string oper
        {
      try
      {
                string oper = "edit";
        IDepartment dept = new DepartmentRepository();
        if (deptname == "" )
        {
          JsonResult jr = new JsonResult();
          jr.Data = new { msgid = 9, msg = "Department Name is required" };
          return jr;
        }

        else if (oper == "add")
        {

          Department newdept = new Department();
          var deptcount = dept.getDepartmentDetails().Where(d => d.DeptName == deptname).Count();
          if(deptcount>0)
          {
              JsonResult jr = new JsonResult();
              jr.Data = new { msgid = 9, msg = "Department Name Exist" };
              return jr;
          }
          newdept.DeptName = deptname;
          newdept.DeptDescription = deptdescription;
          if (deptdisabled == "Yes")
          {
            newdept.Disabled = true;
          }
          else
          {
            newdept.Disabled = false;
          }
          newdept.CreatedBy = 1;
          //newdept.CreatedDate = System.DateTime.Now;

          dept.CreateDepartment(newdept);

        }
        else
        {
          int departmentid = Convert.ToInt32(id);
          Department editdept = dept.getDepartmentbyid(departmentid);

          if (oper == "edit")
          {
              var deptcount = dept.getDepartmentDetails().Where(d => d.DeptName == deptname && d.DeptID!=departmentid).Count();
              if (deptcount > 0)
              {
                  JsonResult jr = new JsonResult();
                  jr.Data = new { msgid = 9, msg = "Department Name Exist" };
                  return jr;
              }
            editdept.DeptName = deptname;
            editdept.DeptDescription = deptdescription;
            if (deptdisabled == "Yes")
            {
              editdept.Disabled = true;
            }
            else
            {
              editdept.Disabled = false;
            }
            editdept.ModifiedBy = 1;
           // editdept.ModifiedDate = System.DateTime.Now;
            dept.UpdateDepartment(editdept);

          }
          else if (oper == "del")
          {
            dept.DeleteDepartment(departmentid);
          }
        }
      }
      catch (Exception ex)
      {
          Logger.Write(ex.Message);
      }
      return RedirectToAction("GetDepartmentDetails");
    }


        public ActionResult SaveDepartmentDetails()
        {
            return RedirectToAction("GetDepartmentDetails");
        }
  }
}