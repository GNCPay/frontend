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
           
                //Nếu id = empty -> lay all giao dich cua user
                //Neu id <> empty -> lay giao dich cua user ma co transaction_type = id
                dynamic[] list_profile = Helper.DataHelper.List("transactions", Query.EQ("created_by", User.Identity.Name), SortBy.Descending("system_created_time"));
                ViewBag.Type = id;
                ViewBag.List = list_profile;
            return View();
        }
    }
}