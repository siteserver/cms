using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class SearchAttribute
    {
        protected SearchAttribute()
        {
        }

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string KeywordID = "KeywordID";
        public const string IsDisabled = "IsDisabled";
        public const string PVCount = "PVCount";
        public const string Title = "Title";
        public const string ImageUrl = "ImageUrl";
        public const string Summary = "Summary";
        public const string ContentImageUrl = "ContentImageUrl";
        public const string IsOutsiteSearch = "IsOutsiteSearch";
        public const string IsNavigation = "IsNavigation";
        public const string NavTitleColor = "NavTitleColor";
        public const string NavImageColor = "NavImageColor";
        public const string ImageAreaTitle = "ImageAreaTitle";
        public const string ImageAreaChannelID = "ImageAreaChannelID";
        public const string TextAreaTitle = "TextAreaTitle";
        public const string TextAreaChannelID = "TextAreaChannelID";

        private static List<string> allAttributes;
        public static List<string> AllAttributes
        {
            get
            {
                if (allAttributes == null)
                {
                    allAttributes = new List<string>();
                    allAttributes.Add(ID);
                    allAttributes.Add(PublishmentSystemID);
                    allAttributes.Add(KeywordID);
                    allAttributes.Add(IsDisabled);
                    allAttributes.Add(PVCount);
                    allAttributes.Add(Title);
                    allAttributes.Add(ImageUrl);
                    allAttributes.Add(Summary);
                    allAttributes.Add(ContentImageUrl);
                    allAttributes.Add(IsOutsiteSearch);
                    allAttributes.Add(IsNavigation);
                    allAttributes.Add(NavTitleColor);
                    allAttributes.Add(NavImageColor);
                    allAttributes.Add(ImageAreaTitle);
                    allAttributes.Add(ImageAreaChannelID);
                    allAttributes.Add(TextAreaTitle);
                    allAttributes.Add(TextAreaChannelID);
                }

                return allAttributes;
            }
        }
    }
    public class SearchInfo : BaseInfo
    {
        public SearchInfo() { }
        public SearchInfo(object dataItem) : base(dataItem) { }
        public SearchInfo(NameValueCollection form) : base(form) { }
        public SearchInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public int KeywordID { get; set; }
        public bool IsDisabled { get; set; }
        public int PVCount { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Summary { get; set; }
        public string ContentImageUrl { get; set; }
        public bool IsOutsiteSearch { get; set; }
        public bool IsNavigation { get; set; }
        public string NavTitleColor { get; set; }
        public string NavImageColor { get; set; }
        public string ImageAreaTitle { get; set; }
        public int ImageAreaChannelID { get; set; }
        public string TextAreaTitle { get; set; }
        public int TextAreaChannelID { get; set; }
        protected override List<string> AllAttributes
        {
            get
            {
                return SearchAttribute.AllAttributes;
            }
        }
    }
}
