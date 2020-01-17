namespace SiteServer.API.Controllers.Pages.Cms
{
    public partial class PagesTemplateAssetsEditorController
    {
        public class GetResult
        {
            public string Path { get; set; }
            public string ExtName { get; set; }
            public string Content { get; set; }
        }

        public class FileRequest
        {
            public int SiteId { get; set; }
            public string DirectoryPath { get; set; }
			public string FileName { get; set; }
        }

        public class ContentRequest
        {
            public int SiteId { get; set; }
            public string Path { get; set; }
            public string ExtName { get; set; }
            public string Content { get; set; }
            public string DirectoryPath { get; set; }
            public string FileName { get; set; }
        }

        public class ContentResult
        {
            public string DirectoryPath { get; set; }
            public string FileName { get; set; }
        }
    }
}
