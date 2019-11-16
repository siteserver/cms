using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Home
{
    [OpenApiIgnore]
    [RoutePrefix("home/contentsLayerWord")]
    public class HomeContentsLayerWordController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = await AuthenticatedRequest.GetRequestAsync();

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");

                if (!request.IsUserLoggin ||
                    !await request.UserPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentAdd))
                {
                    return Unauthorized();
                }

                var site = await SiteManager.GetSiteAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var (isChecked, checkedLevel) = await CheckManager.GetUserCheckLevelAsync(request.AdminPermissionsImpl, site, siteId);
                var checkedLevels = CheckManager.GetCheckedLevels(site, isChecked, checkedLevel, false);

                return Ok(new
                {
                    Value = checkedLevels,
                    CheckedLevel = CheckManager.LevelInt.CaoGao
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<IHttpActionResult> Upload()
        {
            try
            {
                var request = await AuthenticatedRequest.GetRequestAsync();

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");

                if (!request.IsUserLoggin ||
                    !await request.UserPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentAdd))
                {
                    return Unauthorized();
                }

                var fileName = request.HttpRequest["fileName"];

                var fileCount = request.HttpRequest.Files.Count;

                string filePath = null;

                if (fileCount > 0)
                {
                    var file = request.HttpRequest.Files[0];

                    if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

                    var extendName = fileName.Substring(fileName.LastIndexOf(".", StringComparison.Ordinal)).ToLower();
                    if (extendName == ".doc" || extendName == ".docx")
                    {
                        filePath = PathUtils.GetTemporaryFilesPath(fileName);
                        DirectoryUtils.CreateDirectoryIfNotExists(filePath);
                        file.SaveAs(filePath);
                    }
                }

                FileInfo fileInfo = null;
                if (!string.IsNullOrEmpty(filePath))
                {
                    fileInfo = new FileInfo(filePath);
                }
                if (fileInfo != null)
                {
                    return Ok(new
                    {
                        fileName,
                        length = fileInfo.Length,
                        ret = 1
                    });
                }

                return Ok(new
                {
                    ret = 0
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            try
            {
                var request = await AuthenticatedRequest.GetRequestAsync();

                var siteId = request.GetPostInt("siteId");
                var channelId = request.GetPostInt("channelId");
                var isFirstLineTitle = request.GetPostBool("isFirstLineTitle");
                var isFirstLineRemove = request.GetPostBool("isFirstLineRemove");
                var isClearFormat = request.GetPostBool("isClearFormat");
                var isFirstLineIndent = request.GetPostBool("isFirstLineIndent");
                var isClearFontSize = request.GetPostBool("isClearFontSize");
                var isClearFontFamily = request.GetPostBool("isClearFontFamily");
                var isClearImages = request.GetPostBool("isClearImages");
                var checkedLevel = request.GetPostInt("checkedLevel");
                var fileNames = TranslateUtils.StringCollectionToStringList(request.GetPostString("fileNames"));

                if (!request.IsUserLoggin ||
                    !await request.UserPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentAdd))
                {
                    return Unauthorized();
                }

                var site = await SiteManager.GetSiteAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var tableName = await ChannelManager.GetTableNameAsync(site, channelInfo);
                var styleList = await TableStyleManager.GetContentStyleListAsync(site, channelInfo);
                var isChecked = checkedLevel >= site.CheckContentLevel;

                var contentIdList = new List<int>();

                foreach (var fileName in fileNames)
                {
                    if (string.IsNullOrEmpty(fileName)) continue;

                    var formCollection = await WordUtils.GetWordNameValueCollectionAsync(siteId, isFirstLineTitle, isFirstLineRemove, isClearFormat, isFirstLineIndent, isClearFontSize, isClearFontFamily, isClearImages, fileName);

                    if (string.IsNullOrEmpty(formCollection[ContentAttribute.Title])) continue;

                    var dict = await BackgroundInputTypeParser.SaveAttributesAsync(site, styleList, formCollection, ContentAttribute.AllAttributes.Value);

                    var contentInfo = new Content(dict)
                    {
                        ChannelId = channelInfo.Id,
                        SiteId = siteId,
                        AddUserName = request.AdminName,
                        AddDate = DateTime.Now,
                        SourceId = SourceManager.User,
                        AdminId = request.AdminId,
                        UserId = request.UserId,
                        Checked = isChecked,
                        CheckedLevel = checkedLevel
                    };

                    contentInfo.LastEditUserName = contentInfo.AddUserName;
                    contentInfo.LastEditDate = contentInfo.AddDate;

                    contentInfo.Title = formCollection[ContentAttribute.Title];

                    contentInfo.Id = await DataProvider.ContentDao.InsertAsync(tableName, site, channelInfo, contentInfo);

                    contentIdList.Add(contentInfo.Id);
                }

                if (isChecked)
                {
                    foreach (var contentId in contentIdList)
                    {
                        await CreateManager.CreateContentAsync(siteId, channelInfo.Id, contentId);
                    }
                    await CreateManager.TriggerContentChangedEventAsync(siteId, channelInfo.Id);
                }

                return Ok(new
                {
                    Value = contentIdList
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }
    }
}
