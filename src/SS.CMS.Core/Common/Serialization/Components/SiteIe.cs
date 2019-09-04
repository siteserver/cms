using System;
using System.Threading.Tasks;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;
using SS.CMS.Utils.Atom.Atom.Core;

namespace SS.CMS.Core.Serialization.Components
{
    internal class SiteIe
    {
        private readonly Site _site;
        private readonly string _siteContentDirectoryPath;
        private readonly ChannelIe _channelIe;
        private readonly ContentIe _contentIe;
        private readonly IPluginManager _pluginManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;

        public SiteIe(Site site, string siteContentDirectoryPath)
        {
            _siteContentDirectoryPath = siteContentDirectoryPath;
            _site = site;
            _channelIe = new ChannelIe(site);
            _contentIe = new ContentIe(site, siteContentDirectoryPath);
        }

        public async Task<int> ImportChannelsAndContentsAsync(string filePath, bool isImportContents, bool isOverride, int theParentId, int userId)
        {
            var psChildCount = await _channelRepository.GetCountAsync(_site.Id);
            var indexNameList = await _channelRepository.GetIndexNameListAsync(_site.Id);

            if (!FileUtils.IsFileExists(filePath)) return 0;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

            var firstIndex = filePath.LastIndexOf(PathUtils.SeparatorChar) + 1;
            var lastIndex = filePath.LastIndexOf(".", StringComparison.Ordinal);
            var orderString = filePath.Substring(firstIndex, lastIndex - firstIndex);

            var idx = orderString.IndexOf("_", StringComparison.Ordinal);
            if (idx != -1)
            {
                var secondOrder = TranslateUtils.ToInt(orderString.Split('_')[1]);
                secondOrder = secondOrder + psChildCount;
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

            var parentId = await _channelRepository.GetIdAsync(_site.Id, orderString);
            if (theParentId != 0)
            {
                parentId = theParentId;
            }

            var parentIdOriginal = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(feed.AdditionalElements, ChannelAttribute.ParentId));
            int channelId;
            if (parentIdOriginal == 0)
            {
                channelId = _site.Id;
                var node = await _channelRepository.GetChannelAsync(_site.Id);
                await _channelIe.ImportChannelAsync(node, feed.AdditionalElements, parentId, indexNameList);

                await _channelRepository.UpdateAsync(node);

                if (isImportContents)
                {
                    await _contentIe.ImportContentsAsync(feed.Entries, node, 0, isOverride, userId);
                }
            }
            else
            {
                var node = new Channel();
                await _channelIe.ImportChannelAsync(node, feed.AdditionalElements, parentId, indexNameList);
                if (string.IsNullOrEmpty(node.ChannelName)) return 0;

                var isUpdate = false;
                var theSameNameChannelId = 0;
                if (isOverride)
                {
                    theSameNameChannelId = await _channelRepository.GetIdByParentIdAndChannelNameAsync(_site.Id, parentId, node.ChannelName, false);
                    if (theSameNameChannelId != 0)
                    {
                        isUpdate = true;
                    }
                }
                if (!isUpdate)
                {
                    channelId = await _channelRepository.InsertAsync(node);
                }
                else
                {
                    channelId = theSameNameChannelId;
                    node = await _channelRepository.GetChannelAsync(theSameNameChannelId);
                    var tableName = _channelRepository.GetTableName(_site, node);
                    await _channelIe.ImportChannelAsync(node, feed.AdditionalElements, parentId, indexNameList);

                    await _channelRepository.UpdateAsync(node);

                    //DataProvider.ContentDao.DeleteContentsByChannelId(_site.Id, tableName, theSameNameChannelId);
                }

                if (isImportContents)
                {
                    await _contentIe.ImportContentsAsync(feed.Entries, node, 0, isOverride, userId);
                }
            }

            return channelId;
        }

        public async Task ExportAsync(int siteId, int channelId, bool isSaveContents)
        {
            var channel = await _channelRepository.GetChannelAsync(channelId);
            if (channel == null) return;

            var site = await _siteRepository.GetSiteAsync(siteId);

            var fileName = await _channelRepository.GetOrderStringInSiteAsync(channelId);

            var filePath = _siteContentDirectoryPath + PathUtils.SeparatorChar + fileName + ".xml";

            var contentRepository = _channelRepository.GetContentRepository(_site, channel);

            var feed = await _channelIe.ExportChannelAsync(channel);

            if (isSaveContents)
            {
                var contentIdList = await contentRepository.GetContentIdListCheckedAsync(channelId, TaxisType.OrderByTaxis);
                foreach (var contentId in contentIdList)
                {
                    var content = await contentRepository.GetContentInfoAsync(contentId);
                    //ContentUtility.PutImagePaths(site, content as BackgroundContent, collection);
                    var entry = _contentIe.ExportContentInfo(content);
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
