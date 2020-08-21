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
    public partial class LayerFileSelectController : ControllerBase
    {
        private const string Route = "common/material/layerFileSelect";
        private const string RouteSelect = "common/material/layerFileSelect/actions/select";

        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IMaterialGroupRepository _materialGroupRepository;
        private readonly IMaterialFileRepository _materialFileRepository;
        private readonly ISiteRepository _siteRepository;

        public LayerFileSelectController(ISettingsManager settingsManager, IPathManager pathManager, IMaterialGroupRepository materialGroupRepository, IMaterialFileRepository materialFileRepository, ISiteRepository siteRepository)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _materialGroupRepository = materialGroupRepository;
            _materialFileRepository = materialFileRepository;
            _siteRepository = siteRepository;
        }

        public class QueryRequest
        {
            public string Keyword { get; set; }
            public int GroupId { get; set; }
            public int Page { get; set; }
            public int PerPage { get; set; }
        }

        public class QueryResult
        {
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
