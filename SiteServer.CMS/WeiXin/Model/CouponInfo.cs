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

        public const string Id = nameof(CouponInfo.Id);
        public const string PublishmentSystemId = nameof(CouponInfo.PublishmentSystemId);
        public const string ActId = nameof(CouponInfo.ActId);
        public const string Title = nameof(CouponInfo.Title);
        public const string TotalNum = nameof(CouponInfo.TotalNum);
        public const string HoldNum = nameof(CouponInfo.HoldNum);
        public const string CashNum = nameof(CouponInfo.CashNum);
        public const string AddDate = nameof(CouponInfo.AddDate);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            ActId,
            Title,
            TotalNum,
            HoldNum,
            CashNum,
            AddDate
        });
    }

    public class CouponInfo : BaseInfo
    {
        public CouponInfo() { }
        public CouponInfo(object dataItem) : base(dataItem) { }
        public CouponInfo(NameValueCollection form) : base(form) { }
        public CouponInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public int ActId { get; set; }
        public string Title { get; set; }
        public int TotalNum { get; set; }
        public int HoldNum { get; set; }
        public int CashNum { get; set; }
        public DateTime AddDate { get; set; }

        protected override List<string> AllAttributes => CouponAttribute.AllAttributes;
    }
}
