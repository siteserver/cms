using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.V1
{
    [RoutePrefix("v1/contents")]
    public partial class ContentsController : ApiController
    {
        private const string Route = "";
        private const string RouteCheck = "check";
        private const string RouteChannel = "{siteId:int}/{channelId:int}";
        private const string RouteContent = "{siteId:int}/{channelId:int}/{id:int}";

        [OpenApiOperation("获取内容列表 API", "https://sscms.com/docs/v6/api/guide/contents/list.html")]
        [HttpPost, Route(Route)]
        public QueryResult GetContents([FromBody] QueryRequest request)
        {
            var req = new AuthenticatedRequest();
            var sourceId = req.GetPostInt(ContentAttribute.SourceId.ToCamelCase());
            var channelId = request.ChannelId ?? request.SiteId;

            bool isAuth;
            if (sourceId == SourceManager.User)
            {
                isAuth = req.IsUserLoggin && req.UserPermissions.HasChannelPermissions(request.SiteId, channelId, ConfigManager.ChannelPermissions.ContentView);
            }
            else
            {
                isAuth = req.IsApiAuthenticated &&
                         AccessTokenManager.IsScope(req.ApiToken, AccessTokenManager.ScopeContents) ||
                         req.IsUserLoggin &&
                         req.UserPermissions.HasChannelPermissions(request.SiteId, channelId,
                             ConfigManager.ChannelPermissions.ContentView) ||
                         req.IsAdminLoggin &&
                         req.AdminPermissions.HasChannelPermissions(request.SiteId, channelId,
                             ConfigManager.ChannelPermissions.ContentView);
            }
            if (!isAuth) return Request.Unauthorized<QueryResult>();

            var site = SiteManager.GetSiteInfo(request.SiteId);
            if (site == null) return Request.BadRequest<QueryResult>("无法确定内容对应的站点");

            var channelInfo = ChannelManager.GetChannelInfo(request.SiteId, channelId);
            if (channelInfo == null) return Request.BadRequest<QueryResult>("无法确定内容对应的栏目");

            if (!req.AdminPermissionsImpl.HasChannelPermissions(request.SiteId, channelId,
                ConfigManager.ChannelPermissions.ContentView)) return Request.Unauthorized<QueryResult>();

            var tableName = site.TableName;
            var query = GetQuery(request.SiteId, request.ChannelId, request);
            var totalCount = DataProvider.ContentDao.GetTotalCount(tableName, query);
            var channelContentIds = DataProvider.ContentDao.GetChannelContentIdList(tableName, query);

            var contents = new List<Dictionary<string, object>>();
            foreach (var channelContentId in channelContentIds)
            {
                var content = ContentManager.GetContentInfo(site, channelContentId.ChannelId, channelContentId.Id);
                contents.Add(content.ToDictionary());
            }

            return new QueryResult
            {
                Contents = contents,
                TotalCount = totalCount
            };
        }

        [OpenApiOperation("获取内容 API", "https://sscms.com/docs/v6/api/guide/contents/get.html")]
        [HttpGet, Route(RouteContent)]
        public IHttpActionResult Get(int siteId, int channelId, int id)
        {
            try
            {
                var request = new AuthenticatedRequest();
                var sourceId = request.GetPostInt(ContentAttribute.SourceId.ToCamelCase());
                bool isAuth;
                if (sourceId == SourceManager.User)
                {
                    isAuth = request.IsUserLoggin && request.UserPermissions.HasChannelPermissions(siteId, channelId, ConfigManager.ChannelPermissions.ContentView);
                }
                else
                {
                    isAuth = request.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeContents) ||
                             request.IsUserLoggin &&
                             request.UserPermissions.HasChannelPermissions(siteId, channelId,
                                 ConfigManager.ChannelPermissions.ContentView) ||
                             request.IsAdminLoggin &&
                             request.AdminPermissions.HasChannelPermissions(siteId, channelId,
                                 ConfigManager.ChannelPermissions.ContentView);
                }
                if (!isAuth) return Unauthorized();

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                if (!request.AdminPermissionsImpl.HasChannelPermissions(siteId, channelId,
                    ConfigManager.ChannelPermissions.ContentView)) return Unauthorized();

                var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, id);
                if (contentInfo == null) return NotFound();

                return Ok(new
                {
                    Value = contentInfo.ToDictionary()
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [OpenApiOperation("新增内容 API", "https://sscms.com/docs/v6/api/guide/contents/create.html")]
        [HttpPost, Route(RouteChannel)]
        public IHttpActionResult Create(int siteId, int channelId)
        {
            try
            {
                var request = new AuthenticatedRequest();
                var sourceId = request.GetPostInt(ContentAttribute.SourceId.ToCamelCase());
                bool isAuth;
                if (sourceId == SourceManager.User)
                {
                    isAuth = request.IsUserLoggin && request.UserPermissions.HasChannelPermissions(siteId, channelId, ConfigManager.ChannelPermissions.ContentAdd);
                }
                else
                {
                    isAuth = request.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeContents) ||
                             request.IsUserLoggin &&
                             request.UserPermissions.HasChannelPermissions(siteId, channelId,
                                 ConfigManager.ChannelPermissions.ContentAdd) ||
                             request.IsAdminLoggin &&
                             request.AdminPermissions.HasChannelPermissions(siteId, channelId,
                                 ConfigManager.ChannelPermissions.ContentAdd);
                }
                if (!isAuth) return Unauthorized();

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                if (!channelInfo.Additional.IsContentAddable) return BadRequest("此栏目不能添加内容");

                var attributes = request.GetPostObject<Dictionary<string, object>>();
                if (attributes == null) return BadRequest("无法从body中获取内容实体");
                var checkedLevel = request.GetPostInt("checkedLevel");

                var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
                var adminName = request.AdminName;

                var isChecked = checkedLevel >= siteInfo.Additional.CheckContentLevel;
                if (isChecked)
                {
                    if (sourceId == SourceManager.User || request.IsUserLoggin)
                    {
                        isChecked = request.UserPermissionsImpl.HasChannelPermissions(siteId, channelId,
                            ConfigManager.ChannelPermissions.ContentCheck);
                    }
                    else if (request.IsAdminLoggin)
                    {
                        isChecked = request.AdminPermissionsImpl.HasChannelPermissions(siteId, channelId,
                            ConfigManager.ChannelPermissions.ContentCheck);
                    }
                }

                var contentInfo = new ContentInfo(attributes)
                {
                    SiteId = siteId,
                    ChannelId = channelId,
                    AddUserName = adminName,
                    LastEditDate = DateTime.Now,
                    LastEditUserName = adminName,
                    AdminId = request.AdminId,
                    UserId = request.UserId,
                    SourceId = sourceId,
                    IsChecked = isChecked,
                    CheckedLevel = checkedLevel
                };

                contentInfo.Id = DataProvider.ContentDao.Insert(tableName, siteInfo, channelInfo, contentInfo);

                foreach (var service in PluginManager.Services)
                {
                    try
                    {
                        service.OnContentFormSubmit(new ContentFormSubmitEventArgs(siteId, channelId, contentInfo.Id, attributes, contentInfo));
                    }
                    catch (Exception ex)
                    {
                        LogUtils.AddErrorLog(service.PluginId, ex, nameof(IService.ContentFormSubmit));
                    }
                }

                if (contentInfo.IsChecked)
                {
                    CreateManager.CreateContent(siteId, channelId, contentInfo.Id);
                    CreateManager.TriggerContentChangedEvent(siteId, channelId);
                }

                request.AddSiteLog(siteId, channelId, contentInfo.Id, "添加内容",
                    $"栏目:{ChannelManager.GetChannelNameNavigation(siteId, contentInfo.ChannelId)},内容标题:{contentInfo.Title}");

                return Ok(new
                {
                    Value = contentInfo.ToDictionary()
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [OpenApiOperation("修改内容 API", "https://sscms.com/docs/v6/api/guide/contents/update.html")]
        [HttpPut, Route(RouteContent)]
        public IHttpActionResult Update(int siteId, int channelId, int id)
        {
            try
            {
                var request = new AuthenticatedRequest();
                var sourceId = request.GetPostInt(ContentAttribute.SourceId.ToCamelCase());
                bool isAuth;
                if (sourceId == SourceManager.User)
                {
                    isAuth = request.IsUserLoggin && request.UserPermissions.HasChannelPermissions(siteId, channelId, ConfigManager.ChannelPermissions.ContentEdit);
                }
                else
                {
                    isAuth = request.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeContents) ||
                             request.IsUserLoggin &&
                             request.UserPermissions.HasChannelPermissions(siteId, channelId,
                                 ConfigManager.ChannelPermissions.ContentEdit) ||
                             request.IsAdminLoggin &&
                             request.AdminPermissions.HasChannelPermissions(siteId, channelId,
                                 ConfigManager.ChannelPermissions.ContentEdit);
                }
                if (!isAuth) return Unauthorized();

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var attributes = request.GetPostObject<Dictionary<string, object>>();
                if (attributes == null) return BadRequest("无法从body中获取内容实体");

                var adminName = request.AdminName;

                var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, id);
                if (contentInfo == null) return NotFound();

                contentInfo.Load(attributes);
                contentInfo.Load(new
                {
                    SiteId = siteId,
                    ChannelId = channelId,
                    AddUserName = adminName,
                    LastEditDate = DateTime.Now,
                    LastEditUserName = adminName,
                    SourceId = sourceId
                });

                var postCheckedLevel = request.GetPostInt(ContentAttribute.CheckedLevel.ToCamelCase());
                var isChecked = postCheckedLevel >= siteInfo.Additional.CheckContentLevel;
                var checkedLevel = postCheckedLevel;

                contentInfo.Load(new
                {
                    IsChecked = isChecked,
                    CheckedLevel = checkedLevel
                });

                DataProvider.ContentDao.Update(siteInfo, channelInfo, contentInfo);

                foreach (var service in PluginManager.Services)
                {
                    try
                    {
                        service.OnContentFormSubmit(new ContentFormSubmitEventArgs(siteId, channelId, contentInfo.Id, attributes, contentInfo));
                    }
                    catch (Exception ex)
                    {
                        LogUtils.AddErrorLog(service.PluginId, ex, nameof(IService.ContentFormSubmit));
                    }
                }

                if (contentInfo.IsChecked)
                {
                    CreateManager.CreateContent(siteId, channelId, contentInfo.Id);
                    CreateManager.TriggerContentChangedEvent(siteId, channelId);
                }

                request.AddSiteLog(siteId, channelId, contentInfo.Id, "修改内容",
                    $"栏目:{ChannelManager.GetChannelNameNavigation(siteId, contentInfo.ChannelId)},内容标题:{contentInfo.Title}");

                return Ok(new
                {
                    Value = contentInfo.ToDictionary()
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [OpenApiOperation("删除内容 API", "https://sscms.com/docs/v6/api/guide/contents/delete.html")]
        [HttpDelete, Route(RouteContent)]
        public IHttpActionResult Delete(int siteId, int channelId, int id)
        {
            try
            {
                var request = new AuthenticatedRequest();
                var sourceId = request.GetPostInt(ContentAttribute.SourceId.ToCamelCase());
                bool isAuth;
                if (sourceId == SourceManager.User)
                {
                    isAuth = request.IsUserLoggin && request.UserPermissions.HasChannelPermissions(siteId, channelId, ConfigManager.ChannelPermissions.ContentDelete);
                }
                else
                {
                    isAuth = request.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeContents) ||
                             request.IsUserLoggin &&
                             request.UserPermissions.HasChannelPermissions(siteId, channelId,
                                 ConfigManager.ChannelPermissions.ContentDelete) ||
                             request.IsAdminLoggin &&
                             request.AdminPermissions.HasChannelPermissions(siteId, channelId,
                                 ConfigManager.ChannelPermissions.ContentDelete);
                }
                if (!isAuth) return Unauthorized();

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                if (!request.AdminPermissionsImpl.HasChannelPermissions(siteId, channelId,
                    ConfigManager.ChannelPermissions.ContentDelete)) return Unauthorized();

                var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

                var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, id);
                if (contentInfo == null) return NotFound();

                ContentUtility.Delete(tableName, siteInfo, channelId, id);

                //DataProvider.ContentDao.DeleteContent(tableName, siteInfo, channelId, id);

                return Ok(new
                {
                    Value = contentInfo.ToDictionary()
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [OpenApiOperation("审核内容 API", "https://sscms.com/docs/v6/api/guide/contents/check.html")]
        [HttpPost, Route(RouteCheck)]
        public CheckResult Check([FromBody] CheckRequest request)
        {
            var req = new AuthenticatedRequest();

            if (!req.IsApiAuthenticated ||
                !AccessTokenManager.IsScope(req.ApiToken, AccessTokenManager.ScopeContents))
            {
                return Request.Unauthorized<CheckResult>();
            }

            var site = SiteManager.GetSiteInfo(request.SiteId);
            if (site == null) return Request.BadRequest<CheckResult>("无法确定内容对应的站点");

            var contents = new List<Dictionary<string, object>>();
            foreach (var channelContentId in request.Contents)
            {
                var channel = ChannelManager.GetChannelInfo(request.SiteId, channelContentId.ChannelId);
                var tableName = ChannelManager.GetTableName(site, channel);
                var content = ContentManager.GetContentInfo(site, channel, channelContentId.Id);
                if (content == null) continue;

                content.Set(ContentAttribute.CheckUserName, req.AdminName);
                content.Set(ContentAttribute.CheckDate, DateTime.Now);
                content.Set(ContentAttribute.CheckReasons, request.Reasons);
                content.Checked = true;
                content.CheckedLevel = 0;

                DataProvider.ContentDao.Update(site, channel, content);

                contents.Add(content.ToDictionary());

                var contentCheck = new ContentCheckInfo
                {
                    TableName = tableName,
                    SiteId = request.SiteId,
                    ChannelId = content.ChannelId,
                    ContentId = content.Id,
                    UserName = req.AdminName,
                    IsChecked = true,
                    CheckedLevel = 0,
                    CheckDate = DateTime.Now,
                    Reasons = request.Reasons
                };

                DataProvider.ContentCheckDao.Insert(contentCheck);
            }

            req.AddSiteLog(request.SiteId, "批量审核内容");

            foreach (var content in request.Contents)
            {
                CreateManager.CreateContent(request.SiteId, content.ChannelId, content.Id);
            }

            foreach (var distinctChannelId in request.Contents.Select(x => x.ChannelId).Distinct())
            {
                CreateManager.TriggerContentChangedEvent(request.SiteId, distinctChannelId);
            }

            CreateManager.CreateChannel(request.SiteId, request.SiteId);

            return new CheckResult
            {
                Contents = contents
            };
        }

        //[OpenApiOperation("获取站点内容API", "")]
        //[HttpPost, Route(RouteSite)]
        //public QueryResult GetSiteContents([FromUri]int siteId, [FromBody] QueryRequest request)
        //{
        //    var req = new AuthenticatedRequest();
        //    var sourceId = req.GetPostInt(ContentAttribute.SourceId.ToCamelCase());
        //    bool isAuth;
        //    if (sourceId == SourceManager.User)
        //    {
        //        isAuth = req.IsUserLoggin && req.UserPermissions.HasChannelPermissions(siteId, siteId, ConfigManager.ChannelPermissions.ContentView);
        //    }
        //    else
        //    {
        //        isAuth = req.IsApiAuthenticated &&
        //                     AccessTokenManager.IsScope(req.ApiToken, AccessTokenManager.ScopeContents) ||
        //                 req.IsUserLoggin && req.UserPermissions.HasChannelPermissions(siteId, siteId,
        //                     ConfigManager.ChannelPermissions.ContentView) ||
        //                 req.IsAdminLoggin && req.AdminPermissions.HasChannelPermissions(siteId, siteId,
        //                     ConfigManager.ChannelPermissions.ContentView);
        //    }
        //    if (!isAuth) return Request.Unauthorized<QueryResult>();

        //    var site = SiteManager.GetSiteInfo(siteId);
        //    if (site == null) return Request.BadRequest<QueryResult>("无法确定内容对应的站点");

        //    if (!req.AdminPermissionsImpl.HasChannelPermissions(siteId, siteId,
        //        ConfigManager.ChannelPermissions.ContentView)) return Request.Unauthorized<QueryResult>();

        //    var tableName = site.TableName;
        //    var query = GetQuery(siteId, null, request);
        //    var totalCount = DataProvider.ContentDao.GetTotalCount(tableName, query);
        //    var channelContentIds = DataProvider.ContentDao.GetChannelContentIdList(tableName, query);

        //    var contents = new List<ContentInfo>();
        //    foreach (var channelContentId in channelContentIds)
        //    {
        //        var content = ContentManager.GetContentInfo(site, channelContentId.ChannelId, channelContentId.Id);
        //        contents.Add(content);
        //    }

        //    return new QueryResult
        //    {
        //        Contents = contents,
        //        TotalCount = totalCount
        //    };
        //}

        //[OpenApiOperation("获取站点内容API", "")]
        //[HttpGet, Route(RouteSite)]
        //public IHttpActionResult GetSiteContents(int siteId)
        //{
        //    try
        //    {
        //        var request = new AuthenticatedRequest();
        //        var sourceId = request.GetPostInt(ContentAttribute.SourceId.ToCamelCase());
        //        bool isAuth;
        //        if (sourceId == SourceManager.User)
        //        {
        //            isAuth = request.IsUserLoggin && request.UserPermissions.HasChannelPermissions(siteId, siteId, ConfigManager.ChannelPermissions.ContentView);
        //        }
        //        else
        //        {
        //            isAuth = request.IsApiAuthenticated &&
        //                     AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeContents) ||
        //                     request.IsUserLoggin &&
        //                     request.UserPermissions.HasChannelPermissions(siteId, siteId,
        //                         ConfigManager.ChannelPermissions.ContentView) ||
        //                     request.IsAdminLoggin &&
        //                     request.AdminPermissions.HasChannelPermissions(siteId, siteId,
        //                         ConfigManager.ChannelPermissions.ContentView);
        //        }
        //        if (!isAuth) return Unauthorized();

        //        var siteInfo = SiteManager.GetSiteInfo(siteId);
        //        if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

        //        if (!request.AdminPermissionsImpl.HasChannelPermissions(siteId, siteId,
        //            ConfigManager.ChannelPermissions.ContentView)) return Unauthorized();

        //        var tableName = siteInfo.TableName;

        //        var parameters = new ApiContentsParameters(request);

        //        var tupleList = DataProvider.ContentDao.ApiGetContentIdListBySiteId(tableName, siteId, parameters, out var count);
        //        var value = new List<Dictionary<string, object>>();
        //        foreach (var tuple in tupleList)
        //        {
        //            var contentInfo = ContentManager.GetContentInfo(siteInfo, tuple.Item1, tuple.Item2);
        //            if (contentInfo != null)
        //            {
        //                value.Add(contentInfo.ToDictionary());
        //            }
        //        }

        //        return Ok(new PageResponse(value, parameters.Top, parameters.Skip, request.HttpRequest.Url.AbsoluteUri) {Count = count});
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtils.AddErrorLog(ex);
        //        return InternalServerError(ex);
        //    }
        //}

        //[HttpGet, Route(RouteChannel)]
        //public IHttpActionResult GetChannelContents(int siteId, int channelId)
        //{
        //    try
        //    {
        //        var request = new AuthenticatedRequest();
        //        var sourceId = request.GetPostInt(ContentAttribute.SourceId.ToCamelCase());
        //        bool isAuth;
        //        if (sourceId == SourceManager.User)
        //        {
        //            isAuth = request.IsUserLoggin && request.UserPermissions.HasChannelPermissions(siteId, channelId, ConfigManager.ChannelPermissions.ContentView);
        //        }
        //        else
        //        {
        //            isAuth = request.IsApiAuthenticated &&
        //                     AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeContents) ||
        //                     request.IsUserLoggin &&
        //                     request.UserPermissions.HasChannelPermissions(siteId, channelId,
        //                         ConfigManager.ChannelPermissions.ContentView) ||
        //                     request.IsAdminLoggin &&
        //                     request.AdminPermissions.HasChannelPermissions(siteId, channelId,
        //                         ConfigManager.ChannelPermissions.ContentView);
        //        }
        //        if (!isAuth) return Unauthorized();

        //        var siteInfo = SiteManager.GetSiteInfo(siteId);
        //        if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

        //        var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
        //        if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

        //        if (!request.AdminPermissionsImpl.HasChannelPermissions(siteId, channelId,
        //            ConfigManager.ChannelPermissions.ContentView)) return Unauthorized();

        //        var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

        //        var top = request.GetQueryInt("top", 20);
        //        var skip = request.GetQueryInt("skip");
        //        var like = request.GetQueryString("like");
        //        var orderBy = request.GetQueryString("orderBy");

        //        var list = DataProvider.ContentDao.ApiGetContentIdListByChannelId(tableName, siteId, channelId, top, skip, like, orderBy, request.QueryString, out var count);
        //        var value = new List<Dictionary<string, object>>();
        //        foreach(var (contentChannelId, contentId) in list)
        //        {
        //            var contentInfo = ContentManager.GetContentInfo(siteInfo, contentChannelId, contentId);
        //            if (contentInfo != null)
        //            {
        //                value.Add(contentInfo.ToDictionary());
        //            }
        //        }

        //        return Ok(new PageResponse(value, top, skip, request.HttpRequest.Url.AbsoluteUri) { Count = count });
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtils.AddErrorLog(ex);
        //        return InternalServerError(ex);
        //    }
        //}
    }
}
