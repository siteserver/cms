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
	public class AlbumManager
    {
        public static string GetImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl)
        {
            return PageUtils.AddProtocolToUrl(string.IsNullOrEmpty(imageUrl) ? SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/album/img/start.jpg") : PageUtility.ParseNavigationUrl(publishmentSystemInfo, imageUrl));
        }

        public static string GetContentImageUrl(PublishmentSystemInfo publishmentSystemInfo, string imageUrl)
        {
            return PageUtils.AddProtocolToUrl(PageUtility.ParseNavigationUrl(publishmentSystemInfo, imageUrl));
        }

        private static string GetAlbumUrl(PublishmentSystemInfo publishmentSystemInfo)
        {
            return PageUtils.AddProtocolToUrl(SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "weixin/album/index.html"));
        }

        public static string GetAlbumUrl(PublishmentSystemInfo publishmentSystemInfo, AlbumInfo albumInfo, string wxOpenId)
        {
            var attributes = new NameValueCollection
            {
                {"publishmentSystemID", albumInfo.PublishmentSystemID.ToString()},
                {"albumID", albumInfo.ID.ToString()},
                {"wxOpenID", wxOpenId}
            };
            return PageUtils.AddQueryString(GetAlbumUrl(publishmentSystemInfo), attributes);
        }

        public static List<Article> Trigger(PublishmentSystemInfo publishmentSystemInfo, Model.KeywordInfo keywordInfo, string wxOpenId)
        {
            var articleList = new List<Article>();

            DataProviderWX.CountDAO.AddCount(keywordInfo.PublishmentSystemID, ECountType.RequestNews);

            var albumInfoList = DataProviderWX.AlbumDAO.GetAlbumInfoListByKeywordID(keywordInfo.PublishmentSystemID, keywordInfo.KeywordID);

            foreach (var albumInfo in albumInfoList)
            {
                Article article = null;

                if (albumInfo != null)
                {
                    var imageUrl = GetImageUrl(publishmentSystemInfo, albumInfo.ImageUrl);
                    var pageUrl = GetAlbumUrl(publishmentSystemInfo, albumInfo, wxOpenId);

                    article = new Article
                    {
                        Title = albumInfo.Title,
                        Description = albumInfo.Summary,
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
