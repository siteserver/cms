using System.Collections.Generic;
using SiteServer.CMS.Model;
using SiteServer.CMS.WeiXin.Manager.Store;
using SiteServer.CMS.WeiXin.WeiXinMP.Entities.GoogleMap;
using SiteServer.CMS.WeiXin.WeiXinMP.Entities.Request;
using SiteServer.CMS.WeiXin.WeiXinMP.Entities.Response;
using SiteServer.CMS.WeiXin.WeiXinMP.Helpers;

namespace SiteServer.CMS.WeiXin.MP
{
    public class LocationService
    {
        public ResponseMessageNews GetResponseMessage(PublishmentSystemInfo publishmentSystemInfo, RequestMessageLocation requestMessage, string wxOpenID)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage);

            var articleList = StoreManager.TriggerStoreItem(publishmentSystemInfo, requestMessage.Location_X.ToString(), requestMessage.Location_Y.ToString(), wxOpenID);

            var markersList = new List<Markers>();
            markersList.Add(new Markers()
            {
                X = requestMessage.Location_X,
                Y = requestMessage.Location_Y,
                Color = "red",
                Label = "S",
                Size = MarkerSize.Default,
            });
            var mapSize = "480x600";
            var mapUrl = GoogleMapHelper.GetGoogleStaticMap(19 /*requestMessage.Scale*//*微信和GoogleMap的Scale不一致，这里建议使用固定值*/,
                                                            markersList, mapSize);
            responseMessage.Articles.Add(new Article()
            {
                Description =
                    $"根据您的地理位置获取的附近门店。Location_X：{requestMessage.Location_X}，Location_Y：{requestMessage.Location_Y}，Scale：{requestMessage.Scale}，标签：{requestMessage.Label}",
                PicUrl = articleList[0].PicUrl,
                Title = articleList[0].Title,
                Url = articleList[0].Url
            });

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