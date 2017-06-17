using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class CardDao : DataProviderBase
    {
        private const string TableName = "wx_Card";
         
        public int Insert(CardInfo cardInfo)
        {
            var cardId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(cardInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);
              
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        cardId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return cardId;
        }

        public void Update(CardInfo cardInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(cardInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);
        }
  
        public void AddPvCount(int cardId)
        {
            if (cardId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {CardAttribute.PvCount} = {CardAttribute.PvCount} + 1 WHERE ID = {cardId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemId, int cardId)
        {
            if (cardId > 0)
            {
                var cardIdList = new List<int>();
                cardIdList.Add(cardId);
                DataProviderWx.KeywordDao.Delete(GetKeywordIdList(cardIdList));

                string sqlString = $"DELETE FROM {TableName} WHERE ID = {cardId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemId, List<int> cardIdList)
        {
            if (cardIdList != null && cardIdList.Count > 0)
            {
                DataProviderWx.KeywordDao.Delete(GetKeywordIdList(cardIdList));

                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(cardIdList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetKeywordIdList(List<int> cardIdList)
        {
            var keywordIdList = new List<int>();

            string sqlString =
                $"SELECT {CardAttribute.KeywordId} FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(cardIdList)})";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    keywordIdList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return keywordIdList;
        }

        public CardInfo GetCardInfo(int cardId)
        {
            CardInfo cardInfo = null;

            string sqlWhere = $"WHERE ID = {cardId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    cardInfo = new CardInfo(rdr);
                }
                rdr.Close();
            }

            return cardInfo;
        }

        public List<CardInfo> GetCardInfoList(int publishmentSystemId)
        {
            var cardInfoList = new List<CardInfo>();

            string sqlWhere = $"WHERE {CardAttribute.PublishmentSystemId} = {publishmentSystemId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var cardInfo = new CardInfo(rdr);
                    cardInfoList.Add(cardInfo);
                }
                rdr.Close();
            }

            return cardInfoList;
        }

        public List<CardInfo> GetCardInfoListByKeywordId(int publishmentSystemId, int keywordId)
        {
            var cardInfoList = new List<CardInfo>();

            string sqlWhere =
                $"WHERE {CardAttribute.PublishmentSystemId} = {publishmentSystemId} AND {CardAttribute.IsDisabled} <> '{true}'";
            if (keywordId > 0)
            {
                sqlWhere += $" AND {CardAttribute.KeywordId} = {keywordId}";
            }

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var cardInfo = new CardInfo(rdr);
                    cardInfoList.Add(cardInfo);
                }
                rdr.Close();
            }

            return cardInfoList;
        }

        public int GetFirstIdByKeywordId(int publishmentSystemId, int keywordId)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TableName} WHERE {CardAttribute.PublishmentSystemId} = {publishmentSystemId} AND {CardAttribute.IsDisabled} <> '{true}' AND {CardAttribute.KeywordId} = {keywordId}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetTitle(int cardId)
        {
            var title = string.Empty;

            string sqlWhere = $"WHERE ID = {cardId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, CardAttribute.Title, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    title = rdr.GetValue(0).ToString();
                }
                rdr.Close();
            }

            return title;
        }

        public string GetSelectString(int publishmentSystemId)
        {
            string whereString = $"WHERE {CardAttribute.PublishmentSystemId} = {publishmentSystemId}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }
    }
}
