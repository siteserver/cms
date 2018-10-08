using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Home
{
    [RoutePrefix("api/home")]
    public class HomeController : ApiController
    {
        private const string Route = "";

        private const string PageNameRegister = "register";
        private const string PageNameProfile = "profile";
        private const string PageNameIndex = "index";
        private const string PageNameContents = "contents";
        private const string PageNameContentAdd = "contentAdd";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var request = new RequestImpl();
                var pageName = request.GetQueryString("pageName");

                if (pageName == PageNameRegister || pageName == PageNameProfile)
                {
                    return Ok(GetRegisterProfile(request));
                }
                if (pageName == PageNameIndex)
                {   
                    return Ok(GetIndex(request));
                }
                if (pageName == PageNameContents)
                {
                    return Ok(GetContents(request));
                }
                if (pageName == PageNameContentAdd)
                {
                    return Ok(GetContentAdd(request));
                }

                return Ok(new
                {
                    Value = ConfigManager.Instance.SystemConfigInfo,
                    request.IsUserLoggin
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        public object GetRegisterProfile(RequestImpl request)
        {
            return new
            {
                Value = ConfigManager.Instance.SystemConfigInfo,
                request.IsUserLoggin,
                Styles = TableStyleManager.GetUserStyleInfoList()
            };
        }

        public object GetIndex(RequestImpl request)
        {
            var menus = new List<object>();
            if (request.IsUserLoggin)
            {
                var userMenus = UserMenuManager.GetAllUserMenuInfoList();
                foreach (var menuInfo1 in userMenus)
                {
                    if (menuInfo1.IsDisabled || menuInfo1.ParentId != 0 ||
                        !string.IsNullOrEmpty(menuInfo1.GroupIdCollection) &&
                        !StringUtils.In(menuInfo1.GroupIdCollection, request.UserInfo.GroupId)) continue;
                    var children = new List<object>();
                    foreach (var menuInfo2 in userMenus)
                    {
                        if (menuInfo2.IsDisabled || menuInfo2.ParentId != menuInfo1.Id ||
                            !string.IsNullOrEmpty(menuInfo2.GroupIdCollection) &&
                            !StringUtils.In(menuInfo2.GroupIdCollection, request.UserInfo.GroupId)) continue;

                        children.Add(new
                        {
                            menuInfo2.Title,
                            menuInfo2.Url,
                            menuInfo2.IconClass,
                            menuInfo2.IsOpenWindow
                        });
                    }

                    menus.Add(new
                    {
                        menuInfo1.Title,
                        menuInfo1.Url,
                        menuInfo1.IconClass,
                        menuInfo1.IsOpenWindow,
                        Menus = children
                    });
                }
            }

            return new
            {
                Value = ConfigManager.Instance.SystemConfigInfo,
                request.IsUserLoggin,
                Menus = menus
            };
        }

        public object GetContents(RequestImpl request)
        {
            var requestSiteId = request.SiteId;
            var sites = new List<object>();
            var channels = new List<object>();
            object site = null;
            object channel = null;

            if (request.IsUserLoggin)
            {
                SiteInfo siteInfo = null;
                ChannelInfo channelInfo = null;
                var siteIdList = request.UserPermissionsImpl.GetSiteIdList();
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
                    var channelIdList = request.UserPermissionsImpl.GetChannelIdList(siteInfo.Id,
                        ConfigManager.ChannelPermissions.ContentAdd);
                    foreach (var permissionChannelId in channelIdList)
                    {
                        var permissionChannelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, permissionChannelId);
                        if (channelInfo == null)
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
                Value = ConfigManager.Instance.SystemConfigInfo,
                request.IsUserLoggin,
                Sites = sites,
                Channels = channels,
                Site = site,
                Channel = channel
            };
        }

        public object GetContentAdd(RequestImpl request)
        {
            var requestSiteId = request.SiteId;
            var sites = new List<object>();
            var channels = new List<object>();
            object site = null;
            object channel = null;
            List<TableStyleInfo> styles = null;

            if (request.IsUserLoggin)
            {
                SiteInfo siteInfo = null;
                ChannelInfo channelInfo = null;
                var siteIdList = request.UserPermissionsImpl.GetSiteIdList();
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
                    var channelIdList = request.UserPermissionsImpl.GetChannelIdList(siteInfo.Id,
                        ConfigManager.ChannelPermissions.ContentAdd);
                    foreach (var permissionChannelId in channelIdList)
                    {
                        var permissionChannelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, permissionChannelId);
                        if (channelInfo == null)
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

                    styles = TableStyleManager.GetContentStyleInfoList(siteInfo, channelInfo);
                }
            }

            return new
            {
                Value = ConfigManager.Instance.SystemConfigInfo,
                request.IsUserLoggin,
                Sites = sites,
                Channels = channels,
                Site = site,
                Channel = channel,
                Styles = styles
            };
        }
    }
}
