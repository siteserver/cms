using System;
using System.Threading.Tasks;
using SSCMS.Core.Utils;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;
using SSCMS.Models;
using SSCMS.Enums;
using SSCMS.Dto;

namespace SSCMS.Core.Services
{
    public partial class CloudManager : ICloudManager
    {
        
        private const string RouteBackup = "backup";
        private const string RouteGetDownloadUrl = "clouds/actions/getDownloadUrl";
        private const string RouteGetOssCredentials = "clouds/actions/getOssCredentials";
        private const string RouteCensor = "censor";
        private const string RouteCensorAddWhiteList = "censor/actions/addWhiteList";
        private const string RouteSpell = "spell";
        private const string RouteSpellAddWhiteList = "spell/actions/addWhiteList";
        private const string RouteVod = "vod";
        private const string RouteSms = "sms";
        private const string RouteMail = "mail";
        public const string DomainDns = "https://a.sscms.net";
        public const string DataZipFileName = "sscms-data.zip";
        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly ICacheManager _cacheManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IConfigRepository _configRepository;
        private readonly IStorageFileRepository _storageFileRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public CloudManager(ISettingsManager settingsManager, IPathManager pathManager, ICacheManager cacheManager, IDatabaseManager databaseManager, IConfigRepository configRepository, IStorageFileRepository storageFileRepository, IErrorLogRepository errorLogRepository)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _cacheManager = cacheManager;
            _databaseManager = databaseManager;
            _configRepository = configRepository;
            _storageFileRepository = storageFileRepository;
            _errorLogRepository = errorLogRepository;
        }

        public static string GetCloudUrl(string relatedUrl) => PageUtils.Combine(CloudUtils.CloudApiHost,
            "v7/cms",
            relatedUrl);

        public async Task<bool> IsAuthenticationAsync()
        {
            var config = await _configRepository.GetAsync();
            return IsAuthentication(config);
        }

        private bool IsAuthentication(Config config)
        {
            return !string.IsNullOrEmpty(config.CloudUserName) && !string.IsNullOrEmpty(config.CloudToken);
        }

        private bool IsFree(Config config)
        {
            return config.CloudType == CloudType.Free || config.CloudExpirationDate < DateTime.Now.AddDays(-1);
        }

        public async Task<CloudType> GetCloudTypeAsync()
        {
            var config = await _configRepository.GetAsync();
            return IsFree(config) ? CloudType.Free : config.CloudType;
        }

        public async Task SetAuthenticationAsync(int userId, string userName, string mobile, string token)
        {
            var config = await _configRepository.GetAsync();
            config.CloudUserId = userId;
            config.CloudUserName = userName;
            config.CloudMobile = mobile;
            config.CloudToken = token;
            await _configRepository.UpdateAsync(config);
        }

        public async Task SetCloudTypeAsync(CloudType cloudType, DateTime expirationDate)
        {
            var config = await _configRepository.GetAsync();
            config.CloudType = cloudType;
            config.CloudExpirationDate = expirationDate;
            await _configRepository.UpdateAsync(config);
        }

        public async Task RemoveAuthenticationAsync()
        {
            var config = await _configRepository.GetAsync();
            config.CloudUserId = 0;
            config.CloudUserName = string.Empty;
            config.CloudMobile = string.Empty;
            config.CloudToken = string.Empty;
            config.CloudType = CloudType.Free;
            await _configRepository.UpdateAsync(config);
        }

        public class GetDownloadUrlRequest
        {
            public string ResourceType { get; set; }
            public string UserName { get; set; }
            public string Name { get; set; }
            public string Version { get; set; }
        }

        public class GetDownloadUrlResult
        {
            public string DownloadUrl { get; set; }
        }

        public async Task<string> GetThemeDownloadUrlAsync(string userName, string name)
        {
            var config = await _configRepository.GetAsync();
            if (string.IsNullOrEmpty(config.CloudUserName) || string.IsNullOrEmpty(config.CloudToken))
            {
                throw new Exception("云助手未登录");
            }

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(name))
            {
                throw new Exception("模板不能为空");
            }

            var url = GetCloudUrl(RouteGetDownloadUrl);
            var (success, result, errorMessage) = await RestUtils.PostAsync<GetDownloadUrlRequest, GetDownloadUrlResult>(url, new GetDownloadUrlRequest
            {
                ResourceType = "Theme",
                UserName = userName,
                Name = name,
            }, config.CloudToken);

            if (!success)
            {
                throw new Exception(errorMessage);
            }

            return result.DownloadUrl;
        }

        public async Task<string> GetExtensionDownloadUrlAsync(string userName, string name, string version)
        {
            var config = await _configRepository.GetAsync();
            if (string.IsNullOrEmpty(config.CloudUserName) || string.IsNullOrEmpty(config.CloudToken))
            {
                throw new Exception("云助手未登录");
            }

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(name))
            {
                throw new Exception("插件不能为空");
            }

            var url = GetCloudUrl(RouteGetDownloadUrl);
            var (success, result, errorMessage) = await RestUtils.PostAsync<GetDownloadUrlRequest, GetDownloadUrlResult>(url, new GetDownloadUrlRequest
            {
                ResourceType = "Extension",
                UserName = userName,
                Name = name,
                Version = version
            }, config.CloudToken);

            if (!success)
            {
                throw new Exception(errorMessage);
            }

            return result.DownloadUrl;
        }

        public async Task<OssCredentials> GetOssCredentialsAsync()
        {
            var config = await _configRepository.GetAsync();
            if (string.IsNullOrEmpty(config.CloudUserName) || string.IsNullOrEmpty(config.CloudToken))
            {
                throw new Exception("云助手未登录");
            }

            var url = GetCloudUrl(RouteGetOssCredentials);
            var (success, result, errorMessage) = await RestUtils.PostAsync<OssCredentials>(url, config.CloudToken);

            if (!success)
            {
                throw new Exception(errorMessage);
            }

            return result;
        }
    }
}
