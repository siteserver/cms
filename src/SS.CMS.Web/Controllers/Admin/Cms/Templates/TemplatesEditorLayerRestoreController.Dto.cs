using System.Collections.Generic;

namespace SS.CMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesEditorLayerRestoreController
    {
        public class GetResult
        {
            public IEnumerable<KeyValuePair<int, string>> Logs { get; set; }
            public int LogId { get; set; }
			public string Original { get; set; }
            public string Modified { get; set; }
        }

        public class TemplateRequest
        {
            public int SiteId { get; set; }
            public int TemplateId { get; set; }
			public int LogId { get; set; }
        }
    }
}
