using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils.Wx;

namespace SSCMS.Web.Controllers.Wx
{
    public partial class PushController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<string>> Get([FromRoute] int siteId, [FromQuery] string signature, [FromQuery] string timestamp, [FromQuery] string nonce, [FromQuery] string echostr)
        {
            var account = await _wxAccountRepository.GetBySiteIdAsync(siteId);

            var hash = string.Empty;
            var ret = WXBizMsgCrypt.GenarateSinature(account.MpToken, timestamp, nonce, string.Empty, ref hash);
            if (ret == 0 && hash == signature)
            {
                return echostr;
            }
            return "error";
        }
    }
}
