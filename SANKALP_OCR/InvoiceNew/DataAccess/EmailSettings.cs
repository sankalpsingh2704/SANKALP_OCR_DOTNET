using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceNew.DataAccess
{
    public class EmailSettings
    {
        private bool disposed = false;



        public void inseremailSettings(SystemConfiguration email)
        {
           
        }

        public void updateemailsettings(SystemConfiguration email)
        {
     
        }

        public IEnumerable<SystemConfiguration> SelectAllEmails()
        {
            return null;
        }

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!this.disposed)
        //    {
        //        if (disposing)
        //        {
        //            context.Dispose();
        //        }
        //    }
        //    this.disposed = true;
        //}
        
    }
}