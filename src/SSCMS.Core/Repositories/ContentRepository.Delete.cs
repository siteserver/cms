using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Plugins;
using SSCMS.Services;

namespace SSCMS.Core.Repositories
{
    public partial class ContentRepository
    {
        public async Task TrashContentsAsync(Site site, Channel channel, List<int> contentIdList, int adminId)
        {
            if (contentIdList == null || !contentIdList.Any()) return;

            var repository = GetRepository(site, channel);

            var cacheKeys = new List<string>
            {
                GetListKey(repository.TableName, site.Id, channel.Id)
            };
            foreach (var contentId in contentIdList)
            {
                cacheKeys.Add(GetEntityKey(repository.TableName, contentId));
            }

            var referenceSummaries = await GetReferenceIdListAsync(repository.TableName, contentIdList);
            if (referenceSummaries.Count > 0)
            {
                foreach (var referenceSummary in referenceSummaries)
                {
                    await DeleteReferenceContentsAsync(site, referenceSummary);
                }
            }

            await repository.UpdateAsync(
                GetQuery(site.Id, channel.Id)
                    .SetRaw("ChannelId = -ChannelId")
                    .Set(nameof(Content.LastEditAdminId), adminId)
                    .WhereIn(nameof(Content.Id), contentIdList)
                    .CachingRemove(cacheKeys.ToArray())
            );
        }

        public async Task TrashContentsAsync(Site site, int channelId, int adminId)
        {
            var channelIds = await _channelRepository.GetChannelIdsAsync(site.Id, channelId, ScopeType.All);
            foreach (var selectedId in channelIds)
            {
                var selected = await _channelRepository.GetAsync(selectedId);
                var contentIds = await GetContentIdsAsync(site, selected);
                await TrashContentsAsync(site, selected, contentIds, adminId);
            }
        }

        public async Task TrashContentAsync(Site site, Channel channel, int contentId, int adminId)
        {
            var repository = GetRepository(site, channel);

            var cacheKeys = new List<string>
            {
                GetListKey(repository.TableName, site.Id, channel.Id), 
                GetEntityKey(repository.TableName, contentId)
            };

            var referenceSummaries = await GetReferenceIdListAsync(repository.TableName, new List<int>
            {
                contentId
            });
            if (referenceSummaries.Count > 0)
            {
                foreach (var referenceSummary in referenceSummaries)
                {
                    await DeleteReferenceContentsAsync(site, referenceSummary);
                }
            }

            await repository.UpdateAsync(
                GetQuery(site.Id, channel.Id)
                    .SetRaw("ChannelId = -ChannelId")
                    .Set(nameof(Content.LastEditAdminId), adminId)
                    .Where(nameof(Content.Id), contentId)
                    .CachingRemove(cacheKeys.ToArray())
            );
        }

        public async Task DeletePreviewAsync(Site site, Channel channel)
        {
            if (!channel.IsPreviewContentsExists) return;

            var repository = GetRepository(site, channel);
            await repository.DeleteAsync(Q
                .Where(nameof(Content.SiteId), site.Id)
                .Where(nameof(Content.ChannelId), channel.Id)
                .Where(nameof(Content.SourceId), SourceManager.Preview)
            );

            channel.IsPreviewContentsExists = false;
            await _channelRepository.UpdateAsync(channel);
        }

        // 回收站 - 删除选中
        public async Task DeleteTrashAsync(Site site, int channelId, string tableName, List<int> contentIdList, IPluginManager pluginManager)
        {
            if (contentIdList == null || contentIdList.Count == 0) return;

            var repository = GetRepository(tableName);

            var cacheKeys = new List<string>
            {
                GetListKey(repository.TableName, site.Id, channelId)
            };
            foreach (var contentId in contentIdList)
            {
                cacheKeys.Add(GetEntityKey(repository.TableName, contentId));
            }

            await repository.DeleteAsync(Q
                .Where(nameof(Content.SiteId), site.Id)
                .Where(nameof(Content.ChannelId), "<", 0)
                .WhereIn(nameof(Content.Id), contentIdList)
                .CachingRemove(cacheKeys.ToArray())
            );

            var handlers = pluginManager.GetExtensions<PluginContentHandler>();
            foreach (var handler in handlers)
            {
                try
                {
                    foreach (var contentId in contentIdList)
                    {
                        handler.OnDeleted(site.Id, channelId, contentId);
                        await handler.OnDeletedAsync(site.Id, channelId, contentId);
                    }
                }
                catch (Exception ex)
                {
                    await _errorLogRepository.AddErrorLogAsync(ex);
                }
            }

            //foreach (var plugin in oldPluginManager.GetPlugins())
            //{
            //    try
            //    {
            //        plugin.OnContentDeleteCompleted(new ContentEventArgs(site.Id, channel.Id, contentId));
            //    }
            //    catch (Exception ex)
            //    {
            //        await _errorLogRepository.AddErrorLogAsync(plugin.PluginId, ex, nameof(plugin.OnContentDeleteCompleted));
            //    }
            //}
        }

