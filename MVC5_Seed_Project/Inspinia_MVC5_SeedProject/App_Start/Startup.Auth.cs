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
using System.Configuration;
using System.Threading.Tasks;
using System;
using Microsoft.Owin.Security.Facebook;
using System.Security.Claims;
using System.Net.Http;
using System.Net;
using Microsoft.Owin.Security.DataProtection;
//using Microsoft.Owin.Security.DataProtection;
namespace Inspinia_MVC5_SeedProject
{
    public partial class Startup
    {
        internal static IDataProtectionProvider DataProtectionProvider { get; private set; }

        public void ConfigureAuth(IAppBuilder app)
        {
            DataProtectionProvider = app.GetDataProtectionProvider();
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<AppUserManager, ApplicationUser>(
                    validateInterval: TimeSpan.FromSeconds(0),
                    regenerateIdentity: (manager, user) => manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie))
                }
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

            //app.UseFacebookAuthentication(
            //   appId: "159579124429445",
            //   appSecret: "8e2368cff3e546eb233bcd3a428e9ab1");

            const string XmlSchemaString = "http://www.w3.org/2001/XMLSchema#string";
            const string ignoreClaimPrefix = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims";
            if (ConfigurationManager.AppSettings.Get("FacebookAppId").Length > 0)
            {
                var facebookOptions = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationOptions()
                {
                    AppId = ConfigurationManager.AppSettings.Get("FacebookAppId"),
                    AppSecret = ConfigurationManager.AppSettings.Get("FacebookAppSecret"),
                //    BackchannelHttpHandler = new FacebookBackChannelHandler(),
                //UserInformationEndpoint = "https://graph.facebook.com/v2.4/me?fields=id,name,email,first_name,last_name,location",

                    //Provider = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationProvider()
                    //{
                    //    OnAuthenticated = (context) =>
                    //    {
                    //        context.Identity.AddClaim(new System.Security.Claims.Claim("urn:facebook:access_token", context.AccessToken, XmlSchemaString, "Facebook"));
                    //        context.Identity.AddClaim(new System.Security.Claims.Claim("urn:facebook:name", context.Name, XmlSchemaString, "Facebook"));
                    //     //   context.Identity.AddClaim(new System.Security.Claims.Claim("urn:facebook:email", context.Email, XmlSchemaString, "Facebook"));
                    //        //var claimType = string.Format("urn:facebook:{0}", claim.Key);
                    //        //try { 
                    //        //context.Identity.AddClaim(new System.Security.Claims.Claim("urn:facebook:email", context.Email, XmlSchemaString, "Facebook"));
                    //        //}
                    //        //catch (Exception e)
                    //        //{

                    //        //}
                    //        return Task.FromResult(0);
                    //    }
                    //}
                    Provider = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationProvider()
                    {
                        OnAuthenticated = async context =>
                            {
                                context.Identity.AddClaim(new Claim("urn:facebook:access_token", context.AccessToken));
                                foreach (var claim in context.User)
                                {
                                    var claimType = string.Format("urn:facebook:{0}", claim.Key);
                                    string claimValue = claim.Value.ToString();
                                    if (!context.Identity.HasClaim(claimType, claimValue))
                                        context.Identity.AddClaim(new System.Security.Claims.Claim(claimType, claimValue, XmlSchemaString, "Facebook"));
                                }
                            }
                    }
                    //Provider = new FacebookAuthenticationProvider
                    //{
                    //    OnAuthenticated = async ctx =>
                    //    {
                    //        ctx.Identity.AddClaim(new Claim(ClaimTypes.DateOfBirth, ctx.User["birthday"].ToString()));
                    //        ctx.Identity.AddClaim(new Claim(ClaimTypes.Country, ctx.User["birthday"].ToString()));
                    //        ctx.Identity.AddClaim(new Claim(ClaimTypes.Gender, ctx.User["birthday"].ToString()));
                    //        ctx.Identity.AddClaim(new Claim(ClaimTypes.MobilePhone, ctx.User["birthday"].ToString()));
                    //        ctx.Identity.AddClaim(new Claim(ClaimTypes.OtherPhone, ctx.User["birthday"].ToString()));
                    //        ctx.Identity.AddClaim(new Claim(ClaimTypes.HomePhone, ctx.User["birthday"].ToString()));
                    //        ctx.Identity.AddClaim(new Claim(ClaimTypes.StateOrProvince, ctx.User["birthday"].ToString()));
                    //        ctx.Identity.AddClaim(new Claim(ClaimTypes.Email, ctx.User["birthday"].ToString()));
                    //        ctx.Identity.AddClaim(new Claim(ClaimTypes.Country, ctx.User["birthday"].ToString()));
                    //        ctx.Identity.AddClaim(new Claim(ClaimTypes.Actor, ctx.User["birthday"].ToString()));
                    //        ctx.Identity.AddClaim(new Claim(ClaimTypes.DateOfBirth, ctx.User["birthday"].ToString()));
                    //    }
                    //}
                };
                facebookOptions.Scope.Add("user_birthday");
                //facebookOptions.Scope.Add("first_name");
                //facebookOptions.Scope.Add("last_name");
                
                facebookOptions.Scope.Add("user_location");
                facebookOptions.Scope.Add("email");
                app.UseFacebookAuthentication(facebookOptions);
            }
            
            //app.UseGoogleAuthentication();
        }
    }
    public class FacebookBackChannelHandler : HttpClientHandler
    {
        protected override async System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            // Replace the RequestUri so it's not malformed
            if (!request.RequestUri.AbsolutePath.Contains("/oauth"))
            {
                request.RequestUri = new Uri(request.RequestUri.AbsoluteUri.Replace("?access_token", "&access_token"));
            }

            return await base.SendAsync(request, cancellationToken);
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