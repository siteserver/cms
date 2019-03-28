using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Http;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.Database.Attributes;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [RoutePrefix("pages/cms/contentsLayerWord")]
    public class PagesContentsLayerWordController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var rest = new Rest(Request);

                var siteId = rest.GetQueryInt("siteId");
                var channelId = rest.GetQueryInt("channelId");

                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentAdd))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var isChecked = CheckManager.GetUserCheckLevel(rest.AdminPermissionsImpl, siteInfo, siteId, out var checkedLevel);
                var checkedLevels = CheckManager.GetCheckedLevels(siteInfo, isChecked, checkedLevel, false);

                return Ok(new
                {
                    Value = checkedLevels,
                    CheckedLevel = CheckManager.LevelInt.CaoGao
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteUpload)]
        public IHttpActionResult Upload()
        {
            try
            {
#pragma warning disable CS0612 // '“RequestImpl”已过时
                var request = new RequestImpl(HttpContext.Current.Request);
#pragma warning restore CS0612 // '“RequestImpl”已过时

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasChannelPermissions(siteId, channelId,
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
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var rest = new Rest(Request);

                var siteId = rest.GetPostInt("siteId");
                var channelId = rest.GetPostInt("channelId");
                var isFirstLineTitle = rest.GetPostBool("isFirstLineTitle");
                var isFirstLineRemove = rest.GetPostBool("isFirstLineRemove");
                var isClearFormat = rest.GetPostBool("isClearFormat");
                var isFirstLineIndent = rest.GetPostBool("isFirstLineIndent");
                var isClearFontSize = rest.GetPostBool("isClearFontSize");
                var isClearFontFamily = rest.GetPostBool("isClearFontFamily");
                var isClearImages = rest.GetPostBool("isClearImages");
                var checkedLevel = rest.GetPostInt("checkedLevel");
                var fileNames = TranslateUtils.StringCollectionToStringList(rest.GetPostString("fileNames"));

                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentAdd))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
                var styleInfoList = TableStyleManager.GetContentStyleInfoList(siteInfo, channelInfo);
                var isChecked = checkedLevel >= siteInfo.CheckContentLevel;

                var contentIdList = new List<int>();

                foreach (var fileName in fileNames)
                {
                    if (string.IsNullOrEmpty(fileName)) continue;

                    var formCollection = WordUtils.GetWordNameValueCollection(siteId, isFirstLineTitle, isFirstLineRemove, isClearFormat, isFirstLineIndent, isClearFontSize, isClearFontFamily, isClearImages, fileName);

                    if (string.IsNullOrEmpty(formCollection[ContentAttribute.Title])) continue;

                    var dict = BackgroundInputTypeParser.SaveAttributes(siteInfo, styleInfoList, formCollection, ContentAttribute.AllAttributes.Value);

                    var contentInfo = new ContentInfo(dict)
                    {
                        ChannelId = channelInfo.Id,
                        SiteId = siteId,
                        AddUserName = rest.AdminName,
                        AddDate = DateTime.Now
                    };

                    contentInfo.LastEditUserName = contentInfo.AddUserName;
                    contentInfo.LastEditDate = contentInfo.AddDate;
                    contentInfo.Checked = isChecked;
                    contentInfo.CheckedLevel = checkedLevel;

                    contentInfo.Title = formCollection[ContentAttribute.Title];

                    contentInfo.Id = DataProvider.ContentRepository.Insert(tableName, siteInfo, channelInfo, contentInfo);

                    contentIdList.Add(contentInfo.Id);
                }

                if (isChecked)
                {
                    foreach (var contentId in contentIdList)
                    {
                        CreateManager.CreateContent(siteId, channelInfo.Id, contentId);
                    }
                    CreateManager.TriggerContentChangedEvent(siteId, channelInfo.Id);
                }

                return Ok(new
                {
                    Value = contentIdList
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }
    }
}
