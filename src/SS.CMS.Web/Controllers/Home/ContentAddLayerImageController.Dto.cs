using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Home
{
    public partial class ContentAddLayerImageController
    {
        public class GetResult
        {
            public Site Site { get; set; }
        }

        public class UploadRequest : ChannelRequest
        {
            public IFormFile File { set; get; }
        }

        public class UploadResult
        {
            public string Path { get; set; }
            public string Url { get; set; }
            public long ContentLength { get; set; }
        }

        public class SubmitRequest : ChannelRequest
        {
            public bool IsFix { get; set; }
            public string FixWidth { get; set; }
            public string FixHeight { get; set; }
            public bool IsEditor { get; set; }
            public bool EditorIsFix { get; set; }
            public string EditorFixWidth { get; set; }
            public string EditorFixHeight { get; set; }
            public bool EditorIsLinkToOriginal { get; set; }
            public List<string> FilePaths { get; set; }
        }

        public class SubmitResult
        {
            public List<string> Value { get; set; }
            public List<object> Editors { get; set; }
        }
    }
}
