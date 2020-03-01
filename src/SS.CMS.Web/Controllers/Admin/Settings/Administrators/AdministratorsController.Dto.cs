using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsController
    {
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
