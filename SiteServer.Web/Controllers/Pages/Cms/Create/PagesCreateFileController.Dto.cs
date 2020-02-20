using System.Collections.Generic;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Create
{
    public partial class PagesCreateFileController
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