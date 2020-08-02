using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;
using SSCMS.Wx;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UsersController : ControllerBase
    {
        private const string Route = "wx/users";

        private readonly IAuthManager _authManager;
        private readonly IWxManager _wxManager;
        private readonly IWxUserRepository _wxUserRepository;

        public UsersController(IAuthManager authManager, IWxManager wxManager, IWxUserRepository wxUserRepository)
        {
            _authManager = authManager;
            _wxManager = wxManager;
            _wxUserRepository = wxUserRepository;
        }

        public class GetRequest : SiteRequest
        {
            public bool Init { get; set; }
            public int TagId { get; set; }
            public string Keyword { get; set; }
            public int Page { get; set; }
            public int PerPage { get; set; }
        }

        public class GetResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public IEnumerable<WxUserTag> Tags { get; set; }
            public int Total { get; set; }
            public int Count { get; set; }
            public IEnumerable<WxUser> Users { get; set; }
        }
    }
}
