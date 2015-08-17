using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace eWallet.Portal.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult eWallet()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            Session["user_profile"] = null;
            return RedirectToAction("eWallet", "Home");
        }
        public ActionResult Register()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


    }
}