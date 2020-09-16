using System.Collections.Generic;

namespace SSCMS.Web.Controllers.Admin.Settings.Utilities
{
    public partial class UtilitiesParametersController
    {
        public class GetResult
        {
            public List<KeyValuePair<string, string>> Environments { get; set; }
            public List<KeyValuePair<string, string>> Settings { get; set; }
        }
    }
}
