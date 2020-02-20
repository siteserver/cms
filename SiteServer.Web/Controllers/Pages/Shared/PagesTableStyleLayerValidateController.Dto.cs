using System.Collections.Generic;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto;

namespace SiteServer.API.Controllers.Pages.Shared
{
    public partial class PagesTableStyleLayerValidateController
    {
        public class GetRequest
        {
            public string TableName { get; set; }
            public string AttributeName { get; set; }
            public List<int> RelatedIdentities { get; set; }
        }

        public class GetResult
        {
            public IEnumerable<Select<string>> Options { get; set; }
            public IEnumerable<TableStyleRule> Rules { get; set; }
        }

        public class SubmitRequest
        {
            public string TableName { get; set; }
            public string AttributeName { get; set; }
            public List<int> RelatedIdentities { get; set; }
            public List<TableStyleRule> Rules { get; set; }
        }
    }
}
