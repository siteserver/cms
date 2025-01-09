using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Enums;
using SSCMS.Utils;

// https://developers.weixin.qq.com/doc/offiaccount/Draft_Box/Add_draft.html

namespace SSCMS.Core.Services
{
    public partial class WxManager
    {
        public async Task<(bool success, string mediaId, string errorMessage)> DraftAddAsync(string accessToken, int messageId)
        {
            var message = await _materialMessageRepository.GetAsync(messageId);
            var articles = new List<DraftArticle>();
            foreach (var item in message.Items)
            {
                var article = new DraftArticle
                {
                    thumb_media_id = item.ThumbMediaId,
                    author = item.Author,
                    title = item.Title,
                    content_source_url = item.ContentSourceUrl,
                    content = item.Content,
                    digest = item.Digest,
                    // show_cover_pic = item.ShowCoverPic ? "1" : "0",
                    // thumb_url = item.ThumbUrl,
                    need_open_comment = item.CommentType == CommentType.Block ? 0 : 1,
                    only_fans_can_comment = item.CommentType == CommentType.OnlyFans ? 1 : 0
                };
                articles.Add(article);
            }

            var mediaId = string.Empty;
            var url = $"https://api.weixin.qq.com/cgi-bin/draft/add?access_token={accessToken}";
            var (success, result, errorMessage) = await RestUtils.PostStringAsync(url, TranslateUtils.JsonSerialize(articles));

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
