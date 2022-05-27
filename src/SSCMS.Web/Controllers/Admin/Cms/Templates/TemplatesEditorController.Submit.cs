using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;
using SSCMS.Utils;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesEditorController
	{
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.Templates))
            {
                return Unauthorized();
            }

            if (_settingsManager.IsSafeMode)
            {
                return this.Error(Constants.ErrorSafeMode);
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var template = await _templateRepository.GetAsync(request.TemplateId);

            await _pathManager.WriteContentToTemplateFileAsync(site, template, request.Content, _authManager.AdminId);
            await CreatePagesAsync(template);

            await _authManager.AddSiteLogAsync(site.Id,
                $"修改{template.TemplateType.GetDisplayName()}",
                $"模板名称:{template.TemplateName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
