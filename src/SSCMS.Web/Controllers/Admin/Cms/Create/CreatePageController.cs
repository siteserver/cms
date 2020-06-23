using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Create
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class CreatePageController : ControllerBase
    {
        private const string Route = "cms/create/createPage";
        private const string RouteAll = "cms/create/createPage/all";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ITemplateRepository _templateRepository;

        public CreatePageController(IAuthManager authManager, ICreateManager createManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, ITemplateRepository templateRepository)
        {
            _authManager = authManager;
            _createManager = createManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _templateRepository = templateRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var permission = string.Empty;
            if (request.Type == CreateType.Index)
            {
                permission = AuthTypes.SitePermissions.CreateIndex;
            }
            else if (request.Type == CreateType.Channel)
            {
                permission = AuthTypes.SitePermissions.CreateChannels;
            }
            else if (request.Type == CreateType.Content)
            {
                permission = AuthTypes.SitePermissions.CreateContents;
            }
            else if (request.Type == CreateType.All)
            {
                permission = AuthTypes.SitePermissions.CreateAll;
            }

            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, permission))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.SiteId);
            var allChannelIds = new List<int>();
            var cascade = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                allChannelIds.Add(summary.Id);
                var count = await _contentRepository.GetCountAsync(site, summary);
                var entity = await _channelRepository.GetAsync(summary.Id);
                return new
                {
                    Count = count,
                    entity.ChannelTemplateId,
                    entity.ContentTemplateId
                };
            });

            var channelTemplates = await _templateRepository.GetTemplatesByTypeAsync(request.SiteId, TemplateType.ChannelTemplate);
            var contentTemplates = await _templateRepository.GetTemplatesByTypeAsync(request.SiteId, TemplateType.ContentTemplate);

            return new GetResult
            {
                Channels = cascade,
                AllChannelIds = allChannelIds,
                ChannelTemplates = channelTemplates,
                ContentTemplates = contentTemplates
            };
        }
        

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Create([FromBody] CreateRequest request)
        {
            var permission = string.Empty;
            if (request.Type == CreateType.Index)
            {
                permission = AuthTypes.SitePermissions.CreateIndex;
            }
            else if (request.Type == CreateType.Channel)
            {
                permission = AuthTypes.SitePermissions.CreateChannels;
            }
            else if (request.Type == CreateType.Content)
            {
                permission = AuthTypes.SitePermissions.CreateContents;
            }
            else if (request.Type == CreateType.All)
            {
                permission = AuthTypes.SitePermissions.CreateAll;
            }

            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, permission))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            var selectedChannelIdList = new List<int>();

            if (request.IsAllChecked)
            {
                selectedChannelIdList = await _channelRepository.GetChannelIdsAsync(request.SiteId);
            }
            else if (request.IsDescendent)
            {
                foreach (var channelId in request.ChannelIdList)
                {
                    selectedChannelIdList.Add(channelId);

                    var channelInfo = await _channelRepository.GetAsync(channelId);
                    if (channelInfo.ChildrenCount > 0)
                    {
                        var descendentIdList = await _channelRepository.GetChannelIdsAsync(request.SiteId, channelId, ScopeType.Descendant);
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
                if (request.Scope == "1month")
                {
                    var lastEditList = await _contentRepository.GetChannelIdsCheckedByLastModifiedDateHourAsync(site, 720);
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
                    var lastEditList = await _contentRepository.GetChannelIdsCheckedByLastModifiedDateHourAsync(site, 24);
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
                    var lastEditList = await _contentRepository.GetChannelIdsCheckedByLastModifiedDateHourAsync(site, 2);
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
                    await _createManager.CreateChannelAsync(request.SiteId, channelId);
                }
                if (request.IsContentPage)
                {
                    await _createManager.CreateAllContentAsync(request.SiteId, channelId);
                }
            }

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteAll)]
        public async Task<ActionResult<BoolResult>> CreateAll([FromBody] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.CreateAll))
            {
                return Unauthorized();
            }

            await _createManager.CreateByAllAsync(request.SiteId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
