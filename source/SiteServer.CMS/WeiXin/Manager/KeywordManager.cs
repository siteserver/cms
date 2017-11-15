using BaiRong.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Manager.Store;
using SiteServer.CMS.WeiXin.Model.Enumerations;
using KeywordInfo = SiteServer.CMS.WeiXin.Model.KeywordInfo;

namespace SiteServer.CMS.WeiXin.Manager
{
	public class KeywordManager
    {
        public static bool IsExists(int publishmentSystemID, string keyword)
        {
            return DataProviderWx.KeywordMatchDao.IsExists(publishmentSystemID, keyword);
        }

        public static bool IsKeywordInsertConflict(int publishmentSystemID, string keywords, out string conflictKeywords)
        {
            var isConflict = false;
            conflictKeywords = string.Empty;

            foreach (var str in TranslateUtils.StringCollectionToStringList(keywords, ' '))
            {
                var keyword = str.Trim();
                if (!string.IsNullOrEmpty(keyword))
                {
                    if (DataProviderWx.KeywordMatchDao.IsExists(publishmentSystemID, keyword))
                    {
                        isConflict = true;
                        conflictKeywords += keyword + " ";
                    }
                }
            }
            conflictKeywords = conflictKeywords.Trim();

            return isConflict;
        }

        public static bool IsKeywordUpdateConflict(int publishmentSystemID, string keywordsNow, string keywordsUpdate, out string conflictKeywords)
        {
            var isConflict = false;
            conflictKeywords = string.Empty;

            if (keywordsNow != keywordsUpdate)
            {
                var keywordListNow = TranslateUtils.StringCollectionToStringList(keywordsNow, ' ');
                foreach (var str in TranslateUtils.StringCollectionToStringList(keywordsUpdate, ' '))
                {
                    var keyword = str.Trim();
                    if (!string.IsNullOrEmpty(keyword) && !keywordListNow.Contains(keyword))
                    {
                        if (DataProviderWx.KeywordMatchDao.IsExists(publishmentSystemID, keyword))
                        {
                            isConflict = true;
                            conflictKeywords += keyword + " ";
                        }
                    }
                }

                conflictKeywords = conflictKeywords.Trim();
            }

            return isConflict;
        }

        public static bool IsKeywordUpdateConflict(int publishmentSystemID, int keywordID, string keywordsUpdate, out string conflictKeywords)
        {
            var keywordsNow = DataProviderWx.KeywordDao.GetKeywords(keywordID);

            return IsKeywordUpdateConflict(publishmentSystemID, keywordsNow, keywordsUpdate, out conflictKeywords);
        }

