using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;

namespace InvoiceNew.Models
{
    public class UserModel
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "Department Name Is Required")]
        public int UserDepartmentID { get; set; }

        //[Required(ErrorMessage = "Usertype Name Is Required")]
        public int UserTypeID { get; set; }


        [Required(ErrorMessage = "User Name Is Required")]
        //[Remote("CheckLogin", "Users", AdditionalFields = "UserID", ErrorMessage = "User with Login Name Already Exists")]
        public string UserLoginName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is Required")]
        //  [Remote("CheckEmail", "Users", AdditionalFields = "UserID", ErrorMessage = "User with Email Already Exists")]
        public string UserEmailID { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "User Password")]
        [StringLength(20, ErrorMessage = "Minimum length has to be more than 6", MinimumLength = 6)]
        public string UserPassword { get; set; }

        // If 'System.Web.Mvc' is not used, system shows default error message
        [DataType(DataType.Password)]
        [Display(Name = "Compare Password")]
        [System.Web.Mvc.Compare("UserPassword", ErrorMessage = "Password and Re-enter Password does not match!")]
        public string ComparePassword { get; set; }

        [Required(ErrorMessage = "First Name Is Required")] // (ErrorMessage = "Please enter your first name!")
        [StringLength(100, ErrorMessage = "String Length Exceeded")]
        public string UserFirstName { get; set; }

        [StringLength(100, ErrorMessage = " ")]
        public string UserMiddleName { get; set; }

        [Required(ErrorMessage = "Last Name Is Required")]
        [StringLength(100, ErrorMessage = "String Length Exceeded")]
        public string UserLastName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "User Password")]
        [StringLength(20, ErrorMessage = "Minimum length has to be more than 6", MinimumLength = 6)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Compare Password")]
        [System.Web.Mvc.Compare("NewPassword", ErrorMessage = "Password and Re-enter Password does not match!")]
        public string ConfirmPassword { get; set; }

        public bool HasRemove { get; set; }
        public string UserPic { get; set; }
        public bool IsActive { get; set; }
        public bool IsFinance { get; set; }
        public string DepartmentName { get; set; }
        public string Usertype { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        // Added as per new change request
        public bool IsVendor { get; set; }
    }
}