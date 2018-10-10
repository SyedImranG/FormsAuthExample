using FormsAuthExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace FormsAuthExample.Controllers
{
    public class MyAccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        // Simple Authentication
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Login(Login l, string ReturnUrl = "")
        //{
        //    using (FormsAuthDBEntities db = new FormsAuthDBEntities())
        //    {
        //        var user = db.UserDetails.Where(a => a.UserName.Equals(l.Username) && a.Password.Equals(l.Password))
        //            .FirstOrDefault();
        //        if(user != null)
        //        {
        //            FormsAuthentication.SetAuthCookie(user.UserName, l.RememberMe);
        //            if(Url.IsLocalUrl(ReturnUrl))
        //            {
        //                return Redirect(ReturnUrl);
        //            }
        //            else
        //            {
        //                return RedirectToAction("MyProfile", "Home");
        //            }
        //        }
        //    }
        //    ModelState.Remove("Password");
        //    return View();
        //}

        // Second Approach
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Login(Login l, string ReturnUrl = "")
        //{
        //    if(ModelState.IsValid)
        //    {
        //        MyMembershipProvider membership = new MyMembershipProvider();
        //        var isValidUser = membership.ValidateUser(l.Username, l.Password);
        //        if (isValidUser)
        //        {
        //            FormsAuthentication.SetAuthCookie(l.Username, l.RememberMe);
        //            if (Url.IsLocalUrl(ReturnUrl))
        //            {
        //                return Redirect(ReturnUrl);
        //            }
        //            else
        //            {
        //                return RedirectToAction("MyProfile", "Home");
        //            }
        //        }
        //    }
                
        //    ModelState.Remove("Password");
        //    return View();
        //} 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login l, string ReturnUrl = "/")
        {
            if(ModelState.IsValid)
            {
                MyMembershipProvider membership = new MyMembershipProvider();
                bool isValidUser = membership.ValidateUser(l.Username, l.Password);
                if(isValidUser)
                {
                    UserDetail user = null;
                    using(FormsAuthDBEntities dc = new FormsAuthDBEntities())
                    {
                        user = dc.UserDetails.Where(a => a.UserName.Equals(l.Username)).FirstOrDefault();
                    }
                    if(user != null)
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        string data = js.Serialize(user);
                        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket
                            (1, user.UserName, DateTime.Now, DateTime.Now.AddMinutes(30), l.RememberMe, data);
                        string encToken = FormsAuthentication.Encrypt(ticket);
                        HttpCookie authoCookies = new HttpCookie(FormsAuthentication.FormsCookieName, encToken);
                        Response.Cookies.Add(authoCookies);
                        return Redirect(ReturnUrl);
                    }
                }
            }
            ModelState.Remove("Password");
            return View();
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

    }
}
    