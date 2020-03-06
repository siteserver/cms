using System.Collections.Generic;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;

namespace SS.CMS.Web.Controllers.Home
{
    public partial class ContentsController
    {
        public class ListRequest : ChannelRequest
        {
            public int Page { get; set; }
        }

        public class Permissions
        {
            public bool IsAdd { get; set; }
            public bool IsDelete { get; set; }
            public bool IsEdit { get; set; }
            public bool IsTranslate { get; set; }
            public bool IsCheck { get; set; }
            public bool IsCreate { get; set; }
            public bool IsChannelEdit { get; set; }
        }

        public class ListResult
        {
            public List<Content> Contents { get; set; }
            public int Count { get; set; }
            public int Pages { get; set; }
            public Permissions Permissions { get; set; }
            public List<ContentColumn> Columns { get; set; }
        }
    }
}
