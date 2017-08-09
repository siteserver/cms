using System;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class CardSnDao : DataProviderBase
    {
        private const string TableName = "wx_CardSN";

        public int Insert(CardSnInfo cardSnInfo)
        {
            var cardSnid = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(cardSnInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);


            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        cardSnid = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return cardSnid;
        }

        public void Update(CardSnInfo cardSnInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(cardSnInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);
        }

        public void UpdateStatus(int cardId, bool isDisabled, List<int> cardSnidList)
        {
            string sqlString =
                $"UPDATE {TableName} SET {CardSnAttribute.IsDisabled} = '{isDisabled}' WHERE CardID={cardId} AND ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(cardSnidList)})  ";

            ExecuteNonQuery(sqlString);
        }

        public void Delete(int publishmentSystemId, int cardSnid)
        {
            if (cardSnid > 0)
            {
                string sqlString = $"DELETE FROM {TableName} WHERE ID = {cardSnid}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemId, List<int> cardSnidList)
        {
            if (cardSnidList != null && cardSnidList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(cardSnidList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Recharge(int cardSnid, string userName, decimal amount)
        {
            string sqlString =
                $"UPDATE {TableName} SET {CardSnAttribute.Amount} = {CardSnAttribute.Amount}+{amount} WHERE ID = {cardSnid} AND UserName='{PageUtils.FilterSql(userName)}' ";

            ExecuteNonQuery(sqlString);
        }

        public void Recharge(int cardSnid, string userName, decimal amount, CardCashLogInfo cardCashInfo, IDbTransaction trans)
        {
            DataProviderWx.CardCashLogDao.Insert(cardCashInfo);

            string sqlString =
                $"UPDATE {TableName} SET {CardSnAttribute.Amount} = {CardSnAttribute.Amount}+{amount} WHERE ID = {cardSnid} AND UserName='{PageUtils.FilterSql(userName)}' ";

            ExecuteNonQuery(trans, sqlString);

        }

        public void Consume(int cardSnid, string userName, decimal amount)
        {
            string sqlString =
                $"UPDATE {TableName} SET {CardSnAttribute.Amount} = {CardSnAttribute.Amount}-{amount} WHERE ID = {cardSnid} AND UserName='{PageUtils.FilterSql(userName)}' ";

            ExecuteNonQuery(sqlString);
        }

        public void Exchange(int cardSnid, string userName, decimal amount)
        {
            string sqlString =
                $"UPDATE {TableName} SET {CardSnAttribute.Amount} = {CardSnAttribute.Amount}+{amount} WHERE ID = {cardSnid} AND UserName='{PageUtils.FilterSql(userName)}' ";

            ExecuteNonQuery(sqlString);
        }
        public CardSnInfo GetCardSnInfo(int cardSnid)
        {
            CardSnInfo cardSnInfo = null;

            string sqlWhere = $"WHERE ID = {cardSnid}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    cardSnInfo = new CardSnInfo(rdr);
                }
                rdr.Close();
            }

            return cardSnInfo;
        }

        public CardSnInfo GetCardSnInfo(int publishmentSystemId, int cardId, string cardSn, string userName)
        {
            CardSnInfo cardSnInfo = null;

            string sqlWhere = $"WHERE {CardSnAttribute.PublishmentSystemId} = {publishmentSystemId} ";
            if (cardId > 0)
            {
                sqlWhere += $" AND {CardSnAttribute.CardId}='{cardId}'";
            }
            if (!string.IsNullOrEmpty(cardSn))
            {
                sqlWhere += $" AND {CardSnAttribute.Sn}='{PageUtils.FilterSql(cardSn)}'";
            }
            if (!string.IsNullOrEmpty(userName))
            {
                sqlWhere += $" AND {CardSnAttribute.UserName}='{PageUtils.FilterSql(userName)}'";
            }
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    cardSnInfo = new CardSnInfo(rdr);
                }
                rdr.Close();
            }
            return cardSnInfo;
        }

        public List<CardSnInfo> GetCardSnInfoList(int publishmentSystemId, int cardId)
        {
            var cardSnInfoList = new List<CardSnInfo>();

            string sqlWhere =
                $"WHERE {CardSnAttribute.PublishmentSystemId} = {publishmentSystemId} AND {CardSnAttribute.CardId} = {cardId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var cardSnInfo = new CardSnInfo(rdr);
                    cardSnInfoList.Add(cardSnInfo);
                }
                rdr.Close();
            }
            return cardSnInfoList;
        }

        public List<string> GetUserNameList(int publishmentSystemId, int cardId, string cardSn, string userName)
        {
            var userNameList = new List<string>();

            string sqlWhere = $"WHERE {CardSnAttribute.PublishmentSystemId} = {publishmentSystemId} ";

            if (cardId > 0)
            {
                sqlWhere += $" AND CardID = {cardId}";
            }
            if (!string.IsNullOrEmpty(cardSn))
            {
                sqlWhere += $" AND {CardSnAttribute.Sn}='{PageUtils.FilterSql(cardSn)}'";
            }
            if (!string.IsNullOrEmpty(userName))
            {
                sqlWhere += $" AND {CardSnAttribute.UserName} ='{PageUtils.FilterSql(userName)}'";
            }
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var cardSnInfo = new CardSnInfo(rdr);
                    if (!userNameList.Contains(cardSnInfo.UserName))
                    {
                        userNameList.Add(cardSnInfo.UserName);
                    }
                }
                rdr.Close();
            }
            return userNameList;
        }

        public bool IsExists(int publishmentSystemId, int cardId, string userName)
        {
            var isExist = false;

            string sqlWhere = $"WHERE PublishmentSystemID = {publishmentSystemId}";
            if (cardId > 0)
            {
                sqlWhere += $" AND CardID={cardId}";
            }
            if (!string.IsNullOrEmpty(userName))
            {
                sqlWhere += $" AND UserName='{PageUtils.FilterSql(userName)}'";
            }
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    isExist = true;
                }
                rdr.Close();
            }
            return isExist;
        }

        public decimal GetAmount(int cardSnid, string userName)
        {
            decimal amount = 0;

            string sqlWhere = $"WHERE ID = {cardSnid} AND userName='{PageUtils.FilterSql(userName)}'";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    var cardSnInfo = new CardSnInfo(rdr);
                    amount = cardSnInfo.Amount;
                }
                rdr.Close();
            }
            return amount;
        }

        public string GetNextCardSn(int publishmentSystemId, int cardId)
        {
            var cardSn = string.Empty;
            string sqlWhere =
                $"WHERE {CardSnAttribute.PublishmentSystemId} = {publishmentSystemId} AND {CardSnAttribute.CardId}={cardId}";
            string sqlOrder = $" ORDER BY AddDate {"DESC"}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, "SN", sqlWhere, sqlOrder);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    cardSn = ConvertHelper.GetString(rdr.GetValue(0));
                }
                rdr.Close();
            }
            if (string.IsNullOrEmpty(cardSn))
            {
                cardSn = "100001";
            }
            else
            {
                var curCardSn = Convert.ToInt32(cardSn);
                var nextCardSn = (curCardSn + 1).ToString();

                var len = cardSn.Length;
                var i = nextCardSn.Length;
                while (i < len)
                {
                    nextCardSn = "0" + nextCardSn;
                    i++;
                }
                return nextCardSn;
            }
            return cardSn;
        }


        public string GetSelectString(int publishmentSystemId, int cardId, string cardSn, string userName, string mobile)
        {
            string whereString =
                $"WHERE {CardSnAttribute.PublishmentSystemId} = {publishmentSystemId} AND {CardSnAttribute.CardId}={cardId}";
            if (!string.IsNullOrEmpty(cardSn))
            {
                whereString += $" AND {CardSnAttribute.Sn}='{PageUtils.FilterSql(cardSn)}'";
            }
            if (!string.IsNullOrEmpty(userName))
            {
                whereString += $" AND {CardSnAttribute.UserName}='{PageUtils.FilterSql(userName)}'";
            }
            if (!string.IsNullOrEmpty(mobile))
            {
                whereString +=
                    $" AND {CardSnAttribute.UserName} IN (SELECT UserName FROM bairong_Users WHERE Mobile='{PageUtils.FilterSql(mobile)}')";
            }

            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

    }
}
