using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eWallet.Portal.Controllers
{
    public class PaymentController : Controller
    {
        //
        // GET: /Payment/
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Success(string transaction_type, string trans_id, string amount)
        {
            string request = @"{system:'web_frontend', module:'transaction',type:'two_way', function:'confirm',request:{transaction_type:'" + transaction_type + "', user_id:'" + ((dynamic)Session["user_profile"])._id
                + "',trans_id:'"
            + trans_id +
            "', amount: " + amount +
            "}}";
            dynamic result = JObject.Parse(Helper.RequestToServer(request));
            ViewBag.Result = "không thành công";
            if (result.error_code == "00")
            {
                ViewBag.Result = "thành công";

            }
            return View("Result");
        }

        public ActionResult Fail(string transaction_type, string trans_id, string amount)
        {
            string request = @"{system:'web_frontend', module:'transaction',type:'two_way', function:'cancel',request:{transaction_type:'" + transaction_type + "', user_id:" + ((dynamic)Session["user_profile"])._id
                + ",trans_id:'"
            + trans_id +
            "', amount: " + amount +
            "}}";
            dynamic result = JObject.Parse(Helper.RequestToServer(request));
            ViewBag.Result = "không thành công";
            if (result.error_code == "00")
            {
                ViewBag.Result = "thành công";

            }
            return View("Result");
        }

        public ActionResult Confirm(string transaction_type, string trans_id, long amount)
        {
            Helper.SendRequestOTP(((dynamic)Session["user_profile"])._id);
            ViewBag.transaction_type = transaction_type;
            ViewBag.trans_id = trans_id;
            ViewBag.amount = amount;
            return View();
        }

        public ActionResult ConfirmWithOTP(string transaction_type, string trans_id, long amount, string otp)
        {
            string request = @"{system:'web_frontend', module:'transaction',type:'two_way',function:'confirm_otp',request:{user_id:" + ((dynamic)Session["user_profile"])._id
                + ",transaction_type:'" + transaction_type + "', trans_id:'" + trans_id + "', amount: " + amount + ", otp:'" + otp + "'}}";
            dynamic result = JObject.Parse(Helper.RequestToServer(request));
            if (result.error_code != "00")
            {
                ViewBag.Result = "không thành công";
                if (result.response.url_redirect != null) return Redirect(result.response.url_redirect.ToString());
                return View("Result");
            }
            else
            {
                ViewBag.Result = "thành công";
                if (result.response.url_redirect != null) return Redirect(result.response.url_redirect.ToString());
                ViewBag.Trans = result.response;
                return View("Result");
            }
        }
	}
}