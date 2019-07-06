using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Core.Plugin;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentRepository
    {
        public async Task RemoveCacheBySiteIdAsync(string tableName, int siteId)
        {
            foreach (var channelId in await _channelRepository.GetChannelIdListAsync(siteId))
            {
                ListRemove(channelId);
            }
            CountClear(tableName);
        }

        public void RemoveCache(string tableName, int channelId)
        {
            ListRemove(channelId);
            CountClear(tableName);
        }

        public void RemoveCountCache(string tableName)
        {
            CountClear(tableName);
        }

        private async Task InsertCacheAsync(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo)
        {
            if (contentInfo.SourceId == SourceManager.Preview) return;

            ListAdd(channelInfo, contentInfo);

            var tableName = await _channelRepository.GetTableNameAsync(_pluginManager, siteInfo, channelInfo);
            CountAdd(tableName, contentInfo);
        }

        private async Task UpdateCacheAsync(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfoToUpdate)
        {
            var contentInfo = GetContentInfo(contentInfoToUpdate.Id);
            if (contentInfo != null)
            {
                if (ListIsChanged(channelInfo, contentInfo, contentInfoToUpdate))
                {
                    ListRemove(channelInfo.Id);
                }

                if (CountIsChanged(contentInfo, contentInfoToUpdate))
                {
                    var tableName = await _channelRepository.GetTableNameAsync(_pluginManager, siteInfo, channelInfo);
                    CountRemove(tableName, contentInfo);
                    CountAdd(tableName, contentInfoToUpdate);
                }
            }
        }

        public async Task<ContentInfo> CalculateAsync(int sequence, ContentInfo contentInfo, List<ContentColumn> columns, Dictionary<string, Dictionary<string, Func<IContentContext, string>>> pluginColumns)
        {
            if (contentInfo == null) return null;

            var retVal = new ContentInfo(contentInfo.ToDictionary());

            foreach (var column in columns)
            {
                if (!column.IsCalculate) continue;

                if (StringUtils.EqualsIgnoreCase(column.AttributeName, ContentAttribute.Sequence))
                {
                    retVal.Set(ContentAttribute.Sequence, sequence);
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, ContentAttribute.UserId))
                {
                    var value = string.Empty;
                    if (contentInfo.UserId > 0)
                    {
                        var userInfo = await _userRepository.GetByUserIdAsync(contentInfo.UserId);
                        if (userInfo != null)
                        {
                            value = string.IsNullOrEmpty(userInfo.DisplayName) ? userInfo.UserName : userInfo.DisplayName;
                        }
                    }
                    retVal.Set(ContentAttribute.UserId, value);
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, ContentAttribute.SourceId))
                {
                    retVal.Set(ContentAttribute.SourceId, await _channelRepository.GetSourceNameAsync(contentInfo.SourceId));
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, ContentAttribute.AddUserName))
                {
                    var value = string.Empty;
                    if (!string.IsNullOrEmpty(contentInfo.AddUserName))
                    {
                        var userInfo = await _userRepository.GetByUserNameAsync(contentInfo.AddUserName);
                        if (userInfo != null)
                        {
                            value = string.IsNullOrEmpty(userInfo.DisplayName) ? userInfo.UserName : userInfo.DisplayName;
                        }
                    }
                    retVal.Set(ContentAttribute.AddUserName, value);
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, ContentAttribute.LastEditUserName))
                {
                    var value = string.Empty;
                    if (!string.IsNullOrEmpty(contentInfo.LastEditUserName))
                    {
                        var userInfo = await _userRepository.GetByUserNameAsync(contentInfo.LastEditUserName);
                        if (userInfo != null)
                        {
                            value = string.IsNullOrEmpty(userInfo.DisplayName) ? userInfo.UserName : userInfo.DisplayName;
                        }
                    }
                    retVal.Set(ContentAttribute.LastEditUserName, value);
                }
            }

            if (pluginColumns != null)
            {
                foreach (var pluginId in pluginColumns.Keys)
                {
                    var contentColumns = pluginColumns[pluginId];
                    if (contentColumns == null || contentColumns.Count == 0) continue;

                    foreach (var columnName in contentColumns.Keys)
                    {
                        var attributeName = $"{pluginId}:{columnName}";
                        if (columns.All(x => x.AttributeName != attributeName)) continue;

                        try
                        {
                            var func = contentColumns[columnName];
                            var value = func(new ContentContext
                            {
                                SiteId = contentInfo.SiteId,
                                ChannelId = contentInfo.ChannelId,
                                ContentId = contentInfo.Id
                            });

                            retVal.Set(attributeName, value);
                        }
                        catch (Exception ex)
                        {
                            await _errorLogRepository.AddErrorLogAsync(pluginId, ex);
                        }
                    }
                }
            }

            return retVal;
        }

        public bool IsCreatable(ChannelInfo channelInfo, ContentInfo contentInfo)
        {
            if (channelInfo == null || contentInfo == null) return false;

            //引用链接，不需要生成内容页；引用内容，需要生成内容页；
            if (contentInfo.ReferenceId > 0 &&
                TranslateContentType.Parse(contentInfo.TranslateContentType) !=
                TranslateContentType.ReferenceContent)
            {
                return false;
            }

            return channelInfo.IsContentCreatable && string.IsNullOrEmpty(contentInfo.LinkUrl) && contentInfo.IsChecked && contentInfo.SourceId != SourceManager.Preview && contentInfo.ChannelId > 0;
        }
    }
}