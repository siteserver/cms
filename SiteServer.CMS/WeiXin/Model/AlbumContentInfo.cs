using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class AlbumContentAttribute
    {
        protected AlbumContentAttribute()
        {
        }

        public const string Id = nameof(AlbumContentInfo.Id);
        public const string PublishmentSystemId = nameof(AlbumContentInfo.PublishmentSystemId);
        public const string AlbumId = nameof(AlbumContentInfo.AlbumId);
        public const string ParentId = nameof(AlbumContentInfo.ParentId);
        public const string Title = nameof(AlbumContentInfo.Title);
        public const string ImageUrl = nameof(AlbumContentInfo.ImageUrl);
        public const string LargeImageUrl = nameof(AlbumContentInfo.LargeImageUrl);
          
        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            AlbumId,
            ParentId,
            Title,
            ImageUrl,
            LargeImageUrl
        });
    }

    public class AlbumContentInfo : BaseInfo
    {
        public AlbumContentInfo() { }
        public AlbumContentInfo(object dataItem) : base(dataItem) { }
        public AlbumContentInfo(NameValueCollection form) : base(form) { }
        public AlbumContentInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public int AlbumId { get; set; }
        public int ParentId { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string LargeImageUrl { get; set; }
        
        protected override List<string> AllAttributes => AlbumContentAttribute.AllAttributes;
    }
}
