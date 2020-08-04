using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Senparc.CO2NET.AspNet.HttpUtility;
using Senparc.NeuChar.MessageHandlers;
using Senparc.Weixin.MP;
using Senparc.Weixin.WxOpen.Entities.Request;

namespace SSCMS.Web.Controllers.Wx
{
    public partial class IndexController
    {
        ///// <summary>
        ///// 用户发送消息后，微信平台自动Post一个请求到这里
        ///// </summary>
        //[HttpGet, Route(Route)]
        //public async Task<ActionResult<string>> Post([FromRoute] int siteId, [FromBody] PostModel postModel)
        //{
        //    var account = await _wxAccountRepository.GetBySiteIdAsync(siteId);

        //    if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, account.MpToken))
        //    {
        //        return Content("参数错误！");
        //    }

        //    postModel.Token = account.MpToken;//根据自己后台的设置保持一致
        //    postModel.EncodingAESKey = account.MpEncodingAESKey;//根据自己后台的设置保持一致
        //    postModel.AppId = account.MpAppId;//根据自己后台的设置保持一致（必须提供）

        //    var cancellationToken = new CancellationToken();//给异步方法使用

        //    var messageHandler = new CustomMessageHandler(Request.GetRequestMemoryStream(), postModel, 10);

        //    #region 没有重写的异步方法将默认尝试调用同步方法中的代码（为了偷懒）

        //    /* 使用 SelfSynicMethod 的好处是可以让异步、同步方法共享同一套（同步）代码，无需写两次，
        //     * 当然，这并不一定适用于所有场景，所以是否选用需要根据实际情况而定，这里只是演示，并不盲目推荐。*/
        //    messageHandler.DefaultMessageHandlerAsyncEvent = DefaultMessageHandlerAsyncEvent.SelfSynicMethod;

        //    #endregion

        //    #region 设置消息去重 设置

        //    /* 如果需要添加消息去重功能，只需打开OmitRepeatedMessage功能，SDK会自动处理。
        //     * 收到重复消息通常是因为微信服务器没有及时收到响应，会持续发送2-5条不等的相同内容的RequestMessage*/
        //    messageHandler.OmitRepeatedMessage = true;//默认已经开启，此处仅作为演示，也可以设置为false在本次请求中停用此功能

        //    #endregion

        //    messageHandler.SaveRequestMessageLog();//记录 Request 日志（可选）

        //    await messageHandler.ExecuteAsync(cancellationToken); //执行微信处理过程（关键）

        //    messageHandler.SaveResponseMessageLog();//记录 Response 日志（可选）

        //    MessageHandler = messageHandler;//开放出MessageHandler是为了做单元测试，实际使用过程中这一行不需要

        //    return new FixWeixinBugWeixinResult(messageHandler);
        //}
    }
}
