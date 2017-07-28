using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class CouponSNAttribute
    {
        protected CouponSNAttribute()
        {
        }

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string CouponID = "CouponID";
        public const string SN = "SN";
        public const string Status = "Status";
        public const string HoldDate = "HoldDate";
        public const string HoldRealName = "HoldRealName";
        public const string HoldMobile = "HoldMobile";
        public const string HoldEmail = "HoldEmail";
        public const string HoldAddress = "HoldAddress";
        public const string CookieSN = "CookieSN";
        public const string WXOpenID = "WXOpenID";
        public const string CashDate = "CashDate";
        public const string CashUserName = "CashUserName";

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
                    allAttributes.Add(CouponID);
                    allAttributes.Add(SN);
                    allAttributes.Add(Status);
                    allAttributes.Add(HoldDate);
                    allAttributes.Add(HoldRealName);
                    allAttributes.Add(HoldMobile);
                    allAttributes.Add(HoldEmail);
                    allAttributes.Add(HoldAddress);
                    allAttributes.Add(CookieSN);
                    allAttributes.Add(WXOpenID);
                    allAttributes.Add(CashDate);
                    allAttributes.Add(CashUserName);
                }

                return allAttributes;
            }
        }
    }
    public class CouponSNInfo : BaseInfo
    {
        public CouponSNInfo()
        {
            HoldDate = DateTime.Now;
            CashDate = DateTime.Now;
        }

        public CouponSNInfo(object dataItem) : base(dataItem) { }
        public CouponSNInfo(NameValueCollection form) : base(form) { }
        public CouponSNInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public int CouponID { get; set; }
        public string SN { get; set; }
        public string Status { get; set; }
        public DateTime HoldDate { get; set; }
        public string HoldRealName { get; set; }
        public string HoldMobile { get; set; }
        public string HoldEmail { get; set; }
        public string HoldAddress { get; set; }
        public string CookieSN { get; set; }
        public string WXOpenID { get; set; }
        public DateTime CashDate { get; set; }
        public string CashUserName { get; set; }

        protected override List<string> AllAttributes
        {
            get
            {
                return CouponSNAttribute.AllAttributes;
            }
        }
    }
}