        // 回收站 - 删除全部
        public async Task DeleteTrashAsync(Site site, IPluginManager pluginManager)
        {
            var tableNames = await _siteRepository.GetTableNamesAsync(site);
            foreach (var tableName in tableNames)
            {
                var repository = GetRepository(tableName);

                var channelIds = await repository.GetAllAsync<int>(Q
                    .Select(nameof(Content.ChannelId))
                    .Where(nameof(Content.SiteId), site.Id)
                    .Where(nameof(Content.ChannelId), "<", 0)
                    .Distinct()
                );

                foreach (var channelId in channelIds)
                {
                    var contentIds = await repository.GetAllAsync<int>(Q
                        .Select(nameof(Content.Id))
                        .Where(nameof(Content.SiteId), site.Id)
                        .Where(nameof(Content.ChannelId), channelId)
                        .Distinct()
                    );

                    await DeleteTrashAsync(site, channelId, tableName, contentIds, pluginManager);
                }
            }
        }

        // 回收站 - 恢复全部
        public async Task RestoreTrashAsync(Site site, int restoreChannelId)
        {
            var tableNames = await _siteRepository.GetTableNamesAsync(site);
            foreach (var tableName in tableNames)
            {
                var repository = GetRepository(tableName);

                var cacheKeys = new List<string>
                {
                    GetListKey(repository.TableName, site.Id, restoreChannelId)
                };

                var channelIds = await repository.GetAllAsync<int>(Q
                    .Select(nameof(Content.ChannelId))
                    .Where(nameof(Content.SiteId), site.Id)
                    .Where(nameof(Content.ChannelId), "<", 0)
                    .Distinct()
                );

                cacheKeys.AddRange(channelIds.Select(channelId => GetListKey(tableName, site.Id, channelId)));

                await repository.UpdateAsync(Q
                    .Set(nameof(Content.ChannelId), restoreChannelId)
                    .Where(nameof(Content.SiteId), site.Id)
                    .Where(nameof(Content.ChannelId), "<", 0)
                    .CachingRemove(cacheKeys.ToArray())
                );
            }
        }

        // 回收站 - 恢复选中
        public async Task RestoreTrashAsync(Site site, int channelId, string tableName, List<int> contentIdList, int restoreChannelId)
        {
            if (contentIdList == null || contentIdList.Count == 0) return;

            var repository = GetRepository(tableName);

            var cacheKeys = new List<string>
            {
                GetListKey(repository.TableName, site.Id, restoreChannelId),
                GetListKey(repository.TableName, site.Id, channelId)
            };
            cacheKeys.AddRange(contentIdList.Select(contentId => GetEntityKey(repository.TableName, contentId)));

            await repository.UpdateAsync(Q
                .Set(nameof(Content.ChannelId), restoreChannelId)
                .Where(nameof(Content.SiteId), site.Id)
                .Where(nameof(Content.ChannelId), "<", 0)
                .WhereIn(nameof(Content.Id), contentIdList)
                .CachingRemove(cacheKeys.ToArray())
            );
        }

        private async Task DeleteReferenceContentsAsync(Site site, ContentSummary summary)
        {
            var channel = await _channelRepository.GetAsync(summary.ChannelId);
            var repository = GetRepository(site, channel);

            await repository.DeleteAsync(
                GetQuery(site.Id, channel.Id)
                    .Where(nameof(Content.ReferenceId), ">", 0)
                    .Where(nameof(Content.Id), summary.Id)
                    .CachingRemove(
                        GetListKey(repository.TableName, site.Id, channel.Id),
                        GetEntityKey(repository.TableName, summary.Id)
                    )
            );
        }
    }
}
