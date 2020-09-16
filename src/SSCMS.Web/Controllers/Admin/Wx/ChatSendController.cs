using System.Collections.Generic;
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
    public partial class ChatSendController : ControllerBase
    {
        private const string Route = "wx/chatSend";
        private const string RouteActionsUpload = "wx/chatSend/actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IWxManager _wxManager;
        private readonly IPathManager _pathManager;
        private readonly IWxChatRepository _wxChatRepository;
        private readonly IMaterialImageRepository _materialImageRepository;
        private readonly IMaterialAudioRepository _materialAudioRepository;
        private readonly IMaterialVideoRepository _materialVideoRepository;
        private readonly ISiteRepository _siteRepository;

        public ChatSendController(IAuthManager authManager, IWxManager wxManager, IPathManager pathManager, IWxChatRepository wxChatRepository, IMaterialImageRepository materialImageRepository, IMaterialAudioRepository materialAudioRepository, IMaterialVideoRepository materialVideoRepository, ISiteRepository siteRepository)
        {
            _authManager = authManager;
            _wxManager = wxManager;
            _pathManager = pathManager;
            _wxChatRepository = wxChatRepository;
            _materialImageRepository = materialImageRepository;
            _materialAudioRepository = materialAudioRepository;
            _materialVideoRepository = materialVideoRepository;
            _siteRepository = siteRepository;
        }

        public class GetRequest : SiteRequest
        {
            public string OpenId { get; set; }
        }

        public class GetResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public WxUser User { get; set; }
            public List<WxChat> Chats { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public string OpenId { get; set; }
            public MaterialType MaterialType { get; set; }
            public int MaterialId { get; set; }
            public string Text { get; set; }
        }

        public class SubmitResult
        {
            public List<WxChat> Chats { get; set; }
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
