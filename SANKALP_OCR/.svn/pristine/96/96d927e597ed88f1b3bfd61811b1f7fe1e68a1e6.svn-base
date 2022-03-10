using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;

namespace IQinvoiceNew.DataAccess
{
  public class LoginCredentials
  {
        private static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
        SqlConnection sqlcon = new SqlConnection(connectionString);

      public DataRow GetLogin(string UserName, string pwd)
      {
            DataTable dt = new DataTable();
            //  SqlDataAdapter da = new SqlDataAdapter("CheckCredential", con);
            using (SqlCommand cmd = new SqlCommand("CheckCredential", sqlcon))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserName", UserName);
                cmd.Parameters.AddWithValue("@UserPassword", pwd);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt.Rows[0];
        }
  }
}