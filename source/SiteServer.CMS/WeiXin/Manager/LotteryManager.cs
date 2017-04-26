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
    public class LotteryManager
    {
        public static string GetImageUrl(PublishmentSystemInfo publishmentSystemInfo, ELotteryType lotteryType, string imageUrl)
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, imageUrl));
            }
            var directoryName = "img" + ELotteryTypeUtils.GetValue(lotteryType);
            return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, $"weixin/lottery/{directoryName}/start.jpg"));
        }

        public static string GetContentImageUrl(PublishmentSystemInfo publishmentSystemInfo, ELotteryType lotteryType, string contentImageUrl)
        {
            if (!string.IsNullOrEmpty(contentImageUrl))
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, contentImageUrl));
            }
            var directoryName = "img" + ELotteryTypeUtils.GetValue(lotteryType);
            return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, $"weixin/lottery/{directoryName}/content.jpg"));
        }

        public static string GetContentAwardImageUrl(PublishmentSystemInfo publishmentSystemInfo, ELotteryType lotteryType, string contentImageUrl, int awardCount)
        {
            if (!string.IsNullOrEmpty(contentImageUrl))
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, contentImageUrl));
            }
            var fileName = awardCount + ".png";
            if (awardCount < 2 || awardCount > 9)
            {
                fileName = "contentAward.png";
            }
            var directoryName = "img" + ELotteryTypeUtils.GetValue(lotteryType);
            return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, $"weixin/lottery/{directoryName}/{fileName}"));
        }

        public static string GetAwardImageUrl(PublishmentSystemInfo publishmentSystemInfo, ELotteryType lotteryType, string contentImageUrl)
        {
            if (!string.IsNullOrEmpty(contentImageUrl))
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, contentImageUrl));
            }
            var directoryName = "img" + ELotteryTypeUtils.GetValue(lotteryType);
            return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, $"weixin/lottery/{directoryName}/award.jpg"));
        }

        public static string GetEndImageUrl(PublishmentSystemInfo publishmentSystemInfo, ELotteryType lotteryType, string endImageUrl)
        {
            if (!string.IsNullOrEmpty(endImageUrl))
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, endImageUrl));
            }
            var directoryName = "img" + ELotteryTypeUtils.GetValue(lotteryType);
            return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, $"weixin/lottery/{directoryName}/end.jpg"));
        }

        private static string GetLotteryUrl(PublishmentSystemInfo publishmentSystemInfo, LotteryInfo lotteryInfo)
        {
            var fileName = ELotteryTypeUtils.GetValue(ELotteryTypeUtils.GetEnumType(lotteryInfo.LotteryType)).ToLower();
            return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, $"weixin/lottery/{fileName}.html"));
        }

        public static string GetLotteryUrl(PublishmentSystemInfo publishmentSystemInfo, LotteryInfo lotteryInfo, string wxOpenId)
        {
            var attributes = new NameValueCollection
            {
                {"lotteryID", lotteryInfo.ID.ToString()},
                {"publishmentSystemID", lotteryInfo.PublishmentSystemID.ToString()},
                {"wxOpenID", wxOpenId}
            };

            return PageUtils.AddQueryString(GetLotteryUrl(publishmentSystemInfo, lotteryInfo), attributes);
        }

        public static string GetLotteryTemplateDirectoryUrl(PublishmentSystemInfo publishmentSystemInfo)
        {
            return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/lottery"));
        }

        private static LotteryAwardInfo GetAwardInfo(List<LotteryAwardInfo> awardInfoList, int awardId)
        {
            LotteryAwardInfo awardInfo = null;
            foreach (var lotteryAwardInfo in awardInfoList)
            {
                if (lotteryAwardInfo.ID == awardId)
                {
                    awardInfo = lotteryAwardInfo;
                    break;
                }
            }
            return awardInfo;
        }

        public static bool Lottery(LotteryInfo lotteryInfo, List<LotteryAwardInfo> awardInfoList, string cookieSN, string wxOpenID, out LotteryAwardInfo awardInfo, out LotteryWinnerInfo winnerInfo, string userName, out string errorMessage)
        {
            errorMessage = string.Empty;

            awardInfo = null;
            winnerInfo = DataProviderWX.LotteryWinnerDAO.GetWinnerInfo(lotteryInfo.PublishmentSystemID, lotteryInfo.ID, cookieSN, wxOpenID, userName);
            if (winnerInfo != null)
            {
                awardInfo = GetAwardInfo(awardInfoList, winnerInfo.AwardID);
            }
            else
            {
                var isMaxCount = false;
                var isMaxDailyCount = false;

                DataProviderWX.LotteryLogDAO.AddCount(lotteryInfo.PublishmentSystemID, lotteryInfo.ID, cookieSN, wxOpenID, userName, lotteryInfo.AwardMaxCount, lotteryInfo.AwardMaxDailyCount, out isMaxCount, out isMaxDailyCount);

                if (isMaxCount)
                {
                    errorMessage = $"对不起，每人最多允许抽奖{lotteryInfo.AwardMaxCount}次";
                    return false;
                }
                else if (isMaxDailyCount)
                {
                    errorMessage = $"对不起，每人每天最多允许抽奖{lotteryInfo.AwardMaxDailyCount}次";
                    return false;
                }
                else
                {
                    if (awardInfoList != null && awardInfoList.Count > 0)
                    {
                        var idWithProbabilityDictionary = new Dictionary<int, decimal>();
                        foreach (var lotteryAwardInfo in awardInfoList)
                        {
                            idWithProbabilityDictionary.Add(lotteryAwardInfo.ID, lotteryAwardInfo.Probability);
                        }

                        var awardID = WeiXinManager.Lottery(idWithProbabilityDictionary);
                        if (awardID > 0)
                        {
                            var lotteryAwardInfo = GetAwardInfo(awardInfoList, awardID);

                            if (lotteryAwardInfo != null && lotteryAwardInfo.TotalNum > 0)
                            {
                                var wonNum = DataProviderWX.LotteryWinnerDAO.GetTotalNum(awardID);
                                if (lotteryAwardInfo.TotalNum > wonNum)
                                {
                                    awardInfo = lotteryAwardInfo;
                                    winnerInfo = new LotteryWinnerInfo { PublishmentSystemID = lotteryInfo.PublishmentSystemID, LotteryType = lotteryInfo.LotteryType, LotteryID = lotteryInfo.ID, AwardID = awardID, Status = EWinStatusUtils.GetValue(EWinStatus.Won), CookieSN = cookieSN, WXOpenID = wxOpenID, UserName = userName, AddDate = DateTime.Now };
                                    winnerInfo.ID = DataProviderWX.LotteryWinnerDAO.Insert(winnerInfo);

                                    DataProviderWX.LotteryAwardDAO.UpdateWonNum(awardID);

                                    DataProviderWX.LotteryDAO.AddUserCount(winnerInfo.LotteryID);
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        public static void AddApplication(int winnerID, string realName, string mobile, string email, string address)
        {
            var winnerInfo = DataProviderWX.LotteryWinnerDAO.GetWinnerInfo(winnerID);

            var oldCashSN = winnerInfo.CashSN;

            winnerInfo.RealName = realName;
            winnerInfo.Mobile = mobile;
            winnerInfo.Email = email;
            winnerInfo.Address = address;
            winnerInfo.Status = EWinStatusUtils.GetValue(EWinStatus.Applied);
            winnerInfo.CashSN = StringUtils.GetShortGuid(true);

            if (string.IsNullOrEmpty(oldCashSN))
            {
                DataProviderWX.LotteryWinnerDAO.Update(winnerInfo);
            }
        }

        public static List<Article> Trigger(Model.KeywordInfo keywordInfo, ELotteryType lotteryType, string requestFromUserName)
        {
            var articleList = new List<Article>();

            DataProviderWX.CountDAO.AddCount(keywordInfo.PublishmentSystemID, ECountType.RequestNews);

            var lotteryInfoList = DataProviderWX.LotteryDAO.GetLotteryInfoListByKeywordID(keywordInfo.PublishmentSystemID, lotteryType, keywordInfo.KeywordID);

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(keywordInfo.PublishmentSystemID);

            foreach (var lotteryInfo in lotteryInfoList)
            {
                Article article = null;

                if (lotteryInfo != null && lotteryInfo.StartDate < DateTime.Now)
                {
                    var isEnd = false;

                    if (lotteryInfo.EndDate < DateTime.Now)
                    {
                        isEnd = true;
                    }

                    if (isEnd)
                    {
                        var endImageUrl = GetEndImageUrl(publishmentSystemInfo, lotteryType, lotteryInfo.EndImageUrl);

                        article = new Article()
                        {
                            Title = lotteryInfo.EndTitle,
                            Description = lotteryInfo.EndSummary,
                            PicUrl = endImageUrl
                        };
                    }
                    else
                    {
                        var imageUrl = GetImageUrl(publishmentSystemInfo, lotteryType, lotteryInfo.ImageUrl);
                        var pageUrl = GetLotteryUrl(publishmentSystemInfo, lotteryInfo, requestFromUserName);

                        article = new Article()
                        {
                            Title = lotteryInfo.Title,
                            Description = lotteryInfo.Summary,
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
    }
}
