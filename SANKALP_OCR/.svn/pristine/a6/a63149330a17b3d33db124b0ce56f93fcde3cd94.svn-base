using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvoiceNew.Models
{
    public class UserMappingModel
    {
        public int RowNo { get; set; }
        public int UserID { get; set; }
        public int UserTypeID { get; set; }
        public string UserName { get; set; }
        public string UserTypeName { get; set; }
        public bool MultiBranch { get; set; }

        public List<BranchFillModel> BranchDetails { set; get; }
        public List<string> SelectedItemIds { get; set; }
        public MultiSelectList Items { get; set; }


        public List<string> TeamIds { get; set; }
        public MultiSelectList Teams { get; set; }

        public List<UserTypes> UserTypes { get; set; }
    }
    public class BranchFillModel
    {
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public string UserName { get; set; }
        public int UserID { get; set; }
    }

 
}