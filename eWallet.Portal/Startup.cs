using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(eWallet.Portal.Startup))]
namespace eWallet.Portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
