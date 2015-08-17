using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eWallet.Portal.Controllers
{
    public class ProxyController : Controller
    {
        public static string[] banks_list = new string[] { 
            "Ngân hàng Ngoại Thương Việt Nam - Vietcombank",
            "Ngân hàng Kỹ thương Việt Nam - Techcombank",
            "Ngân hàng Quân đội - MBBank",
            "Ngân hàng Quốc tế - VIB",
            "Ngân hàng Công thương - ViettinBank",
            "Ngân hàng Hàng hải - Maritimebank",
            "Ngân hàng Việt Nam Thịnh vượng - VPBank",
            "Ngân hàng Nam Á - NamAbank",
            "Ngân hàng Sài Gon - Saigonbank",
            "Ngân hàng Xăng dầu Petrolimex - PGBank",
            "Ngân hàng Phát triển Nông thôn - Agribank",
            "Ngân hàng Việt Á - VietAbank",
            "Ngân hàng Đại dương - Oceanbank",
            "Ngân hàng An Bình - ABBank",
            "Ngân hàng Tiên Phong - TPBank",
            "Ngân hàng Đầu tư và phát triển Việt Nam - BIDV",
            "Ngân hàng SHB - SHBank",
            "Ngân hàng Đông Nam Á - Seabank",
            "Ngân hàng Bắc Á - BACA"
        };
        public static string[] banks_code = new string[]{
            "970436",
            "970407",
            "970422",
            "970441",
            "970489",
            "970441",
            "970432",
            "970428",
            "161087",
            "970430",
            "970499",
            "970427",
            "970414",
            "970459",
            "970423",
            "970488",
            "970443",
            "970468",
            "970409"
        };
        //
        // GET: /Proxy/
        public JsonResult ListBank(string q)
        {
            q = q.ToLower();
            string[] _values = new string[] { };
            _values = (from e in banks_list where e.ToLower().Contains(q) select e).ToArray();
            return Json(_values, JsonRequestBehavior.AllowGet);
        }

        public static string GetBankCode(string BankName)
        {
            int idx = -1;
            for (int i = 0; i < banks_list.Length; i++)
                if (banks_list[i].Equals(BankName))
                {
                    idx = i;
                    break;
                }
            if(idx != -1)
                return banks_code[idx];
            return String.Empty;
        }
    }
}