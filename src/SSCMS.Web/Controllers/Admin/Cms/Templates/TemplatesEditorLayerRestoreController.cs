using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class TemplatesEditorLayerRestoreController : ControllerBase
    {
        private const string Route = "cms/templates/templatesEditorLayerRestore";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly ITemplateLogRepository _templateLogRepository;

        public TemplatesEditorLayerRestoreController(IAuthManager authManager, IPathManager pathManager, ISiteRepository siteRepository, ITemplateRepository templateRepository, ITemplateLogRepository templateLogRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _templateRepository = templateRepository;
            _templateLogRepository = templateLogRepository;
        }

        public class GetResult
        {
            public IEnumerable<KeyValuePair<int, string>> Logs { get; set; }
            public int LogId { get; set; }
            public string Original { get; set; }
            public string Modified { get; set; }
        }

        public class TemplateRequest
        {
            public int SiteId { get; set; }
            public int TemplateId { get; set; }
            public int LogId { get; set; }
        }
    }
}
