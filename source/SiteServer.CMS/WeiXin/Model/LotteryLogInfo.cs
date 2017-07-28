using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class LotteryLogAttribute
    {
        protected LotteryLogAttribute()
        {
        }

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string LotteryID = "LotteryID";
        public const string CookieSN = "CookieSN";
        public const string WXOpenID = "WXOpenID";
        public const string UserName = "UserName";
        public const string LotteryCount = "LotteryCount";
        public const string LotteryDailyCount = "LotteryDailyCount";
        public const string LastLotteryDate = "LastLotteryDate";

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
                    allAttributes.Add(LotteryID);
                    allAttributes.Add(CookieSN);
                    allAttributes.Add(WXOpenID);
                    allAttributes.Add(UserName);
                    allAttributes.Add(LotteryCount);
                    allAttributes.Add(LotteryDailyCount);
                    allAttributes.Add(LastLotteryDate);
                }

                return allAttributes;
            }
        }
    }
    public class LotteryLogInfo : BaseInfo
    {
        public LotteryLogInfo() { }
        public LotteryLogInfo(object dataItem) : base(dataItem) { }
        public LotteryLogInfo(NameValueCollection form) : base(form) { }
        public LotteryLogInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public int LotteryID { get; set; }
        public string CookieSN { get; set; }
        public string WXOpenID { get; set; }
        public string UserName { get; set; }
        public int LotteryCount { get; set; }
        public int LotteryDailyCount { get; set; }
        public DateTime LastLotteryDate { get; set; }

        protected override List<string> AllAttributes
        {
            get
            {
                return LotteryLogAttribute.AllAttributes;
            }
        }
    }
}
