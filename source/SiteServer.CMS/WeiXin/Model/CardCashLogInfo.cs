using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class CardCashLogAttribute
    {
        protected CardCashLogAttribute()
        {
        }

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string UserName = "UserName";
        public const string CardID = "CardID";
        public const string CardSNID = "CardSNID";
        public const string CashType = "CashType";
        public const string Amount = "Amount";
        public const string CurAmount = "CurAmount";
        public const string ConsumeType = "ConsumeType";
        public const string Operator = "Operator";
        public const string Description = "Description";
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
                    allAttributes.Add(CardSNID);
                    allAttributes.Add(CashType);
                    allAttributes.Add(Amount);
                    allAttributes.Add(CurAmount);
                    allAttributes.Add(ConsumeType);
                    allAttributes.Add(Operator);
                    allAttributes.Add(Description);
                    allAttributes.Add(AddDate);
                     
                 }

                return allAttributes;
            }
        }
    }
    public class CardCashLogInfo : BaseInfo
    {
        public CardCashLogInfo() { }
        public CardCashLogInfo(object dataItem) : base(dataItem) { }
        public CardCashLogInfo(NameValueCollection form) : base(form) { }
        public CardCashLogInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public string UserName { get; set; }
        public int CardID { get; set; }
        public int CardSNID { get; set; }
        public string CashType { get; set; }
        public decimal Amount { get; set; }
        public decimal CurAmount { get; set; }
        public string ConsumeType { get; set; }
        public string Operator { get; set; }
        public string Description { get; set; }
        public DateTime AddDate { get; set; }
         
        protected override List<string> AllAttributes
        {
            get
            {
                return CardCashLogAttribute.AllAttributes;
            }
        }
    }
}
