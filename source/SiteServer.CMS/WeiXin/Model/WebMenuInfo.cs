using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Model;

namespace SiteServer.CMS.WeiXin.Model
{
    public class WebMenuAttribute
    {
        protected WebMenuAttribute()
        {
        }

        public const string ID = "ID";
        public const string PublishmentSystemID = "PublishmentSystemID";
        public const string MenuName = "MenuName";
        public const string IconUrl = "IconUrl";
        public const string IconCssClass = "IconCssClass";
        public const string NavigationType = "NavigationType";
        public const string Url = "Url";
        public const string ChannelID = "ChannelID";
        public const string ContentID = "ContentID";
        public const string KeywordType = "KeywordType";
        public const string FunctionID = "FunctionID";
        public const string ParentID = "ParentID";
        public const string Taxis = "Taxis";

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
                    allAttributes.Add(MenuName);
                    allAttributes.Add(IconUrl);
                    allAttributes.Add(IconCssClass);
                    allAttributes.Add(NavigationType);
                    allAttributes.Add(Url);
                    allAttributes.Add(ChannelID);
                    allAttributes.Add(ContentID);
                    allAttributes.Add(KeywordType);
                    allAttributes.Add(FunctionID);
                    allAttributes.Add(ParentID);
                    allAttributes.Add(Taxis);
                }

                return allAttributes;
            }
        }
    }
    public class WebMenuInfo : BaseInfo
    {
        public WebMenuInfo() { }
        public WebMenuInfo(object dataItem) : base(dataItem) { }
        public WebMenuInfo(NameValueCollection form) : base(form) { }
        public WebMenuInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemID { get; set; }
        public string MenuName { get; set; }
        public string IconUrl { get; set; }
        public string IconCssClass { get; set; }
        public string NavigationType { get; set; }
        public string Url { get; set; }
        public int ChannelID { get; set; }
        public int ContentID { get; set; }
        public string KeywordType { get; set; }
        public int FunctionID { get; set; }
        public int ParentID { get; set; }
        public int Taxis { get; set; }

        protected override List<string> AllAttributes
        {
            get
            {
                return WebMenuAttribute.AllAttributes;
            }
        }
    }
}
