using System;
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
    public partial class AdministratorsController : ControllerBase
    {
        private const string Route = "settings/administrators";
        private const string RoutePermissions = "settings/administrators/permissions/{adminId:int}";
        private const string RouteLock = "settings/administrators/actions/lock";
        private const string RouteUnLock = "settings/administrators/actions/unLock";
        private const string RouteImport = "settings/administrators/actions/import";
        private const string RouteExport = "settings/administrators/actions/export";

        private readonly ICacheManager _cacheManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IAdministratorsInRolesRepository _administratorsInRolesRepository;

        public AdministratorsController(ICacheManager cacheManager, IAuthManager authManager, IPathManager pathManager, IDatabaseManager databaseManager, IAdministratorRepository administratorRepository, IRoleRepository roleRepository, ISiteRepository siteRepository, IAdministratorsInRolesRepository administratorsInRolesRepository)
        {
            _cacheManager = cacheManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _administratorRepository = administratorRepository;
            _roleRepository = roleRepository;
            _siteRepository = siteRepository;
            _administratorsInRolesRepository = administratorsInRolesRepository;
        }

        public class GetRequest
        {
            public string Role { get; set; }
            public string Order { get; set; }
            public int LastActivityDate { get; set; }
            public string Keyword { get; set; }
            public int Offset { get; set; }
            public int Limit { get; set; }
        }

        public class Admin
        {
            public int Id { get; set; }
            public string AvatarUrl { get; set; }
            public string UserName { get; set; }
            public string DisplayName { get; set; }
            public string Mobile { get; set; }
            public DateTime? LastActivityDate { get; set; }
            public int CountOfLogin { get; set; }
            public bool Locked { get; set; }
            public string Roles { get; set; }
        }

        public class GetResult
        {
            public List<Admin> Administrators { get; set; }
            public int Count { get; set; }
            public List<KeyValuePair<string, string>> Roles { get; set; }
            public bool IsSuperAdmin { get; set; }
            public int AdminId { get; set; }
        }

        public class GetPermissionsResult
        {
            public List<string> Roles { get; set; }
            public List<Site> AllSites { get; set; }
            public string AdminLevel { get; set; }
            public List<int> CheckedSites { get; set; }
            public List<string> CheckedRoles { get; set; }
        }

        public class SavePermissionsRequest
        {
            public string AdminLevel { get; set; }
            public List<int> CheckedSites { get; set; }
            public List<string> CheckedRoles { get; set; }
        }

        public class SavePermissionsResult
        {
            public string Roles { get; set; }
        }

        public class ImportResult
        {
            public bool Value { set; get; }
            public int Success { set; get; }
            public int Failure { set; get; }
            public string ErrorMessage { set; get; }
        }
    }
}
