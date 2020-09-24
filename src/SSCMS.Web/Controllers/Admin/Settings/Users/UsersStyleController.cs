using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UsersStyleController : ControllerBase
    {
        private const string Route = "settings/usersStyle";
        private const string RouteImport = "settings/usersStyle/actions/import";
        private const string RouteExport = "settings/usersStyle/actions/export";
        private const string RouteReset = "settings/usersStyle/actions/reset";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IUserRepository _userRepository;
        private readonly ITableStyleRepository _tableStyleRepository;

        public UsersStyleController(IAuthManager authManager, IPathManager pathManager, IDatabaseManager databaseManager, IUserRepository userRepository, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _userRepository = userRepository;
            _tableStyleRepository = tableStyleRepository;
        }

        public class GetResult
        {
            public List<InputStyle> Styles { get; set; }
            public string TableName { get; set; }
            public List<int> RelatedIdentities { get; set; }
        }

        public class DeleteRequest
        {
            public string AttributeName { get; set; }
        }

        public class DeleteResult
        {
            public List<InputStyle> Styles { get; set; }
        }

        public class ResetResult
        {
            public List<InputStyle> Styles { get; set; }
        }
    }
}
