using System.Collections.Generic;
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
    public partial class FormTemplatesController : ControllerBase
    {
        private const string Route = "cms/forms/formTemplates";
        private const string RouteDelete = "cms/forms/formTemplates/actions/delete";
        private const string RouteExport = "cms/forms/formTemplates/actions/export";
        private const string RouteImport = "cms/forms/formTemplates/actions/import";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IFormManager _formManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IFormRepository _formRepository;

        public FormTemplatesController(IAuthManager authManager, IPathManager pathManager, IFormManager formManager, ISiteRepository siteRepository, IFormRepository formRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _formManager = formManager;
            _siteRepository = siteRepository;
            _formRepository = formRepository;
        }

        public class GetResult
        {
            public List<Form> Forms { get; set; }
            public List<FormTemplate> Templates { get; set; }
            public string SiteUrl { get; set; }
            public string SiteDir { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public string Name { get; set; }
        }

        public class ExportRequest : SiteRequest
        {
            public bool IsSystem { get; set; }
            public string Name { get; set; }
        }
    }
}
