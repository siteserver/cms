using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS;
using SSCMS.Dto;
using SSCMS.Dto.Request;
using SSCMS.Dto.Result;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    [Route("admin/cms/templates/templatesMatch")]
    public partial class TemplatesMatchController : ControllerBase
    {
        private const string Route = "";
        private const string RouteCreate = "actions/create";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ITemplateRepository _templateRepository;

        public TemplatesMatchController(IAuthManager authManager, IPathManager pathManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, ITemplateRepository templateRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _templateRepository = templateRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> GetConfig([FromQuery] SiteRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.TemplateMatch))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.SiteId);
            var cascade = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);
                var entity = await _channelRepository.GetAsync(summary.Id);
                return new
                {
                    Count = count,
                    entity.ChannelTemplateId,
                    entity.ContentTemplateId
                };
            });

            var channelTemplates = await _templateRepository.GetTemplateListByTypeAsync(request.SiteId, TemplateType.ChannelTemplate);
            var contentTemplates = await _templateRepository.GetTemplateListByTypeAsync(request.SiteId, TemplateType.ContentTemplate);

            return new GetResult
            {
                Channels = cascade,
                ChannelTemplates = channelTemplates,
                ContentTemplates = contentTemplates
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<ObjectResult<Cascade<int>>>> Submit([FromBody]MatchRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.TemplateMatch))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            if (request.ChannelIds != null && request.ChannelIds.Count > 0)
            {
                if (request.IsChannelTemplate)
                {
                    foreach (var channelId in request.ChannelIds)
                    {
                        var channelInfo = await _channelRepository.GetAsync(channelId);
                        channelInfo.ChannelTemplateId = request.TemplateId;
                        await _channelRepository.UpdateChannelTemplateIdAsync(channelInfo);
                    }
                }
                else
                {
                    foreach (var channelId in request.ChannelIds)
                    {
                        var channelInfo = await _channelRepository.GetAsync(channelId);
                        channelInfo.ContentTemplateId = request.TemplateId;
                        await _channelRepository.UpdateContentTemplateIdAsync(channelInfo);
                    }
                }
            }

            await _authManager.AddSiteLogAsync(request.SiteId, "模板匹配");

            var channel = await _channelRepository.GetAsync(request.SiteId);
            var cascade = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);
                var entity = await _channelRepository.GetAsync(summary.Id);
                return new
                {
                    Count = count,
                    entity.ChannelTemplateId,
                    entity.ContentTemplateId
                };
            });

            return new ObjectResult<Cascade<int>>
            {
                Value = cascade
            };
        }

        [HttpPost, Route(RouteCreate)]
        public async Task<ActionResult<GetResult>> Create([FromBody]CreateRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.TemplateMatch))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var adminId = await _authManager.GetAdminIdAsync();
            if (request.IsChannelTemplate && request.IsChildren)
            {
                await CreateChannelChildrenTemplateAsync(site, request, adminId);
            }
            else if (request.IsChannelTemplate && !request.IsChildren)
            {
                await CreateChannelTemplateAsync(site, request, adminId);
            }
            else if (!request.IsChannelTemplate && request.IsChildren)
            {
                await CreateContentChildrenTemplateAsync(site, request, adminId);
            }
            else if (!request.IsChannelTemplate && !request.IsChildren)
            {
                await CreateContentTemplateAsync(site, request, adminId);
            }

            await _authManager.AddSiteLogAsync(request.SiteId, "生成并匹配栏目模版");

            var channel = await _channelRepository.GetAsync(request.SiteId);
            var cascade = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);
                var entity = await _channelRepository.GetAsync(summary.Id);
                return new
                {
                    Count = count,
                    entity.ChannelTemplateId,
                    entity.ContentTemplateId
                };
            });

            var channelTemplates = await _templateRepository.GetTemplateListByTypeAsync(request.SiteId, TemplateType.ChannelTemplate);
            var contentTemplates = await _templateRepository.GetTemplateListByTypeAsync(request.SiteId, TemplateType.ContentTemplate);

            return new GetResult
            {
                Channels = cascade,
                ChannelTemplates = channelTemplates,
                ContentTemplates = contentTemplates
            };
        }

        private async Task CreateChannelTemplateAsync(Site site, CreateRequest request, int adminId)
        {
            var defaultChannelTemplateId = await _templateRepository.GetDefaultTemplateIdAsync(request.SiteId, TemplateType.ChannelTemplate);
            var relatedFileNameList = await _templateRepository.GetRelatedFileNameListAsync(request.SiteId, TemplateType.ChannelTemplate);
            var templateNameList = await _templateRepository.GetTemplateNameListAsync(request.SiteId, TemplateType.ChannelTemplate);

            foreach (var channelId in request.ChannelIds)
            {
                var channelTemplateId = -1;

                var nodeInfo = await _channelRepository.GetAsync(channelId);
                if (nodeInfo.ParentId > 0)
                {
                    channelTemplateId = nodeInfo.ChannelTemplateId;
                }

                if (channelTemplateId != -1 && channelTemplateId != 0 && channelTemplateId != defaultChannelTemplateId)
                {
                    if (await _templateRepository.GetAsync(channelTemplateId) == null)
                    {
                        channelTemplateId = -1;
                    }
                }

                if (channelTemplateId != -1)
                {
                    var templateInfo = new Template
                    {
                        Id = 0,
                        SiteId = request.SiteId,
                        TemplateName = nodeInfo.ChannelName,
                        TemplateType = TemplateType.ChannelTemplate,
                        RelatedFileName = "T_" + nodeInfo.ChannelName + ".html",
                        CreatedFileFullName = "index.html",
                        CreatedFileExtName = ".html",
                        DefaultTemplate = false
                    };

                    if (StringUtils.ContainsIgnoreCase(relatedFileNameList, templateInfo.RelatedFileName))
                    {
                        continue;
                    }
                    if (templateNameList.Contains(templateInfo.TemplateName))
                    {
                        continue;
                    }
                    var insertedTemplateId = await _templateRepository.InsertAsync(_pathManager, site, templateInfo, string.Empty, adminId);
                    if (nodeInfo.ParentId > 0)
                    {
                        nodeInfo.ChannelTemplateId = insertedTemplateId;
                        await _channelRepository.UpdateChannelTemplateIdAsync(nodeInfo);

                        //TemplateManager.UpdateChannelTemplateId(SiteId, channelId, insertedTemplateId);
                        //DataProvider.BackgroundNodeDAO.UpdateChannelTemplateID(channelId, insertedTemplateID);
                    }

                }
            }
        }

        private async Task CreateChannelChildrenTemplateAsync(Site site, CreateRequest request, int adminId)
        {
            var relatedFileNameList = await _templateRepository.GetRelatedFileNameListAsync(request.SiteId, TemplateType.ChannelTemplate);
            var templateNameList = await _templateRepository.GetTemplateNameListAsync(request.SiteId, TemplateType.ChannelTemplate);
            foreach (var channelId in request.ChannelIds)
            {
                var nodeInfo = await _channelRepository.GetAsync(channelId);

                var templateInfo = new Template
                {
                    Id = 0,
                    SiteId = request.SiteId,
                    TemplateName = nodeInfo.ChannelName + "_下级",
                    TemplateType = TemplateType.ChannelTemplate,
                    RelatedFileName = "T_" + nodeInfo.ChannelName + "_下级.html",
                    CreatedFileFullName = "index.html",
                    CreatedFileExtName = ".html",
                    DefaultTemplate = false
                };

                if (StringUtils.ContainsIgnoreCase(relatedFileNameList, templateInfo.RelatedFileName))
                {
                    continue;
                }
                if (templateNameList.Contains(templateInfo.TemplateName))
                {
                    continue;
                }
                var insertedTemplateId = await _templateRepository.InsertAsync(_pathManager, site, templateInfo, string.Empty, adminId);
                var childChannelIdList = await _channelRepository.GetChannelIdsAsync(site.Id, channelId, ScopeType.Descendant);
                foreach (var childChannelId in childChannelIdList)
                {
                    var childChannelInfo = await _channelRepository.GetAsync(childChannelId);
                    childChannelInfo.ChannelTemplateId = insertedTemplateId;
                    await _channelRepository.UpdateChannelTemplateIdAsync(childChannelInfo);

                    //TemplateManager.UpdateChannelTemplateId(SiteId, childChannelId, insertedTemplateId);
                    //DataProvider.BackgroundNodeDAO.UpdateChannelTemplateID(childChannelId, insertedTemplateID);
                }
            }
        }

        private async Task CreateContentTemplateAsync(Site site, CreateRequest request, int adminId)
        {
            var defaultContentTemplateId = await _templateRepository.GetDefaultTemplateIdAsync(request.SiteId, TemplateType.ContentTemplate);
            var relatedFileNameList = await _templateRepository.GetRelatedFileNameListAsync(request.SiteId, TemplateType.ContentTemplate);
            var templateNameList = await _templateRepository.GetTemplateNameListAsync(request.SiteId, TemplateType.ContentTemplate);
            foreach (var channelId in request.ChannelIds)
            {
                var nodeInfo = await _channelRepository.GetAsync(channelId);

                var contentTemplateId = nodeInfo.ContentTemplateId;

                if (contentTemplateId != 0 && contentTemplateId != defaultContentTemplateId)
                {
                    if (await _templateRepository.GetAsync(contentTemplateId) == null)
                    {
                        contentTemplateId = -1;
                    }
                }

                if (contentTemplateId != -1)
                {
                    var templateInfo = new Template
                    {
                        Id = 0,
                        SiteId = request.SiteId,
                        TemplateName = nodeInfo.ChannelName,
                        TemplateType = TemplateType.ContentTemplate,
                        RelatedFileName = "T_" + nodeInfo.ChannelName + ".html",
                        CreatedFileFullName = "index.html",
                        CreatedFileExtName = ".html",
                        DefaultTemplate = false
                    };
                    if (StringUtils.ContainsIgnoreCase(relatedFileNameList, templateInfo.RelatedFileName))
                    {
                        continue;
                    }
                    if (templateNameList.Contains(templateInfo.TemplateName))
                    {
                        continue;
                    }
                    var insertedTemplateId = await _templateRepository.InsertAsync(_pathManager, site, templateInfo, string.Empty, adminId);

                    var channelInfo = await _channelRepository.GetAsync(channelId);
                    channelInfo.ContentTemplateId = insertedTemplateId;
                    await _channelRepository.UpdateContentTemplateIdAsync(channelInfo);

                    //TemplateManager.UpdateContentTemplateId(SiteId, channelId, insertedTemplateId);
                    //DataProvider.BackgroundNodeDAO.UpdateContentTemplateID(channelId, insertedTemplateID);
                }
            }
        }

        private async Task CreateContentChildrenTemplateAsync(Site site, CreateRequest request, int adminId)
        {
            var relatedFileNameList = await _templateRepository.GetRelatedFileNameListAsync(request.SiteId, TemplateType.ContentTemplate);
            var templateNameList = await _templateRepository.GetTemplateNameListAsync(request.SiteId, TemplateType.ContentTemplate);
            foreach (var channelId in request.ChannelIds)
            {
                var nodeInfo = await _channelRepository.GetAsync(channelId);

                var templateInfo = new Template
                {
                    Id = 0,
                    SiteId = request.SiteId,
                    TemplateName = nodeInfo.ChannelName + "_下级",
                    TemplateType = TemplateType.ContentTemplate,
                    RelatedFileName = "T_" + nodeInfo.ChannelName + "_下级.html",
                    CreatedFileFullName = "index.html",
                    CreatedFileExtName = ".html",
                    DefaultTemplate = false
                };

                if (StringUtils.ContainsIgnoreCase(relatedFileNameList, templateInfo.RelatedFileName))
                {
                    continue;
                }
                if (templateNameList.Contains(templateInfo.TemplateName))
                {
                    continue;
                }
                var insertedTemplateId = await _templateRepository.InsertAsync(_pathManager, site, templateInfo, string.Empty, adminId);
                var childChannelIdList = await _channelRepository.GetChannelIdsAsync(request.SiteId, channelId, ScopeType.Descendant);
                foreach (var childChannelId in childChannelIdList)
                {
                    var childChannelInfo = await _channelRepository.GetAsync(childChannelId);
                    childChannelInfo.ContentTemplateId = insertedTemplateId;
                    await _channelRepository.UpdateContentTemplateIdAsync(childChannelInfo);

                    //TemplateManager.UpdateContentTemplateId(SiteId, childChannelId, insertedTemplateId);
                    //DataProvider.BackgroundNodeDAO.UpdateContentTemplateID(childChannelId, insertedTemplateID);
                }
            }
        }
    }
}
