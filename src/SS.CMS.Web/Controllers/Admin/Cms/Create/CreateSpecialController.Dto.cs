using System.Collections.Generic;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Admin.Cms.Create
{
    public partial class CreateSpecialController
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