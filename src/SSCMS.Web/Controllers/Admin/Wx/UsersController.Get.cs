using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Wx;
using SSCMS.Core.Utils;

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

            List<WxUserTag> tags = null;
            var total = 0;
            var count = 0;
            List<WxUser> users = null;

            var (success, token, errorMessage) = await _wxManager.GetAccessTokenAsync(request.SiteId);
            if (success)
            {
                if (request.IsBlock)
                {
                    var openIds = await _wxManager.GetUserOpenIdsAsync(token, request.IsBlock);
                    count = openIds.Count;
                    var pageOpenIds = openIds.Skip((request.Page - 1) * request.PerPage).Take(request.PerPage).ToList();
                    users = await _wxManager.GetUsersAsync(token, pageOpenIds);
                    await _wxUserRepository.UpdateAllAsync(request.SiteId, users);
                }
                else
                {
                    if (request.Init)
                    {
                        var openIds = await _wxManager.GetUserOpenIdsAsync(token, false);
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
            }

            return new GetResult
            {
                Success = success,
                ErrorMessage = errorMessage,
                Tags = tags,
                Total = total,
                Count = count,
                Users = users
            };
        }
    }
}
