using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    [Route("home")]
    public partial class IndexController : ControllerBase
    {
        private const string Route = "";

        private const string PageNameRegister = "register";
        private const string PageNameIndex = "index";
        private const string PageNameProfile = "profile";
        private const string PageNameContents = "contents";
        private const string PageNameContentAdd = "contentAdd";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IPluginManager _pluginManager;
        private readonly IConfigRepository _configRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IUserMenuRepository _userMenuRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IContentGroupRepository _contentGroupRepository;

        public IndexController(IAuthManager authManager, IPathManager pathManager, IPluginManager pluginManager, IConfigRepository configRepository, ITableStyleRepository tableStyleRepository, IUserGroupRepository userGroupRepository, IUserMenuRepository userMenuRepository, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IContentGroupRepository contentGroupRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _pluginManager = pluginManager;
            _configRepository = configRepository;
            _tableStyleRepository = tableStyleRepository;
            _userGroupRepository = userGroupRepository;
            _userMenuRepository = userMenuRepository;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _contentGroupRepository = contentGroupRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<object>> GetConfig([FromQuery]GetRequest request)
        {
            if (request.PageName == PageNameRegister)
            {
                return await GetRegisterAsync();
            }
            if (request.PageName == PageNameIndex)
            {
                return await GetIndexAsync();
            }
            if (request.PageName == PageNameProfile)
            {
                return await GetProfileAsync();
            }
            if (request.PageName == PageNameContents)
            {
                return await GetContentsAsync(request.SiteId, request.ContentId);
            }
            if (request.PageName == PageNameContentAdd)
            {
                return await GetContentAddAsync(request.SiteId, request.ContentId, request.ContentId);
            }

            var config = await _configRepository.GetAsync();

            return new
            {
                Value = await _authManager.GetUserAsync(),
                Config = config
            };
        }

        private async Task<object> GetRegisterAsync()
        {
            var config = await _configRepository.GetAsync();
            var user = await _authManager.GetUserAsync();

            return new
            {
                Value = user,
                Config = config,
                Styles = await _tableStyleRepository.GetUserStyleListAsync(),
                Groups = await _userGroupRepository.GetUserGroupListAsync()
            };
        }

        private async Task<object> GetIndexAsync()
        {
            var menus = new List<object>();
            var defaultPageUrl = string.Empty;
            var user = await _authManager.GetUserAsync();

            if (await _authManager.IsUserAuthenticatedAsync())
            {
                var userMenus = await _userMenuRepository.GetUserMenuListAsync();

                foreach (var menuInfo1 in userMenus)
                {
                    if (menuInfo1.Disabled || menuInfo1.ParentId != 0 ||
                        menuInfo1.GroupIds.Contains(user.GroupId)) continue;
                    var children = new List<object>();
                    foreach (var menuInfo2 in userMenus)
                    {
                        if (menuInfo2.Disabled || menuInfo2.ParentId != menuInfo1.Id ||
                            menuInfo2.GroupIds.Contains(user.GroupId)) continue;

                        children.Add(new
                        {
                            menuInfo2.Text,
                            menuInfo2.IconClass,
                            menuInfo2.Href,
                            menuInfo2.Target
                        });
                    }

                    menus.Add(new
                    {
                        menuInfo1.Text,
                        menuInfo1.IconClass,
                        menuInfo1.Href,
                        menuInfo1.Target,
                        Menus = children
                    });
                }

                defaultPageUrl = await _pluginManager.GetHomeDefaultPageUrlAsync();
            }

            var config = await _configRepository.GetAsync();

            return new
            {
                Value = user,
                Config = config,
                Menus = menus,
                DefaultPageUrl = defaultPageUrl
            };
        }

        private async Task<object> GetProfileAsync()
        {
            var config = await _configRepository.GetAsync();
            var user = await _authManager.GetUserAsync();

            return new
            {
                Value = user,
                Config = config,
                Styles = await _tableStyleRepository.GetUserStyleListAsync()
            };
        }

        private async Task<object> GetContentsAsync(int requestSiteId, int requestChannelId)
        {
            var sites = new List<object>();
            var channels = new List<object>();
            object siteInfo = null;
            object channel = null;

            if (await _authManager.IsUserAuthenticatedAsync())
            {
                Site site = null;
                Channel channelInfo = null;
                var siteIdList = await _authManager.GetSiteIdListAsync();
                foreach (var siteId in siteIdList)
                {
                    var permissionSite = await _siteRepository.GetAsync(siteId);
                    if (requestSiteId == siteId)
                    {
                        site = permissionSite;
                    }
                    sites.Add(new
                    {
                        permissionSite.Id,
                        permissionSite.SiteName
                    });
                }

                if (site == null && siteIdList.Count > 0)
                {
                    site = await _siteRepository.GetAsync(siteIdList[0]);
                }

                if (site != null)
                {
                    var channelIdList = await _authManager.GetChannelIdListAsync(site.Id,
                        Constants.ChannelPermissions.ContentAdd);
                    foreach (var permissionChannelId in channelIdList)
                    {
                        var permissionChannelInfo = await _channelRepository.GetAsync(permissionChannelId);
                        if (channelInfo == null || requestChannelId == permissionChannelId)
                        {
                            channelInfo = permissionChannelInfo;
                        }
                        channels.Add(new
                        {
                            permissionChannelInfo.Id,
                            ChannelName = await _channelRepository.GetChannelNameNavigationAsync(site.Id, permissionChannelId)
                        });
                    }

                    siteInfo = new
                    {
                        site.Id,
                        site.SiteName,
                        SiteUrl = _pathManager.GetSiteUrlAsync(site, false)
                    };
                }

                if (channelInfo != null)
                {
                    channel = new
                    {
                        channelInfo.Id,
                        ChannelName = await _channelRepository.GetChannelNameNavigationAsync(site.Id, channelInfo.Id)
                    };
                }
            }

            var config = await _configRepository.GetAsync();
            var user = await _authManager.GetUserAsync();

            return new
            {
                Value = user,
                Config = config,
                Sites = sites,
                Channels = channels,
                Site = siteInfo,
                Channel = channel
            };
        }

        private async Task<object> GetContentAddAsync(int requestSiteId, int requestChannelId, int requestContentId)
        {
            var sites = new List<object>();
            var channels = new List<object>();
            object site = null;
            object channel = null;
            IEnumerable<string> groupNames = null;
            List<string> tagNames = null;
            Content content = null;
            List<TableStyle> styles = null;
            List<KeyValuePair<int, string>> checkedLevels = null;
            var checkedLevel = 0;

            if (await _authManager.IsUserAuthenticatedAsync())
            {
                Site siteInfo = null;
                Channel channelInfo = null;
                var siteIdList = await _authManager.GetSiteIdListAsync();
                foreach (var siteId in siteIdList)
                {
                    var permissionSiteInfo = await _siteRepository.GetAsync(siteId);
                    if (requestSiteId == siteId)
                    {
                        siteInfo = permissionSiteInfo;
                    }
                    sites.Add(new
                    {
                        permissionSiteInfo.Id,
                        permissionSiteInfo.SiteName
                    });
                }

                if (siteInfo == null && siteIdList.Count > 0)
                {
                    siteInfo = await _siteRepository.GetAsync(siteIdList[0]);
                }

                if (siteInfo != null)
                {
                    var channelIdList = await _authManager.GetChannelIdListAsync(siteInfo.Id,
                        Constants.ChannelPermissions.ContentAdd);
                    foreach (var permissionChannelId in channelIdList)
                    {
                        var permissionChannelInfo = await _channelRepository.GetAsync(permissionChannelId);
                        if (channelInfo == null || permissionChannelInfo.Id == requestChannelId)
                        {
                            channelInfo = permissionChannelInfo;
                        }
                        channels.Add(new
                        {
                            permissionChannelInfo.Id,
                            ChannelName = await _channelRepository.GetChannelNameNavigationAsync(siteInfo.Id, permissionChannelId)
                        });
                    }

                    site = new
                    {
                        siteInfo.Id,
                        siteInfo.SiteName,
                        SiteUrl = _pathManager.GetSiteUrlAsync(siteInfo, false)
                    };

                    groupNames = await _contentGroupRepository.GetGroupNamesAsync(siteInfo.Id);
                    tagNames = new List<string>();
                }

                if (channelInfo != null)
                {
                    channel = new
                    {
                        channelInfo.Id,
                        ChannelName = await _channelRepository.GetChannelNameNavigationAsync(siteInfo.Id, channelInfo.Id)
                    };

                    var tableName = _channelRepository.GetTableName(siteInfo, channelInfo);
                    styles = await _tableStyleRepository.GetContentStyleListAsync(channelInfo, tableName);

                    var (userIsChecked, userCheckedLevel) = await CheckManager.GetUserCheckLevelAsync(_authManager, siteInfo, siteInfo.Id);
                    checkedLevels = CheckManager.GetCheckedLevels(siteInfo, userIsChecked, userCheckedLevel, true);

                    if (requestContentId != 0)
                    {
                        //checkedLevels.Insert(0, new KeyValuePair<int, string>(CheckManager.LevelInt.NotChange, CheckManager.Level.NotChange));
                        //checkedLevel = CheckManager.LevelInt.NotChange;

                        content = await _contentRepository.GetAsync(siteInfo, channelInfo, requestContentId);
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

            return new
            {
                Value = user,
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
