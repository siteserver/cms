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

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string AlbumID = "AlbumID";
        public const string ParentID = "ParentID";
        public const string Title = "Title";
        public const string ImageUrl = "ImageUrl";
        public const string LargeImageUrl = "LargeImageUrl";
          
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
                    allAttributes.Add(AlbumID);
                    allAttributes.Add(ParentID);
                    allAttributes.Add(Title);
                    allAttributes.Add(ImageUrl);
                    allAttributes.Add(LargeImageUrl);
                    
                }

                return allAttributes;
            }
        }
    }
    public class AlbumContentInfo : BaseInfo
    {
        public AlbumContentInfo() { }
        public AlbumContentInfo(object dataItem) : base(dataItem) { }
        public AlbumContentInfo(NameValueCollection form) : base(form) { }
        public AlbumContentInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public int AlbumID { get; set; }
        public int ParentID { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string LargeImageUrl { get; set; }
        
        protected override List<string> AllAttributes
        {
            get
            {
                return AlbumContentAttribute.AllAttributes;
            }
        }
    }
}
