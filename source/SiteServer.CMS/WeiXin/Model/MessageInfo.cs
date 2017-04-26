using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class MessageAttribute
    {
        protected MessageAttribute()
        {
        }

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string KeywordID = "KeywordID";
        public const string IsDisabled = "IsDisabled";
        public const string UserCount = "UserCount";
        public const string PVCount = "PVCount";
        public const string Title = "Title";
        public const string ImageUrl = "ImageUrl";
        public const string Summary = "Summary";
        public const string ContentImageUrl = "ContentImageUrl";
        public const string ContentDescription = "ContentDescription";

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
                    allAttributes.Add(UserCount);
                    allAttributes.Add(PVCount);
                    allAttributes.Add(Title);
                    allAttributes.Add(ImageUrl);
                    allAttributes.Add(Summary);
                    allAttributes.Add(ContentImageUrl);
                    allAttributes.Add(ContentDescription);
                }

                return allAttributes;
            }
        }
    }
    public class MessageInfo : BaseInfo
    {
        public MessageInfo() { }
        public MessageInfo(object dataItem) : base(dataItem) { }
        public MessageInfo(NameValueCollection form) : base(form) { }
        public MessageInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public int KeywordID { get; set; }
        public bool IsDisabled { get; set; }
        public int UserCount { get; set; }
        public int PVCount { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Summary { get; set; }
        public string ContentImageUrl { get; set; }
        public string ContentDescription { get; set; }

        protected override List<string> AllAttributes
        {
            get
            {
                return MessageAttribute.AllAttributes;
            }
        }
    }
}
