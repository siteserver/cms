using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class AppointmentAttribute
    {
        protected AppointmentAttribute()
        {
        }

        public const string Id = nameof(AppointmentInfo.Id);
        public const string PublishmentSystemId = nameof(AppointmentInfo.PublishmentSystemId);
        public const string KeywordId = nameof(AppointmentInfo.KeywordId);
        public const string UserCount = nameof(AppointmentInfo.UserCount);
        public const string PvCount = nameof(AppointmentInfo.PvCount);
        public const string StartDate = nameof(AppointmentInfo.StartDate);
        public const string EndDate = nameof(AppointmentInfo.EndDate);
        public const string IsDisabled = nameof(AppointmentInfo.IsDisabled);
        public const string Title = nameof(AppointmentInfo.Title);
        public const string ImageUrl = nameof(AppointmentInfo.ImageUrl);
        public const string Summary = nameof(AppointmentInfo.Summary);
        public const string ContentIsSingle = nameof(AppointmentInfo.ContentIsSingle);
        public const string ContentImageUrl = nameof(AppointmentInfo.ContentImageUrl);
        public const string ContentDescription = nameof(AppointmentInfo.ContentDescription);
        public const string ContentResultTopImageUrl = nameof(AppointmentInfo.ContentResultTopImageUrl);
        public const string EndTitle = nameof(AppointmentInfo.EndTitle);
        public const string EndImageUrl = nameof(AppointmentInfo.EndImageUrl);
        public const string EndSummary = nameof(AppointmentInfo.EndSummary);
        public const string IsFormRealName = nameof(AppointmentInfo.IsFormRealName);
        public const string FormRealNameTitle = nameof(AppointmentInfo.FormRealNameTitle);
        public const string IsFormMobile = nameof(AppointmentInfo.IsFormMobile);
        public const string FormMobileTitle = nameof(AppointmentInfo.FormMobileTitle);
        public const string IsFormEmail = nameof(AppointmentInfo.IsFormEmail);
        public const string FormEmailTitle = nameof(AppointmentInfo.FormEmailTitle);
          
        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            KeywordId,
            UserCount,
            PvCount,
            StartDate,
            EndDate,
            IsDisabled,
            Title,
            ImageUrl,
            Summary,
            ContentIsSingle,
            ContentImageUrl,
            ContentDescription,
            ContentResultTopImageUrl,
            EndTitle,
            EndImageUrl,
            EndSummary,
            IsFormRealName,
            FormRealNameTitle,
            IsFormMobile,
            FormMobileTitle,
            IsFormEmail,
            FormEmailTitle
        });
    }

    public class AppointmentInfo : BaseInfo
    {
        public AppointmentInfo() { }
        public AppointmentInfo(object dataItem) : base(dataItem) { }
        public AppointmentInfo(NameValueCollection form) : base(form) { }
        public AppointmentInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public int KeywordId { get; set; }
        public int UserCount { get;set;}
        public int PvCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsDisabled { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Summary { get; set; }
        public bool ContentIsSingle { get; set; }
        public string ContentImageUrl { get; set; }
        public string ContentDescription { get; set; }
        public string ContentResultTopImageUrl { get; set; }
        public string EndTitle { get; set; }
        public string EndImageUrl { get; set; }
        public string EndSummary { get; set; }
        public string IsFormRealName { get; set; }
        public string FormRealNameTitle { get; set; }
        public string IsFormMobile { get; set; }
        public string FormMobileTitle { get; set; }
        public string IsFormEmail { get; set; }
        public string FormEmailTitle { get; set; }

        protected override List<string> AllAttributes => AppointmentAttribute.AllAttributes;
    }
}
