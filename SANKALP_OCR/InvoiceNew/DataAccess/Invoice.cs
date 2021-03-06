using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvoiceNew.Models;
using System.Data.SqlClient;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace InvoiceNew.DataAccess
{
    public class InvoiceRepository
    {
        private static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
        SqlConnection sqlcon = new SqlConnection(connectionString);

        /// <summary>
        /// Update Status of Invoices
        /// </summary>
        /// <param name="model">InvoiceModel</param>
        /// <param name="UserID">UserID</param>
        /// <returns></returns>
        public int UpdateInvoiceStatus(InvoiceModel model, int UserID)
        {
            SqlCommand cmd = new SqlCommand("sp_UpdateInvoiceStatus", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            //   string vendor = Request.Form["vendor"];
            
            //model.InVoiceFiles = new InVoiceFile();
            string comment = string.Empty;
            cmd.Parameters.AddWithValue("@InvoiceID", model.InvoiceID);
            cmd.Parameters.AddWithValue("@userID", UserID);
            cmd.Parameters.AddWithValue("@Status", model.status);
          
            if (String.IsNullOrEmpty(model.Comment)) { model.Comment = ""; }
            cmd.Parameters.AddWithValue("@ReAssignUserID", model.ReAssignID);
            //else 
            cmd.Parameters.AddWithValue("@Comment", model.Comment);
            cmd.Parameters.AddWithValue("@Rating", model.Rating);
            cmd.Parameters.AddWithValue("@VendorID", model.VendorID);
            //model.VendorComment = "";
            if (!String.IsNullOrWhiteSpace(model.VendorComment)) { model.VendorComment = ""; }
            cmd.Parameters.AddWithValue("@VendorComment", model.VendorComment);
            

            sqlcon.Open();
            int retVal = Convert.ToInt32(cmd.ExecuteScalar());
            sqlcon.Close();
            return retVal;

        }

        public Boolean IfInvoiceNumberExists(string vendor, string InvoiceNo)
        {
            Boolean ifInvoiceNoExist = false;
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("sp_CheckDuplicateInvoiceNumber", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@VendorName", vendor);
            cmd.Parameters.AddWithValue("@InvoiceNumber", InvoiceNo);
            ifInvoiceNoExist = Convert.ToBoolean(cmd.ExecuteScalar());
            sqlcon.Close();
            return ifInvoiceNoExist;
        }
        /// <summary>
        /// get details of Debit Data
        /// </summary>
        /// <param name="invoiceid"></param>
        /// <returns>ist of DebitDataL</returns>
        public List<DebitData> getdetailsdata(int invoiceid)
        {
            List<DebitData> ObjCreditData = new List<DebitData>();
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("SP_GetDebitData", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@InvoiceId", invoiceid);
            
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {
                    DebitData cData = new Models.DebitData();
                    cData.InvoiceId = Convert.ToInt32(sdr["InvoiceID"]);
                    cData.DebitNoteId = Convert.ToInt32(sdr["DebitNoteId"]);
                    cData.DebitFileName = sdr["DebitFileName"].ToString();
                    cData.DebitAmount = Convert.ToInt64(sdr["DebitAmount"]);
                    ObjCreditData.Add(cData);
                }
            }
            sqlcon.Close();
            return ObjCreditData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceid"></param>
        /// <returns></returns>
        public IEnumerable<DebitData> getdetailscredit(int invoiceid)
        {
            List<DebitData> ObjCreditData = new List<DebitData>();
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("SP_GetDebitDetails", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@InvoiceId", invoiceid);

            List<UserTypes> objUsrList = new List<UserTypes>();
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {
                    DebitData cData = new Models.DebitData();
                    cData.InvoiceId = Convert.ToInt32(sdr["InvoiceID"]);
                    cData.DebitNoteId = Convert.ToInt32(sdr["DebitNoteId"]);
                    cData.DebitFileName = sdr["DebitFileName"].ToString();
                    cData.DebitAmount = Convert.ToDouble(sdr["DebitAmount"]);
                    ObjCreditData.Add(cData);
                }
            }
            sqlcon.Close();
            return ObjCreditData;
        }

        /// <summary>
        /// Update Debit Data
        /// </summary>
        /// <param name="invoiceid">invoiceid</param>
        /// <param name="amount">Amount</param>
        /// <returns></returns>
        public int UpdateDebitData(int invoiceid, double amount, string fileName)
        {
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("sp_insertDebitNoteDetails", sqlcon);
            int retVal = 0;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@InvoiceId", invoiceid);
            cmd.Parameters.AddWithValue("@DebitFileName", fileName);
            cmd.Parameters.AddWithValue("@DebitAmount", amount);
            retVal = cmd.ExecuteNonQuery();
            sqlcon.Close();
            return retVal;
        }

        public int UpdateDebitDetail(int debitId, double amount)
        {
            int retVal = 0;
            int Id = Convert.ToInt32(debitId);
            sqlcon.Open();
            //sp_UpdateDebitNoteDetails
            SqlCommand cmd = new SqlCommand("sp_UpdateDebitNoteDetails", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@Id", debitId);
            retVal = cmd.ExecuteNonQuery();

            sqlcon.Close();
            return retVal;
        }

        /// <summary>
        /// Delete the Debit Details of an Invoice
        /// </summary>
        /// <param name="debitId">DebitNoteId</param>
        /// <returns></returns>
        public int DeleteDebitDetail(int debitId)
        {
            int retVal = 0;
            int Id = Convert.ToInt32(debitId);
            sqlcon.Open();
            //sp_deleteDebitNotes
            SqlCommand cmd = new SqlCommand("sp_deleteDebitNotes", sqlcon);
            //int retVal = 0;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DebitNoteId", debitId);
            retVal = cmd.ExecuteNonQuery();

            sqlcon.Close();
            return retVal;

        }
        /// <summary>
        /// Validate the PO Number with Amount for new Invoice.
        /// </summary>
        /// <param name="pONumber">PO Number</param>
        /// <param name="poamount">PO Amount</param>
        /// <returns>int flag value</returns>
        public int CheckInvoiceAmount(string pONumber, Nullable<decimal> poamount)
        {
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("CheckInvoiceAmount", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PONumber", pONumber);
            cmd.Parameters.AddWithValue("@poamount", poamount);
            int retVal = Convert.ToInt32(cmd.ExecuteScalar());
            sqlcon.Close();
            return retVal;
        }

        /// <summary>
        /// SEND mail Functionality
        /// </summary>
        /// <param name="mailfrom"></param>
        /// <param name="mailto"></param>
        /// <param name="invoiceid"></param>
        /// <param name="sub"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public bool Sendmail(int mailfrom, int mailto, int invoiceid, string sub, string body)
        {
            try
            {
                if (mailto == 0)
                {
                    return false;
                }
                else
                {
                    UserRepository usr = new DataAccess.UserRepository();
                    ForgotPassword objforgotpwd = new ForgotPassword();
                    // SystemConfiguration profile = SystemConfiguration.FirstOrDefault();
                    SystemConfiguration profiles = new DataAccess.SystemConfiguration();
                    SystemConfigurationModel profile = profiles.SystemConfig();
                    string emailfrom = string.Empty;
                    emailfrom = profile.CompanyEmailFrom;
                    //var username = (from g in context.Users where g.UserID == mailfrom select new { g.FirstName, g.LastName }).ToList().FirstOrDefault();
                    //var parentuser = (from g in context.Users where g.UserID == mailto select new { g.EmailAddress }).ToList().FirstOrDefault();
                    //var invoice = (from ga in context.InVoices where ga.InvoiceID == invoiceid select ga.InvoiceNumber).ToList().FirstOrDefault();
                    string username = usr.getUserbyid(mailfrom).UserLoginName.ToString();
                    string parentuser = usr.getUserbyid(mailto).UserEmailID.ToString();

                    // Commented as per new change request
                    //var ccuser = (from g in context.Users where g.UserID == mailcc select new { g.EmailAddress }).ToList().FirstOrDefault();

                    //  string Decrypass = profile.CompanyEmailFromPassword;
                    string password = objforgotpwd.Decrypt(profile.CompanyEmailFromPassword, true);
                    string Decrypass = password;

                    MailMessage mail = new MailMessage();
                    //mail.To.Add(parentuser.EmailAddress.Trim().ToString());
                    mail.To.Add(parentuser.ToString().Trim());
                    // Commented as per new change request
                    //if (mailcc != 0 && ccuser.EmailAddress != null && ccuser.EmailAddress != string.Empty)
                    //{
                    //    mail.CC.Add(ccuser.EmailAddress.Trim().ToString());
                    //}
                    mail.From = new MailAddress(profile.CompanyEmailFrom);
                    mail.Subject = sub;


                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = profile.SmtpServer;
                    smtp.Port = int.Parse(profile.OutgoingPortNo.ToString());

                    ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                { return true; };

                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential(profile.CompanyEmailFrom, Decrypass); // Enter sender's User name and password
                    smtp.EnableSsl = bool.Parse(profile.SSL.ToString());
                    smtp.Send(mail);


                    return true;
                }
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        /// <summary>
        /// get UserId to which Invoice is assigned to
        /// </summary>
        /// <param name="invoiceId">invoiceId</param>
        /// <param name="userId">invoiceId</param>
        /// <returns></returns>
        public int GetAssignedToUserID(int invoiceId, int userId)
        {
            int retVal = 0;
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("sp_getAssignedToUserID", sqlcon);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@InvoiceId", invoiceId);
            cmd.Parameters.AddWithValue("@UserId", userId);
            retVal = Convert.ToInt32(cmd.ExecuteScalar());
            sqlcon.Close();
            return retVal;
        }

        /// <summary>
        /// Insert Invoice File
        /// </summary>
        /// <param name="inf"></param>
        /// <returns></returns>
        public bool AddInvoiceFiles(InVoiceFile inf)
        {
            try
            {
                sqlcon.Open();
                SqlCommand cmd = new SqlCommand("sp_getAssignedToUserID", sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@invoiceid", inf.InvoiceID);
                cmd.Parameters.AddWithValue("@filename", inf.FileName);
                cmd.Parameters.AddWithValue("@filetype", inf.FileType);

                Convert.ToInt32(cmd.ExecuteScalar());
                sqlcon.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// get All the InvoiceFiles
        /// </summary>
        /// <returns></returns>
        public IEnumerable<InVoiceFile> GetAllInvoiceFiles()
        {
           List<DebitData> ObjCreditData = new List<DebitData>();
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("sp_GetAllInvoiceFiles", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;

            List<InVoiceFile> objFiles = new List<InVoiceFile>();
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {
                    InVoiceFile cData = new Models.InVoiceFile();
                    cData.InvoiceID = Convert.ToInt32(sdr["InvoiceID"]);
                    cData.FileName = Convert.ToString(sdr["FileName"]);
                    cData.FileType = sdr["FileType"].ToString();
                    objFiles.Add(cData);
                }
            }
            sqlcon.Close();
            return objFiles;
        }

        /// <summary>
        /// get all Invoice Files for an Invoice based on Invoice ID
        /// </summary>
        /// <param name="invoiceid"></param>
        /// <returns></returns>
        public int getlastfileid(int invoiceid)
        {
            var InVoiceFiles = GetAllInvoiceFiles();
            if ((from i in InVoiceFiles where i.InvoiceID == invoiceid select i).Count() > 0)
            {
                string lastfile = (from i in InVoiceFiles where i.InvoiceID == invoiceid select i.FileName).First().ToString();
                string lastindex = lastfile.Substring(lastfile.LastIndexOf('-') + 1);
                string[] index = lastindex.Split('.');
                return int.Parse(index[0]);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// get Invoice Number for Invoice Files
        /// </summary>
        /// <param name="invoicenumber"></param>
        /// <returns></returns>
        public string GetInvoiceNumber(string invoicenumber)
        {
            // string invoicenumber = "INV-";
            var InVoiceFiles = GetAllInvoiceFiles();
            string format = "M-d-yy";
            int invoiceunique;
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("sp_GetInvoiceNumber", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;

            invoiceunique = Convert.ToInt32(cmd.ExecuteScalar());
            sqlcon.Close();
            if (invoiceunique <= 0)
            {
                invoiceunique = 1;
            }
            DateTime date = DateTime.Now;
            string cdate = date.ToString(format);

            return "INV-" + invoicenumber + "-" + cdate;
        }
    }
}