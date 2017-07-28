using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class CardCashMonthCountAttribute
    {
        protected CardCashMonthCountAttribute()
        {
        }

        public const string Year = "Year";
        public const string Month = "Month";
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
                    allAttributes.Add(Month);
                    allAttributes.Add(TotalConsume);
                    allAttributes.Add(TotalRecharge);
                    allAttributes.Add(TotalExchange);
                 }

                return allAttributes;
            }
        }
    }
    public class CardCashMonthCountInfo : BaseInfo
    {
        public CardCashMonthCountInfo() { }
        public CardCashMonthCountInfo(object dataItem) : base(dataItem) { }
        public CardCashMonthCountInfo(NameValueCollection form) : base(form) { }
        public CardCashMonthCountInfo(IDataReader rdr) : base(rdr) { }
        public string Year { get; set; }
        public string Month { get; set; }
        public decimal TotalConsume { get; set; }
        public decimal TotalRecharge { get; set; }
        public decimal TotalExchange { get; set; }

        public List<CardCashLogInfo> CardCashLogInfoList { get; set; }
        
        protected override List<string> AllAttributes
        {
            get
            {
                return CardCashMonthCountAttribute.AllAttributes;
            }
        }
    }
}
