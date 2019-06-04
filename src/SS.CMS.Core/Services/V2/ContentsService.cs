using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using SS.CMS.Core.Api.V1;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Cache.Content;
using SS.CMS.Core.Common;
using SS.CMS.Core.Common.Create;
using SS.CMS.Core.Models;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Core.Plugin;
using SS.CMS.Core.Plugin.Apis;
using SS.CMS.Plugin;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services.V2
{
    public class ContentsService : ServiceBase
    {
        private const string RouteSite = "contents/{siteId:int}";
        private const string RouteChannel = "contents/{siteId:int}/{channelId:int}";
        private const string RouteContent = "contents/{siteId:int}/{channelId:int}/{id:int}";

        public ResponseResult<object> Create(IRequest request, IResponse response)
        {
            var siteId = TranslateUtils.Get<int>(request.RouteValues, "siteId");
            var channelId = TranslateUtils.Get<int>(request.RouteValues, "channelId");

            var sourceId = request.GetPostInt(ContentAttribute.SourceId.ToCamelCase());
            bool isAuth;
            if (sourceId == SourceManager.User)
            {
                isAuth = request.IsUserLoggin && request.UserPermissions.HasChannelPermissions(siteId, channelId, ConfigManager.ChannelPermissions.ContentAdd);
            }
            else
            {
                isAuth = !string.IsNullOrEmpty(request.ApiToken) &&
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

            if (!channelInfo.IsContentAddable) return BadRequest("此栏目不能添加内容");

            if (!request.TryGetPost<Dictionary<string, object>>(out var attributes))
            {
                return BadRequest("无法从body中获取内容实体");
            }

            var checkedLevel = request.GetPostInt("checkedLevel");

            var adminName = request.AdminName;

            var isChecked = checkedLevel >= siteInfo.CheckContentLevel;
            if (isChecked)
            {
                if (sourceId == SourceManager.User || request.IsUserLoggin)
                {
                    isChecked = request.UserPermissions.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentCheck);
                }
                else if (request.IsAdminLoggin)
                {
                    isChecked = request.AdminPermissions.HasChannelPermissions(siteId, channelId,
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
                Checked = isChecked,
                CheckedLevel = checkedLevel
            };

            contentInfo.Id = channelInfo.ContentDao.Insert(siteInfo, channelInfo, contentInfo);

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

            if (contentInfo.Checked)
            {
                CreateManager.CreateContent(siteId, channelId, contentInfo.Id);
                CreateManager.TriggerContentChangedEvent(siteId, channelId);
            }

            AdminApi.Instance.Log(adminName, siteId, channelId, contentInfo.Id, "添加内容",
                $"栏目:{ChannelManager.GetChannelNameNavigation(siteId, contentInfo.ChannelId)},内容标题:{contentInfo.Title}");

            return Ok(new
            {
                Value = contentInfo
            });
        }

        public ResponseResult<object> Update(IRequest request, IResponse response)
        {
            var siteId = TranslateUtils.Get<int>(request.RouteValues, "siteId");
            var channelId = TranslateUtils.Get<int>(request.RouteValues, "channelId");
            var id = TranslateUtils.Get<int>(request.RouteValues, "id");

            var sourceId = request.GetPostInt(ContentAttribute.SourceId.ToCamelCase());
            bool isAuth;
            if (sourceId == SourceManager.User)
            {
                isAuth = request.IsUserLoggin && request.UserPermissions.HasChannelPermissions(siteId, channelId, ConfigManager.ChannelPermissions.ContentEdit);
            }
            else
            {
                isAuth = !string.IsNullOrEmpty(request.ApiToken) &&
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

            if (!request.TryGetPost<Dictionary<string, object>>(out var attributes))
            {
                return BadRequest("无法从body中获取内容实体");
            }

            var adminName = request.AdminName;

            var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, id);
            if (contentInfo == null) return NotFound();
            var isChecked = contentInfo.Checked;
            var checkedLevel = contentInfo.CheckedLevel;

            foreach (var attribute in attributes)
            {
                contentInfo.Set(attribute.Key, attribute.Value);
            }
            contentInfo.SiteId = siteId;
            contentInfo.ChannelId = channelId;
            contentInfo.AddUserName = adminName;
            contentInfo.LastEditDate = DateTime.Now;
            contentInfo.LastEditUserName = adminName;
            contentInfo.SourceId = sourceId;

            var postCheckedLevel = request.GetPostInt(ContentAttribute.CheckedLevel.ToCamelCase());
            if (postCheckedLevel != CheckManager.LevelInt.NotChange)
            {
                isChecked = postCheckedLevel >= siteInfo.CheckContentLevel;
                checkedLevel = postCheckedLevel;
            }

            contentInfo.Checked = isChecked;
            contentInfo.CheckedLevel = checkedLevel;

            channelInfo.ContentDao.Update(siteInfo, channelInfo, contentInfo);

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

            if (contentInfo.Checked)
            {
                CreateManager.CreateContent(siteId, channelId, contentInfo.Id);
                CreateManager.TriggerContentChangedEvent(siteId, channelId);
            }
            
            AdminApi.Instance.Log(adminName, siteId, channelId, contentInfo.Id, "修改内容",
                $"栏目:{ChannelManager.GetChannelNameNavigation(siteId, contentInfo.ChannelId)},内容标题:{contentInfo.Title}");

            return Ok(new
            {
                Value = contentInfo
            });
        }

        public ResponseResult<object> Delete(IRequest request, IResponse response)
        {
            var siteId = TranslateUtils.Get<int>(request.RouteValues, "siteId");
            var channelId = TranslateUtils.Get<int>(request.RouteValues, "channelId");
            var id = TranslateUtils.Get<int>(request.RouteValues, "id");

            var sourceId = request.GetPostInt(ContentAttribute.SourceId.ToCamelCase());
            bool isAuth;
            if (sourceId == SourceManager.User)
            {
                isAuth = request.IsUserLoggin && request.UserPermissions.HasChannelPermissions(siteId, channelId, ConfigManager.ChannelPermissions.ContentDelete);
            }
            else
            {
                isAuth = !string.IsNullOrEmpty(request.ApiToken) &&
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

            if (!request.AdminPermissions.HasChannelPermissions(siteId, channelId,
                ConfigManager.ChannelPermissions.ContentDelete)) return Unauthorized();

            var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, id);
            if (contentInfo == null) return NotFound();

            ContentUtility.Delete(siteInfo, channelInfo, id);

            //DataProvider.ContentDao.DeleteContent(tableName, siteInfo, channelId, id);

            return Ok(new
            {
                Value = contentInfo
            });
        }

        public ResponseResult<object> Get(IRequest request, IResponse response)
        {
            var siteId = TranslateUtils.Get<int>(request.RouteValues, "siteId");
            var channelId = TranslateUtils.Get<int>(request.RouteValues, "channelId");
            var id = TranslateUtils.Get<int>(request.RouteValues, "id");

            var sourceId = request.GetPostInt(ContentAttribute.SourceId.ToCamelCase());
            bool isAuth;
            if (sourceId == SourceManager.User)
            {
                isAuth = request.IsUserLoggin && request.UserPermissions.HasChannelPermissions(siteId, channelId, ConfigManager.ChannelPermissions.ContentView);
            }
            else
            {
                isAuth = !string.IsNullOrEmpty(request.ApiToken) &&
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

            if (!request.AdminPermissions.HasChannelPermissions(siteId, channelId,
                ConfigManager.ChannelPermissions.ContentView)) return Unauthorized();

            var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, id);
            if (contentInfo == null) return NotFound();

            return Ok(new
            {
                Value = contentInfo
            });
        }

        public ResponseResult<object> GetSiteContents(IRequest request, IResponse response)
        {
            var siteId = TranslateUtils.Get<int>(request.RouteValues, "siteId");

            var sourceId = request.GetPostInt(ContentAttribute.SourceId.ToCamelCase());
            bool isAuth;
            if (sourceId == SourceManager.User)
            {
                isAuth = request.IsUserLoggin && request.UserPermissions.HasChannelPermissions(siteId, siteId, ConfigManager.ChannelPermissions.ContentView);
            }
            else
            {
                isAuth = !string.IsNullOrEmpty(request.ApiToken) &&
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

            if (!request.AdminPermissions.HasChannelPermissions(siteId, siteId,
                ConfigManager.ChannelPermissions.ContentView)) return Unauthorized();

            var parameters = new ApiContentsParameters(request);

            var tupleList = siteInfo.ContentDao.ApiGetContentIdListBySiteId(siteId, parameters, out var count);
            var value = new List<IDictionary<string, object>>();
            foreach (var tuple in tupleList)
            {
                var channelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, tuple.Item1);
                var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, tuple.Item2);
                if (contentInfo != null)
                {
                    value.Add(contentInfo.ToDictionary());
                }
            }

            return Ok(
                new PageResponse(value, parameters.Top, parameters.Skip, request.RawUrl) {Count = count}
            );
        }

        public ResponseResult<object> GetChannelContents(IRequest request, IResponse response)
        {
            var siteId = TranslateUtils.Get<int>(request.RouteValues, "siteId");
            var channelId = TranslateUtils.Get<int>(request.RouteValues, "channelId");

            var sourceId = request.GetPostInt(ContentAttribute.SourceId.ToCamelCase());
            bool isAuth;
            if (sourceId == SourceManager.User)
            {
                isAuth = request.IsUserLoggin && request.UserPermissions.HasChannelPermissions(siteId, channelId, ConfigManager.ChannelPermissions.ContentView);
            }
            else
            {
                isAuth = !string.IsNullOrEmpty(request.ApiToken) &&
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

            if (!request.AdminPermissions.HasChannelPermissions(siteId, channelId,
                ConfigManager.ChannelPermissions.ContentView)) return Unauthorized();

            var top = request.GetQueryInt("top", 20);
            var skip = request.GetQueryInt("skip");
            var like = request.GetQueryString("like");
            var orderBy = request.GetQueryString("orderBy");

            var queryString = new NameValueCollection();
            foreach (var key in request.QueryKeys)
            {
                queryString[key] = request.GetQueryString(key);
            }

            var list = channelInfo.ContentDao.ApiGetContentIdListByChannelId(siteId, channelId, top, skip, like, orderBy, queryString, out var count);
            var value = new List<IDictionary<string, object>>();
            foreach (var (contentChannelId, contentId) in list)
            {
                var contentChannelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, contentChannelId);
                var contentInfo = ContentManager.GetContentInfo(siteInfo, contentChannelInfo, contentId);
                if (contentInfo != null)
                {
                    value.Add(contentInfo.ToDictionary());
                }
            }

            return Ok(
                new PageResponse(value, top, skip, request.RawUrl) {Count = count}
            );
        }
    }
}
