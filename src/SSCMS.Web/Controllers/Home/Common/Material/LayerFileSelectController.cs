using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Home.Common.Material
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.User)]
    [Route(Constants.ApiHomePrefix)]
    public partial class LayerFileSelectController : ControllerBase
    {
        private const string Route = "common/material/layerFileSelect";
        private const string RouteSelect = "common/material/layerFileSelect/actions/select";

        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IConfigRepository _configRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IMaterialGroupRepository _materialGroupRepository;
        private readonly IMaterialFileRepository _materialFileRepository;

        public LayerFileSelectController(ISettingsManager settingsManager, IPathManager pathManager, IConfigRepository configRepository, ISiteRepository siteRepository, IMaterialGroupRepository materialGroupRepository, IMaterialFileRepository materialFileRepository)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _configRepository = configRepository;
            _siteRepository = siteRepository;
            _materialGroupRepository = materialGroupRepository;
            _materialFileRepository = materialFileRepository;
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
            public IEnumerable<MaterialFile> Items { get; set; }
        }

        public class SelectRequest : SiteRequest
        {
            public int MaterialId { get; set; }
        }
    }
}
