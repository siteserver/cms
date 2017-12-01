using BaiRong.Core.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Model
{
    public class AdvertisementFloatImageInfo : ExtendedAttributes
	{
        public AdvertisementFloatImageInfo(string settings) : base(settings)
        {

        }

        public AdvertisementFloatImageInfo(bool isCloseable, EPositionType positionType, int positionX, int positionY, ERollingType rollingType, string navigationUrl, string imageUrl, int height, int width)
        {
            IsCloseable = isCloseable;
            PositionType = positionType;
            PositionX = positionX;
            PositionY = positionY;
            RollingType = rollingType;
            NavigationUrl = navigationUrl;
            ImageUrl = imageUrl;
            Height = height;
            Width = width;
        }

		public bool IsCloseable
		{
            get { return GetBool("IsCloseable"); }
            set { Set("IsCloseable", value.ToString()); }
		}

        public EPositionType PositionType
		{
            get { return EPositionTypeUtils.GetEnumType(GetString("PositionType")); }
            set { Set("PositionType", EPositionTypeUtils.GetValue(value)); }
		}

        public int PositionX
		{
            get { return GetInt("PositionX"); }
            set { Set("PositionX", value.ToString()); }
		}

        public int PositionY
		{
            get { return GetInt("PositionY"); }
            set { Set("PositionY", value.ToString()); }
		}

		public ERollingType RollingType
		{
            get { return ERollingTypeUtils.GetEnumType(GetString("RollingType")); }
            set { Set("RollingType", ERollingTypeUtils.GetValue(value)); }
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
