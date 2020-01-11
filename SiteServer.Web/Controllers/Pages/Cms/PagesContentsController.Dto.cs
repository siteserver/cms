using System.Collections.Generic;
using SiteServer.Abstractions;

namespace SiteServer.API.Controllers.Pages.Cms
{
    public partial class PagesContentsController
    {
        public class CascadeResult
        {
            
        }

        public class ListResult
        {
            public List<Content> PageContents { get; set; }
            public int Total { get; set; }
            public int PageSize { get; set; }
            public Permissions Permissions { get; set; }
            public List<ContentColumn> Columns { get; set; }
            public bool IsAllContents { get; set; }
        }

        public class Permissions
        {
            public bool IsAdd { get; set; }
            public bool IsDelete { get; set; }
            public bool IsEdit { get; set; }
            public bool IsArrange { get; set; }
            public bool IsTranslate { get; set; }
            public bool IsCheck { get; set; }
            public bool IsCreate { get; set; }
            public bool IsChannelEdit { get; set; }
        }
    }
}