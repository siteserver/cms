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
    public partial class FormStylesController : ControllerBase
    {
        private const string Route = "cms/forms/formStyles";
        private const string RouteImport = "cms/forms/formStyles/actions/import";
        private const string RouteExport = "cms/forms/formStyles/actions/export";
        private const string RouteDelete = "cms/forms/formStyles/actions/delete";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IFormRepository _formRepository;

        public FormStylesController(IAuthManager authManager, IPathManager pathManager, IFormRepository formRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _formRepository = formRepository;
        }

        public class FormRequest : SiteRequest
        {
            public int FormId { get; set; }
        }

        public class GetResult
        {
            public IEnumerable<Select<string>> InputTypes { get; set; }
            public string TableName { get; set; }
            public string RelatedIdentities { get; set; }
            public List<TableStyle> Styles { get; set; }
        }

        public class DeleteRequest : FormRequest
        {
            public string AttributeName { get; set; }
        }

        public class DeleteResult
        {
            public List<TableStyle> Styles { get; set; }
        }
    }
}
