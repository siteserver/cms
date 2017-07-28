using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class StoreItemAttribute
    {
        protected StoreItemAttribute()
        {
        }

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string StoreID = "StoreID";
        public const string CategoryID = "CategoryID";
        public const string StoreName = "StoreName";
        public const string Tel = "Tel";
        public const string Mobile = "Mobile";
        public const string ImageUrl = "ImageUrl";
        public const string Address = "Address";
        public const string Longitude = "Longitude";
        public const string Latitude = "Latitude";
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
                    allAttributes.Add(StoreID);
                    allAttributes.Add(CategoryID);
                    allAttributes.Add(StoreName);
                    allAttributes.Add(Tel);
                    allAttributes.Add(Mobile);
                    allAttributes.Add(ImageUrl);
                    allAttributes.Add(Address);
                    allAttributes.Add(Longitude);
                    allAttributes.Add(Latitude);
                    allAttributes.Add(Summary);

                }

                return allAttributes;
            }
        }
    }

    public class StoreItemInfo : BaseInfo
    {
        public StoreItemInfo() { }
        public StoreItemInfo(object dataItem) : base(dataItem) { }
        public StoreItemInfo(NameValueCollection form) : base(form) { }
        public StoreItemInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public int StoreID { get; set; }
        public int CategoryID { get; set; }
        public string StoreName { get; set; }
        public string Tel { get; set; }
        public string Mobile { get; set; }
        public string ImageUrl { get; set; }
        public string Address { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Summary { get; set; }
        
        protected override List<string> AllAttributes
        {
            get
            {
                return StoreItemAttribute.AllAttributes;
            }
        }
    }
}
