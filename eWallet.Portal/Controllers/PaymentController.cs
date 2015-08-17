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
	}
}