using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Utils;

// https://developers.weixin.qq.com/doc/offiaccount/Draft_Box/Add_draft.html

namespace SSCMS.Core.Services
{
    public partial class WxManager
    {
        public async Task<(bool success, string mediaId, string errorMessage)> DraftAddAsync(string accessToken, List<DraftArticle> articles)
        {
            var mediaId = string.Empty;
            var url = $"https://api.weixin.qq.com/cgi-bin/draft/add?access_token={accessToken}";
            var (success, result, errorMessage) = await RestUtils.PostAsync<List<DraftArticle>, string>(url, articles);

            if (success)
            {
                if (StringUtils.Contains(result, "errcode"))
                {
                    success = false;
                    var jsonError = TranslateUtils.JsonDeserialize<JsonResult>(result);
                    errorMessage = $"API 调用发生错误：{jsonError.errmsg}";

                    await _errorLogRepository.AddErrorLogAsync(new Exception(result), "WxManager.DraftAddAsync");
                }
                else
                {
                    var jsonMediaId = TranslateUtils.JsonDeserialize<JsonMediaId>(result);
                    mediaId = jsonMediaId.media_id;
                }
            }
            else
            {
                await _errorLogRepository.AddErrorLogAsync(new Exception(errorMessage), "WxManager.DraftAddAsync");
            }

            return (success, mediaId, errorMessage);
        }
    }
}
