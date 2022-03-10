using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace InvoiceNew.Controllers
{
    public class IQinvoiceController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
          try
          { 

            if (Session["UserID"] == null)
            {

                var url = new UrlHelper(filterContext.RequestContext);
                var loginUrl = url.Content("~/Login/Login");
                filterContext.HttpContext.Response.Redirect(loginUrl, true);

            }
          

            string actionName = filterContext.ActionDescriptor.ActionName;
            bool CheckSession = true;

            switch (actionName)
            {
                
                case "Login":
               
                    CheckSession = false;
                    break;
                default:
                    CheckSession = true;
                    break;
            }

            if (CheckSession)
            {
                HttpSessionStateBase session = filterContext.HttpContext.Session;
                var user = session["UserID"];
                if (user=="")
                {
                    var url = new UrlHelper(filterContext.RequestContext);
                    var loginUrl = url.Content("~/Login/Login");
                    filterContext.HttpContext.Response.Redirect(loginUrl, true);
                }
                if (((user == null) && (!session.IsNewSession)) || (session.IsNewSession))
                {
                    //Redirect to Timeout page
                    //var url = new UrlHelper(filterContext.RequestContext);
                    //var loginUrl = url.Content("~/Home/TimeoutRedirect");
                    //filterContext.HttpContext.Response.Redirect(loginUrl, true);

                    // For round-trip posts, we're forcing a redirect to Home/TimeoutRedirect/, which
                    //filterContext.Result = new RedirectToRouteResult(
                    //    new RouteValueDictionary {
                    //{ "Controller", "Home" },
                    //{ "Action", "TimeoutRedirect" }
                    //});

                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        // For AJAX requests, we're overriding the returned JSON result with a simple string,
                        // indicating to the calling JavaScript code that a redirect should be performed.
                        // filterContext.Result = Json(new { Data = "Session timeout. Redirecting..." }, JsonRequestBehavior.AllowGet);
                        //filterContext.Result = new JsonResult { Data = "Session timeout. Redirecting..." };
                        JsonResult jsonRes = new JsonResult();
                        jsonRes.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                        jsonRes.Data = "Session timeout. Redirecting...";

                        filterContext.Result = jsonRes;
                    }
                    else
                    {
                        //Redirect to Timeout page
                        var url = new UrlHelper(filterContext.RequestContext);
                        var loginUrl = url.Content("~/Login/Login");
                        filterContext.HttpContext.Response.Redirect(loginUrl, true);
                    }
                }
            }

            if (filterContext.HttpContext.Request.IsAjaxRequest() && actionName.Equals("Login"))
            {
                JsonResult jsonRes = new JsonResult();
                jsonRes.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                jsonRes.Data = "Session timeout. Redirecting...";

                filterContext.Result = jsonRes;
            }

            base.OnActionExecuting(filterContext);

            }
          catch(Exception e)
          {
              Logger.Write(e.Message);
              
          }

        }
      

        public string Besttxtfunction(string txt_in)
        {
            txt_in = txt_in.Trim();
        Line1:
            int len1 = txt_in.Trim().Length;
            string my_chr, left1, right1, chant_to, tmp_result, tmp_left, tmp_mid;

            for (int i = 0; i < len1; i++)
            {
                my_chr = txt_in[i].ToString();
                left1 = txt_in.Substring(0, i);
                int x = i + 1;
                right1 = txt_in.Substring(Convert.ToInt16(x), len1 - x);

                switch (my_chr)
                {
                    case "ة":
                        chant_to = "ه";
                        txt_in = left1 + chant_to + right1;
                        goto Line1;
                    //break;
                    case "أ":
                    case "إ":
                    case "آ":
                        chant_to = "ا";
                        txt_in = left1 + chant_to + right1;
                        goto Line1;
                    //break;
                    case "ي":
                        if (len1 - 1 == i)
                        {
                            chant_to = "ى";
                            txt_in = left1 + chant_to + right1;
                            goto Line1;
                        }
                        else if (txt_in.Substring(i, 1) == " ")
                        {
                            chant_to = "ى";
                            txt_in = left1 + chant_to + right1;
                            goto Line1;
                        }
                        break;

                    case "ـ":
                        chant_to = "";
                        txt_in = left1 + chant_to + right1;
                        goto Line1;
                    //break;
                    case " ":
                        if (len1 >= i + 1)
                        {
                            if (txt_in.Substring(i + 1, 1) == " ")
                            {
                                chant_to = "";
                                txt_in = left1 + chant_to + right1;
                                goto Line1;
                            }
                        }
                        break;
                }

            }
            //BestText = txt_in
            int len_Word = 0;
            //NoSpaceWord = tmpword
            string bestText = txt_in;

            len_Word = txt_in.Length;
            for (int j = 0; j < len_Word; j++)
            {
                int Diff = len_Word - j;
                if (Diff >= 5)
                {
                    tmp_result = bestText.Substring(j, 5);
                    //MsgBox Len(tmp_result)
                    if (tmp_result == "عبدال")
                    {
                        tmp_left = bestText.Substring(0, j);

                        int y = j + 4;

                        tmp_mid = bestText.Substring(y, len_Word - y);
                        bestText = tmp_left + "عبد ا" + tmp_mid;
                        len_Word = bestText.Length;
                    }

                }
            }
            return bestText;
        } 
	}
}