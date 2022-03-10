using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvoiceNew.Controllers;
using System.Configuration;
using System.Xml;
using System.IO;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Threading;
using System.Diagnostics;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using InvoiceNew.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using System.Net;

namespace InvoiceNew.Controllers
{
    public class HomeController : IQinvoiceController//Controller
    {
        static string constr = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBconnection"].ConnectionString;

        private static VendorDetails commonVD = new VendorDetails();
        static string Exeptionfrom = string.Empty;
        // GET: Home
        public ActionResult Invoice()
        {
            SqlConnection con = new SqlConnection(constr);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                Exeptionfrom = "HomeController/Invoice";
                con.Open();

                SqlCommand cmd = new SqlCommand("sp_getInvoiceDetails_2", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@InvoiceId", 2);

                da.SelectCommand = cmd;
                da.Fill(dt);
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
                            //   selectID = sdr["UsertypeID"].ToString();
                        }
                    }
                    //  con.Close();
                }
                ViewBag.Users = items;
                ////////
                con.Close();
            }
            catch (Exception ex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            return View(dt);
        }

        public ActionResult SubmitInvoice(FormCollection form)
        {
            SqlConnection con = new SqlConnection(constr);
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                /////////////////
                con.Open();
                SqlCommand cmd = new SqlCommand("InsertInvoiceItemDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                string vendor = Request.Form["vendor"];
                cmd.Parameters.AddWithValue("@InvoiceID", Request.Form["InvoiceID"]);
                cmd.Parameters.AddWithValue("@InvoiceNumber", Request.Form["InvoiceNumber"]);
                cmd.Parameters.AddWithValue("@InvoiceAmount", Request.Form["InvoiceAmount"]);
                cmd.Parameters.AddWithValue("@PONumber", Request.Form["PONumber"]);
                cmd.Parameters.AddWithValue("@PANNumber", Request.Form["PANNumber"]);
                cmd.Parameters.AddWithValue("@VendorName", Request.Form["VendorName"]);
                cmd.Parameters.AddWithValue("@InvoiceDate", Request.Form["InvoiceDate"]);
                cmd.Parameters.AddWithValue("@InvoiceDueDate", Request.Form["InvoiceDueDate"]);
                cmd.Parameters.AddWithValue("@InvoiceReceiveddate", Request.Form["InvoiceReceiveddate"]);
                cmd.Parameters.AddWithValue("@Dateofpayment", Request.Form["Dateofpayment"]);
                cmd.Parameters.AddWithValue("@DateofAccount", Request.Form["DateofAccount"]);
                DataTable dt = new DataTable();
                dt.Columns.Add("ItemName", typeof(string));
                dt.Columns.Add("Price", typeof(string));

                //if (true)
                //{
                //    foreach (var m in p.ProductDetails)
                //    {
                //        if (m.Tax == null)
                //        {
                //            m.Tax = string.Empty;
                //        }
                //        dt.Rows.Add(m.ItemName.ToString(), m.Price.ToString(), m.Qty.ToString(), m.Tax.ToString(), m.TOTAL.ToString(), Convert.ToBoolean(m.Asset.ToString()), m.Code.ToString(), m.LastIndex);
                //    }
                //}
                //Pass table Valued parameter to Store Procedure
                SqlParameter sqlParam = cmd.Parameters.AddWithValue("@ItemTable", dt);
                sqlParam.SqlDbType = SqlDbType.Structured;
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
            return View("Invoice");
        }
        public ActionResult Index()
        {
            /*
            if (Convert.ToBoolean(Session["IsAdmin"]) == false)
            {
                return RedirectToAction("Login", "Login");
            }*/
            //Logger.Write("Logging Started");
            //Logger.Write("Logging Started");
            VendorDetails objVd = new VendorDetails();
            ViewBag.ErrMsg = "";
            objVd.Amount = "0";
            objVd.InvoiceDate = "";
            objVd.InvoiceNo = "";
            objVd.PAN = "";
            objVd.PO = "";
            objVd.PODate = "";
            objVd.Vendor = "";
            List<Products> objProdList = new List<Products>();
            Products objpro = new Products();
            objpro.ItemName = "";
            objpro.Price = "0";
            objpro.Qty = "0";
            objProdList.Add(objpro);
            objVd.ProductDetails = objProdList;
            ViewBag.ErrMsg = TempData["ErrMessage"];
            ViewBag.ShowPartial = "none";
            return View("Index",objVd);
        }

        List<string> imglist = new List<string>();
        [HttpPost]
        public ActionResult NewgetImageLists(dynamic formdata)
        {
            string sessionname = Guid.NewGuid().ToString();
            Session["UserHit"] = sessionname;
            Session["Hit"] = sessionname;

            if (TempData["T"] != null)
            {
                TempData["T"] = TempData["T"] + "," + sessionname;
                TempData.Keep(); // Keep TempData
            }
            else
            {
                TempData["T"] = sessionname;
                TempData.Keep(); // Keep TempData
            }

            int n = Request.Files.Count;
            string path = Server.MapPath("~/Files/Temp/") + sessionname;
            if (n != 0)
            {
                if (!(Directory.Exists(path)))
                {
                    Directory.CreateDirectory(path);
                }
            }

            for (int i = 0; i < n; i++)
            {
                HttpPostedFileBase file = Request.Files[i];
                string src;
                var fileName = Path.GetFileName(file.FileName);
                Session["fileName"] = fileName;
                Session["file"] = fileName;// extract only the fielname
                var ext = Path.GetExtension(fileName.ToLower());

                if (ext == ".pdf")
                {
                    file.SaveAs(Server.MapPath("~/Files/Temp/") + sessionname + "/" + fileName); //File will be saved in application root
                    var filepath = Server.MapPath("~/Files/Temp/") + sessionname + "/" + fileName;
                    src = Server.MapPath(("~/Files/pdf.jpg")) + "~" + filepath;
                }
                else
                {
                    file.SaveAs(Server.MapPath("~/Files/Temp/") + sessionname + "/" + fileName); //File will be saved in application root
                    var filepath = Server.MapPath("~/Files/Temp/") + sessionname + "/" + fileName;
                    src = filepath + "~" + filepath;
                }
                imglist.Add(src);

            }
            return Json(new { imglist }, JsonRequestBehavior.AllowGet);
        }
        /*
        public ActionResult BindPdfValue()
        {
            //Logger.Write("Reached inside OCR Method");
            System.Diagnostics.ProcessStartInfo objProcess = new System.Diagnostics.ProcessStartInfo(); // Create the process object
                                                                                                        // objProcess.FileName = Server.MapPath("~/OCRExtratorApp/OCRExtractor.bat");
                                                                                                        // Logger.Write(objProcess.FileName.ToString());
                                                                                                        // // string[] path = { Server.MapPath("~/Files/Temp/") + Session["Hit"] + "\\" + Session["file"], "CSV" };
                                                                                                        // objProcess.Arguments = String.Format("{0} {1}", Server.MapPath("~/Files/Temp/") + Session["Hit"] + "\\" + Session["file"], " XML");
                                                                                                        // //objProcess.Arguments = Server.MapPath("~/Files/Temp/") + Session["Hit"] + "\\" + Session["file"];
                                                                                                        // //objProcess.Arguments += " CSV";
                                                                                                        // Logger.Write(objProcess.Arguments.ToString());
                                                                                                        // objProcess.WorkingDirectory = Server.MapPath("~/OCRExtratorApp");
                                                                                                        // objProcess.RedirectStandardOutput = true;
                                                                                                        // objProcess.RedirectStandardError = true;
                                                                                                        // objProcess.CreateNoWindow = true;
                                                                                                        // objProcess.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                                                                                                        // objProcess.UseShellExecute = false;
                                                                                                        // System.Diagnostics.Process listFiles1;
                                                                                                        // Logger.Write("OCR Calling");
                                                                                                        // listFiles1 = System.Diagnostics.Process.Start(objProcess);
                                                                                                        //  Logger.Write(listFiles1.ToString());
                                                                                                        // System.IO.StreamReader myOutput = listFiles1.StandardOutput;


            // string output = "";
            // //if (listFiles1.HasExited)
            // //{
            //     output = myOutput.ReadToEnd();
            //     Logger.Write("output" + output);

            //// }
            //     listFiles1.WaitForExit();
            // listFiles1.Close();
            string outp = ""; ;
            using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
            using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
            {
                using (Process process = new Process())
                {
                    // preparing ProcessStartInfo
                    process.StartInfo.FileName = Server.MapPath("~/OCRExtratorApp/OCRExtractor.bat");
                    string fpath = "\"" + Server.MapPath("~/Files/Temp/") + Session["Hit"] + "\\" + Session["file"] + "\"";
                    process.StartInfo.Arguments = String.Format("{0} {1}", fpath, " XML");
                    //process.StartInfo.Arguments = String.Format("{0} {1}", Server.MapPath("~/Files/Temp/") + Session["Hit"] + "\\" + Session["file"], " XML");

                    process.StartInfo.WorkingDirectory = Server.MapPath("~/OCRExtratorApp");
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    StringBuilder output = new StringBuilder();
                    StringBuilder error = new StringBuilder();
                    try
                    {
                        process.OutputDataReceived += (sender, e) =>
                        {
                            if (e.Data == null)
                            {
                                outputWaitHandle.Set();
                            }
                            else
                            {
                                output.AppendLine(e.Data);
                            }
                        };
                        process.ErrorDataReceived += (sender, e) =>
                        {
                            if (e.Data == null)
                            {
                                errorWaitHandle.Set();
                            }
                            else
                            {
                                output.AppendLine(e.Data);
                            }
                        };

                        process.Start();

                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        if (process.WaitForExit(100000) &&
             outputWaitHandle.WaitOne(100000) &&
             errorWaitHandle.WaitOne(100000))

                        {
                            // Process completed. Check process.ExitCode here.
                        }
                        else
                        {
                            // Timed out.
                        }

                        outp = output.ToString().Replace("Empty page!!", "");
                        outp = outp.Remove(outp.LastIndexOf(Environment.NewLine));
                       // Logger.Write("going to write output");
                        //Logger.Write(outp);
                    }
                    finally
                    {

                    }
                }
            }
            VendorDetails objVd = new VendorDetails();
            string op = "";
            objVd = ReadXML(outp);

            // objVd.ImageFilePath = Server.MapPath("~/Files/Temp/") + Session["Hit"] + "\\" + Session["file"];
            commonVD = objVd;
            InvoiceModel objInvoice = new InvoiceModel();
            objInvoice = ReadXMLInvoice(outp);
            PopulateDropdown();
            //return PartialView("ItemView", objVd);
            ViewBag.ShowPartial = "show";
            return PartialView("ItemViewVendor", objVd);
            //return Json(objVd);
            //return Json(output, JsonRequestBehavior.AllowGet);
        }
       */
        public ActionResult BindPdfValue()
        {
            VendorDetails objVd = new VendorDetails();

            #region Extractor


            string outp = "";
            string fPath = Server.MapPath("~/Files/Temp/") + Session["Hit"] + "\\" + Session["file"];
            //string url = System.Web.Configuration.WebConfigurationManager.AppSettings["WebServiceURL"].ToString();
            //    string url = "http://192.168.1.99:8080/OCRRestService/textcapture/extractinvoice/upload";
            //string url = "http://localhost:8080/fileuploader/integrated.php";
            //string url = "http://192.168.2.140/iqinvoiceapi/integrated.php";
            string url = "http://192.168.2.140/iqinvoiceapi/integrated.php";
            string[] fPatharray = { fPath };
            // fPatharray[0] = fPath;
            outp = UploadFilesToRemoteUrl(url, fPatharray);

            string output = outp;
            string op = "";
            objVd = ReadXML(outp);

            //     objVd.ImageFilePath = Server.MapPath("~/Files/Temp/") + Session["Hit"] + "\\" + Session["file"];
            //return Json(objVd, JsonRequestBehavior.AllowGet);

            //return Json(output, JsonRequestBehavior.AllowGet);
            ViewBag.ShowPartial = "show";
            return PartialView("ItemViewVendor", objVd);

            #endregion


        }

        public static string UploadFilesToRemoteUrl(string url, string[] files, NameValueCollection formFields = null)
        {
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            Stream memStream = new System.IO.MemoryStream();
            var boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            var endBoundaryBytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--");


            string formdataTemplate = "\r\n--" + boundary +
                                        "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";

            if (formFields != null)
            {
                foreach (string key in formFields.Keys)
                {
                    string formitem = string.Format(formdataTemplate, key, formFields[key]);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    memStream.Write(formitembytes, 0, formitembytes.Length);
                }
            }

            /*string headerTemplate =
                "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                "Content-Type: application/vnd.pdf\r\n\r\n";*/
            string headerTemplate =
               "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
               "Content-Type: application/vnd.pdf\r\n\r\n";

            for (int i = 0; i < files.Length; i++)
            {
                memStream.Write(boundarybytes, 0, boundarybytes.Length);
                var header = string.Format(headerTemplate, "image", files[i]);
                var headerbytes = System.Text.Encoding.UTF8.GetBytes(header);

                memStream.Write(headerbytes, 0, headerbytes.Length);

                using (var fileStream = new FileStream(files[i], FileMode.Open, FileAccess.Read))
                {
                    var buffer = new byte[1024];
                    var bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        memStream.Write(buffer, 0, bytesRead);
                    }
                }
            }

            memStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
            request.ContentLength = memStream.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                memStream.Position = 0;
                byte[] tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                memStream.Close();
                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            }
            try
            {
                // XDocument doc;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        // doc = XDocument.Load(stream);
                        using (var streamRdr = new StreamReader(stream))
                        {
                            var resp = streamRdr.ReadToEnd();

                            response.Close();

                            return resp;
                        }
                    }
                }

            }
            catch (WebException webex)
            {
                WebResponse errResp = webex.Response;
                using (Stream respStream = errResp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(respStream);
                    string text = reader.ReadToEnd();
                }
            }
            return "";
        }


        public InvoiceModel ReadXMLInvoice(string xmlString)
        {
            InvoiceModel objInvoice = new InvoiceModel();
            try
            {
                Exeptionfrom = "HomeController/ReadXMLInvoice";
                XmlDocument xmlInputItem = new XmlDocument();
                xmlString = xmlString.Replace("&", "&amp;");
                xmlInputItem.LoadXml(xmlString);
                XmlNodeList xmlItems;
                xmlItems = xmlInputItem.SelectNodes("INVOICE/ITEMS/ITEM");
                DateTime dateValue;
                DateTime? date = null;

                //create the DataTable that will hold the data

                XmlNode xmlVendor = xmlInputItem.SelectSingleNode("INVOICE");
                objInvoice.VendorName = xmlVendor.SelectSingleNode("VENDOR").InnerText.Trim();
                objInvoice.InvoiceNumber = xmlVendor.SelectSingleNode("INVOICENO").InnerText.Trim();

                if (DateTime.TryParse(xmlVendor.SelectSingleNode("INVOICEDATE").InnerText.Trim(), out dateValue))
                {
                    date = dateValue;
                }
                objInvoice.InvoiceDate = date;/// xmlVendor.SelectSingleNode("INVOICEDATE").InnerText.Trim();
                objInvoice.PONumber = xmlVendor.SelectSingleNode("PO").InnerText.Trim(); ;
                ///      objInvoice.InvoiceReceiveddate = xmlVendor.SelectSingleNode("PO_DATE").InnerText.Trim(); ;
                objInvoice.InvoiceAmount = xmlVendor.SelectSingleNode("AMOUNT").InnerText.Trim(); ;
                objInvoice.PANNumber = xmlVendor.SelectSingleNode("PAN").InnerText.Trim(); ;

                return objInvoice;
            }
            catch (Exception ex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
                return objInvoice;
            }
        }
        public VendorDetails ReadXML(string xmlString)
        {
            VendorDetails objVendor = new VendorDetails();
            try
            {
                Exeptionfrom = "HomeController/ReadXML";
                XmlDocument xmlInputItem = new XmlDocument();
                xmlString = xmlString.Replace("&", "&amp;");
                xmlInputItem.LoadXml(xmlString);
                XmlNodeList xmlItems;
                xmlItems = xmlInputItem.SelectNodes("INVOICE/ITEMS/ITEM");

                //create the DataTable that will hold the data

                XmlNode xmlVendor = xmlInputItem.SelectSingleNode("INVOICE");
                objVendor.Vendor = xmlVendor.SelectSingleNode("VENDOR").InnerText.Trim(); ;
                objVendor.InvoiceNo = xmlVendor.SelectSingleNode("INVOICENO").InnerText.Trim(); ;
                objVendor.InvoiceDate = xmlVendor.SelectSingleNode("INVOICEDATE").InnerText.Trim(); ;
                objVendor.PO = xmlVendor.SelectSingleNode("PO").InnerText.Trim(); ;
                //objVendor.PODate = xmlVendor.SelectSingleNode("PO_DATE").InnerText.Trim(); ;
                objVendor.Amount = xmlVendor.SelectSingleNode("AMOUNT").InnerText.Trim(); ;
                objVendor.PAN = xmlVendor.SelectSingleNode("PAN").InnerText.Trim(); ;
                //objVendor.BUYERCSTNO = xmlVendor.SelectSingleNode("BUYERCSTNO").InnerText.Trim(); ;
                //objVendor.BUYERVATTIN = xmlVendor.SelectSingleNode("BUYERVATTIN").InnerText.Trim(); ;
                //objVendor.COMPANYCSTNO = xmlVendor.SelectSingleNode("COMPANYCSTNO").InnerText.Trim(); ;
               // objVendor.COMPANYVATTIN = xmlVendor.SelectSingleNode("COMPANYVATTIN").InnerText.Trim(); ;



                List<Products> objProdList = new List<Products>();

                //     int tempId = 1;
                foreach (XmlNode x in xmlItems)
                {
                    Products prd = new Products();
                    //   prd.ProductId = tempId++;
                    if (x.Attributes["NAME"] != null)
                        prd.ItemName = x.Attributes["NAME"].InnerText.Trim();
                    else
                        prd.ItemName = string.Empty;
                    // prd.Price = double.Parse(x.Attributes["PRICE"].InnerText);
                    if (x.Attributes["PRICE"] != null)
                        prd.Price = x.Attributes["PRICE"].InnerText.Replace(",", "").Trim();
                    else
                        prd.Price = string.Empty;
                    //prd.Qty =  Int32.Parse(x.Attributes["QTY"].InnerText.Replace(",","").Replace("No","").Trim());
                    if (x.Attributes["QTY"] != null)
                        prd.Qty = x.Attributes["QTY"].InnerText.Replace(",", "").Replace("No", "").Replace("Nos", "").Replace("sqft", "").Trim();
                    else
                        prd.Qty = string.Empty;
                    //prd.TOTAL = Single.Parse(x.Attributes["TOTAL"].InnerText.Replace(",", "").Trim());
                    if (x.Attributes["TOTAL"] != null)
                        prd.TOTAL = x.Attributes["TOTAL"].InnerText.Replace(",", "").Trim();
                    else
                        prd.TOTAL = string.Empty;

                    if (x.Attributes["TAX"] != null)
                        prd.Tax = x.Attributes["TAX"].InnerText.Replace(",", "").Trim();
                    else
                        prd.Tax = string.Empty;

                    prd.ItemCodes = PopulateDropdown();
                    objProdList.Add(prd);
                }
                objVendor.ProductDetails = objProdList;
                return objVendor; ;

            }
            catch (Exception ex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
                return objVendor;
            }
        }

        [HttpPost]
        public ActionResult Submit(VendorDetails p)
        {
            //if (command == "AddNew")
            //{
            //    Products prd = new Products();
            //    p.ProductDetails.Add(prd);
            //    //commonVD.ProductDetails.Add(prd);
            //    //commonVD.ImageFilePath = Server.MapPath("~/Files/Temp/") + Session["Hit"] + "\\" + Session["file"];
            //    return PartialView("ItemView", p);
            //}
            //else
            //{
            //string outputFilePath = ConfigurationManager.AppSettings["xmlOutputFilePath"];
            //string outputFileName = ConfigurationManager.AppSettings["xmlOutputFileName"];

            //TempData["ErrMessage"] = "";

            //try
            //{
            //    XmlDocument xmlDoc = new XmlDocument();
            //    XmlNode xmlItems;
            //    XmlAttribute xmlAttrib;
            //    if (ModelState.IsValid)
            //    {
            //        XmlNode xmlItemRoot = xmlDoc.CreateElement("Invoice");
            //        xmlDoc.AppendChild(xmlItemRoot);
            //        xmlItems = xmlDoc.CreateElement("Vendor");
            //        xmlItems.InnerText = p.Vendor;
            //        xmlItemRoot.AppendChild(xmlItems);

            //        xmlItems = xmlDoc.CreateElement("InvoiceNo");
            //        xmlItems.InnerText = p.InvoiceNo;
            //        xmlItemRoot.AppendChild(xmlItems);

            //        xmlItems = xmlDoc.CreateElement("InvoiceDate");
            //        xmlItems.InnerText = p.InvoiceDate;
            //        xmlItemRoot.AppendChild(xmlItems);

            //        xmlItems = xmlDoc.CreateElement("Amount");
            //        xmlItems.InnerText = p.Amount;
            //        xmlItemRoot.AppendChild(xmlItems);

            //        xmlItems = xmlDoc.CreateElement("PAN");
            //        xmlItems.InnerText = p.PAN;
            //        xmlItemRoot.AppendChild(xmlItems);

            //        xmlItems = xmlDoc.CreateElement("ITEMS");
            //        xmlItemRoot.AppendChild(xmlItems);
            //        xmlItemRoot = xmlDoc.SelectSingleNode("Invoice/ITEMS");
            //        foreach (var m in p.ProductDetails)
            //        {
            //            xmlItems = xmlDoc.CreateElement("Item");
            //            xmlAttrib = xmlDoc.CreateAttribute("Name");
            //            xmlAttrib.Value = m.ItemName;
            //            xmlItems.Attributes.Append(xmlAttrib);

            //            xmlAttrib = xmlDoc.CreateAttribute("Price");
            //            xmlAttrib.Value = m.Price.ToString();
            //            xmlItems.Attributes.Append(xmlAttrib);
            //            xmlItemRoot.AppendChild(xmlItems);

            //            xmlAttrib = xmlDoc.CreateAttribute("Qty");
            //            xmlAttrib.Value = m.Qty.ToString();
            //            xmlItems.Attributes.Append(xmlAttrib);
            //            xmlItemRoot.AppendChild(xmlItems);

            //            xmlAttrib = xmlDoc.CreateAttribute("Total");
            //            xmlAttrib.Value = m.TOTAL.ToString();
            //            xmlItems.Attributes.Append(xmlAttrib);
            //            xmlItemRoot.AppendChild(xmlItems);

            //        }

            //        xmlDoc.Save(Server.MapPath("/" + outputFilePath + "/" + outputFileName));
            //        TempData["ErrMessage"] = "XML Saved Successfully";
            //    }
            //    else
            //    {
            //        TempData["ErrMessage"] = "Please Correct the invalid Data/File.";
            //    }
            //}
            //catch(Exception ex)
            //{
            //    TempData["ErrMessage"] = "Please Correct the invalid data";
            //}
            ////return PartialView("ItemView", p);
            //return RedirectToAction("Index");

            Boolean ifInvoiceNoExist = ifExistingInvoiceNo(p.Vendor, p.InvoiceNo);


            if (ifInvoiceNoExist)
            {
                TempData["InvoiceNoExist"] = ifInvoiceNoExist;
                ViewBag.ErrMsg = " Invoice already exists";
                ///     return RedirectToAction("Index");
                return PartialView("ItemView", p);

            }

            if (ModelState.IsValid)
            {
                if (ConfigurationManager.AppSettings["OutPutType"].ToString() == "EXCEL")
                {
                    var products = new System.Data.DataTable("ItemDetails");
                    products.Columns.Add("Name", typeof(string));
                    products.Columns.Add("Price", typeof(string));
                    products.Columns.Add("Qty", typeof(string));
                    products.Columns.Add("Total", typeof(string));

                    foreach (var m in p.ProductDetails)
                    {
                        products.Rows.Add(m.ItemName.ToString(), m.Price.ToString(), m.Qty.ToString(), m.TOTAL.ToString());
                    }

                    var grid = new GridView();
                    grid.DataSource = products;
                    grid.DataBind();

                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=MyExcelFile.xls");
                    Response.ContentType = "application/ms-excel";

                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);

                    grid.RenderControl(htw);

                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();



                    //string outputFilePath = ConfigurationManager.AppSettings["xmlOutputFilePath"];
                    //string outputFileName = ConfigurationManager.AppSettings["xmlOutputFileName"];

                    //TempData["ErrMessage"] = "";

                    //try
                    //{
                    //    XmlDocument xmlDoc = new XmlDocument();
                    //    XmlNode xmlItems;
                    //    XmlAttribute xmlAttrib;
                    //    if (ModelState.IsValid)
                    //    {
                    //        XmlNode xmlItemRoot = xmlDoc.CreateElement("Invoice");
                    //        xmlDoc.AppendChild(xmlItemRoot);
                    //        xmlItems = xmlDoc.CreateElement("Vendor");
                    //        xmlItems.InnerText = p.Vendor;
                    //        xmlItemRoot.AppendChild(xmlItems);

                    //        xmlItems = xmlDoc.CreateElement("InvoiceNo");
                    //        xmlItems.InnerText = p.InvoiceNo;
                    //        xmlItemRoot.AppendChild(xmlItems);

                    //        xmlItems = xmlDoc.CreateElement("InvoiceDate");
                    //        xmlItems.InnerText = p.InvoiceDate;
                    //        xmlItemRoot.AppendChild(xmlItems);

                    //        xmlItems = xmlDoc.CreateElement("Amount");
                    //        xmlItems.InnerText = p.Amount;
                    //        xmlItemRoot.AppendChild(xmlItems);

                    //        xmlItems = xmlDoc.CreateElement("PAN");
                    //        xmlItems.InnerText = p.PAN;
                    //        xmlItemRoot.AppendChild(xmlItems);

                    //        xmlItems = xmlDoc.CreateElement("ITEMS");
                    //        xmlItemRoot.AppendChild(xmlItems);
                    //        xmlItemRoot = xmlDoc.SelectSingleNode("Invoice/ITEMS");
                    //        foreach (var m in p.ProductDetails)
                    //        {
                    //            xmlItems = xmlDoc.CreateElement("Item");
                    //            xmlAttrib = xmlDoc.CreateAttribute("Name");
                    //            xmlAttrib.Value = m.ItemName;
                    //            xmlItems.Attributes.Append(xmlAttrib);

                    //            xmlAttrib = xmlDoc.CreateAttribute("Price");
                    //            xmlAttrib.Value = m.Price.ToString();
                    //            xmlItems.Attributes.Append(xmlAttrib);
                    //            xmlItemRoot.AppendChild(xmlItems);

                    //            xmlAttrib = xmlDoc.CreateAttribute("Qty");
                    //            xmlAttrib.Value = m.Qty.ToString();
                    //            xmlItems.Attributes.Append(xmlAttrib);
                    //            xmlItemRoot.AppendChild(xmlItems);

                    //            xmlAttrib = xmlDoc.CreateAttribute("Total");
                    //            xmlAttrib.Value = m.TOTAL.ToString();
                    //            xmlItems.Attributes.Append(xmlAttrib);
                    //            xmlItemRoot.AppendChild(xmlItems);

                    //        }

                    //        xmlDoc.Save(Server.MapPath("/" + outputFilePath + "/" + outputFileName));
                    //        TempData["ErrMessage"] = "XML Saved Successfully";
                    //    }
                    //    else
                    //    {
                    //        TempData["ErrMessage"] = "Please Correct the invalid Data/File.";
                    //    }
                }
                else if (ConfigurationManager.AppSettings["OutPutType"].ToString() == "DB")
                {

                    DataTable dt = new DataTable();
                    dt.Columns.Add("ItemName", typeof(string));
                    dt.Columns.Add("Price", typeof(string));
                    dt.Columns.Add("Qty", typeof(string));
                    dt.Columns.Add("Tax", typeof(string));
                    dt.Columns.Add("Total", typeof(string));
                    dt.Columns.Add("Asset", typeof(bool));
                    dt.Columns.Add("Code", typeof(string));
                    dt.Columns.Add("LastIndex", typeof(int));
                    if (p.ProductDetails != null)
                    {
                        foreach (var m in p.ProductDetails)
                        {
                            m.ItemCodes = PopulateDropdown();
                            var selectedItem = m.ItemCodes.Find(x => x.Value == m.CodeId.ToString());
                            if (selectedItem != null)
                            {
                                selectedItem.Selected = true;
                                m.Code = selectedItem.Text;
                            }
                            else
                                m.Code = "DEFAULT";
                            m.LastIndex = 0;

                            if (m.Tax == null)
                            {
                                m.Tax = string.Empty;
                            }
                            dt.Rows.Add(m.ItemName.ToString(), m.Price.ToString(), m.Qty.ToString(), m.Tax.ToString(), m.TOTAL.ToString(), Convert.ToBoolean(m.Asset.ToString()), m.Code.ToString(), m.LastIndex);
                        }
                    }
                    string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
                    SqlConnection con = new SqlConnection(connectionString);
                    con.Open();
                    SqlCommand cmd = new SqlCommand("InsertProductItemDetails_1", con);
                    cmd.CommandType = CommandType.StoredProcedure;
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
                    //Double? amount = (String.IsNullOrEmpty(p.Amount) ? (Double?)null : Double.Parse(p.Amount));

                    cmd.Parameters.AddWithValue("@Vendor", p.Vendor);
                    cmd.Parameters.AddWithValue("@InvoiceNo", p.InvoiceNo);

                    cmd.Parameters.AddWithValue("@InvoiceDate", date);
                    cmd.Parameters.AddWithValue("@PO", p.PO);
                    cmd.Parameters.AddWithValue("@PODate", podate);
                    //cmd.Parameters.AddWithValue("@Amount", Convert.ToDouble(p.Amount));
                    cmd.Parameters.AddWithValue("@Amount", p.Amount);
                    cmd.Parameters.AddWithValue("@BUYERCSTNO", p.BUYERCSTNO);
                    cmd.Parameters.AddWithValue("@BUYERVATTIN", p.BUYERVATTIN);
                    cmd.Parameters.AddWithValue("@COMPANYCSTNO", p.COMPANYCSTNO);
                    cmd.Parameters.AddWithValue("@COMPANYVATTIN", p.COMPANYVATTIN);
                    cmd.Parameters.AddWithValue("@PAN", p.PAN);
                    cmd.Parameters.AddWithValue("@ImageFilePath", p.ImageFilePath);

                    //Pass table Valued parameter to Store Procedure
                    SqlParameter sqlParam = cmd.Parameters.AddWithValue("@ItemTable", dt);
                    sqlParam.SqlDbType = SqlDbType.Structured;
                    cmd.ExecuteNonQuery();
                    con.Close();

                    //}
                    //catch(Exception ex)
                    //{
                    //    TempData["ErrMessage"] = "Please Correct the invalid data";
                    //}
                    //   
                    //     ViewBag.ShowPartial = "NO";
                    return RedirectToAction("Index");
                }
            }
            //   ViewBag.ShowPartial = "YES";
            return PartialView("ItemView", p);
            //  return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SubmitInvoice(InvoiceModel m)
        {
            if (ConfigurationManager.AppSettings["OutPutType"].ToString() == "DB")
            {

                string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
                SqlConnection con = new SqlConnection(connectionString);
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("sp_InsertIQInvoiceItem", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    DateTime dateValue;
                    DateTime? date = null;
                    DateTime? invRecdate = null;
                    DateTime? invDuedate = null;
                    if (DateTime.TryParse(m.InvoiceDate.ToString(), out dateValue))
                    {
                        date = dateValue;
                    }
                    if (DateTime.TryParse(m.InvoiceReceiveddate.ToString(), out dateValue))
                    {
                        invRecdate = dateValue;
                    }
                    if (DateTime.TryParse(m.InvoiceDueDate.ToString(), out dateValue))
                    {
                        invDuedate = dateValue;
                    }
                    int userID = Convert.ToInt32(Session["UserID"]);
                    //Double? amount = (String.IsNullOrEmpty(p.Amount) ? (Double?)null : Double.Parse(p.Amount));  @PANNumber
                    cmd.Parameters.AddWithValue("@InvoiceNumber", m.InvoiceNumber);
                    cmd.Parameters.AddWithValue("@PANNumber", m.PANNumber);
                    cmd.Parameters.AddWithValue("@PONumber", m.PONumber);



                    // cmd.Parameters.AddWithValue("@Vendor", m.VendorName);

                    cmd.Parameters.AddWithValue("@InvoiceDate", date);
                    cmd.Parameters.AddWithValue("@InvoiceReceiveddate", invRecdate);
                    cmd.Parameters.AddWithValue("@InvoiceDueDate", invDuedate);
                    //cmd.Parameters.AddWithValue("@Amount", Convert.ToDouble(p.Amount));
                    cmd.Parameters.AddWithValue("@InvoiceAmount", m.InvoiceAmount);
                    cmd.Parameters.AddWithValue("@CreatedBy", userID);

                    //Pass table Valued parameter to Store Procedure
                    //SqlParameter sqlParam = cmd.Parameters.AddWithValue("@ItemTable", dt);
                    //sqlParam.SqlDbType = SqlDbType.Structured;
                    cmd.ExecuteNonQuery();
                    con.Close();

                    //}
                    //catch(Exception ex)
                    //{
                    //    TempData["ErrMessage"] = "Please Correct the invalid data";
                    //}
                    //   
                    //     ViewBag.ShowPartial = "NO";
                }
                catch (Exception ex)
                {
                    Logger.Write(Exeptionfrom);
                    Logger.Write(ex.Message);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
                return RedirectToAction("Index");
            }
            return PartialView("ItemView", m);
        }
        public static List<SelectListItem> PopulateDropdown()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            string constr = ConfigurationManager.ConnectionStrings["MYDBconnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = " SELECT * FROM dbo.ItemCodes";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    try
                    {
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
                    catch (Exception ex)
                    {
                        Logger.Write(Exeptionfrom);
                        Logger.Write(ex.Message);
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                            con.Close();
                    }
                }
            }
            return items;
        }
        [HttpPost]
        public ActionResult Delete(int id = 0)
        {
            Products prd = new Products();
            commonVD.ProductDetails.RemoveAt(id - 1);
            commonVD.ImageFilePath = Server.MapPath("~/Files/Temp/") + Session["Hit"] + "\\" + Session["file"];
            return PartialView("ItemView", commonVD);
        }


        public ActionResult DeleteRow(int id)
        {
            Products prd = new Products();
            prd = commonVD.ProductDetails[id];
            commonVD.ProductDetails.Remove(prd);
            commonVD.ImageFilePath = Server.MapPath("~/Files/Temp/") + Session["Hit"] + "\\" + Session["file"];
            //   return Json(commonVD, JsonRequestBehavior.AllowGet);
            return PartialView("ItemView", commonVD);
        }
        //  
        //public ActionResult AddNewItem(string myObject)

        //{
        //    myObject = myObject.Replace(@"\", @"\\");
        //    VendorDetails VendorDetails = JsonConvert.DeserializeObject<VendorDetails>(myObject);
        //    Products prd = new Products();
        //    VendorDetails.ProductDetails.Add(prd);
        //    return PartialView("ItemView", VendorDetails);
        //}

        [HttpPost]
        public ActionResult AddNewItem(VendorDetails myObject)
        {
            Products prd = new Products();
            prd.ItemCodes = PopulateDropdown();
            if (myObject.ProductDetails != null)
            {
                myObject.ProductDetails.Add(prd);
            }
            else
            {
                List<Products> objProdList = new List<Products>();
                objProdList.Add(prd);
                myObject.ProductDetails = objProdList;
            }
            commonVD = myObject;
            ViewBag.ShowPartial = "show";
            //return PartialView("ItemView", myObject);
            return PartialView("ItemViewVendor",myObject);
        }        //


        public bool ifExistingInvoiceNo(string vendor, string InvoiceNo)
        {
            bool ifInvoiceNoExist = false;
            string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_CheckDuplicateInvoiceNumber", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@VendorName", vendor);
                cmd.Parameters.AddWithValue("@InvoiceNumber", InvoiceNo);
                ifInvoiceNoExist = Convert.ToBoolean(cmd.ExecuteScalar());
                con.Close();
            }
            catch (Exception ex)
            {
                Logger.Write(Exeptionfrom);
                Logger.Write(ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
            return ifInvoiceNoExist;

        }

        public ActionResult UserTypes()
        {
            List<UserPreferenceViewModel> usrModel = new List<UserPreferenceViewModel>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select UserTypeID,UserType,ColumnOrder from dbo.UserTypes where Disabled=0 order by ColumnOrder";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            UserPreferenceViewModel objUser = new UserPreferenceViewModel();
                            objUser.Id = (Int32)rdr["UserTypeID"];
                            objUser.Description = (String)rdr["UserType"];
                            objUser.ColumnOrder = (Int32)rdr["ColumnOrder"];
                            usrModel.Add(objUser);
                        }
                    }
                    con.Close();
                }
            }
            return View(usrModel);
        }
        public ActionResult DisabledUserTypes()
        {
            List<UserPreferenceViewModel> usrModel = new List<UserPreferenceViewModel>();

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "select UserTypeID,UserType,ColumnOrder from dbo.UserTypes where Disabled=1 order by ColumnOrder";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            UserPreferenceViewModel objUser = new UserPreferenceViewModel();
                            objUser.Id = (Int32)rdr["UserTypeID"];
                            objUser.Description = (String)rdr["UserType"];
                            objUser.ColumnOrder = (Int32)rdr["ColumnOrder"];
                            usrModel.Add(objUser);
                        }
                    }
                    con.Close();
                }
            }
            return PartialView("_DisabledUserTypes", usrModel);

        }
        //SubmitUserTypes FormCollection form 
        [HttpPost]
        public ActionResult SubmitUserTypes(IEnumerable<UserPreferenceViewModel> model)
        {
            this.disableUserTypes();
            foreach (var m in model)
            {
                int? id = m.Id;
                //if (m.ColumnOrder >= 0)
                //{
                //    clm = m.ColumnOrder;
                //}
                //else
                int clm = m.ColumnOrder;
                string disc = m.Description;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    try
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand("sp_InsertUpdateUserTypes", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@Description", disc);
                        cmd.Parameters.AddWithValue("@ColOrder", clm);
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(Exeptionfrom);
                        Logger.Write(ex.Message);
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                            con.Close();
                    }
                }
            }
            //   return View("UserTypes", model);
            return RedirectToAction("UserTypes");
        }

        [HttpPost]
        public ActionResult disableUserTypeById(string id)
        {
            int UserTypeID = Convert.ToInt32(id);
            using (SqlConnection conn = new SqlConnection(constr))
            {
                try
                {
                    string query = "UPDATE dbo.UserTypes SET Disabled=1 Where UserTypeID=" + UserTypeID;
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    Logger.Write(Exeptionfrom);
                    Logger.Write(ex.Message);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }
            return RedirectToAction("UserTypes");
        }
        public void disableUserTypes(int id = 0)
        {
            using (SqlConnection conn = new SqlConnection(constr))
            {
                try
                {
                    string query = "UPDATE dbo.UserTypes SET Disabled=1";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    Logger.Write(Exeptionfrom);
                    Logger.Write(ex.Message);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }

        }

        [HttpPost]
        public ActionResult SaveUserType(string Id, string desc)
        {
            int UserTypeID = Convert.ToInt32(Id);
            using (SqlConnection conn = new SqlConnection(constr))
            {
                try
                {
                    string query = "UPDATE dbo.UserTypes SET UserType = '" + desc + "' Where UserTypeID=" + UserTypeID;
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    Logger.Write(Exeptionfrom);
                    Logger.Write(ex.Message);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }
            return RedirectToAction("UserTypes");
        }

        [HttpPost]
        public ActionResult enableUserTypeById(string id)
        {
            int UserTypeID = Convert.ToInt32(id);
            using (SqlConnection conn = new SqlConnection(constr))
            {
                try
                {
                    string query = "UPDATE dbo.UserTypes SET Disabled=0 Where UserTypeID=" + UserTypeID;
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    Logger.Write(Exeptionfrom);
                    Logger.Write(ex.Message);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }
            return RedirectToAction("UserTypes");
        }
    }
}