using Atom.Core;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.ImportExport.Components
{
	internal class TagStyleIe
	{
		private readonly int _publishmentSystemId;
		private readonly string _filePath;

        public TagStyleIe(int publishmentSystemId, string filePath)
		{
			_publishmentSystemId = publishmentSystemId;
			_filePath = filePath;
		}

		public void ExportTagStyle()
		{
			var feed = AtomUtility.GetEmptyFeed();

			var tagStyleInfoArrayList = DataProvider.TagStyleDao.GetTagStyleInfoArrayList(_publishmentSystemId);

            foreach (TagStyleInfo tagStyleInfo in tagStyleInfoArrayList)
			{
                var entry = ExportTagStyleInfo(tagStyleInfo);
				feed.Entries.Add(entry);
			}

			feed.Save(_filePath);
		}

        public void ExportTagStyle(TagStyleInfo tagStyleInfo)
        {
            var feed = AtomUtility.GetEmptyFeed();

            var entry = ExportTagStyleInfo(tagStyleInfo);
            feed.Entries.Add(entry);

            feed.Save(_filePath);
        }

        private static AtomEntry ExportTagStyleInfo(TagStyleInfo tagStyleInfo)
		{
			var entry = AtomUtility.GetEmptyEntry();

            AtomUtility.AddDcElement(entry.AdditionalElements, "StyleId", tagStyleInfo.StyleId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "StyleName", tagStyleInfo.StyleName);
            AtomUtility.AddDcElement(entry.AdditionalElements, "ElementName", tagStyleInfo.ElementName);
            AtomUtility.AddDcElement(entry.AdditionalElements, "PublishmentSystemId", tagStyleInfo.PublishmentSystemId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "IsTemplate", tagStyleInfo.IsTemplate.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "StyleTemplate", AtomUtility.Encrypt(tagStyleInfo.StyleTemplate));
            AtomUtility.AddDcElement(entry.AdditionalElements, "ScriptTemplate", AtomUtility.Encrypt(tagStyleInfo.ScriptTemplate));
            AtomUtility.AddDcElement(entry.AdditionalElements, "ContentTemplate", AtomUtility.Encrypt(tagStyleInfo.ContentTemplate));
            AtomUtility.AddDcElement(entry.AdditionalElements, "SettingsXml", AtomUtility.Encrypt(tagStyleInfo.SettingsXml));

			return entry;
		}


		public void ImportTagStyle(bool overwrite)
		{
			if (!FileUtils.IsFileExists(_filePath)) return;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(_filePath));

			foreach (AtomEntry entry in feed.Entries)
			{
                var styleName = AtomUtility.GetDcElementContent(entry.AdditionalElements, "StyleName");

				if (!string.IsNullOrEmpty(styleName))
				{
				    var tagStyleInfo = new TagStyleInfo
				    {
				        StyleName = styleName,
				        ElementName = AtomUtility.GetDcElementContent(entry.AdditionalElements, "ElementName"),
				        PublishmentSystemId = _publishmentSystemId,
				        IsTemplate =
				            TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, "IsTemplate")),
				        StyleTemplate =
				            AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements,
				                "StyleTemplate")),
				        ScriptTemplate =
				            AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements,
				                "ScriptTemplate")),
				        ContentTemplate =
				            AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements,
				                "ContentTemplate")),
				        SettingsXml =
				            AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "SettingsXML"))
				    };

				    var srcTagStyleInfo = DataProvider.TagStyleDao.GetTagStyleInfo(_publishmentSystemId, tagStyleInfo.ElementName, tagStyleInfo.StyleName);
                    if (srcTagStyleInfo != null)
					{
						if (overwrite)
						{
                            tagStyleInfo.StyleId = srcTagStyleInfo.StyleId;
                            DataProvider.TagStyleDao.Update(tagStyleInfo);
						}
						else
						{
                            tagStyleInfo.StyleName = DataProvider.TagStyleDao.GetImportStyleName(_publishmentSystemId, tagStyleInfo.ElementName, tagStyleInfo.StyleName);
                            DataProvider.TagStyleDao.Insert(tagStyleInfo);
						}
					}
					else
					{
                        DataProvider.TagStyleDao.Insert(tagStyleInfo);
					}
				}
			}
		}

	}
}
