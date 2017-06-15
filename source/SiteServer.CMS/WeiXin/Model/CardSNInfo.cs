using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class CardSnAttribute
    {
        protected CardSnAttribute()
        {
        }

        public const string Id = nameof(CardSnInfo.Id);
        public const string PublishmentSystemId = nameof(CardSnInfo.PublishmentSystemId);
        public const string UserName = nameof(CardSnInfo.UserName);
        public const string CardId = nameof(CardSnInfo.CardId);
        public const string Sn = nameof(CardSnInfo.Sn);
        public const string Amount = nameof(CardSnInfo.Amount);
        public const string IsDisabled = nameof(CardSnInfo.IsDisabled);
        public const string AddDate = nameof(CardSnInfo.AddDate);
      
        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            UserName,
            CardId,
            Sn,
            Amount,
            IsDisabled,
            AddDate
        });
    }

    public class CardSnInfo : BaseInfo
    {
        public CardSnInfo() { }
        public CardSnInfo(object dataItem) : base(dataItem) { }
        public CardSnInfo(NameValueCollection form) : base(form) { }
        public CardSnInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public string UserName { get; set; }
        public int CardId { get; set; }
        public string Sn { get; set; }
        public decimal Amount { get; set; }
        public bool IsDisabled { get; set; }
        public DateTime AddDate { get; set; }

        protected override List<string> AllAttributes => CardSnAttribute.AllAttributes;
    }
}
