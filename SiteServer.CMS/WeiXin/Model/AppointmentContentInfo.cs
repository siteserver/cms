using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class AppointmentContentAttribute
    {
        protected AppointmentContentAttribute()
        {
        }

        public const string Id = nameof(AppointmentContentInfo.Id);
        public const string PublishmentSystemId = nameof(AppointmentContentInfo.PublishmentSystemId);
        public const string AppointmentId = nameof(AppointmentContentInfo.AppointmentId);
        public const string AppointmentItemId = nameof(AppointmentContentInfo.AppointmentItemId);
        public const string CookieSn = nameof(AppointmentContentInfo.CookieSn);
        public const string WxOpenId = nameof(AppointmentContentInfo.WxOpenId);
        public const string UserName = nameof(AppointmentContentInfo.UserName);
        public const string RealName = nameof(AppointmentContentInfo.RealName);
        public const string Mobile = nameof(AppointmentContentInfo.Mobile);
        public const string Email = nameof(AppointmentContentInfo.Email);
        public const string SettingsXml = nameof(AppointmentContentInfo.SettingsXml);
        public const string Status = nameof(AppointmentContentInfo.Status);
        public const string Message = nameof(AppointmentContentInfo.Message);
        public const string AddDate = nameof(AppointmentContentInfo.AddDate);
        public const string Reason = nameof(AppointmentContentInfo.Reason);
        public const string StartDate = nameof(AppointmentContentInfo.StartDate);
        public const string EndDate = nameof(AppointmentContentInfo.EndDate);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            AppointmentId,
            AppointmentItemId,
            CookieSn,
            WxOpenId,
            UserName,
            RealName,
            Mobile,
            Email,
            SettingsXml,
            Status,
            Message,
            AddDate,
            Reason,
            StartDate,
            EndDate
        });
    }

    public class AppointmentContentInfo : BaseInfo
    {
        public AppointmentContentInfo() { }
        public AppointmentContentInfo(object dataItem) : base(dataItem) { }
        public AppointmentContentInfo(NameValueCollection form) : base(form) { }
        public AppointmentContentInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public int AppointmentId { get; set; }
        public int AppointmentItemId { get; set; }
        public string CookieSn { get; set; }
        public string WxOpenId { get; set; }
        public string UserName { get; set; }
        public string RealName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string SettingsXml { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public DateTime AddDate { get; set; }
        public string Reason { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        protected override List<string> AllAttributes => AppointmentContentAttribute.AllAttributes;
    }
}
