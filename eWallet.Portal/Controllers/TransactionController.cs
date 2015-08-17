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
        //
        // GET: /Transaction/
        public ActionResult List(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                id = "Personal";
            }
            dynamic[] list_profile = App_Start.eWalletConfig.BusinessDataHelper.List("transactions",null,SortBy.Descending("system_created_time"));
            ViewBag.Type = id;
            ViewBag.List = list_profile;
            return View();
        }
	}
}