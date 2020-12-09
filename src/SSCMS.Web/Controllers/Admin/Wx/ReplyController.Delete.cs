using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class ReplyController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<DeleteResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.WxReply))
            {
                return Unauthorized();
            }

            await _wxReplyMessageRepository.DeleteAllAsync(request.SiteId, request.RuleId);
            await _wxReplyKeywordRepository.DeleteAllAsync(request.SiteId, request.RuleId);
            await _wxReplyRuleRepository.DeleteAsync(request.RuleId);

            var count = await _wxReplyRuleRepository.GetCount(request.SiteId, request.Keyword);
            var rules = await _wxReplyRuleRepository.GetRulesAsync(request.SiteId, request.Keyword, request.Page, request.PerPage);

            foreach (var rule in rules)
            {
                rule.Keywords = await _wxReplyKeywordRepository.GetKeywordsAsync(request.SiteId, rule.Id);
                rule.Messages = await _wxManager.GetMessagesAsync(request.SiteId, rule.Id);
            }

            return new DeleteResult
            {
                Rules = rules,
                Count = count
            };
        }
    }
}
