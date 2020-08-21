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
    public partial class ReplyController : ControllerBase
    {
        private const string Route = "wx/reply";

        private readonly IAuthManager _authManager;
        private readonly IWxManager _wxManager;
        private readonly IWxReplyRuleRepository _wxReplyRuleRepository;
        private readonly IWxReplyKeywordRepository _wxReplyKeywordRepository;
        private readonly IWxReplyMessageRepository _wxReplyMessageRepository;

        public ReplyController(IAuthManager authManager, IWxManager wxManager, IWxReplyRuleRepository wxReplyRuleRepository, IWxReplyKeywordRepository wxReplyKeywordRepository, IWxReplyMessageRepository wxReplyMessageRepository)
        {
            _authManager = authManager;
            _wxManager = wxManager;
            _wxReplyRuleRepository = wxReplyRuleRepository;
            _wxReplyKeywordRepository = wxReplyKeywordRepository;
            _wxReplyMessageRepository = wxReplyMessageRepository;
        }

        public class GetRequest : SiteRequest
        {
            public string Keyword { get; set; }
            public int Page { get; set; }
            public int PerPage { get; set; }
        }

        public class GetResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public IEnumerable<WxReplyRule> Rules { get; set; }
            public int Count { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public int RuleId { get; set; }
            public string Keyword { get; set; }
            public int Page { get; set; }
            public int PerPage { get; set; }
        }

        public class DeleteResult
        {
            public IEnumerable<WxReplyRule> Rules { get; set; }
            public int Count { get; set; }
        }
    }
}
