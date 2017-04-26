using Atom.Core;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.ImportExport.Components
{
	internal class RelatedFieldIe
	{
		private readonly int _publishmentSystemId;
		private readonly string _directoryPath;

        public RelatedFieldIe(int publishmentSystemId, string directoryPath)
		{
			_publishmentSystemId = publishmentSystemId;
			_directoryPath = directoryPath;
		}

        public void ExportRelatedField(RelatedFieldInfo relatedFieldInfo)
		{
            var filePath = _directoryPath + PathUtils.SeparatorChar + relatedFieldInfo.RelatedFieldID + ".xml";

            var feed = ExportRelatedFieldInfo(relatedFieldInfo);

            var relatedFieldItemInfoList = DataProvider.RelatedFieldItemDao.GetRelatedFieldItemInfoList(relatedFieldInfo.RelatedFieldID, 0);

            foreach (var relatedFieldItemInfo in relatedFieldItemInfoList)
			{
                AddAtomEntry(feed, relatedFieldItemInfo, 1);
			}
			feed.Save(filePath);
		}

        private static AtomFeed ExportRelatedFieldInfo(RelatedFieldInfo relatedFieldInfo)
		{
			var feed = AtomUtility.GetEmptyFeed();

            AtomUtility.AddDcElement(feed.AdditionalElements, "RelatedFieldID", relatedFieldInfo.RelatedFieldID.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, "RelatedFieldName", relatedFieldInfo.RelatedFieldName);
            AtomUtility.AddDcElement(feed.AdditionalElements, "PublishmentSystemID", relatedFieldInfo.PublishmentSystemID.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, "TotalLevel", relatedFieldInfo.TotalLevel.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, "Prefixes", relatedFieldInfo.Prefixes);
            AtomUtility.AddDcElement(feed.AdditionalElements, "Suffixes", relatedFieldInfo.Suffixes);

			return feed;
		}

        private static void AddAtomEntry(AtomFeed feed, RelatedFieldItemInfo relatedFieldItemInfo, int level)
		{
			var entry = AtomUtility.GetEmptyEntry();

            AtomUtility.AddDcElement(entry.AdditionalElements, "ID", relatedFieldItemInfo.ID.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "RelatedFieldID", relatedFieldItemInfo.RelatedFieldID.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "ItemName", relatedFieldItemInfo.ItemName);
            AtomUtility.AddDcElement(entry.AdditionalElements, "ItemValue", relatedFieldItemInfo.ItemValue);
            AtomUtility.AddDcElement(entry.AdditionalElements, "ParentID", relatedFieldItemInfo.ParentID.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "Taxis", relatedFieldItemInfo.Taxis.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "Level", level.ToString());

            feed.Entries.Add(entry);

            var relatedFieldItemInfoList = DataProvider.RelatedFieldItemDao.GetRelatedFieldItemInfoList(relatedFieldItemInfo.RelatedFieldID, relatedFieldItemInfo.ID);

            foreach (var itemInfo in relatedFieldItemInfoList)
            {
                AddAtomEntry(feed, itemInfo, level + 1);
            }
		}

		public void ImportRelatedField(bool overwrite)
		{
			if (!DirectoryUtils.IsDirectoryExists(_directoryPath)) return;
			var filePaths = DirectoryUtils.GetFilePaths(_directoryPath);

			foreach (var filePath in filePaths)
			{
                var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

                var relatedFieldName = AtomUtility.GetDcElementContent(feed.AdditionalElements, "RelatedFieldName");
                var totalLevel = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(feed.AdditionalElements, "TotalLevel"));
                var prefixes = AtomUtility.GetDcElementContent(feed.AdditionalElements, "Prefixes");
                var suffixes = AtomUtility.GetDcElementContent(feed.AdditionalElements, "Suffixes");

                var relatedFieldInfo = new RelatedFieldInfo(0, relatedFieldName, _publishmentSystemId, totalLevel, prefixes, suffixes);

                var srcRelatedFieldInfo = DataProvider.RelatedFieldDao.GetRelatedFieldInfo(_publishmentSystemId, relatedFieldName);
                if (srcRelatedFieldInfo != null)
                {
                    if (overwrite)
                    {
                        DataProvider.RelatedFieldDao.Delete(srcRelatedFieldInfo.RelatedFieldID);
                    }
                    else
                    {
                        relatedFieldInfo.RelatedFieldName = DataProvider.RelatedFieldDao.GetImportRelatedFieldName(_publishmentSystemId, relatedFieldInfo.RelatedFieldName);
                    }
                }

                var relatedFieldId = DataProvider.RelatedFieldDao.Insert(relatedFieldInfo);

                var lastInertedLevel = 1;
                var lastInsertedParentId = 0;
                var lastInsertedId = 0;
				foreach (AtomEntry entry in feed.Entries)
				{
                    var itemName = AtomUtility.GetDcElementContent(entry.AdditionalElements, "ItemName");
                    var itemValue = AtomUtility.GetDcElementContent(entry.AdditionalElements, "ItemValue");
                    var level = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "Level"));
                    var parentId = 0;
                    if (level > 1)
                    {
                        parentId = level != lastInertedLevel ? lastInsertedId : lastInsertedParentId;
                    }

                    var relatedFieldItemInfo = new RelatedFieldItemInfo(0, relatedFieldId, itemName, itemValue, parentId, 0);
                    lastInsertedId = DataProvider.RelatedFieldItemDao.Insert(relatedFieldItemInfo);
                    lastInsertedParentId = parentId;
                    lastInertedLevel = level;
				}
			}
		}

	}
}
