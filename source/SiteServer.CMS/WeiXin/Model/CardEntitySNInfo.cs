using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class CardEntitySnAttribute
    {
        protected CardEntitySnAttribute()
        {
        }

        public const string Id = nameof(CardEntitySnInfo.Id);
        public const string PublishmentSystemId = nameof(CardEntitySnInfo.PublishmentSystemId);
        public const string CardId = nameof(CardEntitySnInfo.CardId);
        public const string Sn = nameof(CardEntitySnInfo.Sn);
        public const string UserName = nameof(CardEntitySnInfo.UserName);
        public const string Mobile = nameof(CardEntitySnInfo.Mobile);
        public const string Amount = nameof(CardEntitySnInfo.Amount);
        public const string Credits = nameof(CardEntitySnInfo.Credits);
        public const string Email = nameof(CardEntitySnInfo.Email);
        public const string Address = nameof(CardEntitySnInfo.Address);
        public const string IsBinding = nameof(CardEntitySnInfo.IsBinding);
        public const string AddDate = nameof(CardEntitySnInfo.AddDate);
         
        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            CardId,
            Sn,
            UserName,
            Mobile,
            Amount,
            Credits,
            Email,
            Address,
            IsBinding,
            AddDate
        });
    }

    public class CardEntitySnInfo : BaseInfo
    {
        public CardEntitySnInfo() { }
        public CardEntitySnInfo(object dataItem) : base(dataItem) { }
        public CardEntitySnInfo(NameValueCollection form) : base(form) { }
        public CardEntitySnInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public int CardId { get; set; }
        public string Sn { get; set; }
        public string UserName { get; set; }
        public string Mobile { get; set; }
        public decimal Amount { get; set; }
        public int Credits { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public bool IsBinding { get; set; }
        public DateTime AddDate { get; set; }
         
        protected override List<string> AllAttributes => CardEntitySnAttribute.AllAttributes;
    }
}
