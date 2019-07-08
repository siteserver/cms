namespace SS.CMS.Api.Controllers.Sites
{
    public partial class SitesController
    {
        public class CreateRequest
        {
            public string CreateType { get; set; }
            public string CreateTemplateId { get; set; }
            public string SiteName { get; set; }
            public bool IsRoot { get; set; }
            public int ParentId { get; set; }
            public string SiteDir { get; set; }
            public string TableRule { get; set; }
            public string TableChoose { get; set; }
            public string TableHandWrite { get; set; }
            public bool IsImportContents { get; set; }
            public bool IsImportTableStyles { get; set; }
        }
    }
}
