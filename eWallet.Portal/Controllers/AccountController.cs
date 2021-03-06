﻿using eWallet.Portal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using MongoDB.AspNet.Identity;
using MongoDB.Driver.Builders;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Facebook;
using Newtonsoft.Json;
using Microsoft.Owin.Security.Facebook;
using System.Configuration;
using Microsoft.Owin.Host.SystemWeb;
using System.Net;
using System.Dynamic;
using System.Text.RegularExpressions;
using MongoDB.Driver;
using System.Text;

namespace eWallet.Portal.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/
        [Authorize]
        public ActionResult List(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                id = "Personal";
            }
            dynamic[] list_profile = App_Start.eWalletConfig.BusinessDataHelper.List("finance_account", Query.NE("type", "P"), SortBy.Ascending("profile"));
            ViewBag.Type = id;
            ViewBag.List = list_profile;
            return View();
        }

        //get friend facebook
        public ActionResult FacebookFriend()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var facebookClaim = identity.Claims.FirstOrDefault(c => c.Type == "FacebookAccessToken");
            ViewBag.data = null;
            if (facebookClaim != null)
            {
                var client = new FacebookClient(facebookClaim.Value);
                dynamic friendlist = client.Get("me/taggable_friends?fields=name");
                var data = friendlist["data"].ToString();
                ViewBag.data = friendlist.data;
            }
            return View();
        }
        [Authorize]
        public ActionResult Personal()
        {
            return View();
        }
        public ActionResult Business()
        {
            return View();
        }
        [Authorize]
        public ActionResult Me()
        {
            dynamic profile = Helper.DataHelper.Get("profile", Query.EQ("user_name", User.Identity.Name));
            dynamic[] accounts = Helper.DataHelper.List("finance_account", Query.EQ("profile", profile._id));
            dynamic model = new Data.DynamicObj();
            model.Profile = profile;
            model.Accounts = accounts;

            ViewBag.accounts = null;//Helper.DataHelper.List("finance_account",Query.EQ("profile_user_id result.response;
            return View(model);
        }
        [Authorize]
        public ActionResult Profile()
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.profile = Helper.DataHelper.Get("profile", Query.EQ("user_name", User.Identity.Name));
            return View();
        }

        public ActionResult CashIn()
        {
            return View();
        }
        [Authorize]
        public ActionResult CashOut()
        {
            return View();
        }
        #region "Boxes"
        public ActionResult CashInATM()
        {
            return View(Url.Content("/Views/Box/CashIn_ATM.cshtml"));
        }

        public ActionResult CashInBankAccount()
        {
            return View(Url.Content("/Views/Box/CashIn_BankAccount.cshtml"));
        }

        public ActionResult CashInOthers()
        {
            return View(Url.Content("/Views/Box/CashIn_Others.cshtml"));
        }

        public ActionResult CashOutBank()
        {
            ViewBag.accounts = Helper.DataHelper.List("cashout_bank_account",
               Query.EQ("profile", User.Identity.Name));
            return View(Url.Content("/Views/Box/CashOut_Bank.cshtml"));
        }
        public ActionResult CashOutBankAccounts()
        {
            //Lay danh sach cac tai khoan Cashout da khai bao
            ViewBag.accounts = Helper.DataHelper.List("cashout_bank_account",
                Query.EQ("profile", User.Identity.Name));
            return View(Url.Content("/Views/Box/CashOut_BankAccounts.cshtml"));
        }

        public ActionResult BusinessProfile()
        {
            return View(Url.Content("/Views/Box/Business_Profile.cshtml"));
        }
        #endregion

        #region "Process to server"

        public JsonResult PostRegister(string full_name, string email, string mobile)
        {
            string request = @"{system:'web_frontend', module:'profile', function:'register', type:'two_way', request:{full_name:'" + full_name + "', id:'" + email + "', mobile:'" + mobile + "'}}";
            dynamic response = JObject.Parse(Helper.RequestToServer(request));
            return Json(new { error_code = response.error_code.ToString(), error_message = response.error_message.ToString() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PostLogin(string email, string password, bool is_remember)
        {
            string _srequest = @"{system:'web_frontend', module:'security',type:'two_way', function:'login',request:{id:'" + email + "', password:'" + password + "'}}";
            dynamic result = new eWallet.Data.DynamicObj(Helper.RequestToServer(_srequest));
            if (result.error_code.ToString() == "00")
            {
                FormsAuthentication.SetAuthCookie(email, false);
                Session["user_profile"] = result.response;
                //if (!String.IsNullOrEmpty(ReturnUrl)) return Redirect(ReturnUrl);
                //return RedirectToAction("Index", "Home");
            }
            else
            {
                //ModelState.AddModelError("login_error", result.error_message.ToString());
                //return View();
            }
            return Json(new { error_code = result.error_code.ToString(), error_message = result.error_message.ToString() }, JsonRequestBehavior.AllowGet);
        }
        #endregion "Process to server"
        #region "Process to database"

        public JsonResult CashOut_RemoveBankAccount(string id)
        {
            Helper.DataHelper.Delete("cashout_bank_account", id);
            return Json(new { error_code = "00", error_message = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CashOut_SaveBankAccount(string id, string code, string bank, string branch, string number, string name)
        {
            dynamic account = new Data.DynamicObj();
            if (String.IsNullOrEmpty(id)) id = Guid.NewGuid().ToString();
            account._id = id;
            account.code = code;
            account.bank = bank;
            account.branch = branch;
            account.number = number;
            account.name = name;
            account.profile = User.Identity.Name;
            Helper.DataHelper.Save("cashout_bank_account", account);

            return Json(new { error_code = "00", error_message = "Cập nhật thành công" }, JsonRequestBehavior.AllowGet);
        }
        #endregion "Process to database"
        public AccountController()
        //: this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
            this.UserManager = new UserManager<ApplicationUser>
                (
                new UserStore<ApplicationUser>("CoreDB_Server")
                );
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                        if (user.Roles[0] == "CUSTOMER")
                        {
                            if (user.Status == "LOCKED")
                            {
                                ModelState.AddModelError("", "Tài khoản của bạn đang bị tạm khoá liên hệ với Admin hệ thống để mở khoá !");
                            }
                            else
                            {
                                await SignInAsync(user, model.RememberMe);
                                return RedirectToLocal(returnUrl);
                            }
                        }
                        else
                            ModelState.AddModelError("", "Bạn chỉ có thể đăng nhập bằng tài khoản người dùng !");                  
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không chính xác !");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (IsValidEmail(model.Email) == true)
                {
                    if(CheckIphone(model.Mobile)==true)
                    {
                        if (CheckPhoneSupport(model.Mobile) == true)
                        {                         
                            var user = new ApplicationUser() { UserName = model.Email};
                            var result = await UserManager.CreateAsync(user, model.Password);
                            if (result.Succeeded)
                            {
                                //add Roles to User
                                string[] roles = new string[] { "CUSTOMER" };
                                var roleResult = await UserManager.AddToRoleAsync(user.Id, roles[0]);
                                //Goi ham dang ky tren server de tao finance_account
                                PostRegister(model.Fullname, model.Email, model.Mobile);
                                await SignInAsync(user, isPersistent: false);
                                string alert = "Chuc mung ban dang ky thanh cong tai khoan BigPay !";                               
                                TempData["alertMessage"] = alert;
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                AddErrors(result);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Số điện thoại không chính xác !");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Số điện thoại đã tồn tại!");
                    }
                }
                else
                {
                    ModelState.AddModelError("", " Email không chính xác !");
                }

            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public static bool CheckIphone(string iphone)
        {
            string a = iphone.Insert(0, "84");
            a = a.Remove(2, 1);
            dynamic profile = Helper.DataHelper.Get("profile", Query.EQ("mobile", a));
            if (profile != null)
            {
                return false;
            }
            return true;
        }
        public static bool CheckPhoneSupport(string phone_number)
        {
            const int RegionConuntryCode = 84;

            if (phone_number.Length == 10)
            {
                if (phone_number.StartsWith("+"))
                {
                    phone_number = phone_number.Replace("+", "0");
                }

                if (phone_number.StartsWith("0" + RegionConuntryCode))
                {
                    phone_number = phone_number.Replace("0" + RegionConuntryCode, "0");
                }
                    string[] networkSupport_2 = { "096", "097", "098", "090", "093", "091", "094", "092", "099" };
                    const int networkLength = 3;
                    var startphone_number = phone_number.Substring(0, networkLength);
                    return networkSupport_2.Any(startphone_number.Equals);

            }
            if (phone_number.Length == 11)
            {
                if (phone_number.StartsWith("+"))
                {
                    phone_number = phone_number.Replace("+", "0");
                }

                if (phone_number.StartsWith("0" + RegionConuntryCode))
                {
                    phone_number = phone_number.Replace("0" + RegionConuntryCode, "0");
                }
               
                    string[] networkSupport_1 = {"0162", "0163", "0164", "0165", "0166", "0167", "0168", "0169",
            "0120", "0121", "0122","0126","0128",
            "0123","0124","0125","0127","0129",
            "0188", "0186",
            "0199"};
                    const int networkLength_2 = 4;
                    var startphone_number_2 = phone_number.Substring(0, networkLength_2);
                    return networkSupport_1.Any(startphone_number_2.Equals);  
            }
            return false;
        }

        static Regex ValidEmailRegex = CreateValidEmailRegex();
        private static Regex CreateValidEmailRegex()
        {
            string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            return new Regex(validEmailPattern, RegexOptions.IgnoreCase);
        }

        internal bool IsValidEmail(string email)
        {
            bool isValid = ValidEmailRegex.IsMatch(email);
            return isValid;
        }
        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        //return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                        TempData["alertMessage"] = "Mat khau thay doi thanh cong !";
                        return RedirectToAction("Manage");
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }
            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            var claimsIdentity = await AuthenticationManager.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);
            if (user != null)
            {

                //await StoreFacebookAuthToken(user);
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Name = loginInfo.DefaultUserName });
            }
        }

        //private async Task StoreFacebookAuthToken(ApplicationUser user)
        //{
        //    var claimsIdentity = await AuthenticationManager.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);
        //    if(claimsIdentity!=null)
        //    {
        //        var currentClaims = await UserManager.GetClaimsAsync(user.Id);
        //        var facebookAccessToken = claimsIdentity.FindAll("FacebookAccessToken").First();
        //        if(currentClaims.Count()<=0)
        //        {
        //            await UserManager.AddClaimAsync(user.Id, facebookAccessToken);
        //        }
        //        else
        //        {
        //            await UserManager.RemoveClaimAsync(user.Id, currentClaims[0]);
        //            await UserManager.AddClaimAsync(user.Id, facebookAccessToken);
        //            //Session["FacebookAccessToken"] = facebookAccessToken;
        //        }
        //    }
        //}
        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                if (CheckIphone(model.Mobile) == true)
                {
                    if (CheckPhoneSupport(model.Mobile) == true)
                    {
                        var user = new ApplicationUser() { UserName = model.UserName };
                        var result = await UserManager.CreateAsync(user);
                        if (result.Succeeded)
                        {
                            result = await UserManager.AddLoginAsync(user.Id, info.Login);
                            if (result.Succeeded)
                            {
                                var claimsIdentity = await AuthenticationManager.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);
                                foreach (var claim in claimsIdentity.Claims)
                                    if (claim.Type == "FacebookAccessToken")
                                    {
                                        UserManager.AddClaim(user.Id, claim);
                                    }
                                //Goi sang server de tao tai khoan
                                PostRegister(model.Name, model.UserName, model.Mobile);
                                await SignInAsync(user, isPersistent: false);
                                return RedirectToLocal(returnUrl);
                            }
                        }
                        AddErrors(result);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Số điện thoại không chính xác !");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Số điện thoại đã tồn tại!");
                }
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }
       
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}