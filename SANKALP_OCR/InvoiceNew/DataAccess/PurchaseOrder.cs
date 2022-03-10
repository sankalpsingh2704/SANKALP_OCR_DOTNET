using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvoiceNew.Models;
using System.Data.SqlClient;
using System.Data;

namespace InvoiceNew.DataAccess
{
    public class PurchaseOrderRepository
    {
        private static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
        SqlConnection sqlcon = new SqlConnection(connectionString);

        /// <summary>
        /// Insert new Purchase Order
        /// </summary>
        /// <param name="model"></param>
        public void AddPurchaseOrder(PurchaseOrderModel model)
        {
            SqlCommand cmd = new SqlCommand("sp_InsertPurchaseOrder", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            //   string vendor = Request.Form["vendor"];
            string comment = string.Empty;
            cmd.Parameters.AddWithValue("@PODate", model.PODate);
            cmd.Parameters.AddWithValue("@PONumber", model.PONumber);
            cmd.Parameters.AddWithValue("@POAmount", model.POAmount);
            cmd.Parameters.AddWithValue("@Createdby", model.Createdby);
            cmd.Parameters.AddWithValue("@POFileName", model.POFileName);
            sqlcon.Open();
            cmd.ExecuteNonQuery();
            sqlcon.Close();

        }

        /// <summary>
        /// get all the purchase Order
        /// </summary>
        /// <returns></returns>
        public List<PurchaseOrderModel> GetAllPurchaseDetails()
        {
            List<PurchaseOrderModel> objPO = new List<PurchaseOrderModel>();

            sqlcon.Open();
            // string query = "select * from PurchaseOrder";///sp_getPurchaseOrder
            SqlCommand cmd = new SqlCommand("sp_getPurchaseOrder", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    PurchaseOrderModel po = new PurchaseOrderModel();
                    po.POID = Convert.ToInt32(rdr["POID"]);
                    if (rdr["Createdby"] != DBNull.Value)
                    {
                        po.Createdby = Convert.ToInt32(rdr["Createdby"]);
                    }
                    if (rdr["ModifiedBy"] != DBNull.Value)
                    {
                        po.ModifiedBy = Convert.ToInt32(rdr["ModifiedBy"]);
                    }
                    if (rdr["PODate"] != DBNull.Value)
                    {
                        po.PODate = Convert.ToDateTime(rdr["PODate"]).Date;
                    }
                    if (rdr["PONumber"] != DBNull.Value)
                    {
                        po.PONumber = rdr["PONumber"].ToString();
                    }
                    if (rdr["PODate"] != DBNull.Value)
                    {
                        po.POAmount = Convert.ToDecimal(rdr["POAmount"].ToString());
                    }
                    if (rdr["POFileName"] != DBNull.Value)
                    {
                        po.POFileName = rdr["POFileName"].ToString();
                    }
                    objPO.Add(po);
                }
            }
            sqlcon.Close();
            return objPO;
        }

        /// <summary>
        /// delete  Purchase Order By ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeletePurchaseOrder(int id)
        {
            int retVal = 0;
            sqlcon.Open();
            string query = "DELETE from PurchaseOrder WHERE POID = " + id;
            using (SqlCommand comm = new SqlCommand(query, sqlcon))
            {
                retVal = comm.ExecuteNonQuery();
            }
            sqlcon.Close();
            return retVal;
        }

        public int UpdatePurchaseOrder(int id, decimal poAmount)
        {
            int retVal = 0;
            sqlcon.Open();
            string query = "UPDATE PurchaseOrder SET POAmount = " + poAmount + " WHERE POID = " + id;
            using (SqlCommand comm = new SqlCommand(query, sqlcon))
            {
                retVal = comm.ExecuteNonQuery();
            }
            sqlcon.Close();
            return retVal;
        }


        public bool ifPONumberExists(string poNumber)
        {
            bool retVal = false;
            SqlCommand cmd = new SqlCommand("sp_IsPoNumberExists", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PONumber", poNumber);
            sqlcon.Open();
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    retVal = Convert.ToBoolean(rdr[0]);
                }
            }
            sqlcon.Close();
            return retVal;
        }
    }
}