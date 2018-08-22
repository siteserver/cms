using System.Web.Hosting;
using System.Web.Http;
using Microsoft.Owin;
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

            GlobalConfiguration.Configure(WebApiConfig.Register);

            PluginManager.LoadPlugins(HostingEnvironment.ApplicationPhysicalPath);
        }
    }
}