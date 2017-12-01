using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class CollectItemAttribute
    {
        protected CollectItemAttribute()
        {
        }

        public const string Id = nameof(CollectItemInfo.Id);
        public const string CollectId = nameof(CollectItemInfo.CollectId);
        public const string PublishmentSystemId = nameof(CollectItemInfo.PublishmentSystemId);
        public const string Title = nameof(CollectItemInfo.Title);
        public const string SmallUrl = nameof(CollectItemInfo.SmallUrl);
        public const string LargeUrl = nameof(CollectItemInfo.LargeUrl);
        public const string Description = nameof(CollectItemInfo.Description);
        public const string Mobile = nameof(CollectItemInfo.Mobile);
        public const string IsChecked = nameof(CollectItemInfo.IsChecked);
        public const string VoteNum = nameof(CollectItemInfo.VoteNum);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            CollectId,
            PublishmentSystemId,
            Title,
            SmallUrl,
            LargeUrl,
            Description,
            Mobile,
            IsChecked,
            VoteNum
        });
    }

    public class CollectItemInfo : BaseInfo
    {
        public CollectItemInfo() { }
        public CollectItemInfo(object dataItem) : base(dataItem) { }
        public CollectItemInfo(NameValueCollection form) : base(form) { }
        public CollectItemInfo(IDataReader rdr) : base(rdr) { }
        public int CollectId { get; set; }
        public int PublishmentSystemId { get; set; }
        public string Title { get; set; }
        public string SmallUrl { get; set; }
        public string LargeUrl { get; set; }
        public string Description { get; set; }
        public string Mobile { get; set; }
        public bool IsChecked { get; set; }
        public int VoteNum { get; set; }

        protected override List<string> AllAttributes => CollectItemAttribute.AllAttributes;
    }
}
