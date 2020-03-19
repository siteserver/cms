using System.Collections.Generic;
using SSCMS;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Shared
{
    public partial class TableStyleLayerValidateController
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
