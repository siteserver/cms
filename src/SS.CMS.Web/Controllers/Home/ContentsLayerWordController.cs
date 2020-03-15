using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Core.Office;
using SS.CMS.Extensions;

namespace SS.CMS.Web.Controllers.Home
{
    [Route("home/contentsLayerWord")]
    public partial class ContentsLayerWordController : ControllerBase
    {
        private const string Route = "";
        private const string RouteUpload = "actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICreateManager _createManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ITableStyleRepository _tableStyleRepository;

        public ContentsLayerWordController(IAuthManager authManager, IPathManager pathManager, ICreateManager createManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _createManager = createManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _tableStyleRepository = tableStyleRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] ChannelRequest request)
        {
            if (!await _authManager.IsUserAuthenticatedAsync() ||
                !await _authManager.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentAdd))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var (isChecked, checkedLevel) = await CheckManager.GetUserCheckLevelAsync(_authManager, site, request.SiteId);
            var checkedLevels = CheckManager.GetCheckedLevels(site, isChecked, checkedLevel, false);

            return new GetResult
            {
                CheckedLevels = checkedLevels,
                CheckedLevel = CheckManager.LevelInt.CaoGao
            };
        }

        [HttpPost, Route(RouteUpload)]
        public async Task<ActionResult<UploadResult>> Upload([FromQuery] ChannelRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.IsUserAuthenticatedAsync() ||
                !await _authManager.HasChannelPermissionsAsync(request.SiteId, request.ChannelId,
                    Constants.ChannelPermissions.ContentAdd))
            {
                return Unauthorized();
            }

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var fileName = Path.GetFileName(file.FileName);

            if (!PathUtils.IsExtension(PathUtils.GetExtension(fileName), ".doc", ".docx", ".wps"))
            {
                return this.Error("文件只能是 Image 格式，请选择有效的文件上传!");
            }

            var filePath = _pathManager.GetTemporaryFilesPath(fileName);
            await _pathManager.UploadAsync(file, filePath);

            FileInfo fileInfo = null;
            if (!string.IsNullOrEmpty(filePath))
            {
                fileInfo = new FileInfo(filePath);
            }
            if (fileInfo != null)
            {
                return new UploadResult
                {
                    FileName = fileName,
                    Length = fileInfo.Length,
                    Ret = 1
                };
            }

            return new UploadResult
            {
                Ret = 0
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.IsUserAuthenticatedAsync() ||
                !await _authManager.HasChannelPermissionsAsync(request.SiteId, request.ChannelId,
                    Constants.ChannelPermissions.ContentAdd))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var tableName = _channelRepository.GetTableName(site, channel);
            var styleList = await _tableStyleRepository.GetContentStyleListAsync(channel, tableName);
            var isChecked = request.CheckedLevel >= site.CheckContentLevel;
            var adminId = await _authManager.GetAdminIdAsync();
            var userId = await _authManager.GetUserIdAsync();

            var contentIdList = new List<int>();
            foreach (var fileName in request.FileNames)
            {
                if (string.IsNullOrEmpty(fileName)) continue;

                var filePath = _pathManager.GetTemporaryFilesPath(fileName);
                var (title, content) = await WordManager.GetWordAsync(_pathManager, site, request.IsFirstLineTitle, request.IsClearFormat, request.IsFirstLineIndent, request.IsClearFontSize, request.IsClearFontFamily, request.IsClearImages, filePath);

                if (string.IsNullOrEmpty(title)) continue;

                var dict = await ColumnsManager.SaveAttributesAsync(_pathManager, site, styleList, new NameValueCollection(), ColumnsManager.MetadataAttributes.Value);

                var contentInfo = new Content
                {
                    ChannelId = channel.Id,
                    SiteId = request.SiteId,
                    AddDate = DateTime.Now,
                    SourceId = SourceManager.User,
                    AdminId = adminId,
                    UserId = userId,
                    LastEditAdminId = adminId,
                    Checked = isChecked,
                    CheckedLevel = request.CheckedLevel
                };
                contentInfo.LoadDict(dict);

                contentInfo.Title = title;
                contentInfo.Body = content;

                contentInfo.Id = await _contentRepository.InsertAsync(site, channel, contentInfo);

                contentIdList.Add(contentInfo.Id);
            }

            if (isChecked)
            {
                foreach (var contentId in contentIdList)
                {
                    await _createManager.CreateContentAsync(request.SiteId, channel.Id, contentId);
                }
                await _createManager.TriggerContentChangedEventAsync(request.SiteId, channel.Id);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
