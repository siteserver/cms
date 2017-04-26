using Atom.Core;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.ImportExport.Components
{
   public class SeoIe
    {
        private readonly int _publishmentSystemId;
		private readonly string _filePath;

        public SeoIe(int publishmentSystemId, string filePath)
		{
			_publishmentSystemId = publishmentSystemId;
			_filePath = filePath;
		}

		public void ExportSeo()
		{
			var feed = AtomUtility.GetEmptyFeed();

			var seoNameArrayList = DataProvider.SeoMetaDao.GetSeoMetaNameArrayList(_publishmentSystemId);

            foreach (string seoName in seoNameArrayList)
            {
                var seoInfo = DataProvider.SeoMetaDao.GetSeoMetaInfoBySeoMetaName(_publishmentSystemId,seoName );
                var entry = ExportSeoInfo(seoInfo);
                feed.Entries.Add(entry);
            }

			feed.Save(_filePath);
		}
        
        private static AtomEntry ExportSeoInfo(SeoMetaInfo seoInfo)
        {
            var entry = AtomUtility.GetEmptyEntry();

            AtomUtility.AddDcElement(entry.AdditionalElements, "SeoMetaID", seoInfo.SeoMetaId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "PublishmentSystemID", seoInfo.PublishmentSystemId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "SeoMetaName", seoInfo.SeoMetaName);
            AtomUtility.AddDcElement(entry.AdditionalElements, "IsDefault", seoInfo.IsDefault.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, "PageTitle", seoInfo.PageTitle);
            AtomUtility.AddDcElement(entry.AdditionalElements, "Keywords", seoInfo.Keywords);
            AtomUtility.AddDcElement(entry.AdditionalElements, "Description", seoInfo.Description);
            AtomUtility.AddDcElement(entry.AdditionalElements, "Copyright", seoInfo.Copyright);
            AtomUtility.AddDcElement(entry.AdditionalElements, "Author", seoInfo.Author);
            AtomUtility.AddDcElement(entry.AdditionalElements, "Email", seoInfo.Email);
            AtomUtility.AddDcElement(entry.AdditionalElements, "Language", seoInfo.Language);
            AtomUtility.AddDcElement(entry.AdditionalElements, "Charset", seoInfo.Charset);
            AtomUtility.AddDcElement(entry.AdditionalElements, "Distribution", seoInfo.Distribution);
            AtomUtility.AddDcElement(entry.AdditionalElements, "Rating", seoInfo.Rating);
            AtomUtility.AddDcElement(entry.AdditionalElements, "Robots", seoInfo.Robots);
            AtomUtility.AddDcElement(entry.AdditionalElements, "RevisitAfter", seoInfo.RevisitAfter);
            AtomUtility.AddDcElement(entry.AdditionalElements, "Expires", seoInfo.Expires);

            return entry;
        }

        public void ImportSeo(bool overwrite)
        {
            if (!FileUtils.IsFileExists(_filePath)) return;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(_filePath));

            foreach (AtomEntry entry in feed.Entries)
            {
                var seoMetaId =ConvertHelper.GetInteger( AtomUtility.GetDcElementContent(entry.AdditionalElements, "SeoMetaID"));

                if (!string.IsNullOrEmpty(seoMetaId.ToString()))
                {
                    var seoInfo = new SeoMetaInfo();
                    seoInfo.SeoMetaId = seoMetaId;
                    seoInfo.PublishmentSystemId = _publishmentSystemId;
                    seoInfo.SeoMetaName = AtomUtility.GetDcElementContent(entry.AdditionalElements, "SeoMetaName");
                    seoInfo.IsDefault = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, "IsDefault"));
                    seoInfo.PageTitle = AtomUtility.GetDcElementContent(entry.AdditionalElements, "PageTitle");
                    seoInfo.Keywords = AtomUtility.GetDcElementContent(entry.AdditionalElements, "Keywords");
                    seoInfo.Description = AtomUtility.GetDcElementContent(entry.AdditionalElements, "Description");
                    seoInfo.Copyright = AtomUtility.GetDcElementContent(entry.AdditionalElements, "Copyright");
                    seoInfo.Author = AtomUtility.GetDcElementContent(entry.AdditionalElements, "Author");
                    seoInfo.Email = AtomUtility.GetDcElementContent(entry.AdditionalElements, "Email");
                    seoInfo.Language =AtomUtility.GetDcElementContent(entry.AdditionalElements, "Language");
                    seoInfo.Charset = AtomUtility.GetDcElementContent(entry.AdditionalElements, "Charset");
                    seoInfo.Distribution = AtomUtility.GetDcElementContent(entry.AdditionalElements, "Distribution");
                    seoInfo.Rating = AtomUtility.GetDcElementContent(entry.AdditionalElements, "Rating");
                    seoInfo.Robots =AtomUtility.GetDcElementContent(entry.AdditionalElements, "Robots");
                    seoInfo.RevisitAfter = AtomUtility.GetDcElementContent(entry.AdditionalElements, "RevisitAfter");
                    seoInfo.Expires = AtomUtility.GetDcElementContent(entry.AdditionalElements, "Expires");

                    var seoMetaInfo = DataProvider.SeoMetaDao.GetSeoMetaInfoBySeoMetaName(_publishmentSystemId, seoInfo.SeoMetaName);
                    if (seoMetaInfo != null)
                    {
                        if (overwrite)
                        {
                            DataProvider.SeoMetaDao.Update(seoInfo);
                        }
                    }
                    else
                    {
                        DataProvider.SeoMetaDao.Insert(seoInfo);
                    }
                }
            }
        }
    }
}
