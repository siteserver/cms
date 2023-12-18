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
        private const string RouteDelete = "wx/menus/actions/delete";
        private const string RoutePull = "wx/menus/actions/pull";
        private const string RoutePush = "wx/menus/actions/push";

        private readonly IAuthManager _authManager;
        private readonly IWxManager _wxManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IWxMenuRepository _wxMenuRepository;

        public MenusController(IAuthManager authManager, IWxManager wxManager, ISiteRepository siteRepository, IWxMenuRepository wxMenuRepository)
        {
            _authManager = authManager;
            _wxManager = wxManager;
            _siteRepository = siteRepository;
            _wxMenuRepository = wxMenuRepository;
        }

        public class WxMenusResult
        {
            public List<WxMenu> WxMenus { get; set; }
        }

        public class GetResult : WxMenusResult
        {
            public bool IsWxEnabled { get; set; }
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
    }
}
