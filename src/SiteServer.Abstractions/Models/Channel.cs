using System;
using System.Collections.Generic;
using Datory;
using Datory.Annotations;
using Newtonsoft.Json;

namespace SiteServer.Abstractions
{
    [DataTable("siteserver_Channel")]
    public class Channel : Entity
    {
        [DataColumn]
        public string ChannelName { get; set; }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string ContentModelPluginId { get; set; }

        [DataColumn]
        [JsonIgnore]
        private string ContentRelatedPluginIds { get; set; }

        [DataIgnore]
        public List<string> ContentRelatedPluginIdList
        {
            get => StringUtils.GetStringList(ContentRelatedPluginIds);
            set => ContentRelatedPluginIds = StringUtils.Join(value);
        }

        [DataColumn]
        public int ParentId { get; set; }

        [DataColumn]
        public string ParentsPath { get; set; }

        [DataColumn]
        public int ParentsCount { get; set; }

        [DataColumn]
        public int ChildrenCount { get; set; }

        [DataColumn]
        public string IndexName { get; set; }

        [DataColumn]
        [JsonIgnore]
        private string GroupNameCollection { get; set; }

        [DataIgnore]
        public List<string> GroupNames
        {
            get => StringUtils.GetStringList(GroupNameCollection);
            set => GroupNameCollection = StringUtils.Join(value);
        }

        [DataColumn]
        public int Taxis { get; set; }

        [DataColumn]
        public DateTime? AddDate { get; set; }

        [DataColumn]
        public string ImageUrl { get; set; }

        [DataColumn(Text = true)]
        public string Content { get; set; }

        [DataColumn]
        public string FilePath { get; set; }

        [DataColumn]
        public string ChannelFilePathRule { get; set; }

        [DataColumn]
        public string ContentFilePathRule { get; set; }

        [DataColumn]
        public string LinkUrl { get; set; }

        [DataColumn]
        public LinkType LinkType { get; set; }

        [DataColumn]
        public int ChannelTemplateId { get; set; }

        [DataColumn]
        public int ContentTemplateId { get; set; }

        [DataColumn]
        public string Keywords { get; set; }

        [DataColumn]
        public string Description { get; set; }

        [DataColumn(Text = true, Extend = true)]
        public string ExtendValues { get; set; }

        public IList<Channel> Children { get; set; }

        public bool IsCreateChannelIfContentChanged { get; set; } = true;

        public string CreateChannelIdsIfContentChanged { get; set; }

        public string ContentAttributesOfDisplay { get; set; }

        public TransType TransType { get; set; }

        public int TransSiteId { get; set; }

        public string TransChannelIds { get; set; }

        public string TransChannelNames { get; set; }

        public bool TransIsAutomatic { get; set; }

        //跨站转发操作类型：复制 引用地址 引用内容
        public TranslateContentType TransDoneType { get; set; }

        public bool IsPreviewContentsExists { get; set; }

        public TaxisType DefaultTaxisType { get; set; } = TaxisType.OrderByTaxisDesc;

        //显示下级栏目内容
        public bool IsAllContents { get; set; }

        //只显示自己添加的内容
        public bool IsSelfOnly { get; set; }

        public Channel Clone()
        {
            return (Channel) MemberwiseClone();
        }
    }
}