using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AdministratorsLayerProfileController : ControllerBase
    {
        private const string Route = "settings/administratorsLayerProfile";
        private const string RouteUpload = "settings/administratorsLayerProfile/actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IAdministratorRepository _administratorRepository;

        public AdministratorsLayerProfileController(IAuthManager authManager, IPathManager pathManager, IAdministratorRepository administratorRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _administratorRepository = administratorRepository;
        }

        public class GetRequest
        {
            public string UserName { get; set; }
        }

        public class GetResult
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string DisplayName { get; set; }
            public string AvatarUrl { get; set; }
            public string Mobile { get; set; }
            public string Email { get; set; }
        }

        public class SubmitRequest
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string DisplayName { get; set; }
            public string Password { get; set; }
            public string AvatarUrl { get; set; }
            public string Mobile { get; set; }
            public string Email { get; set; }
        }
    }
}
