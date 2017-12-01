using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class CouponActAttribute
    {
        protected CouponActAttribute()
        {
        }

        public const string Id = nameof(CouponActInfo.Id);
        public const string PublishmentSystemId = nameof(CouponActInfo.PublishmentSystemId);
        public const string KeywordId = nameof(CouponActInfo.KeywordId);
        public const string IsDisabled = nameof(CouponActInfo.IsDisabled);
        public const string UserCount = nameof(CouponActInfo.UserCount);
        public const string PvCount = nameof(CouponActInfo.PvCount);
        public const string StartDate = nameof(CouponActInfo.StartDate);
        public const string EndDate = nameof(CouponActInfo.EndDate);
        public const string Title = nameof(CouponActInfo.Title);
        public const string ImageUrl = nameof(CouponActInfo.ImageUrl);
        public const string Summary = nameof(CouponActInfo.Summary);
        public const string ContentImageUrl = nameof(CouponActInfo.ContentImageUrl);
        public const string ContentUsage = nameof(CouponActInfo.ContentUsage);
        public const string ContentDescription = nameof(CouponActInfo.ContentDescription);
        public const string AwardCode = nameof(CouponActInfo.AwardCode);
        public const string IsFormRealName = nameof(CouponActInfo.IsFormRealName);
        public const string FormRealNameTitle = nameof(CouponActInfo.FormRealNameTitle);
        public const string IsFormMobile = nameof(CouponActInfo.IsFormMobile);
        public const string FormMobileTitle = nameof(CouponActInfo.FormMobileTitle);
        public const string IsFormEmail = nameof(CouponActInfo.IsFormEmail);
        public const string FormEmailTitle = nameof(CouponActInfo.FormEmailTitle);
        public const string IsFormAddress = nameof(CouponActInfo.IsFormAddress);
        public const string FormAddressTitle = nameof(CouponActInfo.FormAddressTitle);
        public const string EndTitle = nameof(CouponActInfo.EndTitle);
        public const string EndImageUrl = nameof(CouponActInfo.EndImageUrl);
        public const string EndSummary = nameof(CouponActInfo.EndSummary);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
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
            ContentUsage,
            ContentDescription,
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

    public class CouponActInfo : BaseInfo
    {
        public CouponActInfo() { }
        public CouponActInfo(object dataItem) : base(dataItem) { }
        public CouponActInfo(NameValueCollection form) : base(form) { }
        public CouponActInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
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
        public string ContentUsage { get; set; }
        public string ContentDescription { get; set; }
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

        protected override List<string> AllAttributes => CouponActAttribute.AllAttributes;
    }
}
