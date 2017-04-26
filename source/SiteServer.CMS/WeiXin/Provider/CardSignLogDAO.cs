using System;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class CardSignLogDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_CardSignLog";

        public int Insert(CardSignLogInfo cardSignLogInfo)
        {
            var cardSignLogID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(cardSignLogInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);


            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        cardSignLogID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return cardSignLogID;
        }

        public void Update(CardSignLogInfo cardSignLogInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(cardSignLogInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        public void Delete(int publishmentSystemID, int cardSignLogID)
        {
            if (cardSignLogID > 0)
            {
                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE ID = {cardSignLogID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemID, List<int> cardSignLogIDList)
        {
            if (cardSignLogIDList != null && cardSignLogIDList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(cardSignLogIDList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        public bool IsSign(int publishmentSystemID, string userName)
        {
            var isSign = false;

            string SQL_WHERE =
                $"WHERE PublishmentSystemID ={publishmentSystemID} AND UserName = '{PageUtils.FilterSql(userName)}' AND SignDate > '{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}'";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    isSign = true;
                }
                rdr.Close();
            }

            return isSign;
        }

        public CardSignLogInfo GetCardSignLogInfo(int cardSignLogID)
        {
            CardSignLogInfo cardSignLogInfo = null;

            string SQL_WHERE = $"WHERE ID = {cardSignLogID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    cardSignLogInfo = new CardSignLogInfo(rdr);
                }
                rdr.Close();
            }

            return cardSignLogInfo;
        }

        public List<CardSignLogInfo> GetCardSignLogInfoList(int publishmentSystemID, string userName)
        {
            var cardSignLogInfoList = new List<CardSignLogInfo>();

            string SQL_WHERE =
                $"WHERE PublishmentSystemID = {publishmentSystemID} AND UserName='{PageUtils.FilterSql(userName)}'";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
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

        public List<DateTime> GetSignDateList(int publishmentSystemID, string userName)
        {
            var signDateList = new List<DateTime>();

            string SQL_WHERE =
                $"WHERE PublishmentSystemID = {publishmentSystemID} AND UserName='{PageUtils.FilterSql(userName)}'";
            var SQL_ORDER = " ORDER BY SignDate DESC ";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, CardSignLogAttribute.SignDate, SQL_WHERE, SQL_ORDER);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    signDateList.Add(ConvertHelper.GetDateTime(rdr.GetValue(0)));
                }
                rdr.Close();
            }

            return signDateList;
        }
        public string GetSelectString(int publishmentSystemID)
        {
            string whereString = $"WHERE {CardSignLogAttribute.PublishmentSystemID} = {publishmentSystemID}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public string GetSignAction()
        {
            return "签到领取积分";
        }
        public List<CardSignLogInfo> GetCardSignLogInfoList(int publishmentSystemID)
        {
            var cardSignLogInfoList = new List<CardSignLogInfo>();

            string SQL_WHERE = $"WHERE PublishmentSystemID = {publishmentSystemID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
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
