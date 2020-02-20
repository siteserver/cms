using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Templates
{
    public partial class PagesSpecialController
    {
        public class GetSpecialResult
        {
            public Special Special { get; set; }
            public string Guid { get; set; }
        }

        public class UploadRequest : SiteRequest
        {
            public string Guid { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public int Id { get; set; }
            public string Guid { get; set; }
            public string Title { get; set; }
            public string Url { get; set; }
            public IEnumerable<string> FileNames { get; set; }
            public bool IsEditOnly { get; set; }
            public bool IsUploadOnly { get; set; }
        }
    }
}