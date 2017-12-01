using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class CollectLogAttribute
    {
        protected CollectLogAttribute()
        {
        }

        public const string Id = nameof(CollectLogInfo.Id);
        public const string PublishmentSystemId = nameof(CollectLogInfo.PublishmentSystemId);
        public const string CollectId = nameof(CollectLogInfo.CollectId);
        public const string ItemId = nameof(CollectLogInfo.ItemId);
        public const string IpAddress = nameof(CollectLogInfo.IpAddress);
        public const string CookieSn = nameof(CollectLogInfo.CookieSn);
        public const string WxOpenId = nameof(CollectLogInfo.WxOpenId);
        public const string UserName = nameof(CollectLogInfo.UserName);
        public const string AddDate = nameof(CollectLogInfo.AddDate);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            CollectId,
            ItemId,
            IpAddress,
            CookieSn,
            WxOpenId,
            UserName,
            AddDate
        });
    }

    public class CollectLogInfo : BaseInfo
    {
        public CollectLogInfo() { }
        public CollectLogInfo(object dataItem) : base(dataItem) { }
        public CollectLogInfo(NameValueCollection form) : base(form) { }
        public CollectLogInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public int CollectId { get; set; }
        public int ItemId { get; set; }
        public string IpAddress { get; set; }
        public string CookieSn { get; set; }
        public string WxOpenId { get; set; }
        public string UserName { get; set; }
        public DateTime AddDate { get; set; }

        protected override List<string> AllAttributes => CollectLogAttribute.AllAttributes;
    }
}
