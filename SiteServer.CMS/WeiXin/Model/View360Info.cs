using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class View360Attribute
    {
        protected View360Attribute()
        {
        }

        public const string Id = nameof(View360Info.Id);
        public const string PublishmentSystemId = nameof(View360Info.PublishmentSystemId);
        public const string KeywordId = nameof(View360Info.KeywordId);
        public const string IsDisabled = nameof(View360Info.IsDisabled);
        public const string PvCount = nameof(View360Info.PvCount);
        public const string Title = nameof(View360Info.Title);
        public const string ImageUrl = nameof(View360Info.ImageUrl);
        public const string Summary = nameof(View360Info.Summary);
        public const string ContentImageUrl1 = nameof(View360Info.ContentImageUrl1);
        public const string ContentImageUrl2 = nameof(View360Info.ContentImageUrl2);
        public const string ContentImageUrl3 = nameof(View360Info.ContentImageUrl3);
        public const string ContentImageUrl4 = nameof(View360Info.ContentImageUrl4);
        public const string ContentImageUrl5 = nameof(View360Info.ContentImageUrl5);
        public const string ContentImageUrl6 = nameof(View360Info.ContentImageUrl6);

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
            Summary,
            ContentImageUrl1,
            ContentImageUrl2,
            ContentImageUrl3,
            ContentImageUrl4,
            ContentImageUrl5,
            ContentImageUrl6
        });
    }

    public class View360Info : BaseInfo
    {
        public View360Info() { }
        public View360Info(object dataItem) : base(dataItem) { }
        public View360Info(NameValueCollection form) : base(form) { }
        public View360Info(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public int KeywordId { get; set; }
        public bool IsDisabled { get; set; }
        public int PvCount { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Summary { get; set; }
        public string ContentImageUrl1 { get; set; }
        public string ContentImageUrl2 { get; set; }
        public string ContentImageUrl3 { get; set; }
        public string ContentImageUrl4 { get; set; }
        public string ContentImageUrl5 { get; set; }
        public string ContentImageUrl6 { get; set; }

        protected override List<string> AllAttributes => View360Attribute.AllAttributes;
    }
}
