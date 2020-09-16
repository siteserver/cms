using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ReplyAddController : ControllerBase
    {
        private const string Route = "wx/replyAdd";

        private readonly IAuthManager _authManager;
        private readonly IWxManager _wxManager;
        private readonly IWxReplyKeywordRepository _wxReplyKeywordRepository;
        private readonly IWxReplyMessageRepository _wxReplyMessageRepository;
        private readonly IWxReplyRuleRepository _wxReplyRuleRepository;
        private readonly IMaterialMessageRepository _materialMessageRepository;
        private readonly IMaterialImageRepository _materialImageRepository;
        private readonly IMaterialAudioRepository _materialAudioRepository;
        private readonly IMaterialVideoRepository _materialVideoRepository;

        public ReplyAddController(IAuthManager authManager, IWxManager wxManager, IWxReplyKeywordRepository wxReplyKeywordRepository, IWxReplyMessageRepository wxReplyMessageRepository, IWxReplyRuleRepository wxReplyRuleRepository, IMaterialMessageRepository materialMessageRepository, IMaterialImageRepository materialImageRepository, IMaterialAudioRepository materialAudioRepository, IMaterialVideoRepository materialVideoRepository)
        {
            _authManager = authManager;
            _wxManager = wxManager;
            _wxReplyKeywordRepository = wxReplyKeywordRepository;
            _wxReplyMessageRepository = wxReplyMessageRepository;
            _wxReplyRuleRepository = wxReplyRuleRepository;
            _materialMessageRepository = materialMessageRepository;
            _materialImageRepository = materialImageRepository;
            _materialAudioRepository = materialAudioRepository;
            _materialVideoRepository = materialVideoRepository;
        }

        public class GetRequest : SiteRequest
        {
            public int RuleId { get; set; }
        }

        public class GetResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public string RuleName { get; set; }
            public bool Random { get; set; }
            public List<WxReplyKeyword> Keywords { get; set; }
            public List<WxReplyMessage> Messages { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public int RuleId { get; set; }
            public string RuleName { get; set; }
            public bool Random { get; set; }
            public List<WxReplyKeyword> Keywords { get; set; }
            public List<WxReplyMessage> Messages { get; set; }
        }
    }
}
