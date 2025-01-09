using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ComponentEditorController : ControllerBase
    {
        private const string Route = "cms/material/componentEditor";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IConfigRepository _configRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IMaterialGroupRepository _materialGroupRepository;
        private readonly IMaterialComponentRepository _materialComponentRepository;

        public ComponentEditorController(
            ISettingsManager settingsManager,
            IAuthManager authManager,
            IConfigRepository configRepository,
            ISiteRepository siteRepository,
            IMaterialGroupRepository materialGroupRepository,
            IMaterialComponentRepository materialComponentRepository
        )
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _configRepository = configRepository;
            _siteRepository = siteRepository;
            _materialGroupRepository = materialGroupRepository;
            _materialComponentRepository = materialComponentRepository;
        }

        public class GetRequest : SiteRequest
        {
            public int ComponentId { get; set; }
        }

        public class GetResult
        {
            public MaterialComponent Component { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public int GroupId { get; set; }
            public int ComponentId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string ImageUrl { get; set; }
            public string Parameters { get; set; }
            public string Content { get; set; }
        }
    }
}
