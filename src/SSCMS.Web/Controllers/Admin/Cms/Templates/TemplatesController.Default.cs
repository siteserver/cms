using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesController
    {
        [HttpPost, Route(RouteDefault)]
        public async Task<ActionResult<GetResult>> Default([FromBody] TemplateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.Templates))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var templateInfo = await _templateRepository.GetAsync(request.TemplateId);
            if (templateInfo != null && !templateInfo.DefaultTemplate)
            {
                await _templateRepository.SetDefaultAsync(request.TemplateId);
                await _authManager.AddSiteLogAsync(site.Id,
                    $"设置默认{templateInfo.TemplateType.GetDisplayName()}",
                    $"模板名称:{templateInfo.TemplateName}");
            }

            return await GetResultAsync(site);
        }
    }
}
