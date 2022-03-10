//using OfficeOpenXml;
//using InvoiceNew.BI;
using InvoiceNew.DataAccess;
using InvoiceNew.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvoiceNew.Controllers
{
    public class OrderItemsController : IQinvoiceController
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
        SqlConnection con = new SqlConnection(connectionString);
        OrderItems gblobjvendor = new OrderItems();
        // GET: OrderItems
        public ActionResult Index()
        {
            List<OrderItems> objlist = GetSelectedItems();
            return View(objlist);
        }

        public ActionResult mydynamicView()
        {
            //   con.Open();

            //   SqlCommand cmd = new SqlCommand("GetOrderItems", con);
            //    cmd.CommandType = CommandType.StoredProcedure;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            string query = "SELECT * FROM dbo.ColumnMapping";

            //using (SqlConnection sqlConn = new SqlConnection(con))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
                dt.Load(cmd.ExecuteReader());

            }
            return View(dt);
        }

        public ActionResult submitmydynamicView(FormCollection form)
        {
            int orderItems = 0;
            List<int> list = new List<int>();
            int rows = 4;
            SqlDataAdapter da = new SqlDataAdapter();
            string query = "SELECT ColumnID FROM dbo.ColumnMapping";

            //using (SqlConnection sqlConn = new SqlConnection(con))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        list.Add(rdr.GetInt32(0));
                    }
                }

            }
            con.Close();
            int[] myArray = list.ToArray();
            string val = string.Empty;
            string txt2 = Request.Form["txt_00"];
            string txt3 = Request.Form["txt_10"];
            string txt4 = Request.Form["txt_11"];
            for (int row = 1; row < rows; row++)
            {
                foreach (int c in myArray)
                {
                    string txtid = "txt_" + row.ToString() + c.ToString();
                    val = Request.Form[txtid];

                    con.Open();
                    SqlCommand cmd = new SqlCommand("InsertOrderItemDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //Double? amount = (String.IsNullOrEmpty(p.Amount) ? (Double?)null : Double.Parse(p.Amount));

                    cmd.Parameters.AddWithValue("@rowNo", row);
                    cmd.Parameters.AddWithValue("@colId", c);
                    cmd.Parameters.AddWithValue("@columnVal", val);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    /// c, row,
                    /// 


                }
            }
            return RedirectToAction("mydynamicView");
        }

        public ActionResult myView()
        {

            con.Open();

            SqlCommand cmd = new SqlCommand("GetOrderItems", con);
            cmd.CommandType = CommandType.StoredProcedure;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(dt);

            //SqlDataAdapter da = new SqlDataAdapter(cmd);
            //DataTable dt = new DataTable();
            //da.Fill(dt);
            con.Close();
            return View(dt);

        }

        ///  public static 
        public ActionResult Edit(int ID)
        {
            //string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
            //SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("GetOrderItemsBYID", con);
            cmd.Parameters.AddWithValue("@ID", ID);
            cmd.CommandType = CommandType.StoredProcedure;
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(ds);
            OrderItems objvendor = new OrderItems();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                objvendor.ID = Convert.ToInt32(dr["ID"].ToString());
                objvendor.Vendor = dr["Vendor"].ToString();
                objvendor.InvoiceNo = dr["InvoiceNo"].ToString();
                objvendor.InvoiceDate = dr["InvoiceDate"].ToString();
                objvendor.PO = dr["PO"].ToString();
                objvendor.PODate = dr["PODate"].ToString();
                objvendor.Amount = dr["Amount"].ToString();
                objvendor.PAN = dr["PAN"].ToString();
                objvendor.COMPANYVATTIN = dr["COMPANYVATTIN"].ToString();
                objvendor.COMPANYCSTNO = dr["COMPANYCSTNO"].ToString();
                objvendor.BUYERVATTIN = dr["BUYERVATTIN"].ToString();
                objvendor.BUYERCSTNO = dr["BUYERCSTNO"].ToString();
                objvendor.ImageFilePath = dr["ImageFilePath"].ToString();
            }

            List<OrderItemsDetails> objProdList = new List<OrderItemsDetails>();
            foreach (DataRow dr in ds.Tables[1].Rows)
            {

                OrderItemsDetails prd = new OrderItemsDetails();
                prd.ID = Int32.Parse(dr["ID"].ToString());
                prd.ItemName = dr["ITemNAME"].ToString();
                prd.Price = dr["PRICE"].ToString();
                prd.Qty = dr["QTY"].ToString();
                prd.Tax = dr["Tax"].ToString();
                prd.TOTAL = dr["TOTAL"].ToString();
                /*
                if (!String.IsNullOrWhiteSpace(dr["Asset"].ToString()))
                {
                    prd.Asset = Boolean.Parse(dr["Asset"].ToString());
                }
                else
                {
                    prd.Asset = false;
                }

                prd.ItemCodes = PopulateDropDown();
                */

                objProdList.Add(prd);
            }

            objvendor.ProductDetails = objProdList;
            //  con.Open();
            //cmd = new SqlCommand("SELECT * FROM DBO.VendorCodes", con);
            //cmd.CommandType = CommandType.Text;
            //SqlDataReader rdr = cmd.ExecuteReader();
            //List<SelectListItem> li = new List<SelectListItem>();


            //while (rdr.Read())
            //{

            //    li.Add(new SelectListItem { Text = rdr["Code"].ToString(), Value = rdr["ID"].ToString() });

            //}
            //con.Close();
            //ViewData["Codes"] = li;

            //ViewBag.ItemCodes = li;
            //  ViewBag.ContactRoleID = new SelectList(db.ContactRoles, "ContactRoleID", "Role", ContactModel.ContactRoleID);
            //IEnumerable<CodeValue> Colors = new List<CodeValue> {
            //        new CodeValue {
            //            CodeId = 1,
            //            Name = "Red"
            //        },
            //        new CodeValue {
            //            CodeId = 2,
            //            Name = "Blue"
            //        }
            //        };
            return View(objvendor);
        }


        public FileResult GetFile(string filepath)
        {
            filepath = "D:\\InvoiceNew\\InvoiceNew\\Files\\Temp\\62c7c5ba-787e-4857-b675-137a16e9f424\\Invoice_Bizerba.pdf";
            string fileName = Path.GetFileName(filepath);

            byte[] bytes = System.IO.File.ReadAllBytes(filepath);
            // Force the pdf document to be displayed in the browser
            Response.AppendHeader("Content-Disposition", "inline; filename=" + fileName + ";");

            return File(bytes, "application/pdf");
        }

        public List<OrderItems> GetSelectedItems()
        {

            //string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
            //SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("GetOrderItems", con);
            cmd.CommandType = CommandType.StoredProcedure;
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(ds);

            List<OrderItems> objList = new List<OrderItems>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {

                OrderItems objvendor = new OrderItems();
                objvendor.ID = Convert.ToInt32(dr["ID"].ToString());
                objvendor.Vendor = dr["Vendor"].ToString();
                objvendor.InvoiceNo = dr["InvoiceNo"].ToString();
                //     objvendor.InvoiceDate = Convert.ToDateTime(dr["InvoiceDate"]).ToString();
                objvendor.InvoiceDate = dr["InvoiceDate"].ToString();
                objvendor.PO = dr["PO"].ToString();
                objvendor.PODate = dr["PODate"].ToString();
                objvendor.Amount = dr["Amount"].ToString();
                objvendor.PAN = dr["PAN"].ToString();
                objvendor.COMPANYVATTIN = dr["COMPANYVATTIN"].ToString();
                objvendor.COMPANYCSTNO = dr["COMPANYCSTNO"].ToString();
                objvendor.BUYERVATTIN = dr["BUYERVATTIN"].ToString();
                objvendor.BUYERCSTNO = dr["BUYERCSTNO"].ToString();
                objList.Add(objvendor);

            }
            con.Close();
            return objList;

        }

        public ActionResult submit(Models.OrderItems p)
        {
            if (ModelState.IsValid)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("ID", typeof(int));
                dt.Columns.Add("ItemName", typeof(string));
                dt.Columns.Add("Price", typeof(string));
                dt.Columns.Add("Qty", typeof(string));
                dt.Columns.Add("Tax", typeof(string));
                dt.Columns.Add("Total", typeof(string));
                dt.Columns.Add("Asset", typeof(bool));

                foreach (var m in p.ProductDetails)
                {
                    //var selectedItem = m.ItemCodes.Find(x => x.Value == m.CodeId.ToString());
                    /*
                    if (selectedItem != null)
                    {
                        selectedItem.Selected = true;
                        m.Code = selectedItem.Text;
                    }
                    else
                        m.Code = "DEFAULT";
                        */
                    if (m.Tax == null)
                    {
                        m.Tax = string.Empty;
                    }
                    m.Asset = false;

                    //, Convert.ToBoolean(m.Asset.ToString(),
                    dt.Rows.Add(Convert.ToInt32(m.ID.ToString()), m.ItemName.ToString(), m.Price.ToString(), m.Qty.ToString(), m.Tax.ToString(), m.TOTAL.ToString(), Convert.ToBoolean(m.Asset));
                }
                DateTime dateValue;
                DateTime? date = null;
                DateTime? podate = null;
                if (DateTime.TryParse(p.InvoiceDate, out dateValue))
                {
                    date = dateValue;
                }
                if (DateTime.TryParse(p.PODate, out dateValue))
                {
                    podate = dateValue;
                }
                //string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
                //SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand("UpdateProductItemDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Vendor", p.Vendor);
                cmd.Parameters.AddWithValue("@InvoiceNo", p.InvoiceNo);
                cmd.Parameters.AddWithValue("@InvoiceDate", date);
                cmd.Parameters.AddWithValue("@PO", p.PO);
                //cmd.Parameters.AddWithValue("@PODate", podate);
                cmd.Parameters.AddWithValue("@Amount", Convert.ToDouble(p.Amount));
                cmd.Parameters.AddWithValue("@BUYERCSTNO", p.BUYERCSTNO);
                cmd.Parameters.AddWithValue("@BUYERVATTIN", p.BUYERVATTIN);
                cmd.Parameters.AddWithValue("@COMPANYCSTNO", p.COMPANYCSTNO);
                cmd.Parameters.AddWithValue("@COMPANYVATTIN", p.COMPANYVATTIN);
                cmd.Parameters.AddWithValue("@PAN", p.PAN);
                cmd.Parameters.AddWithValue("@ID", p.ID);
                //Pass table Valued parameter to Store Procedure
                SqlParameter sqlParam = cmd.Parameters.AddWithValue("@ItemTable", dt);
                sqlParam.SqlDbType = SqlDbType.Structured;
                cmd.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Edit", new { ID = p.ID });
        }

        public void AssetExportToExcel()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("GetAssetItemDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(ds);
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=" + HttpUtility.UrlEncode("AssetDetails.xlsx", System.Text.Encoding.UTF8));

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Maintenance Object Parts");
                ws.Cells["A1"].LoadFromDataTable(ds.Tables[0], true);
                // ws = pck.Workbook.Worksheets.Add("OrderItemDetails");
                // ws.Cells["A1"].LoadFromDataTable(ds.Tables[0], true);
                var ms = new System.IO.MemoryStream();
                pck.SaveAs(ms);
                ms.WriteTo(Response.OutputStream);
            }
        }

        public void OrderExportToExcel()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("GetAllOrderwithItemDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(ds);
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=" + HttpUtility.UrlEncode("OrderItems.xlsx", System.Text.Encoding.UTF8));

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Maintenance Object Parts");
                ws.Cells["A1"].LoadFromDataTable(ds.Tables[0], true);
                // ws = pck.Workbook.Worksheets.Add("OrderItemDetails");
                // ws.Cells["A1"].LoadFromDataTable(ds.Tables[0], true);
                var ms = new System.IO.MemoryStream();
                pck.SaveAs(ms);
                ms.WriteTo(Response.OutputStream);
            }
        }
        public void ExportToExcel()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("GetOrderDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(ds);
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=" + HttpUtility.UrlEncode("OrderItems.xlsx", System.Text.Encoding.UTF8));

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("OrderItems");
                ws.Cells["A1"].LoadFromDataTable(ds.Tables[0], true);
                ws = pck.Workbook.Worksheets.Add("OrderItemDetails");
                ws.Cells["A1"].LoadFromDataTable(ds.Tables[1], true);
                var ms = new System.IO.MemoryStream();
                pck.SaveAs(ms);
                ms.WriteTo(Response.OutputStream);
            }
        }
        public void ExportToExcelGST()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("GetOrderDetailsGST", con);
            cmd.CommandType = CommandType.StoredProcedure;
            UserRepository ur = new UserRepository();
            DataTable blist = ur.getBranchList((List<int>)Session["BranchList"]);
            cmd.Parameters.AddWithValue("@BranchList",blist);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(ds);
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=" + HttpUtility.UrlEncode("GSTFormat.xlsx", System.Text.Encoding.UTF8));

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("B2B");
                ws.Column(3).Style.Numberformat.Format = "dd-mmm-yy";
                ws.Cells["A1"].LoadFromDataTable(ds.Tables[0], true);

               // ws = pck.Workbook.Worksheets.Add("OrderItemDetails");
              //  ws.Cells["A1"].LoadFromDataTable(ds.Tables[1], true);
                var ms = new System.IO.MemoryStream();
                pck.SaveAs(ms);
                ms.WriteTo(Response.OutputStream);
            }
        }
        public ActionResult DeleteRow(int Id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            int orderItemID = 0;
            //SqlCommand cmd = new SqlCommand("UpdateProductItemDetails", con);
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@ID", id);
            SqlCommand cmd = new SqlCommand("DeleteProductItemDetailsValue", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@ID", Id));
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    orderItemID = Convert.ToInt32(rdr["OrderItemID"].ToString());
                }
            }
            con.Close();
            return RedirectToAction("Edit", new { ID = orderItemID });
        }


        public ActionResult AddNewItem(int orderItemID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("InsertProductItemDetailsValue", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@orderItemID", orderItemID));
            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("Edit", new { ID = orderItemID });
        }

        [HttpGet]
        public ActionResult Delete(string Id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            int orderItemID = 0;
            int ItemID = Convert.ToInt32(Id);
            SqlCommand cmd = new SqlCommand("DeleteProductItemDetailsValue", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@ID", Id));
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    orderItemID = Convert.ToInt32(rdr["OrderItemID"].ToString());
                }
            }
            con.Close();
            return Json(orderItemID.ToString(), JsonRequestBehavior.AllowGet);
            // return RedirectToAction("Edit", new { ID = orderItemID });
            //   return Json(orderItemID, JsonRequestBehavior.AllowGet);
        }

        public void PopulateDropdown()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM DBO.VendorCodes", con);
            cmd.CommandType = CommandType.Text;
            SqlDataReader dr = cmd.ExecuteReader();
            List<SelectListItem> li = new List<SelectListItem>();
            while (dr.Read())
            {
                li.Add(new SelectListItem { Text = dr["Code"].ToString(), Value = dr["ID"].ToString() });
            }
            con.Close();
            ViewData["Codes"] = li;
            ViewBag.ItemCodes = li;


        }
        public ActionResult CheckExistingInvoiceNo(string InvoiceNo)
        {
            bool ifInvoiceNoExist = false;
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_CheckDuplicateInvoiceNumber", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@InvoiceNo", InvoiceNo);
                ifInvoiceNoExist = (Boolean)cmd.ExecuteScalar();
                con.Close();
                return Json(!ifInvoiceNoExist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }


        private static List<SelectListItem> PopulateDropDown()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            string constr = ConfigurationManager.ConnectionStrings["MYDBconnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = " SELECT * FROM dbo.ItemCodes";
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
                                Text = sdr["Code"].ToString(),
                                Value = sdr["ID"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }
    }


    /*
    public class OrderItemsController : IQinvoiceController
    {
        
        OrderItems gblobjvendor = new OrderItems();
        // GET: OrderItems
        public ActionResult Index()
        {
            List<OrderItems> objlist = GetSelectedItems();

            return View(objlist);
        }

        public ActionResult Edit(int ID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("GetOrderItemsBYID", con);
            cmd.Parameters.AddWithValue("@ID", ID);
            cmd.CommandType = CommandType.StoredProcedure;
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(ds);
            OrderItems objvendor = new OrderItems();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                objvendor.ID = Convert.ToInt32(dr["ID"].ToString());
                objvendor.Vendor = dr["Vendor"].ToString();
                objvendor.InvoiceNo = dr["InvoiceNo"].ToString();
                objvendor.InvoiceDate = dr["InvoiceDate"].ToString();
                objvendor.PO = dr["PO"].ToString();
                objvendor.Amount = dr["Amount"].ToString();
                objvendor.PAN = dr["PAN"].ToString();
                objvendor.COMPANYVATTIN = dr["COMPANYVATTIN"].ToString();
                objvendor.COMPANYCSTNO = dr["COMPANYCSTNO"].ToString();
                objvendor.BUYERVATTIN = dr["BUYERVATTIN"].ToString();
                objvendor.BUYERCSTNO = dr["BUYERCSTNO"].ToString();
                objvendor.ImageFilePath = dr["ImageFilePath"].ToString();
            }

            List<OrderItemsDetails> objProdList = new List<OrderItemsDetails>();
            foreach (DataRow dr in ds.Tables[1].Rows)
            {

                OrderItemsDetails prd = new OrderItemsDetails();
                prd.ID = Int32.Parse(dr["ID"].ToString());
                prd.ItemName = dr["ITemNAME"].ToString();
                prd.Price = dr["PRICE"].ToString();
                prd.Qty = dr["QTY"].ToString();
                prd.Tax = dr["Tax"].ToString();
                prd.TOTAL = dr["TOTAL"].ToString();
                if (!String.IsNullOrWhiteSpace(dr["Asset"].ToString()))
                {
                    prd.Asset = Boolean.Parse(dr["Asset"].ToString());
                }
                else
                {
                    prd.Asset = false;
                }
                objProdList.Add(prd);


            }

            objvendor.ProductDetails = objProdList;

            return View(objvendor);
        }


        public FileResult GetFile(string filepath)
        {

            string fileName = Path.GetFileName(filepath);

            byte[] bytes = System.IO.File.ReadAllBytes(filepath);
            // Force the pdf document to be displayed in the browser
            Response.AppendHeader("Content-Disposition", "inline; filename=" + fileName + ";");

            return File(bytes, "application/pdf");
        }

        public List<OrderItems> GetSelectedItems()
        {

            string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("GetOrderItems", con);
            cmd.CommandType = CommandType.StoredProcedure;
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(ds);

            List<OrderItems> objList = new List<OrderItems>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {

                OrderItems objvendor = new OrderItems();
                objvendor.ID = Convert.ToInt32(dr["ID"].ToString());
                objvendor.Vendor = dr["Vendor"].ToString();
                objvendor.InvoiceNo = dr["InvoiceNo"].ToString();
                objvendor.InvoiceDate = dr["InvoiceDate"].ToString();
                objvendor.PO = dr["PO"].ToString();
                objvendor.Amount = dr["Amount"].ToString();
                objvendor.PAN = dr["PAN"].ToString();
                objvendor.COMPANYVATTIN = dr["COMPANYVATTIN"].ToString();
                objvendor.COMPANYCSTNO = dr["COMPANYCSTNO"].ToString();
                objvendor.BUYERVATTIN = dr["BUYERVATTIN"].ToString();
                objvendor.BUYERCSTNO = dr["BUYERCSTNO"].ToString();
                objList.Add(objvendor);

            }

            return objList;

        }

        public ActionResult submit(OrderItems p)
        {
            if (ModelState.IsValid)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("ID", typeof(int));
                dt.Columns.Add("ItemName", typeof(string));
                dt.Columns.Add("Price", typeof(string));
                dt.Columns.Add("Qty", typeof(string));
                dt.Columns.Add("Tax", typeof(string));
                dt.Columns.Add("Total", typeof(string));
                dt.Columns.Add("Asset", typeof(bool));

                foreach (var m in p.ProductDetails)
                {
                    if (m.Tax == null)
                    {
                        m.Tax = string.Empty;
                    }
                    dt.Rows.Add(Convert.ToInt32(m.ID.ToString()), m.ItemName.ToString(), m.Price.ToString(), m.Qty.ToString(), m.Tax.ToString(), m.TOTAL.ToString(), Convert.ToBoolean(m.Asset.ToString()));
                }
                DateTime dateValue;
                DateTime? date = null;
                if (DateTime.TryParse(p.InvoiceDate, out dateValue))
                {
                    date = dateValue;
                }
                string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand("UpdateProductItemDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Vendor", p.Vendor);
                cmd.Parameters.AddWithValue("@InvoiceNo", p.InvoiceNo);
                cmd.Parameters.AddWithValue("@InvoiceDate", date);
                cmd.Parameters.AddWithValue("@PO", p.PO);
                cmd.Parameters.AddWithValue("@Amount", Convert.ToDouble(p.Amount));
                cmd.Parameters.AddWithValue("@BUYERCSTNO", p.BUYERCSTNO);
                cmd.Parameters.AddWithValue("@BUYERVATTIN", p.BUYERVATTIN);
                cmd.Parameters.AddWithValue("@COMPANYCSTNO", p.COMPANYCSTNO);
                cmd.Parameters.AddWithValue("@COMPANYVATTIN", p.COMPANYVATTIN);
                cmd.Parameters.AddWithValue("@PAN", p.PAN);
                cmd.Parameters.AddWithValue("@ID", p.ID);
                //Pass table Valued parameter to Store Procedure
                SqlParameter sqlParam = cmd.Parameters.AddWithValue("@ItemTable", dt);
                sqlParam.SqlDbType = SqlDbType.Structured;
                cmd.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Edit", new { ID = p.ID });
        }

        public void AssetExportToExcel()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("GetAssetItemDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(ds);
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=" + HttpUtility.UrlEncode("AssetDetails.xlsx", System.Text.Encoding.UTF8));

            //using (ExcelPackage pck = new ExcelPackage())
            //{
            //    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Maintenance Object Parts");
            //    ws.Cells["A1"].LoadFromDataTable(ds.Tables[0], true);
            //    // ws = pck.Workbook.Worksheets.Add("OrderItemDetails");
            //    // ws.Cells["A1"].LoadFromDataTable(ds.Tables[0], true);
            //    var ms = new System.IO.MemoryStream();
            //    pck.SaveAs(ms);
            //    ms.WriteTo(Response.OutputStream);
            //}
        }

        public void OrderExportToExcel()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("GetAllOrderwithItemDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(ds);
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=" + HttpUtility.UrlEncode("OrderItems.xlsx", System.Text.Encoding.UTF8));

            //using (ExcelPackage pck = new ExcelPackage())
            //{
            //    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Maintenance Object Parts");
            //    ws.Cells["A1"].LoadFromDataTable(ds.Tables[0], true);
            //    // ws = pck.Workbook.Worksheets.Add("OrderItemDetails");
            //    // ws.Cells["A1"].LoadFromDataTable(ds.Tables[0], true);
            //    var ms = new System.IO.MemoryStream();
            //    pck.SaveAs(ms);
            //    ms.WriteTo(Response.OutputStream);
            //}
        }
        public void ExportToExcel()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("GetOrderDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(ds);
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=" + HttpUtility.UrlEncode("OrderItems.xlsx", System.Text.Encoding.UTF8));

            //using (ExcelPackage pck = new ExcelPackage())
            //{
            //    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("OrderItems");
            //    ws.Cells["A1"].LoadFromDataTable(ds.Tables[0], true);
            //    ws = pck.Workbook.Worksheets.Add("OrderItemDetails");
            //    ws.Cells["A1"].LoadFromDataTable(ds.Tables[1], true);
            //    var ms = new System.IO.MemoryStream();
            //    pck.SaveAs(ms);
            //    ms.WriteTo(Response.OutputStream);
            //}
        }
        public ActionResult DeleteRow(int Id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            int orderItemID =0;
            //SqlCommand cmd = new SqlCommand("UpdateProductItemDetails", con);
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@ID", id);
            SqlCommand cmd = new SqlCommand("DeleteProductItemDetailsValue", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@ID", Id));
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    orderItemID = Convert.ToInt32(rdr["OrderItemID"].ToString());
                }
            }
            con.Close();
            return RedirectToAction("Edit", new { ID = orderItemID });
        }


        public ActionResult AddNewItem(int orderItemID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand("InsertProductItemDetailsValue", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@orderItemID", orderItemID));
            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("Edit", new { ID = orderItemID });
        }

        [HttpGet]
        public ActionResult Delete(string Id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            int orderItemID = 0;
            int ItemID = Convert.ToInt32(Id);
            SqlCommand cmd = new SqlCommand("DeleteProductItemDetailsValue", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@ID", Id));
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    orderItemID = Convert.ToInt32(rdr["OrderItemID"].ToString());
                }
            }
            con.Close();
            return Json(orderItemID.ToString(), JsonRequestBehavior.AllowGet);
            // return RedirectToAction("Edit", new { ID = orderItemID });
            //   return Json(orderItemID, JsonRequestBehavior.AllowGet);
        }
    }
*/
}