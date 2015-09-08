﻿using eWallet.Portal.Models;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
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
        public JsonResult UserWallet(string userwallet, int? page, int? page_size)
        {
            IMongoQuery query = Query.NE("type", "P");
            if (!string.IsNullOrEmpty(userwallet))
                query = Query.And(
                         query,
                      Query.EQ("user_name", userwallet)
                      );
            if (userwallet == User.Identity.Name)
            {
                return Json(new { error_code = "00", error_message = "tài khoản nhận tiền phải khác tài khoản đang đăng nhâp!", list = "" }, JsonRequestBehavior.AllowGet);

            }
            
            if (page == null) page = 1;
            if (page_size == null) page_size = 25;
            long total_page = 0;
            var _list = Helper.DataHelper.ListPagging("profile",
            query,
            SortBy.Ascending("_id"),
            (int)page_size,
            (int)page,
            out total_page
            );
            var list_accounts = (from e in _list select e).Select(p => new
            {
                _id = p._id,
                full_name = p.full_name,
                user_name = p.user_name              
            }).ToArray();
            return Json(new { error_code = "00", error_message = "Sussess!", list = list_accounts }, JsonRequestBehavior.AllowGet);//
        }
        #region "CASHIN PROCESS"
        [Authorize]
        public JsonResult CashIn_ATM(string amount, string bank)
        {
            bank = ProxyController.GetBankCode(bank);
            string request = @"{system:'web_frontend', module:'transaction',type:'two_way', function:'CASHIN',request:{channel:'WEB', profile:'"
                 + User.Identity.Name
                + "',service:'GNCP', provider:'BANKNET', payment_provider:'BANKNET', amount: " + amount +
            ", note: '" + "CASH IN ACCOUNT " + User.Identity.Name + ", AMOUNT " + amount +
            "', bank:'" + bank +
            "'}}";
            dynamic response = new eWallet.Data.DynamicObj(Helper.RequestToServer(request));
            return Json(new { error_code = response.error_code, error_message = response.error_message, url_redirect = response.response.url_redirect }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CashOut_Bank(string account_id, string amount, string note)
        {
            dynamic account = Helper.DataHelper.Get("cashout_bank_account",
                Query.EQ("_id", account_id));
            string request = @"{system:'web_frontend', module:'transaction',type:'two_way', function:'CASHOUT',request:{channel:'WEB', profile:'"
               + User.Identity.Name + "',service:'GNCP', provider:'BANK',payment_provider:'GNCA',amount: " + amount +
         ", note: '" + note +
         "', receiver:{account_bank:'" + account.bank +
         "', account_branch:'" + account.branch + "',account_number:'" + account.number +
         "',account_name:'" + account.name + "'}}}";
            dynamic response = new eWallet.Data.DynamicObj(Helper.RequestToServer(request));
            if (response.error_code == "00")
                return Json(new { error_code = response.error_code, error_message = response.error_message, url_redirect = response.response.url_redirect, trans_id = response.response.trans_id, amount = response.response.amount }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { error_code = response.error_code, error_message = response.error_message }, JsonRequestBehavior.AllowGet);
        }
        #endregion "CASHIN PROCESS"

        #region "CASHOUT PROCESS"
        public JsonResult CashIn_Bank
            (string trans_date, string account_bank, string account_number, long amount, string note)
        {
            string request = @"{system:'web_frontend', module:'transaction',type:'two_way', function:'CASHIN',request:{channel:'WEB', profile:'"
               + User.Identity.Name + "',service:'GNCP', provider:'BANK',payment_provider:'GNCA',amount: " + amount +
         ", note: '" + note +
         "', sender:{account_bank:'" + account_bank +
         "', account_number:'" + account_number +
         "',transfer_date:'" + trans_date + "'}}}";
            dynamic response = new eWallet.Data.DynamicObj(Helper.RequestToServer(request));
            if (response.error_code == "00")
                return Json(new { error_code = response.error_code, error_message = response.error_message, url_redirect = response.response.url_redirect, trans_id = response.response.trans_id, amount = response.response.amount }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { error_code = response.error_code, error_message = response.error_message }, JsonRequestBehavior.AllowGet);
        }
        #endregion "CASHOUT PROCESS"

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

        public JsonResult Payment_PayBill(string service, string provider, string bill_code, long amount, string payment_provider, string bank)
        {
            string request = @"{system:'web_frontend', module:'transaction',type:'two_way', function:'payment',request:{channel:'web', profile:'"
                + User.Identity.Name
               + "', product_code: '" + bill_code
               + "', service: '" + service
               + "', provider: '" + provider
               + "', amount: " + amount
               + ", payment_provider: '" + payment_provider
               + "', bank: '" + bank +
           "'}}";
            dynamic response = new eWallet.Data.DynamicObj(Helper.RequestToServer(request));
            if (response.error_code == "00")
                return Json(new { error_code = response.error_code, error_message = response.error_message, url_redirect = response.response.url_redirect, trans_id = response.response.trans_id, amount = response.response.amount }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { error_code = response.error_code, error_message = response.error_message }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region "TOPUP PROCESS"
        public JsonResult Topup_Mobile(string mobile, string service, string provider, string price, string payment_provider, string bank)
        {
            string request = @"{system:'web_frontend', module:'transaction',type:'two_way', function:'TOPUP',request:{channel:'WEB', profile:'"
                + User.Identity.Name
               + "', service:'" + service
               + "', provider:'" + provider
               + "', ref_id: '" + mobile
               + "', amount: " + price
               + ", quantity: 1"
               + ", payment_provider: '" + payment_provider
               + "', bank: '" + bank +
           "'}}";
            dynamic response = new eWallet.Data.DynamicObj(Helper.RequestToServer(request));
            return Json(new { error_code = response.error_code, error_message = response.error_message, url_redirect = response.response.url_redirect, trans_id = response.response.trans_id, amount = response.response.amount }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Topup_Online(string accoount_id, string service, string provider, string price, string payment_provider, string bank)
        {
            string request = @"{system:'web_frontend', module:'transaction',type:'two_way', function:'TOPUP',request:{channel:'WEB', profile:'"
                + User.Identity.Name
               + "', service:'" + service
               + "', provider:'" + provider
               + "', ref_id: '" + accoount_id
               + "', amount: " + price
               + ", quantity: 1"
               + ", payment_provider: '" + payment_provider
               + "', bank: '" + bank +
           "'}}";
            dynamic response = new eWallet.Data.DynamicObj(Helper.RequestToServer(request));
            return Json(new { error_code = response.error_code, error_message = response.error_message, url_redirect = response.response.url_redirect, trans_id = response.response.trans_id, amount = response.response.amount }, JsonRequestBehavior.AllowGet);
        }
        #endregion "TOPUP PROCESS"
    }
}