using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class LotteryWinnerAttribute
    {
        protected LotteryWinnerAttribute()
        {
        }

        public const string Id = nameof(LotteryWinnerInfo.Id);
        public const string PublishmentSystemId = nameof(LotteryWinnerInfo.PublishmentSystemId);
        public const string LotteryType = nameof(LotteryWinnerInfo.LotteryType);
        public const string LotteryId = nameof(LotteryWinnerInfo.LotteryId);
        public const string AwardId = nameof(LotteryWinnerInfo.AwardId);
        public const string Status = nameof(LotteryWinnerInfo.Status);
        public const string CookieSn = nameof(LotteryWinnerInfo.CookieSn);
        public const string WxOpenId = nameof(LotteryWinnerInfo.WxOpenId);
        public const string UserName = nameof(LotteryWinnerInfo.UserName);
        public const string RealName = nameof(LotteryWinnerInfo.RealName);
        public const string Mobile = nameof(LotteryWinnerInfo.Mobile);
        public const string Email = nameof(LotteryWinnerInfo.Email);
        public const string Address = nameof(LotteryWinnerInfo.Address);
        public const string AddDate = nameof(LotteryWinnerInfo.AddDate);
        public const string CashSn = nameof(LotteryWinnerInfo.CashSn);
        public const string CashDate = nameof(LotteryWinnerInfo.CashDate);
        

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            LotteryType,
            LotteryId,
            AwardId,
            Status,
            CookieSn,
            WxOpenId,
            UserName,
            RealName,
            Mobile,
            Email,
            Address,
            AddDate,
            CashSn,
            CashDate
        });
    }

    public class LotteryWinnerInfo : BaseInfo
    {
        public LotteryWinnerInfo() { }
        public LotteryWinnerInfo(object dataItem) : base(dataItem) { }
        public LotteryWinnerInfo(NameValueCollection form) : base(form) { }
        public LotteryWinnerInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public string LotteryType { get; set; }
        public int LotteryId { get; set; }
        public int AwardId { get; set; }
        public string Status { get; set; }
        public string CookieSn { get; set; }
        public string WxOpenId { get; set; }
        public string UserName { get; set; }
        public string RealName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime AddDate { get; set; }
        public string CashSn { get; set; }
        public DateTime CashDate { get; set; }

        protected override List<string> AllAttributes => LotteryWinnerAttribute.AllAttributes;
    }
}
