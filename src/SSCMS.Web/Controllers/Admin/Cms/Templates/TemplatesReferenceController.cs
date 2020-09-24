using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class TemplatesReferenceController : ControllerBase
    {
        private const string Route = "cms/templates/templatesReference";

        private readonly IAuthManager _authManager;

        public TemplatesReferenceController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

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

        public class ListAttribute
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }

        public class ListTag
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public List<ListAttribute> Attributes { get; set; }
            public List<ListReference> References { get; set; }
        }

        public class ListReference
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }

        public class ListResult
        {
            public double Version { get; set; }
            public List<ListTag> Tags { get; set; }
        }
    }
}
