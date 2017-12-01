using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class ConferenceAttribute
    {
        protected ConferenceAttribute()
        {
        }

        public const string Id = nameof(ConferenceInfo.Id);
        public const string PublishmentSystemId = nameof(ConferenceInfo.PublishmentSystemId);
        public const string KeywordId = nameof(ConferenceInfo.KeywordId);
        public const string IsDisabled = nameof(ConferenceInfo.IsDisabled);
        public const string UserCount = nameof(ConferenceInfo.UserCount);
        public const string PvCount = nameof(ConferenceInfo.PvCount);
        public const string StartDate = nameof(ConferenceInfo.StartDate);
        public const string EndDate = nameof(ConferenceInfo.EndDate);
        public const string Title = nameof(ConferenceInfo.Title);
        public const string ImageUrl = nameof(ConferenceInfo.ImageUrl);
        public const string Summary = nameof(ConferenceInfo.Summary);
        public const string BackgroundImageUrl = nameof(ConferenceInfo.BackgroundImageUrl);
        public const string ConferenceName = nameof(ConferenceInfo.ConferenceName);
        public const string Address = nameof(ConferenceInfo.Address);
        public const string Duration = nameof(ConferenceInfo.Duration);
        public const string Introduction = nameof(ConferenceInfo.Introduction);
        public const string IsAgenda = nameof(ConferenceInfo.IsAgenda);
        public const string AgendaTitle = nameof(ConferenceInfo.AgendaTitle);
        public const string AgendaList = nameof(ConferenceInfo.AgendaList);
        public const string IsGuest = nameof(ConferenceInfo.IsGuest);
        public const string GuestTitle = nameof(ConferenceInfo.GuestTitle);
        public const string GuestList = nameof(ConferenceInfo.GuestList);
        public const string EndTitle = nameof(ConferenceInfo.EndTitle);
        public const string EndImageUrl = nameof(ConferenceInfo.EndImageUrl);
        public const string EndSummary = nameof(ConferenceInfo.EndSummary);

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
            BackgroundImageUrl,
            ConferenceName,
            Address,
            Duration,
            Introduction,
            IsAgenda,
            AgendaTitle,
            AgendaList,
            IsGuest,
            GuestTitle,
            GuestList,
            EndTitle,
            EndImageUrl,
            EndSummary
        });
    }

    public class ConferenceInfo : BaseInfo
    {
        public ConferenceInfo() { }
        public ConferenceInfo(object dataItem) : base(dataItem) { }
        public ConferenceInfo(NameValueCollection form) : base(form) { }
        public ConferenceInfo(IDataReader rdr) : base(rdr) { }
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
        public string BackgroundImageUrl { get; set; }
        public string ConferenceName { get; set; }
        public string Address { get; set; }
        public string Duration { get; set; }
        public string Introduction { get; set; }
        public bool IsAgenda { get; set; }
        public string AgendaTitle { get; set; }
        public string AgendaList { get; set; }
        public bool IsGuest { get; set; }
        public string GuestTitle { get; set; }
        public string GuestList { get; set; }
        public string EndTitle { get; set; }
        public string EndImageUrl { get; set; }
        public string EndSummary { get; set; }

        protected override List<string> AllAttributes => ConferenceAttribute.AllAttributes;
    }
}
