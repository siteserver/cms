using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class MapAttribute
    {
        protected MapAttribute()
        {
        }

        public const string Id = nameof(MapInfo.Id);
        public const string PublishmentSystemId = nameof(MapInfo.PublishmentSystemId);
        public const string KeywordId = nameof(MapInfo.KeywordId);
        public const string IsDisabled = nameof(MapInfo.IsDisabled);
        public const string PvCount = nameof(MapInfo.PvCount);
        public const string Title = nameof(MapInfo.Title);
        public const string ImageUrl = nameof(MapInfo.ImageUrl);
        public const string Summary = nameof(MapInfo.Summary);
        public const string MapWd = nameof(MapInfo.MapWd);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            KeywordId,
            IsDisabled,
            PvCount,
            Title,
            ImageUrl,
            Summary,
            MapWd
        });
    }

    public class MapInfo : BaseInfo
    {
        public MapInfo() { }
        public MapInfo(object dataItem) : base(dataItem) { }
        public MapInfo(NameValueCollection form) : base(form) { }
        public MapInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public int KeywordId { get; set; }
        public bool IsDisabled { get; set; }
        public int PvCount { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Summary { get; set; }
        public string MapWd { get; set; }

        protected override List<string> AllAttributes => MapAttribute.AllAttributes;
    }
}
