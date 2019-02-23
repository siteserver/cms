using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Extends;
using SiteServer.CMS.Database.Repositories.Contents;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_Channel")]
    public class ChannelInfo : IDataInfo, IChannelInfo, ICloneable
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string ChannelName { get; set; }

	    public int SiteId { get; set; }

        public string ContentModelPluginId { get; set; }

        public string ContentRelatedPluginIds { get; set; }

        public int ParentId { get; set; }

	    public string ParentsPath { get; set; }

	    public int ParentsCount { get; set; }

	    public int ChildrenCount { get; set; }

        private string IsLastNode { get; set; }

        [Computed]
        public bool LastNode
        {
            get => TranslateUtils.ToBool(IsLastNode);
            set => IsLastNode = value.ToString();
        }

        public string IndexName { get; set; }

	    public string GroupNameCollection { get; set; }

	    public int Taxis { get; set; }

	    public DateTime? AddDate { get; set; }

	    public string ImageUrl { get; set; }

        [Text]
        public string Content { get; set; }

        public string FilePath { get; set; }

	    public string ChannelFilePathRule { get; set; }

	    public string ContentFilePathRule { get; set; }

	    public string LinkUrl { get; set; }

	    public string LinkType { get; set; }

	    public int ChannelTemplateId { get; set; }

	    public int ContentTemplateId { get; set; }

	    public string Keywords { get; set; }

	    public string Description { get; set; }

        [Text]
        private string ExtendValues { get; set; }

        private ChannelInfoExtend _extend;

        [JsonIgnore]
        [Computed]
        public IAttributes Attributes => Extend;

        [JsonIgnore]
        [Computed]
        public ChannelInfoExtend Extend => _extend ?? (_extend = new ChannelInfoExtend(this));

        public string GetExtendValues()
        {
            return ExtendValues;
        }

        public void SetExtendValues(string json)
        {
            ExtendValues = json;
            _extend = null;
        }

        private ContentTableRepository _contentRepository;

        [JsonIgnore]
        [Computed]
        public ContentTableRepository ContentRepository => _contentRepository ?? (_contentRepository = ContentTableRepository.GetContentRepository(SiteId, ChannelManager.GetTableName(SiteManager.GetSiteInfo(SiteId), this)));

        public Dictionary<string, object> ToDictionary()
        {
            var jObject = JObject.FromObject(this);

            var styleInfoList = TableStyleManager.GetChannelStyleInfoList(this);

            foreach (var styleInfo in styleInfoList)
            {
                jObject[styleInfo.AttributeName] = Attributes.GetString(styleInfo.AttributeName);
            }

            var siteInfo = SiteManager.GetSiteInfo(SiteId);

            if (!string.IsNullOrEmpty(ImageUrl))
            {
                jObject[nameof(ImageUrl)] = PageUtility.ParseNavigationUrl(siteInfo, ImageUrl, false);
            }

            jObject["NavigationUrl"] = PageUtility.GetChannelUrl(siteInfo, this, false);

            return jObject.ToObject<Dictionary<string, object>>();
        }

        public object Clone()
        {
            return (ChannelInfo) MemberwiseClone();
        }
    }
}
