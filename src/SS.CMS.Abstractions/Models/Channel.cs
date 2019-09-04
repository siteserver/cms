using System;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Serializable]
    [DataTable("siteserver_Channel")]
    public partial class Channel : Entity, ICloneable
    {
        [DataColumn]
        public string ChannelName { get; set; }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string TableName { get; set; }

        [DataColumn]
        public string ContentModelPluginId { get; set; }

        [DataColumn(Text = true)]
        public string ContentRelatedPluginIds { get; set; }

        [DataColumn]
        public int ParentId { get; set; }

        [DataColumn]
        public string ParentsPath { get; set; }

        [DataColumn]
        public string IndexName { get; set; }

        [DataColumn(Text = true)]
        public string GroupNameCollection { get; set; }

        [DataColumn]
        public int Taxis { get; set; }

        [DataColumn(Length = 1000)]
        public string ImageUrl { get; set; }

        [DataColumn(Text = true)]
        public string Content { get; set; }

        [DataColumn(Length = 1000)]
        public string FilePath { get; set; }

        [DataColumn(Length = 1000)]
        public string ChannelFilePathRule { get; set; }

        [DataColumn(Length = 1000)]
        public string ContentFilePathRule { get; set; }

        [DataColumn(Length = 1000)]
        public string LinkUrl { get; set; }

        [DataColumn]
        public string LinkType { get; set; }

        [DataColumn]
        public int ChannelTemplateId { get; set; }

        [DataColumn]
        public int ContentTemplateId { get; set; }

        [DataColumn(Length = 2000)]
        public string Keywords { get; set; }

        [DataColumn(Length = 2000)]
        public string Description { get; set; }

        [DataColumn(Text = true, Extend = true)]
        public string ExtendValues { get; set; }

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
