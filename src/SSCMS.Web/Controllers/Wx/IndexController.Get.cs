using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.MP;
using Senparc.Weixin.WxOpen.Entities.Request;

namespace SSCMS.Web.Controllers.Wx
{
    public partial class IndexController
    {
        /// <summary>
        /// GET请求用于处理微信小程序后台的URL验证
        /// </summary>
        [HttpGet, Route(Route)]
        public async Task<ActionResult<string>> Get([FromRoute] int siteId, [FromQuery] PostModel postModel, [FromQuery] string echostr)
        {
            var account = await _wxAccountRepository.GetBySiteIdAsync(siteId);

            if (account != null && CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, account.MpToken))
            {
                if (!account.MpConnected)
                {
                    account.MpConnected = true;
                    await _wxAccountRepository.SetAsync(account);
                }
                return echostr; //返回随机字符串则表示验证通过
            }

            if (account != null && account.MpConnected)
            {
                account.MpConnected = false;
                await _wxAccountRepository.SetAsync(account);
            }

            return "failed:" + postModel.Signature;
        }
    }
}
