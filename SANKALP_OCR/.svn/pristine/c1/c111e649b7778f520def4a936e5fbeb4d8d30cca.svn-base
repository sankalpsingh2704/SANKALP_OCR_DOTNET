using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvoiceNew.Models;
using InvoiceNew.DataAccess;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;

namespace InvoiceNew.DataAccess
{
    public class CostCenterRepository : ICostCenter, IDisposable
    {


        private static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
        SqlConnection sqlcon = new SqlConnection(connectionString);

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CostCenter> getCostCenterDetails()
        {
            List<CostCenter> objCostCenters = new List<CostCenter>();

            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("sp_getCostCenters", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    CostCenter cCenter = new CostCenter();
                    cCenter.CostCenterID = Convert.ToInt32(rdr["CostCenterID"]);
                    cCenter.CostCenterName = rdr["CostCenterName"].ToString();
                    cCenter.CostCenterDescription = rdr["CostCenterDescription"].ToString();
                    //dept.IsActive = Convert.ToBoolean(rdr["Disabled"]);
                    cCenter.Disabled = Convert.ToBoolean(rdr["Disabled"]);

                    objCostCenters.Add(cCenter);
                }
            }
            sqlcon.Close();
            return objCostCenters;
        }

        public CostCenter getCostCenterbyid(int costcenterid)
        {
            CostCenter objCostCenter = new CostCenter();
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("sp_getCostCenters", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CostCenterID", costcenterid);

            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {

                    objCostCenter.CostCenterID = Convert.ToInt32(rdr["CostCenterID"]);
                    objCostCenter.CostCenterName = rdr["CostCenterName"].ToString();
                    objCostCenter.CostCenterDescription = rdr["CostCenterDescription"].ToString();
                    objCostCenter.Disabled = Convert.ToBoolean(rdr["Disabled"]);
                    //IsVendor = (u.IsVendor == null) ? (u.IsVendor == false) : (u.IsVendor == true)
                    ///  }).ToList();
                }
            }

            sqlcon.Close();
            return objCostCenter;
        }

        public void CreateCostCenter(CostCenter costcenter)
        {
            //context.CostCenters.Add(costcenter);
            //context.SaveChanges();
        }

        public void UpdateCostCenter(CostCenter costcenter)
        {
            //context.Entry(costcenter).State = System.Data.Entity.EntityState.Modified;
            //context.SaveChanges();
        }

        public void DeleteCostCenter(int costcenterid)
        {
            CostCenter objCostCenter = new CostCenter();
            sqlcon.Open();
            string query = "DELETE from CostCenters WHERE CostCenterID = " + costcenterid;
            using (SqlCommand comm = new SqlCommand(query, sqlcon))
            {
                comm.ExecuteNonQuery();
            }
            sqlcon.Close();
        }
    }
}