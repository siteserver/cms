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

        public const string Year = nameof(CardCashMonthCountInfo.Year);
        public const string Month = nameof(CardCashMonthCountInfo.Month);
        public const string TotalConsume = nameof(CardCashMonthCountInfo.TotalConsume);
        public const string TotalRecharge = nameof(CardCashMonthCountInfo.TotalRecharge);
        public const string TotalExchange = nameof(CardCashMonthCountInfo.TotalExchange);
       
        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ??
                                                    (_allAttributes = new List<string> {Year, Month, TotalConsume, TotalRecharge, TotalExchange});
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
        
        protected override List<string> AllAttributes => CardCashMonthCountAttribute.AllAttributes;
    }
}
