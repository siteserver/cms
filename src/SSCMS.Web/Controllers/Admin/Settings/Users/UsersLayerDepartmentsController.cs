using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UsersLayerDepartmentsController : ControllerBase
    {
        private const string Route = "settings/usersLayerDepartments";
        private const string RouteOptions = "settings/usersLayerDepartments/actions/options";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICreateManager _createManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUserRepository _userRepository;

        public UsersLayerDepartmentsController(IAuthManager authManager, IPathManager pathManager, ICreateManager createManager, IDatabaseManager databaseManager, IPluginManager pluginManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IDepartmentRepository departmentRepository, IUserRepository userRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _createManager = createManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _departmentRepository = departmentRepository;
            _userRepository = userRepository;
        }

        public class GetRequest
        {
            public string UserIds { get; set; }
        }

        public class GetResult
        {
            public IEnumerable<User> Users { get; set; }
        }

        public class GetOptionsRequest
        {
            public int DepartmentId { get; set; }
        }

        public class GetOptionsResult
        {
            public List<Cascade<int>> TransDepartments { get; set; }
        }

        public class SubmitRequest
        {
            public int DepartmentId { get; set; }
            public string UserIds { get; set; }
            public int TransDepartmentId { get; set; }
        }
    }
}
