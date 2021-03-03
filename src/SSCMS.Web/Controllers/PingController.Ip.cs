using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers
{
    public partial class PingController
    {
        [HttpGet, Route(RouteIp)]
        public async Task<string> Ip()
        {
            return await RestUtils.GetIpAddressAsync();
        }
    }
}