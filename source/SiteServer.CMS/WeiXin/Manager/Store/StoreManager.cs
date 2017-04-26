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

namespace SiteServer.CMS.WeiXin.Manager.Store
{
    public class StoreManager
    {
        public static string GetImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/store/img/start.jpg"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, imageUrl));
            }
        }

        public static string GetContentImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl, int sequence)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, $"weixin/store/img/pic{sequence}.jpg"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, imageUrl));
            }
        }

        private static string GetStoreUrl(PublishmentSystemInfo publishmentSystemInfo)
        {
            return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/store/index.html"));
        }

        public static string GetStoreUrl(PublishmentSystemInfo publishmentSystemInfo, StoreInfo storeInfo, string wxOpenID)
        {
            var attributes = new NameValueCollection();
            attributes.Add("publishmentSystemID", storeInfo.PublishmentSystemID.ToString());
            attributes.Add("storeID", storeInfo.ID.ToString());
            attributes.Add("wxOpenID", wxOpenID);
            attributes.Add("parentID", "0");
            return PageUtils.AddQueryString(GetStoreUrl(publishmentSystemInfo), attributes);
        }

        private static string GetStoreItemUrl(PublishmentSystemInfo publishmentSystemInfo)
        {
            return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/store/content.html"));
        }

        public static string GetStoreItemUrl(PublishmentSystemInfo publishmentSystemInfo, StoreItemInfo storeItemInfo, string wxOpenID)
        {
            var attributes = new NameValueCollection();
            attributes.Add("publishmentSystemID", storeItemInfo.PublishmentSystemID.ToString());
            attributes.Add("storeID", storeItemInfo.StoreID.ToString());
            attributes.Add("storeItemID", storeItemInfo.ID.ToString());
            attributes.Add("wxOpenID", wxOpenID);
            return PageUtils.AddQueryString(GetStoreItemUrl(publishmentSystemInfo), attributes);
        }

        public static List<Article> Trigger(PublishmentSystemInfo publishmentSystemInfo, Model.KeywordInfo keywordInfo, string wxOpenID)
        {
            var articleList = new List<Article>();

            DataProviderWX.CountDAO.AddCount(keywordInfo.PublishmentSystemID, ECountType.RequestNews);

            var storeInfoList = DataProviderWX.StoreDAO.GetStoreInfoListByKeywordID(keywordInfo.PublishmentSystemID, keywordInfo.KeywordID);

            foreach (var storeInfo in storeInfoList)
            {
                Article article = null;

                if (storeInfo != null)
                {
                    var imageUrl = GetImageUrl(publishmentSystemInfo, storeInfo.ImageUrl);
                    var pageUrl = GetStoreUrl(publishmentSystemInfo, storeInfo, wxOpenID);

                    article = new Article()
                    {
                        Title = storeInfo.Title,
                        Description = storeInfo.Summary,
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

        public static List<Article> TriggerStoreItem(PublishmentSystemInfo publishmentSystemInfo, string location_X, string location_Y, string wxOpenID)
        {
            var articleList = new List<Article>();

            var storeItemInfoList = DataProviderWX.StoreItemDAO.GetAllStoreItemInfoListByLocation(publishmentSystemInfo.PublishmentSystemId, location_X);

            foreach (var storeItemInfo in storeItemInfoList)
            {
                Article article = null;

                if (storeItemInfo != null)
                {
                    var imageUrl = GetImageUrl(publishmentSystemInfo, storeItemInfo.ImageUrl);
                    var pageUrl = GetStoreItemUrl(publishmentSystemInfo, storeItemInfo, wxOpenID);

                    var beginPointStore = new Point(Convert.ToDouble(location_X), Convert.ToDouble(location_Y));
                    var endPointStore = new Point(Convert.ToDouble(storeItemInfo.Latitude), Convert.ToDouble(storeItemInfo.Longitude));

                    var NewTitile = storeItemInfo.StoreName + "，距离" + Earth.GetDistance(beginPointStore, endPointStore) * 1000 + "米";

                    article = new Article()
                    {
                        Title = NewTitile,
                        Description = storeItemInfo.Summary,
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

    public class Earth
    {
        /// <summary>  
        /// 地球的半径  
        /// </summary>  
        public const double EARTH_RADIUS = 6378.137;

        /// <summary>  
        /// 计算坐标点的距离  
        /// </summary>  
        /// <param name="begin">开始的经度纬度</param>  
        /// <param name="end">结束的经度纬度</param>  
        /// <returns>距离(公里)</returns>  
        public static double GetDistance(Point begin, Point end)
        {
            var lat = begin.RadLat - end.RadLat;
            var lng = begin.RadLng - end.RadLng;

            var dis = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(lat / 2), 2) + Math.Cos(begin.RadLat) * Math.Cos(end.RadLat) * Math.Pow(Math.Sin(lng / 2), 2)));
            dis = dis * EARTH_RADIUS;
            dis = Math.Round(dis * 1e4) / 1e4;

            return dis;
        }
    }

    /// <summary>  
    /// 代表经度, 纬度  
    /// </summary>  
    public class Point
    {
        /// <param name="lat">纬度 X</param>  
        /// <param name="lng">经度 Y</param>  
        public Point(double lat, double lng)
        {
            this.lat = lat;
            this.lng = lng;
        }

        //  纬度 X  
        private double lat;

        // 经度 Y  
        private double lng;

        /// <summary>  
        /// 代表纬度 X轴  
        /// </summary>  
        public double Lat { set; get; }

        /// <summary>  
        /// 代表经度 Y轴  
        /// </summary>  
        public double Lng { get; set; }

        public double RadLat { get { return lat * Math.PI / 180; } }

        public double RadLng { get { return lng * Math.PI / 180; } }
    }
}
