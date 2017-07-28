using Microsoft.Owin;
using Owin;
using SiteServer.API;

[assembly: OwinStartup(typeof(Startup))]

namespace SiteServer.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}