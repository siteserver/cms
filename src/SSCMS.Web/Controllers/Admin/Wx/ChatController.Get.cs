using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class ChatController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.WxChat))
            {
                return Unauthorized();
            }

            List<WxChat> chats = null;
            var count = 0;
            var users = new List<WxUser>();

            var site = await _siteRepository.GetAsync(request.SiteId);
            var isWxEnabled = await _wxManager.IsEnabledAsync(site);

            if (isWxEnabled)
            {
                var (success, token, errorMessage) = await _wxManager.GetAccessTokenAsync(request.SiteId);
                if (!success)
                {
                    return this.Error(errorMessage);
                }
                
                count = await _wxChatRepository.GetCountAsync(request.SiteId, request.Star, request.Keyword);
                chats = await _wxChatRepository.GetChatsAsync(request.SiteId, request.Star, request.Keyword, request.Page, request.PerPage);

                var openIds = chats.Select(x => x.OpenId).Distinct().ToList();
                var dbOpenIds = await _wxUserRepository.GetAllOpenIds(request.SiteId);

                var inserts = openIds.Where(openId => !dbOpenIds.Contains(openId)).ToList();
                foreach (var wxUser in await _wxManager.GetUsersAsync(token, inserts))
                {
                    await _wxUserRepository.InsertAsync(request.SiteId, wxUser);
                }

                users = await _wxManager.GetUsersAsync(token, openIds);
                await _wxUserRepository.UpdateAllAsync(request.SiteId, users);
            }

            return new GetResult
            {
                IsWxEnabled = isWxEnabled,
                Chats = chats,
                Count = count,
                Users = users
            };
        }
    }
}
