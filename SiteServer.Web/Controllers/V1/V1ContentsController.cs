using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Core.RestRoutes.V1;
using SiteServer.CMS.Database.Attributes;
using SiteServer.CMS.Database.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.V1
{
    [RoutePrefix("v1/contents")]
    public class V1ContentsController : ApiController
    {
        private const string RouteSite = "{siteId:int}";
        private const string RouteChannel = "{siteId:int}/{channelId:int}";
        private const string RouteContent = "{siteId:int}/{channelId:int}/{id:int}";

        [HttpPost, Route(RouteChannel)]
        public IHttpActionResult Create(int siteId, int channelId)
        {
            try
            {
                var rest = new Rest(Request);
                var sourceId = rest.GetPostInt(ContentAttribute.SourceId.ToCamelCase());
                bool isAuth;
                if (sourceId == SourceManager.User)
                {
                    isAuth = rest.IsUserLoggin && rest.UserPermissions.HasChannelPermissions(siteId, channelId, ConfigManager.ChannelPermissions.ContentAdd);
                }
                else
                {
                    isAuth = rest.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(rest.ApiToken, AccessTokenManager.ScopeContents) ||
                             rest.IsUserLoggin &&
                             rest.UserPermissions.HasChannelPermissions(siteId, channelId,
                                 ConfigManager.ChannelPermissions.ContentAdd) ||
                             rest.IsAdminLoggin &&
                             rest.AdminPermissions.HasChannelPermissions(siteId, channelId,
                                 ConfigManager.ChannelPermissions.ContentAdd);
                }
                if (!isAuth) return Unauthorized();

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                if (!channelInfo.Extend.IsContentAddable) return BadRequest("此栏目不能添加内容");

                var attributes = rest.GetPostObject<Dictionary<string, object>>();
                if (attributes == null) return BadRequest("无法从body中获取内容实体");
                var checkedLevel = rest.GetPostInt("checkedLevel");

                var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
                var adminName = rest.AdminName;

                var isChecked = checkedLevel >= siteInfo.Extend.CheckContentLevel;
                if (isChecked)
                {
                    if (sourceId == SourceManager.User || rest.IsUserLoggin)
                    {
                        isChecked = rest.UserPermissionsImpl.HasChannelPermissions(siteId, channelId,
                            ConfigManager.ChannelPermissions.ContentCheck);
                    }
                    else if (rest.IsAdminLoggin)
                    {
                        isChecked = rest.AdminPermissionsImpl.HasChannelPermissions(siteId, channelId,
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
                    AdminId = rest.AdminId,
                    UserId = rest.UserId,
                    SourceId = sourceId,
                    IsChecked = isChecked,
                    CheckedLevel = checkedLevel
                };

                contentInfo.Id = DataProvider.ContentRepository.Insert(tableName, siteInfo, channelInfo, contentInfo);

                foreach (var service in PluginManager.Services)
                {
                    try
                    {
                        service.OnContentFormSubmit(new ContentFormSubmitEventArgs(siteId, channelId, contentInfo.Id, new AttributesImpl(attributes), contentInfo));
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

                rest.AddSiteLog(siteId, channelId, contentInfo.Id, "添加内容",
                    $"栏目:{ChannelManager.GetChannelNameNavigation(siteId, contentInfo.ChannelId)},内容标题:{contentInfo.Title}");

                return Ok(new
                {
                    Value = contentInfo
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route(RouteContent)]
        public IHttpActionResult Update(int siteId, int channelId, int id)
        {
            try
            {
                var rest = new Rest(Request);
                var sourceId = rest.GetPostInt(ContentAttribute.SourceId.ToCamelCase());
                bool isAuth;
                if (sourceId == SourceManager.User)
                {
                    isAuth = rest.IsUserLoggin && rest.UserPermissions.HasChannelPermissions(siteId, channelId, ConfigManager.ChannelPermissions.ContentEdit);
                }
                else
                {
                    isAuth = rest.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(rest.ApiToken, AccessTokenManager.ScopeContents) ||
                             rest.IsUserLoggin &&
                             rest.UserPermissions.HasChannelPermissions(siteId, channelId,
                                 ConfigManager.ChannelPermissions.ContentEdit) ||
                             rest.IsAdminLoggin &&
                             rest.AdminPermissions.HasChannelPermissions(siteId, channelId,
                                 ConfigManager.ChannelPermissions.ContentEdit);
                }
                if (!isAuth) return Unauthorized();

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var attributes = rest.GetPostObject<Dictionary<string, object>>();
                if (attributes == null) return BadRequest("无法从body中获取内容实体");

                var adminName = rest.AdminName;

                var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, id);
                if (contentInfo == null) return NotFound();
                var isChecked = contentInfo.IsChecked;
                var checkedLevel = contentInfo.CheckedLevel;

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

                var postCheckedLevel = rest.GetPostInt(ContentAttribute.CheckedLevel.ToCamelCase());
                if (postCheckedLevel != CheckManager.LevelInt.NotChange)
                {
                    isChecked = postCheckedLevel >= siteInfo.Extend.CheckContentLevel;
                    checkedLevel = postCheckedLevel;
                }

                contentInfo.Load(new
                {
                    IsChecked = isChecked,
                    CheckedLevel = checkedLevel
                });

                DataProvider.ContentRepository.Update(siteInfo, channelInfo, contentInfo);

                foreach (var service in PluginManager.Services)
                {
                    try
                    {
                        service.OnContentFormSubmit(new ContentFormSubmitEventArgs(siteId, channelId, contentInfo.Id, new AttributesImpl(attributes), contentInfo));
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

                rest.AddSiteLog(siteId, channelId, contentInfo.Id, "修改内容",
                    $"栏目:{ChannelManager.GetChannelNameNavigation(siteId, contentInfo.ChannelId)},内容标题:{contentInfo.Title}");

                return Ok(new
                {
                    Value = contentInfo
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(RouteContent)]
        public IHttpActionResult Delete(int siteId, int channelId, int id)
        {
            try
            {
                var rest = new Rest(Request);
                var sourceId = rest.GetPostInt(ContentAttribute.SourceId.ToCamelCase());
                bool isAuth;
                if (sourceId == SourceManager.User)
                {
                    isAuth = rest.IsUserLoggin && rest.UserPermissions.HasChannelPermissions(siteId, channelId, ConfigManager.ChannelPermissions.ContentDelete);
                }
                else
                {
                    isAuth = rest.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(rest.ApiToken, AccessTokenManager.ScopeContents) ||
                             rest.IsUserLoggin &&
                             rest.UserPermissions.HasChannelPermissions(siteId, channelId,
                                 ConfigManager.ChannelPermissions.ContentDelete) ||
                             rest.IsAdminLoggin &&
                             rest.AdminPermissions.HasChannelPermissions(siteId, channelId,
                                 ConfigManager.ChannelPermissions.ContentDelete);
                }
                if (!isAuth) return Unauthorized();

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                if (!rest.AdminPermissionsImpl.HasChannelPermissions(siteId, channelId,
                    ConfigManager.ChannelPermissions.ContentDelete)) return Unauthorized();

                var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

                var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, id);
                if (contentInfo == null) return NotFound();

                DataProvider.ContentRepository.DeleteContent(tableName, siteInfo, channelId, id);

                return Ok(new
                {
                    Value = contentInfo
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteContent)]
        public IHttpActionResult Get(int siteId, int channelId, int id)
        {
            try
            {
                var rest = new Rest(Request);
                var sourceId = rest.GetPostInt(ContentAttribute.SourceId.ToCamelCase());
                bool isAuth;
                if (sourceId == SourceManager.User)
                {
                    isAuth = rest.IsUserLoggin && rest.UserPermissions.HasChannelPermissions(siteId, channelId, ConfigManager.ChannelPermissions.ContentView);
                }
                else
                {
                    isAuth = rest.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(rest.ApiToken, AccessTokenManager.ScopeContents) ||
                             rest.IsUserLoggin &&
                             rest.UserPermissions.HasChannelPermissions(siteId, channelId,
                                 ConfigManager.ChannelPermissions.ContentView) ||
                             rest.IsAdminLoggin &&
                             rest.AdminPermissions.HasChannelPermissions(siteId, channelId,
                                 ConfigManager.ChannelPermissions.ContentView);
                }
                if (!isAuth) return Unauthorized();

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                if (!rest.AdminPermissionsImpl.HasChannelPermissions(siteId, channelId,
                    ConfigManager.ChannelPermissions.ContentView)) return Unauthorized();

                var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, id);
                if (contentInfo == null) return NotFound();

                return Ok(new
                {
                    Value = contentInfo
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteSite)]
        public IHttpActionResult GetSiteContents(int siteId)
        {
            try
            {
#pragma warning disable CS0612 // '“RequestImpl”已过时
                var request = new RequestImpl(HttpContext.Current.Request);
#pragma warning restore CS0612 // '“RequestImpl”已过时

                var sourceId = request.GetPostInt(ContentAttribute.SourceId.ToCamelCase());
                bool isAuth;
                if (sourceId == SourceManager.User)
                {
                    isAuth = request.IsUserLoggin && request.UserPermissions.HasChannelPermissions(siteId, siteId, ConfigManager.ChannelPermissions.ContentView);
                }
                else
                {
                    isAuth = request.IsApiAuthenticated &&
                             AccessTokenManager.IsScope(request.ApiToken, AccessTokenManager.ScopeContents) ||
                             request.IsUserLoggin &&
                             request.UserPermissions.HasChannelPermissions(siteId, siteId,
                                 ConfigManager.ChannelPermissions.ContentView) ||
                             request.IsAdminLoggin &&
                             request.AdminPermissions.HasChannelPermissions(siteId, siteId,
                                 ConfigManager.ChannelPermissions.ContentView);
                }
                if (!isAuth) return Unauthorized();

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                if (!request.AdminPermissionsImpl.HasChannelPermissions(siteId, siteId,
                    ConfigManager.ChannelPermissions.ContentView)) return Unauthorized();

                var tableName = siteInfo.TableName;

                var parameters = new ApiContentsParameters(request);

                var tupleList = DataProvider.ContentRepository.ApiGetContentIdListBySiteId(tableName, siteId, parameters, out var count);
                var value = new List<Dictionary<string, object>>();
                foreach (var tuple in tupleList)
                {
                    var contentInfo = ContentManager.GetContentInfo(siteInfo, tuple.Item1, tuple.Item2);
                    if (contentInfo != null)
                    {
                        value.Add(contentInfo.ToDictionary());
                    }
                }

                return Ok(new PageResponse(value, parameters.Top, parameters.Skip, request.HttpRequest.Url.AbsoluteUri) {Count = count});
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteChannel)]
        public IHttpActionResult GetChannelContents(int siteId, int channelId)
        {
            try
            {
#pragma warning disable CS0612 // '“RequestImpl”已过时
                var request = new RequestImpl(HttpContext.Current.Request);
#pragma warning restore CS0612 // '“RequestImpl”已过时

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

                var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

                var top = request.GetQueryInt("top", 20);
                var skip = request.GetQueryInt("skip");
                var like = request.GetQueryString("like");
                var orderBy = request.GetQueryString("orderBy");

                int count;
                var contentIdList = DataProvider.ContentRepository.ApiGetContentIdListByChannelId(tableName, siteId, channelId, top, skip, like, orderBy, request.QueryString, out count);
                var value = new List<Dictionary<string, object>>();
                foreach(var contentId in contentIdList)
                {
                    var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);
                    if (contentInfo != null)
                    {
                        value.Add(contentInfo.ToDictionary());
                    }
                }

                return Ok(new PageResponse(value, top, skip, request.HttpRequest.Url.AbsoluteUri) { Count = count });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }
    }
}
