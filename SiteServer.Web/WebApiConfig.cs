using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Dispatcher;
using System.Web.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using SiteServer.Utils;

namespace SiteServer.API
{
    // https://docs.microsoft.com/en-us/aspnet/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes(new CentralizedPrefixProvider(WebConfigUtils.ApiPrefix));

            config.Services.Replace(typeof(IHttpControllerSelector),
                new NamespaceHttpControllerSelector(config));

            var corsAttr = new EnableCorsAttribute("*", "*", "*")
            {
                SupportsCredentials = true
            };
            config.EnableCors(corsAttr);

            RouteTable.Routes.Ignore(""); //Allow index.html to load

            var jsonFormatter = config.Formatters.JsonFormatter;
            //var settings = new JsonSerializerSettings
            //{
            //    ContractResolver = new CamelCasePropertyNamesContractResolver()
            //};

            //var timeFormat = new IsoDateTimeConverter
            //{
            //    DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
            //};
            //settings.Converters.Add(timeFormat);
            //jsonFormatter.SerializerSettings = settings;
            jsonFormatter.SerializerSettings = TranslateUtils.JsonSettings;
            jsonFormatter.Indent = true;

            var formatters = config.Formatters.Where(formatter =>
                    formatter.SupportedMediaTypes.Any(media => media.MediaType == "application/xml"))
                .ToList();

            foreach (var match in formatters)
            {
                config.Formatters.Remove(match);
            }

            config.EnsureInitialized();
        }
    }
}