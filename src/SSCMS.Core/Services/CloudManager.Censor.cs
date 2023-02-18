using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class CloudManager
    {
        [Obsolete]
        public Task<bool> IsTextEnabledAsync()
        {
            return Task.FromResult(false);
        }

        [Obsolete]
        public Task<bool> IsImageEnabledAsync()
        {
            return Task.FromResult(false);
        }

        [Obsolete]
        public Task<CensorResult> ScanText(string text)
        {
            throw new System.NotImplementedException();
        }

        [Obsolete]
        public Task<CensorResult> ScanImage(string imagePath)
        {
            throw new System.NotImplementedException();
        }

        public async Task<CensorSettings> GetCensorSettingsAsync()
        {
            var config = await _configRepository.GetAsync();
            var isAuthentication = IsAuthentication(config);
            return new CensorSettings
            {
                IsCensorText = isAuthentication && config.IsCloudCensorText,
                IsCensorTextAuto = config.IsCloudCensorTextAuto,
                IsCensorTextIgnore = config.IsCloudCensorTextIgnore,
                IsCensorTextWhiteList = config.IsCloudCensorTextWhiteList,
            };
        }

        public async Task<CensorResult> CensorTextAsync(string text)
        {
            var config = await _configRepository.GetAsync();
            var isAuthentication = IsAuthentication(config);
            if (!isAuthentication)
            {
                throw new Exception("云助手未登录");
            }

            text = StringUtils.StripTags(text);
            if (string.IsNullOrEmpty(text) || !config.IsCloudCensorText)
            {
                return new CensorResult
                {
                    IsBadWords = false,
                    BadWords = new List<BadWord>(),
                };
            }

            var url = GetCloudUrl(RouteCensor);
            var (success, result, errorMessage) = await RestUtils.PostAsync<CensorRequest, CensorResult>(url, new CensorRequest
            {
                Text = text
            }, config.CloudToken);

            if (!success)
            {
                throw new Exception(errorMessage);
            }

            return result;
        }

        public class CensorAddWhiteListRequest
        {
            public string Word { get; set; }
        }
        
        public async Task<(bool success, string errorMessage)> AddCensorWhiteListAsync(string word)
        {
            var config = await _configRepository.GetAsync();
            var isAuthentication = IsAuthentication(config);
            if (!isAuthentication)
            {
                throw new Exception("云助手未登录");
            }

            if (string.IsNullOrEmpty(word))
            {
                throw new Exception("违规词不能为空");
            }

            var url = GetCloudUrl(RouteCensorAddWhiteList);
            var (success, result, errorMessage) = await RestUtils.PostAsync<CensorAddWhiteListRequest, BoolResult>(url, new CensorAddWhiteListRequest
            {
                Word = word
            }, config.CloudToken);

            return (success, errorMessage);
        }
    }
}
