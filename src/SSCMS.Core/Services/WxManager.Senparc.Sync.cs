using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities.Menu;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class WxManager
    {
        public async Task<string> PushMaterialAsync(string accessTokenOrAppId, MaterialType materialType, int materialId)
        {
            string mediaId = null;

            if (materialType == MaterialType.Message)
            {
                var message = await _materialMessageRepository.GetAsync(materialId);

                var drafts = new List<DraftArticle>();
                foreach (var item in message.Items)
                {
                    var filePath = _pathManager.ParsePath(item.ThumbUrl);
                    var results = await MediaApi.UploadForeverMediaAsync(accessTokenOrAppId, filePath, UploadForeverMediaType.image);
                    var draft = new DraftArticle
                    {
                        thumb_media_id = results.media_id,
                        author = item.Author,
                        title = item.Title,
                        content_source_url = item.ContentSourceUrl,
                        content = item.Content,
                        digest = item.Digest,
                        // show_cover_pic = item.ShowCoverPic ? "1" : "0",
                        // thumb_url = item.ThumbUrl,
                        need_open_comment = 0,
                        only_fans_can_comment = 0
                    };
                    drafts.Add(draft);
                }

                var url = $"https://api.weixin.qq.com/cgi-bin/draft/add?access_token={accessTokenOrAppId}";
                var (success, result, errorMessage) = await RestUtils.PostStringAsync(url, @"{""articles"":" + TranslateUtils.JsonSerialize(drafts) + "}");

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
            }
            else if (materialType == MaterialType.Image)
            {
                var image = await _materialImageRepository.GetAsync(materialId);
                var filePath = _pathManager.ParsePath(image.Url);
                if (FileUtils.IsFileExists(filePath))
                {
                    var result = await MediaApi.UploadForeverMediaAsync(accessTokenOrAppId, filePath, UploadForeverMediaType.image);
                    mediaId = result.media_id;
                }
            }
            else if (materialType == MaterialType.Audio)
            {
                var audio = await _materialAudioRepository.GetAsync(materialId);
                var filePath = _pathManager.ParsePath(audio.Url);
                if (FileUtils.IsFileExists(filePath))
                {
                    var result = await MediaApi.UploadForeverMediaAsync(accessTokenOrAppId, filePath, UploadForeverMediaType.voice);
                    mediaId = result.media_id;
                }
            }
            else if (materialType == MaterialType.Video)
            {
                var video = await _materialVideoRepository.GetAsync(materialId);
                var filePath = _pathManager.ParsePath(video.Url);
                if (FileUtils.IsFileExists(filePath))
                {
                    var result = await MediaApi.UploadForeverMediaAsync(accessTokenOrAppId, filePath, UploadForeverMediaType.voice);
                    mediaId = result.media_id;
                }
            }

            return mediaId;
        }

        public async Task PullMenuAsync(string accessTokenOrAppId, int siteId)
        {
            var result = CommonApi.GetMenu(accessTokenOrAppId);

            if (result == null || result.menu == null || result.menu.button == null) return;

            await _wxMenuRepository.DeleteAllAsync(siteId);

            var json = result.menu.button.ToJson();
            var buttons = TranslateUtils.JsonDeserialize<List<MenuFull_RootButton>>(json);

            var firstTaxis = 1;
            foreach (var button in buttons)
            {

                var first = new WxMenu
                {
                    SiteId = siteId,
                    ParentId = 0,
                    Taxis = firstTaxis++,
                    Text = button.name,
                    MenuType = TranslateUtils.ToEnum(button.type, WxMenuType.View),
                    Key = button.key,
                    Url = button.url,
                    AppId = button.appid,
                    PagePath = button.pagepath,
                    MediaId = button.media_id
                };
                var menuId = await _wxMenuRepository.InsertAsync(first);
                if (button.sub_button != null && button.sub_button.Count > 0)
                {
                    var childTaxis = 1;
                    foreach (var sub in button.sub_button)
                    {
                        var child = new WxMenu
                        {
                            SiteId = siteId,
                            ParentId = menuId,
                            Taxis = childTaxis++,
                            Text = sub.name,
                            MenuType = TranslateUtils.ToEnum(sub.type, WxMenuType.View),
                            Key = sub.key,
                            Url = sub.url,
                            AppId = sub.appid,
                            PagePath = sub.pagepath,
                            MediaId = sub.media_id
                        };
                        await _wxMenuRepository.InsertAsync(child);
                    }
                }
            }
        }

        public async Task PushMenuAsync(string accessTokenOrAppId, int siteId)
        {
            var resultFull = new GetMenuResultFull
            {
                menu = new MenuFull_ButtonGroup
                {
                    button = new List<MenuFull_RootButton>()
                }
            };

            var openMenus = await _wxMenuRepository.GetMenusAsync(siteId);

            foreach (var firstMenu in openMenus.Where(x => x.ParentId == 0))
            {
                var root = new MenuFull_RootButton
                {
                    name = firstMenu.Text,
                    type = firstMenu.MenuType.GetValue(),
                    url = firstMenu.Url,
                    key = firstMenu.Key,
                    sub_button = new List<MenuFull_RootButton>()
                };
                foreach (var child in openMenus.Where(x => x.ParentId == firstMenu.Id))
                {
                    root.sub_button.Add(new MenuFull_RootButton
                    {
                        name = child.Text,
                        type = child.MenuType.GetValue(),
                        url = child.Url,
                        key = child.Key
                    });
                }

                resultFull.menu.button.Add(root);
            }

            var buttonGroup = CommonApi.GetMenuFromJsonResult(resultFull, new ButtonGroup()).menu;
            var result = CommonApi.CreateMenu(accessTokenOrAppId, buttonGroup);

            if (result.errmsg != "ok")
            {
                throw new Exception(result.errmsg);
            }
        }
    }
}
