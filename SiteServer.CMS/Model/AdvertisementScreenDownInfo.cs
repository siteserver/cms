using SiteServer.Utils.Model;

namespace SiteServer.CMS.Model
{
    public class AdvertisementScreenDownInfo : ExtendedAttributes
	{
        public AdvertisementScreenDownInfo(string settings) : base(settings)
        {

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
            get { return GetString("NavigationUrl"); }
            set { Set("NavigationUrl", value); }
        }

        public string ImageUrl
		{
            get { return GetString("ImageUrl"); }
            set { Set("ImageUrl", value); }
		}

        public int Delay
        {
            get { return GetInt("Delay"); }
            set { Set("Delay", value.ToString()); }
        }

		public int Height
		{
            get { return GetInt("Height"); }
            set { Set("Height", value.ToString()); }
		}

		public int Width
		{
            get { return GetInt("Width"); }
            set { Set("Width", value.ToString()); }
		}
	}
}
