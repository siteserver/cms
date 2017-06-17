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

        public const string Id = nameof(LotteryLogInfo.Id);
        public const string PublishmentSystemId = nameof(LotteryLogInfo.PublishmentSystemId);
        public const string LotteryId = nameof(LotteryLogInfo.LotteryId);
        public const string CookieSn = nameof(LotteryLogInfo.CookieSn);
        public const string WxOpenId = nameof(LotteryLogInfo.WxOpenId);
        public const string UserName = nameof(LotteryLogInfo.UserName);
        public const string LotteryCount = nameof(LotteryLogInfo.LotteryCount);
        public const string LotteryDailyCount = nameof(LotteryLogInfo.LotteryDailyCount);
        public const string LastLotteryDate = nameof(LotteryLogInfo.LastLotteryDate);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            LotteryId,
            CookieSn,
            WxOpenId,
            UserName,
            LotteryCount,
            LotteryDailyCount,
            LastLotteryDate
        });
    }

    public class LotteryLogInfo : BaseInfo
    {
        public LotteryLogInfo() { }
        public LotteryLogInfo(object dataItem) : base(dataItem) { }
        public LotteryLogInfo(NameValueCollection form) : base(form) { }
        public LotteryLogInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public int LotteryId { get; set; }
        public string CookieSn { get; set; }
        public string WxOpenId { get; set; }
        public string UserName { get; set; }
        public int LotteryCount { get; set; }
        public int LotteryDailyCount { get; set; }
        public DateTime LastLotteryDate { get; set; }

        protected override List<string> AllAttributes => LotteryLogAttribute.AllAttributes;
    }
}
