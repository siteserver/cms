using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class LotteryAttribute
    {
        protected LotteryAttribute()
        {
        }

        public const string Id = nameof(LotteryInfo.Id);
        public const string PublishmentSystemId = nameof(LotteryInfo.PublishmentSystemId);
        public const string LotteryType = nameof(LotteryInfo.LotteryType);
        public const string KeywordId = nameof(LotteryInfo.KeywordId);
        public const string IsDisabled = nameof(LotteryInfo.IsDisabled);
        public const string UserCount = nameof(LotteryInfo.UserCount);
        public const string PvCount = nameof(LotteryInfo.PvCount);
        public const string StartDate = nameof(LotteryInfo.StartDate);
        public const string EndDate = nameof(LotteryInfo.EndDate);
        public const string Title = nameof(LotteryInfo.Title);
        public const string ImageUrl = nameof(LotteryInfo.ImageUrl);
        public const string Summary = nameof(LotteryInfo.Summary);
        public const string ContentImageUrl = nameof(LotteryInfo.ContentImageUrl);
        public const string ContentAwardImageUrl = nameof(LotteryInfo.ContentAwardImageUrl);
        public const string ContentUsage = nameof(LotteryInfo.ContentUsage);
        public const string AwardImageUrl = nameof(LotteryInfo.AwardImageUrl);
        public const string AwardUsage = nameof(LotteryInfo.AwardUsage);
        public const string IsAwardTotalNum = nameof(LotteryInfo.IsAwardTotalNum);
        public const string AwardMaxCount = nameof(LotteryInfo.AwardMaxCount);
        public const string AwardMaxDailyCount = nameof(LotteryInfo.AwardMaxDailyCount);
        public const string AwardCode = nameof(LotteryInfo.AwardCode);
        public const string IsFormRealName = nameof(LotteryInfo.IsFormRealName);
        public const string FormRealNameTitle = nameof(LotteryInfo.FormRealNameTitle);
        public const string IsFormMobile = nameof(LotteryInfo.IsFormMobile);
        public const string FormMobileTitle = nameof(LotteryInfo.FormMobileTitle);
        public const string IsFormEmail = nameof(LotteryInfo.IsFormEmail);
        public const string FormEmailTitle = nameof(LotteryInfo.FormEmailTitle);
        public const string IsFormAddress = nameof(LotteryInfo.IsFormAddress);
        public const string FormAddressTitle = nameof(LotteryInfo.FormAddressTitle);
        public const string EndTitle = nameof(LotteryInfo.EndTitle);
        public const string EndImageUrl = nameof(LotteryInfo.EndImageUrl);
        public const string EndSummary = nameof(LotteryInfo.EndSummary);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            LotteryType,
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
            ContentAwardImageUrl,
            ContentUsage,
            AwardImageUrl,
            AwardUsage,
            IsAwardTotalNum,
            AwardMaxCount,
            AwardMaxDailyCount,
            AwardCode,
            IsFormRealName,
            FormRealNameTitle,
            IsFormMobile,
            FormMobileTitle,
            IsFormEmail,
            FormEmailTitle,
            IsFormAddress,
            FormAddressTitle,
            EndTitle,
            EndImageUrl,
            EndSummary
        });
    }

    public class LotteryInfo : BaseInfo
    {
        public LotteryInfo() { }
        public LotteryInfo(object dataItem) : base(dataItem) { }
        public LotteryInfo(NameValueCollection form) : base(form) { }
        public LotteryInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public string LotteryType { get; set; }
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
        public string ContentAwardImageUrl { get; set; }
        public string ContentUsage { get; set; }
        public string AwardImageUrl { get; set; }
        public string AwardUsage { get; set; }
        public bool IsAwardTotalNum { get; set; }
        public int AwardMaxCount { get; set; }
        public int AwardMaxDailyCount { get; set; }
        public string AwardCode { get; set; }
        public bool IsFormRealName { get; set; }
        public string FormRealNameTitle { get; set; }
        public bool IsFormMobile { get; set; }
        public string FormMobileTitle { get; set; }
        public bool IsFormEmail { get; set; }
        public string FormEmailTitle { get; set; }
        public bool IsFormAddress { get; set; }
        public string FormAddressTitle { get; set; }
        public string EndTitle { get; set; }
        public string EndImageUrl { get; set; }
        public string EndSummary { get; set; }

        protected override List<string> AllAttributes => LotteryAttribute.AllAttributes;
    }
}
