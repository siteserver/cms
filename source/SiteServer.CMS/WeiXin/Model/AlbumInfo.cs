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

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string KeywordID = "KeywordID";
        public const string IsDisabled = "IsDisabled";
        public const string PVCount = "PVCount";
        public const string Title = "Title";
        public const string ImageUrl = "ImageUrl";
        public const string Summary = "Summary";
          
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
                    allAttributes.Add(KeywordID);
                    allAttributes.Add(IsDisabled);
                    allAttributes.Add(PVCount);
                    allAttributes.Add(Title);
                    allAttributes.Add(ImageUrl);
                    allAttributes.Add(Summary);
                    
                }

                return allAttributes;
            }
        }
    }
    public class AlbumInfo : BaseInfo
    {
        public AlbumInfo() { }
        public AlbumInfo(object dataItem) : base(dataItem) { }
        public AlbumInfo(NameValueCollection form) : base(form) { }
        public AlbumInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public int KeywordID { get; set; }
        public bool IsDisabled { get; set; }
        public int PVCount { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Summary { get; set; }
        
        protected override List<string> AllAttributes
        {
            get
            {
                return AlbumAttribute.AllAttributes;
            }
        }
    }
}
