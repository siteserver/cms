using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class CloudManager
    {
        // public async Task<bool> IsSpellingCheckAsync()
        // {
        //     var config = await _configRepository.GetAsync();
        //     return config.IsCloudSpellingCheck;
        // }

        // public async Task<SpellResult> SpellingCheckAsync(string text)
        // {
        //     var config = await _configRepository.GetAsync();
        //     if (string.IsNullOrEmpty(config.CloudUserName) || string.IsNullOrEmpty(config.CloudToken))
        //     {
        //         throw new Exception("未登录");
        //     }

        //     text = StringUtils.StripTags(text);
        //     if (string.IsNullOrEmpty(text) || !config.IsCloudSpellingCheck)
        //     {
        //         return new SpellResult
        //         {
        //             IsErrorWords = false,
        //             ErrorWords = new List<ErrorWord>(),
        //         };
        //     }

        //     var url = GetCloudUrl(RouteSpell);
        //     var (success, result, errorMessage) = await RestUtils.PostAsync<SpellRequest, SpellResult>(url, new SpellRequest
        //     {
        //         Text = text
        //     }, config.CloudToken);

        //     if (!success)
        //     {
        //         throw new Exception(errorMessage);
        //     }

        //     return result;
        // }

        public async Task<SpellSettings> GetSpellSettingsAsync()
        {
            var config = await _configRepository.GetAsync();
            var isAuthentication = IsAuthentication(config);
            return new SpellSettings
            {
                IsSpellingCheck = isAuthentication && config.IsCloudSpellingCheck,
                IsSpellingCheckAuto = config.IsCloudSpellingCheckAuto,
                IsSpellingCheckIgnore = config.IsCloudSpellingCheckIgnore,
                IsSpellingCheckWhiteList = config.IsCloudSpellingCheckWhiteList,
            };
        }

        public async Task<SpellResult> SpellingCheckAsync(string text)
        {
            var config = await _configRepository.GetAsync();
            var isAuthentication = IsAuthentication(config);
            if (!isAuthentication)
            {
                throw new Exception("云助手未登录");
            }

            text = StringUtils.StripTags(text);
            if (string.IsNullOrEmpty(text) || !config.IsCloudSpellingCheck)
            {
                return new SpellResult
                {
                    IsErrorWords = false,
                    ErrorWords = new List<ErrorWord>(),
                };
            }

            var url = GetCloudUrl(RouteSpell);
            var (success, result, errorMessage) = await RestUtils.PostAsync<SpellRequest, SpellResult>(url, new SpellRequest
            {
                Text = text
            }, config.CloudToken);

            if (!success)
            {
                throw new Exception(errorMessage);
            }

            return result;
        }

        public class SpellAddWhiteListRequest
        {
            public string Word { get; set; }
        }
        
        public async Task<(bool success, string errorMessage)> AddSpellWhiteListAsync(string word)
        {
            var config = await _configRepository.GetAsync();
            if (string.IsNullOrEmpty(config.CloudUserName) || string.IsNullOrEmpty(config.CloudToken))
            {
                throw new Exception("云助手未登录");
            }

            if (string.IsNullOrEmpty(word))
            {
                throw new Exception("文本不能为空");
            }

            var url = GetCloudUrl(RouteSpellAddWhiteList);
            var (success, result, errorMessage) = await RestUtils.PostAsync<SpellAddWhiteListRequest, BoolResult>(url, new SpellAddWhiteListRequest
            {
                Word = word
            }, config.CloudToken);

            return (success, errorMessage);
        }
    }
}
