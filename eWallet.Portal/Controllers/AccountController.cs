using MongoDB.Driver.Builders;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace eWallet.Portal.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/
        public ActionResult List(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                id = "Personal";
            }
            dynamic[] list_profile = App_Start.eWalletConfig.BusinessDataHelper.List("finance_account", Query.NE("type","P"), SortBy.Ascending("profile"));
            ViewBag.Type = id;
            ViewBag.List = list_profile;
            return View();
        }

        public ActionResult Personal()
        {
            return View();
        }
        public ActionResult Business()
        {
            return View();
        }
        public ActionResult Me()
        {
            if (Session["user_profile"] == null)
                return RedirectToAction("Login", "Home");
            string _request = @"{system:'web_frontend', module:'finance', type:'two_way', function:'list_account_profile', request:{profile_id:"
            + ((dynamic)Session["user_profile"])._id +
            "}}";

            dynamic result = new eWallet.Data.DynamicObj(Helper.RequestToServer(_request));
            ViewBag.accounts = result.response;
            return View();
            return View();
        }

        public ActionResult Profile()
        {
            return View();
        }

        public ActionResult CashIn()
        {
            return View();
        }

        public ActionResult CashOut()
        {
            return View();
        }
        #region "Boxes"
        public ActionResult CashInATM()
        {
            return View(Url.Content("/Views/Box/CashIn_ATM.cshtml"));
        }

        public ActionResult CashInBankAccount()
        {
            return View(Url.Content("/Views/Box/CashIn_BankAccount.cshtml"));
        }

        public ActionResult CashInOthers()
        {
            return View(Url.Content("/Views/Box/CashIn_Others.cshtml"));
        }

        public ActionResult CashOutBank()
        {
            return View(Url.Content("/Views/Box/CashOut_Bank.cshtml"));
        }
        public ActionResult CashOutBankAccounts()
        {
            return View(Url.Content("/Views/Box/CashOut_BankAccounts.cshtml"));
        }

        public ActionResult BusinessProfile()
        {
            return View(Url.Content("/Views/Box/Business_Profile.cshtml"));
        }
        #endregion

        #region "Process to server"
        
        public JsonResult PostRegister(string full_name, string email, string mobile)
        {
            string request = @"{system:'web_frontend', module:'profile', function:'register', type:'two_way', request:{full_name:'" + full_name + "', id:'" + email + "', mobile:'" + mobile + "'}}";
            dynamic response = JObject.Parse(Helper.RequestToServer(request));
            return Json(new { error_code = response.error_code.ToString(), error_message = response.error_message.ToString() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PostLogin(string email, string password, bool is_remember)
        {
            string _srequest = @"{system:'web_frontend', module:'security',type:'two_way', function:'login',request:{id:'" + email + "', password:'" + password + "'}}";
            dynamic result = new eWallet.Data.DynamicObj(Helper.RequestToServer(_srequest));
            if (result.error_code.ToString() == "00")
            {
                FormsAuthentication.SetAuthCookie(email, false);
                Session["user_profile"] = result.response;
                //if (!String.IsNullOrEmpty(ReturnUrl)) return Redirect(ReturnUrl);
                //return RedirectToAction("Index", "Home");
            }
            else
            {
                //ModelState.AddModelError("login_error", result.error_message.ToString());
                //return View();
            }
            return Json(new { error_code = result.error_code.ToString(), error_message = result.error_message.ToString() }, JsonRequestBehavior.AllowGet);
        }
        #endregion "Process to server"
    }
}