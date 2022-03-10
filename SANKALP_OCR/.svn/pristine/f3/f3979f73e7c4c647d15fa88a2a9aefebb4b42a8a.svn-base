using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvoiceNew.Models;
using InvoiceNew.DataAccess;
using System.Data;
using System.Web.Script.Serialization;
using System.Data.SqlClient;

namespace InvoiceNew.Controllers
{
    internal class BranchSelectModel
    {
        public int BranchID { get; set; }
        public string BranchName { get; set; }
    }
    public class UsersMappingController : IQinvoiceController
    {
        // GET: UsersMapping
        UserMappingModel ousr = new UserMappingModel();
        UserRepository usrRep = new DataAccess.UserRepository();
        static string constr = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBconnection"].ConnectionString;
        static SqlConnection con = new SqlConnection(constr);
        [OutputCache(Duration = 0)]
        public ActionResult Index()
        {
            List<SelectListItem> UserTypeitems = new List<SelectListItem>();
            IEnumerable<UserTypes> ObjUsers = (IEnumerable<UserTypes>)usrRep.getUserTypes();
            UserTypes usr = new UserTypes();

            foreach (var itm in ObjUsers)
            {
                UserTypeitems.Add(new SelectListItem
                {
                    Text = itm.UserTypeName.ToString(),
                    Value = itm.UserTypeID.ToString()
                });

            }
            usr.Users = UserTypeitems;
            ViewBag.Users = UserTypeitems;

            List<BranchSelectModel> bl = new List<BranchSelectModel>();
            con.Open();
            SqlCommand cmd = new SqlCommand("getBranchDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {
                    BranchSelectModel model = new BranchSelectModel();
                    model.BranchID = Convert.ToInt32(sdr["BranchID"]);
                    model.BranchName = sdr["BranchName"].ToString();
                   
                    bl.Add(model);

                }
            }
            con.Close();
            JavaScriptSerializer serial = new JavaScriptSerializer();
            var BranchData = Json(new { branchlist = bl });
            ViewBag.BranchData = serial.Serialize(bl);
            

            int selectedUserTypeID = usrRep.getUserMappingDetails().Min(x => x.UserTypeID);
            Session["selectedUserTypeID"] = selectedUserTypeID;
            return View();
      

        }

        public ActionResult DisplayData(int Id)
        {
            UserRepository usrRep = new DataAccess.UserRepository();
            List<UserMappingModel> objusrList = new List<UserMappingModel>();
            List<UserMappingModel> objusr = new List<UserMappingModel>();
            List<SelectListItem> UserTypeitems = new List<SelectListItem>();

            IEnumerable<UserTypes> ObjUsers = (IEnumerable<UserTypes>)usrRep.getUserTypes();
            UserTypes usr = new UserTypes();

            foreach (var itm in ObjUsers)
            {
                UserTypeitems.Add(new SelectListItem
                {
                    Text = itm.UserTypeName.ToString(),
                    Value = itm.UserTypeID.ToString()
                });

            }
            usr.Users = UserTypeitems;
            ViewBag.Users = UserTypeitems;
            //   usr.UserName =

            //int selectedUserTypeID = Convert.ToInt32(Session["selectedUserTypeID"]);
            //for (int x = 0; x < MaxRowCount; x++)
            //{
            objusr = usrRep.getUserMappingDetails().Where(a => a.UserTypeID == Id).ToList<UserMappingModel>();

            List<SelectListItem> items = new List<SelectListItem>();
            bool sel = false;
            foreach (var team in objusr)
            {
                sel = false;
                if (team.UserTypeID == Id)
                {
                    sel = true;
                }
                var item = new SelectListItem
                {
                    Value = team.UserID.ToString(),
                    Text = team.UserName,
                    Selected = sel

                };
                items.Add(item);
            }
            string[] TeamsIds = new string[objusr.Count];
            for (int i = 0; i < objusr.Count; i++)
            {
                TeamsIds[i] = objusr[i].UserID.ToString();
            }

            //    MultiSelectList teamsList = new MultiSelectList(items.OrderBy(i => i.Text), "Value", "Text");
            MultiSelectList teamsList = new MultiSelectList(objusr.ToList().OrderBy(i => i.UserID), "UserID", "UserName", TeamsIds);

            ousr.TeamIds = TeamsIds.ToList();
            ousr.Teams = teamsList;
            ModelState.Clear();
            return View("Index", ousr);
        }


