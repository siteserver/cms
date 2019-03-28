using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Caches.Content;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Home
{
    [RoutePrefix("home")]
    public class HomeController : ApiController
    {
        private const string Route = "";

        private const string PageNameRegister = "register";
        private const string PageNameIndex = "index";
        private const string PageNameProfile = "profile";
        private const string PageNameContents = "contents";
        private const string PageNameContentAdd = "contentAdd";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var rest = new Rest(Request);
                var pageName = rest.GetQueryString("pageName");

                if (pageName == PageNameRegister)
                {
                    return Ok(GetRegister(rest));
                }
                if (pageName == PageNameIndex)
                {   
                    return Ok(GetIndex(rest));
                }
                if (pageName == PageNameProfile)
                {
                    return Ok(GetProfile(rest));
                }
                if (pageName == PageNameContents)
                {
                    return Ok(GetContents(rest));
                }
                if (pageName == PageNameContentAdd)
                {
                    return Ok(GetContentAdd(rest));
                }

                return Ok(new
                {
                    Value = rest.UserInfo,
                    Config = ConfigManager.Instance
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        public object GetRegister(Rest rest)
        {
            return new
            {
                Value = rest.UserInfo,
                Config = ConfigManager.Instance,
                Styles = TableStyleManager.GetUserStyleInfoList(),
                Groups = UserGroupManager.GetUserGroupInfoList()
            };
        }

        public object GetIndex(Rest rest)
        {
            var menus = new List<object>();
            var defaultPageUrl = string.Empty;

            if (rest.IsUserLoggin)
            {
                var userMenus = UserMenuManager.GetAllUserMenuInfoList();
                foreach (var menuInfo1 in userMenus)
                {
                    if (menuInfo1.Disabled || menuInfo1.ParentId != 0 ||
                        !string.IsNullOrEmpty(menuInfo1.GroupIdCollection) &&
                        !StringUtils.In(menuInfo1.GroupIdCollection, rest.UserInfo.GroupId)) continue;
                    var children = new List<object>();
                    foreach (var menuInfo2 in userMenus)
                    {
                        if (menuInfo2.Disabled || menuInfo2.ParentId != menuInfo1.Id ||
                            !string.IsNullOrEmpty(menuInfo2.GroupIdCollection) &&
                            !StringUtils.In(menuInfo2.GroupIdCollection, rest.UserInfo.GroupId)) continue;

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

                defaultPageUrl = PluginMenuManager.GetHomeDefaultPageUrl();
            }

            return new
            {
                Value = rest.UserInfo,
                Config = ConfigManager.Instance,
                Menus = menus,
                DefaultPageUrl = defaultPageUrl
            };
        }

        public object GetProfile(Rest rest)
        {
            return new
            {
                Value = rest.UserInfo,
                Config = ConfigManager.Instance,
                Styles = TableStyleManager.GetUserStyleInfoList()
            };
        }

        public object GetContents(Rest rest)
        {
            var requestSiteId = rest.SiteId;
            var requestChannelId = rest.ChannelId;

            var sites = new List<object>();
            var channels = new List<object>();
            object site = null;
            object channel = null;

            if (rest.IsUserLoggin)
            {
                SiteInfo siteInfo = null;
                ChannelInfo channelInfo = null;
                var siteIdList = rest.UserPermissionsImpl.GetSiteIdList();
                foreach (var siteId in siteIdList)
                {
                    var permissionSiteInfo = SiteManager.GetSiteInfo(siteId);
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
                    siteInfo = SiteManager.GetSiteInfo(siteIdList[0]);
                }

                if (siteInfo != null)
                {
                    var channelIdList = rest.UserPermissionsImpl.GetChannelIdList(siteInfo.Id,
                        ConfigManager.ChannelPermissions.ContentAdd);
                    foreach (var permissionChannelId in channelIdList)
                    {
                        var permissionChannelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, permissionChannelId);
                        if (channelInfo == null || requestChannelId == permissionChannelId)
                        {
                            channelInfo = permissionChannelInfo;
                        }
                        channels.Add(new
                        {
                            permissionChannelInfo.Id,
                            ChannelName = ChannelManager.GetChannelNameNavigation(siteInfo.Id, permissionChannelId)
                        });
                    }

                    site = new
                    {
                        siteInfo.Id,
                        siteInfo.SiteName,
                        SiteUrl = PageUtility.GetSiteUrl(siteInfo, false)
                    };
                }

                if (channelInfo != null)
                {
                    channel = new
                    {
                        channelInfo.Id,
                        ChannelName = ChannelManager.GetChannelNameNavigation(siteInfo.Id, channelInfo.Id)
                    };
                }
            }

            return new
            {
                Value = rest.UserInfo,
                Config = ConfigManager.Instance,
                Sites = sites,
                Channels = channels,
                Site = site,
                Channel = channel
            };
        }

        public object GetContentAdd(Rest rest)
        {
            var requestSiteId = rest.SiteId;
            var requestChannelId = rest.ChannelId;
            var requestContentId = rest.ContentId;

            var sites = new List<object>();
            var channels = new List<object>();
            object site = null;
            object channel = null;
            List<string> groupNames = null;
            List<string> tagNames = null;
            ContentInfo contentInfo = null;
            List<TableStyleInfo> styles = null;
            List<KeyValuePair<int, string>> checkedLevels = null;
            var checkedLevel = 0;

            if (rest.IsUserLoggin)
            {
                SiteInfo siteInfo = null;
                ChannelInfo channelInfo = null;
                var siteIdList = rest.UserPermissionsImpl.GetSiteIdList();
                foreach (var siteId in siteIdList)
                {
                    var permissionSiteInfo = SiteManager.GetSiteInfo(siteId);
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
                    siteInfo = SiteManager.GetSiteInfo(siteIdList[0]);
                }

                if (siteInfo != null)
                {
                    var channelIdList = rest.UserPermissionsImpl.GetChannelIdList(siteInfo.Id,
                        ConfigManager.ChannelPermissions.ContentAdd);
                    foreach (var permissionChannelId in channelIdList)
                    {
                        var permissionChannelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, permissionChannelId);
                        if (channelInfo == null || permissionChannelInfo.Id == requestChannelId)
                        {
                            channelInfo = permissionChannelInfo;
                        }
                        channels.Add(new
                        {
                            permissionChannelInfo.Id,
                            ChannelName = ChannelManager.GetChannelNameNavigation(siteInfo.Id, permissionChannelId)
                        });
                    }

                    site = new
                    {
                        siteInfo.Id,
                        siteInfo.SiteName,
                        SiteUrl = PageUtility.GetSiteUrl(siteInfo, false)
                    };

                    groupNames = ContentGroupManager.GetGroupNameList(siteInfo.Id);
                    tagNames = ContentTagManager.GetTagNameList(siteInfo.Id);
                }

                if (channelInfo != null)
                {
                    channel = new
                    {
                        channelInfo.Id,
                        ChannelName = ChannelManager.GetChannelNameNavigation(siteInfo.Id, channelInfo.Id)
                    };

                    styles = TableStyleManager.GetContentStyleInfoList(siteInfo, channelInfo);

                    var checkKeyValuePair = CheckManager.GetUserCheckLevel(rest.AdminPermissionsImpl, siteInfo, siteInfo.Id);
                    checkedLevels = CheckManager.GetCheckedLevels(siteInfo, checkKeyValuePair.Key, checkedLevel, true);

                    if (requestContentId != 0)
                    {
                        checkedLevels.Insert(0, new KeyValuePair<int, string>(CheckManager.LevelInt.NotChange, CheckManager.Level.NotChange));
                        checkedLevel = CheckManager.LevelInt.NotChange;

                        contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, requestContentId);
                        if (contentInfo != null &&
                            (contentInfo.SiteId != siteInfo.Id || contentInfo.ChannelId != channelInfo.Id))
                        {
                            contentInfo = null;
                        }
                    }
                    else
                    {
                        contentInfo = new ContentInfo
                        {
                            Id = 0,
                            SiteId = siteInfo.Id,
                            ChannelId = channelInfo.Id,
                            AddDate = DateTime.Now
                        };
                    }
                }
            }

            return new
            {
                Value = rest.UserInfo,
                Config = ConfigManager.Instance,
                Sites = sites,
                Channels = channels,
                Site = site,
                Channel = channel,
                AllGroupNames = groupNames,
                AllTagNames = tagNames,
                Styles = styles,
                CheckedLevels = checkedLevels,
                CheckedLevel = checkedLevel,
                Content = contentInfo,
            };
        }
    }
}
