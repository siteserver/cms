using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class LotteryWinnerDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_LotteryWinner";

        public int Insert(LotteryWinnerInfo winnerInfo)
        {
            var winnerID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(winnerInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);


            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        winnerID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return winnerID;
        }

        public void UpdateStatus(EWinStatus status, List<int> winnerIDList)
        {
            string sqlString =
                $"UPDATE {TABLE_NAME} SET {LotteryWinnerAttribute.Status} = '{EWinStatusUtils.GetValue(status)}' WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(winnerIDList)})";

            if (status == EWinStatus.Cashed)
            {
                sqlString =
                    $"UPDATE {TABLE_NAME} SET {LotteryWinnerAttribute.Status} = '{EWinStatusUtils.GetValue(status)}', {LotteryWinnerAttribute.CashDate} = getdate() WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(winnerIDList)})";
            }

            ExecuteNonQuery(sqlString);
        }

        public void Update(LotteryWinnerInfo winnerInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(winnerInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        private void UpdateUserCount(int publishmentSystemID)
        {
            var lotteryIDWithCount = new Dictionary<int, int>();

            string sqlString =
                $"SELECT {LotteryWinnerAttribute.LotteryID}, COUNT(*) FROM {TABLE_NAME} WHERE {LotteryWinnerAttribute.PublishmentSystemID} = {publishmentSystemID} GROUP BY {LotteryWinnerAttribute.LotteryID}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    lotteryIDWithCount.Add(rdr.GetInt32(0), rdr.GetInt32(1));
                }
                rdr.Close();
            }

            DataProviderWX.LotteryDAO.UpdateUserCount(publishmentSystemID, lotteryIDWithCount);

        }

        public void Delete(int publishmentSystemID, List<int> winnerIDList)
        {

            var awardIDList = new List<int>();

            if (winnerIDList != null && winnerIDList.Count > 0)
            {

                for (var i = 0; i < winnerIDList.Count; i++)
                {
                    var getAwardID = GetWinnerInfo(winnerIDList[i]).AwardID;
                    awardIDList.Add(getAwardID);
                }

                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(winnerIDList)})";
                ExecuteNonQuery(sqlString);

                UpdateUserCount(publishmentSystemID);

                for (var j = 0; j < awardIDList.Count; j++)
                {
                    DataProviderWX.LotteryAwardDAO.UpdateWonNum(awardIDList[j]);
                }
            }

        }

        public void DeleteAll(int lotteryID)
        {
            if (lotteryID > 0)
            {
                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE {LotteryWinnerAttribute.LotteryID} = {lotteryID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public LotteryWinnerInfo GetWinnerInfo(int winnerID)
        {
            LotteryWinnerInfo winnerInfo = null;

            string SQL_WHERE = $"WHERE ID = {winnerID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    winnerInfo = new LotteryWinnerInfo(rdr);
                }
                rdr.Close();
            }

            return winnerInfo;
        }

        public LotteryWinnerInfo GetWinnerInfo(int publishmentSystemID, int lotteryID, string cookieSN, string wxOpenID, string userName)
        {

            ///改成 wxopenID

            var SQL_WHERE = "";

            LotteryWinnerInfo winnerInfo = null;

            if (string.IsNullOrEmpty(wxOpenID))
            {
                SQL_WHERE =
                    $"WHERE {LotteryWinnerAttribute.PublishmentSystemID} = {publishmentSystemID} AND {LotteryWinnerAttribute.LotteryID} = {lotteryID} AND {LotteryWinnerAttribute.CookieSN}= '{cookieSN}'";
            }
            else
            {
                SQL_WHERE =
                    $"WHERE {LotteryWinnerAttribute.PublishmentSystemID} = {publishmentSystemID} AND {LotteryWinnerAttribute.LotteryID} = {lotteryID} AND {LotteryWinnerAttribute.WXOpenID}= '{wxOpenID}'";
            }
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    winnerInfo = new LotteryWinnerInfo(rdr);
                }
                rdr.Close();
            }

            if (winnerInfo == null)
            {
                //6月30日需要删除
                if (!string.IsNullOrEmpty(wxOpenID))
                {
                    SQL_WHERE += $" AND {LotteryWinnerAttribute.WXOpenID} = '{wxOpenID}'";
                }
                else if (!string.IsNullOrEmpty(userName))
                {
                    SQL_WHERE += $" AND {LotteryWinnerAttribute.UserName} = '{userName}'";
                }

                SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

                using (var rdr = ExecuteReader(SQL_SELECT))
                {
                    if (rdr.Read())
                    {
                        winnerInfo = new LotteryWinnerInfo(rdr);
                    }
                    rdr.Close();
                }
            }

            return winnerInfo;
        }

        public List<LotteryWinnerInfo> GetWinnerInfoList(int publishmentSystemID, ELotteryType lotteryType, int lotteryID)
        {
            var winnerInfoList = new List<LotteryWinnerInfo>();

            string SQL_WHERE =
                $"WHERE {LotteryWinnerAttribute.PublishmentSystemID} = {publishmentSystemID} AND {LotteryWinnerAttribute.LotteryType} = '{ELotteryTypeUtils.GetValue(lotteryType)}'";
            if (lotteryID > 0)
            {
                SQL_WHERE =
                    $"WHERE {LotteryWinnerAttribute.PublishmentSystemID} = {publishmentSystemID} AND {LotteryWinnerAttribute.LotteryID} = {lotteryID}";
            }

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, "ORDER BY ID DESC");

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var winnerInfo = new LotteryWinnerInfo(rdr);
                    winnerInfoList.Add(winnerInfo);
                }
                rdr.Close();
            }

            return winnerInfoList;
        }

        public int GetTotalNum(int publishmentSystemID, ELotteryType lotteryType, int lotteryID)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TABLE_NAME} WHERE {LotteryWinnerAttribute.PublishmentSystemID} = {publishmentSystemID} AND {LotteryWinnerAttribute.LotteryType} = '{ELotteryTypeUtils.GetValue(lotteryType)}'";

            if (lotteryID > 0)
            {
                sqlString =
                    $"SELECT COUNT(*) FROM {TABLE_NAME} WHERE {LotteryWinnerAttribute.PublishmentSystemID} = {publishmentSystemID} AND {LotteryWinnerAttribute.LotteryID} = {lotteryID}";
            }

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetTotalNum(int awardID)
        {
            string sqlString = $"SELECT COUNT(*) FROM {TABLE_NAME} WHERE {LotteryWinnerAttribute.AwardID} = {awardID}";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetCashNum(int publishmentSystemID, int lotteryID)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TABLE_NAME} WHERE {LotteryWinnerAttribute.PublishmentSystemID} = {publishmentSystemID} AND {LotteryWinnerAttribute.LotteryID} = {lotteryID} AND {LotteryWinnerAttribute.Status} = '{EWinStatusUtils.GetValue(EWinStatus.Cashed)}'";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetSelectString(int publishmentSystemID, ELotteryType lotteryType, int lotteryID, int awardID)
        {
            string whereString =
                $"WHERE {LotteryWinnerAttribute.PublishmentSystemID} = {publishmentSystemID} AND {LotteryWinnerAttribute.LotteryType} = '{ELotteryTypeUtils.GetValue(lotteryType)}'";
            if (lotteryID > 0)
            {
                whereString =
                    $"WHERE {LotteryWinnerAttribute.PublishmentSystemID} = {publishmentSystemID} AND {LotteryWinnerAttribute.LotteryID} = {lotteryID}";
            }
            if (awardID > 0)
            {
                whereString = $" AND {LotteryWinnerAttribute.AwardID} = {awardID}";
            }
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public List<LotteryWinnerInfo> GetLotteryWinnerInfoList(int publishmentSystemID, int lotteryID, int awardID)
        {
            var lotteryWinnerInfoList = new List<LotteryWinnerInfo>();

            string SQL_WHERE =
                $"WHERE {LotteryWinnerAttribute.PublishmentSystemID} = {publishmentSystemID} AND {LotteryWinnerAttribute.LotteryID} = {lotteryID} AND {LotteryWinnerAttribute.AwardID} = {awardID}";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var lotteryWinnerInfo = new LotteryWinnerInfo(rdr);
                    lotteryWinnerInfoList.Add(lotteryWinnerInfo);
                }
                rdr.Close();
            }

            return lotteryWinnerInfoList;
        }

    }
}
