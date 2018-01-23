using System.Collections.Generic;

namespace SiteServer.CMS.Model
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
        public const string Author = "Author";
        public const string Source = "Source";
        public const string Summary = "Summary";
        public const string Content = "Content";
        //not exists
        public const string TitleFormatString = "TitleFormatString";
        public const string PageContent = "PageContent";
        public const string NavigationUrl = "NavigationUrl";
        public const string CountOfPhotos = "CountOfPhotos";

        public static List<string> AllAttributes
        {
            get
            {
                var list = new List<string>(ContentAttribute.AllAttributesLowercase);
                list.AddRange(SystemAttributes);
                return list;
            }
        }

        private static List<string> _systemAttributes;
        public static List<string> SystemAttributes => _systemAttributes ?? (_systemAttributes = new List<string>
        {
            SubTitle.ToLower(),
            ImageUrl.ToLower(),
            VideoUrl.ToLower(),
            FileUrl.ToLower(),
            Content.ToLower(),
            Author.ToLower(),
            Source.ToLower(),
            Summary.ToLower()
        });
	}
}
