using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class CardDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_Card";
         
        public int Insert(CardInfo cardInfo)
        {
            var cardID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(cardInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);
              
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        cardID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return cardID;
        }

        public void Update(CardInfo cardInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(cardInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            ExecuteNonQuery(SQL_UPDATE, parms);
        }
  
        public void AddPVCount(int cardID)
        {
            if (cardID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {CardAttribute.PVCount} = {CardAttribute.PVCount} + 1 WHERE ID = {cardID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemID, int cardID)
        {
            if (cardID > 0)
            {
                var CardIDList = new List<int>();
                CardIDList.Add(cardID);
                DataProviderWX.KeywordDAO.Delete(GetKeywordIDList(CardIDList));

                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE ID = {cardID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemID, List<int> cardIDList)
        {
            if (cardIDList != null && cardIDList.Count > 0)
            {
                DataProviderWX.KeywordDAO.Delete(GetKeywordIDList(cardIDList));

                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(cardIDList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetKeywordIDList(List<int> cardIDList)
        {
            var keywordIDList = new List<int>();

            string sqlString =
                $"SELECT {CardAttribute.KeywordID} FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(cardIDList)})";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    keywordIDList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return keywordIDList;
        }

        public CardInfo GetCardInfo(int cardID)
        {
            CardInfo CardInfo = null;

            string SQL_WHERE = $"WHERE ID = {cardID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    CardInfo = new CardInfo(rdr);
                }
                rdr.Close();
            }

            return CardInfo;
        }

        public List<CardInfo> GetCardInfoList(int publishmentSystemID)
        {
            var cardInfoList = new List<CardInfo>();

            string SQL_WHERE = $"WHERE {CardAttribute.PublishmentSystemID} = {publishmentSystemID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
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

        public List<CardInfo> GetCardInfoListByKeywordID(int publishmentSystemID, int keywordID)
        {
            var CardInfoList = new List<CardInfo>();

            string SQL_WHERE =
                $"WHERE {CardAttribute.PublishmentSystemID} = {publishmentSystemID} AND {CardAttribute.IsDisabled} <> '{true}'";
            if (keywordID > 0)
            {
                SQL_WHERE += $" AND {CardAttribute.KeywordID} = {keywordID}";
            }

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var CardInfo = new CardInfo(rdr);
                    CardInfoList.Add(CardInfo);
                }
                rdr.Close();
            }

            return CardInfoList;
        }

        public int GetFirstIDByKeywordID(int publishmentSystemID, int keywordID)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TABLE_NAME} WHERE {CardAttribute.PublishmentSystemID} = {publishmentSystemID} AND {CardAttribute.IsDisabled} <> '{true}' AND {CardAttribute.KeywordID} = {keywordID}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetTitle(int cardID)
        {
            var title = string.Empty;

            string SQL_WHERE = $"WHERE ID = {cardID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, CardAttribute.Title, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    title = rdr.GetValue(0).ToString();
                }
                rdr.Close();
            }

            return title;
        }

        public string GetSelectString(int publishmentSystemID)
        {
            string whereString = $"WHERE {CardAttribute.PublishmentSystemID} = {publishmentSystemID}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }
    }
}
