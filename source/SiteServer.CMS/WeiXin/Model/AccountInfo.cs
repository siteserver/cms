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

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string Token = "Token";
        public const string IsBinding = "IsBinding";
        public const string AccountType = "AccountType";
        public const string WeChatID = "WeChatID";
        public const string SourceID = "SourceID";
        public const string ThumbUrl = "ThumbUrl";
        public const string AppID = "AppID";
        public const string AppSecret = "AppSecret";
        public const string IsWelcome = "IsWelcome";
        public const string WelcomeKeyword = "WelcomeKeyword";
        public const string IsDefaultReply = "IsDefaultReply";
        public const string DefaultReplyKeyword = "DefaultReplyKeyword";

        private static List<string> allAttributes;
        public static List<string> AllAttributes
        {
            get
            {
                if (allAttributes == null)
                {
                    allAttributes = new List<string>();
                    allAttributes.Add(ID);
                    allAttributes.Add(PublishmentSystemID);
                    allAttributes.Add(Token);
                    allAttributes.Add(IsBinding);
                    allAttributes.Add(AccountType);
                    allAttributes.Add(WeChatID);
                    allAttributes.Add(SourceID);
                    allAttributes.Add(ThumbUrl);
                    allAttributes.Add(AppID);
                    allAttributes.Add(AppSecret);
                    allAttributes.Add(IsWelcome);
                    allAttributes.Add(WelcomeKeyword);
                    allAttributes.Add(IsDefaultReply);
                    allAttributes.Add(DefaultReplyKeyword);
                }

                return allAttributes;
            }
        }
    }
    public class AccountInfo : BaseInfo
    {
        public AccountInfo() { }
        public AccountInfo(object dataItem) : base(dataItem) { }
        public AccountInfo(NameValueCollection form) : base(form) { }
        public AccountInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public string Token { get; set; }
        public bool IsBinding { get; set; }
        public string AccountType { get; set; }
        public string WeChatID { get; set; }
        public string SourceID { get; set; }
        public string ThumbUrl { get; set; }
        public string AppID { get; set; }
        public string AppSecret { get; set; }
        public bool IsWelcome { get; set; }
        public string WelcomeKeyword { get; set; }
        public bool IsDefaultReply { get; set; }
        public string DefaultReplyKeyword { get; set; }

        protected override List<string> AllAttributes
        {
            get
            {
                return AccountAttribute.AllAttributes;
            }
        }
    }
}
