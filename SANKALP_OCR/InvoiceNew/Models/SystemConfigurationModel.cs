using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceNew.Models
{
    public class SystemConfigurationModel
    {
        public int SystemID { get; set; }
        public string SmtpServer { get; set; }
        public Nullable<int> OutgoingPortNo { get; set; }
        public string CompanyEmailFrom { get; set; }
        public string CompanyEmailFromPassword { get; set; }
        public Nullable<bool> SSL { get; set; }
        public Nullable<int> EmailFrequencyHoursBeforeDueDate { get; set; }
        public Nullable<int> EmailSendDaysBeforeDueDate { get; set; }
        public Nullable<int> EmailFrequencyAfterDueDate { get; set; }
        public Nullable<int> EmailSendDayLimitAfterDueDate { get; set; }
    }
}