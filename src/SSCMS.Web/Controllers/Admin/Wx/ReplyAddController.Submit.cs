using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class ReplyAddController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.WxReply))
            {
                return Unauthorized();
            }

            if (request.RuleId > 0)
            {
                var rule = await _wxReplyRuleRepository.GetAsync(request.RuleId);
                rule.RuleName = request.RuleName;
                rule.Random = request.Random;
                await _wxReplyRuleRepository.UpdateAsync(rule);

                await _wxReplyKeywordRepository.DeleteAllAsync(request.SiteId, request.RuleId);
                await _wxReplyMessageRepository.DeleteAllAsync(request.SiteId, request.RuleId);

                foreach (var keyword in request.Keywords)
                {
                    await _wxReplyKeywordRepository.InsertAsync(new WxReplyKeyword
                    {
                        SiteId = request.SiteId,
                        RuleId = request.RuleId,
                        Exact = keyword.Exact,
                        Text = keyword.Text
                    });
                }

                foreach (var message in request.Messages)
                {
                    await _wxReplyMessageRepository.InsertAsync(new WxReplyMessage
                    {
                        SiteId = request.SiteId,
                        RuleId = request.RuleId,
                        MaterialType = message.MaterialType,
                        MaterialId = message.MaterialId,
                        Text = message.Text
                    });
                }
            }
            else
            {
                var rule = new WxReplyRule
                {
                    SiteId = request.SiteId,
                    RuleName = request.RuleName,
                    Random = request.Random
                };
                var ruleId = await _wxReplyRuleRepository.InsertAsync(rule);

                foreach (var keyword in request.Keywords)
                {
                    await _wxReplyKeywordRepository.InsertAsync(new WxReplyKeyword
                    {
                        SiteId = request.SiteId,
                        RuleId = ruleId,
                        Exact = keyword.Exact,
                        Text = keyword.Text
                    });
                }

                foreach (var message in request.Messages)
                {
                    await _wxReplyMessageRepository.InsertAsync(new WxReplyMessage
                    {
                        SiteId = request.SiteId,
                        RuleId = ruleId,
                        MaterialType = message.MaterialType,
                        MaterialId = message.MaterialId,
                        Text = message.Text
                    });
                }
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
