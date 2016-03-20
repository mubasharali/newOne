using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Text;
namespace Inspinia_MVC5_SeedProject
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            //routes.Add(new SubdomainRoute());
            //routes.MapRoute(
            //    name: "default1",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });

            //routes.MapRoute(
            //    name: "some",
            //    url: "ads/{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });

            //routes.MapRoute(
            //    name: "custom",
            //    url: "{controller}/{category}/{subcategory}/{lowcategory}/{id}/{title}",
            //    defaults: new { controller = "Home", action = "Index", category = UrlParameter.Optional, subcategory = UrlParameter.Optional, lowcategory = UrlParameter.Optional, id = UrlParameter.Optional, title = "" }
            //    );


            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

            //below are route suggested by owl88
            //routes.MapRoute(
            //    name: "ID",
            //    url: "{category}/{subcategory}/{lowercategory}/{lowercategory1}/{id}/{ignore}",
            //    defaults: new { controller = "Home", action = "Index", ignore = UrlParameter.Optional }
            //);

            //routes.MapRoute(
            //    name: "LowerCategory1",
            //    url: "{category}/{subcategory}/{lowercategory}/{lowercategory1}",
            //    defaults: new { controller = "Home", action = "Index" }
            //);

            //routes.MapRoute(
            //    name: "LowerCategory",
            //    url: "{category}/{subcategory}/{lowercategory}",
            //    defaults: new { controller = "Home", action = "Index" }
            //);

            //routes.MapRoute(
            //    name: "Subcategory",
            //    url: "{category}/{subcategory}",
            //    defaults: new { controller = "Home", action = "Index" }
            //);

            //routes.MapRoute(
            //    name: "Category",
            //    url: "{category}",
            //    defaults: new { controller = "Home", action = "Index" }
            //);

            //routes.MapRoute(
            //    name: "ForIgnoreTitle",
            //    url: "{controller}/{action}/{id}/{title}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional, title = UrlParameter.Optional }
            //);
            //routes.MapRoute(
            //    name: "ForIgnoreTitle",
            //    url: "{controller}/{action}/{id}/{title}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional, title = UrlParameter.Optional }
            //);
            routes.MapRoute("Robots.txt",
                "robots.txt",
                new { controller = "Home", action = "Robots" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
    
    //public class SubdomainRoute : RouteBase
    //{
    //    public override RouteData GetRouteData(HttpContextBase httpContext)
    //    {
    //        var host = httpContext.Request.Url.Host;
    //        var index = host.IndexOf(".");
    //        string[] segments = httpContext.Request.Url.PathAndQuery.Split('/');
    //        if (index < 0)
    //            return null;
    //        var subdomain = host.Substring(0, index);
    //        string controller = (segments.Length > 0) ? segments[0] : "Home";
    //        string action = (segments.Length > 1) ? segments[1] : "Index";
    //        var routeData = new RouteData(this, new MvcRouteHandler());
    //        routeData.Values.Add("controller", controller); //Goes to the relevant Controller  class
    //        routeData.Values.Add("action", action); //Goes to the relevant action method on the specified Controller
    //        routeData.Values.Add("subdomain", subdomain); //pass subdomain as argument to action method
    //        return routeData;
    //    }
    //    public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
    //    {
    //        //Implement your formating Url formating here
    //        return null;
    //    }
    //}
}
