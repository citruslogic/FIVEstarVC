using System;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;


namespace FIVESTARVC
{
    public static class FIVESTARAuthentication
    {
        public const String ApplicationCookie = "FIVESTARAuthenticationType";
    }

    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // need to add UserManager into owin, because this is used in cookie invalidation
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = FIVESTARAuthentication.ApplicationCookie,
                LoginPath = new PathString("/Login"),
                Provider = new CookieAuthenticationProvider(),
                CookieName = "FIVESTARVCookie",
                CookieHttpOnly = true,
                ExpireTimeSpan = TimeSpan.FromHours(8), // adjust to your needs
            });
        }
    }
}