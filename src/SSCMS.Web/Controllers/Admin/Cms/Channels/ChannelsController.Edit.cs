using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsController
    {
        [HttpPut, Route(Route)]
        public async Task<ActionResult<List<int>>> Edit([FromBody] PutRequest request)
        {
            if (!await _authManager.HasChannelPermissionsAsync(request.SiteId, request.Id, MenuUtils.ChannelPermissions.Edit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            if (string.IsNullOrEmpty(request.ChannelName))
            {
                return this.Error("栏目修改失败，必须填写栏目名称！");
            }

            var channel = await _channelRepository.GetAsync(request.Id);
            if (!StringUtils.Equals(channel.IndexName, request.IndexName) && !string.IsNullOrEmpty(request.IndexName))
            {
                if (await _channelRepository.IsIndexNameExistsAsync(request.SiteId, request.IndexName))
                {
                    return this.Error("栏目修改失败，栏目索引已存在！");
                }
            }

            if (!StringUtils.Equals(channel.FilePath, request.FilePath) && !string.IsNullOrEmpty(request.FilePath))
            {
                if (!DirectoryUtils.IsDirectoryNameCompliant(request.FilePath))
                {
                    return this.Error("栏目页面路径不符合系统要求！");
                }

                if (PathUtils.IsDirectoryPath(request.FilePath))
                {
                    request.FilePath = PageUtils.Combine(request.FilePath, "index.html");
                }

                if (await _channelRepository.IsFilePathExistsAsync(request.SiteId, request.FilePath))
                {
                    return this.Error("栏目修改失败，栏目页面路径已存在！");
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

            var styles = await GetInputStylesAsync(channel);
            foreach (var style in styles)
            {
                var inputType = style.InputType;
                if (inputType == InputType.TextEditor)
                {
                    var value = request.Get(style.AttributeName, string.Empty);
                    value = await _pathManager.EncodeTextEditorAsync(site, value);
                    value = UEditorUtils.TranslateToStlElement(value);
                    channel.Set(style.AttributeName, value);
                }
                else if (inputType == InputType.Image ||
                         inputType == InputType.Video ||
                         inputType == InputType.File)
                {
                    var count = request.Get(ColumnsManager.GetCountName(style.AttributeName), 0);
                    channel.Set(ColumnsManager.GetCountName(style.AttributeName), count);
                    for (var n = 0; n <= count; n++)
                    {
                        channel.Set(ColumnsManager.GetExtendName(style.AttributeName, n), request.Get(ColumnsManager.GetExtendName(style.AttributeName, n), string.Empty));
                    }
                }
                else if (inputType == InputType.CheckBox ||
                         style.InputType == InputType.SelectMultiple)
                {
                    var list = request.Get<List<object>>(style.AttributeName);
                    channel.Set(style.AttributeName, ListUtils.ToString(list));
                }
                else
                {
                    var value = request.Get(style.AttributeName, string.Empty);
                    channel.Set(style.AttributeName, value);
                }
            }

            channel.ChannelName = request.ChannelName;
            channel.IndexName = request.IndexName;
            channel.GroupNames = request.GroupNames;
            //channel.Content = request.Content;
            channel.ChannelTemplateId = request.ChannelTemplateId;
            channel.ContentTemplateId = request.ContentTemplateId;
            channel.LinkUrl = request.LinkUrl;
            channel.LinkType = request.LinkType;
            channel.DefaultTaxisType = request.DefaultTaxisType;
            channel.FilePath = request.FilePath;
            channel.ChannelFilePathRule = request.ChannelFilePathRule;
            channel.ContentFilePathRule = request.ContentFilePathRule;
            channel.Keywords = request.Keywords;
            channel.Description = request.Description;

            await _channelRepository.UpdateAsync(channel);

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