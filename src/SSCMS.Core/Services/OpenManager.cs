using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP.Containers;
using SSCMS.Services;

namespace SSCMS.Core.Services
{
    public class OpenManager : IOpenManager
    {
        private readonly ISettingsManager _settingsManager;

        public OpenManager(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        public (bool, string, string) GetWxAccessToken(string appId, string appSecret)
        {
            string token;
            try
            {
                token = AccessTokenContainer.TryGetAccessToken(appId, appSecret);
            }
            catch (ErrorJsonResultException ex)
            {
                return (false, string.Empty, $"API 调用发生错误：{ex.JsonResult.errmsg}");
            }
            catch (Exception ex)
            {
                return (false, string.Empty, $"执行过程发生错误：{ex.Message}");
            }

            return (true, token, string.Empty);
        }
    }
}
