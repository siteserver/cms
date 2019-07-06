using System;
using Newtonsoft.Json;
using SS.CMS.Data;
using SS.CMS.Repositories.IContentRepository;

namespace SS.CMS.Models
{
    [Serializable]
    [Table("siteserver_Channel")]
    public class ChannelInfo : Entity, ICloneable
    {
        [TableColumn]
        public string ChannelName { get; set; }

        [TableColumn]
        public int SiteId { get; set; }

        [TableColumn]
        public string ContentModelPluginId { get; set; }

        [TableColumn(Text = true)]
        public string ContentRelatedPluginIds { get; set; }

        [TableColumn]
        public int ParentId { get; set; }

        [TableColumn]
        public string ParentsPath { get; set; }

        [TableColumn]
        public int ParentsCount { get; set; }

        [TableColumn]
        public int ChildrenCount { get; set; }

        [TableColumn]
        public bool IsLastNode { get; set; }

        [TableColumn]
        public string IndexName { get; set; }

        [TableColumn(Text = true)]
        public string GroupNameCollection { get; set; }

        [TableColumn]
        public int Taxis { get; set; }

        [TableColumn(Length = 1000)]
        public string ImageUrl { get; set; }

        [TableColumn(Text = true)]
        public string Content { get; set; }

        [TableColumn(Length = 1000)]
        public string FilePath { get; set; }

        [TableColumn(Length = 1000)]
        public string ChannelFilePathRule { get; set; }

        [TableColumn(Length = 1000)]
        public string ContentFilePathRule { get; set; }

        [TableColumn(Length = 1000)]
        public string LinkUrl { get; set; }

        [TableColumn]
        public string LinkType { get; set; }

        [TableColumn]
        public int ChannelTemplateId { get; set; }

        [TableColumn]
        public int ContentTemplateId { get; set; }

        [TableColumn(Length = 2000)]
        public string Keywords { get; set; }

        [TableColumn(Length = 2000)]
        public string Description { get; set; }

        [TableColumn(Text = true, Extend = true)]
        public string ExtendValues { get; set; }

        public object Clone()
        {
            return (ChannelInfo)MemberwiseClone();
        }

        [JsonIgnore]
        public IContentRepository ContentRepository { get; set; }

        //是否可以添加栏目
        public bool IsChannelAddable { get; set; } = true;

        //是否可以添加内容
        public bool IsContentAddable { get; set; } = true;

        //是否可以生成栏目
        public bool IsChannelCreatable { get; set; } = true;

        //是否可以生成内容
        public bool IsContentCreatable { get; set; } = true;

        public bool IsCreateChannelIfContentChanged { get; set; } = true;

        public string CreateChannelIdsIfContentChanged { get; set; }

        public string ContentAttributesOfDisplay { get; set; }

        public string TransType { get; set; } = "AllSite";

        public int TransSiteId { get; set; }

        public string TransChannelIds { get; set; }

        public string TransChannelNames { get; set; }

        public bool IsTransAutomatic { get; set; }

        //跨站转发操作类型：复制 引用地址 引用内容
        public string TransDoneType { get; set; } = "Copy";

        public bool IsPreviewContentsExists { get; set; }

        public string DefaultTaxisType { get; set; } = "OrderByTaxisDesc";
    }
}
