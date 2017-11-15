using BaiRong.Core;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Model
{
    public class AdvertisementOpenWindowInfo : ExtendedAttributes
	{
        public AdvertisementOpenWindowInfo(string settings)
        {
            var nameValueCollection = TranslateUtils.ToNameValueCollection(settings);
            SetExtendedAttribute(nameValueCollection);
        }

        public AdvertisementOpenWindowInfo(string fileUrl, int height, int width)
        {
            FileUrl = fileUrl;
            Height = height;
            Width = width;
        }

        public string FileUrl
		{
            get { return GetExtendedAttribute("FileUrl"); }
            set { SetExtendedAttribute("FileUrl", value); }
		}

		public int Height
		{
            get { return GetInt("Height"); }
            set { SetExtendedAttribute("Height", value.ToString()); }
		}

		public int Width
		{
            get { return GetInt("Width"); }
            set { SetExtendedAttribute("Width", value.ToString()); }
		}
	}
}
