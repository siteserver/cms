using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class AgentController
    {
        [HttpPost, Route(RouteSetDomains)]
        public async Task<ActionResult<BoolResult>> SetDomains([FromBody] SetDomainsRequest request)
        {
            if (string.IsNullOrEmpty(request.SecurityKey))
            {
                return this.Error("系统参数不足");
            }
            if (_settingsManager.SecurityKey != request.SecurityKey)
            {
                return this.Error("SecurityKey不正确");
            }

            foreach (var siteDomain in request.SiteDomains)
            {
                var domain = siteDomain.Domain;
                if (!string.IsNullOrEmpty(domain) && !domain.EndsWith("/"))
                {
                    domain = domain + "/";
                }

                var site = await _siteRepository.GetAsync(siteDomain.SiteId);

                site.IsSeparatedWeb = true;
                site.SeparatedWebUrl = domain;

                site.IsSeparatedApi = true;
                site.SeparatedApiUrl = request.HostDomain;

                await _siteRepository.UpdateAsync(site);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
