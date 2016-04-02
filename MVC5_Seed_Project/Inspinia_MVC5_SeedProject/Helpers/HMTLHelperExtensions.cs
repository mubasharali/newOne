using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Routing;
namespace Inspinia_MVC5_SeedProject
{
    public static class UrlHelperExtensions
    {
        public static string Action(this UrlHelper helper, string actionName, string controllerName, object routeValues, string protocol, bool defaultPort)
        {
            return Action(helper, actionName, controllerName, routeValues, protocol, null, defaultPort);
        }

        public static string Action(this UrlHelper helper, string actionName, string controllerName, object routeValues, string protocol, string hostName, bool defaultPort)
        {
            if (!defaultPort)
            {
                return helper.Action(actionName, controllerName, new RouteValueDictionary(routeValues), protocol, hostName);
            }

            string port = "80";
            if (protocol.Equals("https", StringComparison.OrdinalIgnoreCase))
            {
                port = "443";
            }

            Uri requestUrl = helper.RequestContext.HttpContext.Request.Url;
            string defaultPortRequestUrl = Regex.Replace(requestUrl.ToString(), @"(?<=:)\d+?(?=/)", port);
            Uri url = new Uri(new Uri(defaultPortRequestUrl, UriKind.Absolute), requestUrl.PathAndQuery);

            var requestContext = GetRequestContext(url);
            var urlHelper = new UrlHelper(requestContext, helper.RouteCollection);

            var values = new RouteValueDictionary(routeValues);
            values.Add("controller", controllerName);
            values.Add("action", actionName);

            return urlHelper.RouteUrl(null, values, protocol, hostName);
        }

        private static RequestContext GetRequestContext(Uri uri)
        {
            // Create a TextWriter with null stream as a backing stream 
            // which doesn't consume resources
            using (var writer = new StreamWriter(Stream.Null))
            {
                var request = new HttpRequest(
                    filename: string.Empty,
                    url: uri.ToString(),
                    queryString: string.IsNullOrEmpty(uri.Query) ? string.Empty : uri.Query.Substring(1));
                var response = new HttpResponse(writer);
                var httpContext = new HttpContext(request, response);
                var httpContextBase = new HttpContextWrapper(httpContext);
                return new RequestContext(httpContextBase, new RouteData());
            }
        }
    }
    public static class HMTLHelperExtensions
    {
        public static string IsSelected(this HtmlHelper html, string controller = null, string action = null)
        {
            string cssClass = "active";
            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (String.IsNullOrEmpty(controller))
                controller = currentController;

            if (String.IsNullOrEmpty(action))
                action = currentAction;

            return controller == currentController && action == currentAction ?
                cssClass : String.Empty;
        }
    }
}
