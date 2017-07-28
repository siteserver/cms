using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class CollectLogAttribute
    {
        protected CollectLogAttribute()
        {
        }

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string CollectID = "CollectID";
        public const string ItemID = "ItemID";
        public const string IPAddress = "IPAddress";
        public const string CookieSN = "CookieSN";
        public const string WXOpenID = "WXOpenID";
        public const string UserName = "UserName";
        public const string AddDate = "AddDate";

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
                    allAttributes.Add(CollectID);
                    allAttributes.Add(ItemID);
                    allAttributes.Add(IPAddress);
                    allAttributes.Add(CookieSN);
                    allAttributes.Add(WXOpenID);
                    allAttributes.Add(UserName);
                    allAttributes.Add(AddDate);
                }

                return allAttributes;
            }
        }
    }
    public class CollectLogInfo : BaseInfo
    {
        public CollectLogInfo() { }
        public CollectLogInfo(object dataItem) : base(dataItem) { }
        public CollectLogInfo(NameValueCollection form) : base(form) { }
        public CollectLogInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public int CollectID { get; set; }
        public int ItemID { get; set; }
        public string IPAddress { get; set; }
        public string CookieSN { get; set; }
        public string WXOpenID { get; set; }
        public string UserName { get; set; }
        public DateTime AddDate { get; set; }

        protected override List<string> AllAttributes
        {
            get
            {
                return CollectLogAttribute.AllAttributes;
            }
        }
    }
}
