using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class CouponSnAttribute
    {
        protected CouponSnAttribute()
        {
        }

        public const string Id = nameof(CouponSnInfo.Id);
        public const string PublishmentSystemId = nameof(CouponSnInfo.PublishmentSystemId);
        public const string CouponId = nameof(CouponSnInfo.CouponId);
        public const string Sn = nameof(CouponSnInfo.Sn);
        public const string Status = nameof(CouponSnInfo.Status);
        public const string HoldDate = nameof(CouponSnInfo.HoldDate);
        public const string HoldRealName = nameof(CouponSnInfo.HoldRealName);
        public const string HoldMobile = nameof(CouponSnInfo.HoldMobile);
        public const string HoldEmail = nameof(CouponSnInfo.HoldEmail);
        public const string HoldAddress = nameof(CouponSnInfo.HoldAddress);
        public const string CookieSn = nameof(CouponSnInfo.CookieSn);
        public const string WxOpenId = nameof(CouponSnInfo.WxOpenId);
        public const string CashDate = nameof(CouponSnInfo.CashDate);
        public const string CashUserName = nameof(CouponSnInfo.CashUserName);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            CouponId,
            Sn,
            Status,
            HoldDate,
            HoldRealName,
            HoldMobile,
            HoldEmail,
            HoldAddress,
            CookieSn,
            WxOpenId,
            CashDate,
            CashUserName
        });
    }

    public class CouponSnInfo : BaseInfo
    {
        public CouponSnInfo()
        {
            HoldDate = DateTime.Now;
            CashDate = DateTime.Now;
        }

        public CouponSnInfo(object dataItem) : base(dataItem) { }
        public CouponSnInfo(NameValueCollection form) : base(form) { }
        public CouponSnInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public int CouponId { get; set; }
        public string Sn { get; set; }
        public string Status { get; set; }
        public DateTime HoldDate { get; set; }
        public string HoldRealName { get; set; }
        public string HoldMobile { get; set; }
        public string HoldEmail { get; set; }
        public string HoldAddress { get; set; }
        public string CookieSn { get; set; }
        public string WxOpenId { get; set; }
        public DateTime CashDate { get; set; }
        public string CashUserName { get; set; }

        protected override List<string> AllAttributes => CouponSnAttribute.AllAttributes;
    }
}
