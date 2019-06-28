using System;

namespace SS.CMS.Api.Controllers.Admin
{
    public partial class IndexController
    {
        public class GetModel
        {
            public string Version { get; set; }
            public DateTime? LastActivityDate { get; set; }
            public DateTime? UpdateDate { get; set; }
        }
    }
}
