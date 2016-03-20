using System.Web.Http;
using System.Net.Http.Formatting;
class WebApiConfig
{
    public static void Register(HttpConfiguration configuration)
    {
       // configuration.MapHttpAttributeRoutes();

        configuration.Routes.MapHttpRoute(
            name: "WithActionApi",
            routeTemplate : "api/{controller}/{action}/{id}",
            defaults: new { id = RouteParameter.Optional}
        );

        configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        configuration.Formatters.Clear();
        configuration.Formatters.Add(new JsonMediaTypeFormatter());
        configuration.MapHttpAttributeRoutes();
        //var json = configuration.Formatters.JsonFormatter;
        //json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
        //configuration.Formatters.Remove(configuration.Formatters.XmlFormatter);
    }
}