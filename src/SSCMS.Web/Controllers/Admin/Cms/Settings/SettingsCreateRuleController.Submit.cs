using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Utils;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using System.Collections.Generic;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsCreateRuleController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<List<int>>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.SettingsCreateRule))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            var channel = await _channelRepository.GetAsync(request.Id);

            if (request.Id != request.SiteId)
            {
                channel.LinkType = request.LinkType;

                if (channel.LinkType == Enums.LinkType.None)
                {
                    channel.LinkUrl = request.LinkUrl;
                }
                else if (channel.LinkType == Enums.LinkType.LinkToChannel)
                {
                    channel.LinkUrl = ListUtils.ToString(request.ChannelIds);
                }
                else if (channel.LinkType == Enums.LinkType.LinkToContent)
                {
                    channel.LinkUrl = ListUtils.ToString(request.ChannelIds) + "_" + request.ContentId;
                }
                else
                {
                    channel.LinkUrl = string.Empty;
                }

                var filePath = channel.FilePath;
                if (!string.IsNullOrEmpty(request.FilePath) && !StringUtils.EqualsIgnoreCase(filePath, request.FilePath))
                {
                    if (!DirectoryUtils.IsDirectoryNameCompliant(request.FilePath))
                    {
                        return this.Error("栏目页面路径不符合系统要求！");
                    }

                    if (PathUtils.IsDirectoryPath(request.FilePath))
                    {
                        request.FilePath = PageUtils.Combine(request.FilePath, "index.html");
                    }

                    var filePathArrayList = await _channelRepository.GetAllFilePathBySiteIdAsync(request.SiteId);
                    if (filePathArrayList.Contains(request.FilePath))
                    {
                        return this.Error("栏目修改失败，栏目页面路径已存在！");
                    }
                }

                if (request.FilePath != await _pathManager.GetInputChannelUrlAsync(site, channel, false))
                {
                    channel.FilePath = request.FilePath;
                }
            }

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

            if (request.ChannelFilePathRule != await _pathManager.GetChannelFilePathRuleAsync(site, request.Id))
            {
                channel.ChannelFilePathRule = request.ChannelFilePathRule;
            }
            if (request.ContentFilePathRule != await _pathManager.GetContentFilePathRuleAsync(site, request.Id))
            {
                channel.ContentFilePathRule = request.ContentFilePathRule;
            }

            await _channelRepository.UpdateAsync(channel);

            await _createManager.CreateChannelAsync(request.SiteId, request.Id);

            await _authManager.AddSiteLogAsync(request.SiteId, request.Id, 0, "设置页面生成规则", $"栏目：{channel.ChannelName}");

            var expendedChannelIds = new List<int>
            {
                request.SiteId
            };
            if (!expendedChannelIds.Contains(channel.ParentId))
            {
                expendedChannelIds.Add(channel.ParentId);
            }

            return expendedChannelIds;
        }
    }
}