using System;
using System.IO;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.IO;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Manager.Store;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;
using SiteServer.CMS.WeiXin.WeiXinMP.Context;
using SiteServer.CMS.WeiXin.WeiXinMP.Entities.Request;
using SiteServer.CMS.WeiXin.WeiXinMP.Entities.Response;
using SiteServer.CMS.WeiXin.WeiXinMP.MessageHandlers;
using KeywordInfo = SiteServer.CMS.WeiXin.Model.KeywordInfo;
using MessageManager = SiteServer.CMS.WeiXin.Manager.MessageManager;

namespace SiteServer.CMS.WeiXin.MP
{
    /// <summary>
    /// 自定义MessageHandler
    /// 把MessageHandler作为基类，重写对应请求的处理方法
    /// </summary>
    public partial class GexiaMessageHandler : MessageHandler<MessageContext>
    {
        /*
         * 重要提示：v1.5起，MessageHandler提供了一个DefaultResponseMessage的抽象方法，
         * DefaultResponseMessage必须在子类中重写，用于返回没有处理过的消息类型（也可以用于默认消息，如帮助信息等）；
         * 其中所有原OnXX的抽象方法已经都改为虚方法，可以不必每个都重写。若不重写，默认返回DefaultResponseMessage方法中的结果。
         */

        private PublishmentSystemInfo publishmentSystemInfo;
        private AccountInfo accountInfo;

        public GexiaMessageHandler(AccountInfo accountInfo, Stream inputStream, int maxRecordCount = 0)
            : base(inputStream, maxRecordCount)
        {
            //这里设置仅用于测试，实际开发可以在外部更全局的地方设置，
            //比如MessageHandler<MessageContext>.GlobalWeixinContext.ExpireMinutes = 3。
            WeixinContext.ExpireMinutes = 3;

            publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(accountInfo.PublishmentSystemID);
            this.accountInfo = accountInfo;
        }

        private IResponseMessageBase GetResponseMessage(string keyword)
        {
            KeywordInfo keywordInfo = null;

            var keywordID = DataProviderWX.KeywordMatchDAO.GetKeywordIDByMPController(publishmentSystemInfo.PublishmentSystemId, keyword);
            if (keywordID == 0 && StringUtils.StartsWith(keyword, "GX_"))
            {
                var keywordType = EKeywordTypeUtils.GetEnumType(keyword.Substring(3));
                keywordInfo = new KeywordInfo(0, publishmentSystemInfo.PublishmentSystemId, keyword, false, keywordType, EMatchType.Exact, string.Empty, DateTime.Now, 0);
            }

            if (keywordID == 0 && keywordInfo == null && accountInfo.IsDefaultReply && !string.IsNullOrEmpty(accountInfo.DefaultReplyKeyword))
            {
                keywordID = DataProviderWX.KeywordMatchDAO.GetKeywordIDByMPController(publishmentSystemInfo.PublishmentSystemId, accountInfo.DefaultReplyKeyword);
            }

            if (keywordInfo == null && keywordID > 0)
            {
                keywordInfo = DataProviderWX.KeywordDAO.GetKeywordInfo(keywordID);
            }               

            return GetResponseMessage(keywordInfo, keyword);
        }

