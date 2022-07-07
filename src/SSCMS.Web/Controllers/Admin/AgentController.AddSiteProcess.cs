using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class AgentController
    {
        [HttpPost, Route(RouteAddSiteProcess)]
        public ActionResult<CacheUtils.Process> AddSiteProcess([FromBody] AddSiteProcessRequest request)
        {
            if (string.IsNullOrEmpty(request.SecurityKey))
            {
                return this.Error("系统参数不足");
            }
            if (_settingsManager.SecurityKey != request.SecurityKey)
            {
                return this.Error("SecurityKey不正确");
            }

            var caching = new CacheUtils(_cacheManager);
            return caching.GetProcess(request.Guid);
        }
    }
}