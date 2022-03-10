using InvoiceNew.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace InvoiceNew.DataAccess
{
    public class InvoiceReport
    {
        private static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
        SqlConnection sqlcon = new SqlConnection(connectionString);


        public DataTable GetInvoiceReports(string startdate,string enddate,int branchid)
        {
            DataTable dt = new DataTable();
            try
            {
                string sdate = DateTime.ParseExact(startdate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                string edate = DateTime.ParseExact(enddate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

                List<InvoiceModel> ObjInvoiceModel = new List<InvoiceModel>();
                sqlcon.Open();
                SqlCommand cmd = new SqlCommand("GetInvoiceReport", sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Startdate", sdate);
                cmd.Parameters.AddWithValue("@enddate", edate);
                cmd.Parameters.AddWithValue("@BranchID", branchid);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(dt);

                sqlcon.Close();
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
            }
            finally
            {
                if (sqlcon.State == ConnectionState.Open)
                    sqlcon.Close();
            }
            return dt;
        }

        public DataTable GetGSTReturnsReport(string startdate, string enddate,int branchid)
        {
            DataTable dt = new DataTable();
            try
            {
                string sdate = DateTime.ParseExact(startdate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                string edate = DateTime.ParseExact(enddate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

                List<InvoiceModel> ObjInvoiceModel = new List<InvoiceModel>();
                sqlcon.Open();
                SqlCommand cmd = new SqlCommand("usp_GetInvoiceGSTReturns", sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Startdate", sdate);
                cmd.Parameters.AddWithValue("@enddate", edate);
                cmd.Parameters.AddWithValue("@BranchID", branchid);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(dt);

                sqlcon.Close();
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
            }
            finally
            {
                if (sqlcon.State == ConnectionState.Open)
                    sqlcon.Close();
            }
            return dt;
        }

        public DataTable PurchaseReport(string startdate, string enddate,int branchid)
        {
            DataTable dt = new DataTable();
            try
            {
                string sdate = DateTime.ParseExact(startdate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                string edate = DateTime.ParseExact(enddate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

                List<InvoiceModel> ObjInvoiceModel = new List<InvoiceModel>();
                sqlcon.Open();
                SqlCommand cmd = new SqlCommand("usp_PurchaseReport", sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Startdate", sdate);
                cmd.Parameters.AddWithValue("@enddate", edate);
				cmd.Parameters.AddWithValue("@BranchID", branchid);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(dt);
                dt.Columns.Remove("VoucherType");

                sqlcon.Close();
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
            }
            finally
            {
                if (sqlcon.State == ConnectionState.Open)
                    sqlcon.Close();
            }
            return dt;
        }

        public DataTable JournalReport(string startdate, string enddate,int BranchID)
        {
            DataTable dt = new DataTable();
            try
            {
                string sdate = DateTime.ParseExact(startdate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                string edate = DateTime.ParseExact(enddate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

                List<InvoiceModel> ObjInvoiceModel = new List<InvoiceModel>();
                sqlcon.Open();
                SqlCommand cmd = new SqlCommand("usp_JournalReport", sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Startdate", sdate);
                cmd.Parameters.AddWithValue("@enddate", edate);
				 cmd.Parameters.AddWithValue("@BranchID", BranchID);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(dt);

                sqlcon.Close();
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
            }
            finally
            {
                if (sqlcon.State == ConnectionState.Open)
                    sqlcon.Close();
            }
            return dt;
        }

        public DataTable GetCategoryInvoiceItemsReport(string startdate, string enddate,int branchid)
        {
            DataTable dt = new DataTable();
            try
            {
                string sdate = DateTime.ParseExact(startdate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                string edate = DateTime.ParseExact(enddate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

                List<InvoiceModel> ObjInvoiceModel = new List<InvoiceModel>();
                sqlcon.Open();
                SqlCommand cmd = new SqlCommand("usp_GetItemCategoryInvoiceSummary", sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Startdate", sdate);
                cmd.Parameters.AddWithValue("@enddate", edate);
                cmd.Parameters.AddWithValue("@BranchID", branchid);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(dt);

                sqlcon.Close();
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
            }
            finally
            {
                if (sqlcon.State == ConnectionState.Open)
                    sqlcon.Close();
            }
            return dt;
        }
    }
}