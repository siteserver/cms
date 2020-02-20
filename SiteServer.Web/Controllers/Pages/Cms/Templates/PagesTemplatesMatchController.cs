using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto;
using SiteServer.Abstractions.Dto.Request;
using SiteServer.Abstractions.Dto.Result;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Templates
{
    [RoutePrefix("pages/cms/templates/templatesMatch")]
    public partial class PagesTemplateMatchController : ApiController
    {
        private const string Route = "";
        private const string RouteCreate = "actions/create";

        [HttpGet, Route(Route)]
        public async Task<GetResult> GetConfig([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.TemplateMatch))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.SiteId);
            var cascade = await DataProvider.ChannelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await DataProvider.ContentRepository.GetCountAsync(site, summary);
                var entity = await DataProvider.ChannelRepository.GetAsync(summary.Id);
                return new
                {
                    Count = count,
                    entity.ChannelTemplateId,
                    entity.ContentTemplateId
                };
            });

            var channelTemplates = await DataProvider.TemplateRepository.GetTemplateListByTypeAsync(request.SiteId, TemplateType.ChannelTemplate);
            var contentTemplates = await DataProvider.TemplateRepository.GetTemplateListByTypeAsync(request.SiteId, TemplateType.ContentTemplate);

            return new GetResult
            {
                Channels = cascade,
                ChannelTemplates = channelTemplates,
                ContentTemplates = contentTemplates
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ObjectResult<Cascade<int>>> Submit([FromBody]MatchRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.TemplateMatch))
            {
                return Request.Unauthorized<ObjectResult<Cascade<int>>>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<ObjectResult<Cascade<int>>>();

            if (request.ChannelIds != null && request.ChannelIds.Count > 0)
            {
                if (request.IsChannelTemplate)
                {
                    foreach (var channelId in request.ChannelIds)
                    {
                        var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
                        channelInfo.ChannelTemplateId = request.TemplateId;
                        await DataProvider.ChannelRepository.UpdateChannelTemplateIdAsync(channelInfo);
                    }
                }
                else
                {
                    foreach (var channelId in request.ChannelIds)
                    {
                        var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
                        channelInfo.ContentTemplateId = request.TemplateId;
                        await DataProvider.ChannelRepository.UpdateContentTemplateIdAsync(channelInfo);
                    }
                }
            }

            await auth.AddSiteLogAsync(request.SiteId, "模板匹配");

            var channel = await DataProvider.ChannelRepository.GetAsync(request.SiteId);
            var cascade = await DataProvider.ChannelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await DataProvider.ContentRepository.GetCountAsync(site, summary);
                var entity = await DataProvider.ChannelRepository.GetAsync(summary.Id);
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
        public async Task<GetResult> Create([FromBody]CreateRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.TemplateMatch))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            if (request.IsChannelTemplate && request.IsChildren)
            {
                await CreateChannelChildrenTemplateAsync(site, request, auth.AdminId);
            }
            else if (request.IsChannelTemplate && !request.IsChildren)
            {
                await CreateChannelTemplateAsync(site, request, auth.AdminId);
            }
            else if (!request.IsChannelTemplate && request.IsChildren)
            {
                await CreateContentChildrenTemplateAsync(site, request, auth.AdminId);
            }
            else if (!request.IsChannelTemplate && !request.IsChildren)
            {
                await CreateContentTemplateAsync(site, request, auth.AdminId);
            }

            await auth.AddSiteLogAsync(request.SiteId, "生成并匹配栏目模版");

            var channel = await DataProvider.ChannelRepository.GetAsync(request.SiteId);
            var cascade = await DataProvider.ChannelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await DataProvider.ContentRepository.GetCountAsync(site, summary);
                var entity = await DataProvider.ChannelRepository.GetAsync(summary.Id);
                return new
                {
                    Count = count,
                    entity.ChannelTemplateId,
                    entity.ContentTemplateId
                };
            });

            var channelTemplates = await DataProvider.TemplateRepository.GetTemplateListByTypeAsync(request.SiteId, TemplateType.ChannelTemplate);
            var contentTemplates = await DataProvider.TemplateRepository.GetTemplateListByTypeAsync(request.SiteId, TemplateType.ContentTemplate);

            return new GetResult
            {
                Channels = cascade,
                ChannelTemplates = channelTemplates,
                ContentTemplates = contentTemplates
            };
        }

        private async Task CreateChannelTemplateAsync(Site site, CreateRequest request, int adminId)
        {
            var defaultChannelTemplateId = await DataProvider.TemplateRepository.GetDefaultTemplateIdAsync(request.SiteId, TemplateType.ChannelTemplate);
            var relatedFileNameList = await DataProvider.TemplateRepository.GetRelatedFileNameListAsync(request.SiteId, TemplateType.ChannelTemplate);
            var templateNameList = await DataProvider.TemplateRepository.GetTemplateNameListAsync(request.SiteId, TemplateType.ChannelTemplate);

            foreach (var channelId in request.ChannelIds)
            {
                var channelTemplateId = -1;

                var nodeInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
                if (nodeInfo.ParentId > 0)
                {
                    channelTemplateId = nodeInfo.ChannelTemplateId;
                }

                if (channelTemplateId != -1 && channelTemplateId != 0 && channelTemplateId != defaultChannelTemplateId)
                {
                    if (await DataProvider.TemplateRepository.GetAsync(channelTemplateId) == null)
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
                        Default = false
                    };

                    if (StringUtils.ContainsIgnoreCase(relatedFileNameList, templateInfo.RelatedFileName))
                    {
                        continue;
                    }
                    if (templateNameList.Contains(templateInfo.TemplateName))
                    {
                        continue;
                    }
                    var insertedTemplateId = await DataProvider.TemplateRepository.InsertAsync(site, templateInfo, string.Empty, adminId);
                    if (nodeInfo.ParentId > 0)
                    {
                        nodeInfo.ChannelTemplateId = insertedTemplateId;
                        await DataProvider.ChannelRepository.UpdateChannelTemplateIdAsync(nodeInfo);

                        //TemplateManager.UpdateChannelTemplateId(SiteId, channelId, insertedTemplateId);
                        //DataProvider.BackgroundNodeDAO.UpdateChannelTemplateID(channelId, insertedTemplateID);
                    }

                }
            }
        }

        private async Task CreateChannelChildrenTemplateAsync(Site site, CreateRequest request, int adminId)
        {
            var relatedFileNameList = await DataProvider.TemplateRepository.GetRelatedFileNameListAsync(request.SiteId, TemplateType.ChannelTemplate);
            var templateNameList = await DataProvider.TemplateRepository.GetTemplateNameListAsync(request.SiteId, TemplateType.ChannelTemplate);
            foreach (var channelId in request.ChannelIds)
            {
                var nodeInfo = await DataProvider.ChannelRepository.GetAsync(channelId);

                var templateInfo = new Template
                {
                    Id = 0,
                    SiteId = request.SiteId,
                    TemplateName = nodeInfo.ChannelName + "_下级",
                    TemplateType = TemplateType.ChannelTemplate,
                    RelatedFileName = "T_" + nodeInfo.ChannelName + "_下级.html",
                    CreatedFileFullName = "index.html",
                    CreatedFileExtName = ".html",
                    Default = false
                };

                if (StringUtils.ContainsIgnoreCase(relatedFileNameList, templateInfo.RelatedFileName))
                {
                    continue;
                }
                if (templateNameList.Contains(templateInfo.TemplateName))
                {
                    continue;
                }
                var insertedTemplateId = await DataProvider.TemplateRepository.InsertAsync(site, templateInfo, string.Empty, adminId);
                var childChannelIdList = await DataProvider.ChannelRepository.GetChannelIdsAsync(site.Id, channelId, ScopeType.Descendant);
                foreach (var childChannelId in childChannelIdList)
                {
                    var childChannelInfo = await DataProvider.ChannelRepository.GetAsync(childChannelId);
                    childChannelInfo.ChannelTemplateId = insertedTemplateId;
                    await DataProvider.ChannelRepository.UpdateChannelTemplateIdAsync(childChannelInfo);

                    //TemplateManager.UpdateChannelTemplateId(SiteId, childChannelId, insertedTemplateId);
                    //DataProvider.BackgroundNodeDAO.UpdateChannelTemplateID(childChannelId, insertedTemplateID);
                }
            }
        }

        private async Task CreateContentTemplateAsync(Site site, CreateRequest request, int adminId)
        {
            var defaultContentTemplateId = await DataProvider.TemplateRepository.GetDefaultTemplateIdAsync(request.SiteId, TemplateType.ContentTemplate);
            var relatedFileNameList = await DataProvider.TemplateRepository.GetRelatedFileNameListAsync(request.SiteId, TemplateType.ContentTemplate);
            var templateNameList = await DataProvider.TemplateRepository.GetTemplateNameListAsync(request.SiteId, TemplateType.ContentTemplate);
            foreach (var channelId in request.ChannelIds)
            {
                var nodeInfo = await DataProvider.ChannelRepository.GetAsync(channelId);

                var contentTemplateId = nodeInfo.ContentTemplateId;

                if (contentTemplateId != 0 && contentTemplateId != defaultContentTemplateId)
                {
                    if (await DataProvider.TemplateRepository.GetAsync(contentTemplateId) == null)
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
                        Default = false
                    };
                    if (StringUtils.ContainsIgnoreCase(relatedFileNameList, templateInfo.RelatedFileName))
                    {
                        continue;
                    }
                    if (templateNameList.Contains(templateInfo.TemplateName))
                    {
                        continue;
                    }
                    var insertedTemplateId = await DataProvider.TemplateRepository.InsertAsync(site, templateInfo, string.Empty, adminId);

                    var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
                    channelInfo.ContentTemplateId = insertedTemplateId;
                    await DataProvider.ChannelRepository.UpdateContentTemplateIdAsync(channelInfo);

                    //TemplateManager.UpdateContentTemplateId(SiteId, channelId, insertedTemplateId);
                    //DataProvider.BackgroundNodeDAO.UpdateContentTemplateID(channelId, insertedTemplateID);
                }
            }
        }

        private async Task CreateContentChildrenTemplateAsync(Site site, CreateRequest request, int adminId)
        {
            var relatedFileNameList = await DataProvider.TemplateRepository.GetRelatedFileNameListAsync(request.SiteId, TemplateType.ContentTemplate);
            var templateNameList = await DataProvider.TemplateRepository.GetTemplateNameListAsync(request.SiteId, TemplateType.ContentTemplate);
            foreach (var channelId in request.ChannelIds)
            {
                var nodeInfo = await DataProvider.ChannelRepository.GetAsync(channelId);

                var templateInfo = new Template
                {
                    Id = 0,
                    SiteId = request.SiteId,
                    TemplateName = nodeInfo.ChannelName + "_下级",
                    TemplateType = TemplateType.ContentTemplate,
                    RelatedFileName = "T_" + nodeInfo.ChannelName + "_下级.html",
                    CreatedFileFullName = "index.html",
                    CreatedFileExtName = ".html",
                    Default = false
                };

                if (StringUtils.ContainsIgnoreCase(relatedFileNameList, templateInfo.RelatedFileName))
                {
                    continue;
                }
                if (templateNameList.Contains(templateInfo.TemplateName))
                {
                    continue;
                }
                var insertedTemplateId = await DataProvider.TemplateRepository.InsertAsync(site, templateInfo, string.Empty, adminId);
                var childChannelIdList = await DataProvider.ChannelRepository.GetChannelIdsAsync(request.SiteId, channelId, ScopeType.Descendant);
                foreach (var childChannelId in childChannelIdList)
                {
                    var childChannelInfo = await DataProvider.ChannelRepository.GetAsync(childChannelId);
                    childChannelInfo.ContentTemplateId = insertedTemplateId;
                    await DataProvider.ChannelRepository.UpdateContentTemplateIdAsync(childChannelInfo);

                    //TemplateManager.UpdateContentTemplateId(SiteId, childChannelId, insertedTemplateId);
                    //DataProvider.BackgroundNodeDAO.UpdateContentTemplateID(childChannelId, insertedTemplateID);
                }
            }
        }
    }
}
