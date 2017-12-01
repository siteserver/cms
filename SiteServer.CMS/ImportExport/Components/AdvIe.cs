using Atom.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.ImportExport.Components
{
	internal class AdvIe
	{
		private readonly int publishmentSystemID;
		private readonly string filePath;

        public AdvIe(int publishmentSystemID, string filePath)
		{
			this.publishmentSystemID = publishmentSystemID;
			this.filePath = filePath;
		}

		public void ExportAd()
		{
            //AtomFeed feed = AtomUtility.GetEmptyFeed();

            //ArrayList adNameArrayList = DataProvider.AdDAO.GetAdNameArrayList(this.publishmentSystemID);

            //foreach (string adName in adNameArrayList)
            //{
            //    AdInfo adInfo = DataProvider.AdDAO.GetAdInfo(adName, this.publishmentSystemID);
            //    AtomEntry entry = ExportAdInfo(adInfo);
            //    feed.Entries.Add(entry);
            //}

            //feed.Save(filePath);
		}

		private static AtomEntry ExportAdInfo(AdvInfo advInfo)
		{
			var entry = AtomUtility.GetEmptyEntry();

            //AtomUtility.AddDcElement(entry.AdditionalElements, "AdName", adInfo.AdName);
            //AtomUtility.AddDcElement(entry.AdditionalElements, "PublishmentSystemID", adInfo.PublishmentSystemID.ToString());
            //AtomUtility.AddDcElement(entry.AdditionalElements, "AdType", EAdTypeUtils.GetValue(adInfo.AdType));
            //AtomUtility.AddDcElement(entry.AdditionalElements, "Code", AtomUtility.Encrypt(adInfo.Code));//加密
            //AtomUtility.AddDcElement(entry.AdditionalElements, "TextWord", adInfo.TextWord);
            //AtomUtility.AddDcElement(entry.AdditionalElements, "TextLink", adInfo.TextLink);
            //AtomUtility.AddDcElement(entry.AdditionalElements, "TextColor", adInfo.TextColor);
            //AtomUtility.AddDcElement(entry.AdditionalElements, "TextFontSize", adInfo.TextFontSize.ToString());
            //AtomUtility.AddDcElement(entry.AdditionalElements, "ImageUrl", adInfo.ImageUrl);
            //AtomUtility.AddDcElement(entry.AdditionalElements, "ImageLink", adInfo.ImageLink);
            //AtomUtility.AddDcElement(entry.AdditionalElements, "ImageWidth", adInfo.ImageWidth.ToString());
            //AtomUtility.AddDcElement(entry.AdditionalElements, "ImageHeight", adInfo.ImageHeight.ToString());
            //AtomUtility.AddDcElement(entry.AdditionalElements, "ImageAlt", adInfo.ImageAlt);
            //AtomUtility.AddDcElement(entry.AdditionalElements, "IsEnabled", adInfo.IsEnabled.ToString());
            //AtomUtility.AddDcElement(entry.AdditionalElements, "IsDateLimited", adInfo.IsDateLimited.ToString());
            //AtomUtility.AddDcElement(entry.AdditionalElements, "StartDate", adInfo.StartDate.ToLongDateString());
            //AtomUtility.AddDcElement(entry.AdditionalElements, "EndDate", adInfo.EndDate.ToLongDateString());

			return entry;
		}

		public void ImportAd(bool overwrite)
		{
            //if (!FileUtils.IsFileExists(this.filePath)) return;
            //AtomFeed feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

            //foreach (AtomEntry entry in feed.Entries)
            //{
            //    string adName = AtomUtility.GetDcElementContent(entry.AdditionalElements, "AdName");

            //    if (!string.IsNullOrEmpty(adName))
            //    {
            //        AdInfo adInfo = new AdInfo();
            //        adInfo.AdName = adName;
            //        adInfo.PublishmentSystemID = this.publishmentSystemID;
            //        adInfo.AdType = EAdTypeUtils.GetEnumType(AtomUtility.GetDcElementContent(entry.AdditionalElements, "AdType"));
            //        adInfo.Code = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "Code"));//解密
            //        adInfo.TextWord = AtomUtility.GetDcElementContent(entry.AdditionalElements, "TextWord");
            //        adInfo.TextLink = AtomUtility.GetDcElementContent(entry.AdditionalElements, "TextLink");
            //        adInfo.TextColor = AtomUtility.GetDcElementContent(entry.AdditionalElements, "TextColor");
            //        adInfo.TextFontSize = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "TextFontSize"));
            //        adInfo.ImageUrl = AtomUtility.GetDcElementContent(entry.AdditionalElements, "ImageUrl");
            //        adInfo.ImageLink = AtomUtility.GetDcElementContent(entry.AdditionalElements, "ImageLink");
            //        adInfo.ImageWidth = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "ImageWidth"));
            //        adInfo.ImageHeight = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "ImageHeight"));
            //        adInfo.ImageAlt = AtomUtility.GetDcElementContent(entry.AdditionalElements, "ImageAlt");
            //        adInfo.IsEnabled = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, "IsEnabled"));
            //        adInfo.IsDateLimited = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, "IsDateLimited"));
            //        adInfo.StartDate = TranslateUtils.ToDateTime(AtomUtility.GetDcElementContent(entry.AdditionalElements, "StartDate"));
            //        adInfo.EndDate = TranslateUtils.ToDateTime(AtomUtility.GetDcElementContent(entry.AdditionalElements, "EndDate"));

            //        AdInfo srcAdInfo = DataProvider.AdDAO.GetAdInfo(adInfo.AdName, this.publishmentSystemID);
            //        if (srcAdInfo != null)
            //        {
            //            if (overwrite)
            //            {
            //                DataProvider.AdDAO.Update(adInfo);
            //            }
            //        }
            //        else
            //        {
            //            DataProvider.AdDAO.Insert(adInfo);
            //        }
            //    }
            //}
		}

	}
}
