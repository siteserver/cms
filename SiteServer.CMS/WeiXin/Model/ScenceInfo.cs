using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class ScenceAttribute
    {
        protected ScenceAttribute()
        {
        }

        public const string Id = nameof(ScenceInfo.Id);
        public const string PublishmentSystemId = nameof(ScenceInfo.PublishmentSystemId);
        public const string KeywordId = nameof(ScenceInfo.KeywordId);
        public const string IsDisabled = nameof(ScenceInfo.IsDisabled);
        public const string PvCount = nameof(ScenceInfo.PvCount);
        public const string StartDate = nameof(ScenceInfo.StartDate);
        public const string EndDate = nameof(ScenceInfo.EndDate);
        public const string Title = nameof(ScenceInfo.Title);
        public const string ImageUrl = nameof(ScenceInfo.ImageUrl);
        public const string Summary = nameof(ScenceInfo.Summary);
        public const string ClickNum = nameof(ScenceInfo.ClickNum);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            KeywordId,
            IsDisabled,
            PvCount,
            StartDate,
            EndDate,
            Title,
            ImageUrl,
            Summary,
            ClickNum
        });
    }

    public class ScenceInfo : BaseInfo
    {
        public ScenceInfo() { }
        public ScenceInfo(object dataItem) : base(dataItem) { }
        public ScenceInfo(NameValueCollection form) : base(form) { }
        public ScenceInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public int KeywordId { get; set; }
        public bool IsDisabled { get; set; }
        public int PvCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Summary { get; set; }
        public int ClickNum { get; set; }

        protected override List<string> AllAttributes => ScenceAttribute.AllAttributes;
    }
}
