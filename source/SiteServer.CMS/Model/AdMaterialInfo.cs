using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Model
{
	public class AdMaterialInfo
	{
        private int adMaterialID;
        private int publishmentSystemID;
        private int advID;
        private string adMaterialName;
        private EAdvType adMaterialType;
        private string code;
        private string textWord;
        private string textLink;
        private string textColor;
        private int textFontSize;
        private string imageUrl;
        private string imageLink;
        private int imageWidth;
        private int imageHeight;
        private string imageAlt;
        private int weight;
        private bool isEnabled;

		public AdMaterialInfo()
		{
            adMaterialID = 0;
            publishmentSystemID=0;
            advID = 0;
            adMaterialName = string.Empty;
            adMaterialType = EAdvType.HtmlCode;
            code = string.Empty;
            textWord = string.Empty;
            textLink = string.Empty;
            textColor = string.Empty;
            textFontSize = 0;
            imageUrl = string.Empty;
            imageLink = string.Empty;
            imageWidth = 0;
            imageHeight = 0;
            imageAlt = string.Empty;
            weight = 0;
            isEnabled = true;
        }

        public AdMaterialInfo(int adMaterialID, int publishmentSystemID,int advID, string adMaterialName, EAdvType adMaterialType, string code, string textWord, string textLink, string textColor, int textFontSize, string imageUrl, string imageLink, int imageWidth, int imageHeight, string imageAlt, int weight, bool isEnabled) 
		{
            this.adMaterialID = adMaterialID;
            this.publishmentSystemID = publishmentSystemID;
            this.advID = advID;
            this.adMaterialName = adMaterialName;
            this.adMaterialType = adMaterialType;
            this.code = code;
            this.textWord = textWord;
            this.textLink = textLink;
            this.textColor = textColor;
            this.textFontSize = textFontSize;
            this.imageUrl = imageUrl;
            this.imageLink = imageLink;
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;
            this.imageAlt = imageAlt;
            this.weight = weight;
            this.isEnabled = isEnabled;
		}
         
        public int AdMaterialID
		{
			get{ return adMaterialID; }
            set { adMaterialID = value; }
		}

        public int PublishmentSystemID
        {
            get { return publishmentSystemID; }
            set { publishmentSystemID = value; }
        }

        public int AdvID
        {
            get { return advID; }
            set { advID = value; }
        }

        public string AdMaterialName
        {
            get { return adMaterialName; }
            set { adMaterialName = value; }
        }

        public EAdvType AdMaterialType
		{
			get{ return adMaterialType; }
            set { adMaterialType = value; }
		}

        public string Code
        {
            get { return code; }
            set { code = value; }
        }
          
        public string TextWord
        {
            get { return textWord; }
            set { textWord = value; }
        }

        public string TextLink
        {
            get { return textLink; }
            set { textLink = value; }
        }

        public string TextColor
        {
            get { return textColor; }
            set { textColor = value; }
        }

        public int TextFontSize
        {
            get { return textFontSize; }
            set { textFontSize = value; }
        }

        public string ImageUrl
        {
            get { return imageUrl; }
            set { imageUrl = value; }
        }

        public string ImageLink
        {
            get { return imageLink; }
            set { imageLink = value; }
        }

        public int ImageWidth
        {
            get { return imageWidth; }
            set { imageWidth = value; }
        }

        public int ImageHeight
        {
            get { return imageHeight; }
            set { imageHeight = value; }
        }

        public string ImageAlt
        {
            get { return imageAlt; }
            set { imageAlt = value; }
        }

        public int Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }
         
	}
}
