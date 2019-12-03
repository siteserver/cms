using System.Linq;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Dispatcher;
using System.Web.Routing;
using Microsoft.Owin;
using NSwag.AspNet.Owin;
using NSwag.AspNet.WebApi;
using Owin;
using SiteServer.API;
using SiteServer.CMS.Core;
using SiteServer.Abstractions;

[assembly: OwinStartup(typeof(Startup))]

namespace SiteServer.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            SystemManager.LoadSettingsAsync(HostingEnvironment.ApplicationPhysicalPath).GetAwaiter().GetResult();

            GlobalConfiguration.Configure(Register);

            app.UseSwaggerUi3(typeof(Startup).Assembly, settings =>
            {
                settings.Path = "/api/swagger";
                settings.DocumentPath = "/api/swagger/v1/swagger.json";
                settings.GeneratorSettings.DefaultUrlTemplate = "api/{controller}/{id}";
                settings.GeneratorSettings.Title = "SiteServer CMS API";
                settings.GeneratorSettings.Description = "SiteServer CMS RESTful API Document";
            });

            app.UseSwaggerReDoc(typeof(Startup).Assembly, settings =>
            {
                settings.Path = "/api/docs";
                settings.DocumentPath = "/api/swagger/v1/swagger.json";
                settings.GeneratorSettings.DefaultUrlTemplate = "api/{controller}/{id}";
                settings.GeneratorSettings.Title = "SiteServer CMS API";
                settings.GeneratorSettings.Description = "SiteServer CMS RESTful API Document";
            });
        }

        // https://docs.microsoft.com/en-us/aspnet/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2
        private static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes(new CentralizedPrefixProvider("api"));

            config.Filters.Add(new JsonExceptionFilterAttribute());

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