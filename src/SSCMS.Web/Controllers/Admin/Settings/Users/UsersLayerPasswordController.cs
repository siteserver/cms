using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UsersLayerPasswordController : ControllerBase
    {
        private const string Route = "settings/usersLayerPassword";

        private readonly IAuthManager _authManager;
        private readonly IUserRepository _userRepository;

        public UsersLayerPasswordController(IAuthManager authManager, IUserRepository userRepository)
        {
            _authManager = authManager;
            _userRepository = userRepository;
        }

        public class GetRequest
        {
            public int UserId { get; set; }
        }

        public class GetResult
        {
            public User User { get; set; }
        }

        public class SubmitRequest
        {
            public int UserId { get; set; }
            public string Password { get; set; }
        }
    }
}
