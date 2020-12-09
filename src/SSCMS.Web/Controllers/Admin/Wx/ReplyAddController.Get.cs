using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class ReplyAddController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.WxReply))
            {
                return Unauthorized();
            }

            string ruleName = null;
            var random = true;
            List<WxReplyKeyword> keywords = null;
            List<WxReplyMessage> messages = null;

            var (success, _, errorMessage) = await _wxManager.GetAccessTokenAsync(request.SiteId);
            if (success)
            {
                if (request.RuleId > 0)
                {
                    var rule = await _wxReplyRuleRepository.GetAsync(request.RuleId);
                    ruleName = rule.RuleName;
                    random = rule.Random;
                    keywords = await _wxReplyKeywordRepository.GetKeywordsAsync(request.SiteId, request.RuleId);
                    messages = await _wxManager.GetMessagesAsync(request.SiteId, request.RuleId);
                }
            }

            return new GetResult
            {
                Success = success,
                ErrorMessage = errorMessage,
                RuleName = ruleName,
                Random = random,
                Keywords = keywords,
                Messages = messages
            };
        }
    }
}