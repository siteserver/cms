using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{

    public class StoreCategoryAttribute
    {
        protected StoreCategoryAttribute()
        {
        }

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string CategoryName = "CategoryName";
        public const string ParentID = "ParentID";
        public const string Taxis = "Taxis";
        public const string ChildCount = "ChildCount";
        public const string ParentsCount = "ParentsCount";
        public const string ParentsPath = "ParentsPath";
        public const string StoreCount = "StoreCount";
        public const string IsLastNode = "IsLastNode";        

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
                    allAttributes.Add(CategoryName);
                    allAttributes.Add(ParentID);
                    allAttributes.Add(Taxis);
                    allAttributes.Add(ChildCount);
                    allAttributes.Add(ParentsCount);
                    allAttributes.Add(ParentsPath);
                    allAttributes.Add(StoreCount);
                    allAttributes.Add(IsLastNode); 
                }

                return allAttributes;
            }
        }
    }
    public class StoreCategoryInfo : BaseInfo
    {
        public StoreCategoryInfo() { }
        public StoreCategoryInfo(object dataItem) : base(dataItem) { }
        public StoreCategoryInfo(NameValueCollection form) : base(form) { }
        public StoreCategoryInfo(IDataReader rdr) : base(rdr) { }

        public int PublishmentSystemID { get; set; }
        public string CategoryName { get; set; }
        public int ParentID { get; set; }
        public int Taxis { get; set; }
        public int ChildCount { get; set; }
        public int ParentsCount { get; set; }
        public string ParentsPath { get; set; }
        public int StoreCount { get; set; }
        public bool IsLastNode { get; set; }
        protected override List<string> AllAttributes
        {
            get
            {
                return StoreCategoryAttribute.AllAttributes;
            }
        }
    }
}
