using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FIVEstarVC.Startup))]
namespace FIVEstarVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        } 
    }
}