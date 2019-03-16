using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HlidacStatu.Web.Startup))]
namespace HlidacStatu.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
