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

        public const string Id = nameof(CardCashLogInfo.Id);
        public const string PublishmentSystemId = nameof(CardCashLogInfo.PublishmentSystemId);
        public const string UserName = nameof(CardCashLogInfo.UserName);
        public const string CardId = nameof(CardCashLogInfo.CardId);
        public const string CardSnId = nameof(CardCashLogInfo.CardSnId);
        public const string CashType = nameof(CardCashLogInfo.CashType);
        public const string Amount = nameof(CardCashLogInfo.Amount);
        public const string CurAmount = nameof(CardCashLogInfo.CurAmount);
        public const string ConsumeType = nameof(CardCashLogInfo.ConsumeType);
        public const string Operator = nameof(CardCashLogInfo.Operator);
        public const string Description = nameof(CardCashLogInfo.Description);
        public const string AddDate = nameof(CardCashLogInfo.AddDate);
         
        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            UserName,
            CardId,
            CardSnId,
            CashType,
            Amount,
            CurAmount,
            ConsumeType,
            Operator,
            Description,
            AddDate
        });
    }

    public class CardCashLogInfo : BaseInfo
    {
        public CardCashLogInfo() { }
        public CardCashLogInfo(object dataItem) : base(dataItem) { }
        public CardCashLogInfo(NameValueCollection form) : base(form) { }
        public CardCashLogInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public string UserName { get; set; }
        public int CardId { get; set; }
        public int CardSnId { get; set; }
        public string CashType { get; set; }
        public decimal Amount { get; set; }
        public decimal CurAmount { get; set; }
        public string ConsumeType { get; set; }
        public string Operator { get; set; }
        public string Description { get; set; }
        public DateTime AddDate { get; set; }
         
        protected override List<string> AllAttributes => CardCashLogAttribute.AllAttributes;
    }
}
