using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class View360Attribute
    {
        protected View360Attribute()
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
        public const string ContentImageUrl1 = "ContentImageUrl1";
        public const string ContentImageUrl2 = "ContentImageUrl2";
        public const string ContentImageUrl3 = "ContentImageUrl3";
        public const string ContentImageUrl4 = "ContentImageUrl4";
        public const string ContentImageUrl5 = "ContentImageUrl5";
        public const string ContentImageUrl6 = "ContentImageUrl6";

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
                    allAttributes.Add(ContentImageUrl1);
                    allAttributes.Add(ContentImageUrl2);
                    allAttributes.Add(ContentImageUrl3);
                    allAttributes.Add(ContentImageUrl4);
                    allAttributes.Add(ContentImageUrl5);
                    allAttributes.Add(ContentImageUrl6);
                }

                return allAttributes;
            }
        }
    }
    public class View360Info : BaseInfo
    {
        public View360Info() { }
        public View360Info(object dataItem) : base(dataItem) { }
        public View360Info(NameValueCollection form) : base(form) { }
        public View360Info(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public int KeywordID { get; set; }
        public bool IsDisabled { get; set; }
        public int PVCount { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Summary { get; set; }
        public string ContentImageUrl1 { get; set; }
        public string ContentImageUrl2 { get; set; }
        public string ContentImageUrl3 { get; set; }
        public string ContentImageUrl4 { get; set; }
        public string ContentImageUrl5 { get; set; }
        public string ContentImageUrl6 { get; set; }

        protected override List<string> AllAttributes
        {
            get
            {
                return View360Attribute.AllAttributes;
            }
        }
    }
}
