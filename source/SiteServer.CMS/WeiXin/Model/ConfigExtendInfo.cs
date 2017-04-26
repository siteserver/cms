using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class ConfigExtendAttribute
    {
        protected ConfigExtendAttribute()
        {
        }

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string KeywordType = "KeywordType";
        public const string FunctionID = "FunctionID";
        public const string AttributeName = "AttributeName";
        public const string IsVisible = "IsVisible";

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
                    allAttributes.Add(KeywordType);
                    allAttributes.Add(FunctionID);
                    allAttributes.Add(AttributeName);
                    allAttributes.Add(IsVisible);
                }

                return allAttributes;
            }
        }
    }
    public class ConfigExtendInfo : BaseInfo
    {
        public ConfigExtendInfo() { }
        public ConfigExtendInfo(object dataItem) : base(dataItem) { }
        public ConfigExtendInfo(NameValueCollection form) : base(form) { }
        public ConfigExtendInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public string KeywordType { get; set; }
        public int FunctionID { get; set; }
        public string AttributeName { get; set; }
        public string IsVisible { get; set; }

        protected override List<string> AllAttributes
        {
            get
            {
                return ConfigExtendAttribute.AllAttributes;
            }
        }
    }
}
