using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public partial class UsersProfileController : ControllerBase
    {
        private const string Route = "settings/usersProfile";
        private const string RouteUpload = "settings/usersProfile/actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IUserRepository _userRepository;
        private readonly ITableStyleRepository _tableStyleRepository;

        public UsersProfileController(IAuthManager authManager, IPathManager pathManager, IUserRepository userRepository, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _userRepository = userRepository;
            _tableStyleRepository = tableStyleRepository;
        }

        public class GetResult
        {
            public User User { get; set; }
            public IEnumerable<InputStyle> Styles { get; set; }
        }

        public class UploadRequest
        {
            public int UserId { get; set; }
            public IFormFile File { set; get; }
        }
    }
}
