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
        public static Data.MongoHelper DataHelper = new Data.MongoHelper("mongodb://127.0.0.1:27017/ewallet_business", "ewallet_business");
        public JsonResult profile(string password, string address, string personal_id, string personal_id_issued_date, string personal_id_issued_by)
        {
            dynamic response = (dynamic)Session["user_profile"];
            if (response.password != password)
            {
                response.password = Common.Security.GenPasswordHash(response.user_name, password);
            }
                response.address = address;
                response.personal_id = personal_id;
                response.personal_id_issued_date = personal_id_issued_date;
                response.personal_id_issued_by = personal_id_issued_by;
                DataHelper.Save("profile", response);
                Response.Redirect("~/Home/Index");
           return Json(new { error_code = response.error_code.ToString(), error_message = response.error_message.ToString() }, JsonRequestBehavior.AllowGet);

        }
    }
}