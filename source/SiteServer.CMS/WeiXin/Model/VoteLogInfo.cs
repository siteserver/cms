using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class VoteLogAttribute
    {
        protected VoteLogAttribute()
        {
        }

        public const string Id = nameof(VoteLogInfo.Id);
        public const string PublishmentSystemId = nameof(VoteLogInfo.PublishmentSystemId);
        public const string VoteId = nameof(VoteLogInfo.VoteId);
        public const string ItemIdCollection = nameof(VoteLogInfo.ItemIdCollection);
        public const string IpAddress = nameof(VoteLogInfo.IpAddress);
        public const string CookieSn = nameof(VoteLogInfo.CookieSn);
        public const string WxOpenId = nameof(VoteLogInfo.WxOpenId);
        public const string UserName = nameof(VoteLogInfo.UserName);
        public const string AddDate = nameof(VoteLogInfo.AddDate);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            VoteId,
            ItemIdCollection,
            IpAddress,
            CookieSn,
            WxOpenId,
            UserName,
            AddDate
        });
    }

    public class VoteLogInfo : BaseInfo
    {
        public VoteLogInfo() { }
        public VoteLogInfo(object dataItem) : base(dataItem) { }
        public VoteLogInfo(NameValueCollection form) : base(form) { }
        public VoteLogInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public int VoteId { get; set; }
        public string ItemIdCollection { get; set; }
        public string IpAddress { get; set; }
        public string CookieSn { get; set; }
        public string WxOpenId { get; set; }
        public string UserName { get; set; }
        public DateTime AddDate { get; set; }

        protected override List<string> AllAttributes => VoteLogAttribute.AllAttributes;
    }
}
