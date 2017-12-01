using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class MessageAttribute
    {
        protected MessageAttribute()
        {
        }

        public const string Id = nameof(MessageInfo.Id);
        public const string PublishmentSystemId = nameof(MessageInfo.PublishmentSystemId);
        public const string KeywordId = nameof(MessageInfo.KeywordId);
        public const string IsDisabled = nameof(MessageInfo.IsDisabled);
        public const string UserCount = nameof(MessageInfo.UserCount);
        public const string PvCount = nameof(MessageInfo.PvCount);
        public const string Title = nameof(MessageInfo.Title);
        public const string ImageUrl = nameof(MessageInfo.ImageUrl);
        public const string Summary = nameof(MessageInfo.Summary);
        public const string ContentImageUrl = nameof(MessageInfo.ContentImageUrl);
        public const string ContentDescription = nameof(MessageInfo.ContentDescription);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            KeywordId,
            IsDisabled,
            UserCount,
            PvCount,
            Title,
            ImageUrl,
            Summary,
            ContentImageUrl,
            ContentDescription
        });
    }

    public class MessageInfo : BaseInfo
    {
        public MessageInfo() { }
        public MessageInfo(object dataItem) : base(dataItem) { }
        public MessageInfo(NameValueCollection form) : base(form) { }
        public MessageInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public int KeywordId { get; set; }
        public bool IsDisabled { get; set; }
        public int UserCount { get; set; }
        public int PvCount { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Summary { get; set; }
        public string ContentImageUrl { get; set; }
        public string ContentDescription { get; set; }

        protected override List<string> AllAttributes => MessageAttribute.AllAttributes;
    }
}
