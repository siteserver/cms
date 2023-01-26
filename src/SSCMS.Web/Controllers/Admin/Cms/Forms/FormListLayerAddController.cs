using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Forms
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class FormListLayerAddController : ControllerBase
    {
        private const string Route = "cms/forms/formListLayerAdd";
        private const string RouteUpdate = "cms/forms/formListLayerAdd/actions/update";

        private readonly IAuthManager _authManager;
        private readonly IFormRepository _formRepository;

        public FormListLayerAddController(IAuthManager authManager, IFormRepository formRepository)
        {
            _authManager = authManager;
            _formRepository = formRepository;
        }

        public class GetRequest : SiteRequest
        {
            public int FormId { get; set; }
        }

        public class GetResult
        {
            public Form Form { get; set; }
        }

        public class AddRequest : SiteRequest
        {
            public string Title { get; set; }
            public string Description { get; set; }
        }

        public class UpdateRequest : SiteRequest
        {
            public int FormId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
        }
    }
}
