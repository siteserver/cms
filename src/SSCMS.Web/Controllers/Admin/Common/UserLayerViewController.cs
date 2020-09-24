using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;

namespace SSCMS.Web.Controllers.Admin.Common
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UserLayerViewController : ControllerBase
    {
        private const string Route = "common/userLayerView";

        private readonly IUserRepository _userRepository;
        private readonly IUserGroupRepository _userGroupRepository;

        public UserLayerViewController(IUserRepository userRepository, IUserGroupRepository userGroupRepository)
        {
            _userRepository = userRepository;
            _userGroupRepository = userGroupRepository;
        }

        public class GetRequest
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
        }

        public class GetResult
        {
            public User User { get; set; }
            public string GroupName { get; set; }
        }
    }
}
