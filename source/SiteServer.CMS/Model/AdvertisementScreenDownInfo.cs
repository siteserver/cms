using BaiRong.Core;
using BaiRong.Core.Model;

namespace SiteServer.CMS.Model
{
    public class AdvertisementScreenDownInfo : ExtendedAttributes
	{
        public AdvertisementScreenDownInfo(string settings)
        {
            var nameValueCollection = TranslateUtils.ToNameValueCollection(settings);
            SetExtendedAttribute(nameValueCollection);
        }

        public AdvertisementScreenDownInfo(string navigationUrl, string imageUrl, int delay, int height, int width)
        {
            NavigationUrl = navigationUrl;
            ImageUrl = imageUrl;
            Delay = delay;
            Height = height;
            Width = width;
        }

        public string NavigationUrl
        {
            get { return GetExtendedAttribute("NavigationUrl"); }
            set { SetExtendedAttribute("NavigationUrl", value); }
        }

        public string ImageUrl
		{
            get { return GetExtendedAttribute("ImageUrl"); }
            set { SetExtendedAttribute("ImageUrl", value); }
		}

        public int Delay
        {
            get { return GetInt("Delay", 0); }
            set { SetExtendedAttribute("Delay", value.ToString()); }
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
