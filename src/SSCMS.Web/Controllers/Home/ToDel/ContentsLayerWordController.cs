using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Utils;
using SSCMS.Core.Utils.Office;
using SSCMS.Dto;
using SSCMS.Extensions;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home.ToDel
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.User)]
    [Route(Constants.ApiHomePrefix + "todel/")]
    public partial class ContentsLayerWordController : ControllerBase
    {
        private const string Route = "contentsLayerWord";
        private const string RouteUpload = "contentsLayerWord/actions/upload";

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
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ContentPermissions.Add))
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
        public async Task<ActionResult<NameTitle>> Upload([FromQuery] ChannelRequest request, [FromForm] IFormFile file)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ContentPermissions.Add))
            {
                return Unauthorized();
            }

            if (file == null)
            {
                return this.Error("请选择有效的文件上传");
            }

            var title = PathUtils.GetFileNameWithoutExtension(file.FileName);
            var fileName = PathUtils.GetUploadFileName(file.FileName, true);
            var extendName = PathUtils.GetExtension(fileName);

            if (!FileUtils.IsWord(extendName))
            {
                return this.Error("文件只能是 Word 格式，请选择有效的文件上传!");
            }

            var filePath = _pathManager.GetTemporaryFilesPath(fileName);
            await _pathManager.UploadAsync(file, filePath);

            return new NameTitle
            {
                FileName = fileName,
                Title = title
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.ContentPermissions.Add))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var tableName = _channelRepository.GetTableName(site, channel);
            var styleList = await _tableStyleRepository.GetContentStylesAsync(channel, tableName);
            var isChecked = request.CheckedLevel >= site.CheckContentLevel;
            var adminId = _authManager.AdminId;
            var userId = _authManager.UserId;

            var contentIdList = new List<int>();
            foreach (var file in request.Files)
            {
                if (string.IsNullOrEmpty(file.FileName) || string.IsNullOrEmpty(file.Title)) continue;

                var filePath = _pathManager.GetTemporaryFilesPath(file.FileName);
                var (title, imageUrl, content) = await WordManager.GetWordAsync(_pathManager, site, request.IsFirstLineTitle, request.IsClearFormat, request.IsFirstLineIndent, request.IsClearFontSize, request.IsClearFontFamily, request.IsClearImages, filePath, file.Title);

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
                contentInfo.ImageUrl = imageUrl;
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
