using System.Collections.Generic;
using System.Net;
using RestSharp;
using Serilog;
using SSCMS.Cli.Abstractions;
using SSCMS.Core.Plugins;
using SSCMS.Utils;

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
        

        private static (bool success, TResult result, string failureMessage) ExecutePost<TRequest, TResult>(string relatedUrl, TRequest body, string accessToken = null) where TResult : class

        {
            var client = new RestClient(CloudUtils.Api.GetCliUrl(relatedUrl)) { Timeout = -1 };
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.AddHeader("Authorization", $"Bearer {accessToken}");
            }
            request.AddParameter("application/json", TranslateUtils.JsonSerialize(body), ParameterType.RequestBody);
            var response = client.Execute<TResult>(request);
            if (!response.IsSuccessful)
            {
                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    Log.Fatal(response.Content);
                    var error = TranslateUtils.JsonDeserialize<InternalServerError>(response.Content);
                    if (error != null)
                    {
                        return (false, null, error.Message);
                    }
                }
                return (false, null, response.ErrorMessage);
            }

            return (true, response.Data, null);
        }
    }
}
