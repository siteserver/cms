using System;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class LotteryLogDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_LotteryLog";
        public int Insert(LotteryLogInfo lotteryLogInfo)
        {
            var lotteryLogID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(lotteryLogInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        lotteryLogID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return lotteryLogID;
        }

        public void AddCount(int publishmentSystemID, int lotteryID, string cookieSN, string wxOpenID, string userName, int maxCount, int maxDailyCount, out bool isMaxCount, out bool isMaxDailyCount)
        {
            isMaxCount = false;
            isMaxDailyCount = false;

            var logInfo = GetLogInfo(lotteryID, cookieSN, wxOpenID, userName);
            if (logInfo == null)
            {
                logInfo = new LotteryLogInfo { PublishmentSystemID = publishmentSystemID, LotteryID = lotteryID, CookieSN = cookieSN, WXOpenID = wxOpenID, UserName = userName, LotteryCount = 1, LotteryDailyCount = 1, LastLotteryDate = DateTime.Now };

                IDataParameter[] parms = null;

                var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(logInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

                ExecuteNonQuery(SQL_INSERT, parms);
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
                            $"UPDATE {TABLE_NAME} SET {LotteryLogAttribute.LotteryCount} = {LotteryLogAttribute.LotteryCount} + 1, {LotteryLogAttribute.LotteryDailyCount} = 1, {LotteryLogAttribute.LastLotteryDate} = getdate() WHERE ID = {logInfo.ID}";
                        if (theSameDay)
                        {
                            sqlString =
                                $"UPDATE {TABLE_NAME} SET {LotteryLogAttribute.LotteryCount} = {LotteryLogAttribute.LotteryCount} + 1, {LotteryLogAttribute.LotteryDailyCount} = {LotteryLogAttribute.LotteryDailyCount} + 1, {LotteryLogAttribute.LastLotteryDate} = getdate() WHERE ID = {logInfo.ID}";
                        }
                        ExecuteNonQuery(sqlString);
                    }
                }
            }
        }

        public void DeleteAll(int lotteryID)
        {
            if (lotteryID > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE {LotteryLogAttribute.LotteryID} = {lotteryID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(List<int> logIDList)
        {
            if (logIDList != null && logIDList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(logIDList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        public int GetCount(int lotteryID)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TABLE_NAME} WHERE {LotteryLogAttribute.LotteryID} = {lotteryID}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        private LotteryLogInfo GetLogInfo(int lotteryID, string cookieSN, string wxOpenID, string userName)
        {
            LotteryLogInfo logInfo = null;

            string SQL_WHERE =
                $"WHERE {LotteryLogAttribute.LotteryID} = {lotteryID} AND {LotteryLogAttribute.CookieSN} = '{cookieSN}'";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    logInfo = new LotteryLogInfo(rdr);
                }
                rdr.Close();
            }

            return logInfo;
        }

        public string GetSelectString(int lotteryID)
        {
            string whereString = $"WHERE {LotteryLogAttribute.LotteryID} = {lotteryID}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public List<LotteryLogInfo> GetLotteryLogInfoList(int publishmentSystemID, int lotteryID)
        {
            var lotteryLogInfoList = new List<LotteryLogInfo>();

            string SQL_WHERE =
                $"WHERE {LotteryLogAttribute.PublishmentSystemID} = {publishmentSystemID} AND {LotteryLogAttribute.LotteryID} = {lotteryID}";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
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
