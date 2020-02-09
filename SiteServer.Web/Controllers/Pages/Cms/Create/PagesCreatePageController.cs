using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Create
{
    
    [RoutePrefix("pages/cms/create/createPage")]
    public partial class PagesCreatePageController : ApiController
    {
        private const string Route = "";
        private const string RouteAll = "all";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] GetRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            var permission = string.Empty;
            if (request.Type == CreateType.Index)
            {
                permission = Constants.SitePermissions.CreateIndex;
            }
            else if (request.Type == CreateType.Channel)
            {
                permission = Constants.SitePermissions.CreateChannels;
            }
            else if (request.Type == CreateType.Content)
            {
                permission = Constants.SitePermissions.CreateContents;
            }
            else if (request.Type == CreateType.All)
            {
                permission = Constants.SitePermissions.CreateAll;
            }

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, permission))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.SiteId);
            var allChannelIds = new List<int>();
            var cascade = await DataProvider.ChannelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                allChannelIds.Add(summary.Id);
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
                AllChannelIds = allChannelIds,
                ChannelTemplates = channelTemplates,
                ContentTemplates = contentTemplates
            };
        }
        

        [HttpPost, Route(Route)]
        public async Task<BoolResult> Create([FromBody] CreateRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            var permission = string.Empty;
            if (request.Type == CreateType.Index)
            {
                permission = Constants.SitePermissions.CreateIndex;
            }
            else if (request.Type == CreateType.Channel)
            {
                permission = Constants.SitePermissions.CreateChannels;
            }
            else if (request.Type == CreateType.Content)
            {
                permission = Constants.SitePermissions.CreateContents;
            }
            else if (request.Type == CreateType.All)
            {
                permission = Constants.SitePermissions.CreateAll;
            }

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, permission))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var selectedChannelIdList = new List<int>();

            if (request.IsAllChecked)
            {
                selectedChannelIdList = await DataProvider.ChannelRepository.GetChannelIdListAsync(request.SiteId);
            }
            else if (request.IsDescendent)
            {
                foreach (var channelId in request.ChannelIdList)
                {
                    selectedChannelIdList.Add(channelId);

                    var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
                    if (channelInfo.ChildrenCount > 0)
                    {
                        var descendentIdList = await DataProvider.ChannelRepository.GetChannelIdsAsync(channelInfo, EScopeType.Descendant);
                        foreach (var descendentId in descendentIdList)
                        {
                            if (selectedChannelIdList.Contains(descendentId)) continue;

                            selectedChannelIdList.Add(descendentId);
                        }
                    }
                }
            }
            else
            {
                selectedChannelIdList.AddRange(request.ChannelIdList);
            }

            var channelIdList = new List<int>();

            if (request.Scope == "1month" || request.Scope == "1day" || request.Scope == "2hours")
            {
                var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
                var tableName = site.TableName;

                if (request.Scope == "1month")
                {
                    var lastEditList = DataProvider.ContentRepository.GetChannelIdListCheckedByLastEditDateHour(tableName, request.SiteId, 720);
                    foreach (var channelId in lastEditList)
                    {
                        if (selectedChannelIdList.Contains(channelId))
                        {
                            channelIdList.Add(channelId);
                        }
                    }
                }
                else if (request.Scope == "1day")
                {
                    var lastEditList = DataProvider.ContentRepository.GetChannelIdListCheckedByLastEditDateHour(tableName, request.SiteId, 24);
                    foreach (var channelId in lastEditList)
                    {
                        if (selectedChannelIdList.Contains(channelId))
                        {
                            channelIdList.Add(channelId);
                        }
                    }
                }
                else if (request.Scope == "2hours")
                {
                    var lastEditList = DataProvider.ContentRepository.GetChannelIdListCheckedByLastEditDateHour(tableName, request.SiteId, 2);
                    foreach (var channelId in lastEditList)
                    {
                        if (selectedChannelIdList.Contains(channelId))
                        {
                            channelIdList.Add(channelId);
                        }
                    }
                }
            }
            else
            {
                channelIdList = selectedChannelIdList;
            }

            foreach (var channelId in channelIdList)
            {
                if (request.IsChannelPage)
                {
                    await CreateManager.CreateChannelAsync(request.SiteId, channelId);
                }
                if (request.IsContentPage)
                {
                    await CreateManager.CreateAllContentAsync(request.SiteId, channelId);
                }
            }

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteAll)]
        public async Task<BoolResult> CreateAll([FromBody] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.CreateAll))
            {
                return Request.Unauthorized<BoolResult>();
            }

            await CreateManager.CreateByAllAsync(request.SiteId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
