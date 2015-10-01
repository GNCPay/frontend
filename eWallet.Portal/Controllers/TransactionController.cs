using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

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
            dynamic[] list_profile = Helper.DataHelper.List("transactions", query, SortBy.Descending("system_created_time"));
            ViewBag.Type = id;
            ViewBag.List = list_profile;
            return View();
        }
        public ActionResult Cashin(string id)
        {
            IMongoQuery query = null;
            string userName = User.Identity.Name.ToString();
            string transaction = "CASHIN";

            query = (query == null) ? Query.EQ("transaction_type", transaction) : Query.And(
                query,
                Query.EQ("transaction_type", transaction)
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
            string transaction = "CASHOUT";

            query = (query == null) ? Query.EQ("transaction_type", transaction) : Query.And(
                query,
                Query.EQ("transaction_type", transaction)
                );

            if (!string.IsNullOrEmpty(userName))
                query = (query == null) ? Query.EQ("created_by", userName) : Query.And(
                    query,
                    Query.EQ("created_by", userName)
                    );

            dynamic[] list_profile = Helper.DataHelper.List("transactions", query,SortBy.Descending("system_created_time"));
           
            ViewBag.Type = id;
            ViewBag.List = list_profile;       
            return View();
        }

        public ActionResult PayMent(string id)
        {
            IMongoQuery query = null;
            string userName = User.Identity.Name.ToString();
            string transaction = "PAYMENT";

            query = (query == null) ? Query.EQ("transaction_type", transaction) : Query.And(
                query,
                Query.EQ("transaction_type", transaction)
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

        public ActionResult TopUp(string id)
        {
            IMongoQuery query = null;
            string userName = User.Identity.Name.ToString();
            string transaction = "TOPUP";

            query = (query == null) ? Query.EQ("transaction_type", transaction) : Query.And(
                query,
                Query.EQ("transaction_type", transaction)
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

        public ActionResult TransFer(string id)
        {
            IMongoQuery query = null;
            string userName = User.Identity.Name.ToString();
            string transaction = "TRANSFER";

            query = (query == null) ? Query.EQ("transaction_type", transaction) : Query.And(
                query,
                Query.EQ("transaction_type", transaction)
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