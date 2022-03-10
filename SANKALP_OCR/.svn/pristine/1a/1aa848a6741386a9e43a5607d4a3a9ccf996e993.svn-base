using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
//using InvoiceNew.BI;
using InvoiceNew.Models;
using System.IO;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Threading;
using System.Diagnostics;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Collections.Specialized;

namespace InvoiceNew.Controllers
{
    public class UploadOrderItemController : IQinvoiceController
    {

        private static VendorDetails commonVD = new VendorDetails();
        // GET: Home
        public ActionResult WebApiTest()
        {
            return View("WebApiTest");
        }
        public ActionResult SubmitAPI(FormCollection f)
        {
            return View("WebApiTest");
        }
        public ActionResult Index()
        {

            Logger.Write("Logging Started");
            VendorDetails objVd = new VendorDetails();
            ViewBag.ErrMsg = "";
            objVd.Amount = "0";
            objVd.InvoiceDate = "";
            objVd.InvoiceNo = "";
            objVd.PAN = "";
            objVd.PO = "";
            objVd.Vendor = "";
            List<Products> objProdList = new List<Products>();
            Products objpro = new Products();
            objpro.ItemName = "";
            objpro.Price = "0";
            objpro.Qty = "0";
            objProdList.Add(objpro);
            objVd.ProductDetails = objProdList;
            ViewBag.ErrMsg = TempData["ErrMessage"];

            return View("Index", objVd);
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

    
        public void PostMultipleFiles(string url, string[] files)
        {
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;
            Stream memStream = new System.IO.MemoryStream();
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition:  form-data; name=\"{0}\";\r\n\r\n{1}";
            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n Content-Type: application/octet-stream\r\n\r\n";
            memStream.Write(boundarybytes, 0, boundarybytes.Length);
            for (int i = 0; i < files.Length; i++)
            {
                string header = string.Format(headerTemplate, "file" + i, files[i]);
                //string header = string.Format(headerTemplate, "uplTheFile", files[i]);
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                memStream.Write(headerbytes, 0, headerbytes.Length);
                FileStream fileStream = new FileStream(files[i], FileMode.Open,
                FileAccess.Read);
                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStream.Write(buffer, 0, bytesRead);
                }
                memStream.Write(boundarybytes, 0, boundarybytes.Length);
                fileStream.Close();
            }
            httpWebRequest.ContentLength = memStream.Length;
            Stream requestStream = httpWebRequest.GetRequestStream();
            memStream.Position = 0;
            byte[] tempBuffer = new byte[memStream.Length];
            memStream.Read(tempBuffer, 0, tempBuffer.Length);
            memStream.Close();
            requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            requestStream.Close();
            try
            {
                WebResponse webResponse = httpWebRequest.GetResponse();
                Stream stream = webResponse.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                string var = reader.ReadToEnd();

            }
            catch (Exception ex)
            {
               // response.InnerHtml = ex.Message;
            }
            httpWebRequest = null;
        }

        public static string UploadFilesToRemoteUrl(string url, string[] files, NameValueCollection formFields = null)
        {
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "multipart/form-data; boundary=" +
                                    boundary;
            request.Method = "POST";
            request.KeepAlive = true;

            Stream memStream = new System.IO.MemoryStream();

            var boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" +
                                                                    boundary + "\r\n");
            var endBoundaryBytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" +
                                                                        boundary + "--");


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

            string headerTemplate =
                "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                "Content-Type: application/vnd.pdf\r\n\r\n";

            for (int i = 0; i < files.Length; i++)
            {
                memStream.Write(boundarybytes, 0, boundarybytes.Length);
                var header = string.Format(headerTemplate, "file", files[i]);
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
                //using (var response = request.GetResponse())
                //{
                //    Stream stream2 = response.GetResponseStream();
                //    StreamReader reader2 = new StreamReader(stream2);
                //    return reader2.ReadToEnd();
                //}
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
        public ActionResult BindPdfValue()
        {
            Logger.Write("Reached inside OCR Method");
           /// System.Diagnostics.ProcessStartInfo objProcess = new System.Diagnostics.ProcessStartInfo(); // Create the process object
            string outp = "";
            string fPath= Server.MapPath("~/Files/Temp/") + Session["Hit"] + "\\" + Session["file"];
            // string url = System.Web.Configuration.WebConfigurationManager.AppSettings["WebServiceURL"].ToString();
            ///// string url = "http://192.168.1.99:8080/OCRRestService/textcapture/extractinvoice/upload";
            // string[] fPatharray = { fPath };
            // // fPatharray[0] = fPath;
            // outp = UploadFilesToRemoteUrl(url, fPatharray);
            using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
            using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
            {
                using (Process process = new Process())
                {
                    // preparing ProcessStartInfo
                    process.StartInfo.FileName = Server.MapPath("~/OCRExtratorApp/OCRExtractor.bat");
                    process.StartInfo.Arguments = String.Format("{0} {1}", Server.MapPath("~/Files/Temp/") + Session["Hit"] + "\\" + Session["file"], " XML");

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
                        Logger.Write("going to write output");
                        Logger.Write(outp);
                    }
                    finally
                    {

                    }
                }
            }
            VendorDetails objVd = new VendorDetails();
            string op = "";
            objVd = ReadXML(outp);

            objVd.ImageFilePath = Server.MapPath("~/Files/Temp/") + Session["Hit"] + "\\" + Session["file"];
            commonVD = objVd;
            return PartialView("ItemView", objVd);

            //return Json(output, JsonRequestBehavior.AllowGet);
        }

