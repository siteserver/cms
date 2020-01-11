namespace SiteServer.API.Controllers.Pages.Cms
{
    public partial class PagesTemplateReferenceController
    {
        public class Element
        {
            public string Name { get; set; }
            public string ElementName { get; set; }
            public string Title { get; set; }
        }

        public class Field
        {
            public string Name { get; set; }
            public string Title { get; set; }
        }

        public class FieldsRequest
        {
            public int SiteId { get; set; }
            public string ElementName { get; set; }
        }
    }
}
