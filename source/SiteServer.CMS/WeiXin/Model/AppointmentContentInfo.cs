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

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string AppointmentID = "AppointmentID";
        public const string AppointmentItemID = "AppointmentItemID";
        public const string CookieSN = "CookieSN";
        public const string WXOpenID = "WXOpenID";
        public const string UserName = "UserName";
        public const string RealName = "RealName";
        public const string Mobile = "Mobile";
        public const string Email = "Email";
        public const string SettingsXML = "SettingsXML";
        public const string Status = "Status";
        public const string Message = "Message";
        public const string AddDate = "AddDate";
        public const string Reason = "Reason";
        public const string StartDate = "StartDate";
        public const string EndDate = "EndDate";



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
                    allAttributes.Add(AppointmentID);
                    allAttributes.Add(AppointmentItemID);
                    allAttributes.Add(CookieSN);
                    allAttributes.Add(WXOpenID);
                    allAttributes.Add(UserName);
                    allAttributes.Add(RealName);
                    allAttributes.Add(Mobile);
                    allAttributes.Add(Email);
                    allAttributes.Add(SettingsXML);
                    allAttributes.Add(Status);
                    allAttributes.Add(Message);
                    allAttributes.Add(AddDate);
                    allAttributes.Add(Reason);
                    allAttributes.Add(StartDate);
                    allAttributes.Add(EndDate);
                }

                return allAttributes;
            }
        }
    }
    public class AppointmentContentInfo : BaseInfo
    {
        public AppointmentContentInfo() { }
        public AppointmentContentInfo(object dataItem) : base(dataItem) { }
        public AppointmentContentInfo(NameValueCollection form) : base(form) { }
        public AppointmentContentInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public int AppointmentID { get; set; }
        public int AppointmentItemID { get; set; }
        public string CookieSN { get; set; }
        public string WXOpenID { get; set; }
        public string UserName { get; set; }
        public string RealName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string SettingsXML { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public DateTime AddDate { get; set; }
        public string Reason { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        protected override List<string> AllAttributes
        {
            get
            {
                return AppointmentContentAttribute.AllAttributes;
            }
        }
    }
}
