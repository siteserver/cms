using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class SearchNavigationAttribute
    {
        protected SearchNavigationAttribute()
        {
        }

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string SearchID = "SearchID";
        public const string Title = "Title";
        public const string Url = "Url";
        public const string ImageCssClass = "ImageCssClass";
        public const string NavigationType = "NavigationType";
        public const string KeywordType = "KeywordType";
        public const string FunctionID = "FunctionID";
        public const string ChannelID = "ChannelID";
        public const string ContentID = "ContentID";

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
                    allAttributes.Add(SearchID);
                    allAttributes.Add(Title);
                    allAttributes.Add(Url);
                    allAttributes.Add(ImageCssClass);
                    allAttributes.Add(NavigationType);
                    allAttributes.Add(KeywordType);
                    allAttributes.Add(FunctionID);
                    allAttributes.Add(ChannelID);
                    allAttributes.Add(ContentID);

                }

                return allAttributes;
            }
        }
    }
    public class SearchNavigationInfo : BaseInfo
    {
        public SearchNavigationInfo() { }
        public SearchNavigationInfo(object dataItem) : base(dataItem) { }
        public SearchNavigationInfo(NameValueCollection form) : base(form) { }
        public SearchNavigationInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public int SearchID { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string ImageCssClass { get; set; }
        public string NavigationType { get; set; }
        public string KeywordType { get; set; }
        public int FunctionID { get; set; }
        public int ChannelID { get; set; }
        public int ContentID { get; set; }
        protected override List<string> AllAttributes
        {
            get
            {
                return SearchNavigationAttribute.AllAttributes;
            }
        }
    }
}
