using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class AppointmentItemAttribute
    {
        protected AppointmentItemAttribute()
        {
        }

        public const string Id = nameof(AppointmentItemInfo.Id);
        public const string PublishmentSystemId = nameof(AppointmentItemInfo.PublishmentSystemId);
        public const string AppointmentId = nameof(AppointmentItemInfo.AppointmentId);
        public const string UserCount = nameof(AppointmentItemInfo.UserCount);
        public const string Title = nameof(AppointmentItemInfo.Title);
        public const string TopImageUrl = nameof(AppointmentItemInfo.TopImageUrl);
        public const string IsDescription = nameof(AppointmentItemInfo.IsDescription);
        public const string DescriptionTitle = nameof(AppointmentItemInfo.DescriptionTitle);
        public const string Description = nameof(AppointmentItemInfo.Description);
        public const string IsImageUrl = nameof(AppointmentItemInfo.IsImageUrl);
        public const string ImageUrlTitle = nameof(AppointmentItemInfo.ImageUrlTitle);
        public const string ImageUrl = nameof(AppointmentItemInfo.ImageUrl);
        public const string IsVideoUrl = nameof(AppointmentItemInfo.IsVideoUrl);
        public const string VideoUrlTitle = nameof(AppointmentItemInfo.VideoUrlTitle);
        public const string VideoUrl = nameof(AppointmentItemInfo.VideoUrl);
        public const string IsImageUrlCollection = nameof(AppointmentItemInfo.IsImageUrlCollection);
        public const string ImageUrlCollectionTitle = nameof(AppointmentItemInfo.ImageUrlCollectionTitle);
        public const string ImageUrlCollection = nameof(AppointmentItemInfo.ImageUrlCollection);
        public const string LargeImageUrlCollection = nameof(AppointmentItemInfo.LargeImageUrlCollection);
        public const string IsMap = nameof(AppointmentItemInfo.IsMap);
        public const string MapTitle = nameof(AppointmentItemInfo.MapTitle);
        public const string MapAddress = nameof(AppointmentItemInfo.MapAddress);
        public const string IsTel = nameof(AppointmentItemInfo.IsTel);
        public const string TelTitle = nameof(AppointmentItemInfo.TelTitle);
        public const string Tel = nameof(AppointmentItemInfo.Tel);
          
        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            AppointmentId,
            UserCount,
            Title,
            TopImageUrl,
            IsDescription,
            DescriptionTitle,
            Description,
            IsImageUrl,
            ImageUrlTitle,
            ImageUrl,
            IsVideoUrl,
            VideoUrlTitle,
            VideoUrl,
            IsImageUrlCollection,
            ImageUrlCollectionTitle,
            ImageUrlCollection,
            LargeImageUrlCollection,
            IsMap,
            MapTitle,
            MapAddress,
            IsTel,
            TelTitle,
            Tel
        });
    }

    public class AppointmentItemInfo : BaseInfo
    {
        public AppointmentItemInfo() { }
        public AppointmentItemInfo(object dataItem) : base(dataItem) { }
        public AppointmentItemInfo(NameValueCollection form) : base(form) { }
        public AppointmentItemInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public int AppointmentId { get; set; }
        public int UserCount { get; set; }
        public string Title { get; set; }
        public string TopImageUrl { get; set; }
        public bool IsDescription { get; set; }
        public string DescriptionTitle { get; set; }
        public string Description { get; set; }
        public bool IsImageUrl { get; set; }
        public string ImageUrlTitle { get; set; }
        public string ImageUrl { get; set; }
        public bool IsVideoUrl { get; set; }
        public string VideoUrlTitle { get; set; }
        public string VideoUrl { get; set; }
        public bool IsImageUrlCollection { get; set; }
        public string ImageUrlCollectionTitle { get; set; }
        public string ImageUrlCollection { get; set; }
        public string LargeImageUrlCollection { get; set; }
        public bool IsMap { get; set; }
        public string MapTitle { get; set; }
        public string MapAddress { get; set; }
        public bool IsTel { get; set; }
        public string TelTitle { get; set; }
        public string Tel { get; set; }
           
        protected override List<string> AllAttributes => AppointmentItemAttribute.AllAttributes;
    }
}
