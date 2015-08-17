using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace eWallet.Portal
{
    public class Helper
    {
        static eWallet.Data.MongoHelper data;
        public static void Init()
        {
            data = new Data.MongoHelper(
                ConfigurationManager.AppSettings["FrontEndDb_Server"],
                ConfigurationManager.AppSettings["FrontEndDb_Database"]
                );
        }
        private static CoreAPI.ChannelAPIClient client = new CoreAPI.ChannelAPIClient();
        public static string rpHash(string value)
        {
            int hash = 5381;
            value = value.ToUpper();
            for (int i = 0; i < value.Length; i++)
            {
                hash = ((hash << 5) + hash) + value[i];
            }
            return hash.ToString();
        }

        public static void SendRequestOTP(dynamic user_id)
        {
            string request = @"{system:'web_frontend', module:'security',type:'one_way',function:'gen_otp',request:{user_id:'" + user_id + "'}}";
            RequestToServer(request);
        }

        public static string RequestToServer(string request)
        {
            string response = @"{error_code:'96',error_message:'Có lỗi trong quá trình xử lý. Vui lòng thử lại sau'}";
            if (client.State != System.ServiceModel.CommunicationState.Opened)
            {
                try
                {
                    client.Abort();
                    client = new CoreAPI.ChannelAPIClient();
                    client.Open();
                }
                catch {
                    
                }
            }
            try
            {
                response = client.Process(request);
            }
            catch
            {
            }
            return response;
        }
    }
}