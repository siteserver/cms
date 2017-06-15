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
	public class CollectManager
    {
        //http://www.bootply.com/85625
        public static string GetImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/collect/img/start.jpg"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, imageUrl));
            }
        }

        public static string GetContentImageUrl(PublishmentSystemInfo publishmentSystemInfo, string contentImageUrl)
        {
            if (string.IsNullOrEmpty(contentImageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/collect/img/top.jpg"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, contentImageUrl));
            }
        }

        public static string GetEndImageUrl(PublishmentSystemInfo publishmentSystemInfo, string endImageUrl)
        {
            if (string.IsNullOrEmpty(endImageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/collect/img/end.jpg"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, endImageUrl));
            }
        }

        private static string GetCollectUrl(PublishmentSystemInfo publishmentSystemInfo)
        {
            return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/collect/index.html"));
        }

        public static string GetCollectUrl(PublishmentSystemInfo publishmentSystemInfo, CollectInfo collectInfo, string wxOpenId)
        {
            var attributes = new NameValueCollection
            {
                {"publishmentSystemID", collectInfo.PublishmentSystemId.ToString()},
                {"collectID", collectInfo.Id.ToString()},
                {"wxOpenID", wxOpenId}
            };
            return PageUtils.AddQueryString(GetCollectUrl(publishmentSystemInfo), attributes);
        }

        public static List<Article> Trigger(PublishmentSystemInfo publishmentSystemInfo, Model.KeywordInfo keywordInfo, string wxOpenID)
        {
            var articleList = new List<Article>();

            DataProviderWx.CountDao.AddCount(keywordInfo.PublishmentSystemId, ECountType.RequestNews);

            var collectInfoList = DataProviderWx.CollectDao.GetCollectInfoListByKeywordId(keywordInfo.PublishmentSystemId, keywordInfo.KeywordId);

            foreach (var collectInfo in collectInfoList)
            {
                Article article = null;

                if (collectInfo != null && collectInfo.StartDate < DateTime.Now)
                {
                    var isEnd = false;

                    if (collectInfo.EndDate < DateTime.Now)
                    {
                        isEnd = true;
                    }

                    if (isEnd)
                    {
                        var endImageUrl = GetEndImageUrl(publishmentSystemInfo, collectInfo.EndImageUrl);

                        article = new Article()
                        {
                            Title = collectInfo.EndTitle,
                            Description = collectInfo.EndSummary,
                            PicUrl = endImageUrl
                        };
                    }
                    else
                    {
                        var imageUrl = GetImageUrl(publishmentSystemInfo, collectInfo.ImageUrl);
                        var pageUrl = GetCollectUrl(publishmentSystemInfo, collectInfo, wxOpenID);

                        article = new Article()
                        {
                            Title = collectInfo.Title,
                            Description = collectInfo.Summary,
                            PicUrl = imageUrl,
                            Url = pageUrl
                        };
                    }
                }

                if (article != null)
                {
                    articleList.Add(article);
                }
            }

            return articleList;
        }

        public static void Vote(int publishmentSystemID, int collectID, int itemID, string ipAddress, string cookieSN, string wxOpenID, string userName)
        {
            DataProviderWx.CollectDao.AddUserCount(collectID);
            DataProviderWx.CollectItemDao.AddVoteNum(collectID, itemID);

            var logInfo = new CollectLogInfo { Id = 0, PublishmentSystemId = publishmentSystemID, CollectId = collectID, ItemId = itemID, IpAddress = ipAddress, CookieSn = cookieSN, WxOpenId = wxOpenID, UserName = userName, AddDate = DateTime.Now };
            DataProviderWx.CollectLogDao.Insert(logInfo);
        }
	}
}
