using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.MP;
using Senparc.Weixin.WxOpen.Entities.Request;

namespace SSCMS.Web.Controllers.Open
{
    public partial class WxController
    {
        /// <summary>
        /// GET请求用于处理微信小程序后台的URL验证
        /// </summary>
        [HttpGet, Route(Route)]
        public async Task<ActionResult<string>> Get([FromRoute] int siteId, [FromQuery] PostModel postModel, [FromQuery] string echostr)
        {
            var account = await _openAccountRepository.GetBySiteIdAsync(siteId);

            if (account != null && CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, account.WxToken))
            {
                if (!account.WxConnected)
                {
                    account.WxConnected = true;
                    await _openAccountRepository.SetAsync(account);
                }
                return echostr; //返回随机字符串则表示验证通过
            }

            if (account.WxConnected)
            {
                account.WxConnected = false;
                await _openAccountRepository.SetAsync(account);
            }

            return "failed:" + postModel.Signature;
        }
    }
}
