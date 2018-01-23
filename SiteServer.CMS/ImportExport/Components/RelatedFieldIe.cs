using Atom.Core;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.ImportExport.Components
{
	internal class RelatedFieldIe
	{
		private readonly int _siteId;
		private readonly string _directoryPath;

        public RelatedFieldIe(int siteId, string directoryPath)
		{
			_siteId = siteId;
			_directoryPath = directoryPath;
		}

        public void ExportRelatedField(RelatedFieldInfo relatedFieldInfo)
		{
            var filePath = _directoryPath + PathUtils.SeparatorChar + relatedFieldInfo.Id + ".xml";

            var feed = ExportRelatedFieldInfo(relatedFieldInfo);

            var relatedFieldItemInfoList = DataProvider.RelatedFieldItemDao.GetRelatedFieldItemInfoList(relatedFieldInfo.Id, 0);

            foreach (var relatedFieldItemInfo in relatedFieldItemInfoList)
			{
                AddAtomEntry(feed, relatedFieldItemInfo, 1);
			}
			feed.Save(filePath);
		}

        private static AtomFeed ExportRelatedFieldInfo(RelatedFieldInfo relatedFieldInfo)
		{
			var feed = AtomUtility.GetEmptyFeed();

            AtomUtility.AddDcElement(feed.AdditionalElements, "Id", relatedFieldInfo.Id.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, "Title", relatedFieldInfo.Title);
            AtomUtility.AddDcElement(feed.AdditionalElements, "SiteId", relatedFieldInfo.SiteId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, "TotalLevel", relatedFieldInfo.TotalLevel.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, "Prefixes", relatedFieldInfo.Prefixes);
            AtomUtility.AddDcElement(feed.AdditionalElements, "Suffixes", relatedFieldInfo.Suffixes);

			return feed;
		}

        private static void AddAtomEntry(AtomFeed feed, RelatedFieldItemInfo relatedFieldItemInfo, int level)
		{
			var entry = AtomUtility.GetEmptyEntry();

            AtomUtility.AddDcElement(entry.AdditionalElements, "ID", relatedFieldItemInfo.Id.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "RelatedFieldID", relatedFieldItemInfo.RelatedFieldId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "ItemName", relatedFieldItemInfo.ItemName);
            AtomUtility.AddDcElement(entry.AdditionalElements, "ItemValue", relatedFieldItemInfo.ItemValue);
            AtomUtility.AddDcElement(entry.AdditionalElements, "ParentID", relatedFieldItemInfo.ParentId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "Taxis", relatedFieldItemInfo.Taxis.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "Level", level.ToString());

            feed.Entries.Add(entry);

            var relatedFieldItemInfoList = DataProvider.RelatedFieldItemDao.GetRelatedFieldItemInfoList(relatedFieldItemInfo.RelatedFieldId, relatedFieldItemInfo.Id);

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

                var relatedFieldInfo = new RelatedFieldInfo(0, relatedFieldName, _siteId, totalLevel, prefixes, suffixes);

                var srcRelatedFieldInfo = DataProvider.RelatedFieldDao.GetRelatedFieldInfo(_siteId, relatedFieldName);
                if (srcRelatedFieldInfo != null)
                {
                    if (overwrite)
                    {
                        DataProvider.RelatedFieldDao.Delete(srcRelatedFieldInfo.Id);
                    }
                    else
                    {
                        relatedFieldInfo.Title = DataProvider.RelatedFieldDao.GetImportTitle(_siteId, relatedFieldInfo.Title);
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
