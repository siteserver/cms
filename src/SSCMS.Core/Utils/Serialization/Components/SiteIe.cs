using System;
using System.Linq;
using System.Threading.Tasks;
using SSCMS;
using SSCMS.Core.Utils.Serialization.Atom.Atom.Core;
using SSCMS.Utils;

namespace SSCMS.Core.Utils.Serialization.Components
{
    internal class SiteIe
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly Site _site;
        private readonly string _siteContentDirectoryPath;
        private readonly ChannelIe _channelIe;
        private readonly ContentIe _contentIe;

        public SiteIe(IPathManager pathManager, IDatabaseManager databaseManager, Site site, string siteContentDirectoryPath)
        {
            _databaseManager = databaseManager;
            _siteContentDirectoryPath = siteContentDirectoryPath;
            _site = site;
            _channelIe = new ChannelIe(_databaseManager, site);
            _contentIe = new ContentIe(pathManager, databaseManager, site, siteContentDirectoryPath);
        }

        public async Task<int> ImportChannelsAndContentsAsync(string filePath, bool isImportContents, bool isOverride, int theParentId, int adminId, string guid)
        {
            var count = await _databaseManager.ChannelRepository.ImportGetCountAsync(_site.Id, _site.Id);
            var indexNameList = await _databaseManager.ChannelRepository.ImportGetIndexNameListAsync(_site.Id);

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

            var parentId = await _databaseManager.ChannelRepository.ImportGetIdAsync(_site.Id, orderString);
            if (theParentId != 0)
            {
                parentId = theParentId;
            }

            var parentIdOriginal = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(Channel.ParentId)));
            int channelId;
            if (parentIdOriginal == 0)
            {
                channelId = _site.Id;
                var channel = await _databaseManager.ChannelRepository.ImportGetAsync(_site.Id);
                await _channelIe.ImportChannelAsync(channel, feed.AdditionalElements, parentId, indexNameList);

                Caching.SetProcess(guid, $"导入栏目: {channel.ChannelName}");
                await _databaseManager.ChannelRepository.UpdateAsync(channel);

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
                    theSameNameChannelId = await _databaseManager.ChannelRepository.ImportGetChannelIdByParentIdAndChannelNameAsync(_site.Id, parentId, channel.ChannelName, false);
                    if (theSameNameChannelId != 0)
                    {
                        isUpdate = true;
                    }
                }
                if (!isUpdate)
                {
                    Caching.SetProcess(guid, $"导入栏目: {channel.ChannelName}");
                    channelId = await _databaseManager.ChannelRepository.InsertAsync(channel);
                }
                else
                {
                    channelId = theSameNameChannelId;
                    channel = await _databaseManager.ChannelRepository.ImportGetAsync(theSameNameChannelId);
                    //var tableName = _databaseManager.ChannelRepository.GetTableName(_site, node);
                    await _channelIe.ImportChannelAsync(channel, feed.AdditionalElements, parentId, indexNameList);

                    Caching.SetProcess(guid, $"导入栏目: {channel.ChannelName}");
                    await _databaseManager.ChannelRepository.UpdateAsync(channel);

                    //_databaseManager.ContentRepository.DeleteContentsByChannelId(_site.Id, tableName, theSameNameChannelId);
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
            var channelInfo = await _databaseManager.ChannelRepository.ImportGetAsync(channelId);
            if (channelInfo == null) return;

            var fileName = await _databaseManager.ChannelRepository.ImportGetOrderStringInSiteAsync(site.Id, channelId);

            var filePath = _siteContentDirectoryPath + PathUtils.SeparatorChar + fileName + ".xml";

            var feed = await _channelIe.ExportChannelAsync(channelInfo);

            if (isSaveContents)
            {
                var summaries = await _databaseManager.ContentRepository.GetSummariesAsync(site, channelInfo, false);
                var contentIds = summaries.Select(x => x.Id).ToList();
                contentIds.Reverse();
                
                foreach (var contentId in contentIds)
                {
                    var contentInfo = await _databaseManager.ContentRepository.GetAsync(site, channelInfo, contentId);
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
