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
            IMongoQuery query = null;
            string userName = User.Identity.Name.ToString();
            string transaction_type = "CASHOUT";

            if (!string.IsNullOrEmpty(userName))
                query = (query == null) ? Query.EQ("created_by", userName) : Query.And(
                    query,
                    Query.EQ("created_by", userName)
                    );
          
               query = (query == null) ? Query.EQ("transaction_type", transaction_type) : Query.And(
                    query,
                    Query.EQ("transaction_type", transaction_type)
                    );
            //Nếu id = empty -> lay all giao dich cua user
            //Neu id <> empty -> lay giao dich cua user ma co transaction_type = id
            dynamic[] list_profile = Helper.DataHelper.List("transactions",query, SortBy.Ascending("system_created_time"));
                ViewBag.Type = id;
                ViewBag.List = list_profile;
            return View();
        }
    }
}