        public UserMappingModel GetUserData(int usrTypeId)
        {
            UserMappingModel ousr = new UserMappingModel();
            List<UserMappingModel> objusr = new List<UserMappingModel>();
            //UserMappingModel ousr = new UserMappingModel();

            // objusrList = usrRep.getUserMappingDetails();
            List<SelectListItem> UserTypeitems = new List<SelectListItem>();

            IEnumerable<UserTypes> ObjUsers = (IEnumerable<UserTypes>)usrRep.getUserTypes();
            UserTypes usr = new UserTypes();

            foreach (var itm in ObjUsers)
            {
                UserTypeitems.Add(new SelectListItem
                {
                    Text = itm.UserTypeName.ToString(),
                    Value = itm.UserTypeID.ToString()
                });

            }
            usr.Users = UserTypeitems;
            ViewBag.Users = UserTypeitems;
            // int selectedUserTypeID = usrRep.getUserMappingDetails().Min(x => x.UserTypeID);

            objusr = usrRep.getUserMappingDetails().Where(a => a.UserTypeID == usrTypeId).ToList<UserMappingModel>();

            List<SelectListItem> items = new List<SelectListItem>();
            bool sel = false;
            foreach (var team in objusr)
            {
                sel = false;
                if (team.UserTypeID == usrTypeId)
                {
                    sel = true;
                }
                var item = new SelectListItem
                {
                    Value = team.UserID.ToString(),
                    Text = team.UserName,
                    Selected = sel

                };
                items.Add(item);
            }
            string[] TeamsIds = new string[objusr.Count];
            for (int i = 0; i < objusr.Count; i++)
            {
                TeamsIds[i] = objusr[i].UserID.ToString();
            }

            //    MultiSelectList teamsList = new MultiSelectList(items.OrderBy(i => i.Text), "Value", "Text");
            MultiSelectList teamsList = new MultiSelectList(objusr.ToList().OrderBy(i => i.UserID), "UserID", "UserName", TeamsIds);

            ousr.TeamIds = TeamsIds.ToList();
            ousr.Teams = teamsList;
            ousr.MultiBranch = objusr[0].MultiBranch;
            return ousr;
        }
        [HttpPost]
        public ActionResult GetUserDetails(string Id)
        {
            UserMappingModel ousr = new UserMappingModel();
            if (!String.IsNullOrWhiteSpace(Id))
            {
                int usrTypeId = Convert.ToInt32(Id);
                Session["selectedUserTypeID"] = usrTypeId;

                ousr = GetUserData(usrTypeId);
                ousr.BranchDetails = GetBranchDetails(usrTypeId);
                JavaScriptSerializer serial = new JavaScriptSerializer();
                var BranchData = Json(new { branchDetails = ousr.BranchDetails });
                ViewBag.BranchDetails = serial.Serialize(BranchData);
            }
            return PartialView("_EnabledUsers", ousr);
        }
        private List<BranchFillModel> GetBranchDetails(int usertype)
        {
            List<BranchFillModel> list = new List<BranchFillModel>();
            con.Open();
            SqlCommand cmd = new SqlCommand("getBranchFill", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserType",usertype);
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {
                    BranchFillModel model = new BranchFillModel();
                    model.BranchID = Convert.ToInt32(sdr["BranchID"]);
                    model.BranchName = sdr["BranchName"].ToString();
                    model.UserID = Convert.ToInt32(sdr["UserID"].ToString());
                    model.UserName = sdr["UserName"].ToString();
                    list.Add(model);

                }
            }
            con.Close();
            return list;
        }

        [HttpPost]
        public ActionResult GetUserDeatilsList(int usertypeId)
        {
            int usrTypeId = Convert.ToInt32(usertypeId);
            Session["selectedUserTypeID"] = usrTypeId;
            return RedirectToAction("DisplayData", new System.Web.Routing.RouteValueDictionary(new { controller = "UsersMapping", action = "DisplayData", Id = usrTypeId }));
        }
       

        [HttpPost]
        public ActionResult GetUserTypesDeatils(string usertypeId)
        {
            int usrTypeId = Convert.ToInt32(usertypeId);
            Session["selectedUserTypeID"] = usrTypeId;
            return RedirectToAction("DisplayData", new System.Web.Routing.RouteValueDictionary(new { controller = "UsersMapping", action = "DisplayData", Id = usrTypeId }));
        }
        [HttpPost]
        public ActionResult UpdateDetails(string usertypeid, int[] userList , int[][] branchList)
        {
            UserRepository usrRep = new UserRepository();
            int usrTypeId = Convert.ToInt32(usertypeid);
            Session["selectedUserTypeID"] = usrTypeId;
            DataTable myTable = new DataTable();
            DataTable bTable = new DataTable();
            myTable.Columns.Add("UserID", typeof(int));
            foreach (int item in userList)
            {
                myTable.Rows.Add(item);
            }
            bTable.Columns.Add("UserID",typeof(int));
            bTable.Columns.Add("BranchID", typeof(int));
            for(int i = 0; i < branchList.Length; i++)
            {
                for(int j = 0; j < branchList[i].Length; j++)
                {
                    bTable.Rows.Add(userList[i], branchList[i][j]);
                }
                
            }
            usrRep.UpdateUserMappings(usrTypeId, myTable,bTable);
            Session["selectedUserTypeID"] = usrTypeId;
            return RedirectToAction("DisplayData", new System.Web.Routing.RouteValueDictionary(new { controller = "UsersMapping", action = "DisplayData", Id = usrTypeId }));
        }

