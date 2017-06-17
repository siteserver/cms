using System;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class LotteryLogDao : DataProviderBase
    {
        private const string TableName = "wx_LotteryLog";
        public int Insert(LotteryLogInfo lotteryLogInfo)
        {
            var lotteryLogId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(lotteryLogInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        lotteryLogId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return lotteryLogId;
        }

        public void AddCount(int publishmentSystemId, int lotteryId, string cookieSn, string wxOpenId, string userName, int maxCount, int maxDailyCount, out bool isMaxCount, out bool isMaxDailyCount)
        {
            isMaxCount = false;
            isMaxDailyCount = false;

            var logInfo = GetLogInfo(lotteryId, cookieSn, wxOpenId, userName);
            if (logInfo == null)
            {
                logInfo = new LotteryLogInfo { PublishmentSystemId = publishmentSystemId, LotteryId = lotteryId, CookieSn = cookieSn, WxOpenId = wxOpenId, UserName = userName, LotteryCount = 1, LotteryDailyCount = 1, LastLotteryDate = DateTime.Now };

                IDataParameter[] parms = null;

                var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(logInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

                ExecuteNonQuery(sqlInsert, parms);
            }
            else
            {
                if (maxCount > 0 && logInfo.LotteryCount >= maxCount)
                {
                    isMaxCount = true;
                }
                else
                {
                    var theSameDay = DateUtils.IsTheSameDay(DateTime.Now, logInfo.LastLotteryDate);
                    if (theSameDay)
                    {
                        if (maxDailyCount > 0 && logInfo.LotteryDailyCount >= maxDailyCount)
                        {
                            isMaxDailyCount = true;
                        }
                    }

                    if (!isMaxDailyCount)
                    {
                        string sqlString =
                            $"UPDATE {TableName} SET {LotteryLogAttribute.LotteryCount} = {LotteryLogAttribute.LotteryCount} + 1, {LotteryLogAttribute.LotteryDailyCount} = 1, {LotteryLogAttribute.LastLotteryDate} = getdate() WHERE ID = {logInfo.Id}";
                        if (theSameDay)
                        {
                            sqlString =
                                $"UPDATE {TableName} SET {LotteryLogAttribute.LotteryCount} = {LotteryLogAttribute.LotteryCount} + 1, {LotteryLogAttribute.LotteryDailyCount} = {LotteryLogAttribute.LotteryDailyCount} + 1, {LotteryLogAttribute.LastLotteryDate} = getdate() WHERE ID = {logInfo.Id}";
                        }
                        ExecuteNonQuery(sqlString);
                    }
                }
            }
        }

        public void DeleteAll(int lotteryId)
        {
            if (lotteryId > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE {LotteryLogAttribute.LotteryId} = {lotteryId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(List<int> logIdList)
        {
            if (logIdList != null && logIdList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(logIdList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        public int GetCount(int lotteryId)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TableName} WHERE {LotteryLogAttribute.LotteryId} = {lotteryId}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        private LotteryLogInfo GetLogInfo(int lotteryId, string cookieSn, string wxOpenId, string userName)
        {
            LotteryLogInfo logInfo = null;

            string sqlWhere =
                $"WHERE {LotteryLogAttribute.LotteryId} = {lotteryId} AND {LotteryLogAttribute.CookieSn} = '{cookieSn}'";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    logInfo = new LotteryLogInfo(rdr);
                }
                rdr.Close();
            }

            return logInfo;
        }

        public string GetSelectString(int lotteryId)
        {
            string whereString = $"WHERE {LotteryLogAttribute.LotteryId} = {lotteryId}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public List<LotteryLogInfo> GetLotteryLogInfoList(int publishmentSystemId, int lotteryId)
        {
            var lotteryLogInfoList = new List<LotteryLogInfo>();

            string sqlWhere =
                $"WHERE {LotteryLogAttribute.PublishmentSystemId} = {publishmentSystemId} AND {LotteryLogAttribute.LotteryId} = {lotteryId}";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var lotteryLogInfo = new LotteryLogInfo(rdr);
                    lotteryLogInfoList.Add(lotteryLogInfo);
                }
                rdr.Close();
            }

            return lotteryLogInfoList;
        }

    }
}
