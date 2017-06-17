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

        public const string Id = nameof(ConfigExtendInfo.Id);
        public const string PublishmentSystemId = nameof(ConfigExtendInfo.PublishmentSystemId);
        public const string KeywordType = nameof(ConfigExtendInfo.KeywordType);
        public const string FunctionId = nameof(ConfigExtendInfo.FunctionId);
        public const string AttributeName = nameof(ConfigExtendInfo.AttributeName);
        public const string IsVisible = nameof(ConfigExtendInfo.IsVisible);

        private static List<string> _allAttributes;
        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            PublishmentSystemId,
            KeywordType,
            FunctionId,
            AttributeName,
            IsVisible
        });
    }

    public class ConfigExtendInfo : BaseInfo
    {
        public ConfigExtendInfo() { }
        public ConfigExtendInfo(object dataItem) : base(dataItem) { }
        public ConfigExtendInfo(NameValueCollection form) : base(form) { }
        public ConfigExtendInfo(IDataReader rdr) : base(rdr) { }
        public int PublishmentSystemId { get; set; }
        public string KeywordType { get; set; }
        public int FunctionId { get; set; }
        public string AttributeName { get; set; }
        public string IsVisible { get; set; }

        protected override List<string> AllAttributes => ConfigExtendAttribute.AllAttributes;
    }
}
