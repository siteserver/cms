using System.Collections.Generic;
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

        public void ExportRelatedField(RelatedFieldInfo relatedFieldInfo)
        {
            var filePath = _directoryPath + PathUtils.SeparatorChar + relatedFieldInfo.Id + ".xml";

            var feed = ExportRelatedFieldInfo(relatedFieldInfo);

            var relatedFieldItemInfoList = _relatedFieldItemRepository.GetRelatedFieldItemInfoList(relatedFieldInfo.Id, 0);

            foreach (var relatedFieldItemInfo in relatedFieldItemInfoList)
            {
                AddAtomEntry(_relatedFieldItemRepository, feed, relatedFieldItemInfo, 1);
            }
            feed.Save(filePath);
        }

        private static AtomFeed ExportRelatedFieldInfo(RelatedFieldInfo relatedFieldInfo)
        {
            var feed = AtomUtility.GetEmptyFeed();

            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(RelatedFieldInfo.Id), "RelatedFieldID" }, relatedFieldInfo.Id.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(RelatedFieldInfo.Title), "RelatedFieldName" }, relatedFieldInfo.Title);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(RelatedFieldInfo.SiteId), "PublishmentSystemID" }, relatedFieldInfo.SiteId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(RelatedFieldInfo.TotalLevel), relatedFieldInfo.TotalLevel.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(RelatedFieldInfo.Prefixes), relatedFieldInfo.Prefixes);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(RelatedFieldInfo.Suffixes), relatedFieldInfo.Suffixes);

            return feed;
        }

        private static void AddAtomEntry(IRelatedFieldItemRepository relatedFieldItemRepository, AtomFeed feed, RelatedFieldItemInfo relatedFieldItemInfo, int level)
        {
            var entry = AtomUtility.GetEmptyEntry();

            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(RelatedFieldItemInfo.Id), "ID" }, relatedFieldItemInfo.Id.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(RelatedFieldItemInfo.RelatedFieldId), "RelatedFieldID" }, relatedFieldItemInfo.RelatedFieldId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(RelatedFieldItemInfo.ItemName), relatedFieldItemInfo.ItemName);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(RelatedFieldItemInfo.ItemValue), relatedFieldItemInfo.ItemValue);
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(RelatedFieldItemInfo.ParentId), "ParentID" }, relatedFieldItemInfo.ParentId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(RelatedFieldItemInfo.Taxis), relatedFieldItemInfo.Taxis.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "Level", level.ToString());

            feed.Entries.Add(entry);

            var relatedFieldItemInfoList = relatedFieldItemRepository.GetRelatedFieldItemInfoList(relatedFieldItemInfo.RelatedFieldId, relatedFieldItemInfo.Id);

            foreach (var itemInfo in relatedFieldItemInfoList)
            {
                AddAtomEntry(relatedFieldItemRepository, feed, itemInfo, level + 1);
            }
        }

        public void ImportRelatedField(bool overwrite)
        {
            if (!DirectoryUtils.IsDirectoryExists(_directoryPath)) return;
            var filePaths = DirectoryUtils.GetFilePaths(_directoryPath);

            foreach (var filePath in filePaths)
            {
                var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

                var title = AtomUtility.GetDcElementContent(feed.AdditionalElements, new List<string> { nameof(RelatedFieldInfo.Title), "RelatedFieldName" });
                var totalLevel = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(RelatedFieldInfo.TotalLevel)));
                var prefixes = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(RelatedFieldInfo.Prefixes));
                var suffixes = AtomUtility.GetDcElementContent(feed.AdditionalElements, nameof(RelatedFieldInfo.Suffixes));

                var relatedFieldInfo = new RelatedFieldInfo
                {
                    Title = title,
                    SiteId = _siteId,
                    TotalLevel = totalLevel,
                    Prefixes = prefixes,
                    Suffixes = suffixes,
                };

                var srcRelatedFieldInfo = _relatedFieldRepository.GetRelatedFieldInfo(_siteId, title);
                if (srcRelatedFieldInfo != null)
                {
                    if (overwrite)
                    {
                        _relatedFieldRepository.Delete(srcRelatedFieldInfo.Id);
                    }
                    else
                    {
                        relatedFieldInfo.Title = _relatedFieldRepository.GetImportTitle(_siteId, relatedFieldInfo.Title);
                    }
                }

                var relatedFieldId = _relatedFieldRepository.Insert(relatedFieldInfo);

                var lastInertedLevel = 1;
                var lastInsertedParentId = 0;
                var lastInsertedId = 0;
                foreach (AtomEntry entry in feed.Entries)
                {
                    var itemName = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(RelatedFieldItemInfo.ItemName));
                    var itemValue = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(RelatedFieldItemInfo.ItemValue));
                    var level = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "Level"));
                    var parentId = 0;
                    if (level > 1)
                    {
                        parentId = level != lastInertedLevel ? lastInsertedId : lastInsertedParentId;
                    }

                    var itemInfo = new RelatedFieldItemInfo
                    {
                        RelatedFieldId = relatedFieldId,
                        ItemName = itemName,
                        ItemValue = itemValue,
                        ParentId = parentId
                    };

                    lastInsertedId = _relatedFieldItemRepository.Insert(itemInfo);
                    lastInsertedParentId = parentId;
                    lastInertedLevel = level;
                }
            }
        }

    }
}
