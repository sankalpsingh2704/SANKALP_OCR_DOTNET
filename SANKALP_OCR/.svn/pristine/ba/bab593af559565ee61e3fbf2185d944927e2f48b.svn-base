using InvoiceNew.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvoiceNew.Controllers
{
    public class CategoryTaxController : Controller
    {
        static string constr = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBconnection"].ConnectionString;
        static SqlConnection con = new SqlConnection(constr);
        // GET: CategoryTax
        public ActionResult Category()
        {

            return View();
        }
        public ActionResult Tax()
        {

            return View();
        }

        public ActionResult Product()
        {

            return View();
        }

        public ActionResult Ledger()
        {

            return View();
        }

        // GET: CategoryTax/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        
        public JsonResult GetAllCategory()
        {
            con.Open();
            List<CategoryModel> list = new List<CategoryModel>();
            List<TaxModel> taxlist = new List<TaxModel>();
            SqlCommand cmd = new SqlCommand("sp_getAllCategoryData", con);
            cmd.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {
                    CategoryModel model = new CategoryModel();
                    model.CategoryID = Convert.ToInt32(sdr["CategoryID"]);
                    model.Description = sdr["Description"].ToString();
                    model.Name = sdr["Name"].ToString();
                    if(sdr["IsActive"].ToString() == "1")
                    {
                        model.IsActive = true;
                    }
                    model.TaxValues = sdr["TaxIDList"].ToString();

                    list.Add(model);

                }
                
            }

            foreach (var item in list)
            {
                string[] taxids = item.TaxValues.Split(',');
                cmd = new SqlCommand("sp_getOneTaxData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //List<TaxModel> tmodellist = new List<TaxModel>();
                item.TaxList = new List<TaxModel>();
                foreach (var id in taxids)
                {
                    
                    cmd.Parameters.AddWithValue("@TaxID",id);
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        TaxModel tmodel = new TaxModel();
                        while (sdr.Read())
                        {
                            tmodel.TaxID = int.Parse(sdr["TaxID"].ToString());
                            if (sdr["IsActive"].ToString() == "1")
                            {
                                tmodel.IsActive = true;
                            }
                            else
                            {
                                tmodel.IsActive = false;
                            }
                            tmodel.TaxValue = float.Parse(sdr["TaxValue"].ToString());
                            tmodel.TaxName = sdr["TaxName"].ToString();
                            item.TaxList.Add(tmodel);
                        }
                        cmd.Parameters.Clear();
                    }
                }
                
            }
            

            cmd = new SqlCommand("sp_getAllTaxData", con);
            cmd.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while(sdr.Read())
                {
                    TaxModel tax = new TaxModel();
                    tax.TaxID = int.Parse(sdr["TaxID"].ToString());
                    tax.TaxName = sdr["TaxName"].ToString();
                    tax.TaxValue = float.Parse(sdr["TaxValue"].ToString());
                    if (sdr["IsActive"].ToString() == "1")
                    {
                        tax.IsActive = true;
                    }
                    else
                    {
                        tax.IsActive = false;
                    }
                    
                    taxlist.Add(tax);
                }
            }
            con.Close();
            return Json(new { catlist = list,taxlist = taxlist },JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetAllTax()
        {
            con.Open();
            List<TaxModel> list = new List<TaxModel>();
            
            SqlCommand cmd = new SqlCommand("sp_getAllTaxData", con);
            cmd.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {
                    TaxModel model = new TaxModel();
                    model.TaxID = Convert.ToInt32(sdr["TaxID"]);
                    model.TaxValue = float.Parse(sdr["TaxValue"].ToString());
                    model.TaxName = sdr["TaxName"].ToString();
                    if (sdr["IsActive"].ToString() == "1")
                    {
                        model.IsActive = true;
                    }
                    list.Add(model);

                }
            }
            con.Close();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllLedger()
        {
            con.Open();
            List<LedgerAdminModel> list = new List<LedgerAdminModel>();

            SqlCommand cmd = new SqlCommand("sp_getAllLedgerData", con);
            cmd.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {
                    LedgerAdminModel model = new LedgerAdminModel();
                    model.LedgerID = Convert.ToInt32(sdr["LedgerID"]);
                    
                    model.LedgerName = sdr["LedgerName"].ToString();
                    if (sdr["IsActive"].ToString() == "1")
                    {
                        model.IsActive = true;
                    }
                    list.Add(model);

                }
            }
            con.Close();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllProducts(int pagenum = 1,int rows = 25)
        {
            con.Open();
            List<ProductsModel> list = new List<ProductsModel>();
           
            SqlCommand cmd = new SqlCommand("sp_getAllProductData", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PageNumber", pagenum);
            cmd.Parameters.AddWithValue("@RowsPerPage", rows);
            PageModel pmodel = new PageModel();
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                
                while (sdr.Read())
                {
                    pmodel.TotalProducts = Convert.ToInt32(sdr["TotalProducts"].ToString());
                    pmodel.CurrentPage = Convert.ToInt32(sdr["PageNumber"].ToString());
                }
                while (sdr.NextResult()) {
                    while (sdr.Read())
                    {
                        ProductsModel model = new ProductsModel();
                        model.ProductId = Convert.ToInt32(sdr["ProductId"]);
                        model.CategoryId = Convert.ToInt32(sdr["CategoryID"]);
                        model.HSNCode = sdr["HSNCode"].ToString();
                        model.ItemDescription = sdr["ItemDescription"].ToString();
                        model.ItemName = sdr["ItemName"].ToString();
                        model.UOM = sdr["UOM"].ToString();
                        model.Price = sdr["Price"].ToString();
                        if (sdr["SGST"] != DBNull.Value)
                        {
                            model.SGST = int.Parse(sdr["SGST"].ToString());
                        }
                        if (sdr["CGST"] != DBNull.Value)
                        {
                            model.CGST = int.Parse(sdr["CGST"].ToString());
                        }
                        if (sdr["IGST"] != DBNull.Value)
                        {
                            model.IGST = int.Parse(sdr["IGST"].ToString());
                        }
                        if (sdr["IsActive"].ToString() == "1")
                        {
                            model.IsActive = true;
                        }
                        list.Add(model);

                    }
                }
            }
            
            List<CategoryModel> catlist = new List<CategoryModel>();
            cmd = new SqlCommand("sp_getAllCategoryData", con);
            cmd.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {
                    CategoryModel model = new CategoryModel();
                    model.CategoryID = Convert.ToInt32(sdr["CategoryID"]);
                    
                    model.Name = sdr["Name"].ToString();
                    if (sdr["IsActive"].ToString() == "1")
                    {
                        model.IsActive = true;
                    }
                    if(model.IsActive)
                        catlist.Add(model);

                }

            }


            con.Close();
            return Json(new { pmodel, ProductList = list ,CategoryList = catlist}, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getProductSearch(string content,string col) {

            con.Open();
            List<ProductsModel> list = new List<ProductsModel>();

            SqlCommand cmd = new SqlCommand("sp_getSearchedProductData", con);
            cmd.CommandType = CommandType.StoredProcedure;
            
            cmd.Parameters.AddWithValue("@Content",content);
            cmd.Parameters.AddWithValue("@Col", col);
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {


                while (sdr.Read())
                {
                    ProductsModel model = new ProductsModel();
                    model.ProductId = Convert.ToInt32(sdr["ProductId"]);
                    model.CategoryId = Convert.ToInt32(sdr["CategoryID"]);
                    model.HSNCode = sdr["HSNCode"].ToString();
                    model.ItemDescription = sdr["ItemDescription"].ToString();
                    model.ItemName = sdr["ItemName"].ToString();
                    model.UOM = sdr["UOM"].ToString();
                    model.Price = sdr["Price"].ToString();
                    if (sdr["SGST"] != DBNull.Value)
                    {
                        model.SGST = int.Parse(sdr["SGST"].ToString());
                    }
                    if (sdr["CGST"] != DBNull.Value)
                    {
                        model.CGST = int.Parse(sdr["CGST"].ToString());
                    }
                    if (sdr["IGST"] != DBNull.Value)
                    {
                        model.IGST = int.Parse(sdr["IGST"].ToString());
                    }
                    if (sdr["IsActive"].ToString() == "1")
                    {
                        model.IsActive = true;
                    }
                    list.Add(model);

                }
            }

            List<CategoryModel> catlist = new List<CategoryModel>();
            cmd = new SqlCommand("sp_getAllCategoryData", con);
            cmd.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {
                    CategoryModel model = new CategoryModel();
                    model.CategoryID = Convert.ToInt32(sdr["CategoryID"]);

                    model.Name = sdr["Name"].ToString();
                    if (sdr["IsActive"].ToString() == "1")
                    {
                        model.IsActive = true;
                    }
                    if (model.IsActive)
                        catlist.Add(model);

                }

            }


            con.Close();
            return Json(new { ProductList = list, CategoryList = catlist }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AddCategory(CategoryModel cat)
        {
            con.Open();
            //List<CategoryModel> list = new List<CategoryModel>();
            //CategoryModel model = new CategoryModel();

           
            if (cat.InputType == "insert")
                {
                    SqlCommand cmd = new SqlCommand("sp_addCategoryData", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Name", cat?.Name??"");
                    cmd.Parameters.AddWithValue("@Description", cat?.Description??"");
                    cmd.Parameters.AddWithValue("@IsActive", cat?.IsActive);
                    cmd.Parameters.AddWithValue("@TaxIDList", cat?.TaxValues??"");
                    var id = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                }
                else
                {
                    
                    SqlCommand cmd = new SqlCommand("sp_updateCategoryData", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UpdateId",cat?.InputType??"");
                    cmd.Parameters.AddWithValue("@Name", cat?.Name??"");
                    cmd.Parameters.AddWithValue("@Description", cat?.Description??"");
                    cmd.Parameters.AddWithValue("@IsActive", cat?.IsActive);
                    cmd.Parameters.AddWithValue("@TaxIDList", cat?.TaxValues??"");
                    var id = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    
                }
            
            con.Close();
            return Json(cat);
        }
        [HttpPost]
        public JsonResult AddTax(TaxModel taxlist)
        {
            con.Open();    
            SqlCommand cmd = new SqlCommand("sp_addTaxData", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TaxName", taxlist?.TaxName??"");
            cmd.Parameters.AddWithValue("@TaxValue", taxlist?.TaxValue);
            cmd.Parameters.AddWithValue("@IsActive", taxlist?.IsActive);
            var id = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            con.Close();
            return Json(taxlist);
        }
        [HttpPost]
        public JsonResult AddLedger(LedgerAdminModel Ledger)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("sp_addLedgerData", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@LedgerName", Ledger?.LedgerName ?? "");
            cmd.Parameters.AddWithValue("@IsActive", Ledger?.IsActive);
            var id = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            con.Close();
            return Json(Ledger);
        }
        [HttpPost]
        public JsonResult updateTax(TaxModel tax)
        {

            con.Open();
            
            SqlCommand cmd = new SqlCommand("sp_updateTaxData", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UpdateID",tax.UpdateID);
            cmd.Parameters.AddWithValue("@TaxName", tax?.TaxName ?? "");
            cmd.Parameters.AddWithValue("@TaxValue", tax?.TaxValue);
            cmd.Parameters.AddWithValue("@IsActive", tax?.IsActive);
            var id = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            
            con.Close();
            return Json(tax);
        }
        [HttpPost]
        public JsonResult updateLedger(LedgerAdminModel ledger)
        {

            con.Open();

            SqlCommand cmd = new SqlCommand("sp_updateLedgerData", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UpdateID", ledger.UpdateID);
            cmd.Parameters.AddWithValue("@LedgerName", ledger?.LedgerName ?? "");
            cmd.Parameters.AddWithValue("@IsActive", ledger?.IsActive);
            var id = cmd.ExecuteScalar();
            cmd.Parameters.Clear();

            con.Close();
            return Json(ledger);
        }
        [HttpPost]
        public JsonResult AddProduct(ProductsModel catlist)
        {
            con.Open();
           
            SqlCommand cmd = new SqlCommand("sp_addProductData", con);
            cmd.CommandType = CommandType.StoredProcedure;
           
                
                cmd.Parameters.AddWithValue("@CategoryID", catlist.CategoryId);
                cmd.Parameters.AddWithValue("@HSNCode", catlist?.HSNCode??"");
                cmd.Parameters.AddWithValue("@ItemName", catlist?.ItemName??"");
                cmd.Parameters.AddWithValue("@ItemDescription", catlist?.ItemDescription??"");
                cmd.Parameters.AddWithValue("@UOM", catlist?.UOM??"");
                cmd.Parameters.AddWithValue("@Price", catlist?.Price??"");
                cmd.Parameters.AddWithValue("@SGST",catlist?.SGST??0);
                cmd.Parameters.AddWithValue("@CGST", catlist?.CGST??0);
                cmd.Parameters.AddWithValue("@IGST", catlist?.IGST??0);
                cmd.Parameters.AddWithValue("@IsActive", catlist?.IsActive);
                var id = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
          

            con.Close();
            return Json(catlist);
        }
        // GET: CategoryTax/Create
        [HttpPost]
        public JsonResult UpdateProduct(ProductsModel catlist)
        {
            con.Open();
            //List<CategoryModel> list = new List<CategoryModel>();
            //CategoryModel model = new CategoryModel();
            SqlCommand cmd = new SqlCommand("sp_updateProductData", con);
            cmd.CommandType = CommandType.StoredProcedure;
            //foreach (var item in catlist)
            //{
            cmd.Parameters.AddWithValue("@UpdateId",catlist?.UpdateId);
            cmd.Parameters.AddWithValue("@CategoryID", catlist?.CategoryId);
            cmd.Parameters.AddWithValue("@HSNCode", catlist?.HSNCode??"");
            cmd.Parameters.AddWithValue("@ItemName", catlist?.ItemName??"");
            cmd.Parameters.AddWithValue("@ItemDescription", catlist?.ItemDescription??"");
            cmd.Parameters.AddWithValue("@UOM", catlist?.UOM??"");
            cmd.Parameters.AddWithValue("@Price", catlist?.Price??"");
            cmd.Parameters.AddWithValue("@SGST", catlist?.SGST ??0);
            cmd.Parameters.AddWithValue("@CGST", catlist?.CGST ??0);
            cmd.Parameters.AddWithValue("@IGST", catlist?.IGST ??0);
            cmd.Parameters.AddWithValue("@IsActive", catlist?.IsActive);
            var id = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            //}

            con.Close();
            return Json(catlist);
        }
        public JsonResult DeleteCategory(string categoryid)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("sp_DeleteCategory", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CategoryID", categoryid);
            var id = cmd.ExecuteScalar();
            con.Close();
            return Json(id);
        }
        public JsonResult DeleteTax(string taxid)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("sp_DeleteTax", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TaxID", taxid);
            var id = cmd.ExecuteScalar();
            con.Close();
            return Json(id);
        }
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoryTax/Create
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

        // GET: CategoryTax/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CategoryTax/Edit/5
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

        // GET: CategoryTax/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CategoryTax/Delete/5
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
    }
}
