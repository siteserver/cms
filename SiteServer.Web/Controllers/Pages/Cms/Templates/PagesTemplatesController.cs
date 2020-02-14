using System.Threading.Tasks;
using System.Web.Http;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Templates
{
    [RoutePrefix("pages/cms/templates/templates")]
    public partial class PagesTemplatesController : ApiController
    {
        private const string Route = "";
        private const string RouteCreate = "actions/create";
        private const string RouteCopy = "actions/copy";
        private const string RouteDefault = "actions/default";

        [HttpGet, Route(Route)]
        public async Task<GetResult> List([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.Templates))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            return await GetResultAsync(site);
        }

        [HttpPost, Route(RouteDefault)]
        public async Task<GetResult> Default([FromBody] TemplateRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.Templates))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            var templateInfo = await DataProvider.TemplateRepository.GetAsync(request.TemplateId);
            if (templateInfo != null && !templateInfo.Default)
            {
                await DataProvider.TemplateRepository.SetDefaultAsync(request.TemplateId);
                await auth.AddSiteLogAsync(site.Id,
                    $"设置默认{templateInfo.TemplateType.GetDisplayName()}",
                    $"模板名称:{templateInfo.TemplateName}");
            }

            return await GetResultAsync(site);
        }

        [HttpPost, Route(RouteCreate)]
        public async Task<BoolResult> Create([FromBody] TemplateRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.Templates))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<BoolResult>();

            await CreateManager.CreateByTemplateAsync(request.SiteId, request.TemplateId);

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteCopy)]
        public async Task<GetResult> Copy([FromBody] TemplateRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.Templates))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            var template = await DataProvider.TemplateRepository.GetAsync(request.TemplateId);

            var templateName = template.TemplateName + "_复件";
            var relatedFileName = PathUtils.RemoveExtension(template.RelatedFileName) + "_复件";
            var createdFileFullName = PathUtils.RemoveExtension(template.CreatedFileFullName) + "_复件";

            var templateNameList = await DataProvider.TemplateRepository.GetTemplateNameListAsync(request.SiteId, template.TemplateType);
            if (templateNameList.Contains(templateName))
            {
                return Request.BadRequest<GetResult>("模板复制失败，模板名称已存在！");
            }
            var fileNameList = await DataProvider.TemplateRepository.GetRelatedFileNameListAsync(request.SiteId, template.TemplateType);
            if (StringUtils.ContainsIgnoreCase(fileNameList, relatedFileName))
            {
                return Request.BadRequest<GetResult>("模板复制失败，模板文件已存在！");
            }

            var templateInfo = new Template
            {
                SiteId = request.SiteId,
                TemplateName = templateName,
                TemplateType = template.TemplateType,
                RelatedFileName = relatedFileName + template.CreatedFileExtName,
                CreatedFileExtName = template.CreatedFileExtName,
                CreatedFileFullName = createdFileFullName + template.CreatedFileExtName,
                Default = false
            };

            var content = await DataProvider.TemplateRepository.GetTemplateContentAsync(site, template);

            templateInfo.Id = await DataProvider.TemplateRepository.InsertAsync(site, templateInfo, content, auth.AdminId);

            return await GetResultAsync(site);
        }

        [HttpDelete, Route(Route)]
        public async Task<GetResult> Delete([FromBody] TemplateRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.Templates))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            var templateInfo = await DataProvider.TemplateRepository.GetAsync(request.TemplateId);
            if (templateInfo != null && !templateInfo.Default)
            {
                await DataProvider.TemplateRepository.DeleteAsync(site, request.TemplateId);
                await auth.AddSiteLogAsync(site.Id,
                    $"删除{templateInfo.TemplateType.GetDisplayName()}",
                    $"模板名称:{templateInfo.TemplateName}");
            }

            return await GetResultAsync(site);
        }
    }
}
