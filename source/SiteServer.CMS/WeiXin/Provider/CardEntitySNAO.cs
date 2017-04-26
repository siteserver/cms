using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class CardEntitySNDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_CardEntitySN";

        public int Insert(CardEntitySNInfo cardEntitySNInfo)
        {
            var cardEntitySNID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(cardEntitySNInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        cardEntitySNID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return cardEntitySNID;
        }

        public void Update(CardEntitySNInfo cardEntitySNInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(cardEntitySNInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        public void UpdateStatus(bool isBinding, List<int> cardSNIDList)
        {
            string sqlString =
                $"UPDATE {TABLE_NAME} SET {CardEntitySNAttribute.IsBinding} = '{isBinding}' WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(cardSNIDList)})  ";

            ExecuteNonQuery(sqlString);
        }
        public void Delete(int publishmentSystemID, int cardEntitySNID)
        {
            if (cardEntitySNID > 0)
            {
                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE ID = {cardEntitySNID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemID, List<int> cardEntitySNIDList)
        {
            if (cardEntitySNIDList != null && cardEntitySNIDList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(cardEntitySNIDList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        public CardEntitySNInfo GetCardEntitySNInfo(int cardEntitySNID)
        {
            CardEntitySNInfo cardEntitySNInfo = null;

            string SQL_WHERE = $"WHERE ID = {cardEntitySNID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    cardEntitySNInfo = new CardEntitySNInfo(rdr);
                }
                rdr.Close();
            }

            return cardEntitySNInfo;
        }

        public CardEntitySNInfo GetCardEntitySNInfo(int cardID, string cardSN, string mobile)
        {
            CardEntitySNInfo cardEntitySNInfo = null;

            string SQL_WHERE = $"WHERE {CardEntitySNAttribute.CardID}= {cardID}";
            if (!string.IsNullOrEmpty(cardSN))
            {
                SQL_WHERE += $" AND {CardEntitySNAttribute.SN}='{PageUtils.FilterSql(cardSN)}'";
            }
            if (!string.IsNullOrEmpty(mobile))
            {
                SQL_WHERE += $" AND {CardEntitySNAttribute.Mobile}='{PageUtils.FilterSql(mobile)}'";
            }

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    cardEntitySNInfo = new CardEntitySNInfo(rdr);
                }
                rdr.Close();
            }

            return cardEntitySNInfo;
        }

        public bool IsExist(int publishmentSystemID, int cardID, string cardSN)
        {
            var isExists = false;

            string SQL_WHERE =
                $"WHERE {CardEntitySNAttribute.PublishmentSystemID}= {publishmentSystemID} AND {CardEntitySNAttribute.CardID}={cardID} AND {CardEntitySNAttribute.SN}='{PageUtils.FilterSql(cardSN)}' ";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    isExists = true;
                }
                rdr.Close();
            }

            return isExists;
        }

        public bool IsExistMobile(int publishmentSystemID, int cardID, string mobile)
        {
            var isExists = false;

            string SQL_WHERE =
                $"WHERE {CardEntitySNAttribute.PublishmentSystemID}= {publishmentSystemID} AND {CardEntitySNAttribute.CardID}={cardID} AND {CardEntitySNAttribute.Mobile}='{PageUtils.FilterSql(mobile)}' ";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    isExists = true;
                }
                rdr.Close();
            }

            return isExists;
        }

        public string GetSelectString(int publishmentSystemID, int cardID, string cardSN, string userName, string mobile)
        {
            string whereString =
                $"WHERE {CardEntitySNAttribute.PublishmentSystemID} = {publishmentSystemID} AND {CardEntitySNAttribute.CardID} = {cardID}";
            if (!string.IsNullOrEmpty(cardSN))
            {
                whereString += $" AND {CardEntitySNAttribute.SN} ='{PageUtils.FilterSql(cardSN)}')";
            }
            if (!string.IsNullOrEmpty(userName))
            {
                whereString += $" AND {CardEntitySNAttribute.UserName}='{PageUtils.FilterSql(userName)}'";
            }
            if (!string.IsNullOrEmpty(mobile))
            {
                whereString += $" AND {CardEntitySNAttribute.Mobile} ='{PageUtils.FilterSql(mobile)}')";
            }
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public List<CardEntitySNInfo> GetCardEntitySNInfoList(int publishmentSystemID, int cardID)
        {

            var cardEntitySNInfoList = new List<CardEntitySNInfo>();

            string SQL_WHERE = $"WHERE PublishmentSystemID={publishmentSystemID} AND CardID = {cardID}";

            string SQL_ORDER = $" ORDER BY {CardEntitySNAttribute.AddDate} DESC";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, SQL_ORDER);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var cardEntitySNInfo = new CardEntitySNInfo(rdr);
                    cardEntitySNInfoList.Add(cardEntitySNInfo);
                }
                rdr.Close();
            }

            return cardEntitySNInfoList;
        }
    }
}
