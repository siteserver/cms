using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class CollectAttribute
    {
        protected CollectAttribute()
        {
        }

        public const string Id = nameof(CollectInfo.Id);
        public const string PublishmentSystemId = nameof(CollectInfo.PublishmentSystemId);
        public const string KeywordId = nameof(CollectInfo.KeywordId);
        public const string IsDisabled = nameof(CollectInfo.IsDisabled);
        public const string UserCount = nameof(CollectInfo.UserCount);
        public const string PvCount = nameof(CollectInfo.PvCount);
        public const string StartDate = nameof(CollectInfo.StartDate);
        public const string EndDate = nameof(CollectInfo.EndDate);
        public const string Title = nameof(CollectInfo.Title);
        public const string ImageUrl = nameof(CollectInfo.ImageUrl);
        public const string Summary = nameof(CollectInfo.Summary);
        public const string ContentImageUrl = nameof(CollectInfo.ContentImageUrl);
        public const string ContentDescription = nameof(CollectInfo.ContentDescription);
        public const string ContentMaxVote = nameof(CollectInfo.ContentMaxVote);
        public const string ContentIsCheck = nameof(CollectInfo.ContentIsCheck);
        public const string EndTitle = nameof(CollectInfo.EndTitle);
        public const string EndImageUrl = nameof(CollectInfo.EndImageUrl);
        public const string EndSummary = nameof(CollectInfo.EndSummary);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            KeywordId,
            IsDisabled,
            UserCount,
            PvCount,
            StartDate,
            EndDate,
            Title,
            ImageUrl,
            Summary,
            ContentImageUrl,
            ContentDescription,
            ContentMaxVote,
            ContentIsCheck,
            EndTitle,
            EndImageUrl,
            EndSummary
        });
    }

    public class CollectInfo : BaseInfo
    {
        public CollectInfo() { }
        public CollectInfo(object dataItem) : base(dataItem) { }
        public CollectInfo(NameValueCollection form) : base(form) { }
        public CollectInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public int KeywordId { get; set; }
        public bool IsDisabled { get; set; }
        public int UserCount { get; set; }
        public int PvCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Summary { get; set; }
        public string ContentImageUrl { get; set; }
        public string ContentDescription { get; set; }
        public int ContentMaxVote { get; set; }
        public bool ContentIsCheck { get; set; }
        public string EndTitle { get; set; }
        public string EndImageUrl { get; set; }
        public string EndSummary { get; set; }

        protected override List<string> AllAttributes => CollectAttribute.AllAttributes;
    }
}
