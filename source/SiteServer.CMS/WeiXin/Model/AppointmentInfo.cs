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

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string KeywordID = "KeywordID";
        public const string UserCount = "UserCount";
        public const string PVCount = "PVCount";
        public const string StartDate = "StartDate";
        public const string EndDate = "EndDate";
        public const string IsDisabled = "IsDisabled";
        public const string Title = "Title";
        public const string ImageUrl = "ImageUrl";
        public const string Summary = "Summary";
        public const string ContentIsSingle = "ContentIsSingle";
        public const string ContentImageUrl = "ContentImageUrl";
        public const string ContentDescription = "ContentDescription";
        public const string ContentResultTopImageUrl = "ContentResultTopImageUrl";
        public const string EndTitle = "EndTitle";
        public const string EndImageUrl = "EndImageUrl";
        public const string EndSummary = "EndSummary";

        public const string IsFormRealName = "IsFormRealName";
        public const string FormRealNameTitle = "FormRealNameTitle";
        public const string IsFormMobile = "IsFormMobile";
        public const string FormMobileTitle = "FormMobileTitle";
        public const string IsFormEmail = "IsFormEmail";
        public const string FormEmailTitle = "FormEmailTitle";
          
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
                    allAttributes.Add(UserCount);
                    allAttributes.Add(PVCount);
                    allAttributes.Add(StartDate);
                    allAttributes.Add(EndDate);
                    allAttributes.Add(IsDisabled);
                    allAttributes.Add(Title);
                    allAttributes.Add(ImageUrl);
                    allAttributes.Add(Summary);
                    allAttributes.Add(ContentIsSingle);
                    allAttributes.Add(ContentImageUrl);
                    allAttributes.Add(ContentDescription);
                    allAttributes.Add(ContentResultTopImageUrl);
                    allAttributes.Add(EndTitle);
                    allAttributes.Add(EndImageUrl);
                    allAttributes.Add(EndSummary);

                    allAttributes.Add(IsFormRealName);
                    allAttributes.Add(FormRealNameTitle);
                    allAttributes.Add(IsFormMobile);
                    allAttributes.Add(FormMobileTitle);
                    allAttributes.Add(IsFormEmail);
                    allAttributes.Add(FormEmailTitle);

                }

                return allAttributes;
            }
        }
    }
    public class AppointmentInfo : BaseInfo
    {
        public AppointmentInfo() { }
        public AppointmentInfo(object dataItem) : base(dataItem) { }
        public AppointmentInfo(NameValueCollection form) : base(form) { }
        public AppointmentInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public int KeywordID { get; set; }
        public int UserCount { get;set;}
        public int PVCount { get; set; }
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

        protected override List<string> AllAttributes
        {
            get
            {
                return AppointmentAttribute.AllAttributes;
            }
        }
    }
}
