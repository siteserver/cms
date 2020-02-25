using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Core;
using SS.CMS.Framework;
using SS.CMS.Plugins;

namespace SS.CMS.Web.Controllers.Home
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

        public IndexController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<object>> GetConfig([FromQuery]GetRequest request)
        {
            var auth = await _authManager.GetUserAsync();

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

            var config = await DataProvider.ConfigRepository.GetAsync();

            return new
            {
                Value = auth.User,
                Config = config
            };
        }

        private async Task<object> GetRegisterAsync()
        {
            var config = await DataProvider.ConfigRepository.GetAsync();

            return new
            {
                Value = _authManager.User,
                Config = config,
                Styles = await DataProvider.TableStyleRepository.GetUserStyleListAsync(),
                Groups = await DataProvider.UserGroupRepository.GetUserGroupListAsync()
            };
        }

        private async Task<object> GetIndexAsync()
        {
            var menus = new List<object>();
            var defaultPageUrl = string.Empty;

            if (_authManager.IsUserLoggin)
            {
                var userMenus = await DataProvider.UserMenuRepository.GetUserMenuListAsync();
                foreach (var menuInfo1 in userMenus)
                {
                    if (menuInfo1.Disabled || menuInfo1.ParentId != 0 ||
                        menuInfo1.GroupIds.Contains(_authManager.User.GroupId)) continue;
                    var children = new List<object>();
                    foreach (var menuInfo2 in userMenus)
                    {
                        if (menuInfo2.Disabled || menuInfo2.ParentId != menuInfo1.Id ||
                            menuInfo2.GroupIds.Contains(_authManager.User.GroupId)) continue;

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

                defaultPageUrl = await PluginMenuManager.GetHomeDefaultPageUrlAsync();
            }

            var config = await DataProvider.ConfigRepository.GetAsync();

            return new
            {
                Value = _authManager.User,
                Config = config,
                Menus = menus,
                DefaultPageUrl = defaultPageUrl
            };
        }

        private async Task<object> GetProfileAsync()
        {
            var config = await DataProvider.ConfigRepository.GetAsync();

            return new
            {
                Value = _authManager.User,
                Config = config,
                Styles = await DataProvider.TableStyleRepository.GetUserStyleListAsync()
            };
        }

        private async Task<object> GetContentsAsync(int requestSiteId, int requestChannelId)
        {
            var sites = new List<object>();
            var channels = new List<object>();
            object siteInfo = null;
            object channel = null;

            if (_authManager.IsUserLoggin)
            {
                Site site = null;
                Channel channelInfo = null;
                var siteIdList = await _authManager.UserPermissions.GetSiteIdListAsync();
                foreach (var siteId in siteIdList)
                {
                    var permissionSite = await DataProvider.SiteRepository.GetAsync(siteId);
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
                    site = await DataProvider.SiteRepository.GetAsync(siteIdList[0]);
                }

                if (site != null)
                {
                    var channelIdList = await _authManager.UserPermissions.GetChannelIdListAsync(site.Id,
                        Constants.ChannelPermissions.ContentAdd);
                    foreach (var permissionChannelId in channelIdList)
                    {
                        var permissionChannelInfo = await DataProvider.ChannelRepository.GetAsync(permissionChannelId);
                        if (channelInfo == null || requestChannelId == permissionChannelId)
                        {
                            channelInfo = permissionChannelInfo;
                        }
                        channels.Add(new
                        {
                            permissionChannelInfo.Id,
                            ChannelName = await DataProvider.ChannelRepository.GetChannelNameNavigationAsync(site.Id, permissionChannelId)
                        });
                    }

                    siteInfo = new
                    {
                        site.Id,
                        site.SiteName,
                        SiteUrl = PageUtility.GetSiteUrlAsync(site, false)
                    };
                }

                if (channelInfo != null)
                {
                    channel = new
                    {
                        channelInfo.Id,
                        ChannelName = await DataProvider.ChannelRepository.GetChannelNameNavigationAsync(site.Id, channelInfo.Id)
                    };
                }
            }

            var config = await DataProvider.ConfigRepository.GetAsync();

            return new
            {
                Value = _authManager.User,
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

            if (_authManager.IsUserLoggin)
            {
                Site siteInfo = null;
                Channel channelInfo = null;
                var siteIdList = await _authManager.UserPermissions.GetSiteIdListAsync();
                foreach (var siteId in siteIdList)
                {
                    var permissionSiteInfo = await DataProvider.SiteRepository.GetAsync(siteId);
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
                    siteInfo = await DataProvider.SiteRepository.GetAsync(siteIdList[0]);
                }

                if (siteInfo != null)
                {
                    var channelIdList = await _authManager.UserPermissions.GetChannelIdListAsync(siteInfo.Id,
                        Constants.ChannelPermissions.ContentAdd);
                    foreach (var permissionChannelId in channelIdList)
                    {
                        var permissionChannelInfo = await DataProvider.ChannelRepository.GetAsync(permissionChannelId);
                        if (channelInfo == null || permissionChannelInfo.Id == requestChannelId)
                        {
                            channelInfo = permissionChannelInfo;
                        }
                        channels.Add(new
                        {
                            permissionChannelInfo.Id,
                            ChannelName = await DataProvider.ChannelRepository.GetChannelNameNavigationAsync(siteInfo.Id, permissionChannelId)
                        });
                    }

                    site = new
                    {
                        siteInfo.Id,
                        siteInfo.SiteName,
                        SiteUrl = PageUtility.GetSiteUrlAsync(siteInfo, false)
                    };

                    groupNames = await DataProvider.ContentGroupRepository.GetGroupNamesAsync(siteInfo.Id);
                    tagNames = new List<string>();
                }

                if (channelInfo != null)
                {
                    channel = new
                    {
                        channelInfo.Id,
                        ChannelName = await DataProvider.ChannelRepository.GetChannelNameNavigationAsync(siteInfo.Id, channelInfo.Id)
                    };

                    var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(siteInfo, channelInfo);
                    styles = await DataProvider.TableStyleRepository.GetContentStyleListAsync(channelInfo, tableName);

                    var (userIsChecked, userCheckedLevel) = await CheckManager.GetUserCheckLevelAsync(_authManager.AdminPermissions, siteInfo, siteInfo.Id);
                    checkedLevels = CheckManager.GetCheckedLevels(siteInfo, userIsChecked, userCheckedLevel, true);

                    if (requestContentId != 0)
                    {
                        //checkedLevels.Insert(0, new KeyValuePair<int, string>(CheckManager.LevelInt.NotChange, CheckManager.Level.NotChange));
                        //checkedLevel = CheckManager.LevelInt.NotChange;

                        content = await DataProvider.ContentRepository.GetAsync(siteInfo, channelInfo, requestContentId);
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

            var config = await DataProvider.ConfigRepository.GetAsync();

            return new
            {
                Value = _authManager.User,
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
