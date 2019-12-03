using System.Collections.Generic;
using System.Threading.Tasks;
using SiteServer.CMS.Context.Atom.Atom.Core;
using SiteServer.Abstractions;
using SiteServer.CMS.Repositories;

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

        public async Task ExportRelatedFieldAsync(RelatedField relatedField)
		{
            var filePath = _directoryPath + PathUtils.SeparatorChar + relatedField.Id + ".xml";

            var feed = ExportRelatedFieldInfo(relatedField);

            var relatedFieldItemInfoList = await DataProvider.RelatedFieldItemRepository.GetRelatedFieldItemInfoListAsync(relatedField.Id, 0);

            foreach (var relatedFieldItemInfo in relatedFieldItemInfoList)
			{
                await AddAtomEntryAsync(feed, relatedFieldItemInfo, 1);
			}
			feed.Save(filePath);
		}

        private static AtomFeed ExportRelatedFieldInfo(RelatedField relatedField)
		{
			var feed = AtomUtility.GetEmptyFeed();

            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(RelatedField.Id), "RelatedFieldID" }, relatedField.Id.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(RelatedField.Title), "RelatedFieldName" }, relatedField.Title);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(RelatedField.SiteId), "PublishmentSystemID" }, relatedField.SiteId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(RelatedField.TotalLevel), relatedField.TotalLevel.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(RelatedField.Prefixes), relatedField.Prefixes);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(RelatedField.Suffixes), relatedField.Suffixes);

			return feed;
		}

        private static async Task AddAtomEntryAsync(AtomFeed feed, RelatedFieldItem relatedFieldItem, int level)
		{
			var entry = AtomUtility.GetEmptyEntry();

            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(RelatedFieldItem.Id), "ID" }, relatedFieldItem.Id.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(RelatedFieldItem.RelatedFieldId), "RelatedFieldID" }, relatedFieldItem.RelatedFieldId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(RelatedFieldItem.ItemName), relatedFieldItem.ItemName);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(RelatedFieldItem.ItemValue), relatedFieldItem.ItemValue);
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(RelatedFieldItem.ParentId), "ParentID" }, relatedFieldItem.ParentId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(RelatedFieldItem.Taxis), relatedFieldItem.Taxis.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "Level", level.ToString());

            feed.Entries.Add(entry);

            var relatedFieldItemInfoList = await DataProvider.RelatedFieldItemRepository.GetRelatedFieldItemInfoListAsync(relatedFieldItem.RelatedFieldId, relatedFieldItem.Id);

            foreach (var itemInfo in relatedFieldItemInfoList)
            {
                await AddAtomEntryAsync(feed, itemInfo, level + 1);
            }
		}

		public async Task ImportRelatedFieldAsync(bool overwrite)
		{
			if (!DirectoryUtils.IsDirectoryExists(_directoryPath)) return;
			var filePaths = DirectoryUtils.GetFilePaths(_directoryPath);

			foreach (var filePath in filePaths)
			{
                var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

                var title = AtomUtility.GetDcElementContent(feed.AdditionalElements, new List<string> { nameof(RelatedField.Title), "RelatedFieldName" });
                var totalLevel = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(RelatedField.TotalLevel)));
                var prefixes = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(RelatedField.Prefixes));
                var suffixes = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(RelatedField.Suffixes));

                var relatedFieldInfo = new RelatedField
                {
                    Id = 0,
                    Title = title,
                    SiteId = _siteId,
                    TotalLevel = totalLevel,
                    Prefixes = prefixes,
                    Suffixes = suffixes
                };

                var srcRelatedFieldInfo = await DataProvider.RelatedFieldRepository.GetRelatedFieldAsync(_siteId, title);
                if (srcRelatedFieldInfo != null)
                {
                    if (overwrite)
                    {
                        await DataProvider.RelatedFieldRepository.DeleteAsync(srcRelatedFieldInfo.Id);
                    }
                    else
                    {
                        relatedFieldInfo.Title = await DataProvider.RelatedFieldRepository.GetImportTitleAsync(_siteId, relatedFieldInfo.Title);
                    }
                }

                var relatedFieldId = await DataProvider.RelatedFieldRepository.InsertAsync(relatedFieldInfo);

                var lastInertedLevel = 1;
                var lastInsertedParentId = 0;
                var lastInsertedId = 0;
				foreach (AtomEntry entry in feed.Entries)
				{
                    var itemName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(RelatedFieldItem.ItemName));
                    var itemValue = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(RelatedFieldItem.ItemValue));
                    var level = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "Level"));
                    var parentId = 0;
                    if (level > 1)
                    {
                        parentId = level != lastInertedLevel ? lastInsertedId : lastInsertedParentId;
                    }

                    var relatedFieldItemInfo = new RelatedFieldItem
                    {
                        Id = 0,
                        RelatedFieldId = relatedFieldId,
                        ItemName = itemName,
                        ItemValue = itemValue,
                        ParentId = parentId,
                        Taxis = 0
                    };
                    lastInsertedId = await DataProvider.RelatedFieldItemRepository.InsertAsync(relatedFieldItemInfo);
                    lastInsertedParentId = parentId;
                    lastInertedLevel = level;
				}
			}
		}

	}
}