        public VendorDetails ReadXML(string xmlString)
        {
            VendorDetails objVendor = new VendorDetails();
            try
            {
                XmlDocument xmlInputItem = new XmlDocument();
                xmlString = xmlString.Replace("&", "&amp;");
                xmlInputItem.LoadXml(xmlString);
                XmlNodeList xmlItems;
                List<Products> objProdList = new List<Products>();
                if (xmlInputItem.SelectSingleNode("INVOICE/ITEMS/ITEM") != null)
                {
                    xmlItems = xmlInputItem.SelectNodes("INVOICE/ITEMS/ITEM"); foreach (XmlNode x in xmlItems)
                    {
                        Products prd = new Products();
                        //   prd.ProductId = tempId++;
                        if (x.Attributes["NAME"] != null)
                            prd.ItemName = x.Attributes["NAME"].InnerText;
                        else
                            prd.ItemName = string.Empty;
                        // prd.Price = double.Parse(x.Attributes["PRICE"].InnerText);
                        if (x.Attributes["PRICE"] != null)
                            prd.Price = x.Attributes["PRICE"].InnerText.Replace(",", "").Trim();
                        else
                            prd.Price = string.Empty;
                        //prd.Qty =  Int32.Parse(x.Attributes["QTY"].InnerText.Replace(",","").Replace("No","").Trim());
                        if (x.Attributes["QTY"] != null)
                            prd.Qty = x.Attributes["QTY"].InnerText.Replace(",", "").Replace("No", "").Trim();
                        else
                            prd.Qty = string.Empty;
                        //prd.TOTAL = Single.Parse(x.Attributes["TOTAL"].InnerText.Replace(",", "").Trim());
                        if (x.Attributes["TOTAL"] != null)
                            prd.TOTAL = x.Attributes["TOTAL"].InnerText.Replace(",", "").Trim();
                        else
                            prd.TOTAL = string.Empty;
                        objProdList.Add(prd);
                    }
                }


                //create the DataTable that will hold the data
                if (xmlInputItem.SelectSingleNode("INVOICE") != null)
                {
                    XmlNode xmlVendor = xmlInputItem.SelectSingleNode("INVOICE");
                    if(xmlVendor.SelectSingleNode("VENDOR") != null)
                    objVendor.Vendor = xmlVendor.SelectSingleNode("VENDOR").InnerText;
                    if (xmlVendor.SelectSingleNode("INVOICENO") != null)
                        objVendor.InvoiceNo = xmlVendor.SelectSingleNode("INVOICENO").InnerText;
                    if (xmlVendor.SelectSingleNode("INVOICEDATE") != null)
                        objVendor.InvoiceDate = xmlVendor.SelectSingleNode("INVOICEDATE").InnerText;
                    if (xmlVendor.SelectSingleNode("PO") != null)
                        objVendor.PO = xmlVendor.SelectSingleNode("PO").InnerText;
                    if (xmlVendor.SelectSingleNode("AMOUNT") != null)
                        objVendor.Amount = xmlVendor.SelectSingleNode("AMOUNT").InnerText;
                    if (xmlVendor.SelectSingleNode("PAN") != null)
                        objVendor.PAN = xmlVendor.SelectSingleNode("PAN").InnerText;
                    if (xmlVendor.SelectSingleNode("BUYERCSTNO") != null)
                        objVendor.BUYERCSTNO = xmlVendor.SelectSingleNode("BUYERCSTNO").InnerText;
                    if (xmlVendor.SelectSingleNode("BUYERVATTIN") != null)
                        objVendor.BUYERVATTIN = xmlVendor.SelectSingleNode("BUYERVATTIN").InnerText;
                    if (xmlVendor.SelectSingleNode("COMPANYCSTNO") != null)
                        objVendor.COMPANYCSTNO = xmlVendor.SelectSingleNode("COMPANYCSTNO").InnerText;
                    if (xmlVendor.SelectSingleNode("COMPANYVATTIN") != null)
                        objVendor.COMPANYVATTIN = xmlVendor.SelectSingleNode("COMPANYVATTIN").InnerText;
                }
      



             

                //     int tempId = 1;

                objVendor.ProductDetails = objProdList;
                return objVendor; ;

            }
            catch (Exception ex)
            {
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
                    if (p.ProductDetails != null)
                    {
                        foreach (var m in p.ProductDetails)
                        {
                            if (m.Tax == null)
                            {
                                m.Tax = string.Empty;
                            }
                            dt.Rows.Add(m.ItemName.ToString(), m.Price.ToString(), m.Qty.ToString(), m.Tax.ToString(), m.TOTAL.ToString(), Convert.ToBoolean(m.Asset.ToString()));
                        }
                    }
                    string connectionString = ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
                    SqlConnection con = new SqlConnection(connectionString);
                    con.Open();
                    SqlCommand cmd = new SqlCommand("InsertProductItemDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    DateTime dateValue;
                    DateTime? date = null;
                    if (DateTime.TryParse(p.InvoiceDate, out dateValue))
                    {
                        date = dateValue;
                    }
                    //Double? amount = (String.IsNullOrEmpty(p.Amount) ? (Double?)null : Double.Parse(p.Amount));

                    cmd.Parameters.AddWithValue("@Vendor", p.Vendor);
                    cmd.Parameters.AddWithValue("@InvoiceNo", p.InvoiceNo);

                    cmd.Parameters.AddWithValue("@InvoiceDate", date);
                    cmd.Parameters.AddWithValue("@PO", p.PO);
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
            myObject.ProductDetails.Add(prd);
            commonVD = myObject;
            return PartialView("ItemView", myObject);
         }        //
        // POST: /Movies/Delete/5

        //    [HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirmed(int id = 0)
        //{
        //    Products movie = db.Products.Find(id);
        //    if (movie == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    db.Movies.Remove(movie);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
    }
}