        public static string GetFunctionName(EKeywordType keywordType, int functionID)
        {
            var functionName = string.Empty;

            if (functionID > 0)
            {
                if (keywordType == EKeywordType.Album)
                {
                    var albumInfo = DataProviderWx.AlbumDao.GetAlbumInfo(functionID);
                    if (albumInfo != null)
                    {
                        functionName = albumInfo.Title;
                    }
                }
                else if (keywordType == EKeywordType.Appointment)
                {
                    var appointmentInfo = DataProviderWx.AppointmentDao.GetAppointmentInfo(functionID);
                    if (appointmentInfo != null)
                    {
                        functionName = appointmentInfo.Title;
                    }
                }
                else if (keywordType == EKeywordType.Conference)
                {
                    var conferenceInfo = DataProviderWx.ConferenceDao.GetConferenceInfo(functionID);
                    if (conferenceInfo != null)
                    {
                        functionName = conferenceInfo.Title;
                    }
                }
                else if (keywordType == EKeywordType.Coupon)
                {
                    var couponActInfo = DataProviderWx.CouponActDao.GetActInfo(functionID);
                    if (couponActInfo != null)
                    {
                        functionName = couponActInfo.Title;
                    }
                }

                else if (keywordType == EKeywordType.Scratch || keywordType == EKeywordType.BigWheel || keywordType == EKeywordType.GoldEgg || keywordType == EKeywordType.Flap || keywordType == EKeywordType.YaoYao)
                {
                    var lotteryInfo = DataProviderWx.LotteryDao.GetLotteryInfo(functionID);
                    if (lotteryInfo != null)
                    {
                        functionName = lotteryInfo.Title;
                    }
                }

                else if (keywordType == EKeywordType.Map)
                {
                    var mapInfo = DataProviderWx.MapDao.GetMapInfo(functionID);
                    if (mapInfo != null)
                    {
                        functionName = mapInfo.Title;
                    }
                }
                else if (keywordType == EKeywordType.Message)
                {
                    var messageInfo = DataProviderWx.MessageDao.GetMessageInfo(functionID);
                    if (messageInfo != null)
                    {
                        functionName = messageInfo.Title;
                    }
                }
                else if (keywordType == EKeywordType.Search)
                {
                    var searchInfo = DataProviderWx.SearchDao.GetSearchInfo(functionID);
                    if (searchInfo != null)
                    {
                        functionName = searchInfo.Title;
                    }
                }
                else if (keywordType == EKeywordType.Store)
                {
                    var storeInfo = DataProviderWx.StoreDao.GetStoreInfo(functionID);
                    if (storeInfo != null)
                    {
                        functionName = storeInfo.Title;
                    }
                }
                else if (keywordType == EKeywordType.View360)
                {
                    var view360Info = DataProviderWx.View360Dao.GetView360Info(functionID);
                    if (view360Info != null)
                    {
                        functionName = view360Info.Title;
                    }
                }
                else if (keywordType == EKeywordType.Vote)
                {
                    var voteInfo = DataProviderWx.VoteDao.GetVoteInfo(functionID);
                    if (voteInfo != null)
                    {
                        functionName = voteInfo.Title;
                    }
                }
                else if (keywordType == EKeywordType.Card)
                {
                    var cardInfo = DataProviderWx.CardDao.GetCardInfo(functionID);
                    if (cardInfo != null)
                    {
                        functionName = cardInfo.Title;
                    }
                }

                if (!string.IsNullOrEmpty(functionName))
                {
                    functionName = $@"{functionName}({EKeywordTypeUtils.GetText(keywordType)})";
                }
            }

            return functionName;
        }

        public static int GetFunctionID(KeywordInfo keywordInfo)
        {
            var functionID = 0;
            if (keywordInfo != null)
            {
                if (keywordInfo.KeywordType == EKeywordType.Album)
                {
                    functionID = DataProviderWx.AlbumDao.GetFirstIdByKeywordId(keywordInfo.PublishmentSystemId, keywordInfo.KeywordId);
                }
                else if (keywordInfo.KeywordType == EKeywordType.Appointment)
                {
                    functionID = DataProviderWx.AppointmentDao.GetFirstIdByKeywordId(keywordInfo.PublishmentSystemId, keywordInfo.KeywordId);
                }
                else if (keywordInfo.KeywordType == EKeywordType.Conference)
                {
                    functionID = DataProviderWx.ConferenceDao.GetFirstIdByKeywordId(keywordInfo.PublishmentSystemId, keywordInfo.KeywordId);
                }
                else if (keywordInfo.KeywordType == EKeywordType.Coupon)
                {
                    functionID = DataProviderWx.CouponActDao.GetFirstIdByKeywordId(keywordInfo.PublishmentSystemId, keywordInfo.KeywordId);
                }

                else if (keywordInfo.KeywordType == EKeywordType.Scratch)
                {
                    functionID = DataProviderWx.LotteryDao.GetFirstIdByKeywordId(keywordInfo.PublishmentSystemId, ELotteryType.Scratch, keywordInfo.KeywordId);
                }
                else if (keywordInfo.KeywordType == EKeywordType.BigWheel)
                {
                    functionID = DataProviderWx.LotteryDao.GetFirstIdByKeywordId(keywordInfo.PublishmentSystemId, ELotteryType.BigWheel, keywordInfo.KeywordId);
                }
                else if (keywordInfo.KeywordType == EKeywordType.GoldEgg)
                {
                    functionID = DataProviderWx.LotteryDao.GetFirstIdByKeywordId(keywordInfo.PublishmentSystemId, ELotteryType.GoldEgg, keywordInfo.KeywordId);
                }
                else if (keywordInfo.KeywordType == EKeywordType.Flap)
                {
                    functionID = DataProviderWx.LotteryDao.GetFirstIdByKeywordId(keywordInfo.PublishmentSystemId, ELotteryType.Flap, keywordInfo.KeywordId);
                }
                else if (keywordInfo.KeywordType == EKeywordType.YaoYao)
                {
                    functionID = DataProviderWx.LotteryDao.GetFirstIdByKeywordId(keywordInfo.PublishmentSystemId, ELotteryType.YaoYao, keywordInfo.KeywordId);
                }
                
                else if (keywordInfo.KeywordType == EKeywordType.Map)
                {
                    functionID = DataProviderWx.MapDao.GetFirstIdByKeywordId(keywordInfo.PublishmentSystemId, keywordInfo.KeywordId);
                }
                else if (keywordInfo.KeywordType == EKeywordType.Message)
                {
                    functionID = DataProviderWx.MessageDao.GetFirstIdByKeywordId(keywordInfo.PublishmentSystemId, keywordInfo.KeywordId);
                }
                else if (keywordInfo.KeywordType == EKeywordType.Search)
                {
                    functionID = DataProviderWx.SearchDao.GetFirstIdByKeywordId(keywordInfo.PublishmentSystemId, keywordInfo.KeywordId);
                }
                else if (keywordInfo.KeywordType == EKeywordType.Store)
                {
                    functionID = DataProviderWx.StoreDao.GetFirstIdByKeywordId(keywordInfo.PublishmentSystemId, keywordInfo.KeywordId);
                }
                else if (keywordInfo.KeywordType == EKeywordType.View360)
                {
                    functionID = DataProviderWx.View360Dao.GetFirstIdByKeywordId(keywordInfo.PublishmentSystemId, keywordInfo.KeywordId);
                }
                else if (keywordInfo.KeywordType == EKeywordType.Vote)
                {
                    functionID = DataProviderWx.VoteDao.GetFirstIdByKeywordId(keywordInfo.PublishmentSystemId, keywordInfo.KeywordId);
                }
                else if (keywordInfo.KeywordType == EKeywordType.Card)
                {
                    functionID = DataProviderWx.CardDao.GetFirstIdByKeywordId(keywordInfo.PublishmentSystemId, keywordInfo.KeywordId);
                }
            }
            return functionID;
        }

