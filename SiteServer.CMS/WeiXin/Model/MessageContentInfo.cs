using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class MessageContentAttribute
    {
        protected MessageContentAttribute()
        {
        }

        public const string Id = nameof(MessageContentInfo.Id);
        public const string PublishmentSystemId = nameof(MessageContentInfo.PublishmentSystemId);
        public const string MessageId = nameof(MessageContentInfo.MessageId);
        public const string IpAddress = nameof(MessageContentInfo.IpAddress);
        public const string CookieSn = nameof(MessageContentInfo.CookieSn);
        public const string WxOpenId = nameof(MessageContentInfo.WxOpenId);
        public const string UserName = nameof(MessageContentInfo.UserName);
        public const string ReplyCount = nameof(MessageContentInfo.ReplyCount);
        public const string LikeCount = nameof(MessageContentInfo.LikeCount);
        public const string LikeCookieSnCollection = nameof(MessageContentInfo.LikeCookieSnCollection);
        public const string IsReply = nameof(MessageContentInfo.IsReply);
        public const string ReplyId = nameof(MessageContentInfo.ReplyId);
        public const string DisplayName = nameof(MessageContentInfo.DisplayName);
        public const string Color = nameof(MessageContentInfo.Color);
        public const string Content = nameof(MessageContentInfo.Content);
        public const string AddDate = nameof(MessageContentInfo.AddDate);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            MessageId,
            IpAddress,
            CookieSn,
            WxOpenId,
            UserName,
            ReplyCount,
            LikeCount,
            LikeCookieSnCollection,
            IsReply,
            ReplyId,
            DisplayName,
            Color,
            Content,
            AddDate
        });
    }

    public class MessageContentInfo : BaseInfo
    {
        public MessageContentInfo() { }
        public MessageContentInfo(object dataItem) : base(dataItem) { }
        public MessageContentInfo(NameValueCollection form) : base(form) { }
        public MessageContentInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public int MessageId { get; set; }
        public string IpAddress { get; set; }
        public string CookieSn { get; set; }
        public string WxOpenId { get; set; }
        public string UserName { get; set; }
        public int ReplyCount { get; set; }
        public int LikeCount { get; set; }
        public int LikeCookieSnCollection { get; set; }
        public bool IsReply { get; set; }
        public int ReplyId { get; set; }
        public string DisplayName { get; set; }
        public string Color { get; set; }
        public string Content { get; set; }
        public DateTime AddDate { get; set; }

        public List<MessageContentInfo> ReplyContentInfoList { get; set; }

        protected override List<string> AllAttributes => MessageContentAttribute.AllAttributes;
    }
}
