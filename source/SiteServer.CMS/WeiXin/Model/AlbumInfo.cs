using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class AlbumAttribute
    {
        protected AlbumAttribute()
        {
        }

        public const string Id = nameof(AlbumInfo.Id);
        public const string PublishmentSystemId = nameof(AlbumInfo.PublishmentSystemId);
        public const string KeywordId = nameof(AlbumInfo.KeywordId);
        public const string IsDisabled = nameof(AlbumInfo.IsDisabled);
        public const string PvCount = nameof(AlbumInfo.PvCount);
        public const string Title = nameof(AlbumInfo.Title);
        public const string ImageUrl = nameof(AlbumInfo.ImageUrl);
        public const string Summary = nameof(AlbumInfo.Summary);
          
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
            Summary
        });
    }

    public class AlbumInfo : BaseInfo
    {
        public AlbumInfo() { }
        public AlbumInfo(object dataItem) : base(dataItem) { }
        public AlbumInfo(NameValueCollection form) : base(form) { }
        public AlbumInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public int KeywordId { get; set; }
        public bool IsDisabled { get; set; }
        public int PvCount { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Summary { get; set; }
        
        protected override List<string> AllAttributes => AlbumAttribute.AllAttributes;
    }
}
