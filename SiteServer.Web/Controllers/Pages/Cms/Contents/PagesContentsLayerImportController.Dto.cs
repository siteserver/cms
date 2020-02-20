using System.Collections.Generic;
using SiteServer.Abstractions.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    public partial class PagesContentsLayerImportController
    {
        public class GetResult
        {
            public int Value { get; set; }
            public List<KeyValuePair<int, string>> CheckedLevels { get; set; }
        }

        public class UploadResult
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }

        public class SubmitRequest : ChannelRequest
        {
            public string ImportType { get; set; }
            public int CheckedLevel { get; set; }
            public bool IsOverride { get; set; }
            public List<string> FileNames { get; set; }
        }
    }
}