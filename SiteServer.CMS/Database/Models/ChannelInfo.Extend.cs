using Newtonsoft.Json;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core.Enumerations;
using SiteServer.CMS.Database.Repositories.Contents;

namespace SiteServer.CMS.Database.Models
{
    public partial class ChannelInfo
    {
        [JsonIgnore]
        public ContentTableRepository ContentRepository => ContentTableRepository.GetContentRepository(SiteId,
            ChannelManager.GetTableName(SiteManager.GetSiteInfo(SiteId), this));

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
