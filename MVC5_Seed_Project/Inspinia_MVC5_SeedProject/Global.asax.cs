using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;
using System.Data.Entity;//remove this
using System.Web.Security;

using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Threading;
using Inspinia_MVC5_SeedProject.Models;
using WebMatrix.WebData;

namespace Inspinia_MVC5_SeedProject
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


           // LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
            var configuration = GlobalConfiguration.Configuration;
            configuration.Formatters.Remove(configuration.Formatters.XmlFormatter);
            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            //GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
        }
        protected void Application_BeginRequest()

        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }
        //private static SimpleMembershipInitializer _initializer;
        //private static object _initializerLock = new object();
        //private static bool _isInitialized;
        //private class SimpleMembershipInitializer
        //{
        //    public SimpleMembershipInitializer()
        //    {
        //       // Database.SetInitializer<UsersContext>(null);

        //        try
        //        {
                    

        //            WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
        //        }
        //    }
        //}
    }
}
