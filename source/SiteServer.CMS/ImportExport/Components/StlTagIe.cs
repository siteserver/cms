using Atom.Core;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.ImportExport.Components
{
   public class StlTagIe
    {
        private readonly int _publishmentSystemId;
		private readonly string _filePath;

        public StlTagIe(int publishmentSystemId, string filePath)
		{
			_publishmentSystemId = publishmentSystemId;
			_filePath = filePath;
		}

        public void ExportStlTag()
		{
			var feed = AtomUtility.GetEmptyFeed();

            var stlTagArrayList = DataProvider.StlTagDao.GetStlTagNameArrayList(_publishmentSystemId);

            foreach (string stlTagName in stlTagArrayList)
            {
                var stlTagInfo = DataProvider.StlTagDao.GetStlTagInfo(_publishmentSystemId, stlTagName);
                var entry = ExportStlTagInfo(stlTagInfo);
                feed.Entries.Add(entry);
            }

			feed.Save(_filePath);
		}

        private static AtomEntry ExportStlTagInfo(StlTagInfo stlTagInfo)
        {
            var entry = AtomUtility.GetEmptyEntry();

            AtomUtility.AddDcElement(entry.AdditionalElements, "TagName", stlTagInfo.TagName);
            AtomUtility.AddDcElement(entry.AdditionalElements, "PublishmentSystemID", stlTagInfo.PublishmentSystemID.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "TagDescription", stlTagInfo.TagDescription);
            AtomUtility.AddDcElement(entry.AdditionalElements, "TagContent", stlTagInfo.TagContent);
            
            return entry;
        }

        public void ImportStlTag(bool overwrite)
        {
            if (!FileUtils.IsFileExists(_filePath)) return;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(_filePath));

            foreach (AtomEntry entry in feed.Entries)
            {
                var tagName = ConvertHelper.GetString(AtomUtility.GetDcElementContent(entry.AdditionalElements, "TagName"));

                if (!string.IsNullOrEmpty(tagName))
                {
                    var stlTagInfo = new StlTagInfo
                    {
                        TagName = tagName,
                        PublishmentSystemID = _publishmentSystemId,
                        TagDescription = AtomUtility.GetDcElementContent(entry.AdditionalElements, "TagDescription"),
                        TagContent = AtomUtility.GetDcElementContent(entry.AdditionalElements, "TagContent")
                    };


                    var stlTag = DataProvider.StlTagDao.GetStlTagInfo(_publishmentSystemId, stlTagInfo.TagName);
                    if (stlTag != null)
                    {
                        if (overwrite)
                        {
                            DataProvider.StlTagDao.Update(stlTag);
                        }
                    }
                    else
                    {
                        DataProvider.StlTagDao.Insert(stlTagInfo);
                    }
                }
            }
        }
    }
}
