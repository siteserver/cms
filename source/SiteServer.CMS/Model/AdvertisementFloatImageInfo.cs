using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Model
{
    public class AdvertisementFloatImageInfo : ExtendedAttributes
	{
        public AdvertisementFloatImageInfo(string settings)
        {
            var nameValueCollection = TranslateUtils.ToNameValueCollection(settings);
            SetExtendedAttribute(nameValueCollection);
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
            get { return GetBool("IsCloseable", false); }
            set { SetExtendedAttribute("IsCloseable", value.ToString()); }
		}

        public EPositionType PositionType
		{
            get { return EPositionTypeUtils.GetEnumType(GetExtendedAttribute("PositionType")); }
            set { SetExtendedAttribute("PositionType", EPositionTypeUtils.GetValue(value)); }
		}

        public int PositionX
		{
            get { return GetInt("PositionX", 0); }
            set { SetExtendedAttribute("PositionX", value.ToString()); }
		}

        public int PositionY
		{
            get { return GetInt("PositionY", 0); }
            set { SetExtendedAttribute("PositionY", value.ToString()); }
		}

		public ERollingType RollingType
		{
            get { return ERollingTypeUtils.GetEnumType(GetExtendedAttribute("RollingType")); }
            set { SetExtendedAttribute("RollingType", ERollingTypeUtils.GetValue(value)); }
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
