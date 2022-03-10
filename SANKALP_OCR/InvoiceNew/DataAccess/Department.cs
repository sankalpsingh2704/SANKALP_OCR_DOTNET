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
    public class DepartmentRepository : IDepartment, IDisposable
    {

        private static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
        SqlConnection sqlcon = new SqlConnection(connectionString);

        public IEnumerable<Department> getDepartmentDetails()
        {
            //SqlConnection sqlcon = new SqlConnection(connectionString);
            List<Department> objDept = new List<Department>();
            if(sqlcon.State == ConnectionState.Open)
            {
                sqlcon.Close();
            }
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("sp_getCostCenters", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    Department dept = new Department();
                    dept.DeptID = Convert.ToInt32(rdr["DeptID"]);
                    dept.DeptName = rdr["DeptName"].ToString();
                    dept.DeptDescription = rdr["DeptDescription"].ToString();
                    //dept.IsActive = Convert.ToBoolean(rdr["Disabled"]);
                    dept.Disabled = Convert.ToBoolean(rdr["Disabled"]);

                    objDept.Add(dept);
                }
            }
            sqlcon.Close();
            return objDept;
        }

        public Department getDepartmentbyid(int departmentid)
        {
            SqlConnection sqlcon = new SqlConnection(connectionString);
            Department objDept = new Department();
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("sp_getCostCenters", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DepartmentID", departmentid);

            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    objDept.DeptID = Convert.ToInt32(rdr["DeptID"]);
                    objDept.DeptName = rdr["DeptName"].ToString();
                    objDept.DeptDescription = rdr["DeptDescription"].ToString();
                    objDept.IsActive = Convert.ToBoolean(rdr["Disabled"]);
                    //IsVendor = (u.IsVendor == null) ? (u.IsVendor == false) : (u.IsVendor == true)
                    ///  }).ToList();
                }
            }
            sqlcon.Close();
            return objDept;
            // return context.Departments.Find(departmentid);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }


        public void CreateDepartment(Department dept)
        {
            //context.Departments.Add(dept);
            //context.SaveChanges();
        }


        public void UpdateDepartment(Department dept)
        {
            //context.Entry(dept).State = System.Data.Entity.EntityState.Modified;
            //context.SaveChanges();
        }

        public void DeleteDepartment(int id)
        {
            sqlcon.Open();
            
            SqlCommand cmd = new SqlCommand("sp_deleteDepartment", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@DepartmentID", id);
            cmd.ExecuteNonQuery();
            
            sqlcon.Close();
        }
    }
}