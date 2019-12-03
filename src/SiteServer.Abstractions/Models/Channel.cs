using System;
using System.Collections.Generic;
using Datory;
using Datory.Annotations;

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
        public string ContentRelatedPluginIds { get; set; }

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
        public string IsLastNode { get; set; }

        public bool LastNode
        {
            get => TranslateUtils.ToBool(IsLastNode);
            set => IsLastNode = value.ToString();
        }

        [DataColumn]
        public string IndexName { get; set; }

        [DataColumn]
        public string GroupNameCollection { get; set; }

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
        public string LinkType { get; set; }

        [DataColumn]
        public int ChannelTemplateId { get; set; }

        [DataColumn]
        public int ContentTemplateId { get; set; }

        [DataColumn]
        public string Keywords { get; set; }

        [DataColumn]
        public string Description { get; set; }

        [DataColumn(Text = true)]
        public string ExtendValues { get; set; }

        public IList<Channel> Children { get; set; }

        //是否可以添加栏目
        public bool IsChannelAddable { get; set; }

        //是否可以添加内容
        public bool IsContentAddable { get; set; }

        //是否可以生成栏目
        public bool IsChannelCreatable { get; set; }

        //是否可以生成内容
        public bool IsContentCreatable { get; set; }

        public bool IsCreateChannelIfContentChanged { get; set; }

        public string CreateChannelIdsIfContentChanged { get; set; }

        public string ContentAttributesOfDisplay { get; set; }

        public ECrossSiteTransType TransType { get; set; }

        public int TransSiteId { get; set; }

        public string TransChannelIds { get; set; }

        public string TransChannelNames { get; set; }

        public bool TransIsAutomatic { get; set; }

        //跨站转发操作类型：复制 引用地址 引用内容
        public ETranslateContentType TransDoneType { get; set; }

        public bool IsPreviewContentsExists { get; set; }

        public string DefaultTaxisType { get; set; } = ETaxisTypeUtils.GetValue(ETaxisType.OrderByTaxisDesc);

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