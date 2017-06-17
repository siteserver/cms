using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class VoteAttribute
    {
        protected VoteAttribute()
        {
        }

        public const string Id = nameof(VoteInfo.Id);
        public const string PublishmentSystemId = nameof(VoteInfo.PublishmentSystemId);
        public const string KeywordId = nameof(VoteInfo.KeywordId);
        public const string IsDisabled = nameof(VoteInfo.IsDisabled);
        public const string UserCount = nameof(VoteInfo.UserCount);
        public const string PvCount = nameof(VoteInfo.PvCount);
        public const string StartDate = nameof(VoteInfo.StartDate);
        public const string EndDate = nameof(VoteInfo.EndDate);
        public const string Title = nameof(VoteInfo.Title);
        public const string ImageUrl = nameof(VoteInfo.ImageUrl);
        public const string Summary = nameof(VoteInfo.Summary);
        public const string ContentImageUrl = nameof(VoteInfo.ContentImageUrl);
        public const string ContentDescription = nameof(VoteInfo.ContentDescription);
        public const string ContentIsImageOption = nameof(VoteInfo.ContentIsImageOption);
        public const string ContentIsCheckBox = nameof(VoteInfo.ContentIsCheckBox);
        public const string ContentResultVisible = nameof(VoteInfo.ContentResultVisible);
        public const string EndTitle = nameof(VoteInfo.EndTitle);
        public const string EndImageUrl = nameof(VoteInfo.EndImageUrl);
        public const string EndSummary = nameof(VoteInfo.EndSummary);

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
            ContentIsImageOption,
            ContentIsCheckBox,
            ContentResultVisible,
            EndTitle,
            EndImageUrl,
            EndSummary
        });
    }

    public class VoteInfo : BaseInfo
    {
        public VoteInfo() { }
        public VoteInfo(object dataItem) : base(dataItem) { }
        public VoteInfo(NameValueCollection form) : base(form) { }
        public VoteInfo(IDataReader rdr) : base(rdr) { }
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
        public string ContentIsImageOption { get; set; }
        public string ContentIsCheckBox { get; set; }
        public string ContentResultVisible { get; set; }
        public string EndTitle { get; set; }
        public string EndImageUrl { get; set; }
        public string EndSummary { get; set; }

        protected override List<string> AllAttributes => VoteAttribute.AllAttributes;
    }
}
