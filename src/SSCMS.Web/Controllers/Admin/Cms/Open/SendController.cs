using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Open
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SendController : ControllerBase
    {
        private const string Route = "cms/open/send";
        private const string RouteActionsPreview = "cms/open/send/actions/preview";
        private const string RouteActionsUpload = "cms/open/send/actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IOpenManager _openManager;
        private readonly ITaskManager _taskManager;
        private readonly IPathManager _pathManager;
        private readonly IOpenAccountRepository _openAccountRepository;
        private readonly IMaterialMessageRepository _materialMessageRepository;
        private readonly IMaterialImageRepository _materialImageRepository;
        private readonly IMaterialAudioRepository _materialAudioRepository;
        private readonly IMaterialVideoRepository _materialVideoRepository;
        private readonly ISiteRepository _siteRepository;

        public SendController(IAuthManager authManager, IOpenManager openManager, ITaskManager taskManager, IPathManager pathManager, IOpenAccountRepository openAccountRepository, IMaterialMessageRepository materialMessageRepository, IMaterialImageRepository materialImageRepository, IMaterialAudioRepository materialAudioRepository, IMaterialVideoRepository materialVideoRepository, ISiteRepository siteRepository)
        {
            _authManager = authManager;
            _openManager = openManager;
            _taskManager = taskManager;
            _pathManager = pathManager;
            _openAccountRepository = openAccountRepository;
            _materialMessageRepository = materialMessageRepository;
            _materialImageRepository = materialImageRepository;
            _materialAudioRepository = materialAudioRepository;
            _materialVideoRepository = materialVideoRepository;
            _siteRepository = siteRepository;
        }

        public class GetRequest : SiteRequest
        {
            public int MessageId { get; set; }
        }

        public class Tag
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Count { get; set; }
        }

        public class GetResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public IEnumerable<Tag> Tags { get; set; }
            public MaterialMessage Message { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public MaterialType MaterialType { get; set; }
            public int MaterialId { get; set; }
            public string Text { get; set; }
            public bool IsToAll { get; set; }
            public int TagId { get; set; }
            public bool IsTiming { get; set; }
            public bool IsToday { get; set; }
            public int Hour { get; set; }
            public int Minute { get; set; }
        }

        public class PreviewRequest : SiteRequest
        {
            public MaterialType MaterialType { get; set; }
            public int MaterialId { get; set; }
            public string Text { get; set; }
            public string WxNames { get; set; }
        }

        public class UploadRequest : SiteRequest
        {
            public MaterialType MaterialType { get; set; }
        }

        public class UploadResult
        {
            public MaterialType MaterialType { get; set; }
            public MaterialImage Image { get; set; }
            public MaterialAudio Audio { get; set; }
            public MaterialVideo Video { get; set; }
        }
    }
}
