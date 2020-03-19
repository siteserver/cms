using System.Collections.Generic;

namespace SSCMS.Controllers.Admin.Plugins
{
    public partial class AddLayerUploadController
    {
        public class UploadResult
        {
            public string FileName { set; get; }
            public long Length { set; get; }
            public int Ret { set; get; }
        }

        public class SubmitRequest
        {
            public List<string> FileNames { set; get; }
        }
    }
}
