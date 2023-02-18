using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Forms
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class FormTemplatesLayerEditController : ControllerBase
    {
        private const string Route = "cms/forms/formTemplatesLayerEdit";
        private const string RouteUpdate = "cms/forms/formTemplatesLayerEdit/actions/update";
        private const string RouteClone = "cms/forms/formTemplatesLayerEdit/actions/clone";

        private readonly IAuthManager _authManager;
        private readonly IFormManager _formManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IFormRepository _formRepository;

        public FormTemplatesLayerEditController(IAuthManager authManager, IFormManager formManager, ISiteRepository siteRepository, IFormRepository formRepository)
        {
            _authManager = authManager;
            _formManager = formManager;
            _siteRepository = siteRepository;
            _formRepository = formRepository;
        }

        public class GetRequest : SiteRequest
        {
            public string Name { get; set; }
        }

        public class GetResult
        {
            public FormTemplate Template { get; set; }
        }

        public class CloneRequest : SiteRequest
        {
            public bool IsSystemOriginal { get; set; }
            public string NameOriginal { get; set; }
            public string Name { get; set; }
            public bool IsHtml { get; set; }
            public string TemplateHtml { get; set; }
        }

        public class UpdateRequest : SiteRequest
        {
            public bool IsSystemOriginal { get; set; }
            public string NameOriginal { get; set; }
            public string Name { get; set; }
        }
    }
}
