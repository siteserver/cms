using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using RestSharp;
using Serilog;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Models;
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

        public (ConfigStatus status, string failureMessage) GetStatus()
        {
            var status = _configService.Status;
            if (status == null || string.IsNullOrEmpty(status.UserName) || string.IsNullOrEmpty(status.AccessToken))
            {
                return (null, "you have not logged in");
            }

            var client = new RestClient(CloudUtils.Api.GetCliUrl(RestUrlStatus)) { Timeout = -1 };
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {status.AccessToken}");
            var response = client.Execute<StatusResult>(request);

            if (!response.IsSuccessful || response.Data.UserName != status.UserName)
            {
                return (null, "you have not logged in");
            }

            return (status, null);
        }

        public (bool success, string failureMessage) Register(string userName, string mobile, string email, string password)
        {
            var client = new RestClient(CloudUtils.Api.GetCliUrl(RestUrlRegister)) {Timeout = -1};
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", TranslateUtils.JsonSerialize(new RegisterRequest
            {
                UserName = userName,
                Mobile = mobile,
                Email = email,
                Password = password
            }), ParameterType.RequestBody);
            var response = client.Execute(request);

            return response.IsSuccessful ? (true, null) : (false, StringUtils.Trim(response.Content, '"'));
        }

        public async Task<(bool success, string failureMessage)> LoginAsync(string account, string password)
        {
            var client = new RestClient(CloudUtils.Api.GetCliUrl(RestUrlLogin)) { Timeout = -1 };
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", TranslateUtils.JsonSerialize(new LoginRequest
            {
                Account = account,
                Password = AuthUtils.Md5ByString(password),
                IsPersistent = true
            }), ParameterType.RequestBody);
            var response = client.Execute<LoginResult>(request);
            if (!response.IsSuccessful)
            {
                return (false, "your account or password was incorrect");
            }

            var loginResult = response.Data;

            var status = new ConfigStatus
            {
                UserName = loginResult.UserName,
                AccessToken = loginResult.AccessToken
            };

            await _configService.SaveStatusAsync(status);

            return (true, null);
        }

        public (bool success, string failureMessage) PluginsPublish(string publisher, string zipPath)
        {
            var status = _configService.Status;
            if (status == null || string.IsNullOrEmpty(status.UserName) || string.IsNullOrEmpty(status.AccessToken))
            {
                return (false, "you have not logged in");
            }

            if (status.UserName != publisher)
            {
                return (false, $"the publisher in package.json should be '{status.UserName}'");
            }

            var client = new RestClient(CloudUtils.Api.GetCliUrl(RestUrlPluginPublish)) {Timeout = -1};
            var request = new RestRequest(Method.POST);
            //request.AddHeader("Content-Type", "multipart/form-data");
            request.AddHeader("Authorization", $"Bearer {status.AccessToken}");
            request.AddFile("file", zipPath);
            var response = client.Execute(request);

            return response.IsSuccessful ? (true, null) : (false, StringUtils.Trim(response.Content, '"'));
        }

        public (bool success, string failureMessage) UnPluginsPublish(string pluginId)
        {
            var status = _configService.Status;
            if (status == null || string.IsNullOrEmpty(status.UserName) || string.IsNullOrEmpty(status.AccessToken))
            {
                return (false, "you have not logged in");
            }

            var client = new RestClient(CloudUtils.Api.GetCliUrl(RestUrlPluginUnPublish)) { Timeout = -1 };
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {status.AccessToken}");
            request.AddParameter("application/json", TranslateUtils.JsonSerialize(new UnPublishRequest
            {
                PluginId = pluginId
            }), ParameterType.RequestBody);
            var response = client.Execute(request);
            if (!response.IsSuccessful)
            {
                return (false, StringUtils.Trim(response.Content, '"'));
            }

            return (true, null);
        }

        public (bool success, List<PluginAndUser> results, string failureMessage) PluginsSearch(string word)
        {
            //var client = new RestClient(CloudUtils.Api.GetCliUrl(RestUrlPluginSearch)) { Timeout = -1 };
            //var request = new RestRequest(Method.POST);
            //request.AddHeader("Content-Type", "application/json");
            //request.AddParameter("application/json", TranslateUtils.JsonSerialize(new SearchRequest
            //{
            //    Word = word
            //}), ParameterType.RequestBody);
            //var response = client.Execute<List<PluginAndUser>>(request);
            //if (!response.IsSuccessful)
            //{
            //    return (false, null, response.ErrorMessage);
            //}

            //return (true, response.Data, null);

            return ExecutePost<SearchRequest, List<PluginAndUser>>(RestUrlPluginSearch, new SearchRequest
            {
                Word = word
            });
        }

        public (bool success, PluginAndUser result, string failureMessage) PluginsShow(string pluginId)
        {
            //var client = new RestClient(CloudUtils.Api.GetCliUrl(RestUrlPluginShow)) { Timeout = -1 };
            //var request = new RestRequest(Method.POST);
            //request.AddHeader("Content-Type", "application/json");
            //request.AddParameter("application/json", TranslateUtils.JsonSerialize(new ShowRequest
            //{
            //    PluginId = pluginId
            //}), ParameterType.RequestBody);
            //var response = client.Execute<PluginAndUser>(request);
            //if (!response.IsSuccessful)
            //{
            //    return (false, null, response.ErrorMessage);
            //}

            //return (true, response.Data, null);

            return ExecutePost<ShowRequest, PluginAndUser>(RestUrlPluginShow, new ShowRequest
            {
                PluginId = pluginId
            });
        }

        public (bool success, GetReleasesResult result, string failureMessage) GetReleases(bool isNightly, string version, List<string> pluginIds)
        {
            //var client = new RestClient(CloudUtils.Api.GetCliUrl(RestUrlReleases)) { Timeout = -1 };
            //var request = new RestRequest(Method.POST);
            //request.AddHeader("Content-Type", "application/json");
            //request.AddParameter("application/json", TranslateUtils.JsonSerialize(new GetReleasesRequest
            //{
            //    IsNightly = isNightly,
            //    Version = version,
            //    PluginIds = pluginIds
            //}), ParameterType.RequestBody);
            //var response = client.Execute<GetReleasesResult>(request);
            //if (!response.IsSuccessful)
            //{
            //    if (response.StatusCode == HttpStatusCode.InternalServerError)
            //    {
            //        Log.Fatal(response.Content);
            //        var error = TranslateUtils.JsonDeserialize<InternalServerError>(response.Content);
            //        if (error != null)
            //        {
            //            return (false, null, error.Message);
            //        }
            //    }
            //    return (false, null, response.ErrorMessage);
            //}

            //return (true, response.Data, null);

            return ExecutePost<GetReleasesRequest, GetReleasesResult>(RestUrlReleases, new GetReleasesRequest
            {
                IsNightly = isNightly,
                Version = version,
                PluginIds = pluginIds
            });
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
