using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class CardAttribute
    {
        protected CardAttribute()
        {
        }

        public const string Id = nameof(CardInfo.Id);
        public const string PublishmentSystemId = nameof(CardInfo.PublishmentSystemId);
        public const string KeywordId = nameof(CardInfo.KeywordId);
        public const string UserCount = nameof(CardInfo.UserCount);
        public const string PvCount = nameof(CardInfo.PvCount);
        public const string IsDisabled = nameof(CardInfo.IsDisabled);
        public const string Title = nameof(CardInfo.Title);
        public const string ImageUrl = nameof(CardInfo.ImageUrl);
        public const string Summary = nameof(CardInfo.Summary);
        public const string CardTitle = nameof(CardInfo.CardTitle);
        public const string CardTitleColor = nameof(CardInfo.CardTitleColor);
        public const string CardNoColor = nameof(CardInfo.CardNoColor);
        public const string ContentFrontImageUrl = nameof(CardInfo.ContentFrontImageUrl);
        public const string ContentBackImageUrl = nameof(CardInfo.ContentBackImageUrl);
        public const string ShopName = nameof(CardInfo.ShopName);
        public const string ShopAddress = nameof(CardInfo.ShopAddress);
        public const string ShopTel = nameof(CardInfo.ShopTel);
        public const string ShopPosition = nameof(CardInfo.ShopPosition);
        public const string ShopPassword = nameof(CardInfo.ShopPassword);
        public const string ShopOperatorList = nameof(CardInfo.ShopOperatorList);
          
        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            KeywordId,
            UserCount,
            PvCount,
            IsDisabled,
            Title,
            ImageUrl,
            Summary,
            CardTitle,
            CardTitleColor,
            CardNoColor,
            ContentFrontImageUrl,
            ContentBackImageUrl,
            ShopName,
            ShopAddress,
            ShopTel,
            ShopPosition,
            ShopPassword,
            ShopOperatorList
        });
    }

    public class CardInfo : BaseInfo
    {
        public CardInfo() { }
        public CardInfo(object dataItem) : base(dataItem) { }
        public CardInfo(NameValueCollection form) : base(form) { }
        public CardInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public int KeywordId { get; set; }
        public int UserCount { get;set;}
        public int PvCount { get; set; }
        public bool IsDisabled { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Summary { get; set; }
        public string CardTitle { get; set; }
        public string CardTitleColor { get; set; }
        public string CardNoColor { get; set; }
        public string ContentFrontImageUrl { get; set; }
        public string ContentBackImageUrl { get; set; }
        public string ShopName { get; set; }
        public string ShopAddress { get; set; }
        public string ShopTel { get; set; }
        public string ShopPosition { get; set; }
        public string ShopPassword { get; set; }
        public string ShopOperatorList { get; set; }

        protected override List<string> AllAttributes => CardAttribute.AllAttributes;
    }
}
