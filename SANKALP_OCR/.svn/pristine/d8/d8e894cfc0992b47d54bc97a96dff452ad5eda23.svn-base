using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvoiceNew.Models;
using InvoiceNew.DataAccess;
using System.Data;

namespace InvoiceNew.Controllers
{
    public class UsersMappingController : IQinvoiceController
    {
        // GET: UsersMapping
        UserMappingModel ousr = new UserMappingModel();
        UserRepository usrRep = new DataAccess.UserRepository();
        [OutputCache(Duration = 0)]
        public ActionResult Index()
        {
        
            List<UserMappingModel> objusrList = new List<UserMappingModel>();
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
            //   usr.UserName =

            int selectedUserTypeID = usrRep.getUserMappingDetails().Min(x => x.UserTypeID);
            //if (Id > 0)
            //{
            //    selectedUserTypeID = Id;
            //}
            Session["selectedUserTypeID"] = selectedUserTypeID;
            //for (int x = 0; x < MaxRowCount; x++)
            //{
            objusr = usrRep.getUserMappingDetails().Where(a => a.UserTypeID == selectedUserTypeID).ToList<UserMappingModel>();

            List<SelectListItem> items = new List<SelectListItem>();
            bool sel = false;
            foreach (var team in objusr)
            {
                sel = false;
                if (team.UserTypeID == selectedUserTypeID)
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
          //  ModelState.Clear();
            return View(ousr);
      

        }

        public ActionResult DisplayData(int Id)
        {
            UserRepository usrRep = new DataAccess.UserRepository();
            List<UserMappingModel> objusrList = new List<UserMappingModel>();
            List<UserMappingModel> objusr = new List<UserMappingModel>();
          //  UserMappingModel ousr = new UserMappingModel();

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
      [HttpPost]
        public ActionResult GetUserDeatils(int usertypeId)
        {
            int usrTypeId = Convert.ToInt32(usertypeId);
            Session["selectedUserTypeID"] = usrTypeId;

            IEnumerable<UserTypes> ObjUsers = (IEnumerable<UserTypes>)usrRep.getUserMappingDetails().Where(a => a.UserTypeID == usertypeId).ToList<UserMappingModel>();
            UserTypes usr = new UserTypes();
            List<SelectListItem> UserTypeitems = new List<SelectListItem>();

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
            return PartialView("_EnabledUsers", ObjUsers);
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
        public ActionResult UpdateDetails(string usertypeid, int[] userList)
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
            usrRep.UpdateUserMappings(usrTypeId, myTable);
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
            //    return RedirectToAction("Index", new { id = usrTypeId });
            return RedirectToAction("DisplayData", new System.Web.Routing.RouteValueDictionary(new { controller = "UsersMapping", action = "DisplayData", Id = usrTypeId }));
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
            //    return RedirectToAction("Index", new { id = usrTypeId });
            return RedirectToAction("DisplayData", new System.Web.Routing.RouteValueDictionary(new { controller = "UsersMapping", action = "DisplayData", Id = usrTypeId }));
        }
        public PartialViewResult GetFilteredUsers(string Id = "0")
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