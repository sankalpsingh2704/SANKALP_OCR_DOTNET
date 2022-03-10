using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvoiceNew.Models;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;


namespace InvoiceNew.DataAccess
{

    public class UserRepository : IUser, IDisposable
    {
        private static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBconnection"].ToString();
        SqlConnection sqlcon = new SqlConnection(connectionString);

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the details of Users
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserModel> getUserDetails()
        {
            SqlConnection sqlcon = new SqlConnection(connectionString);
            List<UserModel> objUsr = new List<UserModel>();

            // string query = "select USR.*,UT.UserType FROM Users USR INNER JOIN UserTypes UT ON UT.UserTypeID=USR.UserTypeID ";
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("sp_getAllUsers", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    UserModel usrModel = new Models.UserModel();
                    usrModel.UserID = Convert.ToInt32(rdr["UserID"]);
                    usrModel.UserPic = rdr["UserImage"].ToString();
                    usrModel.UserFirstName = rdr["FirstName"].ToString();
                    usrModel.UserMiddleName = rdr["MiddleName"].ToString();
                    usrModel.UserLastName = rdr["LastName"].ToString();
                    usrModel.UserLoginName = rdr["UserName"].ToString();
                    ///DepartmentName = d.DeptName,
                    usrModel.Usertype = rdr["UserType"].ToString();
                    usrModel.UserEmailID = rdr["EmailAddress"].ToString();
                    usrModel.IsActive = Convert.ToBoolean(rdr["Disabled"]);
                    //IsVendor = (u.IsVendor == null) ? (u.IsVendor == false) : (u.IsVendor == true)
                    ///  }).ToList();
                    objUsr.Add(usrModel);
                }
            }
            sqlcon.Close();
            return objUsr;
        }
        public DataTable getBranchList(List<int> blist)
        {
            DataTable table = new DataTable();
            table.Columns.Add("BranchID",typeof(int));
            foreach (var item in blist)
            {
                table.Rows.Add(item);
            }
            return table;
        }
        /// <summary>
        /// Get users from Usertypess Mapping data
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserModel> getUsersForDropdown(int branchid)
        {
            SqlConnection sqlcon = new SqlConnection(connectionString);
            List<UserModel> objUsr = new List<UserModel>();
            sqlcon.Open();
            // string query = "select UTM.ID,USR.UserID,USR.UserName,UTM.UserTypeID from UserTypesMapping UTM INNER JOIN Users USR ON USR.UserID=UTM.UserID WHERE usr.IsAdmin <> 1";
            /// "select usr.UserID,usr.UserName,usrt.UsertypeID from dbo.[Users] usr inner join[UserTypes] usrt on usr.UserTypeID = usrt.UserTypeID AND usrt.Disabled = 0 AND (usr.IsAdmin <> 1 AND usr.isPaymentApproval <> 1)";
            SqlCommand cmd = new SqlCommand("sp_getUsersForDropdown", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BranchID",branchid);
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    //select new UserModel
                    //{

                    UserModel usrModel = new Models.UserModel();
                    usrModel.UserID = Convert.ToInt32(rdr["UserID"]);
                    usrModel.UserLoginName = rdr["UserName"].ToString();
                    //  usrModel.Usertype = rdr["UserType"].ToString();
                    usrModel.UserTypeID = Convert.ToInt32(rdr["UserTypeID"]);
                    //    usrModel.IsActive = Convert.ToBoolean(rdr["Disabled"]);
                    objUsr.Add(usrModel);
                }
            }
            sqlcon.Close();
            return objUsr;
        }

        /// <summary>
        /// get user By userID
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public UserModel getUserbyid(int userid)
        {
            UserModel usrModel = new UserModel();
            sqlcon.Open();
            // string query = "select USR.*,UT.UserType FROM Users USR INNER JOIN UserTypes UT ON UT.UserTypeID = USR.UserTypeID WHERE USR.UserID = " + userid;

            SqlCommand cmd = new SqlCommand("sp_getUserById", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", userid);
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    //select new UserModel
                    //{
                    usrModel.UserID = Convert.ToInt32(rdr["UserID"]);
                    usrModel.UserPic = rdr["UserImage"].ToString();
                    usrModel.UserFirstName = rdr["FirstName"].ToString();
                    usrModel.UserMiddleName = rdr["MiddleName"].ToString();
                    usrModel.UserLastName = rdr["LastName"].ToString();
                    usrModel.UserLoginName = rdr["UserName"].ToString();
                    ///DepartmentName = d.DeptName,
                    usrModel.Usertype = rdr["UserType"].ToString();
                    usrModel.UserEmailID = rdr["EmailAddress"].ToString();
                   
                    usrModel.IsActive = Convert.ToBoolean(rdr["Disabled"]);

                }
            }
            sqlcon.Close();
            return usrModel;
        }


        ////public User getOnlyUserDetails(int userid)
        ////{
        ////    return context.Users.Find(userid);
        ////}

        ////public string getOnlyUserName(int userid)
        ////{
        ////  //  return context.Users.Find(userid).UserName;
        ////}
        public void CreateUser(UserModel user)
        {
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("sp_InsertUserDetail", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserName", user.UserLoginName);
            ///cmd.Parameters.AddWithValue("@UserPassword", (System.Text.Encoding.ASCII.GetBytes(user.UserPassword)));
            cmd.Parameters.AddWithValue("@UserPassword", user.UserPassword);

            cmd.Parameters.AddWithValue("@UserTypeID", user.UserTypeID);
            cmd.Parameters.AddWithValue("@DepartmentID", user.UserDepartmentID);
            cmd.Parameters.AddWithValue("@FirstName", user.UserFirstName);
            //cmd.Parameters.AddWithValue("@Amount", Convert.ToDouble(p.Amount));
            cmd.Parameters.AddWithValue("@MiddleName", user.UserMiddleName);
            cmd.Parameters.AddWithValue("@LastName", user.UserLastName);
            cmd.Parameters.AddWithValue("@EmailAddress", user.UserEmailID);
            cmd.Parameters.AddWithValue("@Disabled", user.IsActive);
            cmd.Parameters.AddWithValue("@UserImage", user.UserPic);
            cmd.Parameters.AddWithValue("@IsFinance", user.IsFinance);
            cmd.Parameters.AddWithValue("@IsVendor", user.IsVendor);
            cmd.Parameters.AddWithValue("@UserId", user.CreatedBy);
           
            cmd.ExecuteNonQuery();
            sqlcon.Close();
        }
        public void UpdateUser(UserModel user)
        {
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("sp_UpdateUserById", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserName", user.UserLoginName);
            ///cmd.Parameters.AddWithValue("@UserPassword", (System.Text.Encoding.ASCII.GetBytes(user.UserPassword)));
            cmd.Parameters.AddWithValue("@UserPassword", user.NewPassword);

            cmd.Parameters.AddWithValue("@UserTypeID", user.UserTypeID);
            cmd.Parameters.AddWithValue("@DepartmentID", user.UserDepartmentID);
            cmd.Parameters.AddWithValue("@FirstName", user.UserFirstName);
            //cmd.Parameters.AddWithValue("@Amount", Convert.ToDouble(p.Amount));
            cmd.Parameters.AddWithValue("@MiddleName", user.UserMiddleName);
            cmd.Parameters.AddWithValue("@LastName", user.UserLastName);
            cmd.Parameters.AddWithValue("@EmailAddress", user.UserEmailID);
            cmd.Parameters.AddWithValue("@Disabled", user.IsActive);
            cmd.Parameters.AddWithValue("@UserImage", user.UserPic);
            cmd.Parameters.AddWithValue("@IsFinance", user.IsFinance);
            cmd.Parameters.AddWithValue("@IsVendor", user.IsVendor);
            cmd.Parameters.AddWithValue("@UserId", user.UserID);
            

            cmd.ExecuteNonQuery();
            sqlcon.Close();
        }
        ////public void UpdateUser(User user)
        ////{

        ////    context.Entry(user).State = System.Data.Entity.EntityState.Modified;
        ////    context.SaveChanges();
        ////}

        //public void DeleteUser(int userid)
        //{
        //    User userdelete = context.Users.Find(userid);
        //    if (userdelete != null)
        //    {
        //        context.Users.Remove(userdelete);
        //        context.SaveChanges();
        //    }
        //}

        //public void Dispose()
        //{
        //    throw new NotImplementedException();
        //}


        //public IEnumerable<User> getUserdetailsbyusergroupid(int usertypeid)
        //{
        //    return context.Users.Where(u => u.Disabled == false && u.UserTypeID == usertypeid).Select(u => u).ToList();
        //}

        //public List<int> getUserDepartmentid(int usertypeid)
        //{
        //    return context.Users.Where(u => u.Disabled == false && u.UserID == usertypeid).Select(u => u.DepartmentID).ToList();
        //}

        //public List<sp_getOutstandingAmountByMonth_Result> getoutamount(int usertypeid)
        //{

        //    return context.sp_getOutstandingAmountByMonth(usertypeid).ToList();           


        //}

        //public List<double?> getpaidamount(int usertypeid)
        //{
        //    if (usertypeid == 1)
        //    {
        //        return context.InVoices.Where(u => u.Dateofpayment != null).Select(u => u.InvoiceAmount).ToList();
        //    }
        //    else
        //    {
        //        return context.InVoices.Where(u => u.Dateofpayment != null && u.DepartmentID == usertypeid).Select(u => u.InvoiceAmount).ToList();
        //    }

        //}
        //public List<sp_getPaidAmountByMonth_Result> getpaidAmountbar(int UserTypeID)
        //{
        //    return context.sp_getPaidAmountByMonth(UserTypeID).ToList();

        //}

        //public List<sp_getOutstandingAmountByMonth_Result> getoutamountPerMonth(int deptid)
        //{
        //    return context.sp_getOutstandingAmountByMonth(deptid).ToList();

        //}
        //public List<sp_getInvoicePercent_Result> getinvoicepercent(int deptid)
        //{
        //    return context.sp_getInvoicePercent(deptid).ToList();
        //}
        //public List<sp_getPaidInvoicePercent_Result> getpaidinvoicepercent(int deptid)
        //{
        //    return context.sp_getPaidInvoicePercent(deptid).ToList();
        //}

        //public List<sp_getDueInvoicePercent_Result> getDueinvoicepercent()
        //{
        //    return context.sp_getDueInvoicePercent().ToList();
        //}


        //public List<sp_getDuePercent_Result> getDuepercent(int deptid)
        //{
        //    return context.sp_getDuePercent(deptid).ToList();
        //}

        //public List<sp_getTotalPaidCount_Result> getTotalPaidCount(int UserTypeID)
        //{
        //    return context.sp_getTotalPaidCount(UserTypeID).ToList();
        //}


        //public string getusergroupname(int currentuserid)
        //{
        //    return (from u in context.Users join ug in context.UserTypes on u.UserTypeID equals ug.UserTypeID where u.UserID == currentuserid select ug.UserType1).FirstOrDefault().ToString();
        //}

        /// <summary>
        /// Get the Mapping details for Users and UserType
        /// </summary>
        /// <returns></returns>
        public List<UserMappingModel> getUserMappingDetails()
        {
            List<UserMappingModel> objusr = new List<UserMappingModel>();
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("sp_GetUserTypesMappingDetails", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;

            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {
                    UserMappingModel usr = new Models.UserMappingModel();
                    usr.RowNo = Convert.ToInt32(sdr["RowNo"]);
                    usr.UserTypeID = Convert.ToInt32(sdr["UserTypeID"]);
                    if (sdr["UserID"] != DBNull.Value)
                        usr.UserID = Convert.ToInt32(sdr["UserID"]);
                    if (sdr["UserName"] != DBNull.Value)
                        usr.UserName = sdr["UserName"].ToString();
                    if (sdr["UserTypeName"] != DBNull.Value)
                        usr.UserTypeName = sdr["UserTypeName"].ToString();
                    if (sdr["MultiBranch"] != DBNull.Value)
                        usr.MultiBranch = bool.Parse(sdr["MultiBranch"].ToString());
                    objusr.Add(usr);
                }
            }
            sqlcon.Close();
            return objusr;
        }

        /// <summary>
        /// get the details for UserTypes
        /// </summary>
        /// <returns></returns>
        public List<UserTypes> getUserTypes()
        {
            List<UserTypes> usrType = new List<UserTypes>();
            sqlcon.Open();
            string query = "select UserTypeID,UserType from UserTypes WHERE Disabled=0";
            using (SqlCommand comm = new SqlCommand(query, sqlcon))
            {
                using (SqlDataReader rdr = comm.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        UserTypes usr = new Models.UserTypes();
                        usr.UserTypeID = Convert.ToInt32(rdr["UserTypeID"]);
                        usr.UserTypeName = rdr["UserType"].ToString();
                        usrType.Add(usr);
                    }
                }
            }
            sqlcon.Close();
            return usrType;
        }

        /// <summary>
        /// Get users Based on UserTypes  which is distinct for each UserTYpes
        /// </summary>
        /// <param name="id">UseeTypeID</param>
        /// <returns></returns>
        public List<UserTypes> getFilteredUser(int id)
        {
            List<UserTypes> usrType = new List<UserTypes>();
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("sp_getFileteredUsers", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@userTypeID", id);
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {
                    UserTypes usr = new UserTypes();
                    usr.UserTypeID = Convert.ToInt32(sdr["UserTypeID"]);
                    usr.UserID = Convert.ToInt32(sdr["UserID"]);
                    usr.UserName = sdr["UserName"].ToString();
                    usrType.Add(usr);
                }
            }
            sqlcon.Close();

            return usrType;
        }

        /// <summary>
        /// To Update The Mapping details of User and userType
        /// </summary>
        /// <param name="UserTypeID">UserTypeID</param>
        /// <param name="myTable">List of UserID</param>
        /// <returns></returns>
        public int UpdateUserMappings(int UserTypeID, DataTable myTable,DataTable bTable)//sp_updateUserMappings  sp_InsertUserMappingDetails
        {
            int retVal = 0;
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("sp_updateUserMappings", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@userTypeID", UserTypeID);
            SqlParameter sqlParam = cmd.Parameters.AddWithValue("@usrTable", myTable);
            cmd.Parameters.AddWithValue("@bTable", bTable);
            sqlParam.SqlDbType = SqlDbType.Structured;
            cmd.ExecuteNonQuery();
            sqlcon.Close();
            return retVal;
        }

        /// <summary>
        /// Save Mapping Details between User And UserType
        /// </summary>
        /// <param name="UserTypeID">UserTypeID</param>
        /// <param name="myTable">List of UserID</param>
        /// <returns></returns>
        public int InsertUserMappingDetails(int UserTypeID, DataTable myTable)
        {
            int retVal = 0;
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("sp_InsertUserMappingDetails", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@userTypeID", UserTypeID);
            SqlParameter sqlParam = cmd.Parameters.AddWithValue("@usrTable", myTable);
            sqlParam.SqlDbType = SqlDbType.Structured;
            cmd.ExecuteNonQuery();
            sqlcon.Close();
            return retVal;
        }

        /// <summary>
        /// Delete Mapping Details between User And UserType
        /// </summary>
        /// <param name="UserTypeID">UserTypeID</param>
        /// <param name="myTable">List of UserID</param>
        /// <returns></returns>
        public int DeleteUserMappingDetails(int UserTypeID, DataTable myTable)//sp_updateUserMappings  sp_InsertUserMappingDetails
        {
            int retVal = 0;
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("sp_DeleteUserMappingDetails", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@userTypeID", UserTypeID);
            SqlParameter sqlParam = cmd.Parameters.AddWithValue("@usrTable", myTable);
            sqlParam.SqlDbType = SqlDbType.Structured;
            cmd.ExecuteNonQuery();
            sqlcon.Close();
            return retVal;
        }
    }
}