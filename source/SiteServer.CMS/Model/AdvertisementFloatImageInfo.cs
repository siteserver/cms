using BaiRong.Core;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Plugin.Models;

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
            get { return GetBool("IsCloseable"); }
            set { SetExtendedAttribute("IsCloseable", value.ToString()); }
		}

        public EPositionType PositionType
		{
            get { return EPositionTypeUtils.GetEnumType(GetExtendedAttribute("PositionType")); }
            set { SetExtendedAttribute("PositionType", EPositionTypeUtils.GetValue(value)); }
		}

        public int PositionX
		{
            get { return GetInt("PositionX"); }
            set { SetExtendedAttribute("PositionX", value.ToString()); }
		}

        public int PositionY
		{
            get { return GetInt("PositionY"); }
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
