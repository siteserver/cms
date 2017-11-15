using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class StoreAttribute
    {
        protected StoreAttribute()
        {
        }

        public const string Id = nameof(StoreInfo.Id);
        public const string PublishmentSystemId = nameof(StoreInfo.PublishmentSystemId);
        public const string KeywordId = nameof(StoreInfo.KeywordId);
        public const string PvCount = nameof(StoreInfo.PvCount);
        public const string IsDisabled = nameof(StoreInfo.IsDisabled);
        public const string Title = nameof(StoreInfo.Title);
        public const string ImageUrl = nameof(StoreInfo.ImageUrl);
        public const string Summary = nameof(StoreInfo.Summary);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            KeywordId,
            IsDisabled,
            PvCount,
            Title,
            ImageUrl,
            Summary
        });
    }

    public class StoreInfo : BaseInfo
    {
        public StoreInfo() { }
        public StoreInfo(object dataItem) : base(dataItem) { }
        public StoreInfo(NameValueCollection form) : base(form) { }
        public StoreInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public int KeywordId { get; set; }
        public bool IsDisabled { get; set; }
        public int PvCount { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Summary { get; set; }        

        protected override List<string> AllAttributes => StoreAttribute.AllAttributes;
    }
}
