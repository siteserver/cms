using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class WebMenuAttribute
    {
        protected WebMenuAttribute()
        {
        }

        public const string Id = nameof(WebMenuInfo.Id);
        public const string PublishmentSystemId = nameof(WebMenuInfo.PublishmentSystemId);
        public const string MenuName = nameof(WebMenuInfo.MenuName);
        public const string IconUrl = nameof(WebMenuInfo.IconUrl);
        public const string IconCssClass = nameof(WebMenuInfo.IconCssClass);
        public const string NavigationType = nameof(WebMenuInfo.NavigationType);
        public const string Url = nameof(WebMenuInfo.Url);
        public const string ChannelId = nameof(WebMenuInfo.ChannelId);
        public const string ContentId = nameof(WebMenuInfo.ContentId);
        public const string KeywordType = nameof(WebMenuInfo.KeywordType);
        public const string FunctionId = nameof(WebMenuInfo.FunctionId);
        public const string ParentId = nameof(WebMenuInfo.ParentId);
        public const string Taxis = nameof(WebMenuInfo.Taxis);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            MenuName,
            IconUrl,
            IconCssClass,
            NavigationType,
            Url,
            ChannelId,
            ContentId,
            KeywordType,
            FunctionId,
            ParentId,
            Taxis
        });
    }

    public class WebMenuInfo : BaseInfo
    {
        public WebMenuInfo() { }
        public WebMenuInfo(object dataItem) : base(dataItem) { }
        public WebMenuInfo(NameValueCollection form) : base(form) { }
        public WebMenuInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public string MenuName { get; set; }
        public string IconUrl { get; set; }
        public string IconCssClass { get; set; }
        public string NavigationType { get; set; }
        public string Url { get; set; }
        public int ChannelId { get; set; }
        public int ContentId { get; set; }
        public string KeywordType { get; set; }
        public int FunctionId { get; set; }
        public int ParentId { get; set; }
        public int Taxis { get; set; }

        protected override List<string> AllAttributes => WebMenuAttribute.AllAttributes;
    }
}
