using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eWallet.Portal.Controllers
{
    public class ProfileController : Controller
    {
        //
        // GET: /Profile/
        public ActionResult List(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                id = "Personal";
            }
            dynamic[] list_profile = App_Start.eWalletConfig.BusinessDataHelper.List("profile", Query.EQ("type",id));
            ViewBag.Type = id;
            ViewBag.List = list_profile;
            return View();
        }
        public JsonResult ChangeLock(long profile_id)
        {
            dynamic profile = App_Start.eWalletConfig.BusinessDataHelper.Get("profile", Query.EQ("_id", profile_id));
            if (profile == null)
            {
                return Json(new { error_code = "96", error_message = "The profile is not exist!"}, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (profile.status == "ACTIVED")
                {
                    profile.status = "LOCKED";
                }
                else profile.status = "ACTIVED";
                App_Start.eWalletConfig.BusinessDataHelper.Save("profile", profile);
                return Json(new { error_code = "00", error_message = "Success!" }, JsonRequestBehavior.AllowGet);
            }
        }
	}
}