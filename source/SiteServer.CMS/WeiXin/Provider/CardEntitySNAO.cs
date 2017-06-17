using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class CardEntitySnDao : DataProviderBase
    {
        private const string TableName = "wx_CardEntitySN";

        public int Insert(CardEntitySnInfo cardEntitySnInfo)
        {
            var cardEntitySnid = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(cardEntitySnInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        cardEntitySnid = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return cardEntitySnid;
        }

        public void Update(CardEntitySnInfo cardEntitySnInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(cardEntitySnInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);
        }

        public void UpdateStatus(bool isBinding, List<int> cardSnidList)
        {
            string sqlString =
                $"UPDATE {TableName} SET {CardEntitySnAttribute.IsBinding} = '{isBinding}' WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(cardSnidList)})  ";

            ExecuteNonQuery(sqlString);
        }
        public void Delete(int publishmentSystemId, int cardEntitySnid)
        {
            if (cardEntitySnid > 0)
            {
                string sqlString = $"DELETE FROM {TableName} WHERE ID = {cardEntitySnid}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemId, List<int> cardEntitySnidList)
        {
            if (cardEntitySnidList != null && cardEntitySnidList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(cardEntitySnidList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        public CardEntitySnInfo GetCardEntitySnInfo(int cardEntitySnid)
        {
            CardEntitySnInfo cardEntitySnInfo = null;

            string sqlWhere = $"WHERE ID = {cardEntitySnid}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    cardEntitySnInfo = new CardEntitySnInfo(rdr);
                }
                rdr.Close();
            }

            return cardEntitySnInfo;
        }

        public CardEntitySnInfo GetCardEntitySnInfo(int cardId, string cardSn, string mobile)
        {
            CardEntitySnInfo cardEntitySnInfo = null;

            string sqlWhere = $"WHERE {CardEntitySnAttribute.CardId}= {cardId}";
            if (!string.IsNullOrEmpty(cardSn))
            {
                sqlWhere += $" AND {CardEntitySnAttribute.Sn}='{PageUtils.FilterSql(cardSn)}'";
            }
            if (!string.IsNullOrEmpty(mobile))
            {
                sqlWhere += $" AND {CardEntitySnAttribute.Mobile}='{PageUtils.FilterSql(mobile)}'";
            }

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    cardEntitySnInfo = new CardEntitySnInfo(rdr);
                }
                rdr.Close();
            }

            return cardEntitySnInfo;
        }

        public bool IsExist(int publishmentSystemId, int cardId, string cardSn)
        {
            var isExists = false;

            string sqlWhere =
                $"WHERE {CardEntitySnAttribute.PublishmentSystemId}= {publishmentSystemId} AND {CardEntitySnAttribute.CardId}={cardId} AND {CardEntitySnAttribute.Sn}='{PageUtils.FilterSql(cardSn)}' ";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    isExists = true;
                }
                rdr.Close();
            }

            return isExists;
        }

        public bool IsExistMobile(int publishmentSystemId, int cardId, string mobile)
        {
            var isExists = false;

            string sqlWhere =
                $"WHERE {CardEntitySnAttribute.PublishmentSystemId}= {publishmentSystemId} AND {CardEntitySnAttribute.CardId}={cardId} AND {CardEntitySnAttribute.Mobile}='{PageUtils.FilterSql(mobile)}' ";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    isExists = true;
                }
                rdr.Close();
            }

            return isExists;
        }

        public string GetSelectString(int publishmentSystemId, int cardId, string cardSn, string userName, string mobile)
        {
            string whereString =
                $"WHERE {CardEntitySnAttribute.PublishmentSystemId} = {publishmentSystemId} AND {CardEntitySnAttribute.CardId} = {cardId}";
            if (!string.IsNullOrEmpty(cardSn))
            {
                whereString += $" AND {CardEntitySnAttribute.Sn} ='{PageUtils.FilterSql(cardSn)}')";
            }
            if (!string.IsNullOrEmpty(userName))
            {
                whereString += $" AND {CardEntitySnAttribute.UserName}='{PageUtils.FilterSql(userName)}'";
            }
            if (!string.IsNullOrEmpty(mobile))
            {
                whereString += $" AND {CardEntitySnAttribute.Mobile} ='{PageUtils.FilterSql(mobile)}')";
            }
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public List<CardEntitySnInfo> GetCardEntitySnInfoList(int publishmentSystemId, int cardId)
        {

            var cardEntitySnInfoList = new List<CardEntitySnInfo>();

            string sqlWhere = $"WHERE PublishmentSystemID={publishmentSystemId} AND CardID = {cardId}";

            string sqlOrder = $" ORDER BY {CardEntitySnAttribute.AddDate} DESC";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, sqlOrder);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var cardEntitySnInfo = new CardEntitySnInfo(rdr);
                    cardEntitySnInfoList.Add(cardEntitySnInfo);
                }
                rdr.Close();
            }

            return cardEntitySnInfoList;
        }
    }
}
