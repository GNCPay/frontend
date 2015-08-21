﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace eWallet.Portal.Controllers
{
    public class UpdateController : Controller
    {

        public JsonResult profile(string password, string address, string personalid, string personalid_issueddate, string personalid_issuedby)
        {
            dynamic profile = (dynamic)Session["user_profile"];
            if (profile.password != password)
            {
                profile.password = Common.Security.GenPasswordHash(profile.user_name, password);
            }
            profile.address = address;
            profile.personal_id = personalid;
            profile.personal_id_issued_date = personalid_issueddate;
            profile.personal_id_issued_by = personalid_issuedby;
            Helper.DataHelper.Save("profile", profile);
            return Json(new {error_code="00", error_message = "cập nhật thành công !" }, JsonRequestBehavior.AllowGet);
        }
    }
}