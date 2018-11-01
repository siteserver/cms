using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace SiteServer.API
{
    //https://www.strathweb.com/2015/10/global-route-prefixes-with-attribute-routing-in-asp-net-web-api/
    public class CentralizedPrefixProvider : DefaultDirectRouteProvider
    {
        private readonly string _centralizedPrefix;

        public CentralizedPrefixProvider(string centralizedPrefix)
        {
            _centralizedPrefix = centralizedPrefix;
        }

        protected override string GetRoutePrefix(HttpControllerDescriptor controllerDescriptor)
        {
            var prefix = _centralizedPrefix;

            var dllName = controllerDescriptor.ControllerType.Assembly.GetName().Name.ToLower();
            if (dllName != "siteserver.api")
            {
                prefix += "/" + dllName;
            }

            var existingPrefix = base.GetRoutePrefix(controllerDescriptor);
            return existingPrefix == null ? prefix : $"{prefix}/{existingPrefix}";
        }
    }
}