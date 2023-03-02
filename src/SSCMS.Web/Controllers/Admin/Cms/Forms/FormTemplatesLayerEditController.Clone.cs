using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Forms
{
    public partial class FormTemplatesLayerEditController
    {
        [HttpPost, Route(RouteClone)]
        public async Task<ActionResult<BoolResult>> Clone([FromBody] CloneRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.FormTemplates))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var templates = await _formManager.GetFormTemplatesAsync(site);
            var originalTemplate = templates.First(x => StringUtils.EqualsIgnoreCase(request.NameOriginal, x.Name));

            if (templates.Any(x => StringUtils.EqualsIgnoreCase(request.Name, x.Name)))
            {
                return this.Error($"标识为 {request.Name} 的模板已存在，请更换模板标识！");
            }

            if (request.IsHtml)
            {
                await _formManager.CloneAsync(site, request.IsSystemOriginal, request.NameOriginal, request.Name, request.TemplateHtml);
            }
            else
            {
                if (request.IsSystemOriginal && site.IsSeparatedApi && !string.IsNullOrEmpty(site.SeparatedApiUrl))
                {
                    var html = await _formManager.GetTemplateHtmlAsync(site, request.IsSystemOriginal, request.NameOriginal);
                    var prefix = PageUtils.Combine(site.SeparatedApiUrl, "/sitefiles/");
                    html = html.Replace("/sitefiles/", prefix);
                    await _formManager.CloneAsync(site, request.IsSystemOriginal, request.NameOriginal, request.Name, html);
                }
                else
                {
                    await _formManager.CloneAsync(site, request.IsSystemOriginal, request.NameOriginal, request.Name);
                }
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
