using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace eWallet.Portal.Controllers
{
    public class UpdateController : Controller
    {
        public JsonResult profile(string password, string diachi, string cmnd, string ngaycap, string noicap)
        {
            dynamic response = (dynamic)Session["user_profile"];
            response.password = Common.Security.GenPasswordHash(response.user_name, password);
            response.diachi = diachi;
            response.cmnd = cmnd;
            response.ngaycap = ngaycap;
            response.noicap = noicap;
            Helper.DataHelper.Save("profile", response);
            return Json(new { error_code = response.error_code.ToString(), error_message = response.error_message.ToString() }, JsonRequestBehavior.AllowGet);
        }
    }
}