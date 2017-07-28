using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class CardCashYearCountAttribute
    {
        protected CardCashYearCountAttribute()
        {
        }
        public const string Year = "Year";
        public const string TotalConsume = "TotalConsume";
        public const string TotalRecharge = "TotalRecharge";
        public const string TotalExchange = "TotalExchange";
         
        private static List<string> allAttributes;
        public static List<string> AllAttributes
        {
            get
            {
                if (allAttributes == null)
                {
                    allAttributes = new List<string>();
                    allAttributes.Add(Year);
                    allAttributes.Add(TotalConsume);
                    allAttributes.Add(TotalRecharge);
                    allAttributes.Add(TotalExchange);
                }

                return allAttributes;
            }
        }
    }
    public class CardCashYearCountInfo : BaseInfo
    {
        public CardCashYearCountInfo() { }
        public CardCashYearCountInfo(object dataItem) : base(dataItem) { }
        public CardCashYearCountInfo(NameValueCollection form) : base(form) { }
        public CardCashYearCountInfo(IDataReader rdr) : base(rdr) { }
        public string Year { get; set; }
        public decimal TotalConsume { get; set; }
        public decimal TotalRecharge { get; set; }
        public decimal TotalExchange { get; set; }

        public List<CardCashMonthCountInfo> CardCashMonthCountInfoList { get; set; }

        protected override List<string> AllAttributes
        {
            get
            {
                return CardCashYearCountAttribute.AllAttributes;
            }
        }
    }
}
