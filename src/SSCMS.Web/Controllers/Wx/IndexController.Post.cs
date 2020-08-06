using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Senparc.CO2NET.AspNet.HttpUtility;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities.Request;

namespace SSCMS.Web.Controllers.Wx
{
    public partial class IndexController
    {
        /// <summary>
        /// 用户发送消息后，微信平台自动Post一个请求到这里
        /// </summary>
        [HttpPost, Route(Route)]
        public async Task<ActionResult> Post([FromRoute] int siteId, PostModel postModel)
        {
            var account = await _wxAccountRepository.GetBySiteIdAsync(siteId);

            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, account.MpToken))
            {
                return Content("参数错误！");
            }

            postModel.Token = account.MpToken;//根据自己后台的设置保持一致
            postModel.EncodingAESKey = account.MpEncodingAESKey;//根据自己后台的设置保持一致
            postModel.AppId = account.MpAppId;//根据自己后台的设置保持一致（必须提供）

            try
            {
                var cancellationToken = new CancellationToken(); //给异步方法使用

                var messageHandler = new CustomMessageHandler(_wxManager, _wxChatRepository, account, Request.GetRequestMemoryStream(), postModel, 10);

                messageHandler.SaveRequestMessageLog(); //记录 Request 日志（可选）

                await messageHandler.ExecuteAsync(cancellationToken); //执行微信处理过程（关键）

                messageHandler.SaveResponseMessageLog(); //记录 Response 日志（可选）

                return new FixWeixinBugWeixinResult(messageHandler);
            }
            catch (Exception ex)
            {
                await _errorLogRepository.AddErrorLogAsync(ex);
            }

            return Content("");
        }
    }
}
