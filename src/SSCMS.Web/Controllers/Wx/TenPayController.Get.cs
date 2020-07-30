using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;

namespace SSCMS.Web.Controllers.Wx
{
    public partial class TenPayController
    {
        /// <summary>
        /// 获取用户的OpenId
        /// </summary>
        [HttpGet, Route(Route)]
        public async Task<ActionResult<RedirectResult>> Get([FromRoute] int siteId, [FromQuery] GetRequest request)
        {
            var account = await _openAccountRepository.GetBySiteIdAsync(siteId);

            var returnUrl = $"{account.TenPayAuthorizeUrl}/authorize?productId={request.ProductId}&returnUrl={request.ReturnUrl}";
            var state = $"{request.ProductId}";
            var url = OAuthApi.GetAuthorizeUrl(account.TenPayAppId, returnUrl, state, OAuthScope.snsapi_userinfo);

            return Redirect(url);
        }
    }
}
