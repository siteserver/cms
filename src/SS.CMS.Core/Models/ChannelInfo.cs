using System;
using Newtonsoft.Json;
using SS.CMS.Abstractions;
using SS.CMS.Core.Models.Enumerations;
using SS.CMS.Core.Repositories;
using SS.CMS.Data;

namespace SS.CMS.Core.Models
{
    [Table("siteserver_Channel")]
    public partial class ChannelInfo : Entity, IChannelInfo, ICloneable
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
        private string IsLastNode { get; set; }

        public bool LastNode
        {
            get => IsLastNode == "True";
            set => IsLastNode = value.ToString();
        }

        [TableColumn]
        public string IndexName { get; set; }

        [TableColumn(Text = true)]
        public string GroupNameCollection { get; set; }

        [TableColumn]
        public int Taxis { get; set; }

        [TableColumn]
        public DateTime? AddDate { get; set; }

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
        public ContentDao ContentDao => ContentDao.Instance(Settings.AppContext.Db, this);

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

        public string TransType { get; set; } = ECrossSiteTransTypeUtils.GetValue(ECrossSiteTransType.AllSite);

        public int TransSiteId { get; set; }

        public string TransChannelIds { get; set; }

        public string TransChannelNames { get; set; }

        public bool TransIsAutomatic { get; set; }

        //跨站转发操作类型：复制 引用地址 引用内容
        public string TransDoneType { get; set; } = ETranslateContentTypeUtils.GetValue(ETranslateContentType.Copy);

        public bool IsPreviewContentsExists { get; set; }

        public string DefaultTaxisType { get; set; } = ETaxisTypeUtils.GetValue(ETaxisType.OrderByTaxisDesc);
    }
}
