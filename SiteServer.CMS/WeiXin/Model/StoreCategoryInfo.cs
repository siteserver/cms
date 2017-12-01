using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{

    public class StoreCategoryAttribute
    {
        protected StoreCategoryAttribute()
        {
        }

        public const string Id = nameof(StoreCategoryInfo.Id);
        public const string PublishmentSystemId = nameof(StoreCategoryInfo.PublishmentSystemId);
        public const string CategoryName = nameof(StoreCategoryInfo.CategoryName);
        public const string ParentId = nameof(StoreCategoryInfo.ParentId);
        public const string Taxis = nameof(StoreCategoryInfo.Taxis);
        public const string ChildCount = nameof(StoreCategoryInfo.ChildCount);
        public const string ParentsCount = nameof(StoreCategoryInfo.ParentsCount);
        public const string ParentsPath = nameof(StoreCategoryInfo.ParentsPath);
        public const string StoreCount = nameof(StoreCategoryInfo.StoreCount);
        public const string IsLastNode = nameof(StoreCategoryInfo.IsLastNode);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            CategoryName,
            ParentId,
            Taxis,
            ChildCount,
            ParentsCount,
            ParentsPath,
            StoreCount,
            IsLastNode
        });
    }

    public class StoreCategoryInfo : BaseInfo
    {
        public StoreCategoryInfo() { }
        public StoreCategoryInfo(object dataItem) : base(dataItem) { }
        public StoreCategoryInfo(NameValueCollection form) : base(form) { }
        public StoreCategoryInfo(IDataReader rdr) : base(rdr) { }

        public int PublishmentSystemId { get; set; }
        public string CategoryName { get; set; }
        public int ParentId { get; set; }
        public int Taxis { get; set; }
        public int ChildCount { get; set; }
        public int ParentsCount { get; set; }
        public string ParentsPath { get; set; }
        public int StoreCount { get; set; }
        public bool IsLastNode { get; set; }

        protected override List<string> AllAttributes => StoreCategoryAttribute.AllAttributes;
    }
}
