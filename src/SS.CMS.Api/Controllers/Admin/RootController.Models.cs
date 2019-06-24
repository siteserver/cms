using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using SS.CMS.Core.Common;
using SS.CMS.Utils;

namespace SS.CMS.Api.Controllers.Admin
{
    public partial class RootController
    {
        public class ValidateResult
        {
            public bool IsInstalled { get; set; }
            public bool IsAdministrator { get; set; }
            public bool IsLockedOut { get; set; }
            public string DatabaseVersion { get; set; }
            public string ProductVersion { get; set; }
            public int SiteId { get; set; }
            public bool RedirectToCreateSite { get; set; }
            public bool RedirectToErrorSite { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}