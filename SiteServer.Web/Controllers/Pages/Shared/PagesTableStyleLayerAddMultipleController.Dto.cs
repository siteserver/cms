using System.Collections.Generic;
using SiteServer.Abstractions;

namespace SiteServer.API.Controllers.Pages.Shared
{
    public partial class PagesTableStyleLayerAddMultipleController
    {

        public class GetResult
        {
            public IEnumerable<KeyValuePair<InputType, string>> InputTypes { get; set; }
            public List<Style> Styles { get; set; }
        }

        public class SubmitRequest
        {
            public string TableName { get; set; }
            public List<int> RelatedIdentities { get; set; }
            public List<Style> Styles { get; set; }
        }

        public class Style
        {
            public string AttributeName { get; set; }
            public string DisplayName { get; set; }
            public InputType InputType { get; set; }
        }
    }
}
