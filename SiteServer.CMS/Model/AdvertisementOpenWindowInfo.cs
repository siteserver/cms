using SiteServer.Utils.Model;

namespace SiteServer.CMS.Model
{
    public class AdvertisementOpenWindowInfo : ExtendedAttributes
	{
        public AdvertisementOpenWindowInfo(string settings) : base(settings)
        {

        }

        public AdvertisementOpenWindowInfo(string fileUrl, int height, int width)
        {
            FileUrl = fileUrl;
            Height = height;
            Width = width;
        }

        public string FileUrl
		{
            get { return GetString("FileUrl"); }
            set { Set("FileUrl", value); }
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
