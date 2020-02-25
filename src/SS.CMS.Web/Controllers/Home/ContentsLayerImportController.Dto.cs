using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Home
{
    public partial class ContentsLayerImportController
    {
        public class GetResult
        {
            public int CheckedLevel { get; set; }
            public List<KeyValuePair<int, string>> CheckedLevels { get; set; }
        }

        public class UploadRequest : ChannelRequest
        {
            public IFormFile File { set; get; }
        }

        public class UploadResult
        {
            public string FileName { set; get; }
            public long Length { set; get; }
            public int Ret { set; get; }
        }

        public class SubmitRequest : ChannelRequest
        {
            public string ImportType { set; get; }
            public int CheckedLevel { set; get; }
            public bool IsOverride { set; get; }
            public List<string> FileNames { set; get; }
        }
    }
}
