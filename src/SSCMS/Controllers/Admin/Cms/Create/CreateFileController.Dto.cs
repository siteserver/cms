using System.Collections.Generic;
using SSCMS.Abstractions;
using SSCMS.Abstractions.Dto.Request;

namespace SSCMS.Controllers.Admin.Cms.Create
{
    public partial class CreateFileController
    {
        public class GetResult
        {
            public IEnumerable<Template> Templates { get; set; }
        }

        public class CreateRequest : SiteRequest
        {
            public IEnumerable<int> TemplateIds { get; set; }
        }
    }
}