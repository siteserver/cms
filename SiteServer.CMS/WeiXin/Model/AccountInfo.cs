using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class AccountAttribute
    {
        protected AccountAttribute()
        {
        }

        public const string Id = nameof(AccountInfo.Id);
        public const string PublishmentSystemId = nameof(AccountInfo.PublishmentSystemId);
        public const string Token = nameof(AccountInfo.Token);
        public const string IsBinding = nameof(AccountInfo.IsBinding);
        public const string AccountType = nameof(AccountInfo.AccountType);
        public const string WeChatId = nameof(AccountInfo.WeChatId);
        public const string SourceId = nameof(AccountInfo.SourceId);
        public const string ThumbUrl = nameof(AccountInfo.ThumbUrl);
        public const string AppId = nameof(AccountInfo.AppId);
        public const string AppSecret = nameof(AccountInfo.AppSecret);
        public const string IsWelcome = nameof(AccountInfo.IsWelcome);
        public const string WelcomeKeyword = nameof(AccountInfo.WelcomeKeyword);
        public const string IsDefaultReply = nameof(AccountInfo.IsDefaultReply);
        public const string DefaultReplyKeyword = nameof(AccountInfo.DefaultReplyKeyword);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            Token,
            IsBinding,
            AccountType,
            WeChatId,
            SourceId,
            ThumbUrl,
            AppId,
            AppSecret,
            IsWelcome,
            WelcomeKeyword,
            IsDefaultReply,
            DefaultReplyKeyword
        });
    }

    public class AccountInfo : BaseInfo
    {
        public AccountInfo() { }
        public AccountInfo(object dataItem) : base(dataItem) { }
        public AccountInfo(NameValueCollection form) : base(form) { }
        public AccountInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public string Token { get; set; }
        public bool IsBinding { get; set; }
        public string AccountType { get; set; }
        public string WeChatId { get; set; }
        public string SourceId { get; set; }
        public string ThumbUrl { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public bool IsWelcome { get; set; }
        public string WelcomeKeyword { get; set; }
        public bool IsDefaultReply { get; set; }
        public string DefaultReplyKeyword { get; set; }

        protected override List<string> AllAttributes => AccountAttribute.AllAttributes;
    }
}
