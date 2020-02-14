using System;
using System.Linq;
using System.Threading.Tasks;
using SiteServer.CMS.Context.Atom.Atom.Core;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;

namespace SiteServer.CMS.ImportExport.Components
{
    internal class SiteIe
    {
        private readonly Site _site;
        private readonly string _siteContentDirectoryPath;
        private readonly ChannelIe _channelIe;
        private readonly ContentIe _contentIe;

        public SiteIe(Site site, string siteContentDirectoryPath)
        {
            _siteContentDirectoryPath = siteContentDirectoryPath;
            _site = site;
            _channelIe = new ChannelIe(site);
            _contentIe = new ContentIe(site, siteContentDirectoryPath);
        }

        public async Task<int> ImportChannelsAndContentsAsync(string filePath, bool isImportContents, bool isOverride, int theParentId, int adminId, string guid)
        {
            var count = await DataProvider.ChannelRepository.ImportGetCountAsync(_site.Id, _site.Id);
            var indexNameList = await DataProvider.ChannelRepository.ImportGetIndexNameListAsync(_site.Id);

            if (!FileUtils.IsFileExists(filePath)) return 0;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

            var firstIndex = filePath.LastIndexOf(PathUtils.SeparatorChar) + 1;
            var lastIndex = filePath.LastIndexOf(".", StringComparison.Ordinal);
            var orderString = filePath.Substring(firstIndex, lastIndex - firstIndex);

            var idx = orderString.IndexOf("_", StringComparison.Ordinal);
            if (idx != -1)
            {
                var secondOrder = TranslateUtils.ToInt(orderString.Split('_')[1]);
                secondOrder = secondOrder + count;
                orderString = orderString.Substring(idx + 1);
                idx = orderString.IndexOf("_", StringComparison.Ordinal);
                if (idx != -1)
                {
                    orderString = orderString.Substring(idx);
                    orderString = "1_" + secondOrder + orderString;
                }
                else
                {
                    orderString = "1_" + secondOrder;
                }

                orderString = orderString.Substring(0, orderString.LastIndexOf("_", StringComparison.Ordinal));
            }

            var parentId = await DataProvider.ChannelRepository.ImportGetIdAsync(_site.Id, orderString);
            if (theParentId != 0)
            {
                parentId = theParentId;
            }

            var parentIdOriginal = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(Channel.ParentId)));
            int channelId;
            if (parentIdOriginal == 0)
            {
                channelId = _site.Id;
                var channel = await DataProvider.ChannelRepository.ImportGetAsync(_site.Id);
                await _channelIe.ImportChannelAsync(channel, feed.AdditionalElements, parentId, indexNameList);

                Caching.SetProcess(guid, $"导入栏目: {channel.ChannelName}");
                await DataProvider.ChannelRepository.UpdateAsync(channel);

                if (isImportContents)
                {
                    await _contentIe.ImportContentsAsync(feed.Entries, channel, 0, isOverride, adminId, guid);
                }
            }
            else
            {
                var channel = new Channel();
                await _channelIe.ImportChannelAsync(channel, feed.AdditionalElements, parentId, indexNameList);
                if (string.IsNullOrEmpty(channel.ChannelName)) return 0;

                var isUpdate = false;
                var theSameNameChannelId = 0;
                if (isOverride)
                {
                    theSameNameChannelId = await DataProvider.ChannelRepository.ImportGetChannelIdByParentIdAndChannelNameAsync(_site.Id, parentId, channel.ChannelName, false);
                    if (theSameNameChannelId != 0)
                    {
                        isUpdate = true;
                    }
                }
                if (!isUpdate)
                {
                    Caching.SetProcess(guid, $"导入栏目: {channel.ChannelName}");
                    channelId = await DataProvider.ChannelRepository.InsertAsync(channel);
                }
                else
                {
                    channelId = theSameNameChannelId;
                    channel = await DataProvider.ChannelRepository.ImportGetAsync(theSameNameChannelId);
                    //var tableName = DataProvider.ChannelRepository.GetTableName(_site, node);
                    await _channelIe.ImportChannelAsync(channel, feed.AdditionalElements, parentId, indexNameList);

                    Caching.SetProcess(guid, $"导入栏目: {channel.ChannelName}");
                    await DataProvider.ChannelRepository.UpdateAsync(channel);

                    //DataProvider.ContentRepository.DeleteContentsByChannelId(_site.Id, tableName, theSameNameChannelId);
                }

                if (isImportContents)
                {
                    await _contentIe.ImportContentsAsync(feed.Entries, channel, 0, isOverride, adminId, guid);
                }
            }

            return channelId;
        }

        public async Task ExportAsync(Site site, int channelId, bool isSaveContents)
        {
            var channelInfo = await DataProvider.ChannelRepository.ImportGetAsync(channelId);
            if (channelInfo == null) return;

            var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channelInfo);

            var fileName = await DataProvider.ChannelRepository.ImportGetOrderStringInSiteAsync(site.Id, channelId);

            var filePath = _siteContentDirectoryPath + PathUtils.SeparatorChar + fileName + ".xml";

            var feed = await _channelIe.ExportChannelAsync(channelInfo);

            if (isSaveContents)
            {
                var orderByString = ETaxisTypeUtils.GetContentOrderByString(TaxisType.OrderByTaxis);
                var contentIdList = DataProvider.ContentRepository.GetContentIdListChecked(tableName, channelId, orderByString);
                foreach (var contentId in contentIdList)
                {
                    var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelInfo, contentId);
                    //ContentUtility.PutImagePaths(site, contentInfo as BackgroundContentInfo, collection);
                    var entry = _contentIe.ExportContentInfo(contentInfo);
                    feed.Entries.Add(entry);

                }
            }
            feed.Save(filePath);

            //  foreach (string imageUrl in collection.Keys)
            //  {
            //     string sourceFilePath = collection[imageUrl];
            //     string destFilePath = PathUtility.MapPath(this.siteContentDirectoryPath, imageUrl);
            //     DirectoryUtils.CreateDirectoryIfNotExists(destFilePath);
            //     FileUtils.MoveFile(sourceFilePath, destFilePath, true);
            //  }
        }
    }
}
