using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using MongoDB.Driver.Builders;
using System.Globalization;

namespace eWallet.Portal.Controllers
{
    public class UpdateController : Controller
    {

        public JsonResult profile(string password,string birthday, string sex, string address, string personalid, string personalid_issueddate, string personalid_issuedby)
        {
            string a="nam";
            string b="nu";
            string c="nữ";
            dynamic profile = Helper.DataHelper.Get("profile", Query.EQ("user_name", User.Identity.Name));
            profile.password = Common.Security.GenPasswordHash(profile.user_name, password);
            profile.address = address;
            profile.personal_id = personalid;
            profile.personal_id_issued_date = personalid_issueddate;
            profile.birthday = birthday;
            if(sex.Length!=0)
            {
                if (sex.Length <= 4)
                {
                    if (string.Compare(sex, a, true) == 0 || string.Compare(sex, b, true) == 0 || string.Compare(sex, c, true) == 0)
                    {

                        profile.sex = sex;
                    }
                    else
                    {
                        return Json(new { error_code = "00", error_message = "Giới tính không chính xác!" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { error_code = "00", error_message = "Nhập sai giới tính !" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                profile.sex = sex;

            }
            profile.personal_id_issued_by = personalid_issuedby;
            Helper.DataHelper.Save("profile", profile);
            return Json(new { error_code = "00", error_message = "cập nhật thành công !" }, JsonRequestBehavior.AllowGet);
        }
    }
}