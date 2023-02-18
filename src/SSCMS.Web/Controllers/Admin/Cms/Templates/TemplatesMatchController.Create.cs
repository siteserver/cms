using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Configuration;
using SSCMS.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesMatchController
    {
        [HttpPost, Route(RouteCreate)]
        public async Task<ActionResult<BoolResult>> Create([FromBody] CreateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.TemplatesMatch))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            if (request.IsChannelTemplate && request.IsChildren)
            {
                await CreateChannelChildrenTemplateAsync(site, request);
            }
            else if (request.IsChannelTemplate && !request.IsChildren)
            {
                await CreateChannelTemplateAsync(site, request);
            }
            else if (!request.IsChannelTemplate && request.IsChildren)
            {
                await CreateContentChildrenTemplateAsync(site, request);
            }
            else if (!request.IsChannelTemplate && !request.IsChildren)
            {
                await CreateContentTemplateAsync(site, request);
            }

            await _authManager.AddSiteLogAsync(request.SiteId, "生成并匹配栏目模版");

            return new BoolResult
            {
                Value = true,
            };
        }
    }
}
