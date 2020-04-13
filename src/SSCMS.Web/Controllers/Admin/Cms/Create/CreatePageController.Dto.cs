using System.Collections.Generic;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Create
{
    public partial class CreatePageController
    {
        public class GetRequest : SiteRequest
        {
            public CreateType Type { get; set; }
            public int ParentId { get; set; }
        }

        public class GetResult
        {
            public Cascade<int> Channels { get; set; }
            public IEnumerable<int> AllChannelIds { get; set; }
            public IEnumerable<Template> ChannelTemplates { get; set; }
            public IEnumerable<Template> ContentTemplates { get; set; }
        }

        public class CreateRequest : SiteRequest
        {
            public CreateType Type { get; set; }
            public IEnumerable<int> ChannelIdList { get; set; }
            public bool IsAllChecked { get; set; }
            public bool IsDescendent { get; set; }
            public bool IsChannelPage { get; set; }
            public bool IsContentPage { get; set; }
            public string Scope { get; set; }
        }
    }
}