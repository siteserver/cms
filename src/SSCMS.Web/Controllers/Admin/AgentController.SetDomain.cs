using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class AgentController
    {
        [HttpPost, Route(RouteSetDomain)]
        public async Task<ActionResult<BoolResult>> SetDomain([FromBody] SetDomainRequest request)
        {
            if (string.IsNullOrEmpty(request.SecurityKey))
            {
                return this.Error("系统参数不足");
            }
            if (_settingsManager.SecurityKey != request.SecurityKey)
            {
                return this.Error("SecurityKey不正确");
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var domain = request.SiteDomain;

            if (!string.IsNullOrEmpty(domain))
            {
                if (!domain.EndsWith("/"))
                {
                    domain = domain + "/";
                }
                site.IsSeparatedWeb = true;
                site.SeparatedWebUrl = domain;
                site.IsSeparatedApi = true;
                site.SeparatedApiUrl = request.HostDomain;
            }
            else
            {
                site.IsSeparatedWeb = false;
                site.SeparatedWebUrl = string.Empty;
                site.IsSeparatedAssets = false;
                site.SeparatedAssetsUrl = string.Empty;
                site.IsSeparatedApi = false;
                site.SeparatedApiUrl = string.Empty;
            }

            await _siteRepository.UpdateAsync(site);

            await _createManager.CreateByAllAsync(site.Id);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
