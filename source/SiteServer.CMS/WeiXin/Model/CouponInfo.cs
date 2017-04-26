using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class CouponAttribute
    {
        protected CouponAttribute()
        {
        }

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string ActID = "ActID";
        public const string Title = "Title";
        public const string TotalNum = "TotalNum";
        public const string HoldNum = "HoldNum";
        public const string CashNum = "CashNum"; 
        public const string AddDate = "AddDate";

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
                    allAttributes.Add(ActID);
                    allAttributes.Add(Title);
                    allAttributes.Add(TotalNum);
                    allAttributes.Add(HoldNum);
                    allAttributes.Add(CashNum);
                    allAttributes.Add(AddDate);
                }

                return allAttributes;
            }
        }
    }
    public class CouponInfo : BaseInfo
    {
        public CouponInfo() { }
        public CouponInfo(object dataItem) : base(dataItem) { }
        public CouponInfo(NameValueCollection form) : base(form) { }
        public CouponInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public int ActID { get; set; }
        public string Title { get; set; }
        public int TotalNum { get; set; }
        public int HoldNum { get; set; }
        public int CashNum { get; set; }
        public DateTime AddDate { get; set; }

        protected override List<string> AllAttributes
        {
            get
            {
                return CouponAttribute.AllAttributes;
            }
        }
    }
}
