using System.Collections.Generic;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsStyleRelatedFieldController
    {
        public class ItemsRequest
        {
            public int SiteId { get; set; }
            public int RelatedFieldId { get; set; }
        }

        public class ItemsResult
        {
            public List<Cascade<int>> Tree { get; set; }
        }

        public class ItemsAddRequest : SiteRequest
        {
            public int RelatedFieldId { get; set; }
            public int ParentId { get; set; }
            public List<KeyValuePair<string, string>> Items { get; set; }
        }

        public class ItemsEditRequest : SiteRequest
        {
            public int RelatedFieldId { get; set; }
            public int Id { get; set; }
            public string Label { get; set; }
            public string Value { get; set; }
        }

        public class ItemsOrderRequest : SiteRequest
        {
            public int RelatedFieldId { get; set; }
            public int Id { get; set; }
            public bool Up { get; set; }
        }

        public class ItemsDeleteRequest: SiteRequest
        {
            public int RelatedFieldId { get; set; }
            public int Id { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public int RelatedFieldId { get; set; }
        }
    }
}
