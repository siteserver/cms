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
	public class SearchManager
    {
        public static string GetImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/search/img/start.jpg"));
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
                return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/search/img/head_img.jpg"));
            }
            else
            {
                return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, imageUrl));
            }
        }

        public static string GetSearchUrl(PublishmentSystemInfo publishmentSystemInfo, SearchInfo searchInfo)
        {
            var attributes = new NameValueCollection();
            attributes.Add("publishmentSystemID", searchInfo.PublishmentSystemID.ToString());
            attributes.Add("searchID", searchInfo.ID.ToString());
            var url = PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/search/index.html"));
            return PageUtils.AddQueryString(url, attributes);
        }

        //public static List<ContentInfo> GetContentInfoList(PublishmentSystemInfo publishmentSystemInfo, int nodeID, string keywords)
        //{
        //    List<ContentInfo> contentInfoList = new List<ContentInfo>();
        //    if (nodeID > 0)
        //    {
        //        contentInfoList = DataProvider.BackgroundContentDAO.
        //    }
        //    if (!string.IsNullOrEmpty(keyWords))
        //    {
        //        contentInfoList = DataProvider.ContentDAO.GetContentInfoList(ETableStyle.Site, publishmentSystemInfo.AuxiliaryTableForContent, publishmentSystemID, nodeID, keyWords);
        //    }

        //    return contentInfoList;
        //}

        public static List<Article> Trigger(PublishmentSystemInfo publishmentSystemInfo, Model.KeywordInfo keywordInfo, string wxOpenID)
        {
            var articleList = new List<Article>();

            DataProviderWX.CountDAO.AddCount(keywordInfo.PublishmentSystemID, ECountType.RequestNews);

            var searchInfoList = DataProviderWX.SearchDAO.GetSearchInfoListByKeywordID(keywordInfo.PublishmentSystemID, keywordInfo.KeywordID);

            foreach (var searchInfo in searchInfoList)
            {
                Article article = null;

                if (searchInfo != null)
                {
                    var imageUrl = GetImageUrl(publishmentSystemInfo, searchInfo.ImageUrl);
                    var pageUrl = GetSearchUrl(publishmentSystemInfo, searchInfo);

                    article = new Article
                    {
                        Title = searchInfo.Title,
                        Description = searchInfo.Summary,
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
