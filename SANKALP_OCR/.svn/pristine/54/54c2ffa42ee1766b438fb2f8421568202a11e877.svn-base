using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IQinvoice.DataAccess;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using InvoiceNew.Models;
using System.Data.Entity.Core.Objects;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using InvoiceNew.DataAccess;

namespace InvoiceNew.Controllers
{
    public class UsersController : IQinvoiceController
    {

        public ActionResult UsersView()
        {

            ViewBag.msg = "";
            if (TempData["Error"] != null)
            {
                ViewBag.msg = TempData["Error"];
            }

            try
            {
                UserModel model = new UserModel();
                model.UserID = 0;

                //var listdept = context.Departments.Where(d => d.Disabled == false).Select(d => new { d.DeptID, d.DeptName });
                //var usertypelist = context.UserTypes.Where(u => u.Disabled == false).Select(u => new { u.UserTypeID, u.UserType1 });
                //ViewBag.Departments = listdept;
                //ViewBag.usertypes = usertypelist;
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
            }
            return View();
        }

        /// <summary>
        /// Description: Method to fetch all Users details to JQGrid
        /// </summary>
        /// <returns></returns>
        public JsonResult GetUserDetails()
        {

                 UserRepository User = new UserRepository();

                IEnumerable<UserModel> userlist =  User.getUserDetails();

                int rowCounter = 0;
                var jgridData = new
                {

                    total = 1,
                    page = 0,
                    records = 1,

                    rows = (from m in userlist
                            select new
                            {
                                UserID = m.UserID,
                                slno = ++rowCounter,
                                UserName = m.UserLoginName,
                                UserTypeID = m.Usertype,
                                DepartmentID = "1",////m.DepartmentName,
                                UserImage = m.UserPic,
                                FirstName = m.UserFirstName,
                                MiddleName = m.UserMiddleName,
                                LastName = m.UserLastName,
                                EmailAddress = m.UserEmailID,
                                Disabled = (m.IsActive) ? "Yes" : "No"                                
                            })

                };
            
           return Json(jgridData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Author:Bharath 
        /// Description: Inserts a new user to the 'Users' table
        /// </summary>
        /// <param name="model"></param>
        /// <returns> View("Users") </returns>
        //[HttpPost]
        public ActionResult CreateUser(UserModel model)
        {
            UserRepository User = new UserRepository();
            UserModel user = new UserModel();
            var filename = String.Empty;
            var path = String.Empty;

            if (ModelState.IsValid)
            {
                if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
                {
                    filename = System.IO.Path.GetFileName(Request.Files[0].FileName);

                    var supportedTypes = new[] { "JPG", "JPEG", "PNG", "TIF", "GIF"};////new[] { "jpg", "Jpg", "JPG", "jpeg", "Jpeg", "JPEG", "png", "Png", "PNG", "tif", "Tif", "TIF", "gif", "Gif", "GIF" };
                    var fileExtension = System.IO.Path.GetExtension(filename).Substring(1).ToUpper();
                    if (!supportedTypes.Contains(fileExtension))
                    {
                        TempData["Error"] = "File is Not Supported";
                        return RedirectToAction("Users");
                    }

                    path = System.IO.Path.Combine(Server.MapPath("~/Content/images/UserPic/"), filename);
                    Request.Files[0].SaveAs(path);
                }

                model.UserPic = filename;


              //  user.UserPassword = Encoding.ASCII.GetBytes(model.UserPassword);

                if (model.UserPic == "")
                {
                    model.UserPic = "male.png";
                }



                //  user.CreatedBy = Convert.ToInt32(Session["UserID"]);
                model.CreatedBy = int.Parse(Session["UserID"].ToString()); ;
                model.CreatedDate = DateTime.Now;

                User.CreateUser(model);
            }
            else
            {
                return RedirectToAction("UsersView");
            }

            return RedirectToAction("UsersView");
        }


        /// <summary>
        /// Update the Users
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Update(UserModel model)
        {
            UserRepository User = new UserRepository();
            UserModel user = User.getUserbyid(model.UserID);////getOnlyUserDetails(model.UserID);
            var filename = String.Empty;
            var path = String.Empty;

            try
            {
                //if (ModelState.IsValid)
                //{
                if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
                {
                    filename = System.IO.Path.GetFileName(Request.Files[0].FileName);

                    var supportedTypes = new[] { "jpg", "Jpg", "JPG", "jpeg", "Jpeg", "JPEG", "png", "Png", "PNG", "tif", "Tif", "TIF", "gif", "Gif", "GIF" };
                    var fileExtension = System.IO.Path.GetExtension(filename).Substring(1);
                    if (!supportedTypes.Contains(fileExtension))
                    {
                        TempData["Error"] = "File is Not Supported";
                        return RedirectToAction("Users");
                    }

                    path = System.IO.Path.Combine(Server.MapPath("~/Files/UserImages"), filename);
                    Request.Files[0].SaveAs(path);
                }

                model.UserPic = filename;

                User.UpdateUser(model);
                //user.UserFirstName = model.UserFirstName;
                //user.UserMiddleName = model.UserMiddleName;
                //user.UserLastName = model.UserLastName;

                //user.UserLoginName = model.UserLoginName;
                //user.UserEmailID = model.UserEmailID;

                //user.DepartmentID = model.UserDepartmentID;
                //user.UserTypeID = model.UserTypeID;


                //user.UserImage = model.UserPic;

                //if (user.UserImage == "")
                //{
                //    user.UserImage = "male.png";
                //}

                //user.Disabled = model.IsActive;
                //user.IsFinance = model.IsFinance;
                //user.IsVendor = model.IsVendor;
                //user.ModifiedBy = int.Parse(Session["UserID"].ToString());
                //user.ModifiedDate = DateTime.Now;

                //User.UpdateUser(user);
                //if (model.NewPassword != null)
                //{

                //    ObjectParameter Passwordstatus = new ObjectParameter("Passwordstatus", typeof(Int32));
                //    context.Sp_ComparePassword(model.UserID, model.NewPassword, Passwordstatus);

                //}
            }
            //  }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
            }
            return RedirectToAction("UsersView");
        }


        ///// <summary>
        ///// Author: Bharath
        ///// Description: Gets the details of the selected user by the UserID from the database
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns> Details of the selected user </returns>
        public JsonResult getDetails(int id)
        {

            //UserRepository User = new UserRepository(new IMSEntities());
            UserRepository User = new UserRepository();

            UserModel userlist = User.getUserbyid(id);

            //var listDetails = context.Users.Where(u => u.UserID == id)
            //    .Select(d => new
            //    {
            //      d.UserID,

            //      d.FirstName,
            //      d.MiddleName,
            //      d.LastName,

            //      d.UserName,
            //      d.EmailAddress,
            //      d.UserTypeID,
            //      d.DepartmentID,
            //      d.UserImage,
            //      d.Disabled
            //    }).FirstOrDefault();

            return Json(userlist, JsonRequestBehavior.AllowGet);
        }





        /// <summary>
        /// Author: Bharath
        /// Description: Checks for an existing Username in the database against the name provided in the 'Username' textbox
        /// </summary>
        /// <param name="UserLoginName"></param>
        /// <param name="UserID"></param>
        /// <returns> An error message if a match is found </returns>

        //public ActionResult CheckLogin(string UserLoginName, int UserID = 0)
        //{

        //    var checkLogin = context.Users.Where(m => m.UserName.ToLower() == UserLoginName.ToLower()).ToList();
        //    var isUserExist = context.Users.Where(m => m.UserID == UserID && m.UserName.ToLower() == UserLoginName.ToLower()).ToList();

        //    if (isUserExist.Count == 0)
        //    {
        //        if (checkLogin.Count() > 0)
        //        {
        //            return Json("Username already exists", JsonRequestBehavior.AllowGet);
        //        }
        //    }

        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}


