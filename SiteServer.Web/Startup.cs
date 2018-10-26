using System.Web.Hosting;
using System.Web.Http;
using Microsoft.Owin;
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
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}