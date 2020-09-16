using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Create
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class CreateFileController : ControllerBase
    {
        private const string Route = "cms/create/createFile";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;
        private readonly ISiteRepository _siteRepository;
        private readonly ITemplateRepository _templateRepository;

        public CreateFileController(IAuthManager authManager, ICreateManager createManager, ISiteRepository siteRepository, ITemplateRepository templateRepository)
        {
            _authManager = authManager;
            _createManager = createManager;
            _siteRepository = siteRepository;
            _templateRepository = templateRepository;
        }

        public class GetResult
        {
            public IEnumerable<Template> Templates { get; set; }
        }

        public class CreateRequest : SiteRequest
        {
            public IEnumerable<int> TemplateIds { get; set; }
        }
    }
}
