using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Core.Services;

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

            var results = new GetResult
            {
                Success = false,
                ErrorMessage = string.Empty,
                Tags = new List<WxUserTag>(),
                Total = 0,
                Count = 0,
                Users = new List<WxUser>()
            };

            var account = await _wxManager.GetAccountAsync(request.SiteId);
            string token;
            (results.Success, token, results.ErrorMessage) = await _wxManager.GetAccessTokenAsync(account);
            if (!results.Success)
            {
                return results;
            }
            if (account.MpType == WxMpType.Subscription || account.MpType == WxMpType.Service)
            {
                results.Success = false;
                results.ErrorMessage = _wxManager.GetErrorUnAuthenticated(account);
                return results;
            }

            if (request.IsBlock)
            {
                var openIds = await _wxManager.GetUserOpenIdsAsync(token, request.IsBlock);
                results.Count = openIds.Count;
                var pageOpenIds = openIds.Skip((request.Page - 1) * request.PerPage).Take(request.PerPage).ToList();
                results.Users = await _wxManager.GetUsersAsync(token, pageOpenIds);
                await _wxUserRepository.UpdateAllAsync(request.SiteId, results.Users);
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

                results.Tags = await _wxManager.GetUserTagsAsync(token);
                List<string> pageOpenIds;
                (results.Total, results.Count, pageOpenIds) = await _wxUserRepository.GetPageOpenIds(request.SiteId, request.TagId, request.Keyword, request.Page, request.PerPage);
                results.Users = await _wxManager.GetUsersAsync(token, pageOpenIds);
                await _wxUserRepository.UpdateAllAsync(request.SiteId, results.Users);
            }

            return results;
        }
    }
}
