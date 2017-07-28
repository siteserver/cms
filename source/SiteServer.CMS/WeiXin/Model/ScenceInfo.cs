using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class ScenceAttribute
    {
        protected ScenceAttribute()
        {
        }

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string KeywordID = "KeywordID";
        public const string IsDisabled = "IsDisabled";
        public const string PVCount = "PVCount";
        public const string StartDate = "StartDate";
        public const string EndDate = "EndDate";
        public const string Title = "Title";
        public const string ImageUrl = "ImageUrl";
        public const string Summary = "Summary";
        public const string ClickNum = "ClickNum";

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
                    allAttributes.Add(PVCount);
                    allAttributes.Add(StartDate);
                    allAttributes.Add(EndDate);
                    allAttributes.Add(Title);
                    allAttributes.Add(ImageUrl);
                    allAttributes.Add(Summary);
                    allAttributes.Add(ClickNum);
                }

                return allAttributes;
            }
        }
    }
    public class ScenceInfo : BaseInfo
    {
        public ScenceInfo() { }
        public ScenceInfo(object dataItem) : base(dataItem) { }
        public ScenceInfo(NameValueCollection form) : base(form) { }
        public ScenceInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public int KeywordID { get; set; }
        public bool IsDisabled { get; set; }
        public int PVCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Summary { get; set; }
        public int ClickNum { get; set; }

        protected override List<string> AllAttributes
        {
            get
            {
                return ScenceAttribute.AllAttributes;
            }
        }
    }
}
