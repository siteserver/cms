namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesReferenceController
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
