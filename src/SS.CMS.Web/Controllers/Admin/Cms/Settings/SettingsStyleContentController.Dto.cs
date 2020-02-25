using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto;
using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsStyleContentController
    {
        public class GetResult
        {
            public List<Style> Styles { get; set; }
            public string TableName { get; set; }
            public List<int> RelatedIdentities { get; set; }
            public Cascade<int> Channels { get; set; }
        }

        public class Style
        {
            public int Id { get; set; }
            public string AttributeName { get; set; }
            public string DisplayName { get; set; }
            public string InputType { get; set; }
            public IEnumerable<TableStyleRule> Rules { get; set; }
            public int Taxis { get; set; }
            public bool IsSystem { get; set; }
        }

        public class DeleteRequest
        {
            public int SiteId { get; set; }
            public int ChannelId { get; set; }
            public string AttributeName { get; set; }
        }

        public class ImportRequest : SiteRequest
        {
            public int ChannelId { get; set; }
            public IFormFile File { get; set; }
        }
    }
}