        private IResponseMessageBase GetResponseMessage(KeywordInfo keywordInfo, string keyword)
        {
            try
            {
                if (keywordInfo != null && !keywordInfo.IsDisabled)
                {
                    if (keywordInfo.KeywordType == EKeywordType.Text)
                    {
                        DataProviderWX.CountDAO.AddCount(publishmentSystemInfo.PublishmentSystemId, ECountType.RequestText);

                        var responseMessage = CreateResponseMessage<ResponseMessageText>();
                        responseMessage.Content = keywordInfo.Reply;
                        return responseMessage;
                    }
                    else if (keywordInfo.KeywordType == EKeywordType.News)
                    {
                        DataProviderWX.CountDAO.AddCount(publishmentSystemInfo.PublishmentSystemId, ECountType.RequestNews);

                        var responseMessage = CreateResponseMessage<ResponseMessageNews>();

                        foreach (var resourceInfo in DataProviderWX.KeywordResourceDAO.GetResourceInfoList(keywordInfo.KeywordID))
                        {
                            var imageUrl = PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, resourceInfo.ImageUrl));

                            var pageUrl = string.Empty;
                            if (resourceInfo.ResourceType == EResourceType.Site)
                            {
                                if (resourceInfo.ChannelID > 0 && resourceInfo.ContentID > 0)
                                {
                                    pageUrl = PageUtilityWX.GetContentUrl(publishmentSystemInfo, resourceInfo.ChannelID, resourceInfo.ContentID, false);
                                }
                                else if (resourceInfo.ChannelID > 0)
                                {
                                    pageUrl = PageUtilityWX.GetChannelUrl(publishmentSystemInfo, resourceInfo.ChannelID);
                                }
                                else
                                {
                                    pageUrl = PageUtilityWX.GetChannelUrl(publishmentSystemInfo, publishmentSystemInfo.PublishmentSystemId);
                                }
                            }
                            else if (resourceInfo.ResourceType == EResourceType.Content)
                            {
                                pageUrl = PageUtilityWX.GetWeiXinFileUrl(publishmentSystemInfo, resourceInfo.KeywordID, resourceInfo.ResourceID);
                            }
                            else if (resourceInfo.ResourceType == EResourceType.Url)
                            {
                                pageUrl = resourceInfo.NavigationUrl;
                            }

                            responseMessage.Articles.Add(new Article()
                            {
                                Title = resourceInfo.Title,
                                Description = MPUtils.GetSummary(resourceInfo.Summary, resourceInfo.Content),
                                PicUrl = imageUrl,
                                Url = pageUrl
                            });
                        }

                        return responseMessage;
                    }
                    else if (keywordInfo.KeywordType == EKeywordType.Coupon)
                    {
                        var responseMessage = CreateResponseMessage<ResponseMessageNews>();
                        var articleList = CouponManager.Trigger(keywordInfo, keyword, RequestMessage.FromUserName);
                        if (articleList.Count > 0)
                        {
                            foreach (var article in articleList)
                            {
                                responseMessage.Articles.Add(article);
                            }
                        }

                        return responseMessage;
                    }
                    else if (keywordInfo.KeywordType == EKeywordType.Vote)
                    {
                        var responseMessage = CreateResponseMessage<ResponseMessageNews>();
                        var articleList = VoteManager.Trigger(publishmentSystemInfo, keywordInfo, RequestMessage.FromUserName);
                        if (articleList.Count > 0)
                        {
                            foreach (var article in articleList)
                            {
                                responseMessage.Articles.Add(article);
                            }
                        }

                        return responseMessage;
                    }
                    else if (keywordInfo.KeywordType == EKeywordType.Message)
                    {
                        var responseMessage = CreateResponseMessage<ResponseMessageNews>();
                        var articleList = MessageManager.Trigger(publishmentSystemInfo, keywordInfo, RequestMessage.FromUserName);
                        if (articleList.Count > 0)
                        {
                            foreach (var article in articleList)
                            {
                                responseMessage.Articles.Add(article);
                            }
                        }

                        return responseMessage;
                    }
                    else if (keywordInfo.KeywordType == EKeywordType.Appointment)
                    {
                        var responseMessage = CreateResponseMessage<ResponseMessageNews>();
                        var articleList = AppointmentManager.Trigger(keywordInfo, RequestMessage.FromUserName);
                        if (articleList.Count > 0)
                        {
                            foreach (var article in articleList)
                            {
                                responseMessage.Articles.Add(article);
                            }
                        }

                        return responseMessage;
                    }
                    else if (keywordInfo.KeywordType == EKeywordType.Conference)
                    {
                        var responseMessage = CreateResponseMessage<ResponseMessageNews>();
                        var articleList = ConferenceManager.Trigger(publishmentSystemInfo, keywordInfo, RequestMessage.FromUserName);
                        if (articleList.Count > 0)
                        {
                            foreach (var article in articleList)
                            {
                                responseMessage.Articles.Add(article);
                            }
                        }

                        return responseMessage;
                    }
                    else if (keywordInfo.KeywordType == EKeywordType.Map)
                    {
                        var responseMessage = CreateResponseMessage<ResponseMessageNews>();
                        var articleList = MapManager.Trigger(publishmentSystemInfo, keywordInfo, RequestMessage.FromUserName);
                        if (articleList.Count > 0)
                        {
                            foreach (var article in articleList)
                            {
                                responseMessage.Articles.Add(article);
                            }
                        }

                        return responseMessage;
                    }
                    else if (keywordInfo.KeywordType == EKeywordType.View360)
                    {
                        var responseMessage = CreateResponseMessage<ResponseMessageNews>();
                        var articleList = View360Manager.Trigger(publishmentSystemInfo, keywordInfo, RequestMessage.FromUserName);
                        if (articleList.Count > 0)
                        {
                            foreach (var article in articleList)
                            {
                                responseMessage.Articles.Add(article);
                            }
                        }

                        return responseMessage;
                    }
                    else if (keywordInfo.KeywordType == EKeywordType.Album)
                    {
                        var responseMessage = CreateResponseMessage<ResponseMessageNews>();
                        var articleList = AlbumManager.Trigger(publishmentSystemInfo, keywordInfo, RequestMessage.FromUserName);
                        if (articleList.Count > 0)
                        {
                            foreach (var article in articleList)
                            {
                                responseMessage.Articles.Add(article);
                            }
                        }

                        return responseMessage;
                    }
                    else if (keywordInfo.KeywordType == EKeywordType.Scratch)
                    {
                        var responseMessage = CreateResponseMessage<ResponseMessageNews>();
                        var articleList = LotteryManager.Trigger(keywordInfo, ELotteryType.Scratch, RequestMessage.FromUserName);
                        if (articleList.Count > 0)
                        {
                            foreach (var article in articleList)
                            {
                                responseMessage.Articles.Add(article);
                            }
                        }

                        return responseMessage;
                    }
                    else if (keywordInfo.KeywordType == EKeywordType.BigWheel)
                    {
                        var responseMessage = CreateResponseMessage<ResponseMessageNews>();
                        var articleList = LotteryManager.Trigger(keywordInfo, ELotteryType.BigWheel, RequestMessage.FromUserName);
                        if (articleList.Count > 0)
                        {
                            foreach (var article in articleList)
                            {
                                responseMessage.Articles.Add(article);
                            }
                        }

                        return responseMessage;
                    }
                    else if (keywordInfo.KeywordType == EKeywordType.GoldEgg)
                    {
                        var responseMessage = CreateResponseMessage<ResponseMessageNews>();
                        var articleList = LotteryManager.Trigger(keywordInfo, ELotteryType.GoldEgg, RequestMessage.FromUserName);
                        if (articleList.Count > 0)
                        {
                            foreach (var article in articleList)
                            {
                                responseMessage.Articles.Add(article);
                            }
                        }

                        return responseMessage;
                    }
                    else if (keywordInfo.KeywordType == EKeywordType.Flap)
                    {
                        var responseMessage = CreateResponseMessage<ResponseMessageNews>();
                        var articleList = LotteryManager.Trigger(keywordInfo, ELotteryType.Flap, RequestMessage.FromUserName);
                        if (articleList.Count > 0)
                        {
                            foreach (var article in articleList)
                            {
                                responseMessage.Articles.Add(article);
                            }
                        }

                        return responseMessage;
                    }
                    else if (keywordInfo.KeywordType == EKeywordType.YaoYao)
                    {
                        var responseMessage = CreateResponseMessage<ResponseMessageNews>();
                        var articleList = LotteryManager.Trigger(keywordInfo, ELotteryType.YaoYao, RequestMessage.FromUserName);
                        if (articleList.Count > 0)
                        {
                            foreach (var article in articleList)
                            {
                                responseMessage.Articles.Add(article);
                            }
                        }

                        return responseMessage;
                    }
                    else if (keywordInfo.KeywordType == EKeywordType.Search)
                    {
                        var responseMessage = CreateResponseMessage<ResponseMessageNews>();
                        var articleList = SearchManager.Trigger(publishmentSystemInfo, keywordInfo, RequestMessage.FromUserName);
                        if (articleList.Count > 0)
                        {
                            foreach (var article in articleList)
                            {
                                responseMessage.Articles.Add(article);
                            }
                        }

                        return responseMessage;
                    }
                    else if (keywordInfo.KeywordType == EKeywordType.Store)
                    {
                        var responseMessage = CreateResponseMessage<ResponseMessageNews>();
                        var articleList = StoreManager.Trigger(publishmentSystemInfo, keywordInfo, RequestMessage.FromUserName);
                        if (articleList.Count > 0)
                        {
                            foreach (var article in articleList)
                            {
                                responseMessage.Articles.Add(article);
                            }
                        }

                        return responseMessage;
                    }
                    else if (keywordInfo.KeywordType == EKeywordType.Collect)
                    {
                        var responseMessage = CreateResponseMessage<ResponseMessageNews>();
                        var articleList = CollectManager.Trigger(publishmentSystemInfo, keywordInfo, RequestMessage.FromUserName);
                        if (articleList.Count > 0)
                        {
                            foreach (var article in articleList)
                            {
                                responseMessage.Articles.Add(article);
                            }
                        }

                        return responseMessage;
                    }
                    else if (keywordInfo.KeywordType == EKeywordType.Card)
                    {
                        var responseMessage = CreateResponseMessage<ResponseMessageNews>();
                        var articleList = CardManager.Trigger(publishmentSystemInfo, keywordInfo, RequestMessage.FromUserName);
                        if (articleList.Count > 0)
                        {
                            foreach (var article in articleList)
                            {
                                responseMessage.Articles.Add(article);
                            }
                        }

                        return responseMessage;
                    }
                }               
            }
            catch (Exception ex)
            {
                LogUtils.AddLog(string.Empty, "Gexia Error", ex.StackTrace);
            }

