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
    public partial class AccountController : ControllerBase
    {
        private const string Route = "wx/account";
        private const string RouteMp = "wx/account/mp";

        private readonly IAuthManager _authManager;
        private readonly IWxManager _wxManager;
        private readonly IWxAccountRepository _wxAccountRepository;

        public AccountController(IAuthManager authManager, IWxManager wxManager, IWxAccountRepository wxAccountRepository)
        {
            _authManager = authManager;
            _wxManager = wxManager;
            _wxAccountRepository = wxAccountRepository;
        }

        public class GetResult
        {
            public string MpUrl { get; set; }
            public WxAccount Account { get; set; }
            public List<Select<string>> MpTypes { get; set; }
        }

        public class MpSubmitRequest
        {
            public int SiteId { get; set; }
            public bool IsEnabled { get; set; }
            public string MpName { get; set; }
            public WxMpType MpType { get; set; }
            public string MpAppId { get; set; }
            public string MpAppSecret { get; set; }
        }

        public class MpSubmitResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public WxAccount Account { get; set; }
        }
    }
}
