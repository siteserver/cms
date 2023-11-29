using System;
using System.Threading.Tasks;
using SSCMS.Utils;

// https://developers.weixin.qq.com/doc/offiaccount/Message_Management/Batch_Sends_and_Originality_Checks.html#_6%E3%80%81%E9%A2%84%E8%A7%88%E6%8E%A5%E5%8F%A3%E3%80%90%E8%AE%A2%E9%98%85%E5%8F%B7%E4%B8%8E%E6%9C%8D%E5%8A%A1%E5%8F%B7%E8%AE%A4%E8%AF%81%E5%90%8E%E5%9D%87%E5%8F%AF%E7%94%A8%E3%80%91

namespace SSCMS.Core.Services
{
    public partial class WxManager
    {
        public async Task<(bool success, string errorMessage)> PreviewAsync(string accessToken, string mediaId, string wxName)
        {
            var body = new JsonPreviewRequest
            {
                towxname = wxName,
                mpnews = new JsonMediaId
                {
                    media_id = mediaId,
                },
                msgtype = "mpnews"
            };
            var url = $"https://api.weixin.qq.com/cgi-bin/message/mass/preview?access_token={accessToken}";
            var (success, result, errorMessage) = await RestUtils.PostStringAsync(url, TranslateUtils.JsonSerialize(body));

            if (success)
            {
                var json = TranslateUtils.JsonDeserialize<JsonResult>(result);
                if (json.errcode != 0)
                {
                    success = false;
                    errorMessage = $"API 调用发生错误：{json.errmsg}";

                    await _errorLogRepository.AddErrorLogAsync(new Exception(result), "WxManager.PreviewAsync");
                }
            }
            else
            {
                await _errorLogRepository.AddErrorLogAsync(new Exception(errorMessage), "WxManager.PreviewAsync");
            }

            return (success, errorMessage);
        }
    }
}
