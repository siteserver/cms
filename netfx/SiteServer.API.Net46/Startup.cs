using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Dispatcher;
using System.Web.Routing;
using Microsoft.Owin;
using Owin;
using SiteServer.API;
using SiteServer.BackgroundPages.Common;
using SiteServer.CMS.Plugin;
using SiteServer.Utils;

[assembly: OwinStartup(typeof(Startup))]

namespace SiteServer.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //app.Use(async (context, next) =>
            //{
            //    var guid = StringUtils.GetGuid();
            //    var authenticatedRequest = new Request(HttpContext.Current.Request);

            //    CacheUtils.Insert(guid, authenticatedRequest);

            //    //UtilsApi.Instance.SetRequest(authenticatedRequest);
            //    context.Response.Headers.Add("X-Powered-By", new[] { "SiteServer CMS" });
            //    context.Response.Headers.Add("X-Session-Guid", new[] { guid });
            //    //return next();


            //    await next.Invoke();

            //    CacheUtils.Remove(guid);
            //});

            var applicationPath = HostingEnvironment.ApplicationVirtualPath;
            var applicationPhysicalPath = HostingEnvironment.ApplicationPhysicalPath;
            AppSettings.Load(applicationPath, applicationPhysicalPath, PathUtils.Combine(applicationPhysicalPath, AppSettings.WebConfigFileName));
            PluginManager.Load(() => Request.Current, () => Response.Current);

            //GlobalConfiguration.Configure(WebApiConfig.Register);

            var config = GlobalConfiguration.Configuration;

            config.MapHttpAttributeRoutes(new CentralizedPrefixProvider(AppSettings.ApiPrefix));

            config.Services.Replace(typeof(IHttpControllerSelector),
                new NamespaceHttpControllerSelector(config));

            var corsAttr = new EnableCorsAttribute("*", "*", "*")
            {
                SupportsCredentials = true
            };
            config.EnableCors(corsAttr);

            RouteTable.Routes.Ignore(""); //Allow index.html to load

            var jsonFormatter = config.Formatters.JsonFormatter;
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