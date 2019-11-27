using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
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
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                var pageName = request.GetQueryString("pageName");

                if (pageName == PageNameRegister)
                {
                    return Ok(await GetRegisterAsync(request));
                }
                if (pageName == PageNameIndex)
                {   
                    return Ok(await GetIndexAsync(request));
                }
                if (pageName == PageNameProfile)
                {
                    return Ok(await GetProfileAsync(request));
                }
                if (pageName == PageNameContents)
                {
                    return Ok(await GetContentsAsync(request));
                }
                if (pageName == PageNameContentAdd)
                {
                    return Ok(await GetContentAddAsync(request));
                }

                var config = await DataProvider.ConfigDao.GetAsync();

                return Ok(new
                {
                    Value = request.User,
                    Config = config
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        private async Task<object> GetRegisterAsync(AuthenticatedRequest request)
        {
            var config = await DataProvider.ConfigDao.GetAsync();

            return new
            {
                Value = request.User,
                Config = config,
                Styles = await TableStyleManager.GetUserStyleListAsync(),
                Groups = await UserGroupManager.GetUserGroupListAsync()
            };
        }

        private async Task<object> GetIndexAsync(AuthenticatedRequest request)
        {
            var menus = new List<object>();
            var defaultPageUrl = string.Empty;

            if (request.IsUserLoggin)
            {
                var userMenus = await UserMenuManager.GetAllUserMenuListAsync();
                foreach (var menuInfo1 in userMenus)
                {
                    if (menuInfo1.Disabled || menuInfo1.ParentId != 0 ||
                        menuInfo1.GroupIds.Contains(request.User.GroupId)) continue;
                    var children = new List<object>();
                    foreach (var menuInfo2 in userMenus)
                    {
                        if (menuInfo2.Disabled || menuInfo2.ParentId != menuInfo1.Id ||
                            menuInfo2.GroupIds.Contains(request.User.GroupId)) continue;

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

            var config = await DataProvider.ConfigDao.GetAsync();

            return new
            {
                Value = request.User,
                Config = config,
                Menus = menus,
                DefaultPageUrl = defaultPageUrl
            };
        }

        private async Task<object> GetProfileAsync(AuthenticatedRequest request)
        {
            var config = await DataProvider.ConfigDao.GetAsync();

            return new
            {
                Value = request.User,
                Config = config,
                Styles = await TableStyleManager.GetUserStyleListAsync()
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
                Channel channelInfo = null;
                var siteIdList = await request.UserPermissionsImpl.GetSiteIdListAsync();
                foreach (var siteId in siteIdList)
                {
                    var permissionSite = await DataProvider.SiteDao.GetAsync(siteId);
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
                    site = await DataProvider.SiteDao.GetAsync(siteIdList[0]);
                }

                if (site != null)
                {
                    var channelIdList = await request.UserPermissionsImpl.GetChannelIdListAsync(site.Id,
                        Constants.ChannelPermissions.ContentAdd);
                    foreach (var permissionChannelId in channelIdList)
                    {
                        var permissionChannelInfo = await ChannelManager.GetChannelAsync(site.Id, permissionChannelId);
                        if (channelInfo == null || requestChannelId == permissionChannelId)
                        {
                            channelInfo = permissionChannelInfo;
                        }
                        channels.Add(new
                        {
                            permissionChannelInfo.Id,
                            ChannelName = await ChannelManager.GetChannelNameNavigationAsync(site.Id, permissionChannelId)
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
                        ChannelName = await ChannelManager.GetChannelNameNavigationAsync(site.Id, channelInfo.Id)
                    };
                }
            }

            var config = await DataProvider.ConfigDao.GetAsync();

            return new
            {
                Value = request.User,
                Config = config,
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
            object site = null;
            object channel = null;
            IEnumerable<string> groupNames = null;
            List<string> tagNames = null;
            Content content = null;
            List<TableStyle> styles = null;
            List<KeyValuePair<int, string>> checkedLevels = null;
            var checkedLevel = 0;

            if (request.IsUserLoggin)
            {
                Site siteInfo = null;
                Channel channelInfo = null;
                var siteIdList = await request.UserPermissionsImpl.GetSiteIdListAsync();
                foreach (var siteId in siteIdList)
                {
                    var permissionSiteInfo = await DataProvider.SiteDao.GetAsync(siteId);
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
                    siteInfo = await DataProvider.SiteDao.GetAsync(siteIdList[0]);
                }

                if (siteInfo != null)
                {
                    var channelIdList = await request.UserPermissionsImpl.GetChannelIdListAsync(siteInfo.Id,
                        Constants.ChannelPermissions.ContentAdd);
                    foreach (var permissionChannelId in channelIdList)
                    {
                        var permissionChannelInfo = await ChannelManager.GetChannelAsync(siteInfo.Id, permissionChannelId);
                        if (channelInfo == null || permissionChannelInfo.Id == requestChannelId)
                        {
                            channelInfo = permissionChannelInfo;
                        }
                        channels.Add(new
                        {
                            permissionChannelInfo.Id,
                            ChannelName = await ChannelManager.GetChannelNameNavigationAsync(siteInfo.Id, permissionChannelId)
                        });
                    }

                    site = new
                    {
                        siteInfo.Id,
                        siteInfo.SiteName,
                        SiteUrl = PageUtility.GetSiteUrl(siteInfo, false)
                    };

                    groupNames = await DataProvider.ContentGroupDao.GetGroupNamesAsync(siteInfo.Id);
                    tagNames = new List<string>();
                }

                if (channelInfo != null)
                {
                    channel = new
                    {
                        channelInfo.Id,
                        ChannelName = await ChannelManager.GetChannelNameNavigationAsync(siteInfo.Id, channelInfo.Id)
                    };

                    styles = await TableStyleManager.GetContentStyleListAsync(siteInfo, channelInfo);

                    var (userIsChecked, userCheckedLevel) = await CheckManager.GetUserCheckLevelAsync(request.AdminPermissionsImpl, siteInfo, siteInfo.Id);
                    checkedLevels = CheckManager.GetCheckedLevels(siteInfo, userIsChecked, userCheckedLevel, true);

                    if (requestContentId != 0)
                    {
                        //checkedLevels.Insert(0, new KeyValuePair<int, string>(CheckManager.LevelInt.NotChange, CheckManager.Level.NotChange));
                        //checkedLevel = CheckManager.LevelInt.NotChange;

                        content = await DataProvider.ContentDao.GetAsync(siteInfo, channelInfo, requestContentId);
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

            var config = await DataProvider.ConfigDao.GetAsync();

            return new
            {
                Value = request.User,
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
