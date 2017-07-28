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

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string AppointmentID = "AppointmentID";
        public const string UserCount = "UserCount";
        public const string Title = "Title";
        public const string TopImageUrl = "TopImageUrl";
        public const string IsDescription = "IsDescription";
        public const string DescriptionTitle = "DescriptionTitle";
        public const string Description = "Description";
        public const string IsImageUrl = "IsImageUrl";
        public const string ImageUrlTitle = "ImageUrlTitle";
        public const string ImageUrl = "ImageUrl";
        public const string IsVideoUrl = "IsVideoUrl";
        public const string VideoUrlTitle = "VideoUrlTitle";
        public const string VideoUrl = "VideoUrl";
        public const string IsImageUrlCollection = "IsImageUrlCollection";
        public const string ImageUrlCollectionTitle = "ImageUrlCollectionTitle";
        public const string ImageUrlCollection = "ImageUrlCollection";
        public const string LargeImageUrlCollection = "LargeImageUrlCollection";
        public const string IsMap = "IsMap";
        public const string MapTitle = "MapTitle";
        public const string MapAddress = "MapAddress";
        public const string IsTel = "IsTel";
        public const string TelTitle = "TelTitle";
        public const string Tel = "Tel";

          
        private static List<string> allAttributes;
        public static List<string> AllAttributes
        {
            get
            {
                if (allAttributes == null)
                {
                    allAttributes = new List<string>();
                    allAttributes.Add(ID);
                    allAttributes.Add(PublishmentSystemID);
                    allAttributes.Add(AppointmentID);
                    allAttributes.Add(UserCount);
                    allAttributes.Add(Title);
                    allAttributes.Add(TopImageUrl);
                    allAttributes.Add(IsDescription);
                    allAttributes.Add(DescriptionTitle);
                    allAttributes.Add(Description);
                    allAttributes.Add(IsImageUrl);
                    allAttributes.Add(ImageUrlTitle);
                    allAttributes.Add(ImageUrl);
                    allAttributes.Add(IsVideoUrl);
                    allAttributes.Add(VideoUrlTitle);
                    allAttributes.Add(VideoUrl);
                    allAttributes.Add(IsImageUrlCollection);
                    allAttributes.Add(ImageUrlCollectionTitle);
                    allAttributes.Add(ImageUrlCollection);
                    allAttributes.Add(LargeImageUrlCollection);
                    allAttributes.Add(IsMap);
                    allAttributes.Add(MapTitle);
                    allAttributes.Add(MapAddress);
                    allAttributes.Add(IsTel);
                    allAttributes.Add(TelTitle);
                    allAttributes.Add(Tel);
                     
                }

                return allAttributes;
            }
        }
    }
    public class AppointmentItemInfo : BaseInfo
    {
        public AppointmentItemInfo() { }
        public AppointmentItemInfo(object dataItem) : base(dataItem) { }
        public AppointmentItemInfo(NameValueCollection form) : base(form) { }
        public AppointmentItemInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public int AppointmentID { get; set; }
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
           
        protected override List<string> AllAttributes
        {
            get
            {
                return AppointmentItemAttribute.AllAttributes;
            }
        }
    }
}
