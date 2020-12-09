using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Home
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class HomeMenusController : ControllerBase
    {
        private const string Route = "settings/homeMenus";
        private const string RouteReset = "settings/homeMenus/actions/reset";

        private readonly IAuthManager _authManager;
        private readonly IUserMenuRepository _userMenuRepository;
        private readonly IUserGroupRepository _userGroupRepository;

        public HomeMenusController(IAuthManager authManager, IUserMenuRepository userMenuRepository, IUserGroupRepository userGroupRepository)
        {
            _authManager = authManager;
            _userMenuRepository = userMenuRepository;
            _userGroupRepository = userGroupRepository;
        }

        public class GetResult
        {
            public List<UserMenu> UserMenus { get; set; }
            public List<UserGroup> Groups { get; set; }
        }

        public class UserMenusResult
        {
            public List<UserMenu> UserMenus { get; set; }
        }

        public class SubmitRequest
        {
            public int Id { get; set; }
            public bool IsGroup { get; set; }
            public List<int> GroupIds { get; set; }
            public bool Disabled { get; set; }
            public int ParentId { get; set; }
            public int Taxis { get; set; }
            public string Text { get; set; }
            public string IconClass { get; set; }
            public string Link { get; set; }
            public string Target { get; set; }
        }
    }
}
