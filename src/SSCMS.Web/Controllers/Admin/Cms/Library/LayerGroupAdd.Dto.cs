using System.Collections.Generic;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Library
{
    public partial class LayerGroupAddController
    {
        public class GetRequest : SiteRequest
        {
            public int GroupId { get; set; }
        }

        public class CreateRequest : SiteRequest
        {
            public string GroupName { get; set; }
            public LibraryType LibraryType { get; set; }
        }

        public class CreateResult
        {
            public List<LibraryGroup> Groups { get; set; }
        }

        public class UpdateRequest : SiteRequest
        {
            public int GroupId { get; set; }
            public string GroupName { get; set; }
        }

        public class UpdateResult
        {
            public List<LibraryGroup> Groups { get; set; }
        }
    }
}
