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

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string MessageID = "MessageID";
        public const string IPAddress = "IPAddress";
        public const string CookieSN = "CookieSN";
        public const string WXOpenID = "WXOpenID";
        public const string UserName = "UserName";
        public const string ReplyCount = "ReplyCount";
        public const string LikeCount = "LikeCount";
        public const string LikeCookieSNCollection = "LikeCookieSNCollection";
        public const string IsReply = "IsReply";
        public const string ReplyID = "ReplyID";
        public const string DisplayName = "DisplayName";
        public const string Color = "Color";
        public const string Content = "Content";
        public const string AddDate = "AddDate";

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
                    allAttributes.Add(MessageID);
                    allAttributes.Add(IPAddress);
                    allAttributes.Add(CookieSN);
                    allAttributes.Add(WXOpenID);
                    allAttributes.Add(UserName);
                    allAttributes.Add(ReplyCount);
                    allAttributes.Add(LikeCount);
                    allAttributes.Add(LikeCookieSNCollection);
                    allAttributes.Add(IsReply);
                    allAttributes.Add(ReplyID);
                    allAttributes.Add(DisplayName);
                    allAttributes.Add(Color);
                    allAttributes.Add(Content);
                    allAttributes.Add(AddDate);
                }

                return allAttributes;
            }
        }
    }
    public class MessageContentInfo : BaseInfo
    {
        public MessageContentInfo() { }
        public MessageContentInfo(object dataItem) : base(dataItem) { }
        public MessageContentInfo(NameValueCollection form) : base(form) { }
        public MessageContentInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public int MessageID { get; set; }
        public string IPAddress { get; set; }
        public string CookieSN { get; set; }
        public string WXOpenID { get; set; }
        public string UserName { get; set; }
        public int ReplyCount { get; set; }
        public int LikeCount { get; set; }
        public int LikeCookieSNCollection { get; set; }
        public bool IsReply { get; set; }
        public int ReplyID { get; set; }
        public string DisplayName { get; set; }
        public string Color { get; set; }
        public string Content { get; set; }
        public DateTime AddDate { get; set; }

        public List<MessageContentInfo> ReplyContentInfoList { get; set; }

        protected override List<string> AllAttributes
        {
            get
            {
                return MessageContentAttribute.AllAttributes;
            }
        }
    }
}
