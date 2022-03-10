using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvoiceNew.Models;
using System.Data.SqlClient;
using System.Data;

namespace InvoiceNew.DataAccess
{
    public class VendorRepository : IDisposable
    {

        private static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
        SqlConnection sqlcon = new SqlConnection(connectionString);

        public IEnumerable<Vendor> getVendorDetails()
        {
            SqlConnection sqlcon = new SqlConnection(connectionString);
            List<Vendor> objVendor = new List<Vendor>();
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("sp_getvendors", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    Vendor vendor = new Vendor();
                    vendor.VendorID = Convert.ToInt32(rdr["VendorID"]);
                    vendor.VendorName = rdr["VendorName"].ToString();
                    vendor.VendorDescription = rdr["VendorDescription"].ToString();
                    vendor.Disabled = Convert.ToBoolean(rdr["Disabled"]);
                    vendor.VendorEmail = rdr["VendorEmail"].ToString();
                    vendor.VendorAddress= rdr["Address"].ToString();
                    vendor.VendorGST = rdr["GSTIN_UIN"].ToString();
                    vendor.VendorPhone = rdr["Phone"].ToString();
                    //IsVendor = (u.IsVendor == null) ? (u.IsVendor == false) : (u.IsVendor == true)
                    ///  }).ToList();
                    objVendor.Add(vendor);
                }
            }
            sqlcon.Close();
            return objVendor;
        }

        public Vendor getVendorbyid(int vendorid)
        {

            SqlConnection sqlcon = new SqlConnection(connectionString);
            Vendor vendor = new Vendor();
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("sp_getvendors", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    vendor.VendorID = Convert.ToInt32(rdr["VendorID"]);
                    vendor.VendorName = rdr["VendorName"].ToString();
                    vendor.VendorDescription = rdr["VendorDescription"].ToString();
                    vendor.Disabled = Convert.ToBoolean(rdr["Disabled"]);
                }
            }
            sqlcon.Close();
            return vendor;
        }

        public void AddUpdateVendors(Vendor vendor)
        {
            SqlConnection sqlcon = new SqlConnection(connectionString);
         
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("AddUpdateVendors", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@VendorID", vendor.VendorID);
            cmd.Parameters.AddWithValue("@VendorName", vendor.VendorName);
            cmd.Parameters.AddWithValue("@VendorDescription", vendor.VendorAddress);
            cmd.Parameters.AddWithValue("@VendorEmail", vendor.VendorEmail);
            cmd.Parameters.AddWithValue("@VendorGst", vendor.VendorGST);
            cmd.Parameters.AddWithValue("@VendorPhone", vendor.VendorPhone);          
                
            cmd.Parameters.AddWithValue("@Disabled", vendor.Disabled);

            cmd.Parameters.AddWithValue("@CreatedBy", vendor.CreatedBy);
            cmd.Parameters.AddWithValue("@ModifiedBy", vendor.ModifiedBy);
            cmd.ExecuteNonQuery();

        }

        public void UpdateVendor(Vendor vendor)
        {
        }

        public void DeleteVendor(int vendorid)
        {

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}