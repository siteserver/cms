using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.CMS.Api.V1;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
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
                var request = new RequestImpl();
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

                request.AddSiteLog(siteId, channelId, contentInfo.Id, "添加内容",
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
                var request = new RequestImpl();
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

                var postCheckedLevel = request.GetPostInt(ContentAttribute.CheckedLevel.ToCamelCase());
                if (postCheckedLevel != CheckManager.LevelInt.NotChange)
                {
                    isChecked = postCheckedLevel >= siteInfo.Additional.CheckContentLevel;
                    checkedLevel = postCheckedLevel;
                }

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

                request.AddSiteLog(siteId, channelId, contentInfo.Id, "修改内容",
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
                var request = new RequestImpl();
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

                DataProvider.ContentDao.DeleteContent(tableName, siteInfo, channelId, id);

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
                var request = new RequestImpl();
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
                var request = new RequestImpl();
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

                var tupleList = DataProvider.ContentDao.ApiGetContentIdListBySiteId(tableName, siteId, parameters, out var count);
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
                var request = new RequestImpl();
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
                var contentIdList = DataProvider.ContentDao.ApiGetContentIdListByChannelId(tableName, siteId, channelId, top, skip, like, orderBy, request.QueryString, out count);
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
