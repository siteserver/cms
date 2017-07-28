using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class VoteItemAttribute
    {
        protected VoteItemAttribute()
        {
        }

        public const string ID = "ID";
        public const string VoteID = "VoteID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string Title = "Title";
        public const string ImageUrl = "ImageUrl";
        public const string NavigationUrl = "NavigationUrl";
        public const string VoteNum = "VoteNum";

        private static List<string> allAttributes;
        public static List<string> AllAttributes
        {
            get
            {
                if (allAttributes == null)
                {
                    allAttributes = new List<string>();
                    allAttributes.Add(ID);
                    allAttributes.Add(VoteID);
                    allAttributes.Add(PublishmentSystemID);
                    allAttributes.Add(Title);
                    allAttributes.Add(ImageUrl);
                    allAttributes.Add(NavigationUrl);
                    allAttributes.Add(VoteNum);
                }

                return allAttributes;
            }
        }
    }
    public class VoteItemInfo : BaseInfo
    {
        public VoteItemInfo() { }
        public VoteItemInfo(object dataItem) : base(dataItem) { }
        public VoteItemInfo(NameValueCollection form) : base(form) { }
        public VoteItemInfo(IDataReader rdr) : base(rdr) { }
        public int VoteID { get; set; }
        public int PublishmentSystemID { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string NavigationUrl { get; set; }
        public int VoteNum { get; set; }

        protected override List<string> AllAttributes
        {
            get
            {
                return VoteItemAttribute.AllAttributes;
            }
        }
    }
}
