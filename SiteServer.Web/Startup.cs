using System.Web.Hosting;
using System.Web.Http;
using Microsoft.Owin;
using Namotion.Reflection;
using NSwag.AspNet.Owin;
using Owin;
using SiteServer.API;
using SiteServer.CMS.Plugin;

[assembly: OwinStartup(typeof(Startup))]

namespace SiteServer.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            PluginManager.LoadPlugins(HostingEnvironment.ApplicationPhysicalPath);

            app.UseSwaggerReDoc(typeof(Startup).Assembly, settings =>
            {
                settings.Path = "/api/docs";
                settings.DocumentPath = "/api/swagger/v1/swagger.json";
                settings.GeneratorSettings.DefaultUrlTemplate = "api/{controller}/{id}";
                settings.GeneratorSettings.Title = "SiteServer CMS API";
                settings.GeneratorSettings.Description = "SiteServer CMS RESTful API Document";
            });

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}