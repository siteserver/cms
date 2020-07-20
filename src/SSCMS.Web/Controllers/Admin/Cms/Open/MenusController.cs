using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Open
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class MenusController : ControllerBase
    {
        private const string Route = "cms/open/menus";
        private const string RouteActionsPull = "cms/open/menus/actions/pull";
        private const string RouteActionsPush = "cms/open/menus/actions/push";

        private readonly IAuthManager _authManager;
        private readonly IOpenAccountRepository _openAccountRepository;
        private readonly IOpenMenuRepository _openMenuRepository;
        private readonly IOpenManager _openManager;

        public MenusController(IAuthManager authManager, IOpenAccountRepository openAccountRepository, IOpenMenuRepository openMenuRepository, IOpenManager openManager)
        {
            _authManager = authManager;
            _openAccountRepository = openAccountRepository;
            _openMenuRepository = openMenuRepository;
            _openManager = openManager;
        }

        public class OpenMenusResult
        {
            public List<OpenMenu> OpenMenus { get; set; }
        }

        public class GetResult : OpenMenusResult
        {
            public IEnumerable<Select<string>> MenuTypes { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public int Id { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public int Id { get; set; }
            public int ParentId { get; set; }
            public int Taxis { get; set; }
            public string Text { get; set; }
            public OpenMenuType MenuType { get; set; }
            public string Key { get; set; }
            public string Url { get; set; }
            public string AppId { get; set; }
            public string PagePath { get; set; }
            public string MediaId { get; set; }
        }
    }
}
