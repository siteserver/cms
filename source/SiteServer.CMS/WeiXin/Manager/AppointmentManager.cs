using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model.Enumerations;
using SiteServer.CMS.WeiXin.WeiXinMP.Entities.Response;

namespace SiteServer.CMS.WeiXin.Manager
{
	public class AppointmentManager
    {
        public static string GetImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/appointment/img/start.jpg"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, imageUrl));
            }
        }

        public static string GetItemTopImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/appointment/img/item.jpg"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, imageUrl));
            }
        }

        public static string GetContentImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/appointment/img/top.jpg"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, imageUrl));
            }
        }

        public static string GetContentResultTopImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/appointment/img/result.jpg"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, imageUrl));
            }
        }


        public static string GetEndImageUrl(PublishmentSystemInfo publishmentSystemInfo, string endImageUrl)
        {
            if (string.IsNullOrEmpty(endImageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/appointment/img/end.jpg"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, endImageUrl));
            }
        }

        public static string GetIndexUrl(PublishmentSystemInfo publishmentSystemInfo, int appointmentID, string wxOpenID)
        {
            var attributes = new NameValueCollection();
            attributes.Add("publishmentSystemID", publishmentSystemInfo.PublishmentSystemId.ToString());
            attributes.Add("appointmentID", appointmentID.ToString());
            attributes.Add("wxOpenID", wxOpenID);
            var url = PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/appointment/index.html"));
            return PageUtils.AddQueryString(url, attributes);
        }

        public static string GetItemUrl(PublishmentSystemInfo publishmentSystemInfo, int appointmentID, int itemID, string wxOpenID)
        {
            var attributes = new NameValueCollection();
            attributes.Add("publishmentSystemID", publishmentSystemInfo.PublishmentSystemId.ToString());
            attributes.Add("appointmentID", appointmentID.ToString());
            attributes.Add("itemID", itemID.ToString());
            attributes.Add("wxOpenID", wxOpenID);
            var url = PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/appointment/item.html"));
            return PageUtils.AddQueryString(url, attributes);
        }

        public static List<Article> Trigger(Model.KeywordInfo keywordInfo, string wxOpenID)
        {
            var articleList = new List<Article>();

            DataProviderWX.CountDAO.AddCount(keywordInfo.PublishmentSystemID, ECountType.RequestNews);

            var appointmentInfoList = DataProviderWX.AppointmentDAO.GetAppointmentInfoListByKeywordID(keywordInfo.PublishmentSystemID, keywordInfo.KeywordID);

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(keywordInfo.PublishmentSystemID);

            foreach (var appointmentInfo in appointmentInfoList)
            {
                Article article = null;

                if (appointmentInfo != null && appointmentInfo.StartDate < DateTime.Now)
                {
                    var isEnd = false;

                    if (appointmentInfo.EndDate < DateTime.Now)
                    {
                        isEnd = true;
                    }

                    if (isEnd)
                    {
                        var endImageUrl = GetEndImageUrl(publishmentSystemInfo, appointmentInfo.EndImageUrl);

                        article = new Article()
                        {
                            Title = appointmentInfo.EndTitle,
                            Description = appointmentInfo.EndSummary,
                            PicUrl = endImageUrl
                        };
                    }
                    else
                    {
                        var imageUrl = GetImageUrl(publishmentSystemInfo, appointmentInfo.ImageUrl);
                        var pageUrl = GetIndexUrl(publishmentSystemInfo, appointmentInfo.ID, wxOpenID);
                        if (appointmentInfo.ContentIsSingle)
                        {
                            var itemID = DataProviderWX.AppointmentItemDAO.GetItemID(publishmentSystemInfo.PublishmentSystemId, appointmentInfo.ID);
                            pageUrl = GetItemUrl(publishmentSystemInfo, appointmentInfo.ID, itemID, wxOpenID);
                        }

                        article = new Article()
                        {
                            Title = appointmentInfo.Title,
                            Description = appointmentInfo.Summary,
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
