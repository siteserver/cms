using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Plugin;

namespace SiteServer.CMS.Model
{
	public class ChannelInfo : IChannelInfo
    {
	    private string _extendValues;

		public ChannelInfo()
		{
			Id = 0;
			ChannelName = string.Empty;
			SiteId = 0;
            ContentModelPluginId = string.Empty;
		    ContentRelatedPluginIds = string.Empty;
			ParentId = 0;
			ParentsPath = string.Empty;
			ParentsCount = 0;
			ChildrenCount = 0;
			IsLastNode = false;
			IndexName = string.Empty;
			GroupNameCollection = string.Empty;
			Taxis = 0;
			AddDate = DateTime.Now;
			ImageUrl = string.Empty;
			Content = string.Empty;
            FilePath = string.Empty;
            ChannelFilePathRule = string.Empty;
            ContentFilePathRule = string.Empty;
            LinkUrl = string.Empty;
            LinkType = string.Empty;
            ChannelTemplateId = 0;
            ContentTemplateId = 0;
            Keywords = string.Empty;
            Description = string.Empty;
            _extendValues = string.Empty;
		}

        public ChannelInfo(int id, string channelName, int siteId, string contentModelPluginId, string contentRelatedPluginIds, int parentId, string parentsPath, int parentsCount, int childrenCount, bool isLastNode, string indexName, string groupNameCollection, int taxis, DateTime addDate, string imageUrl, string content, string filePath, string channelFilePathRule, string contentFilePathRule, string linkUrl, ELinkType linkType, int channelTemplateId, int contentTemplateId, string keywords, string description, string extendValues) 
		{
			Id = id;
			ChannelName = channelName;
			SiteId = siteId;
            ContentModelPluginId = contentModelPluginId;
		    ContentRelatedPluginIds = contentRelatedPluginIds;
			ParentId = parentId;
			ParentsPath = parentsPath;
			ParentsCount = parentsCount;
			ChildrenCount = childrenCount;
			IsLastNode = isLastNode;
			IndexName = indexName;
			GroupNameCollection = groupNameCollection;
			Taxis = taxis;
			AddDate = addDate;
			ImageUrl = imageUrl;
			Content = content;
            FilePath = filePath;
            ChannelFilePathRule = channelFilePathRule;
            ContentFilePathRule = contentFilePathRule;
            LinkUrl = linkUrl;
            LinkType = ELinkTypeUtils.GetValue(linkType);
            ChannelTemplateId = channelTemplateId;
            ContentTemplateId = contentTemplateId;
            Keywords = keywords;
            Description = description;
            _extendValues = extendValues;
		}

        public ChannelInfo(ChannelInfo channelInfo)
        {
            Id = channelInfo.Id;
            ChannelName = channelInfo.ChannelName;
            SiteId = channelInfo.SiteId;
            ContentModelPluginId = channelInfo.ContentModelPluginId;
            ContentRelatedPluginIds = channelInfo.ContentRelatedPluginIds;
            ParentId = channelInfo.ParentId;
            ParentsPath = channelInfo.ParentsPath;
            ParentsCount = channelInfo.ParentsCount;
            ChildrenCount = channelInfo.ChildrenCount;
            IsLastNode = channelInfo.IsLastNode;
            IndexName = channelInfo.IndexName;
            GroupNameCollection = channelInfo.GroupNameCollection;
            Taxis = channelInfo.Taxis;
            AddDate = channelInfo.AddDate;
            ImageUrl = channelInfo.ImageUrl;
            Content = channelInfo.Content;
            FilePath = channelInfo.FilePath;
            ChannelFilePathRule = channelInfo.ChannelFilePathRule;
            ContentFilePathRule = channelInfo.ContentFilePathRule;
            LinkUrl = channelInfo.LinkUrl;
            LinkType = channelInfo.LinkType;
            ChannelTemplateId = channelInfo.ChannelTemplateId;
            ContentTemplateId = channelInfo.ContentTemplateId;
            Keywords = channelInfo.Keywords;
            Description = channelInfo.Description;
            _extendValues = channelInfo._extendValues;
        }

		public int Id { get; set; }

	    public string ChannelName { get; set; }

	    public int SiteId { get; set; }

        public string ContentModelPluginId { get; set; }

        public string ContentRelatedPluginIds { get; set; }

        public int ParentId { get; set; }

	    public string ParentsPath { get; set; }

	    public int ParentsCount { get; set; }

	    public int ChildrenCount { get; set; }

	    public bool IsLastNode { get; set; }

	    public string IndexName { get; set; }

	    public string GroupNameCollection { get; set; }

	    public int Taxis { get; set; }

	    public DateTime AddDate { get; set; }

	    public string ImageUrl { get; set; }

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

        public void SetExtendValues(string extendValues)
        {
            _extendValues = extendValues;
        }

        private ChannelInfoExtend _additional;

        [JsonIgnore]
        public ChannelInfoExtend Additional => _additional ?? (_additional = new ChannelInfoExtend(_extendValues));

        [JsonIgnore]
        public IAttributes Attributes => Additional;

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
    }
}
