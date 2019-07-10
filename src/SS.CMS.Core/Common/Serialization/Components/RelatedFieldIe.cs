using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Utils;
using SS.CMS.Utils.Atom.Atom.Core;

namespace SS.CMS.Core.Serialization.Components
{
    internal class RelatedFieldIe
    {
        private readonly int _siteId;
        private readonly string _directoryPath;
        private readonly IRelatedFieldRepository _relatedFieldRepository;
        private readonly IRelatedFieldItemRepository _relatedFieldItemRepository;

        public RelatedFieldIe(int siteId, string directoryPath)
        {
            _siteId = siteId;
            _directoryPath = directoryPath;
        }

        public async Task ExportRelatedFieldAsync(RelatedField relatedFieldInfo)
        {
            var filePath = _directoryPath + PathUtils.SeparatorChar + relatedFieldInfo.Id + ".xml";

            var feed = ExportRelatedFieldInfo(relatedFieldInfo);

            var relatedFieldItemInfoList = await _relatedFieldItemRepository.GetRelatedFieldItemInfoListAsync(relatedFieldInfo.Id, 0);

            foreach (var relatedFieldItemInfo in relatedFieldItemInfoList)
            {
                await AddAtomEntryAsync(_relatedFieldItemRepository, feed, relatedFieldItemInfo, 1);
            }
            feed.Save(filePath);
        }

        private static AtomFeed ExportRelatedFieldInfo(RelatedField relatedFieldInfo)
        {
            var feed = AtomUtility.GetEmptyFeed();

            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(RelatedField.Id), "RelatedFieldID" }, relatedFieldInfo.Id.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(RelatedField.Title), "RelatedFieldName" }, relatedFieldInfo.Title);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(RelatedField.SiteId), "PublishmentSystemID" }, relatedFieldInfo.SiteId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(RelatedField.TotalLevel), relatedFieldInfo.TotalLevel.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(RelatedField.Prefixes), relatedFieldInfo.Prefixes);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(RelatedField.Suffixes), relatedFieldInfo.Suffixes);

            return feed;
        }

        private static async Task AddAtomEntryAsync(IRelatedFieldItemRepository relatedFieldItemRepository, AtomFeed feed, RelatedFieldItem relatedFieldItemInfo, int level)
        {
            var entry = AtomUtility.GetEmptyEntry();

            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(RelatedFieldItem.Id), "ID" }, relatedFieldItemInfo.Id.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(RelatedFieldItem.RelatedFieldId), "RelatedFieldID" }, relatedFieldItemInfo.RelatedFieldId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(RelatedFieldItem.ItemName), relatedFieldItemInfo.ItemName);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(RelatedFieldItem.ItemValue), relatedFieldItemInfo.ItemValue);
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(RelatedFieldItem.ParentId), "ParentID" }, relatedFieldItemInfo.ParentId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(RelatedFieldItem.Taxis), relatedFieldItemInfo.Taxis.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "Level", level.ToString());

            feed.Entries.Add(entry);

            var relatedFieldItemInfoList = await relatedFieldItemRepository.GetRelatedFieldItemInfoListAsync(relatedFieldItemInfo.RelatedFieldId, relatedFieldItemInfo.Id);

            foreach (var itemInfo in relatedFieldItemInfoList)
            {
                await AddAtomEntryAsync(relatedFieldItemRepository, feed, itemInfo, level + 1);
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
                    Title = title,
                    SiteId = _siteId,
                    TotalLevel = totalLevel,
                    Prefixes = prefixes,
                    Suffixes = suffixes,
                };

                var srcRelatedFieldInfo = await _relatedFieldRepository.GetRelatedFieldInfoAsync(_siteId, title);
                if (srcRelatedFieldInfo != null)
                {
                    if (overwrite)
                    {
                        await _relatedFieldRepository.DeleteAsync(srcRelatedFieldInfo.Id);
                    }
                    else
                    {
                        relatedFieldInfo.Title = await _relatedFieldRepository.GetImportTitleAsync(_siteId, relatedFieldInfo.Title);
                    }
                }

                var relatedFieldId = await _relatedFieldRepository.InsertAsync(relatedFieldInfo);

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

                    var itemInfo = new RelatedFieldItem
                    {
                        RelatedFieldId = relatedFieldId,
                        ItemName = itemName,
                        ItemValue = itemValue,
                        ParentId = parentId
                    };

                    lastInsertedId = await _relatedFieldItemRepository.InsertAsync(itemInfo);
                    lastInsertedParentId = parentId;
                    lastInertedLevel = level;
                }
            }
        }

    }
}
