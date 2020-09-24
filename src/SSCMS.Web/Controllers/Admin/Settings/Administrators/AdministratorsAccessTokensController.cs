using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AdministratorsAccessTokensController : ControllerBase
    {
        private const string Route = "settings/administratorsAccessTokens";

        private readonly IAuthManager _authManager;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IAdministratorRepository _administratorRepository;

        public AdministratorsAccessTokensController(IAuthManager authManager, IAccessTokenRepository accessTokenRepository, IAdministratorRepository administratorRepository)
        {
            _authManager = authManager;
            _accessTokenRepository = accessTokenRepository;
            _administratorRepository = administratorRepository;
        }

        public class ListResult
        {
            public List<AccessToken> Tokens { get; set; }
            public List<string> AdminNames { get; set; }
            public List<string> Scopes { get; set; }
            public string AdminName { get; set; }
        }

        public class TokensResult
        {
            public List<AccessToken> Tokens { get; set; }
        }
    }
}
