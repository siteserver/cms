using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class CardSignLogAttribute
    {
        protected CardSignLogAttribute()
        {
        }

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string UserName = "UserName";
        public const string SignDate = "SignDate";
         
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
                    allAttributes.Add(UserName);
                    allAttributes.Add(SignDate);
                    
                 }

                return allAttributes;
            }
        }
    }
    public class CardSignLogInfo : BaseInfo
    {
        public CardSignLogInfo() { }
        public CardSignLogInfo(object dataItem) : base(dataItem) { }
        public CardSignLogInfo(NameValueCollection form) : base(form) { }
        public CardSignLogInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public string UserName { get; set; }
        public DateTime SignDate { get; set; }
         
        protected override List<string> AllAttributes
        {
            get
            {
                return CardSignLogAttribute.AllAttributes;
            }
        }
    }
}
