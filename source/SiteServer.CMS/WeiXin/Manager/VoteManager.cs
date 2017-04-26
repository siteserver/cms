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
	public class VoteManager
    {
        //http://www.bootply.com/85625
        public static string GetImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/vote/img/start.jpg"));
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
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/vote/img/top.jpg"));
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
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/vote/img/end.jpg"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, endImageUrl));
            }
        }

        private static string GetVoteUrl(PublishmentSystemInfo publishmentSystemInfo, VoteInfo voteInfo)
        {
            if (TranslateUtils.ToBool(voteInfo.ContentIsImageOption))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/vote/image.html"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/vote/text.html"));
            }
        }

        public static string GetVoteUrl(PublishmentSystemInfo publishmentSystemInfo, VoteInfo voteInfo, string wxOpenID)
        {
            var attributes = new NameValueCollection();
            attributes.Add("voteID", voteInfo.ID.ToString());
            attributes.Add("publishmentSystemID", voteInfo.PublishmentSystemID.ToString());
            attributes.Add("wxOpenID", wxOpenID);
            return PageUtils.AddQueryString(GetVoteUrl(publishmentSystemInfo, voteInfo), attributes);
        }

        public static List<Article> Trigger(PublishmentSystemInfo publishmentSystemInfo, Model.KeywordInfo keywordInfo, string wxOpenID)
        {
            var articleList = new List<Article>();

            DataProviderWX.CountDAO.AddCount(keywordInfo.PublishmentSystemID, ECountType.RequestNews);

            var voteInfoList = DataProviderWX.VoteDAO.GetVoteInfoListByKeywordID(keywordInfo.PublishmentSystemID, keywordInfo.KeywordID);

            foreach (var voteInfo in voteInfoList)
            {
                Article article = null;

                if (voteInfo != null && voteInfo.StartDate < DateTime.Now)
                {
                    var isEnd = false;

                    if (voteInfo.EndDate < DateTime.Now)
                    {
                        isEnd = true;
                    }

                    if (isEnd)
                    {
                        var endImageUrl = GetEndImageUrl(publishmentSystemInfo, voteInfo.EndImageUrl);

                        article = new Article()
                        {
                            Title = voteInfo.EndTitle,
                            Description = voteInfo.EndSummary,
                            PicUrl = endImageUrl
                        };
                    }
                    else
                    {
                        var imageUrl = GetImageUrl(publishmentSystemInfo, voteInfo.ImageUrl);
                        var pageUrl = GetVoteUrl(publishmentSystemInfo, voteInfo, wxOpenID);

                        article = new Article()
                        {
                            Title = voteInfo.Title,
                            Description = voteInfo.Summary,
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

        public static void Vote(int publishmentSystemID, int voteID, List<int> itemIDList, string ipAddress, string cookieSN, string wxOpenID, string userName)
        {
            DataProviderWX.VoteDAO.AddUserCount(voteID);
            DataProviderWX.VoteItemDAO.AddVoteNum(voteID, itemIDList);
            var logInfo = new VoteLogInfo { ID = 0, PublishmentSystemID = publishmentSystemID, VoteID = voteID, ItemIDCollection = TranslateUtils.ObjectCollectionToString(itemIDList), IPAddress = ipAddress, CookieSN = cookieSN, WXOpenID = wxOpenID, UserName = userName, AddDate = DateTime.Now };
            DataProviderWX.VoteLogDAO.Insert(logInfo);
        }
	}
}
