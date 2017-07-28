using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class CollectItemAttribute
    {
        protected CollectItemAttribute()
        {
        }

        public const string ID = "ID";
        public const string CollectID = "CollectID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string Title = "Title";
        public const string SmallUrl = "SmallUrl";
        public const string LargeUrl = "LargeUrl";
        public const string Description = "Description";
        public const string Mobile = "Mobile";
        public const string IsChecked = "IsChecked";
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
                    allAttributes.Add(CollectID);
                    allAttributes.Add(PublishmentSystemID);
                    allAttributes.Add(Title);
                    allAttributes.Add(SmallUrl);
                    allAttributes.Add(LargeUrl);
                    allAttributes.Add(Description);
                    allAttributes.Add(Mobile);
                    allAttributes.Add(IsChecked);
                    allAttributes.Add(VoteNum);
                }

                return allAttributes;
            }
        }
    }
    public class CollectItemInfo : BaseInfo
    {
        public CollectItemInfo() { }
        public CollectItemInfo(object dataItem) : base(dataItem) { }
        public CollectItemInfo(NameValueCollection form) : base(form) { }
        public CollectItemInfo(IDataReader rdr) : base(rdr) { }
        public int CollectID { get; set; }
        public int PublishmentSystemID { get; set; }
        public string Title { get; set; }
        public string SmallUrl { get; set; }
        public string LargeUrl { get; set; }
        public string Description { get; set; }
        public string Mobile { get; set; }
        public bool IsChecked { get; set; }
        public int VoteNum { get; set; }

        protected override List<string> AllAttributes
        {
            get
            {
                return CollectItemAttribute.AllAttributes;
            }
        }
    }
}
