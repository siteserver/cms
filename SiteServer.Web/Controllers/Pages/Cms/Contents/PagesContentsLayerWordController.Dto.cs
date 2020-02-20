using System.Collections.Generic;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    public partial class PagesContentsLayerWordController
    {
        public class GetResult
        {
            public List<KeyValuePair<int, string>> Value { get; set; }
            public int CheckedLevel { get; set; }
        }

        public class UploadResult
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }

        public class SubmitRequest : ChannelRequest
        {
            public bool IsFirstLineTitle { get; set; }
            public bool IsClearFormat { get; set; }
            public bool IsFirstLineIndent { get; set; }
            public bool IsClearFontSize { get; set; }
            public bool IsClearFontFamily { get; set; }
            public bool IsClearImages { get; set; }
            public int CheckedLevel { get; set; }
            public List<string> FileNames { get; set; }
        }
    }
}