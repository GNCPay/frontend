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
        //public static Data.MongoHelper DataHelper = new Data.MongoHelper("mongodb://127.0.0.1:27017/ewallet_business", "ewallet_business");

        public JsonResult profile(string password, string Address, string personalid, string personalid_issueddate, string personalid_issuedby)
        {
            dynamic response = (dynamic)Session["user_profile"];
            if (response.password != password)
            {
                response.password = Common.Security.GenPasswordHash(response.user_name, password);
            }
            response.Address = Address;
            response.personalid = personalid;
            response.personalid_issueddate = personalid_issueddate;
            response.personalid_issuedby = personalid_issuedby;
            Helper.DataHelper.Save("profile", response);
            return Json(new { error_message = "cập nhật thành công !" }, JsonRequestBehavior.AllowGet);
        }
    }
}