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

            if (!string.IsNullOrEmpty(userName))
                query = (query == null) ? Query.EQ("created_by", userName) : Query.And(
                    query,
                    Query.EQ("created_by", userName)
                    );

            //Nếu id = empty -> lay all giao dich cua user
            //Neu id <> empty -> lay giao dich cua user ma co transaction_type = id
            dynamic[] list_profile = Helper.DataHelper.List("transactions",query, SortBy.Descending("system_created_time"));
                ViewBag.Type = id;
                ViewBag.List = list_profile;
            return View();
        }
        public ActionResult Cashin(string id)
        {
            IMongoQuery query = null;
            string userName = User.Identity.Name.ToString();
            string transaction = "cashin";

            query = (query == null) ? Query.EQ("transaction", transaction) : Query.And(
                query,
                Query.EQ("transaction", transaction)
                );

            if (!string.IsNullOrEmpty(userName))
                query = (query == null) ? Query.EQ("created_by", userName) : Query.And(
                    query,
                    Query.EQ("created_by", userName)
                    );

            dynamic[] list_profile = Helper.DataHelper.List("transactions", query, SortBy.Descending("system_created_time"));
            ViewBag.Type = id;
            ViewBag.List = list_profile;
            return View();
        }

        public ActionResult Cashout(string id)
        {
            IMongoQuery query = null;
            string userName = User.Identity.Name.ToString();
            string transaction = "cashout";

            query = (query == null) ? Query.EQ("transaction", transaction) : Query.And(
                query,
                Query.EQ("transaction", transaction)
                );

            if (!string.IsNullOrEmpty(userName))
                query = (query == null) ? Query.EQ("created_by", userName) : Query.And(
                    query,
                    Query.EQ("created_by", userName)
                    );

            dynamic[] list_profile = Helper.DataHelper.List("transactions", query, SortBy.Descending("system_created_time"));
            ViewBag.Type = id;
            ViewBag.List = list_profile;
            return View();
        }
    }
}