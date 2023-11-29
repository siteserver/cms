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
            var body = new JsonMediaId
            {
                media_id = mediaId
            };
            var url = $"https://api.weixin.qq.com/cgi-bin/freepublish/submit?access_token={accessToken}";
            var (success, result, errorMessage) = await RestUtils.PostStringAsync(url, TranslateUtils.JsonSerialize(body));

            if (success)
            {
                var json = TranslateUtils.JsonDeserialize<FreePublishSubmitResult>(result);
                if (json.errcode != 0)
                {
                    success = false;
                    errorMessage = $"API 调用发生错误：{json.errmsg}";

                    await _errorLogRepository.AddErrorLogAsync(new Exception(result), "WxManager.FreePublishSubmitAsync");
                }
                else
                {
                    publishId = json.publish_id;
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
