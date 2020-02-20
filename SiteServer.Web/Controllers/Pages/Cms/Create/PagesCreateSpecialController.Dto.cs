using System.Collections.Generic;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Create
{
    public partial class PagesCreateSpecialController
    {
        public class GetResult
        {
            public IEnumerable<Special> Specials { get; set; }
        }

        public class CreateRequest : SiteRequest
        {
            public IEnumerable<int> SpecialIds { get; set; }
        }
    }
}