using System.Collections.Generic;
using SSCMS.Dto;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Create
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