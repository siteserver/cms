using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class SearchAttribute
    {
        protected SearchAttribute()
        {
        }

        public const string Id = nameof(SearchInfo.Id);
        public const string PublishmentSystemId = nameof(SearchInfo.PublishmentSystemId);
        public const string KeywordId = nameof(SearchInfo.KeywordId);
        public const string IsDisabled = nameof(SearchInfo.IsDisabled);
        public const string PvCount = nameof(SearchInfo.PvCount);
        public const string Title = nameof(SearchInfo.Title);
        public const string ImageUrl = nameof(SearchInfo.ImageUrl);
        public const string Summary = nameof(SearchInfo.Summary);
        public const string ContentImageUrl = nameof(SearchInfo.ContentImageUrl);
        public const string IsOutsiteSearch = nameof(SearchInfo.IsOutsiteSearch);
        public const string IsNavigation = nameof(SearchInfo.IsNavigation);
        public const string NavTitleColor = nameof(SearchInfo.NavTitleColor);
        public const string NavImageColor = nameof(SearchInfo.NavImageColor);
        public const string ImageAreaTitle = nameof(SearchInfo.ImageAreaTitle);
        public const string ImageAreaChannelId = nameof(SearchInfo.ImageAreaChannelId);
        public const string TextAreaTitle = nameof(SearchInfo.TextAreaTitle);
        public const string TextAreaChannelId = nameof(SearchInfo.TextAreaChannelId);

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
            ContentImageUrl,
            IsOutsiteSearch,
            IsNavigation,
            NavTitleColor,
            NavImageColor,
            ImageAreaTitle,
            ImageAreaChannelId,
            TextAreaTitle,
            TextAreaChannelId
        });
    }

    public class SearchInfo : BaseInfo
    {
        public SearchInfo() { }
        public SearchInfo(object dataItem) : base(dataItem) { }
        public SearchInfo(NameValueCollection form) : base(form) { }
        public SearchInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public int KeywordId { get; set; }
        public bool IsDisabled { get; set; }
        public int PvCount { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Summary { get; set; }
        public string ContentImageUrl { get; set; }
        public bool IsOutsiteSearch { get; set; }
        public bool IsNavigation { get; set; }
        public string NavTitleColor { get; set; }
        public string NavImageColor { get; set; }
        public string ImageAreaTitle { get; set; }
        public int ImageAreaChannelId { get; set; }
        public string TextAreaTitle { get; set; }
        public int TextAreaChannelId { get; set; }

        protected override List<string> AllAttributes => SearchAttribute.AllAttributes;
    }
}
