using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Dto;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [RoutePrefix("pages/cms/templateMatch")]
    public partial class PagesTemplateMatchController : ApiController
    {
        private const string Route = "";
        private const string RouteCreate = "actions/create";

        [HttpGet, Route(Route)]
        public async Task<GetResult> GetConfig([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            await auth.CheckSitePermissionsAsync(Request, request.SiteId, Constants.SitePermissions.TemplateMatch);

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            var channel = await ChannelManager.GetChannelAsync(request.SiteId, request.SiteId);
            var cascade = await ChannelManager.GetCascadeAsync(site, channel, async (siteInfo, channelInfo) =>
            {
                var dict = new Dictionary<string, object>
                {
                    ["count"] = await DataProvider.ContentRepository.GetCountAsync(siteInfo, channelInfo, 0),
                    ["channelTemplateId"] = channelInfo.ChannelTemplateId,
                    ["contentTemplateId"] = channelInfo.ContentTemplateId
                };
                return dict;
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
        public async Task<GenericResult<Cascade<int>>> Submit([FromBody]MatchRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            await auth.CheckSitePermissionsAsync(Request, request.SiteId, Constants.SitePermissions.TemplateMatch);

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GenericResult<Cascade<int>>>();

            if (request.ChannelIds != null && request.ChannelIds.Count > 0)
            {
                if (request.IsChannelTemplate)
                {
                    foreach (var channelId in request.ChannelIds)
                    {
                        var channelInfo = await ChannelManager.GetChannelAsync(request.SiteId, channelId);
                        channelInfo.ChannelTemplateId = request.TemplateId;
                        await DataProvider.ChannelRepository.UpdateChannelTemplateIdAsync(channelInfo);
                    }
                }
                else
                {
                    foreach (var channelId in request.ChannelIds)
                    {
                        var channelInfo = await ChannelManager.GetChannelAsync(request.SiteId, channelId);
                        channelInfo.ContentTemplateId = request.TemplateId;
                        await DataProvider.ChannelRepository.UpdateContentTemplateIdAsync(channelInfo);
                    }
                }
            }

            await auth.AddSiteLogAsync(request.SiteId, "模板匹配");

            var channel = await ChannelManager.GetChannelAsync(request.SiteId, request.SiteId);
            var cascade = await ChannelManager.GetCascadeAsync(site, channel, async (siteInfo, channelInfo) =>
            {
                var dict = new Dictionary<string, object>
                {
                    ["count"] = await DataProvider.ContentRepository.GetCountAsync(siteInfo, channelInfo, 0),
                    ["channelTemplateId"] = channelInfo.ChannelTemplateId,
                    ["contentTemplateId"] = channelInfo.ContentTemplateId
                };
                return dict;
            });

            return new GenericResult<Cascade<int>>
            {
                Value = cascade
            };
        }

        [HttpPost, Route(RouteCreate)]
        public async Task<GetResult> Create([FromBody]CreateRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            await auth.CheckSitePermissionsAsync(Request, request.SiteId, Constants.SitePermissions.TemplateMatch);

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            if (request.IsChannelTemplate && request.IsChildren)
            {
                await CreateChannelChildrenTemplateAsync(request, auth.AdminName);
            }
            else if (request.IsChannelTemplate && !request.IsChildren)
            {
                await CreateChannelTemplateAsync(request, auth.AdminName);
            }
            else if (!request.IsChannelTemplate && request.IsChildren)
            {
                await CreateContentChildrenTemplateAsync(request, auth.AdminName);
            }
            else if (!request.IsChannelTemplate && !request.IsChildren)
            {
                await CreateContentTemplateAsync(request, auth.AdminName);
            }

            await auth.AddSiteLogAsync(request.SiteId, "生成并匹配栏目模版");

            var channel = await ChannelManager.GetChannelAsync(request.SiteId, request.SiteId);
            var cascade = await ChannelManager.GetCascadeAsync(site, channel, async (siteInfo, channelInfo) =>
            {
                var dict = new Dictionary<string, object>
                {
                    ["count"] = await DataProvider.ContentRepository.GetCountAsync(siteInfo, channelInfo, 0),
                    ["channelTemplateId"] = channelInfo.ChannelTemplateId,
                    ["contentTemplateId"] = channelInfo.ContentTemplateId
                };
                return dict;
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

        private async Task CreateChannelTemplateAsync(CreateRequest request, string adminName)
        {
            var defaultChannelTemplateId = await TemplateManager.GetDefaultTemplateIdAsync(request.SiteId, TemplateType.ChannelTemplate);
            var relatedFileNameList = await DataProvider.TemplateRepository.GetRelatedFileNameListAsync(request.SiteId, TemplateType.ChannelTemplate);
            var templateNameList = await DataProvider.TemplateRepository.GetTemplateNameListAsync(request.SiteId, TemplateType.ChannelTemplate);

            foreach (var channelId in request.ChannelIds)
            {
                var channelTemplateId = -1;

                var nodeInfo = await ChannelManager.GetChannelAsync(request.SiteId, channelId);
                if (nodeInfo.ParentId > 0)
                {
                    channelTemplateId = nodeInfo.ChannelTemplateId;
                }

                if (channelTemplateId != -1 && channelTemplateId != 0 && channelTemplateId != defaultChannelTemplateId)
                {
                    if (await TemplateManager.GetTemplateAsync(request.SiteId, channelTemplateId) == null)
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
                    var insertedTemplateId = await DataProvider.TemplateRepository.InsertAsync(templateInfo, string.Empty, adminName);
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

        private async Task CreateChannelChildrenTemplateAsync(CreateRequest request, string adminName)
        {
            var relatedFileNameList = await DataProvider.TemplateRepository.GetRelatedFileNameListAsync(request.SiteId, TemplateType.ChannelTemplate);
            var templateNameList = await DataProvider.TemplateRepository.GetTemplateNameListAsync(request.SiteId, TemplateType.ChannelTemplate);
            foreach (var channelId in request.ChannelIds)
            {
                var nodeInfo = await ChannelManager.GetChannelAsync(request.SiteId, channelId);

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
                var insertedTemplateId = await DataProvider.TemplateRepository.InsertAsync(templateInfo, string.Empty, adminName);
                var childChannelIdList = await ChannelManager.GetChannelIdListAsync(await ChannelManager.GetChannelAsync(request.SiteId, channelId), EScopeType.Descendant, string.Empty, string.Empty, string.Empty);
                foreach (var childChannelId in childChannelIdList)
                {
                    var childChannelInfo = await ChannelManager.GetChannelAsync(request.SiteId, childChannelId);
                    childChannelInfo.ChannelTemplateId = insertedTemplateId;
                    await DataProvider.ChannelRepository.UpdateChannelTemplateIdAsync(childChannelInfo);

                    //TemplateManager.UpdateChannelTemplateId(SiteId, childChannelId, insertedTemplateId);
                    //DataProvider.BackgroundNodeDAO.UpdateChannelTemplateID(childChannelId, insertedTemplateID);
                }
            }
        }

        private async Task CreateContentTemplateAsync(CreateRequest request, string adminName)
        {
            var defaultContentTemplateId = await TemplateManager.GetDefaultTemplateIdAsync(request.SiteId, TemplateType.ContentTemplate);
            var relatedFileNameList = await DataProvider.TemplateRepository.GetRelatedFileNameListAsync(request.SiteId, TemplateType.ContentTemplate);
            var templateNameList = await DataProvider.TemplateRepository.GetTemplateNameListAsync(request.SiteId, TemplateType.ContentTemplate);
            foreach (var channelId in request.ChannelIds)
            {
                var nodeInfo = await ChannelManager.GetChannelAsync(request.SiteId, channelId);

                var contentTemplateId = nodeInfo.ContentTemplateId;

                if (contentTemplateId != 0 && contentTemplateId != defaultContentTemplateId)
                {
                    if (await TemplateManager.GetTemplateAsync(request.SiteId, contentTemplateId) == null)
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
                    var insertedTemplateId = await DataProvider.TemplateRepository.InsertAsync(templateInfo, string.Empty, adminName);

                    var channelInfo = await ChannelManager.GetChannelAsync(request.SiteId, channelId);
                    channelInfo.ContentTemplateId = insertedTemplateId;
                    await DataProvider.ChannelRepository.UpdateContentTemplateIdAsync(channelInfo);

                    //TemplateManager.UpdateContentTemplateId(SiteId, channelId, insertedTemplateId);
                    //DataProvider.BackgroundNodeDAO.UpdateContentTemplateID(channelId, insertedTemplateID);
                }
            }
        }

        private async Task CreateContentChildrenTemplateAsync(CreateRequest request, string adminName)
        {
            var relatedFileNameList = await DataProvider.TemplateRepository.GetRelatedFileNameListAsync(request.SiteId, TemplateType.ContentTemplate);
            var templateNameList = await DataProvider.TemplateRepository.GetTemplateNameListAsync(request.SiteId, TemplateType.ContentTemplate);
            foreach (var channelId in request.ChannelIds)
            {
                var nodeInfo = await ChannelManager.GetChannelAsync(request.SiteId, channelId);

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
                var insertedTemplateId = await DataProvider.TemplateRepository.InsertAsync(templateInfo, string.Empty, adminName);
                var childChannelIdList = await ChannelManager.GetChannelIdListAsync(await ChannelManager.GetChannelAsync(request.SiteId, channelId), EScopeType.Descendant, string.Empty, string.Empty, string.Empty);
                foreach (var childChannelId in childChannelIdList)
                {
                    var childChannelInfo = await ChannelManager.GetChannelAsync(request.SiteId, childChannelId);
                    childChannelInfo.ContentTemplateId = insertedTemplateId;
                    await DataProvider.ChannelRepository.UpdateContentTemplateIdAsync(childChannelInfo);

                    //TemplateManager.UpdateContentTemplateId(SiteId, childChannelId, insertedTemplateId);
                    //DataProvider.BackgroundNodeDAO.UpdateContentTemplateID(childChannelId, insertedTemplateID);
                }
            }
        }
    }
}
