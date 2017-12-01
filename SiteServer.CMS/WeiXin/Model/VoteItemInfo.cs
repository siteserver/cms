using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class VoteItemAttribute
    {
        protected VoteItemAttribute()
        {
        }

        public const string Id = nameof(VoteItemInfo.Id);
        public const string VoteId = nameof(VoteItemInfo.VoteId);
        public const string PublishmentSystemId = nameof(VoteItemInfo.PublishmentSystemId);
        public const string Title = nameof(VoteItemInfo.Title);
        public const string ImageUrl = nameof(VoteItemInfo.ImageUrl);
        public const string NavigationUrl = nameof(VoteItemInfo.NavigationUrl);
        public const string VoteNum = nameof(VoteItemInfo.VoteNum);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            VoteId,
            PublishmentSystemId,
            Title,
            ImageUrl,
            NavigationUrl,
            VoteNum
        });
    }

    public class VoteItemInfo : BaseInfo
    {
        public VoteItemInfo() { }
        public VoteItemInfo(object dataItem) : base(dataItem) { }
        public VoteItemInfo(NameValueCollection form) : base(form) { }
        public VoteItemInfo(IDataReader rdr) : base(rdr) { }
        public int VoteId { get; set; }
        public int PublishmentSystemId { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string NavigationUrl { get; set; }
        public int VoteNum { get; set; }

        protected override List<string> AllAttributes => VoteItemAttribute.AllAttributes;
    }
}
