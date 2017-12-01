using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class ConferenceContentAttribute
    {
        protected ConferenceContentAttribute()
        {
        }

        public const string Id = nameof(ConferenceContentInfo.Id);
        public const string PublishmentSystemId = nameof(ConferenceContentInfo.PublishmentSystemId);
        public const string ConferenceId = nameof(ConferenceContentInfo.ConferenceId);
        public const string IpAddress = nameof(ConferenceContentInfo.IpAddress);
        public const string CookieSn = nameof(ConferenceContentInfo.CookieSn);
        public const string WxOpenId = nameof(ConferenceContentInfo.WxOpenId);
        public const string UserName = nameof(ConferenceContentInfo.UserName);
        public const string AddDate = nameof(ConferenceContentInfo.AddDate);
        public const string RealName = nameof(ConferenceContentInfo.RealName);
        public const string Mobile = nameof(ConferenceContentInfo.Mobile);
        public const string Email = nameof(ConferenceContentInfo.Email);
        public const string Company = nameof(ConferenceContentInfo.Company);
        public const string Position = nameof(ConferenceContentInfo.Position);
        public const string Note = nameof(ConferenceContentInfo.Note);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            ConferenceId,
            IpAddress,
            CookieSn,
            WxOpenId,
            UserName,
            AddDate,
            RealName,
            Mobile,
            Email,
            Company,
            Position,
            Note
        });
    }

    public class ConferenceContentInfo : BaseInfo
    {
        public ConferenceContentInfo() { }
        public ConferenceContentInfo(object dataItem) : base(dataItem) { }
        public ConferenceContentInfo(NameValueCollection form) : base(form) { }
        public ConferenceContentInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public int ConferenceId { get; set; }
        public string IpAddress { get; set; }
        public string CookieSn { get; set; }
        public string WxOpenId { get; set; }
        public string UserName { get; set; }
        public DateTime AddDate { get; set; }
        public string RealName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }
        public string Note { get; set; }

        protected override List<string> AllAttributes => ConferenceContentAttribute.AllAttributes;
    }
}
