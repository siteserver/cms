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

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string KeywordID = "KeywordID";
        public const string IsDisabled = "IsDisabled";
        public const string UserCount = "UserCount";
        public const string PVCount = "PVCount";
        public const string StartDate = "StartDate";
        public const string EndDate = "EndDate";
        public const string Title = "Title";
        public const string ImageUrl = "ImageUrl";
        public const string Summary = "Summary";
        public const string BackgroundImageUrl = "BackgroundImageUrl";
        public const string ConferenceName = "ConferenceName";
        public const string Address = "Address";
        public const string Duration = "Duration";
        public const string Introduction = "Introduction";
        public const string IsAgenda = "IsAgenda";
        public const string AgendaTitle = "AgendaTitle";
        public const string AgendaList = "AgendaList";
        public const string IsGuest = "IsGuest";
        public const string GuestTitle = "GuestTitle";
        public const string GuestList = "GuestList";
        public const string EndTitle = "EndTitle";
        public const string EndImageUrl = "EndImageUrl";
        public const string EndSummary = "EndSummary";

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
                    allAttributes.Add(KeywordID);
                    allAttributes.Add(IsDisabled);
                    allAttributes.Add(UserCount);
                    allAttributes.Add(PVCount);
                    allAttributes.Add(StartDate);
                    allAttributes.Add(EndDate);
                    allAttributes.Add(Title);
                    allAttributes.Add(ImageUrl);
                    allAttributes.Add(Summary);
                    allAttributes.Add(BackgroundImageUrl);
                    allAttributes.Add(ConferenceName);
                    allAttributes.Add(Address);
                    allAttributes.Add(Duration);
                    allAttributes.Add(Introduction);
                    allAttributes.Add(IsAgenda);
                    allAttributes.Add(AgendaTitle);
                    allAttributes.Add(AgendaList);
                    allAttributes.Add(IsGuest);
                    allAttributes.Add(GuestTitle);
                    allAttributes.Add(GuestList);
                    allAttributes.Add(EndTitle);
                    allAttributes.Add(EndImageUrl);
                    allAttributes.Add(EndSummary);
                }

                return allAttributes;
            }
        }
    }
    public class ConferenceInfo : BaseInfo
    {
        public ConferenceInfo() { }
        public ConferenceInfo(object dataItem) : base(dataItem) { }
        public ConferenceInfo(NameValueCollection form) : base(form) { }
        public ConferenceInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public int KeywordID { get; set; }
        public bool IsDisabled { get; set; }
        public int UserCount { get; set; }
        public int PVCount { get; set; }
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

        protected override List<string> AllAttributes
        {
            get
            {
                return ConferenceAttribute.AllAttributes;
            }
        }
    }
}