        [HttpPost]
        public ActionResult DeleteDetails(string usertypeid, int[] userList)
        {
            UserRepository usrRep = new UserRepository();
            int usrTypeId = Convert.ToInt32(usertypeid);
            Session["selectedUserTypeID"] = usrTypeId;
            DataTable myTable = new DataTable();
            myTable.Columns.Add("UserID", typeof(int));
            foreach (int item in userList)
            {
                myTable.Rows.Add(item);
            }
            usrRep.DeleteUserMappingDetails(usrTypeId, myTable);
            List<UserTypes> objusr = new List<UserTypes>();
            objusr = usrRep.getFilteredUser(usrTypeId);
            List<SelectListItem> UserTypeitems = new List<SelectListItem>();
            //IEnumerable<UserTypes> ObjUsers = (IEnumerable<UserTypes>)usrRep.getUserTypes();
            UserTypes usr = new UserTypes();

            foreach (var itm in objusr)
            {
                UserTypeitems.Add(new SelectListItem
                {
                    Text = itm.UserName.ToString(),
                    Value = itm.UserID.ToString()
                });

            }
            ViewBag.UserList = UserTypeitems;
            return PartialView("_FilteredUsers");
            //      return RedirectToAction("DisplayData", new System.Web.Routing.RouteValueDictionary(new { controller = "UsersMapping", action = "DisplayData", Id = usrTypeId }));
        }

        [HttpPost]
        public ActionResult InsertDetails(int[] userList)
        {
            UserRepository usrRep = new UserRepository();
            int usrTypeId = Convert.ToInt32(Session["selectedUserTypeID"]);
            DataTable myTable = new DataTable();
            myTable.Columns.Add("UserID", typeof(int));
            foreach (int item in userList)
            {
                myTable.Rows.Add(item);
            }
            usrRep.InsertUserMappingDetails(usrTypeId, myTable);
            Session["selectedUserTypeID"] = usrTypeId;
            UserMappingModel ousr = new UserMappingModel();
            ousr = GetUserData(usrTypeId);
            return PartialView("_EnabledUsers",ousr);
        }

        public PartialViewResult GetFilteredUsers(string Id)
        {
            UserRepository usrRep = new UserRepository();
            int usertypeid = Convert.ToInt32(Id);
            if (usertypeid == 0)
            {
                usertypeid = Convert.ToInt32(Session["selectedUserTypeID"]);
            }
            List<UserTypes> objusr = new List<UserTypes>();
            objusr = usrRep.getFilteredUser(usertypeid);
            List<SelectListItem> UserTypeitems = new List<SelectListItem>();
            //IEnumerable<UserTypes> ObjUsers = (IEnumerable<UserTypes>)usrRep.getUserTypes();
            UserTypes usr = new UserTypes();

            foreach (var itm in objusr)
            {
                UserTypeitems.Add(new SelectListItem
                {
                    Text = itm.UserName.ToString(),
                    Value = itm.UserID.ToString()
                });

            }
            ViewBag.UserList = UserTypeitems;

            return PartialView("_FilteredUsers");

        }

        public List<SelectListItem> GetFilteredRoles(int usertypeid)
        {
            UserRepository usrRep = new UserRepository();

            List<UserMappingModel> objusrList = new List<UserMappingModel>();
            List<UserTypes> objusr = new List<UserTypes>();
            UserMappingModel ousr = new UserMappingModel();
            objusr = usrRep.getFilteredUser(usertypeid);
            int id = 3;
            List<SelectListItem> item = new List<SelectListItem>();

            foreach (var rol in objusr)
            {
                bool val = false;
                if (rol.UserID == id)
                {
                    val = true;
                }
                item.Add(new SelectListItem()
                {
                    Value = rol.UserID.ToString(),
                    Text = rol.UserName,
                    Selected = val
                });
            }

            return item;
        }
    }
}

////////////////////////////////////////////////////////////////////////////////////////
//  ousr.UserTypes[0].Users = UserTypeitems;
// ousr.UserTypes[0].SelectedUserId = 0;

//int MaxRowCount = usrRep.getUserMappingDetails().Max(x => x.RowNo);
//for (int x = 0; x < MaxRowCount; x++)
//{
//   objusr = usrRep.getUserMappingDetails().Where(a => a.RowNo == x+1).ToList<UserMappingModel>();

//    List<SelectListItem> items = new List<SelectListItem>();
//    foreach (var team in objusr)
//    {
//        var item = new SelectListItem
//        {
//            Value = team.UserID.ToString(),
//            Text = team.UserName
//        };
//        items.Add(item);
//    }
//    string[] TeamsIds = new string[objusr.Count];
//    int length = objusr.Count;
//    for (int i = 0; i < length; i++)
//    {
//        TeamsIds[i] = objusr[i].UserID.ToString();
//    }

//    //    MultiSelectList teamsList = new MultiSelectList(items.OrderBy(i => i.Text), "Value", "Text");
//    MultiSelectList teamsList = new MultiSelectList(objusr.ToList().OrderBy(i => i.UserID), "UserID", "UserName", TeamsIds);

//    objusr[0].TeamIds = TeamsIds.ToList();
//    objusr[0].Teams = teamsList;
//   // objusrList[x] = objusr[0];
//    objusrList.Add(objusr[0]);
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////