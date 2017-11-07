using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AimsHub.Models;
using AimsHub.Security;
using System.Web.Security;

namespace AimsHub.Controllers
{
    public class LoginController : Controller
    {
        // GET: Account
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Authorize()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Index(AccountModel model, string returnURL)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            try
            {
                if (Membership.ValidateUser(model.UserID, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserID, false);
                    HubSecurity.loadUserAD(model.UserID);
                    if (HubSecurity.isExpired)
                    {
                        return this.RedirectToAction("Change", "Login");
                    }
                    else
                    {
                        if (HubSecurity.isPracAdmin)
                        {
                            return this.RedirectToAction("Index", "PracticeAdmin");
                        }
                        else
                        {
                            return this.RedirectToAction("Index", "Home");
                        }   
                    }                  
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "The username or password is incorrect. Please try again.");
                    return this.View(model);
                }
            }
           catch (Exception ex)
            {
                var test = ex.Message;
                this.ModelState.AddModelError(string.Empty, "Something went wrong trying to reach the login server. Please contact IT.");
                return this.View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult Change()
        {
            var model = new AccountChangeModel
            {
                UserID = HttpContext.User.Identity.Name.ToString(),
                success = false             
            };

            //string ftt = Session["TESTdomainPolicy"].ToString();
            ////int64 sixfour = Convert.ToInt64(ftt);
            //Int64 ft = Convert.ToInt64(ftt);
            ////TEST CODE
            //try
            //{
            //    //model.domainPolicy = DateTime.FromFileTime((long)Session["TESTdomainPolicy"]).ToShortDateString() + ' ' + DateTime.FromFileTime((long)Session["TESTdomainPolicy"]).ToShortTimeString();
                
            //    DateTime dt = new DateTime(1601, 01, 02).AddTicks(ft);
            //    model.domainPolicy = dt.ToString();
            //}
            //catch
            //{
            //    model.domainPolicy = "nope";
            //}
            //try
            //{
            //    //model.lastReset = DateTime.FromFileTime((long)Session["TESTlastReset"]).ToShortDateString() + ' ' + DateTime.FromFileTime((long)Session["TESTlastReset"]).ToShortTimeString();
            //    DateTime test = DateTime.FromFileTime((long)Session["TESTlastReset"]);
            //    model.lastReset = String.Format("You must change your password within" +
            //                          " {0} days"
            //                         , test);
            //}
            //catch
            //{
            //    model.lastReset = "nope";
            //}

            //model.lastReset = DateTime.FromFileTime((long)Session["TESTlastReset"]).ToShortDateString() + ' ' + DateTime.FromFileTime((long)Session["TESTlastReset"]).ToShortTimeString();
            //model.daysLeft = model.domainPolicy - DateTime.Today.Subtract(DateTime.FromFileTime((long)Session["TESTlastReset"]).Day;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Change(AccountChangeModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }
            if (model.newPassword != model.confirmPassword)
            {
                this.ModelState.AddModelError(string.Empty, "The New Password and Confirm Password do not match. Please try again.");
                return this.View(model);
            }

            try
            {              
                if (Membership.ValidateUser(model.UserID, model.Password))
                {
                    HubSecurity.ChangePassword(model);
                    model.success = true;
                    return this.View(model);
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "The username or password is incorrect. Please try again.");
                    return this.View(model);
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
                //return Content(ex.Message);
            }
        }

        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();

            return this.RedirectToAction("Index", "Login");
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return View();
        }

        [AllowAnonymous]
        public ActionResult Timeout()
        {
            FormsAuthentication.SignOut();
            return View();
        }

        [HttpPost]
        public ActionResult ResetSession()
        {
            return Content("");
        }
    }

}