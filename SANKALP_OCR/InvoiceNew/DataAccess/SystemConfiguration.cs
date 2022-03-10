using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using InvoiceNew.Models;

namespace InvoiceNew.DataAccess
{
    /// <summary>
    /// To get the SystemConfiguration details
    /// </summary>
    public class SystemConfiguration
    {
        private static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
        SqlConnection sqlcon = new SqlConnection(connectionString);
        public SystemConfigurationModel SystemConfig()
        {
            SystemConfigurationModel cData = new Models.SystemConfigurationModel();
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("sp_getSystemConfiguration", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                       
                        cData.SmtpServer = sdr["SmtpServer"].ToString();
                        cData.OutgoingPortNo = Convert.ToInt32(sdr["OutgoingPortNo"]);
                        cData.CompanyEmailFrom = sdr["CompanyEmailFrom"].ToString();
                        cData.CompanyEmailFromPassword = sdr["CompanyEmailFromPassword"].ToString();
                        cData.SSL = Convert.ToBoolean(sdr["SSL"]);
                    }
                }
             sqlcon.Close();
            return cData;
        }
    }
}