using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class SearchNavigationAttribute
    {
        protected SearchNavigationAttribute()
        {
        }

        public const string Id = nameof(SearchNavigationInfo.Id);
        public const string PublishmentSystemId = nameof(SearchNavigationInfo.PublishmentSystemId);
        public const string SearchId = nameof(SearchNavigationInfo.SearchId);
        public const string Title = nameof(SearchNavigationInfo.Title);
        public const string Url = nameof(SearchNavigationInfo.Url);
        public const string ImageCssClass = nameof(SearchNavigationInfo.ImageCssClass);
        public const string NavigationType = nameof(SearchNavigationInfo.NavigationType);
        public const string KeywordType = nameof(SearchNavigationInfo.KeywordType);
        public const string FunctionId = nameof(SearchNavigationInfo.FunctionId);
        public const string ChannelId = nameof(SearchNavigationInfo.ChannelId);
        public const string ContentId = nameof(SearchNavigationInfo.ContentId);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            SearchId,
            Title,
            Url,
            ImageCssClass,
            NavigationType,
            KeywordType,
            FunctionId,
            ChannelId,
            ContentId
        });
    }

    public class SearchNavigationInfo : BaseInfo
    {
        public SearchNavigationInfo() { }
        public SearchNavigationInfo(object dataItem) : base(dataItem) { }
        public SearchNavigationInfo(NameValueCollection form) : base(form) { }
        public SearchNavigationInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public int SearchId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string ImageCssClass { get; set; }
        public string NavigationType { get; set; }
        public string KeywordType { get; set; }
        public int FunctionId { get; set; }
        public int ChannelId { get; set; }
        public int ContentId { get; set; }

        protected override List<string> AllAttributes => SearchNavigationAttribute.AllAttributes;
    }
}
