using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerImageController : ControllerBase
    {
        private const string Route = "wx/layerImage";

        private readonly IAuthManager _authManager;
        private readonly IMaterialGroupRepository _materialGroupRepository;
        private readonly IMaterialImageRepository _materialImageRepository;

        public LayerImageController(IAuthManager authManager, IMaterialGroupRepository materialGroupRepository, IMaterialImageRepository materialImageRepository)
        {
            _authManager = authManager;
            _materialGroupRepository = materialGroupRepository;
            _materialImageRepository = materialImageRepository;
        }

        public class QueryRequest
        {
            public int SiteId { get; set; }
            public string Keyword { get; set; }
            public int GroupId { get; set; }
            public int Page { get; set; }
            public int PerPage { get; set; }
        }

        public class QueryResult
        {
            public IEnumerable<MaterialGroup> Groups { get; set; }
            public int Count { get; set; }
            public IEnumerable<MaterialImage> Images { get; set; }
        }
    }
}
