using System.Collections.Generic;

namespace BaiRong.Core.Model.Attributes
{
	public class BackgroundContentAttribute
	{
		protected BackgroundContentAttribute()
		{
		}

        //system
        public const string SubTitle = "SubTitle";
        public const string ImageUrl = "ImageUrl";
        public const string VideoUrl = "VideoUrl";
        public const string FileUrl = "FileUrl";
        public const string LinkUrl = "LinkUrl";
        public const string Author = "Author";
        public const string Source = "Source";
        public const string Summary = "Summary";
        public const string IsRecommend = "IsRecommend";
        public const string IsHot = "IsHot";
        public const string IsColor = "IsColor";
        public const string Content = "Content";
        //not exists
        public const string TitleFormatString = "TitleFormatString";
        public const string StarSetting = "StarSetting";
        public const string Star = "Star";
        public const string Digg = "Digg";
        public const string PageContent = "PageContent";
        public const string NavigationUrl = "NavigationUrl";
        public const string CountOfPhotos = "CountOfPhotos";

        public static List<string> AllAttributes
        {
            get
            {
                var arraylist = new List<string>(ContentAttribute.AllAttributes);
                arraylist.AddRange(SystemAttributes);
                return arraylist;
            }
        }

        private static List<string> _systemAttributes;
        public static List<string> SystemAttributes => _systemAttributes ?? (_systemAttributes = new List<string>
        {
            SubTitle.ToLower(),
            ImageUrl.ToLower(),
            VideoUrl.ToLower(),
            FileUrl.ToLower(),
            LinkUrl.ToLower(),
            Content.ToLower(),
            Author.ToLower(),
            Source.ToLower(),
            Summary.ToLower(),
            IsRecommend.ToLower(),
            IsHot.ToLower(),
            IsColor.ToLower()
        });

	    private static List<string> _excludeAttributes;
        public static List<string> ExcludeAttributes => _excludeAttributes ?? (_excludeAttributes = new List<string>(ContentAttribute.ExcludeAttributes)
        {
            IsRecommend.ToLower(),
            IsHot.ToLower(),
            IsColor.ToLower()
        });
	}
}
