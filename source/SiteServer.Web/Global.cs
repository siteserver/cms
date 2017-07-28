using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace SiteServer.API
{
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            var configuration = GlobalConfiguration.Configuration;
            
            var corsAttr = new EnableCorsAttribute("*", "*", "*")
            {
                SupportsCredentials = true
            };
            configuration.EnableCors(corsAttr);
            configuration.MapHttpAttributeRoutes();

            configuration.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional }
            );

            var jsonFormatter = configuration.Formatters.JsonFormatter;
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var timeFormat = new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
            };
            settings.Converters.Add(timeFormat);
            jsonFormatter.SerializerSettings = settings;
            jsonFormatter.Indent = true;

            configuration.EnsureInitialized();
        }
    }
}
