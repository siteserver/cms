using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class ChannelsController
    {
        [OpenApiOperation("修改栏目 API", "修改栏目，使用PUT发起请求，请求地址为/api/v1/channels/{siteId}/{channelId}")]
        [HttpPut, Route(RouteChannel)]
        public async Task<ActionResult<Channel>> Update([FromRoute] int siteId, [FromRoute] int channelId, [FromBody] UpdateRequest request)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeChannels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(channelId);
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
                    var indexNameList = await _channelRepository.GetIndexNamesAsync(siteId);
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

                    var filePathList = await _channelRepository.GetAllFilePathBySiteIdAsync(siteId);
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
    }
}
