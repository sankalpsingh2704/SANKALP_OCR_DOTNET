
using InvoiceNew.DataAccess;
using InvoiceNew.Models;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvoiceNew.Controllers
{
    /// <summary>
    /// PUrchase Order Details
    /// </summary>
    public class PurchaseOrderController : IQinvoiceController
    {
        PurchaseOrderRepository po = new PurchaseOrderRepository();
        string Exceptionfrom = string.Empty;
        //  IMSEntities context = new IMSEntities();
        // GET: PurchaseOrder
        public ActionResult Index()
        {
            PurchaseOrderModel objPurchase = new PurchaseOrderModel();
            return View(objPurchase);
        }
        /// <summary>
        /// Save Purchase Order Details
        /// </summary>
        /// <param name="POModel">Purchase OrderModel </param>
        /// <param name="file">File to be uploaded</param>
        /// <returns></returns>
        public ActionResult Submit(PurchaseOrderModel POModel, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Exceptionfrom = "PurchaseOrder/Submit";
                    PurchaseOrderRepository context = new PurchaseOrderRepository();
                    //PurchaseOrder po = new PurchaseOrder();
                    PurchaseOrderModel po = new PurchaseOrderModel();
                    po.PONumber = POModel.PONumber;
                    po.POAmount = POModel.POAmount;
                    po.PODate = POModel.PODate;
                    if (file != null && file.ContentLength > 0)
                    {
                        string path = Path.Combine(Server.MapPath("~/POfiles"),
                                       Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        po.POFileName = file.FileName;
                    }

                    po.Createdby = Int32.Parse(Session["UserID"].ToString());
                    context.AddPurchaseOrder(po);
                    // context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Logger.Write(Exceptionfrom);
                    Logger.Write(ex.Message);
                }
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Get the details of Purchase order
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPurchaseDetails()
        {
            try
            {
                Exceptionfrom = "PurchaseOrder/GetPurchaseDetails";
                int userID = Int32.Parse(Session["UserID"].ToString());
                PurchaseOrderRepository po = new PurchaseOrderRepository();

                UserRepository usr = new DataAccess.UserRepository();
                bool IsAdmin = Boolean.Parse(Session["IsAdmin"].ToString());
                int DeptID = Int32.Parse(Session["DepartmentID"].ToString());
                var polist = (from c in po.GetAllPurchaseDetails()
                              join u in usr.getUserDetails() on c.Createdby equals u.UserID
                              where (IsAdmin == true || DeptID == 1 || c.Createdby == userID) //// TODO && u.UserDepartmentID == DeptID
                              select c).ToList();
                return Json(polist, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                Logger.Write(Exceptionfrom);
                Logger.Write(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Download the Purchase order file
        /// </summary>
        /// <param name="id">Purchase order ID</param>
        /// <returns></returns>
        public ActionResult DownloadFile(string id)
        {
            try
            {
                Exceptionfrom = "PurchaseOrder/DownloadFile";
                PurchaseOrderRepository po = new PurchaseOrderRepository();
                int ids = Convert.ToInt32(id);
                // IMSEntities db = new IMSEntities();
                //var result = db.PurchaseOrders.SingleOrDefault(b => b.POID == ids);
                var result = po.GetAllPurchaseDetails().SingleOrDefault(b => b.POID == ids);
                FileStream fs = new FileStream(Server.MapPath(@"~\POfiles\" + result.POFileName + ""), FileMode.Open, FileAccess.Read);
                string pdfFilePath = Server.MapPath(@"~\POfiles\" + result.POFileName + "");
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + result.POFileName + "");
                Response.TransmitFile(pdfFilePath);
                Response.End();
            }
            catch (Exception ex)
            {
                Logger.Write(Exceptionfrom);
                Logger.Write(ex.Message);

            }
            return null;
        }

        /// <summary>
        /// Delete a Purchase order
        /// </summary>
        /// <param name="poid">Purchase orderID</param>
        /// <returns></returns>
        public JsonResult DeletePO(string poid)
        {
            string message = "Some error Occurred While Deleted";
            try
            {
                Exceptionfrom = "PurchaseOrder/DeletePO";
                PurchaseOrderRepository po = new PurchaseOrderRepository();

                int id = Convert.ToInt32(poid);
                if (po.DeletePurchaseOrder(id) > 0)
                {
                    message = "Purchase Order with ID " + poid + " Deleted";
                }
            }
            catch (Exception ex)
            {
                Logger.Write(Exceptionfrom);
                Logger.Write(ex.Message);

            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Update Purchase order details
        /// </summary>
        /// <param name="poid">Purchase orderID</param>
        /// <param name="amount">Purchase order Amount</param>
        /// <returns></returns>
        public JsonResult UpdatePO(string poid, string amount)
        {
            PurchaseOrderRepository po = new PurchaseOrderRepository();
            string message = "Failure";
            try
            {
                Exceptionfrom = "PurchaseOrder/UpdatePO";
                int Id = Convert.ToInt32(poid);
                decimal POAmount = Convert.ToDecimal(amount);
                if (po.UpdatePurchaseOrder(Id, POAmount) > 0)
                {
                    message = "Success";
                }
            }
            catch (Exception ex)
            {
                Logger.Write(Exceptionfrom);
                Logger.Write(ex.Message);

            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Check for DUplicate Purchase Order
        /// </summary>
        /// <param name="PONumber">Purchase Order Number</param>
        /// <returns></returns>
        public ActionResult CheckPONumber(string PONumber)
        {
            bool flag = false;
            try
            {
                Exceptionfrom = "PurchaseOrder/CheckPONumber";
                flag = po.ifPONumberExists(PONumber);
                return Json(!flag, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Write(Exceptionfrom);
                Logger.Write(ex.Message);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

    }
}