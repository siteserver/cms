using System.Collections.Generic;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Common.TableStyle
{
    public partial class LayerValidateController
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
            public IEnumerable<InputStyleRule> Rules { get; set; }
        }

        public class SubmitRequest
        {
            public string TableName { get; set; }
            public string AttributeName { get; set; }
            public List<int> RelatedIdentities { get; set; }
            public List<InputStyleRule> Rules { get; set; }
        }
    }
}
