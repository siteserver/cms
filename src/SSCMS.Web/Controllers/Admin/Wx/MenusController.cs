using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class MenusController : ControllerBase
    {
        private const string Route = "wx/menus";
        private const string RouteActionsPull = "wx/menus/actions/pull";
        private const string RouteActionsPush = "wx/menus/actions/push";

        private readonly IAuthManager _authManager;
        private readonly IWxMenuRepository _wxMenuRepository;
        private readonly IWxManager _wxManager;

        public MenusController(IAuthManager authManager, IWxMenuRepository wxMenuRepository, IWxManager wxManager)
        {
            _authManager = authManager;
            _wxMenuRepository = wxMenuRepository;
            _wxManager = wxManager;
        }

        public class WxMenusResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public List<WxMenu> WxMenus { get; set; }
        }

        public class GetResult : WxMenusResult
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
            public WxMenuType MenuType { get; set; }
            public string Key { get; set; }
            public string Url { get; set; }
            public string AppId { get; set; }
            public string PagePath { get; set; }
            public string MediaId { get; set; }
        }

        public class PushResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
