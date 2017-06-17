using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class CardSignLogAttribute
    {
        protected CardSignLogAttribute()
        {
        }

        public const string Id = nameof(CardSignLogInfo.Id);
        public const string PublishmentSystemId = nameof(CardSignLogInfo.PublishmentSystemId);
        public const string UserName = nameof(CardSignLogInfo.UserName);
        public const string SignDate = nameof(CardSignLogInfo.SignDate);
         
        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            UserName,
            SignDate
        });
    }

    public class CardSignLogInfo : BaseInfo
    {
        public CardSignLogInfo() { }
        public CardSignLogInfo(object dataItem) : base(dataItem) { }
        public CardSignLogInfo(NameValueCollection form) : base(form) { }
        public CardSignLogInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public string UserName { get; set; }
        public DateTime SignDate { get; set; }
         
        protected override List<string> AllAttributes => CardSignLogAttribute.AllAttributes;
    }
}
