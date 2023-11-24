using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Common
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UserLayerViewController : ControllerBase
    {
        private const string Route = "common/userLayerView";

        private readonly IAuthManager _authManager;
        private readonly IUserRepository _userRepository;
        private readonly IUserGroupRepository _userGroupRepository;

        public UserLayerViewController(IAuthManager authManager, IUserRepository userRepository, IUserGroupRepository userGroupRepository)
        {
            _authManager = authManager;
            _userRepository = userRepository;
            _userGroupRepository = userGroupRepository;
        }

        public class GetRequest
        {
            public string Guid { get; set; }
        }

        public class GetResult
        {
            public User User { get; set; }
            public string GroupName { get; set; }
        }
    }
}
