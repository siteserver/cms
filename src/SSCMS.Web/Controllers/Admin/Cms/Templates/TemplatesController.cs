using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Extensions;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class TemplatesController : ControllerBase
    {
        private const string Route = "cms/templates/templates";
        private const string RouteCreate = "cms/templates/templates/actions/create";
        private const string RouteCopy = "cms/templates/templates/actions/copy";
        private const string RouteDefault = "cms/templates/templates/actions/default";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICreateManager _createManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly ITemplateRepository _templateRepository;

        public TemplatesController(IAuthManager authManager, IPathManager pathManager, ICreateManager createManager, ISiteRepository siteRepository, IChannelRepository channelRepository, ITemplateRepository templateRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _createManager = createManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _templateRepository = templateRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> List([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.Templates))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            return await GetResultAsync(site);
        }

        [HttpPost, Route(RouteDefault)]
        public async Task<ActionResult<GetResult>> Default([FromBody] TemplateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.Templates))
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

        [HttpPost, Route(RouteCreate)]
        public async Task<ActionResult<BoolResult>> Create([FromBody] TemplateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.Templates))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            await _createManager.CreateByTemplateAsync(request.SiteId, request.TemplateId);

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteCopy)]
        public async Task<ActionResult<GetResult>> Copy([FromBody] TemplateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.Templates))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var template = await _templateRepository.GetAsync(request.TemplateId);

            var templateName = template.TemplateName + "_复件";
            var relatedFileName = PathUtils.RemoveExtension(template.RelatedFileName) + "_复件";
            var createdFileFullName = PathUtils.RemoveExtension(template.CreatedFileFullName) + "_复件";

            var templateNameList = await _templateRepository.GetTemplateNameListAsync(request.SiteId, template.TemplateType);
            if (templateNameList.Contains(templateName))
            {
                return this.Error("模板复制失败，模板名称已存在！");
            }
            var fileNameList = await _templateRepository.GetRelatedFileNameListAsync(request.SiteId, template.TemplateType);
            if (StringUtils.ContainsIgnoreCase(fileNameList, relatedFileName))
            {
                return this.Error("模板复制失败，模板文件已存在！");
            }

            var templateInfo = new Template
            {
                SiteId = request.SiteId,
                TemplateName = templateName,
                TemplateType = template.TemplateType,
                RelatedFileName = relatedFileName + template.CreatedFileExtName,
                CreatedFileExtName = template.CreatedFileExtName,
                CreatedFileFullName = createdFileFullName + template.CreatedFileExtName,
                DefaultTemplate = false
            };

            var content = await _pathManager.GetTemplateContentAsync(site, template);

            var adminId = _authManager.AdminId;
            templateInfo.Id = await _templateRepository.InsertAsync(_pathManager, site, templateInfo, content, adminId);

            return await GetResultAsync(site);
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<GetResult>> Delete([FromBody] TemplateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.Templates))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var templateInfo = await _templateRepository.GetAsync(request.TemplateId);
            if (templateInfo != null && !templateInfo.DefaultTemplate)
            {
                await _templateRepository.DeleteAsync(_pathManager, site, request.TemplateId);
                await _authManager.AddSiteLogAsync(site.Id,
                    $"删除{templateInfo.TemplateType.GetDisplayName()}",
                    $"模板名称:{templateInfo.TemplateName}");
            }

            return await GetResultAsync(site);
        }
    }
}
