using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace SS.CMS.Web.Controllers.Admin.Plugins
{
    public partial class AddLayerUploadController
    {
        public class UploadRequest
        {
            public IFormFile File { set; get; }
        }

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
