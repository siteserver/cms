using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home.ToDel
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.User)]
    [Route(Constants.ApiHomePrefix + "todel/")]
    public partial class ContentAddController : ControllerBase
    {
        private const string Route = "contentsAdd";

        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IContentGroupRepository _contentGroupRepository;
        private readonly ITableStyleRepository _tableStyleRepository;

        public ContentAddController(IAuthManager authManager, IConfigRepository configRepository, IPathManager pathManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IContentGroupRepository contentGroupRepository, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _configRepository = configRepository;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _contentGroupRepository = contentGroupRepository;
            _tableStyleRepository = tableStyleRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]GetRequest request)
        {
            var sites = new List<SiteResult>();
            var channels = new List<ChannelResult>();
            SiteResult site = null;
            ChannelResult channel = null;
            IEnumerable<string> groupNames = null;
            List<string> tagNames = null;
            Content content = null;
            List<TableStyle> styles = null;
            List<KeyValuePair<int, string>> checkedLevels = null;
            var checkedLevel = 0;

            if (_authManager.IsUser)
            {
                Site siteInfo = null;
                Channel channelInfo = null;
                var siteIdList = await _authManager.GetSiteIdsAsync();
                foreach (var siteId in siteIdList)
                {
                    var permissionSiteInfo = await _siteRepository.GetAsync(siteId);
                    if (request.SiteId == siteId)
                    {
                        siteInfo = permissionSiteInfo;
                    }
                    sites.Add(new SiteResult
                    {
                        Id = permissionSiteInfo.Id,
                        SiteName = permissionSiteInfo.SiteName
                    });
                }

                if (siteInfo == null && siteIdList.Count > 0)
                {
                    siteInfo = await _siteRepository.GetAsync(siteIdList[0]);
                }

                if (siteInfo != null)
                {
                    var channelIdList = await _authManager.GetChannelIdsAsync(siteInfo.Id,
                        AuthTypes.ContentPermissions.Add);
                    foreach (var permissionChannelId in channelIdList)
                    {
                        var permissionChannelInfo = await _channelRepository.GetAsync(permissionChannelId);
                        if (channelInfo == null || permissionChannelInfo.Id == request.ChannelId)
                        {
                            channelInfo = permissionChannelInfo;
                        }
                        channels.Add(new ChannelResult
                        {
                            Id = permissionChannelInfo.Id,
                            ChannelName = await _channelRepository.GetChannelNameNavigationAsync(siteInfo.Id, permissionChannelId)
                        });
                    }

                    site = new SiteResult
                    {
                        Id = siteInfo.Id,
                        SiteName = siteInfo.SiteName,
                        SiteUrl = await _pathManager.GetSiteUrlAsync(siteInfo, false)
                    };

                    groupNames = await _contentGroupRepository.GetGroupNamesAsync(siteInfo.Id);
                    tagNames = new List<string>();
                }

                if (channelInfo != null)
                {
                    channel = new ChannelResult
                    {
                        Id = channelInfo.Id,
                        ChannelName = await _channelRepository.GetChannelNameNavigationAsync(siteInfo.Id, channelInfo.Id)
                    };

                    var tableName = _channelRepository.GetTableName(siteInfo, channelInfo);
                    styles = await _tableStyleRepository.GetContentStylesAsync(channelInfo, tableName);

                    var (userIsChecked, userCheckedLevel) = await CheckManager.GetUserCheckLevelAsync(_authManager, siteInfo, siteInfo.Id);
                    checkedLevels = CheckManager.GetCheckedLevels(siteInfo, userIsChecked, userCheckedLevel, true);

                    if (request.ContentId != 0)
                    {
                        //checkedLevels.Insert(0, new KeyValuePair<int, string>(CheckManager.LevelInt.NotChange, CheckManager.Level.NotChange));
                        //checkedLevel = CheckManager.LevelInt.NotChange;

                        content = await _contentRepository.GetAsync(siteInfo, channelInfo, request.ContentId);
                        if (content != null &&
                            (content.SiteId != siteInfo.Id || content.ChannelId != channelInfo.Id))
                        {
                            content = null;
                        }
                    }
                    else
                    {
                        content = new Content
                        {
                            Id = 0,
                            SiteId = siteInfo.Id,
                            ChannelId = channelInfo.Id,
                            AddDate = DateTime.Now
                        };
                    }
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
                Site = site,
                Channel = channel,
                AllGroupNames = groupNames,
                AllTagNames = tagNames,
                Styles = styles,
                CheckedLevels = checkedLevels,
                CheckedLevel = checkedLevel,
                Content = content,
            };
        }
    }
}
