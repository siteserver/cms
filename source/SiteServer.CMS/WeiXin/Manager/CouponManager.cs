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
    public class CouponManager
    {

        public static List<string> GetCouponSN(int totalNum)
        {
            var list = new List<string>();

            for (var i = 1; i <= totalNum; i++)
            {
                list.Add(StringUtils.GetShortGuid(true));
            }

            return list;
        }

        public static string GetImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/coupon/images/start.jpg"));
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
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/coupon/images/content.jpg"));
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
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/coupon/images/end.jpg"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, endImageUrl));
            }
        }

        public static string GetCouponUrl(PublishmentSystemInfo publishmentSystemInfo)
        {
            return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/coupon/application.html"));
        }

        public static string GetCouponHoldUrl(PublishmentSystemInfo publishmentSystemInfo, int keywordID, int actID, int snID, string keyword)
        {
            var attributes = new NameValueCollection();
            attributes.Add("snID", snID.ToString());
            attributes.Add("keyword", keyword);
            return PageUtils.AddQueryString(GetCouponUrl(publishmentSystemInfo), attributes);
        }
        public static string GetCouponHoldUrl(PublishmentSystemInfo publishmentSystemInfo, int actID)
        {
            var attributes = new NameValueCollection();

            attributes.Add("actID", actID.ToString());
            attributes.Add("PublishmentSystemID", publishmentSystemInfo.PublishmentSystemId.ToString());

            return PageUtils.AddQueryString(GetCouponUrl(publishmentSystemInfo), attributes);
        }

        public static string GetCouponActTemplateDirectoryUrl(PublishmentSystemInfo publishmentSystemInfo)
        {
            return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/coupon"));
        }

        public static List<Article> Trigger(Model.KeywordInfo keywordInfo, string keyword, string requestFromUserName)
        {
            var articleList = new List<Article>();

            DataProviderWX.CountDAO.AddCount(keywordInfo.PublishmentSystemID, ECountType.RequestNews);

            var actInfoList = DataProviderWX.CouponActDAO.GetActInfoListByKeywordID(keywordInfo.PublishmentSystemID, keywordInfo.KeywordID);

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(keywordInfo.PublishmentSystemID);

            foreach (var actInfo in actInfoList)
            {
                Article article = null;

                if (actInfo != null && actInfo.StartDate < DateTime.Now)
                {
                    var isEnd = false;
                    var snID = 0;

                    if (actInfo.EndDate < DateTime.Now)
                    {
                        isEnd = true;
                    }
                    else
                    {
                        snID = DataProviderWX.CouponSNDAO.Hold(keywordInfo.PublishmentSystemID, actInfo.ID, requestFromUserName);
                        if (snID == 0)
                        {
                            isEnd = true;
                        }
                    }

                    if (isEnd)
                    {
                        var endImageUrl = GetEndImageUrl(publishmentSystemInfo, actInfo.EndImageUrl);

                        article = new Article()
                        {
                            Title = actInfo.EndTitle,
                            Description = actInfo.EndSummary,
                            PicUrl = endImageUrl
                        };
                    }
                    else
                    {
                        var imageUrl = GetImageUrl(publishmentSystemInfo, actInfo.ImageUrl);
                        var pageUrl = GetCouponHoldUrl(publishmentSystemInfo, actInfo.ID);

                        article = new Article()
                        {
                            Title = actInfo.Title,
                            Description = actInfo.Summary,
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

        public static int AddApplication(int publishmentSystemID, int actID, string uniqueID, string realName, string mobile, string email, string address)
        {
            try
            {
                var couponInfoList = DataProviderWX.CouponDAO.GetCouponInfoList(publishmentSystemID, actID);

                var snID = 0;
                var couponID = 0;
                var cookieSN = WeiXinManager.GetCookieSN();
                snID = DataProviderWX.CouponSNDAO.Hold(publishmentSystemID, actID, cookieSN);

                var newCouponSNInfo = DataProviderWX.CouponSNDAO.GetSNInfo(snID);
                couponID = newCouponSNInfo.CouponID;

                var couponSNInfo = new CouponSNInfo();

                couponSNInfo.PublishmentSystemID = publishmentSystemID;
                couponSNInfo.CookieSN = cookieSN;
                couponSNInfo.CouponID = couponID;
                couponSNInfo.ID = snID;
                couponSNInfo.HoldDate = DateTime.Now;
                couponSNInfo.HoldRealName = realName;
                couponSNInfo.HoldMobile = mobile;
                couponSNInfo.HoldEmail = email;
                couponSNInfo.HoldAddress = address;
                couponSNInfo.CashDate = DateTime.Now;
                couponSNInfo.Status = ECouponStatusUtils.GetValue(ECouponStatus.Hold);
                couponSNInfo.WXOpenID = uniqueID;

                if (newCouponSNInfo.Status == ECouponStatusUtils.GetValue(ECouponStatus.Cash))
                {
                    couponSNInfo.Status = ECouponStatusUtils.GetValue(ECouponStatus.Cash);
                }
                DataProviderWX.CouponSNDAO.Update(couponSNInfo);

                return newCouponSNInfo.ID;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
    }
}
