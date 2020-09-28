using System;
using System.Collections.Generic;
using Datory;
using Datory.Annotations;
using SSCMS.Enums;

namespace SSCMS.Models
{
    [DataTable("siteserver_Channel")]
    public class Channel : Entity, IChannelSummary
    {
        [DataColumn]
        public string ChannelName { get; set; }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string ContentModelPluginId { get; set; }

        [DataColumn]
        public string TableName { get; set; }

        [DataColumn]
        public int ParentId { get; set; }

        [DataColumn]
        public List<int> ParentsPath { get; set; }

        [DataColumn]
        public int ParentsCount { get; set; }

        [DataColumn]
        public int ChildrenCount { get; set; }

        [DataColumn]
        public string IndexName { get; set; }

        [DataColumn]
        public List<string> GroupNames { get; set; }

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

        public IList<Channel> Children { get; set; }

        public bool IsCreateChannelIfContentChanged { get; set; } = true;

        public string CreateChannelIdsIfContentChanged { get; set; }

        public string ListColumns { get; set; }

        public bool IsPreviewContentsExists { get; set; }

        public TaxisType DefaultTaxisType { get; set; } = TaxisType.OrderByTaxisDesc;

        //显示下级栏目内容
        public bool IsAllContents { get; set; }
    }
}