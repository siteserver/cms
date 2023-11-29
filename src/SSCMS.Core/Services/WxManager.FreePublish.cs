using System;
using System.Threading.Tasks;
using SSCMS.Utils;

// https://developers.weixin.qq.com/doc/offiaccount/Publish/Publish.html

namespace SSCMS.Core.Services
{
    public partial class WxManager
    {
        public async Task<(bool success, string publishId, string errorMessage)> FreePublishSubmitAsync(string accessToken, string mediaId)
        {
            var publishId = string.Empty;
            var url = $"https://api.weixin.qq.com/cgi-bin/freepublish/submit?access_token={accessToken}";
            var (success, result, errorMessage) = await RestUtils.PostAsync<JsonMediaId, FreePublishSubmitResult>(url, new JsonMediaId
            {
                media_id = mediaId
            });

            if (success)
            {
                if (result.errcode != 0)
                {
                    success = false;
                    errorMessage = $"API 调用发生错误：{result.errmsg}";

                    await _errorLogRepository.AddErrorLogAsync(new Exception(TranslateUtils.JsonSerialize(result)), "WxManager.FreePublishSubmitAsync");
                }
                else
                {
                    publishId = result.publish_id;
                }
            }
            else
            {
                await _errorLogRepository.AddErrorLogAsync(new Exception(errorMessage), "WxManager.FreePublishSubmitAsync");
            }

            return (success, publishId, errorMessage);
        }
    }
}
