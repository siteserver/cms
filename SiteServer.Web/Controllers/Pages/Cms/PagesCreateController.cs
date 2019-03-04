using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [RoutePrefix("pages/cms/create")]
    public class PagesCreateController : ApiController
    {
        private const string Route = "";
        private const string RouteAll = "all";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetList()
        {
            try
            {
                var request = new RequestImpl();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSitePermissions(request.SiteId, ConfigManager.WebSitePermissions.Create))
                {
                    return Unauthorized();
                }

                var siteId = request.SiteId;
                var parentId = request.GetQueryInt("parentId");
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                var parent = ChannelManager.GetChannelInfo(siteId, parentId);
                var countDict = new Dictionary<int, int>();
                countDict[parent.Id] = ContentManager.GetCount(siteInfo, parent, true);

                var channelInfoList = new List<ChannelInfo>();

                var channelIdList =
                    ChannelManager.GetChannelIdList(parent, EScopeType.Children, string.Empty, string.Empty, string.Empty);

                foreach (var channelId in channelIdList)
                {
                    var enabled = request.AdminPermissionsImpl.IsOwningChannelId(channelId);
                    
                    if (!enabled)
                    {
                        if (!request.AdminPermissionsImpl.IsDescendantOwningChannelId(siteId, channelId)) continue;
                    }

                    var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                    channelInfoList.Add(channelInfo);
                    countDict[channelInfo.Id] = ContentManager.GetCount(siteInfo, channelInfo, true);
                }

                return Ok(new
                {
                    Value = channelInfoList,
                    Parent = parent,
                    CountDict = countDict
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public class CreateParameter
        {
            public int SiteId { get; set; }
            public IEnumerable<int> ChannelIdList { get; set; }
            public bool IsAllChecked { get; set; }
            public bool IsDescendent { get; set; }
            public bool IsChannelPage { get; set; }
            public bool IsContentPage { get; set; }
            public string Scope { get; set; }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Create([FromBody] CreateParameter parameter)
        {
            try
            {
                var request = new RequestImpl();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSitePermissions(parameter.SiteId, ConfigManager.WebSitePermissions.Create))
                {
                    return Unauthorized();
                }

                var selectedChannelIdList = new List<int>();

                if (parameter.IsAllChecked)
                {
                    selectedChannelIdList = ChannelManager.GetChannelIdList(parameter.SiteId);
                }
                else if (parameter.IsDescendent)
                {
                    foreach (var channelId in parameter.ChannelIdList)
                    {
                        selectedChannelIdList.Add(channelId);

                        var channelInfo = ChannelManager.GetChannelInfo(parameter.SiteId, channelId);
                        if (channelInfo.ChildrenCount > 0)
                        {
                            var descendentIdList = ChannelManager.GetChannelIdList(channelInfo, EScopeType.Descendant);
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
                    selectedChannelIdList.AddRange(parameter.ChannelIdList);
                }

                var channelIdList = new List<int>();

                if (parameter.Scope == "1month" || parameter.Scope == "1day" || parameter.Scope == "2hours")
                {
                    var siteInfo = SiteManager.GetSiteInfo(parameter.SiteId);
                    var tableName = siteInfo.TableName;

                    if (parameter.Scope == "1month")
                    {
                        var lastEditList = DataProvider.ContentDao.GetChannelIdListCheckedByLastEditDateHour(tableName, parameter.SiteId, 720);
                        foreach (var channelId in lastEditList)
                        {
                            if (selectedChannelIdList.Contains(channelId))
                            {
                                channelIdList.Add(channelId);
                            }
                        }
                    }
                    else if (parameter.Scope == "1day")
                    {
                        var lastEditList = DataProvider.ContentDao.GetChannelIdListCheckedByLastEditDateHour(tableName, parameter.SiteId, 24);
                        foreach (var channelId in lastEditList)
                        {
                            if (selectedChannelIdList.Contains(channelId))
                            {
                                channelIdList.Add(channelId);
                            }
                        }
                    }
                    else if (parameter.Scope == "2hours")
                    {
                        var lastEditList = DataProvider.ContentDao.GetChannelIdListCheckedByLastEditDateHour(tableName, parameter.SiteId, 2);
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
                    if (parameter.IsChannelPage)
                    {
                        CreateManager.CreateChannel(parameter.SiteId, channelId);
                    }
                    if (parameter.IsContentPage)
                    {
                        CreateManager.CreateAllContent(parameter.SiteId, channelId);
                    }
                }

                return Ok(new { });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteAll)]
        public IHttpActionResult CreateAll([FromBody] CreateParameter parameter)
        {
            try
            {
                var request = new RequestImpl();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSitePermissions(parameter.SiteId, ConfigManager.WebSitePermissions.Create))
                {
                    return Unauthorized();
                }

                CreateManager.CreateByAll(parameter.SiteId);

                return Ok(new { });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
