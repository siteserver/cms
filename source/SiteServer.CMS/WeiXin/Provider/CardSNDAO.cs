using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class CardSNDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_CardSN";

        public int Insert(CardSNInfo cardSNInfo)
        {
            var cardSNID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(cardSNInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);


            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        cardSNID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return cardSNID;
        }

        public void Update(CardSNInfo cardSNInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(cardSNInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        public void UpdateStatus(int cardID, bool isDisabled, List<int> cardSNIDList)
        {
            string sqlString =
                $"UPDATE {TABLE_NAME} SET {CardSNAttribute.IsDisabled} = '{isDisabled}' WHERE CardID={cardID} AND ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(cardSNIDList)})  ";

            ExecuteNonQuery(sqlString);
        }

        public void Delete(int publishmentSystemID, int cardSNID)
        {
            if (cardSNID > 0)
            {
                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE ID = {cardSNID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemID, List<int> cardSNIDList)
        {
            if (cardSNIDList != null && cardSNIDList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(cardSNIDList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Recharge(int cardSNID, string userName, decimal amount)
        {
            string sqlString =
                $"UPDATE {TABLE_NAME} SET {CardSNAttribute.Amount} = {CardSNAttribute.Amount}+{amount} WHERE ID = {cardSNID} AND UserName='{PageUtils.FilterSql(userName)}' ";

            ExecuteNonQuery(sqlString);
        }

        public void Recharge(int cardSNID, string userName, decimal amount, CardCashLogInfo cardCashInfo, IDbTransaction trans)
        {
            DataProviderWX.CardCashLogDAO.Insert(cardCashInfo);

            string sqlString =
                $"UPDATE {TABLE_NAME} SET {CardSNAttribute.Amount} = {CardSNAttribute.Amount}+{amount} WHERE ID = {cardSNID} AND UserName='{PageUtils.FilterSql(userName)}' ";

            ExecuteNonQuery(trans, sqlString);

        }

        public void Consume(int cardSNID, string userName, decimal amount)
        {
            string sqlString =
                $"UPDATE {TABLE_NAME} SET {CardSNAttribute.Amount} = {CardSNAttribute.Amount}-{amount} WHERE ID = {cardSNID} AND UserName='{PageUtils.FilterSql(userName)}' ";

            ExecuteNonQuery(sqlString);
        }

        public void Exchange(int cardSNID, string userName, decimal amount)
        {
            string sqlString =
                $"UPDATE {TABLE_NAME} SET {CardSNAttribute.Amount} = {CardSNAttribute.Amount}+{amount} WHERE ID = {cardSNID} AND UserName='{PageUtils.FilterSql(userName)}' ";

            ExecuteNonQuery(sqlString);
        }
        public CardSNInfo GetCardSNInfo(int cardSNID)
        {
            CardSNInfo cardSNInfo = null;

            string SQL_WHERE = $"WHERE ID = {cardSNID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    cardSNInfo = new CardSNInfo(rdr);
                }
                rdr.Close();
            }

            return cardSNInfo;
        }

        public CardSNInfo GetCardSNInfo(int publishmentSystemID, int cardID, string cardSN, string userName)
        {
            CardSNInfo cardSNInfo = null;

            string SQL_WHERE = $"WHERE {CardSNAttribute.PublishmentSystemID} = {publishmentSystemID} ";
            if (cardID > 0)
            {
                SQL_WHERE += $" AND {CardSNAttribute.CardID}='{cardID}'";
            }
            if (!string.IsNullOrEmpty(cardSN))
            {
                SQL_WHERE += $" AND {CardSNAttribute.SN}='{PageUtils.FilterSql(cardSN)}'";
            }
            if (!string.IsNullOrEmpty(userName))
            {
                SQL_WHERE += $" AND {CardSNAttribute.UserName}='{PageUtils.FilterSql(userName)}'";
            }
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    cardSNInfo = new CardSNInfo(rdr);
                }
                rdr.Close();
            }
            return cardSNInfo;
        }

        public List<CardSNInfo> GetCardSNInfoList(int publishmentSystemID, int cardID)
        {
            var cardSNInfoList = new List<CardSNInfo>();

            string SQL_WHERE =
                $"WHERE {CardSNAttribute.PublishmentSystemID} = {publishmentSystemID} AND {CardSNAttribute.CardID} = {cardID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var cardSNInfo = new CardSNInfo(rdr);
                    cardSNInfoList.Add(cardSNInfo);
                }
                rdr.Close();
            }
            return cardSNInfoList;
        }

        public ArrayList GetUserNameArrayList(int publishmentSystemID, int cardID, string cardSN, string userName)
        {
            var userNameArrayList = new ArrayList();

            string SQL_WHERE = $"WHERE {CardSNAttribute.PublishmentSystemID} = {publishmentSystemID} ";

            if (cardID > 0)
            {
                SQL_WHERE += $" AND CardID = {cardID}";
            }
            if (!string.IsNullOrEmpty(cardSN))
            {
                SQL_WHERE += $" AND {CardSNAttribute.SN}='{PageUtils.FilterSql(cardSN)}'";
            }
            if (!string.IsNullOrEmpty(userName))
            {
                SQL_WHERE += $" AND {CardSNAttribute.UserName} ='{PageUtils.FilterSql(userName)}'";
            }
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var cardSNInfo = new CardSNInfo(rdr);
                    if (!userNameArrayList.Contains(cardSNInfo.UserName))
                    {
                        userNameArrayList.Add(cardSNInfo.UserName);
                    }
                }
                rdr.Close();
            }
            return userNameArrayList;
        }

        public bool isExists(int publishmentSystemID, int cardID, string userName)
        {
            var isExist = false;

            string SQL_WHERE = $"WHERE PublishmentSystemID = {publishmentSystemID}";
            if (cardID > 0)
            {
                SQL_WHERE += $" AND CardID={cardID}";
            }
            if (!string.IsNullOrEmpty(userName))
            {
                SQL_WHERE += $" AND UserName='{PageUtils.FilterSql(userName)}'";
            }
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    isExist = true;
                }
                rdr.Close();
            }
            return isExist;
        }

        public decimal GetAmount(int cardSNID, string userName)
        {
            decimal amount = 0;

            string SQL_WHERE = $"WHERE ID = {cardSNID} AND userName='{PageUtils.FilterSql(userName)}'";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    var cardSNInfo = new CardSNInfo(rdr);
                    amount = cardSNInfo.Amount;
                }
                rdr.Close();
            }
            return amount;
        }

        public string GetNextCardSN(int publishmentSystemID, int cardID)
        {
            var cardSN = string.Empty;
            string SQL_WHERE =
                $"WHERE {CardSNAttribute.PublishmentSystemID} = {publishmentSystemID} AND {CardSNAttribute.CardID}={cardID}";
            string SQL_ORDER = $" ORDER BY AddDate {"DESC"}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, "SN", SQL_WHERE, SQL_ORDER);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    cardSN = ConvertHelper.GetString(rdr.GetValue(0));
                }
                rdr.Close();
            }
            if (string.IsNullOrEmpty(cardSN))
            {
                cardSN = "100001";
            }
            else
            {
                var curCardSN = Convert.ToInt32(cardSN);
                var nextCardSN = (curCardSN + 1).ToString();

                var len = cardSN.Length;
                var i = nextCardSN.Length;
                while (i < len)
                {
                    nextCardSN = "0" + nextCardSN;
                    i++;
                }
                return nextCardSN;
            }
            return cardSN;
        }


        public string GetSelectString(int publishmentSystemID, int cardID, string cardSN, string userName, string mobile)
        {
            string whereString =
                $"WHERE {CardSNAttribute.PublishmentSystemID} = {publishmentSystemID} AND {CardSNAttribute.CardID}={cardID}";
            if (!string.IsNullOrEmpty(cardSN))
            {
                whereString += $" AND {CardSNAttribute.SN}='{PageUtils.FilterSql(cardSN)}'";
            }
            if (!string.IsNullOrEmpty(userName))
            {
                whereString += $" AND {CardSNAttribute.UserName}='{PageUtils.FilterSql(userName)}'";
            }
            if (!string.IsNullOrEmpty(mobile))
            {
                whereString +=
                    $" AND {CardSNAttribute.UserName} IN (SELECT UserName FROM bairong_Users WHERE Mobile='{PageUtils.FilterSql(mobile)}')";
            }

            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

    }
}
