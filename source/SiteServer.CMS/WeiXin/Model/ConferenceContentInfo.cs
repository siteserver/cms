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

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string ConferenceID = "ConferenceID";
        public const string IPAddress = "IPAddress";
        public const string CookieSN = "CookieSN";
        public const string WXOpenID = "WXOpenID";
        public const string UserName = "UserName";
        public const string AddDate = "AddDate";
        public const string RealName = "RealName";
        public const string Mobile = "Mobile";
        public const string Email = "Email";
        public const string Company = "Company";
        public const string Position = "Position";
        public const string Note = "Note";

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
                    allAttributes.Add(ConferenceID);
                    allAttributes.Add(IPAddress);
                    allAttributes.Add(CookieSN);
                    allAttributes.Add(WXOpenID);
                    allAttributes.Add(UserName);
                    allAttributes.Add(AddDate);
                    allAttributes.Add(RealName);
                    allAttributes.Add(Mobile);
                    allAttributes.Add(Email);
                    allAttributes.Add(Company);
                    allAttributes.Add(Position);
                    allAttributes.Add(Note);
                }

                return allAttributes;
            }
        }
    }
    public class ConferenceContentInfo : BaseInfo
    {
        public ConferenceContentInfo() { }
        public ConferenceContentInfo(object dataItem) : base(dataItem) { }
        public ConferenceContentInfo(NameValueCollection form) : base(form) { }
        public ConferenceContentInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public int ConferenceID { get; set; }
        public string IPAddress { get; set; }
        public string CookieSN { get; set; }
        public string WXOpenID { get; set; }
        public string UserName { get; set; }
        public DateTime AddDate { get; set; }
        public string RealName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }
        public string Note { get; set; }

        protected override List<string> AllAttributes
        {
            get
            {
                return ConferenceContentAttribute.AllAttributes;
            }
        }
    }
}
