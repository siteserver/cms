using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;

namespace SSCMS.Web.Controllers.Admin.Common.Editor
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerComponentController : ControllerBase
    {
        private const string Route = "common/editor/layerComponent";

        private readonly IConfigRepository _configRepository;
        private readonly IMaterialGroupRepository _materialGroupRepository;
        private readonly IMaterialComponentRepository _materialComponentRepository;

        public LayerComponentController(
            IConfigRepository configRepository,
            IMaterialGroupRepository materialGroupRepository,
            IMaterialComponentRepository materialComponentRepository
        )
        {
            _configRepository = configRepository;
            _materialGroupRepository = materialGroupRepository;
            _materialComponentRepository = materialComponentRepository;
        }

        public class GetRequest : SiteRequest
        {
            public string Keyword { get; set; }
            public int GroupId { get; set; }
            public int Page { get; set; }
            public int PerPage { get; set; }
        }

        public class GetResult
        {
            public bool IsSiteOnly { get; set; }
            public IEnumerable<MaterialGroup> Groups { get; set; }
            public int Count { get; set; }
            public IEnumerable<MaterialComponent> Items { get; set; }
        }

        public class GroupRequest
        {
            public int SiteId { get; set; }
            public string Name { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public int ComponentId { get; set; }
            public List<KeyValuePair<string, string>> Parameters { get; set; }
        }
    }
}
