using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.User)]
    [Route(Constants.ApiHomePrefix)]
    public partial class ContentsController : ControllerBase
    {
        private const string Route = "contents";

        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;
        private readonly IOldPluginManager _pluginManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public ContentsController(IAuthManager authManager, IConfigRepository configRepository, IOldPluginManager pluginManager, IDatabaseManager databaseManager, IPathManager pathManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository)
        {
            _authManager = authManager;
            _configRepository = configRepository;
            _pluginManager = pluginManager;
            _databaseManager = databaseManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]ChannelRequest request)
        {
            var sites = new List<SiteResult>();
            var channels = new List<ChannelResult>();
            SiteResult siteResult = null;
            ChannelResult channelResult = null;

            if (_authManager.IsUser)
            {
                Site site = null;
                Channel channelInfo = null;
                var siteIdList = await _authManager.GetSiteIdsAsync();
                foreach (var siteId in siteIdList)
                {
                    var permissionSite = await _siteRepository.GetAsync(siteId);
                    if (request.SiteId == siteId)
                    {
                        site = permissionSite;
                    }
                    sites.Add(new SiteResult
                    {
                        Id = permissionSite.Id,
                        SiteName = permissionSite.SiteName
                    });
                }

                if (site == null && siteIdList.Count > 0)
                {
                    site = await _siteRepository.GetAsync(siteIdList[0]);
                }

                if (site != null)
                {
                    var channelIdList = await _authManager.GetChannelIdsAsync(site.Id,
                        AuthTypes.SiteContentPermissions.Add);
                    foreach (var permissionChannelId in channelIdList)
                    {
                        var permissionChannelInfo = await _channelRepository.GetAsync(permissionChannelId);
                        if (channelInfo == null || request.ChannelId == permissionChannelId)
                        {
                            channelInfo = permissionChannelInfo;
                        }

                        channels.Add(new ChannelResult
                        {
                            Id = permissionChannelInfo.Id,
                            ChannelName =
                                await _channelRepository.GetChannelNameNavigationAsync(site.Id, permissionChannelId)
                        });
                    }

                    siteResult = new SiteResult
                    {
                        Id = site.Id,
                        SiteName = site.SiteName,
                        SiteUrl = await _pathManager.GetSiteUrlAsync(site, false)
                    };
                }

                if (channelInfo != null)
                {
                    channelResult = new ChannelResult
                    {
                        Id = channelInfo.Id,
                        ChannelName = await _channelRepository.GetChannelNameNavigationAsync(site.Id, channelInfo.Id)
                    };
                }
            }

            var config = await _configRepository.GetAsync();
            var user = await _authManager.GetUserAsync();

            return new GetResult
            {
                User = user,
                Config = config,
                Sites = sites,
                Channels = channels,
                Site = siteResult,
                Channel = channelResult
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<ListResult>> List([FromQuery]ListRequest request)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.SiteContentPermissions.View))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var columnsManager = new ColumnsManager(_databaseManager, _pluginManager, _pathManager);
            var columns = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.Contents);
            var pluginIds = _pluginManager.GetContentPluginIds(channel);
            var pluginColumns = _pluginManager.GetContentColumns(pluginIds);

            var pageContentInfoList = new List<Content>();
            var ccIds = await _contentRepository.GetSummariesAsync(site, channel, true);
            var count = ccIds.Count;

            var pages = Convert.ToInt32(Math.Ceiling((double)count / site.PageSize));
            if (pages == 0) pages = 1;

            if (count > 0)
            {
                var offset = site.PageSize * (request.Page - 1);
                var limit = site.PageSize;
                var pageCcIds = ccIds.Skip(offset).Take(limit).ToList();

                var sequence = offset + 1;
                foreach (var channelContentId in pageCcIds)
                {
                    var contentInfo = await _contentRepository.GetAsync(site, channelContentId.ChannelId, channelContentId.Id);
                    if (contentInfo == null) continue;

                    pageContentInfoList.Add(await columnsManager.CalculateContentListAsync(sequence++, site, request.ChannelId, contentInfo, columns, pluginColumns));
                }
            }

            var userPermissions = new Permissions
            {
                IsAdd = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, AuthTypes.SiteContentPermissions.Add),
                IsDelete = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, AuthTypes.SiteContentPermissions.Delete),
                IsEdit = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, AuthTypes.SiteContentPermissions.Edit),
                IsTranslate = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, AuthTypes.SiteContentPermissions.Translate),
                IsCheck = await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, AuthTypes.SiteContentPermissions.CheckLevel1),
                IsCreate = await _authManager.HasSitePermissionsAsync(site.Id, AuthTypes.SitePermissions.CreateContents) || await _authManager.HasContentPermissionsAsync(site.Id, channel.Id, AuthTypes.SiteContentPermissions.Create),
                IsChannelEdit = await _authManager.HasChannelPermissionsAsync(site.Id, channel.Id, AuthTypes.SiteChannelPermissions.Edit)
            };

            return new ListResult
            {
                Contents = pageContentInfoList,
                Count = count,
                Pages = pages,
                Permissions = userPermissions,
                Columns = columns
            };
        }
    }
}