            return null;
        }

        /// <summary>
        /// 处理文字请求
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            //var rm = base.CreateResponseMessage<ResponseMessageText>();
            //rm.Content = "诗水蛇山神庙";
            //return rm;

            var keyword = requestMessage.Content;
            return GetResponseMessage(keyword);
        }

        // <summary>
        // 处理位置请求
        // </summary>
        // <param name="requestMessage"></param>
        // <returns></returns>
        public override IResponseMessageBase OnLocationRequest(RequestMessageLocation requestMessage)
        {
            var locationService = new LocationService();
            var responseMessage = locationService.GetResponseMessage(publishmentSystemInfo, requestMessage as RequestMessageLocation, WeixinOpenId);
            return responseMessage;
        }

        /// <summary>
        /// 处理图片请求
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        //public override IResponseMessageBase OnImageRequest(RequestMessageImage requestMessage)
        //{
        //    var responseMessage = CreateResponseMessage<ResponseMessageNews>();
        //    responseMessage.Articles.Add(new Article()
        //    {
        //        Title = "您刚才发送了图片信息",
        //        Description = "您发送的图片将会显示在边上",
        //        PicUrl = requestMessage.PicUrl,
        //        Url = "http://weixin.senparc.com"
        //    });
        //    responseMessage.Articles.Add(new Article()
        //    {
        //        Title = "第二条",
        //        Description = "第二条带连接的内容",
        //        PicUrl = requestMessage.PicUrl,
        //        Url = "http://weixin.senparc.com"
        //    });
        //    return responseMessage;
        //}

        //<summary>
        //处理语音请求
        //</summary>
        //<param name="requestMessage"></param>
        //<returns></returns>
        public override IResponseMessageBase OnVoiceRequest(RequestMessageVoice requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageMusic>();
            responseMessage.Music.MusicUrl = "http://weixin.senparc.com/Content/music1.mp3";
            responseMessage.Music.Title = "这里是一条音乐消息";
            responseMessage.Music.Description = "来自Jeffrey Su的美妙歌声~~";
            return responseMessage;
        }

        //菜单点击
        public override IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {
            var keyword = requestMessage.EventKey;
            return GetResponseMessage(keyword);
        }

        //关注时触发
        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            DataProviderWX.CountDAO.AddCount(publishmentSystemInfo.PublishmentSystemId , ECountType.UserSubscribe);

            var keywordID = 0;
            var keyword = string.Empty;
            if (accountInfo.IsWelcome && !string.IsNullOrEmpty(accountInfo.WelcomeKeyword))
            {
                keyword = accountInfo.WelcomeKeyword;
                keywordID = DataProviderWX.KeywordMatchDAO.GetKeywordIDByMPController(publishmentSystemInfo.PublishmentSystemId, keyword);
            }

            KeywordInfo keywordInfo = null;
            if (keywordID > 0)
            {
                keywordInfo = DataProviderWX.KeywordDAO.GetKeywordInfo(keywordID);
            }

            return GetResponseMessage(keywordInfo, keyword);
        }

        public override IResponseMessageBase OnEvent_UnsubscribeRequest(RequestMessageEvent_Unsubscribe requestMessage)
        {
            DataProviderWX.CountDAO.AddCount(publishmentSystemInfo.PublishmentSystemId, ECountType.UserUnsubscribe);

            return base.OnEvent_UnsubscribeRequest(requestMessage);
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            /* 所有没有被处理的消息会默认返回这里的结果，
             * 因此，如果想把整个微信请求委托出去（例如需要使用分布式或从其他服务器获取请求），
             * 只需要在这里统一发出委托请求，如：
             * var responseMessage = MessageAgent.RequestResponseMessage(agentUrl, agentToken, RequestDocument.ToString());
             * return responseMessage;
             */

            if (accountInfo.IsDefaultReply && !string.IsNullOrEmpty(accountInfo.DefaultReplyKeyword))
            {
                var keyword = accountInfo.DefaultReplyKeyword;
                return GetResponseMessage(keyword);
            }

            return null;

            //var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            //responseMessage.Content = "这条消息来自DefaultResponseMessage。";
            //return responseMessage;
        }
    }
}
