using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory.Utils;
using Microsoft.AspNetCore.Mvc;
using SSCMS;
using SSCMS.Core.Extensions;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    [Route("v1/channels")]
    public partial class ChannelsController : ControllerBase
    {
        private const string RouteSite = "{siteId:int}";
        private const string RouteChannel = "{siteId:int}/{channelId:int}";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public ChannelsController(IAuthManager authManager, ICreateManager createManager, IAccessTokenRepository accessTokenRepository, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository)
        {
            _authManager = authManager;
            _createManager = createManager;
            _accessTokenRepository = accessTokenRepository;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
        }

        [HttpPost, Route(RouteSite)]
        public async Task<ActionResult<Channel>> Create([FromBody]CreateRequest request)
        {
            var isAuth = await _authManager.IsApiAuthenticatedAsync() && await
                             _accessTokenRepository.IsScopeAsync(_authManager.GetApiToken(), Constants.ScopeChannels) ||
                         await _authManager.IsAdminAuthenticatedAsync() &&
                         await _authManager.HasChannelPermissionsAsync(request.SiteId, request.ParentId,
                             Constants.ChannelPermissions.ChannelAdd);
            if (!isAuth) return Unauthorized();

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channelInfo = new Channel
            {
                SiteId = request.SiteId,
                ParentId = request.ParentId,
                ContentModelPluginId = request.ContentModelPluginId,
                ContentRelatedPluginIds = request.ContentRelatedPluginIds
            };

            if (!string.IsNullOrEmpty(request.IndexName))
            {
                var indexNameList = await _channelRepository.GetIndexNameListAsync(request.SiteId);
                if (indexNameList.Contains(request.IndexName))
                {
                    return this.Error("栏目添加失败，栏目索引已存在！");
                }
            }

            if (!string.IsNullOrEmpty(request.FilePath))
            {
                if (!DirectoryUtils.IsDirectoryNameCompliant(request.FilePath))
                {
                    return this.Error("栏目页面路径不符合系统要求！");
                }

                if (PathUtils.IsDirectoryPath(request.FilePath))
                {
                    request.FilePath = PageUtils.Combine(request.FilePath, "index.html");
                }

                var filePathList = await _channelRepository.GetAllFilePathBySiteIdAsync(request.SiteId);
                if (filePathList.Contains(request.FilePath))
                {
                    return this.Error("栏目添加失败，栏目页面路径已存在！");
                }
            }

            if (!string.IsNullOrEmpty(request.ChannelFilePathRule))
            {
                if (!DirectoryUtils.IsDirectoryNameCompliant(request.ChannelFilePathRule))
                {
                    return this.Error("栏目页面命名规则不符合系统要求！");
                }
                if (PathUtils.IsDirectoryPath(request.ChannelFilePathRule))
                {
                    return this.Error("栏目页面命名规则必须包含生成文件的后缀！");
                }
            }

            if (!string.IsNullOrEmpty(request.ContentFilePathRule))
            {
                if (!DirectoryUtils.IsDirectoryNameCompliant(request.ContentFilePathRule))
                {
                    return this.Error("内容页面命名规则不符合系统要求！");
                }
                if (PathUtils.IsDirectoryPath(request.ContentFilePathRule))
                {
                    return this.Error("内容页面命名规则必须包含生成文件的后缀！");
                }
            }

            //var parentChannel = await _channelRepository.GetAsync(siteId, parentId);
            //var styleList = TableStyleManager.GetChannelStyleList(parentChannel);
            //var extendedAttributes = BackgroundInputTypeParser.SaveAttributes(site, styleList, Request.Form, null);

            foreach (var (key, value) in request)
            {
                channelInfo.Set(key, value);
            }
            //foreach (string key in attributes)
            //{
            //    channel.SetExtendedAttribute(key, attributes[key]);
            //}

            channelInfo.ChannelName = request.ChannelName;
            channelInfo.IndexName = request.IndexName;
            channelInfo.FilePath = request.FilePath;
            channelInfo.ChannelFilePathRule = request.ChannelFilePathRule;
            channelInfo.ContentFilePathRule = request.ContentFilePathRule;

            channelInfo.GroupNames = request.GroupNames;
            channelInfo.ImageUrl = request.ImageUrl;
            channelInfo.Content = request.Content;
            channelInfo.Keywords = request.Keywords;
            channelInfo.Description = request.Description;
            channelInfo.LinkUrl = request.LinkUrl;
            channelInfo.LinkType = request.LinkType;
            channelInfo.ChannelTemplateId = request.ChannelTemplateId;
            channelInfo.ContentTemplateId = request.ContentTemplateId;

            channelInfo.AddDate = DateTime.Now;
            channelInfo.Id = await _channelRepository.InsertAsync(channelInfo);
            //栏目选择投票样式后，内容

            await _createManager.CreateChannelAsync(request.SiteId, channelInfo.Id);

            await _authManager.AddSiteLogAsync(request.SiteId, "添加栏目", $"栏目:{request.ChannelName}");

            return channelInfo;
        }

        [HttpPut, Route(RouteChannel)]
        public async Task<ActionResult<Channel>> Update([FromBody] UpdateRequest request)
        {
            var isAuth = await _authManager.IsApiAuthenticatedAsync() && await
                             _accessTokenRepository.IsScopeAsync(_authManager.GetApiToken(), Constants.ScopeChannels) ||
                         await _authManager.IsAdminAuthenticatedAsync() &&
                         await _authManager.HasChannelPermissionsAsync(request.SiteId, request.ChannelId,
                             Constants.ChannelPermissions.ChannelEdit);
            if (!isAuth) return Unauthorized();

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            foreach (var (key, value) in request)
            {
                channel.Set(key, value);
            }

            if (!string.IsNullOrEmpty(request.ChannelName))
            {
                channel.ChannelName = request.ChannelName;
            }

            if (request.IndexName != null)
            {
                if (!channel.IndexName.Equals(request.IndexName) && !string.IsNullOrEmpty(request.IndexName))
                {
                    var indexNameList = await _channelRepository.GetIndexNameListAsync(request.SiteId);
                    if (indexNameList.Contains(request.IndexName))
                    {
                        return this.Error("栏目属性修改失败，栏目索引已存在！");
                    }
                }
                channel.IndexName = request.IndexName;
            }

            if (request.ContentModelPluginId != null)
            {
                if (channel.ContentModelPluginId != request.ContentModelPluginId)
                {
                    channel.ContentModelPluginId = request.ContentModelPluginId;
                }
            }

            if (request.ContentRelatedPluginIds != null)
            {
                channel.ContentRelatedPluginIds = Utilities.GetStringList(request.ContentRelatedPluginIds);
            }

            if (request.FilePath != null)
            {
                request.FilePath = request.FilePath.Trim();
                if (!channel.FilePath.Equals(request.FilePath) && !string.IsNullOrEmpty(request.FilePath))
                {
                    if (!DirectoryUtils.IsDirectoryNameCompliant(request.FilePath))
                    {
                        return this.Error("栏目页面路径不符合系统要求！");
                    }

                    if (PathUtils.IsDirectoryPath(request.FilePath))
                    {
                        request.FilePath = PageUtils.Combine(request.FilePath, "index.html");
                    }

                    var filePathList = await _channelRepository.GetAllFilePathBySiteIdAsync(request.SiteId);
                    if (filePathList.Contains(request.FilePath))
                    {
                        return this.Error("栏目修改失败，栏目页面路径已存在！");
                    }
                }
                channel.FilePath = request.FilePath;
            }

            if (request.ChannelFilePathRule != null)
            {
                if (!string.IsNullOrEmpty(request.ChannelFilePathRule))
                {
                    var filePathRule = request.ChannelFilePathRule.Replace("|", string.Empty);
                    if (!DirectoryUtils.IsDirectoryNameCompliant(filePathRule))
                    {
                        return this.Error("栏目页面命名规则不符合系统要求！");
                    }
                    if (PathUtils.IsDirectoryPath(filePathRule))
                    {
                        return this.Error("栏目页面命名规则必须包含生成文件的后缀！");
                    }
                }

                channel.ChannelFilePathRule = request.ChannelFilePathRule;
            }

            if (request.ContentFilePathRule != null)
            {
                if (!string.IsNullOrEmpty(request.ContentFilePathRule))
                {
                    var filePathRule = request.ContentFilePathRule.Replace("|", string.Empty);
                    if (!DirectoryUtils.IsDirectoryNameCompliant(filePathRule))
                    {
                        return this.Error("内容页面命名规则不符合系统要求！");
                    }
                    if (PathUtils.IsDirectoryPath(filePathRule))
                    {
                        return this.Error("内容页面命名规则必须包含生成文件的后缀！");
                    }
                }

                channel.ContentFilePathRule = request.ContentFilePathRule;
            }

            if (request.GroupNames != null)
            {
                channel.GroupNames = request.GroupNames;
            }

            if (request.ImageUrl != null)
            {
                channel.ImageUrl = request.ImageUrl;
            }

            if (request.Content != null)
            {
                channel.Content = request.Content;
            }

            if (request.Keywords != null)
            {
                channel.Keywords = request.Keywords;
            }

            if (request.Description != null)
            {
                channel.Description = request.Description;
            }

            if (request.LinkUrl != null)
            {
                channel.LinkUrl = request.LinkUrl;
            }

            if (request.LinkType != null)
            {
                channel.LinkType = TranslateUtils.ToEnum(request.LinkType, LinkType.None);
            }

            if (request.ChannelTemplateId.HasValue)
            {
                channel.ChannelTemplateId = request.ChannelTemplateId.Value;
            }

            if (request.ContentTemplateId.HasValue)
            {
                channel.ContentTemplateId = request.ContentTemplateId.Value;
            }

            await _channelRepository.UpdateAsync(channel);

            return channel;
        }

        [HttpDelete, Route(RouteChannel)]
        public async Task<ActionResult<Channel>> Delete(int siteId, int channelId)
        {
            var isAuth = await _authManager.IsApiAuthenticatedAsync() && await
                             _accessTokenRepository.IsScopeAsync(_authManager.GetApiToken(), Constants.ScopeChannels) ||
                         await _authManager.IsAdminAuthenticatedAsync() &&
                         await _authManager.HasChannelPermissionsAsync(siteId, channelId,
                             Constants.ChannelPermissions.ChannelDelete);
            if (!isAuth) return Unauthorized();

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(channelId);
            if (channel == null) return NotFound();

            var adminId = await _authManager.GetAdminIdAsync();
            await _contentRepository.RecycleAllAsync(site, channelId, adminId);
            await _channelRepository.DeleteAsync(site, channelId, adminId);

            return channel;
        }

        [HttpGet, Route(RouteChannel)]
        public async Task<ActionResult<Channel>> Get(int siteId, int channelId)
        {
            var isAuth = await _authManager.IsApiAuthenticatedAsync() && await
                             _accessTokenRepository.IsScopeAsync(_authManager.GetApiToken(), Constants.ScopeChannels) ||
                         await _authManager.IsAdminAuthenticatedAsync();
            if (!isAuth) return Unauthorized();

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            var channel = await _channelRepository.GetAsync(channelId);
            if (channel == null) return this.Error("无法确定内容对应的栏目");

            channel.Children = await _channelRepository.GetChildrenAsync(siteId, channelId);

            return channel;
        }

        [HttpGet, Route(RouteSite)]
        public async Task<ActionResult<List<IDictionary<string, object>>>> GetChannels(int siteId)
        {
            var isAuth = await _authManager.IsApiAuthenticatedAsync() && await
                             _accessTokenRepository.IsScopeAsync(_authManager.GetApiToken(), Constants.ScopeChannels) ||
                         await _authManager.IsAdminAuthenticatedAsync();
            if (!isAuth) return Unauthorized();

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) return NotFound();

            var channelInfoList = await _channelRepository.GetChannelListAsync(siteId);

            var dictInfoList = new List<IDictionary<string, object>>();
            foreach (var channelInfo in channelInfoList)
            {
                dictInfoList.Add(channelInfo.ToDictionary());
            }

            return dictInfoList;
        }
    }
}
