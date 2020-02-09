using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    
    [RoutePrefix("pages/cms/contents/contentsLayerWord")]
    public partial class PagesContentsLayerWordController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] ChannelRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId,
                    Constants.ChannelPermissions.ContentAdd))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            var channelInfo = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return Request.BadRequest<GetResult>("无法确定内容对应的栏目");

            var (isChecked, checkedLevel) = await CheckManager.GetUserCheckLevelAsync(auth.AdminPermissionsImpl, site, request.SiteId);
            var checkedLevels = CheckManager.GetCheckedLevels(site, isChecked, checkedLevel, false);

            return new GetResult
            {
                Value = checkedLevels,
                CheckedLevel = CheckManager.LevelInt.CaoGao
            };
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<UploadResult> Upload([FromUri] ChannelRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId,
                    Constants.ChannelPermissions.ContentAdd))
            {
                return Request.Unauthorized<UploadResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);

            var fileName = auth.HttpRequest["fileName"];
            var fileCount = auth.HttpRequest.Files.Count;
            if (fileCount == 0)
            {
                return Request.BadRequest<UploadResult>("请选择有效的文件上传");
            }

            var file = auth.HttpRequest.Files[0];

            if (string.IsNullOrEmpty(fileName)) fileName = Path.GetFileName(file.FileName);

            var extendName = fileName.Substring(fileName.LastIndexOf(".", StringComparison.Ordinal)).ToLower();
            if (!StringUtils.EqualsIgnoreCase(extendName, ".doc") && !StringUtils.EqualsIgnoreCase(extendName, ".docx") && !StringUtils.EqualsIgnoreCase(extendName, ".wps"))
            {
                return Request.BadRequest<UploadResult>("文件只能是 Word 格式，请选择有效的文件上传!");
            }

            var filePath = PathUtils.GetTemporaryFilesPath(fileName);
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            file.SaveAs(filePath);

            var url = await PageUtility.GetSiteUrlByPhysicalPathAsync(site, filePath, true);

            return new UploadResult
            {
                Name = fileName,
                Url = url
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ObjectResult<List<int>>> Submit([FromBody] SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId,
                    Constants.ChannelPermissions.ContentAdd))
            {
                return Request.Unauthorized<ObjectResult<List<int>>>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<ObjectResult<List<int>>>();

            var channelInfo = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return Request.BadRequest<ObjectResult<List<int>>>("无法确定内容对应的栏目");

            var styleList = await DataProvider.TableStyleRepository.GetContentStyleListAsync(site, channelInfo);
            var isChecked = request.CheckedLevel >= site.CheckContentLevel;

            var contentIdList = new List<int>();
            foreach (var fileName in request.FileNames)
            {
                if (string.IsNullOrEmpty(fileName)) continue;

                var filePath = PathUtils.GetTemporaryFilesPath(fileName);
                var (title, content) = await WordManager.GetWordAsync(site, request.IsFirstLineTitle, request.IsClearFormat, request.IsFirstLineIndent, request.IsClearFontSize, request.IsClearFontFamily, request.IsClearImages, filePath);

                if (string.IsNullOrEmpty(title)) continue;

                var dict = await BackgroundInputTypeParser.SaveAttributesAsync(site, styleList, new NameValueCollection(), ContentAttribute.AllAttributes.Value);

                var contentInfo = new Content(dict)
                {
                    ChannelId = channelInfo.Id,
                    SiteId = request.SiteId,
                    AdminId = auth.AdminId,
                    LastEditAdminId = auth.AdminId,
                    AddDate = DateTime.Now,
                    Checked = isChecked,
                    CheckedLevel = request.CheckedLevel,
                    Title = title,
                    LastEditDate = DateTime.Now
                };

                contentInfo.Set(ContentAttribute.Content, content);
                await DataProvider.ContentRepository.InsertAsync(site, channelInfo, contentInfo);
                contentIdList.Add(contentInfo.Id);
            }

            if (isChecked)
            {
                foreach (var contentId in contentIdList)
                {
                    await CreateManager.CreateContentAsync(request.SiteId, channelInfo.Id, contentId);
                }
                await CreateManager.TriggerContentChangedEventAsync(request.SiteId, channelInfo.Id);
            }

            return new ObjectResult<List<int>>
            {
                Value = contentIdList
            };
        }
    }
}
