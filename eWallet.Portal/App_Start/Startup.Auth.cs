using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Owin;

namespace eWallet.Portal
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            var options = new FacebookAuthenticationOptions
            {
                AppId = "207429956074584",
                AppSecret = "bce03ed2eac12f52cba2e20b5e2b20a0",
                Provider = new FacebookAuthenticationProvider
                {
                    OnAuthenticated = async context =>
                    {
                        string accessToken = context.AccessToken;
                        string facebookUserName = context.UserName;
                        string facebookName = context.Name;
                        var serializedUser = context.User;
                    }
                }
            };

            app.UseFacebookAuthentication(options);

            Microsoft.Owin.Security.Google.GoogleOAuth2AuthenticationOptions option = new Microsoft.Owin.Security.Google.GoogleOAuth2AuthenticationOptions();
            option.CallbackPath = new PathString("/signin-google");
            option.ClientId = "151452439229-0jk3ndrakujq0kho3v5ctcd070ktia17.apps.googleusercontent.com";
            option.ClientSecret = "aNQFeT1BfjWeSmz3t2Z__gND";

            app.UseGoogleAuthentication(option);
        }
    }
}