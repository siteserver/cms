﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class ReplyController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.WxReply))
            {
                return Unauthorized();
            }

            List<WxReplyRule> rules = null;
            var count = 0;

            var site = await _siteRepository.GetAsync(request.SiteId);
            var isWxEnabled = await _wxManager.IsEnabledAsync(site);

            if (isWxEnabled)
            {
                count = await _wxReplyRuleRepository.GetCount(request.SiteId, request.Keyword);
                rules = await _wxReplyRuleRepository.GetRulesAsync(request.SiteId, request.Keyword, request.Page, request.PerPage);

                foreach (var rule in rules)
                {
                    rule.Keywords = await _wxReplyKeywordRepository.GetKeywordsAsync(request.SiteId, rule.Id);
                    rule.Messages = await _wxManager.GetMessagesAsync(request.SiteId, rule.Id);
                }
            }

            return new GetResult
            {
                IsWxEnabled = isWxEnabled,
                Rules = rules,
                Count = count
            };
        }
    }
}