        /// <summary>
        /// Description: Checks for an existing Email in the database against the name provided in the 'Email' textbox
        /// </summary>
        /// <param name="UserEmailID"></param>
        /// <param name="UserID"></param>
        /// <returns> Prevents form submit if a match is found </returns>
        public ActionResult CheckEmail(string UserEmailID, int UserID = 0)
        {

            //var checkEmail = context.Users.Where(m => m.EmailAddress.ToLower() == UserEmailID.ToLower()).ToList();
            //var isUserExist = context.Users.Where(m => m.UserID == UserID && m.EmailAddress.ToLower() == UserEmailID.ToLower()).ToList();

            //if (isUserExist.Count == 0)
            //{
            //    if (checkEmail.Count() > 0)
            //    {
            //        return Json(false, JsonRequestBehavior.AllowGet);
            //    }
            //}

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Receives the uploaded image name from the 'Users'(cshtml) page and checks for the valid image type
        /// for front-end validation
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public string checkImage(string filename)
        {
            if (filename != String.Empty)
            {
                int languageID = Convert.ToInt32(Session["LanguageID"]);
                var supportedTypes = new[] { "jpg", "Jpg", "JPG", "jpeg", "Jpeg", "JPEG", "png", "Png", "PNG", "tif", "Tif", "TIF", "gif", "Gif", "GIF" };
                var fileExtension = System.IO.Path.GetExtension(filename).Substring(1);
                string msg = String.Empty;

                if (!supportedTypes.Contains(fileExtension))
                {

                    ViewBag.Message = "File format not supported";
                    return msg;
                }
            }

            return String.Empty;
        }
    }
}