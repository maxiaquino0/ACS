using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ACS.Startup))]
namespace ACS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
