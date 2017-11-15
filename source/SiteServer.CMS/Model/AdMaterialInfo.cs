using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Model
{
	public class AdMaterialInfo
	{
	    public AdMaterialInfo()
		{
            AdMaterialId = 0;
            PublishmentSystemId=0;
            AdvId = 0;
            AdMaterialName = string.Empty;
            AdMaterialType = EAdvType.HtmlCode;
            Code = string.Empty;
            TextWord = string.Empty;
            TextLink = string.Empty;
            TextColor = string.Empty;
            TextFontSize = 0;
            ImageUrl = string.Empty;
            ImageLink = string.Empty;
            ImageWidth = 0;
            ImageHeight = 0;
            ImageAlt = string.Empty;
            Weight = 0;
            IsEnabled = true;
        }

        public AdMaterialInfo(int adMaterialId, int publishmentSystemId,int advId, string adMaterialName, EAdvType adMaterialType, string code, string textWord, string textLink, string textColor, int textFontSize, string imageUrl, string imageLink, int imageWidth, int imageHeight, string imageAlt, int weight, bool isEnabled) 
		{
            AdMaterialId = adMaterialId;
            PublishmentSystemId = publishmentSystemId;
            AdvId = advId;
            AdMaterialName = adMaterialName;
            AdMaterialType = adMaterialType;
            Code = code;
            TextWord = textWord;
            TextLink = textLink;
            TextColor = textColor;
            TextFontSize = textFontSize;
            ImageUrl = imageUrl;
            ImageLink = imageLink;
            ImageWidth = imageWidth;
            ImageHeight = imageHeight;
            ImageAlt = imageAlt;
            Weight = weight;
            IsEnabled = isEnabled;
		}
         
        public int AdMaterialId { get; set; }

	    public int PublishmentSystemId { get; set; }

	    public int AdvId { get; set; }

	    public string AdMaterialName { get; set; }

	    public EAdvType AdMaterialType { get; set; }

	    public string Code { get; set; }

	    public string TextWord { get; set; }

	    public string TextLink { get; set; }

	    public string TextColor { get; set; }

	    public int TextFontSize { get; set; }

	    public string ImageUrl { get; set; }

	    public string ImageLink { get; set; }

	    public int ImageWidth { get; set; }

	    public int ImageHeight { get; set; }

	    public string ImageAlt { get; set; }

	    public int Weight { get; set; }

	    public bool IsEnabled { get; set; }
	}
}
