using System.Collections.Generic;
using Atom.Core;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.CMS.ImportExport.Components
{
    internal class ContentModelIe
    {
        private readonly int _publishmentSystemId;
        private readonly string _filePath;

        public ContentModelIe(int publishmentSystemId, string filePath)
        {
            _publishmentSystemId = publishmentSystemId;
            _filePath = filePath;
        }

        public void ExportContentModel(List<ContentModelInfo> conetentModelInfoList)
        {
            var feed = AtomUtility.GetEmptyFeed();

            foreach (var contentModelInfo in conetentModelInfoList)
            {
                var entry = ExportContentModelInfo(contentModelInfo);
                feed.Entries.Add(entry);
            }

            feed.Save(_filePath);
        }

        private static AtomEntry ExportContentModelInfo(ContentModelInfo contentModelInfo)
        {
            var entry = AtomUtility.GetEmptyEntry();

            AtomUtility.AddDcElement(entry.AdditionalElements, "ModelID", contentModelInfo.ModelId);
            AtomUtility.AddDcElement(entry.AdditionalElements, "SiteID", contentModelInfo.SiteId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "ModelName", contentModelInfo.ModelName);
            AtomUtility.AddDcElement(entry.AdditionalElements, "IsSystem", contentModelInfo.IsSystem.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "TableName", contentModelInfo.TableName);
            AtomUtility.AddDcElement(entry.AdditionalElements, "TableType", contentModelInfo.TableType.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "IconUrl", contentModelInfo.IconUrl);
            AtomUtility.AddDcElement(entry.AdditionalElements, "Description", contentModelInfo.Description);

            return entry;
        }


        public void ImportContentModelInfo(bool overwrite)
        {
            if (!FileUtils.IsFileExists(_filePath)) return;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(_filePath));

            foreach (AtomEntry entry in feed.Entries)
            {
                var modelId = AtomUtility.GetDcElementContent(entry.AdditionalElements, "ModelID");

                if (!string.IsNullOrEmpty(modelId))
                {
                    var contentModelInfo = new ContentModelInfo
                    {
                        ModelId = modelId,
                        SiteId = _publishmentSystemId,
                        ModelName = AtomUtility.GetDcElementContent(entry.AdditionalElements, "ModelName"),
                        IsSystem =
                            TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, "IsSystem")),
                        TableName = AtomUtility.GetDcElementContent(entry.AdditionalElements, "TableName"),
                        TableType =
                            EAuxiliaryTableTypeUtils.GetEnumType(
                                AtomUtility.GetDcElementContent(entry.AdditionalElements, "TableType")),
                        IconUrl = AtomUtility.GetDcElementContent(entry.AdditionalElements, "IconUrl"),
                        Description = AtomUtility.GetDcElementContent(entry.AdditionalElements, "Description")
                    };

                    var auxiliaryTableInfo = BaiRongDataProvider.TableCollectionDao.GetAuxiliaryTableInfo(modelId);
                    if (auxiliaryTableInfo != null && auxiliaryTableInfo.TableEnName == modelId)
                    {
                        if (overwrite)
                        {
                            BaiRongDataProvider.ContentModelDao.Update(contentModelInfo);
                        }
                        else
                        {
                            var importContentModelId = BaiRongDataProvider.ContentModelDao.GetImportContentModelId(_publishmentSystemId, contentModelInfo.ModelId);
                            contentModelInfo.ModelId = importContentModelId;
                            BaiRongDataProvider.ContentModelDao.Insert(contentModelInfo);
                        }
                    }
                    else
                    {
                        BaiRongDataProvider.ContentModelDao.Insert(contentModelInfo);
                    }
                }
            }
        }

    }
}
