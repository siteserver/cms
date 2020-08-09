using System.IO;
using System.Threading.Tasks;
using Senparc.NeuChar.Entities;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageHandlers;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Wx
{
    public partial class IndexController
    {
        private sealed class CustomMessageHandler : MessageHandler<CustomMessageContext>
        {
            private readonly WxAccount _wxAccount;
            private readonly IWxManager _wxManager;
            private readonly IWxChatRepository _wxChatRepository;

            public CustomMessageHandler(IWxManager wxManager, IWxChatRepository wxChatRepository, WxAccount account, Stream inputStream, PostModel postModel, int maxRecordCount = 0, bool onlyAllowEncryptMessage = false) : base(inputStream, postModel, maxRecordCount, onlyAllowEncryptMessage)
            {
                _wxManager = wxManager;
                _wxChatRepository = wxChatRepository;
                _wxAccount = account;
            }

            /// <summary>
            /// 处理文字请求
            /// </summary>
            /// <param name="requestMessage">请求消息</param>
            /// <returns></returns>
            public override async Task<IResponseMessageBase> OnTextRequestAsync(RequestMessageText requestMessage)
            {
                var isSession = await _wxChatRepository.UserAdd(new WxChat
                {
                    SiteId = _wxAccount.SiteId,
                    OpenId = requestMessage.FromUserName,
                    IsReply = false,
                    Text = requestMessage.Content
                });

                var messages = await _wxManager.GetMessagesAsync(_wxAccount.SiteId, requestMessage.Content, isSession ? 0 : _wxAccount.MpReplyAutoMessageId);
                foreach (var message in messages)
                {
                    await _wxManager.CustomSendAsync(_wxAccount.MpAppId, requestMessage.FromUserName, message);
                }

                return new SuccessResponseMessage();

                //if (messages == null || messages.Count == 0)
                //{
                //    return new SuccessResponseMessage();
                //}

                //var message = messages.First();

                //IResponseMessageBase defaultResponse = new SuccessResponseMessage();

                //if (message.MaterialType == MaterialType.Message)
                //{
                //    await _wxManager.CustomSendMpNewsAsync(_wxAccount.MpAppId, requestMessage.FromUserName, message.MediaId, false);
                //}
                //else if(message.MaterialType == MaterialType.Text)
                //{
                //    var response = CreateResponseMessage<ResponseMessageText>();
                //    response.Content = message.Text;
                //    defaultResponse = response;
                //}

                //for (var i = 0; i < messages.Count; i++)
                //{
                //    if (i > 0)
                //    {
                //        var message = messages[i];
                //        await _wxManager.CustomSendTextAsync(_wxAccount.MpAppId, requestMessage.FromUserName, "second message here");
                //    }
                //}

                //return defaultResponse;


//                var defaultResponseMessage = CreateResponseMessage<ResponseMessageText>();

//                var requestHandler = await requestMessage.StartHandler()
//                    //关键字不区分大小写，按照顺序匹配成功后将不再运行下面的逻辑
//                    .Keyword("约束", () =>
//                    {
//                        defaultResponseMessage.Content =
//                            @"您正在进行微信内置浏览器约束判断测试。您可以：
//<a href=""https://sdk.weixin.senparc.com/FilterTest/"">点击这里</a>进行客户端约束测试（地址：https://sdk.weixin.senparc.com/FilterTest/），如果在微信外打开将直接返回文字。
//或：
//<a href=""https://sdk.weixin.senparc.com/FilterTest/Redirect"">点击这里</a>进行客户端约束测试（地址：https://sdk.weixin.senparc.com/FilterTest/Redirect），如果在微信外打开将重定向一次URL。";
//                        return defaultResponseMessage;
//                    })
//                    .Keyword("OPEN", () =>
//                    {
//                        var openResponseMessage = requestMessage.CreateResponseMessage<ResponseMessageNews>();
//                        openResponseMessage.Articles.Add(new Article()
//                        {
//                            Title = "开放平台微信授权测试！",
//                            Description = @"点击进入Open授权页面。

//授权之后，您的微信所收到的消息将转发到第三方（盛派网络小助手）的服务器上，并获得对应的回复。

//测试完成后，您可以登陆公众号后台取消授权。",
//                            Url = "https://sdk.weixin.senparc.com/OpenOAuth/JumpToMpOAuth"
//                        });
//                        return openResponseMessage;
//                    })
//                    .Keyword("错误", () =>
//                    {
//                        var errorResponseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
//                        //因为没有设置errorResponseMessage.Content，所以这小消息将无法正确返回。
//                        return errorResponseMessage;
//                    })
//                    .Keyword("容错", () =>
//                    {
//                        Thread.Sleep(4900); //故意延时1.5秒，让微信多次发送消息过来，观察返回结果
//                        var faultTolerantResponseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
//                        faultTolerantResponseMessage.Content =
//                            $"测试容错，MsgId：{requestMessage.MsgId}，Ticks：{SystemTime.Now.Ticks}";
//                        return faultTolerantResponseMessage;
//                    })
//                    .Keyword("OPENID", () =>
//                    {
//                        var openId = requestMessage.FromUserName; //获取OpenId
//                        var userInfo = UserApi.Info(_wxAccount.MpAppId, openId);

//                        defaultResponseMessage.Content =
//                            $"您的OpenID为：{requestMessage.FromUserName}\r\n昵称：{userInfo.nickname}\r\n性别：{(WeixinSex) userInfo.sex}\r\n地区（国家/省/市）：{userInfo.country}/{userInfo.province}/{userInfo.city}\r\n关注时间：{DateTimeHelper.GetDateTimeFromXml(userInfo.subscribe_time)}\r\n关注状态：{userInfo.subscribe}";
//                        return defaultResponseMessage;
//                    })
//                    //选择菜单，关键字：101（微信服务器端最终格式：id="s:101",content="满意"）
//                    .SelectMenuKeyword("101", () =>
//                    {
//                        defaultResponseMessage.Content = $"感谢您的评价（{requestMessage.Content}）！我们会一如既往为提高企业和开发者生产力而努力！";
//                        return defaultResponseMessage;
//                    })
//                    //选择菜单，关键字：102（微信服务器端最终格式：id="s:102",content="一般"）
//                    .SelectMenuKeyword("102", () =>
//                    {
//                        defaultResponseMessage.Content = $"感谢您的评价（{requestMessage.Content}）！希望我们的服务能让您越来越满意！";
//                        return defaultResponseMessage;
//                    })
//                    //选择菜单，关键字：103（微信服务器端最终格式：id="s:103",content="不满意"）
//                    .SelectMenuKeyword("103", () =>
//                    {
//                        defaultResponseMessage.Content =
//                            $"感谢您的评价（{requestMessage.Content}）！我们需要您的意见或建议，欢迎向我们反馈！ <a href=\"https://github.com/JeffreySu/WeiXinMPSDK/issues/new\">点击这里</a>";
//                        return defaultResponseMessage;
//                    })
//                    .SelectMenuKeywords(new[] {"110", "111"}, () =>
//                    {
//                        defaultResponseMessage.Content = "这里只是演示，可以同时支持多个选择菜单";
//                        return defaultResponseMessage;
//                    })


//                    //“一次订阅消息”接口测试
//                    .Keyword("订阅", () =>
//                    {
//                        defaultResponseMessage.Content = "点击打开：https://sdk.weixin.senparc.com/SubscribeMsg";
//                        return defaultResponseMessage;
//                    })
//                    //正则表达式
//                    .Regex(@"^\d+#\d+$", () =>
//                    {
//                        defaultResponseMessage.Content =
//                            $"您输入了：{requestMessage.Content}，符合正则表达式：^\\d+#\\d+$";
//                        return defaultResponseMessage;
//                    })

//                    //当 Default 使用异步方法时，需要写在最后一个，且 requestMessage.StartHandler() 前需要使用 await 等待异步方法执行；
//                    //当 Default 使用同步方法，不一定要在最后一个,并且不需要使用 await
//                    .Default(async () =>
//                    {
//                        var result = new StringBuilder();
//                        result.AppendFormat("您刚才发送了文字信息：{0}\r\n\r\n", requestMessage.Content);

//                        var currentMessageContext = await GetCurrentMessageContext();
//                        if (currentMessageContext.RequestMessages.Count > 1)
//                        {
//                            result.AppendFormat("您刚才还发送了如下消息（{0}/{1}）：\r\n",
//                                currentMessageContext.RequestMessages.Count,
//                                currentMessageContext.StorageData);
//                            for (int i = currentMessageContext.RequestMessages.Count - 2; i >= 0; i--)
//                            {
//                                var historyMessage = currentMessageContext.RequestMessages[i];
//                                result.Append(
//                                    $"{historyMessage.CreateTime:HH:mm:ss} 【{historyMessage.MsgType.ToString()}】{(historyMessage is RequestMessageText ? (historyMessage as RequestMessageText).Content : "[非文字类型]")}\r\n"
//                                );
//                            }

//                            result.AppendLine("\r\n");
//                        }

//                        result.AppendFormat("如果您在{0}分钟内连续发送消息，记录将被自动保留（当前设置：最多记录{1}条）。过期后记录将会自动清除。\r\n",
//                            GlobalMessageContext.ExpireMinutes, GlobalMessageContext.MaxRecordCount);
//                        result.AppendLine("\r\n");
//                        result.AppendLine(
//                            "您还可以发送【位置】【图片】【语音】【视频】等类型的信息（注意是这几种类型，不是这几个文字），查看不同格式的回复。\r\nSDK官方地址：https://sdk.weixin.senparc.com");

//                        defaultResponseMessage.Content = result.ToString();

//                        return defaultResponseMessage;
//                    });

//                return requestHandler.GetResponseMessage();
            }

            public override async Task<IResponseMessageBase> OnEvent_SubscribeRequestAsync(RequestMessageEvent_Subscribe requestMessage)
            {
                var message = await _wxManager.GetMessageAsync(_wxAccount.SiteId, _wxAccount.MpReplyBeAddedMessageId);
                if (message != null)
                {
                    await _wxManager.CustomSendAsync(_wxAccount.MpAppId, requestMessage.FromUserName, message);
                }

                return new SuccessResponseMessage();
            }

            public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
            {
                var responseMessage = CreateResponseMessage<ResponseMessageText>();
                responseMessage.Content = "这条消息来自DefaultResponseMessage。";
                return responseMessage;
            }
        }
    }
}
