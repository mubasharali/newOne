using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Inspinia_MVC5_SeedProject.Controllers;//remove this after
using Inspinia_MVC5_SeedProject.Models;
using Microsoft.AspNet.Identity.EntityFramework;

//remove this
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
//using Microsoft.Owin.Security.DataProtection;
namespace Inspinia_MVC5_SeedProject
{
    public partial class Startup
    {
        //internal static IDataProtectionProvider DataProtectionProvider { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            //DataProtectionProvider = app.GetDataProtectionProvider();
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

            app.UseFacebookAuthentication(
               appId: "159579124429445",
               appSecret: "8e2368cff3e546eb233bcd3a428e9ab1");

            //app.UseGoogleAuthentication();
        }
    }
    //public class IdentityConfig
    //{
    //    public void Configuration(IAppBuilder app)
    //    {
    //        app.CreatePerOwinContext(() => new MyDbContext());
    //        app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
    //        app.CreatePerOwinContext<RoleManager<AppRole>>((options, context) =>
    //            new RoleManager<AppRole>(
    //                new RoleStore<AppRole>(context.Get<MyDbContext>())));

    //        app.UseCookieAuthentication(new CookieAuthenticationOptions
    //        {
    //            AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
    //            LoginPath = new PathString("/Home/Login"),
    //        });
    //    }
    //}
}