        public static string GetFunctionSqlString(int publishmentSystemID, EKeywordType keywordType)
        {
            var sqlString = string.Empty;

            if (keywordType == EKeywordType.Album)
            {
                sqlString = DataProviderWx.AlbumDao.GetSelectString(publishmentSystemID);
            }
            else if (keywordType == EKeywordType.Appointment)
            {
                sqlString = DataProviderWx.AppointmentDao.GetSelectString(publishmentSystemID);
            }
            else if (keywordType == EKeywordType.Conference)
            {
                sqlString = DataProviderWx.ConferenceDao.GetSelectString(publishmentSystemID);
            }
            else if (keywordType == EKeywordType.Coupon)
            {
                sqlString = DataProviderWx.CouponActDao.GetSelectString(publishmentSystemID);
            }

            else if (keywordType == EKeywordType.Scratch)
            {
                sqlString = DataProviderWx.LotteryDao.GetSelectString(publishmentSystemID, ELotteryType.Scratch);
            }
            else if (keywordType == EKeywordType.BigWheel)
            {
                sqlString = DataProviderWx.LotteryDao.GetSelectString(publishmentSystemID, ELotteryType.BigWheel);
            }
            else if (keywordType == EKeywordType.GoldEgg)
            {
                sqlString = DataProviderWx.LotteryDao.GetSelectString(publishmentSystemID, ELotteryType.GoldEgg);
            }
            else if (keywordType == EKeywordType.Flap)
            {
                sqlString = DataProviderWx.LotteryDao.GetSelectString(publishmentSystemID, ELotteryType.Flap);
            }
            else if (keywordType == EKeywordType.YaoYao)
            {
                sqlString = DataProviderWx.LotteryDao.GetSelectString(publishmentSystemID, ELotteryType.YaoYao);
            }
            
            else if (keywordType == EKeywordType.Map)
            {
                sqlString = DataProviderWx.MapDao.GetSelectString(publishmentSystemID);
            }
            else if (keywordType == EKeywordType.Message)
            {
                sqlString = DataProviderWx.MessageDao.GetSelectString(publishmentSystemID);
            }
            else if (keywordType == EKeywordType.Search)
            {
                sqlString = DataProviderWx.SearchDao.GetSelectString(publishmentSystemID);
            }
            else if (keywordType == EKeywordType.Store)
            {
                sqlString = DataProviderWx.StoreDao.GetSelectString(publishmentSystemID);
            }
            else if (keywordType == EKeywordType.View360)
            {
                sqlString = DataProviderWx.View360Dao.GetSelectString(publishmentSystemID);
            }
            else if (keywordType == EKeywordType.Vote)
            {
                sqlString = DataProviderWx.VoteDao.GetSelectString(publishmentSystemID);
            }
            else if (keywordType == EKeywordType.Card)
            {
                sqlString = DataProviderWx.CardDao.GetSelectString(publishmentSystemID);
            }

            return sqlString;
        }

