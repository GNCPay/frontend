using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eWallet.Portal.Controllers
{
    public class TransactionController : Controller
    {

        // GET: /Transaction
        public ActionResult List(string id)
        {
            //id: transaction_type
            if (Session["user_profile"] != null)
            {
                //Nếu id = empty -> lay all giao dich cua user
                //Neu id <> empty -> lay giao dich cua user ma co transaction_type = id
                dynamic trans = (dynamic)Session["user_profile"];
                dynamic[] list_profile = Helper.DataHelper.List("transactions", Query.EQ("created_by", trans._id), SortBy.Descending("system_created_time"));
                ViewBag.Type = id;
                ViewBag.List = list_profile;
            }
            return View();
        }
    }
}