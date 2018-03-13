using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FIVESTARVC.Startup))]
namespace FIVESTARVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}