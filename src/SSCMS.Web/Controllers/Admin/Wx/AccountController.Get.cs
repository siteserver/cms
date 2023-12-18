using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Utils;
using SSCMS.Web.Controllers.Wx;
using SSCMS.Core.Utils;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class AccountController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] int siteId)
        {
            if (!await _authManager.HasSitePermissionsAsync(siteId, MenuUtils.SitePermissions.WxAccount))
            {
                return Unauthorized();
            }

            var account = await _wxAccountRepository.GetBySiteIdAsync(siteId);

            var mpUrl = Request.Scheme + "://" + PageUtils.Combine(Request.Host.Host, Constants.ApiWxPrefix, Controllers.Wx.IndexController.Route.Replace("{siteId}", siteId.ToString()));

            var mpTypes = ListUtils.GetSelects<WxMpType>();

            return new GetResult
            {
                MpUrl = mpUrl,
                Account = account,
                MpTypes = mpTypes
            };
        }
    }
}
