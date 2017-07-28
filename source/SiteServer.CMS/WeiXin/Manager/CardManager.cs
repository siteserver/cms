using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;
using SiteServer.CMS.WeiXin.WeiXinMP.Entities.Response;

namespace SiteServer.CMS.WeiXin.Manager
{
	public class CardManager
    {
        public static string GetImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/card/img/start.jpg"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, imageUrl));
            }
        }

        public static string GetContentFrontImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/card/img/front.png"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, imageUrl));
            }
        }

        public static string GetContentBackImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/card/img/back.png"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, imageUrl));
            }
        }

        private static string GetCardUrl(PublishmentSystemInfo publishmentSystemInfo)
        {
            return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/card/index.html"));
        }

        public static string GetCardUrl(PublishmentSystemInfo publishmentSystemInfo, CardInfo cardInfo, string wxOpenId)
        {
            var attributes = new NameValueCollection
            {
                {"publishmentSystemID", cardInfo.PublishmentSystemID.ToString()},
                {"cardID", cardInfo.ID.ToString()},
                {"wxOpenID", wxOpenId}
            };
            return PageUtils.AddQueryString(GetCardUrl(publishmentSystemInfo), attributes);
        }

        public static int GetSignCredits(PublishmentSystemInfo publishmentSystemInfo, string userName)
        {
            var credits = 0;
            var signCreditsConfigure = publishmentSystemInfo.Additional.CardSignCreditsConfigure;
            if (!string.IsNullOrEmpty(signCreditsConfigure))
            {
                var days = 0;
                var curDateTime = DateTime.Now;
                var signDateList = DataProviderWX.CardSignLogDAO.GetSignDateList(publishmentSystemInfo.PublishmentSystemId, userName);
                foreach (var dateTime in signDateList)
                {
                    days++;
                    curDateTime= curDateTime.AddDays(-1);
                    if ((curDateTime.Day - dateTime.Day) > 0)
                    { 
                        break;
                    }
               }

                var index = 0;
                var configureList = TranslateUtils.StringCollectionToStringList(signCreditsConfigure);
                foreach (string configure in configureList)
                {
                    if (!string.IsNullOrEmpty(configure))
                    {
                        index++;
                        var dayFrom = TranslateUtils.ToInt(configure.Split('&')[0]);
                        var dayTo = TranslateUtils.ToInt(configure.Split('&')[1]);
                        var singCredits = TranslateUtils.ToInt(configure.Split('&')[2]);
                        if (days == 0)
                        {
                            credits = singCredits;
                            break;
                        }
                        else if (days >= dayFrom && days < dayTo)
                        {
                            credits = singCredits;
                            break;
                        }
                        else if (days > dayTo)
                        {
                            if (index == configureList.Count)
                            {
                                credits = singCredits;
                            }
                        }
                    }
                }
            }

            return credits;

        }
        
        public static List<Article> Trigger(PublishmentSystemInfo publishmentSystemInfo, Model.KeywordInfo keywordInfo, string wxOpenID)
        { 
            var articleList = new List<Article>();
             
            DataProviderWX.CountDAO.AddCount(keywordInfo.PublishmentSystemID, ECountType.RequestNews);

            var CardInfoList = DataProviderWX.CardDAO.GetCardInfoListByKeywordID(keywordInfo.PublishmentSystemID, keywordInfo.KeywordID);

            foreach (var cardInfo in CardInfoList)
            {
                Article article = null;

                if (cardInfo != null)
                {
                    var imageUrl = GetImageUrl(publishmentSystemInfo, cardInfo.ImageUrl);
                    var pageUrl = GetCardUrl(publishmentSystemInfo, cardInfo, wxOpenID);

                    article = new Article()
                    {
                        Title = cardInfo.Title,
                        Description = cardInfo.Summary,
                        PicUrl = imageUrl,
                        Url = pageUrl
                    };
                }

                if (article != null)
                {
                    articleList.Add(article);
                }
            }

            return articleList;
        }
	}
}
