using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Data;
using InvoiceNew.DataAccess;
using InvoiceNew.Models;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace InvoiceNew.Controllers
{
    public class ForgotPasswordController : IQinvoiceController
    {
        // GET: Forgotpassword
        public ActionResult forgotpasswordview()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Submit(FormCollection collection)
        {
            string useremail = collection.Get("email");
            string username = collection.Get("username");
            ViewBag.msg = "";
            if(String.IsNullOrWhiteSpace(useremail))
            {
                useremail = string.Empty;
            }
            if (String.IsNullOrWhiteSpace(username))
            {
                username = string.Empty;
            }
            ForgotPassword objresetPassword = new ForgotPassword();
            DataRow objResetPass = objresetPassword.ResetPassword(username, useremail);
            SystemConfiguration sysconfig = new SystemConfiguration();
            //IEmail sysconfig = new EmailSettings(new IMSEntities());
           /* DataRow configdetails = sysconfig.SystemConfig()*/;///.CompanyEmailFrom;
            SystemConfigurationModel configdetails = sysconfig.SystemConfig();
            if (objResetPass["ReturnValue"].ToString() != "0")
            {
                try
                {
                 ///   var configdetails = sysconfig.SelectAllEmails().FirstOrDefault();
                    MailMessage mail = new MailMessage();
                    mail.To.Add(objResetPass["EmailID"].ToString());
                    mail.From = new MailAddress(configdetails.CompanyEmailFrom.ToString());////configdetails["CompanyEmailFrom"]
                    mail.Subject = "Password Reset";
                    string Body = "Password Reset Successfully. Your New password is: " + objResetPass["ReturnValue"].ToString();
                    mail.Body = Body;
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();

                    smtp.Host = configdetails.SmtpServer.ToString();////configdetails["SmtpServer"]
                    smtp.Port = Convert.ToInt32(configdetails.OutgoingPortNo.ToString());
                    if (Convert.ToBoolean(configdetails.SSL) == true)
                    {
                        smtp.EnableSsl = true;
                    }
                    else
                    {
                        smtp.EnableSsl = false;
                    }

                    string password = objresetPassword.Decrypt(configdetails.CompanyEmailFromPassword.ToString(), true);////configdetails["CompanyEmailFromPassword"]
                    string Decrypass = password;// configdetails.CompanyEmailFromPassword;


                    ServicePointManager.ServerCertificateValidationCallback =
         delegate (object s, X509Certificate certificate,X509Chain chain, SslPolicyErrors sslPolicyErrors)
         { return true; };

                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential(configdetails.CompanyEmailFrom.ToString(), Decrypass);// Enter seders User name and password
                    smtp.Send(mail);
                    ViewBag.msg = ""; // SPBaseController.getCM(4, lid);
                    string msg = "Password has been reset successfully, and sent to your registered eMail ID";
                    TempData.Add("Reset", msg);

                }
                catch (Exception ex)
                {
                    Logger.Write(ex.Message);
                    string message = ex.InnerException.ToString();
                    Logger.Write(message);
                }


            }
            else
            {
                ViewBag.msg = "Given User Name or email ID not registered. Password can't Reset";
                return View("forgotpasswordview");
            }


            return RedirectToAction("Login", "Login");
        }
    }
}