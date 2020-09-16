using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class ChannelsController
    {
        [OpenApiOperation("新增栏目 API", "新增栏目，使用POST发起请求，请求地址为/api/v1/channels/{siteId}")]
        [HttpPost, Route(RouteSite)]
        public async Task<ActionResult<Channel>> Create([FromRoute] int siteId, [FromBody] CreateRequest request)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeChannels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) return NotFound();

            var channelInfo = new Channel
            {
                SiteId = siteId,
                ParentId = request.ParentId
            };

            if (!string.IsNullOrEmpty(request.IndexName))
            {
                var indexNameList = await _channelRepository.GetIndexNamesAsync(siteId);
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

                var filePathList = await _channelRepository.GetAllFilePathBySiteIdAsync(siteId);
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

            await _createManager.CreateChannelAsync(siteId, channelInfo.Id);

            await _authManager.AddSiteLogAsync(siteId, "添加栏目", $"栏目:{request.ChannelName}");

            return channelInfo;
        }
    }
}
