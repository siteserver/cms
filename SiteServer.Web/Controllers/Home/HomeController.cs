using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Db;
using SiteServer.CMS.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Home
{
    [OpenApiIgnore]
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
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = new AuthenticatedRequest();
                var pageName = request.GetQueryString("pageName");

                if (pageName == PageNameRegister)
                {
                    return Ok(GetRegister(request));
                }
                if (pageName == PageNameIndex)
                {   
                    return Ok(GetIndex(request));
                }
                if (pageName == PageNameProfile)
                {
                    return Ok(GetProfile(request));
                }
                if (pageName == PageNameContents)
                {
                    return Ok(await GetContentsAsync(request));
                }
                if (pageName == PageNameContentAdd)
                {
                    return Ok(await GetContentAddAsync(request));
                }

                return Ok(new
                {
                    Value = request.User,
                    Config = ConfigManager.Instance.SystemConfigInfo
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        private object GetRegister(AuthenticatedRequest request)
        {
            return new
            {
                Value = request.User,
                Config = ConfigManager.Instance.SystemConfigInfo,
                Styles = TableStyleManager.GetUserStyleInfoList(),
                Groups = UserGroupManager.GetUserGroupInfoList()
            };
        }

        private object GetIndex(AuthenticatedRequest request)
        {
            var menus = new List<object>();
            var defaultPageUrl = string.Empty;

            if (request.IsUserLoggin)
            {
                var userMenus = UserMenuManager.GetAllUserMenuInfoList();
                foreach (var menuInfo1 in userMenus)
                {
                    if (menuInfo1.IsDisabled || menuInfo1.ParentId != 0 ||
                        !string.IsNullOrEmpty(menuInfo1.GroupIdCollection) &&
                        !StringUtils.In(menuInfo1.GroupIdCollection, request.User.GroupId)) continue;
                    var children = new List<object>();
                    foreach (var menuInfo2 in userMenus)
                    {
                        if (menuInfo2.IsDisabled || menuInfo2.ParentId != menuInfo1.Id ||
                            !string.IsNullOrEmpty(menuInfo2.GroupIdCollection) &&
                            !StringUtils.In(menuInfo2.GroupIdCollection, request.User.GroupId)) continue;

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
                Value = request.User,
                Config = ConfigManager.Instance.SystemConfigInfo,
                Menus = menus,
                DefaultPageUrl = defaultPageUrl
            };
        }

        private object GetProfile(AuthenticatedRequest request)
        {
            return new
            {
                Value = request.User,
                Config = ConfigManager.Instance.SystemConfigInfo,
                Styles = TableStyleManager.GetUserStyleInfoList()
            };
        }

        private async Task<object> GetContentsAsync(AuthenticatedRequest request)
        {
            var requestSiteId = request.SiteId;
            var requestChannelId = request.ChannelId;

            var sites = new List<object>();
            var channels = new List<object>();
            object siteInfo = null;
            object channel = null;

            if (request.IsUserLoggin)
            {
                Site site = null;
                ChannelInfo channelInfo = null;
                var siteIdList = request.UserPermissionsImpl.GetSiteIdList();
                foreach (var siteId in siteIdList)
                {
                    var permissionSite = await SiteManager.GetSiteAsync(siteId);
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
                    site = await SiteManager.GetSiteAsync(siteIdList[0]);
                }

                if (site != null)
                {
                    var channelIdList = request.UserPermissionsImpl.GetChannelIdList(site.Id,
                        ConfigManager.ChannelPermissions.ContentAdd);
                    foreach (var permissionChannelId in channelIdList)
                    {
                        var permissionChannelInfo = ChannelManager.GetChannelInfo(site.Id, permissionChannelId);
                        if (channelInfo == null || requestChannelId == permissionChannelId)
                        {
                            channelInfo = permissionChannelInfo;
                        }
                        channels.Add(new
                        {
                            permissionChannelInfo.Id,
                            ChannelName = ChannelManager.GetChannelNameNavigation(site.Id, permissionChannelId)
                        });
                    }

                    siteInfo = new
                    {
                        site.Id,
                        site.SiteName,
                        SiteUrl = PageUtility.GetSiteUrl(site, false)
                    };
                }

                if (channelInfo != null)
                {
                    channel = new
                    {
                        channelInfo.Id,
                        ChannelName = ChannelManager.GetChannelNameNavigation(site.Id, channelInfo.Id)
                    };
                }
            }

            return new
            {
                Value = request.User,
                Config = ConfigManager.Instance.SystemConfigInfo,
                Sites = sites,
                Channels = channels,
                Site = siteInfo,
                Channel = channel
            };
        }

        private async Task<object> GetContentAddAsync(AuthenticatedRequest request)
        {
            var requestSiteId = request.SiteId;
            var requestChannelId = request.ChannelId;
            var requestContentId = request.ContentId;

            var sites = new List<object>();
            var channels = new List<object>();
            object siteInfo = null;
            object channel = null;
            List<string> groupNames = null;
            List<string> tagNames = null;
            ContentInfo contentInfo = null;
            List<TableStyleInfo> styles = null;
            List<KeyValuePair<int, string>> checkedLevels = null;
            var checkedLevel = 0;

            if (request.IsUserLoggin)
            {
                Site site = null;
                ChannelInfo channelInfo = null;
                var siteIdList = request.UserPermissionsImpl.GetSiteIdList();
                foreach (var siteId in siteIdList)
                {
                    var permissionSite = await SiteManager.GetSiteAsync(siteId);
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
                    site = await SiteManager.GetSiteAsync(siteIdList[0]);
                }

                if (site != null)
                {
                    var channelIdList = request.UserPermissionsImpl.GetChannelIdList(site.Id,
                        ConfigManager.ChannelPermissions.ContentAdd);
                    foreach (var permissionChannelId in channelIdList)
                    {
                        var permissionChannelInfo = ChannelManager.GetChannelInfo(site.Id, permissionChannelId);
                        if (channelInfo == null || permissionChannelInfo.Id == requestChannelId)
                        {
                            channelInfo = permissionChannelInfo;
                        }
                        channels.Add(new
                        {
                            permissionChannelInfo.Id,
                            ChannelName = ChannelManager.GetChannelNameNavigation(site.Id, permissionChannelId)
                        });
                    }

                    siteInfo = new
                    {
                        site.Id,
                        site.SiteName,
                        SiteUrl = PageUtility.GetSiteUrl(site, false)
                    };

                    groupNames = ContentGroupManager.GetGroupNameList(site.Id);
                    tagNames = new List<string>();
                }

                if (channelInfo != null)
                {
                    channel = new
                    {
                        channelInfo.Id,
                        ChannelName = ChannelManager.GetChannelNameNavigation(site.Id, channelInfo.Id)
                    };

                    styles = TableStyleManager.GetContentStyleInfoList(site, channelInfo);

                    var checkKeyValuePair = CheckManager.GetUserCheckLevel(request.AdminPermissionsImpl, site, site.Id);
                    checkedLevels = CheckManager.GetCheckedLevels(site, checkKeyValuePair.Key, checkedLevel, true);

                    if (requestContentId != 0)
                    {
                        //checkedLevels.Insert(0, new KeyValuePair<int, string>(CheckManager.LevelInt.NotChange, CheckManager.Level.NotChange));
                        //checkedLevel = CheckManager.LevelInt.NotChange;

                        contentInfo = ContentManager.GetContentInfo(site, channelInfo, requestContentId);
                        if (contentInfo != null &&
                            (contentInfo.SiteId != site.Id || contentInfo.ChannelId != channelInfo.Id))
                        {
                            contentInfo = null;
                        }
                    }
                    else
                    {
                        contentInfo = new ContentInfo(new
                        {
                            Id = 0,
                            SiteId = site.Id,
                            ChannelId = channelInfo.Id,
                            AddDate = DateTime.Now
                        });
                    }
                }
            }

            return new
            {
                Value = request.User,
                Config = ConfigManager.Instance.SystemConfigInfo,
                Sites = sites,
                Channels = channels,
                Site = siteInfo,
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
