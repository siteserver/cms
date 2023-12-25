using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class UsersController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.WxUsers))
            {
                return Unauthorized();
            }

            var tags = new List<WxUserTag>();
            var total = 0;
            var count = 0;
            var users = new List<WxUser>();

            var site = await _siteRepository.GetAsync(request.SiteId);
            var isWxEnabled = await _wxManager.IsEnabledAsync(site);

            if (isWxEnabled)
            {
                var account = await _wxManager.GetAccountAsync(request.SiteId);
                var (success, token, errorMessage) = await _wxManager.GetAccessTokenAsync(account);
                if (!success)
                {
                    return this.Error(errorMessage);
                }

                if (account.MpType == WxMpType.Subscription || account.MpType == WxMpType.Service)
                {
                    return this.Error(_wxManager.GetErrorUnAuthenticated(account));
                }

                if (request.Init)
                {
                    // var openIds = await _wxManager.GetUserOpenIdsAsync(token, false);
                    var openIds = new List<string>();
                    (success, errorMessage) = await _wxManager.UserGetAsync(token, openIds);
                    if (!success)
                    {
                        return this.Error(errorMessage);
                    }

                    var dbOpenIds = await _wxUserRepository.GetAllOpenIds(request.SiteId);

                    var inserts = openIds.Where(openId => !dbOpenIds.Contains(openId)).ToList();
                    var deletes = dbOpenIds.Where(dbOpenId => !openIds.Contains(dbOpenId)).ToList();
                    foreach (var wxUser in await _wxManager.GetUsersAsync(token, inserts))
                    {
                        await _wxUserRepository.InsertAsync(request.SiteId, wxUser);
                    }
                    await _wxUserRepository.DeleteAllAsync(request.SiteId, deletes);
                }

                tags = await _wxManager.GetUserTagsAsync(token);
                List<string> pageOpenIds;
                (total, count, pageOpenIds) = await _wxUserRepository.GetPageOpenIds(request.SiteId, request.TagId, request.Keyword, request.Page, request.PerPage);
                users = await _wxManager.GetUsersAsync(token, pageOpenIds);
                await _wxUserRepository.UpdateAllAsync(request.SiteId, users);
            }

            return new GetResult
            {
                IsWxEnabled = isWxEnabled,
                Tags = tags,
                Total = total,
                Count = count,
                Users = users
            };
        }
    }
}
