using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ReplyMessageController : ControllerBase
    {
        private const string Route = "wx/replyMessage";
        private const string RouteActionsUpload = "wx/replyMessage/actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IWxManager _wxManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IWxAccountRepository _wxAccountRepository;
        private readonly IWxReplyMessageRepository _wxReplyMessageRepository;
        private readonly IMaterialImageRepository _materialImageRepository;
        private readonly IMaterialAudioRepository _materialAudioRepository;
        private readonly IMaterialVideoRepository _materialVideoRepository;

        public ReplyMessageController(IAuthManager authManager, IPathManager pathManager, IWxManager wxManager, ISiteRepository siteRepository, IWxAccountRepository wxAccountRepository, IWxReplyMessageRepository wxReplyMessageRepository, IMaterialImageRepository materialImageRepository, IMaterialAudioRepository materialAudioRepository, IMaterialVideoRepository materialVideoRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _wxManager = wxManager;
            _siteRepository = siteRepository;
            _wxAccountRepository = wxAccountRepository;
            _wxReplyMessageRepository = wxReplyMessageRepository;
            _materialImageRepository = materialImageRepository;
            _materialAudioRepository = materialAudioRepository;
            _materialVideoRepository = materialVideoRepository;
        }

        public class GetRequest : SiteRequest
        {
            public string ActiveName { get; set; }
        }

        public class GetResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public WxReplyMessage Message { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public string ActiveName { get; set; }
            public MaterialType MaterialType { get; set; }
            public int MaterialId { get; set; }
            public string Text { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public string ActiveName { get; set; }
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
