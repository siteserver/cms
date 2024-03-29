﻿using System.Collections.Generic;
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
        private readonly ISiteRepository _siteRepository;
        private readonly IWxReplyKeywordRepository _wxReplyKeywordRepository;
        private readonly IWxReplyMessageRepository _wxReplyMessageRepository;
        private readonly IWxReplyRuleRepository _wxReplyRuleRepository;

        public ReplyAddController(IAuthManager authManager, IWxManager wxManager, ISiteRepository siteRepository, IWxReplyKeywordRepository wxReplyKeywordRepository, IWxReplyMessageRepository wxReplyMessageRepository, IWxReplyRuleRepository wxReplyRuleRepository)
        {
            _authManager = authManager;
            _wxManager = wxManager;
            _siteRepository = siteRepository;
            _wxReplyKeywordRepository = wxReplyKeywordRepository;
            _wxReplyMessageRepository = wxReplyMessageRepository;
            _wxReplyRuleRepository = wxReplyRuleRepository;
        }

        public class GetRequest : SiteRequest
        {
            public int RuleId { get; set; }
        }

        public class GetResult
        {
            public bool IsWxEnabled { get; set; }
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
