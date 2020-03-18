using System.Collections.Generic;
using SSCMS.Abstractions;
using SSCMS.Abstractions.Dto.Request;

namespace SSCMS.Controllers.Admin.Cms.Create
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