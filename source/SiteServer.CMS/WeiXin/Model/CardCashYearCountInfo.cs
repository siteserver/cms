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
        public const string Year = nameof(CardCashYearCountInfo.Year);
        public const string TotalConsume = nameof(CardCashYearCountInfo.TotalConsume);
        public const string TotalRecharge = nameof(CardCashYearCountInfo.TotalRecharge);
        public const string TotalExchange = nameof(CardCashYearCountInfo.TotalExchange);
         
        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ??
                                                    (_allAttributes = new List<string> {Year, TotalConsume, TotalRecharge, TotalExchange});
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

        protected override List<string> AllAttributes => CardCashYearCountAttribute.AllAttributes;
    }
}
