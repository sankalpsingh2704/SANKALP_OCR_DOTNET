using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvoiceNew.Models;

namespace InvoiceNew.Models
{
    public class Department
    {
        //public Department()
        //{
        //    this.InVoices = new HashSet<InVoice>();
        //    this.Users = new HashSet<User>();
        //}

        public int DeptID { get; set; }
        public string DeptName { get; set; }
        public string DeptDescription { get; set; }
        public Nullable<bool> Disabled { get; set; }
        public Nullable<bool> IsActive { get; set; }
        
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        //public virtual ICollection<InVoice> InVoices { get; set; }
        //public virtual ICollection<User> Users { get; set; }
    }
}