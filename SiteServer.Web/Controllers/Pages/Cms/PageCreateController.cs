using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [RoutePrefix("api/pages/cms/create")]
    public class PageCreateController : ApiController
    {
        private const string Route = "";
        private const string RouteAll = "all";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetList()
        {
            try
            {
                var request = new AuthRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(request.SiteId, ConfigManager.WebSitePermissions.Create))
                {
                    return Unauthorized();
                }

                var siteId = request.SiteId;
                var parentId = request.GetQueryInt("parentId");
                var parent = ChannelManager.GetChannelInfo(siteId, parentId);

                var channelInfoList = new List<ChannelInfo>();

                var channelIdList =
                    ChannelManager.GetChannelIdList(parent, EScopeType.Children, string.Empty, string.Empty, string.Empty);

                foreach (var channelId in channelIdList)
                {
                    var enabled = request.AdminPermissions.IsOwningChannelId(channelId);
                    
                    if (!enabled)
                    {
                        if (!request.AdminPermissions.IsDescendantOwningChannelId(siteId, channelId)) continue;
                    }

                    var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);

                    channelInfoList.Add(channelInfo);
                }

                return Ok(new
                {
                    Value = channelInfoList,
                    Parent = parent
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
                var request = new AuthRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(parameter.SiteId, ConfigManager.WebSitePermissions.Create))
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
                var request = new AuthRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(parameter.SiteId, ConfigManager.WebSitePermissions.Create))
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
