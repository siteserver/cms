using System;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class CardSignLogDao : DataProviderBase
    {
        private const string TableName = "wx_CardSignLog";

        public int Insert(CardSignLogInfo cardSignLogInfo)
        {
            var cardSignLogId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(cardSignLogInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);


            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        cardSignLogId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return cardSignLogId;
        }

        public void Update(CardSignLogInfo cardSignLogInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(cardSignLogInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);
        }

        public void Delete(int publishmentSystemId, int cardSignLogId)
        {
            if (cardSignLogId > 0)
            {
                string sqlString = $"DELETE FROM {TableName} WHERE ID = {cardSignLogId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemId, List<int> cardSignLogIdList)
        {
            if (cardSignLogIdList != null && cardSignLogIdList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(cardSignLogIdList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        public bool IsSign(int publishmentSystemId, string userName)
        {
            var isSign = false;

            string sqlWhere =
                $"WHERE PublishmentSystemID ={publishmentSystemId} AND UserName = '{PageUtils.FilterSql(userName)}' AND SignDate > '{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}'";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    isSign = true;
                }
                rdr.Close();
            }

            return isSign;
        }

        public CardSignLogInfo GetCardSignLogInfo(int cardSignLogId)
        {
            CardSignLogInfo cardSignLogInfo = null;

            string sqlWhere = $"WHERE ID = {cardSignLogId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    cardSignLogInfo = new CardSignLogInfo(rdr);
                }
                rdr.Close();
            }

            return cardSignLogInfo;
        }

        public List<CardSignLogInfo> GetCardSignLogInfoList(int publishmentSystemId, string userName)
        {
            var cardSignLogInfoList = new List<CardSignLogInfo>();

            string sqlWhere =
                $"WHERE PublishmentSystemID = {publishmentSystemId} AND UserName='{PageUtils.FilterSql(userName)}'";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var cardSignLogInfo = new CardSignLogInfo(rdr);
                    cardSignLogInfoList.Add(cardSignLogInfo);
                }
                rdr.Close();
            }

            return cardSignLogInfoList;
        }

        public List<DateTime> GetSignDateList(int publishmentSystemId, string userName)
        {
            var signDateList = new List<DateTime>();

            string sqlWhere =
                $"WHERE PublishmentSystemID = {publishmentSystemId} AND UserName='{PageUtils.FilterSql(userName)}'";
            var sqlOrder = " ORDER BY SignDate DESC ";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, CardSignLogAttribute.SignDate, sqlWhere, sqlOrder);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    signDateList.Add(ConvertHelper.GetDateTime(rdr.GetValue(0)));
                }
                rdr.Close();
            }

            return signDateList;
        }
        public string GetSelectString(int publishmentSystemId)
        {
            string whereString = $"WHERE {CardSignLogAttribute.PublishmentSystemId} = {publishmentSystemId}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public string GetSignAction()
        {
            return "签到领取积分";
        }
        public List<CardSignLogInfo> GetCardSignLogInfoList(int publishmentSystemId)
        {
            var cardSignLogInfoList = new List<CardSignLogInfo>();

            string sqlWhere = $"WHERE PublishmentSystemID = {publishmentSystemId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var cardSignLogInfo = new CardSignLogInfo(rdr);
                    cardSignLogInfoList.Add(cardSignLogInfo);
                }
                rdr.Close();
            }

            return cardSignLogInfoList;
        }

    }
}
