using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eWallet.Portal.App_Start
{
    public static class eWalletConfig
    {
        public static Data.MongoHelper BusinessDataHelper, CoreDataHelper;
        public static void Config()
        {
            BusinessDataHelper = new Data.MongoHelper("mongodb://127.0.0.1:27017/ewallet_business", "ewallet_business");
            CoreDataHelper = new Data.MongoHelper("mongodb://127.0.0.1:27017/ewallet_core", "ewallet_core");
        }
    }
}