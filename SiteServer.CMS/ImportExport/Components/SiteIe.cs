using System;
using Atom.Core;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.ImportExport.Components
{
    internal class SiteIe
    {
        private readonly SiteInfo _siteInfo;
        private readonly string _siteContentDirectoryPath;
        private readonly ChannelIe _channelIe;
        private readonly ContentIe _contentIe;

        public SiteIe(SiteInfo siteInfo, string siteContentDirectoryPath)
        {
            _siteContentDirectoryPath = siteContentDirectoryPath;
            _siteInfo = siteInfo;
            _channelIe = new ChannelIe(siteInfo);
            _contentIe = new ContentIe(siteInfo, siteContentDirectoryPath);
        }

        public int ImportChannelsAndContents(string filePath, bool isImportContents, bool isOverride, int theParentId, string adminName)
        {
            var psChildCount = DataProvider.ChannelDao.GetCount(_siteInfo.Id);
            var indexNameList = DataProvider.ChannelDao.GetIndexNameList(_siteInfo.Id);

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

            var parentId = DataProvider.ChannelDao.GetId(_siteInfo.Id, orderString);
            if (theParentId != 0)
            {
                parentId = theParentId;
            }

            var parentIdOriginal = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(feed.AdditionalElements, ChannelAttribute.ParentId));
            int channelId;
            if (parentIdOriginal == 0)
            {
                channelId = _siteInfo.Id;
                var nodeInfo = ChannelManager.GetChannelInfo(_siteInfo.Id, _siteInfo.Id);
                _channelIe.ImportNodeInfo(nodeInfo, feed.AdditionalElements, parentId, indexNameList);

                DataProvider.ChannelDao.Update(nodeInfo);

                if (isImportContents)
                {
                    _contentIe.ImportContents(feed.Entries, nodeInfo, 0, isOverride, adminName);
                }
            }
            else
            {
                var nodeInfo = new ChannelInfo();
                _channelIe.ImportNodeInfo(nodeInfo, feed.AdditionalElements, parentId, indexNameList);
                if (string.IsNullOrEmpty(nodeInfo.ChannelName)) return 0;

                var isUpdate = false;
                var theSameNameChannelId = 0;
                if (isOverride)
                {
                    theSameNameChannelId = ChannelManager.GetChannelIdByParentIdAndChannelName(_siteInfo.Id, parentId, nodeInfo.ChannelName, false);
                    if (theSameNameChannelId != 0)
                    {
                        isUpdate = true;
                    }
                }
                if (!isUpdate)
                {
                    channelId = DataProvider.ChannelDao.Insert(nodeInfo);
                }
                else
                {
                    channelId = theSameNameChannelId;
                    nodeInfo = ChannelManager.GetChannelInfo(_siteInfo.Id, theSameNameChannelId);
                    var tableName = ChannelManager.GetTableName(_siteInfo, nodeInfo);
                    _channelIe.ImportNodeInfo(nodeInfo, feed.AdditionalElements, parentId, indexNameList);

                    DataProvider.ChannelDao.Update(nodeInfo);

                    DataProvider.ContentDao.DeleteContentsByChannelId(_siteInfo.Id, tableName, theSameNameChannelId);
                }

                if (isImportContents)
                {
                    _contentIe.ImportContents(feed.Entries, nodeInfo, 0, isOverride, adminName);
                }
            }

            return channelId;
        }

        public void Export(int siteId, int channelId, bool isSaveContents)
        {
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            if (channelInfo == null) return;

            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

            var fileName = DataProvider.ChannelDao.GetOrderStringInSite(channelId);

            var filePath = _siteContentDirectoryPath + PathUtils.SeparatorChar + fileName + ".xml";

            var feed = _channelIe.ExportNodeInfo(channelInfo);

            if (isSaveContents)
            {
                var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxis);
                var contentIdList = DataProvider.ContentDao.GetContentIdListChecked(tableName, channelId, orderByString);
                foreach (var contentId in contentIdList)
                {
                    var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);
                    //ContentUtility.PutImagePaths(siteInfo, contentInfo as BackgroundContentInfo, collection);
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
