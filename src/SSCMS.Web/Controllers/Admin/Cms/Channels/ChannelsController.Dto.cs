using System.Collections.Generic;
using Datory;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsController
    {
        public class ChannelsResult
        {
            public Cascade<int> Channel { get; set; }
            public IEnumerable<string> IndexNames { get; set; }
            public IEnumerable<string> GroupNames { get; set; }
            public IEnumerable<Template> ChannelTemplates { get; set; }
            public IEnumerable<Template> ContentTemplates { get; set; }
            public IEnumerable<IPackageMetadata> ContentPlugins { get; set; }
            public IEnumerable<IPackageMetadata> RelatedPlugins { get; set; }
        }

        public class ChannelResult
        {
            public Channel Channel { get; set; }
            public IEnumerable<Select<string>> LinkTypes { get; set; }
            public IEnumerable<Select<string>> TaxisTypes { get; set; }
            public IEnumerable<InputStyle> Styles { get; set; }
        }

        public class ImportRequest : ChannelRequest
        {
            public string FileName { get; set; }
            public bool IsOverride { get; set; }
        }

        public class OrderRequest : ChannelRequest
        {
            public int ParentId { get; set; }
            public int Taxis { get; set; }
            public bool IsUp { get; set; }
        }

        public class ChannelIdsRequest : SiteRequest
        {
            public List<int> ChannelIds { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public int ChannelId { get; set; }
            public string ChannelName { get; set; }
            public bool DeleteFiles { get; set; }
        }

        public class PutRequest : Entity
        {
            public int SiteId { get; set; }
            public string ChannelName { get; set; }
            public string IndexName { get; set; }
            public List<string> GroupNames { get; set; }
            public string ImageUrl { get; set; }
            public string Content { get; set; }
            public int ChannelTemplateId { get; set; }
            public int ContentTemplateId { get; set; }
            public string ContentModelPluginId { get; set; }
            public List<string> ContentRelatedPluginIdList { get; set; }
            public string LinkUrl { get; set; }
            public LinkType LinkType { get; set; }
            public TaxisType DefaultTaxisType { get; set; }
            public string FilePath { get; set; }
            public string ChannelFilePathRule { get; set; }
            public string ContentFilePathRule { get; set; }
            public string Keywords { get; set; }
            public string Description { get; set; }
        }

        public class AppendRequest : SiteRequest
        {
            public int ParentId { get; set; }
            public int ChannelTemplateId { get; set; }
            public int ContentTemplateId { get; set; }
            public bool IsParentTemplates { get; set; }
            public bool IsIndexName { get; set; }
            public string Channels { get; set; }
        }
    }
}
