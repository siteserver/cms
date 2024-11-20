using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Common.Material
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerImageSelectController : ControllerBase
    {
        private const string Route = "common/material/layerImageSelect";
        private const string RouteSelect = "common/material/layerImageSelect/actions/select";

        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IConfigRepository _configRepository;
        private readonly IMaterialGroupRepository _materialGroupRepository;
        private readonly IMaterialImageRepository _materialImageRepository;
        private readonly ISiteRepository _siteRepository;

        public LayerImageSelectController(
            ISettingsManager settingsManager,
            IPathManager pathManager,
            IConfigRepository configRepository,
            IMaterialGroupRepository materialGroupRepository,
            IMaterialImageRepository materialImageRepository,
            ISiteRepository siteRepository
        )
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _configRepository = configRepository;
            _materialGroupRepository = materialGroupRepository;
            _materialImageRepository = materialImageRepository;
            _siteRepository = siteRepository;
        }

        public class QueryRequest : SiteRequest
        {
            public string Keyword { get; set; }
            public int GroupId { get; set; }
            public int Page { get; set; }
            public int PerPage { get; set; }
        }

        public class QueryResult
        {
            public bool IsSiteOnly { get; set; }
            public IEnumerable<MaterialGroup> Groups { get; set; }
            public int Count { get; set; }
            public IEnumerable<MaterialImage> Items { get; set; }
        }

        public class SelectRequest : SiteRequest
        {
            public int MaterialId { get; set; }
        }

        public class SelectResult
        {
            public string VirtualUrl { get; set; }
            public string ImageUrl { get; set; }
        }
    }
}
