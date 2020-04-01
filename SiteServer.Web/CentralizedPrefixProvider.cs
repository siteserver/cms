using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using SiteServer.Utils;

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
            var dllName = controllerDescriptor.ControllerType.Assembly.GetName().Name;

            var prefix = !StringUtils.EqualsIgnoreCase(dllName, "SiteServer.API")
                ? PageUtils.Combine(_centralizedPrefix, dllName)
                : _centralizedPrefix;

            var existingPrefix = StringUtils.TrimSlash(base.GetRoutePrefix(controllerDescriptor));
            return PageUtils.Combine(prefix, existingPrefix).TrimEnd('/');
        }
    }
}