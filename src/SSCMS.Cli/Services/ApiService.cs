using System.Collections.Generic;
using SSCMS.Cli.Abstractions;

namespace SSCMS.Cli.Services
{
    public partial class ApiService : IApiService
    {
        private const string RestUrlLogin = "/login";
        private const string RestUrlStatus = "/status";
        private const string RestUrlRegister = "/register";
        private const string RestUrlPluginPublish = "/plugin-publish";
        private const string RestUrlPluginUnPublish = "/plugin-unpublish";
        private const string RestUrlPluginSearch = "/plugin-search";
        private const string RestUrlPluginShow = "/plugin-show";
        private const string RestUrlReleases = "/releases";
        private const string RestUrlThemePublish = "/theme-publish";
        private const string RestUrlThemeUnPublish = "/theme-unpublish";

        private readonly IConfigService _configService;

        public ApiService(IConfigService configService)
        {
            _configService = configService;
        }

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

        public class PluginUnPublishRequest
        {
            public string PluginId { get; set; }
        }

        public class ThemeUnPublishRequest
        {
            public string Name { get; set; }
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
