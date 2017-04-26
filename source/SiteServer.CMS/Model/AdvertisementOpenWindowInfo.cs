using BaiRong.Core;
using BaiRong.Core.Model;

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
            get { return GetInt("Height", 0); }
            set { SetExtendedAttribute("Height", value.ToString()); }
		}

		public int Width
		{
            get { return GetInt("Width", 0); }
            set { SetExtendedAttribute("Width", value.ToString()); }
		}
	}
}
