using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eWallet.Portal.Controllers
{
    public class ServiceController : Controller
    {
        //
        // GET: /Service/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Payment()
        {
            return View();
        }
        public ActionResult Topup()
        {
            return View();
        }

        public ActionResult Transfer()
        {
            return View();
        }

        public ActionResult Collection()
        {
            return View();
        }
        #region "Service Box"
        public ActionResult PaymentBilling()
        {
            return View(Url.Content("/Views/Box/Payment_Billing.cshtml"));
        }
        public ActionResult PaymentLoan()
        {
            return View(Url.Content("/Views/Box/Payment_Loan.cshtml"));
        }
        public ActionResult PaymentCommerce()
        {
            return View(Url.Content("/Views/Box/Payment_Commerce.cshtml"));
        }

        public ActionResult TopupMobile()
        {
            return View(Url.Content("/Views/Box/Topup_Mobile.cshtml"));
        }
        public ActionResult TopupOnline()
        {
            return View(Url.Content("/Views/Box/Topup_Online.cshtml"));
        }

        public ActionResult TransferWallet()
        {
            return View(Url.Content("/Views/Box/Transfer_Wallet.cshtml"));
        }
        public ActionResult TransferBank()
        {
            return View(Url.Content("/Views/Box/Transfer_Bank.cshtml"));
        }
        public ActionResult TransferMobile()
        {
            return View(Url.Content("/Views/Box/Transfer_Mobile.cshtml"));
        }

        public ActionResult CollectionRequest()
        {
            return View(Url.Content("/Views/Box/Collection_Requests.cshtml"));
        }
        #endregion

        #region "CASHIN PROCESS"
        [Authorize]
        public JsonResult CashIn_ATM(string amount, string bank)
        {
            bank = ProxyController.GetBankCode(bank);
            string request = @"{system:'web_frontend', module:'transaction',type:'two_way', function:'CASHIN',request:{channel:'WEB', profile:"
                 + ((dynamic)Session["user_profile"])._id
                + ",service:'GNCP', provider:'BANKNET', payment_provider:'BANKNET', amount: " + amount +
            ", note: '" + "CASH IN ACCOUNT " + ((dynamic)Session["user_profile"])._id  + ", AMOUNT " + amount +
            "', bank:'" + bank +
            "'}}";
            dynamic response = new eWallet.Data.DynamicObj(Helper.RequestToServer(request));
            return Json(new { error_code = response.error_code, error_message = response.error_message, url_redirect = response.response.url_redirect }, JsonRequestBehavior.AllowGet);
        }
        #endregion "CASHIN PROCESS"

        #region "PAYMENT PROCESS"
        public JObject Payment_CheckBill(string service, string provider, string bill_code)
        {
            Session["current_bill"] = null;
            string request = @"{system:'web_frontend', module:'billing',type:'two_way', function:'check_bill',request:{service:'" + service
                + "', provider:'" + provider
                + "', bill_code:'" + bill_code
                + "'}}";
            string _bill = Helper.RequestToServer(request);
            JObject result = JObject.Parse(_bill);
            Session["current_bill"] = _bill;
            return result;
        }
        #endregion
    }
}