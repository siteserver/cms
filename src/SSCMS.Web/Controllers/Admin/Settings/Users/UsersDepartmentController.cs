using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UsersDepartmentController : ControllerBase
    {
        private const string Route = "settings/usersDepartment";
        private const string RouteGet = "settings/usersDepartment/{departmentId:int}";
        private const string RouteUpdate = "settings/usersDepartment/actions/update";
        private const string RouteDelete = "settings/usersDepartment/actions/delete";
        private const string RouteAppend = "settings/usersDepartment/actions/append";
        private const string RouteDrop = "settings/usersDepartment/actions/drop";

        private readonly IAuthManager _authManager;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IConfigRepository _configRepository;

        public UsersDepartmentController(
            IAuthManager authManager,
            IDepartmentRepository departmentRepository,
            IConfigRepository configRepository
        )
        {
            _authManager = authManager;
            _departmentRepository = departmentRepository;
            _configRepository = configRepository;
        }

        public class ListResult
        {
            public List<Cascade<int>> Departments { get; set; }
        }

        public class GetResult
        {
            public Department Department { get; set; }
        }

        public class DropRequest
        {
            public int SourceId { get; set; }
            public int TargetId { get; set; }
            public string DropType { get; set; }
        }

        public class DeleteRequest
        {
            public int DepartmentId { get; set; }
            public string DepartmentName { get; set; }
        }

        public class AppendRequest
        {
            public int ParentId { get; set; }
            public string Departments { get; set; }
        }
    }
}
