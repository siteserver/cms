using System;
using System.Collections.Generic;
using System.Text;

namespace SSCMS.Cli.Services
{
    public partial class ApiService
    {
        public class StatusResult
        {
            public string UserName { get; set; }
            public string DisplayName { get; set; }
            public string Mobile { get; set; }
            public string Email { get; set; }
        }

        public class LoginRequest
        {
            public string Account { get; set; }
            public string Password { get; set; }
            public bool IsPersistent { get; set; }
        }

        public class LoginResult
        {
            public string UserName { get; set; }
            public string AccessToken { get; set; }
        }

        public class RegisterRequest
        {
            public string UserName { get; set; }
            public string Mobile { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class PublishRequest
        {
            public string Account { get; set; }
            public string Password { get; set; }
            public bool IsPersistent { get; set; }
        }

        public class PublishResult
        {
            public string UserName { get; set; }
            public string AccessToken { get; set; }
        }

        public class UnPublishRequest
        {
            public string PluginId { get; set; }
        }

        public class SearchRequest
        {
            public string Word { get; set; }
        }

        public class ShowRequest
        {
            public string PluginId { get; set; }
        }

        public class PluginAndUser
        {
            public Dictionary<string, object> Plugin { get; set; }
            public Dictionary<string, object> User { get; set; }
        }

        public class GetReleasesRequest
        {
            public bool IsNightly { get; set; }
            public string Version { get; set; }
            public List<string> PluginIds { get; set; }
        }

        public class GetReleasesCms
        {
            public string Version { get; set; }
            public string Published { get; set; }
        }

        public class GetReleasesPlugin
        {
            public string PluginId { get; set; }
            public string Version { get; set; }
            public string Published { get; set; }
        }

        public class GetReleasesResult
        {
            public GetReleasesCms Cms { get; set; }
            public List<GetReleasesPlugin> Plugins { get; set; }
        }
    }
}
