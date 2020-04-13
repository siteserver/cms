using System.Collections.Generic;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Home
{
    public partial class ContentsLayerImportController
    {
        public class GetResult
        {
            public int CheckedLevel { get; set; }
            public List<KeyValuePair<int, string>> CheckedLevels { get; set; }
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
