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
    public partial class FormListController : ControllerBase
    {
        private const string Route = "cms/forms/formList";
        private const string RouteUp = "cms/forms/formList/actions/up";
        private const string RouteDown = "cms/forms/formList/actions/down";
        private const string RouteExport = "cms/forms/formList/actions/export";
        private const string RouteImport = "cms/forms/formList/actions/import";
        private const string RouteDelete = "cms/forms/formList/actions/delete";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IFormManager _formManager;
        private readonly ISiteRepository _siteRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly IFormRepository _formRepository;
        private readonly IFormDataRepository _formDataRepository;

        public FormListController(ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, IFormManager formManager, ISiteRepository siteRepository, ITableStyleRepository tableStyleRepository, IFormRepository formRepository, IFormDataRepository formDataRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _formManager = formManager;
            _siteRepository = siteRepository;
            _tableStyleRepository = tableStyleRepository;
            _formRepository = formRepository;
            _formDataRepository = formDataRepository;
        }

        public class FormRequest : SiteRequest
        {
            public int FormId { get; set; }
        }

        public class GetResult
        {
            public List<Form> Forms { get; set; }
            public List<int> AuthFormIds { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public int FormId { get; set; }
        }

        public class TaxisRequest : SiteRequest
        {
            public int FormId { get; set; }
        }

        public class TaxisResult
        {
            public List<Form> Forms { get; set; }
        }
    }
}