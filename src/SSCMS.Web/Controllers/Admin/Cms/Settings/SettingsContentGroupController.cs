using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsContentGroupController : ControllerBase
    {
        private const string Route = "cms/settings/settingsContentGroup";
        private const string RouteOrder = "cms/settings/settingsContentGroup/actions/order";

        private readonly IAuthManager _authManager;
        private readonly IContentGroupRepository _contentGroupRepository;

        public SettingsContentGroupController(IAuthManager authManager, IContentGroupRepository contentGroupRepository)
        {
            _authManager = authManager;
            _contentGroupRepository = contentGroupRepository;
        }

        public class GetResult
        {
            public IEnumerable<ContentGroup> Groups { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public string GroupName { get; set; }
        }

        public class OrderRequest : SiteRequest
        {
            public int GroupId { get; set; }
            public int Taxis { get; set; }
            public bool IsUp { get; set; }
        }
    }
}
