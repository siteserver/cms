using System.Linq;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Routing;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Owin;
using SiteServer.API;
using SiteServer.CMS.Plugin;
using SiteServer.Utils;

[assembly: OwinStartup(typeof(Startup))]

namespace SiteServer.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();

            var config = GlobalConfiguration.Configuration;

            config.MapHttpAttributeRoutes();

            var corsAttr = new EnableCorsAttribute("*", "*", "*")
            {
                SupportsCredentials = true
            };
            config.EnableCors(corsAttr);

            //config.Routes.MapHttpRoute(
            //    "DefaultApi",
            //    "api/{controller}/{id}",
            //    new { id = RouteParameter.Optional }
            //);

            //config.Routes.Add("name", new HttpRoute());

            RouteTable.Routes.Ignore(""); //Allow index.html to load

            var jsonFormatter = config.Formatters.JsonFormatter;
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

            var formatters = config.Formatters.Where(formatter =>
                    formatter.SupportedMediaTypes.Any(media => media.MediaType == "application/xml"))
                .ToList();

            foreach (var match in formatters)
            {
                config.Formatters.Remove(match);
            }

            config.EnsureInitialized();

            WebConfigUtils.Load(HostingEnvironment.ApplicationPhysicalPath);
            var c = PluginManager.PluginInfoListRunnable;
        }
    }
}