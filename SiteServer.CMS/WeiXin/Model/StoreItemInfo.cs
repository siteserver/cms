using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class StoreItemAttribute
    {
        protected StoreItemAttribute()
        {
        }

        public const string Id = nameof(StoreItemInfo.Id);
        public const string PublishmentSystemId = nameof(StoreItemInfo.PublishmentSystemId);
        public const string StoreId = nameof(StoreItemInfo.StoreId);
        public const string CategoryId = nameof(StoreItemInfo.CategoryId);
        public const string StoreName = nameof(StoreItemInfo.StoreName);
        public const string Tel = nameof(StoreItemInfo.Tel);
        public const string Mobile = nameof(StoreItemInfo.Mobile);
        public const string ImageUrl = nameof(StoreItemInfo.ImageUrl);
        public const string Address = nameof(StoreItemInfo.Address);
        public const string Longitude = nameof(StoreItemInfo.Longitude);
        public const string Latitude = nameof(StoreItemInfo.Latitude);
        public const string Summary = nameof(StoreItemInfo.Summary);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            StoreId,
            CategoryId,
            StoreName,
            Tel,
            Mobile,
            ImageUrl,
            Address,
            Longitude,
            Latitude,
            Summary
        });
    }

    public class StoreItemInfo : BaseInfo
    {
        public StoreItemInfo() { }
        public StoreItemInfo(object dataItem) : base(dataItem) { }
        public StoreItemInfo(NameValueCollection form) : base(form) { }
        public StoreItemInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public int StoreId { get; set; }
        public int CategoryId { get; set; }
        public string StoreName { get; set; }
        public string Tel { get; set; }
        public string Mobile { get; set; }
        public string ImageUrl { get; set; }
        public string Address { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Summary { get; set; }
        
        protected override List<string> AllAttributes => StoreItemAttribute.AllAttributes;
    }
}
