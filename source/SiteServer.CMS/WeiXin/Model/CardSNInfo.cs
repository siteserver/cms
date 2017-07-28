using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class CardSNAttribute
    {
        protected CardSNAttribute()
        {
        }

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string UserName = "UserName";
        public const string CardID = "CardID";
        public const string SN = "SN";
        public const string Amount = "Amount";
        public const string IsDisabled = "IsDisabled";
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
                    allAttributes.Add(UserName);
                    allAttributes.Add(CardID);
                    allAttributes.Add(SN);
                    allAttributes.Add(Amount);
                    allAttributes.Add(IsDisabled);
                    allAttributes.Add(AddDate);
                 }

                return allAttributes;
            }
        }
    }
    public class CardSNInfo : BaseInfo
    {
        public CardSNInfo() { }
        public CardSNInfo(object dataItem) : base(dataItem) { }
        public CardSNInfo(NameValueCollection form) : base(form) { }
        public CardSNInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public string UserName { get; set; }
        public int CardID { get; set; }
        public string SN { get; set; }
        public decimal Amount { get; set; }
        public bool IsDisabled { get; set; }
        public DateTime AddDate { get; set; }
        

        protected override List<string> AllAttributes
        {
            get
            {
                return CardSNAttribute.AllAttributes;
            }
        }
    }
}
