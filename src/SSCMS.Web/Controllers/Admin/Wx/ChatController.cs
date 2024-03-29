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
    public partial class ChatController : ControllerBase
    {
        private const string Route = "wx/chat";
        private const string RouteActionsStar = "wx/chat/actions/star";

        private readonly IAuthManager _authManager;
        private readonly IWxManager _wxManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IWxChatRepository _wxChatRepository;

        public ChatController(IAuthManager authManager, IWxManager wxManager, ISiteRepository siteRepository, IWxChatRepository wxChatRepository)
        {
            _authManager = authManager;
            _wxManager = wxManager;
            _siteRepository = siteRepository;
            _wxChatRepository = wxChatRepository;
        }

        public class GetRequest : SiteRequest
        {
            public bool Star { get; set; }
            public string Keyword { get; set; }
            public int Page { get; set; }
            public int PerPage { get; set; }
        }

        public class GetResult
        {
            public bool IsWxEnabled { get; set; }
            public IEnumerable<WxChat> Chats { get; set; }
            public int Count { get; set; }
        }

        public class StarRequest : SiteRequest
        {
            public int ChatId { get; set; }
            public bool Star { get; set; }
        }
    }
}