        public static string GetFunctionUrl(PublishmentSystemInfo publishmentSystemInfo, EKeywordType keywordType, int functionID)
        {
            var functionUrl = string.Empty;

            if (functionID > 0)
            {
                if (keywordType == EKeywordType.Album)
                {
                    var albumInfo = DataProviderWx.AlbumDao.GetAlbumInfo(functionID);
                    if (albumInfo != null)
                    {
                        functionUrl = AlbumManager.GetAlbumUrl(publishmentSystemInfo, albumInfo, string.Empty);
                    }
                }
                else if (keywordType == EKeywordType.Appointment)
                {
                    functionUrl = AppointmentManager.GetIndexUrl(publishmentSystemInfo, functionID, string.Empty);
                }
                else if (keywordType == EKeywordType.Conference)
                {
                    var conferenceInfo = DataProviderWx.ConferenceDao.GetConferenceInfo(functionID);
                    if (conferenceInfo != null)
                    {
                        functionUrl = ConferenceManager.GetConferenceUrl(publishmentSystemInfo, conferenceInfo, string.Empty);
                    }
                }
                else if (keywordType == EKeywordType.Scratch || keywordType == EKeywordType.BigWheel || keywordType == EKeywordType.GoldEgg || keywordType == EKeywordType.Flap || keywordType == EKeywordType.YaoYao)
                {
                    var lotteryInfo = DataProviderWx.LotteryDao.GetLotteryInfo(functionID);
                    if (lotteryInfo != null)
                    {
                        functionUrl = LotteryManager.GetLotteryUrl(publishmentSystemInfo, lotteryInfo, string.Empty);
                    }
                }
                else if (keywordType == EKeywordType.Map)
                {
                    var mapInfo = DataProviderWx.MapDao.GetMapInfo(functionID);
                    if (mapInfo != null)
                    {
                        functionUrl = MapManager.GetMapUrl(publishmentSystemInfo, mapInfo.MapWd);
                    }
                }
                else if (keywordType == EKeywordType.Message)
                {
                    var messageInfo = DataProviderWx.MessageDao.GetMessageInfo(functionID);
                    if (messageInfo != null)
                    {
                        functionUrl = MessageManager.GetMessageUrl(publishmentSystemInfo, messageInfo, string.Empty);
                    }
                }
                else if (keywordType == EKeywordType.Search)
                {
                    var searchInfo = DataProviderWx.SearchDao.GetSearchInfo(functionID);
                    if (searchInfo != null)
                    {
                        functionUrl = SearchManager.GetSearchUrl(publishmentSystemInfo, searchInfo);
                    }
                }
                else if (keywordType == EKeywordType.Store)
                {
                    var storeInfo = DataProviderWx.StoreDao.GetStoreInfo(functionID);
                    if (storeInfo != null)
                    {
                        functionUrl = StoreManager.GetStoreUrl(publishmentSystemInfo, storeInfo, string.Empty);
                    }
                }
                else if (keywordType == EKeywordType.View360)
                {
                    var view360Info = DataProviderWx.View360Dao.GetView360Info(functionID);
                    if (view360Info != null)
                    {
                        functionUrl = View360Manager.GetView360Url(publishmentSystemInfo, view360Info, string.Empty);
                    }
                }
                else if (keywordType == EKeywordType.Vote)
                {
                    var voteInfo = DataProviderWx.VoteDao.GetVoteInfo(functionID);
                    if (voteInfo != null)
                    {
                        functionUrl = VoteManager.GetVoteUrl(publishmentSystemInfo, voteInfo, string.Empty);
                    }
                }                
                else if (keywordType == EKeywordType.Card)
                {
                    var cardInfo = DataProviderWx.CardDao.GetCardInfo(functionID);
                    if (cardInfo != null)
                    {
                        functionUrl = CardManager.GetCardUrl(publishmentSystemInfo, cardInfo, string.Empty);
                    }
                }
            }
            
            return functionUrl;
        }
	}
}
