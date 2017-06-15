using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class LotteryWinnerDao : DataProviderBase
    {
        private const string TableName = "wx_LotteryWinner";

        public int Insert(LotteryWinnerInfo winnerInfo)
        {
            var winnerId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(winnerInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);


            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        winnerId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return winnerId;
        }

        public void UpdateStatus(EWinStatus status, List<int> winnerIdList)
        {
            string sqlString =
                $"UPDATE {TableName} SET {LotteryWinnerAttribute.Status} = '{EWinStatusUtils.GetValue(status)}' WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(winnerIdList)})";

            if (status == EWinStatus.Cashed)
            {
                sqlString =
                    $"UPDATE {TableName} SET {LotteryWinnerAttribute.Status} = '{EWinStatusUtils.GetValue(status)}', {LotteryWinnerAttribute.CashDate} = getdate() WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(winnerIdList)})";
            }

            ExecuteNonQuery(sqlString);
        }

        public void Update(LotteryWinnerInfo winnerInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(winnerInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);
        }

        private void UpdateUserCount(int publishmentSystemId)
        {
            var lotteryIdWithCount = new Dictionary<int, int>();

            string sqlString =
                $"SELECT {LotteryWinnerAttribute.LotteryId}, COUNT(*) FROM {TableName} WHERE {LotteryWinnerAttribute.PublishmentSystemId} = {publishmentSystemId} GROUP BY {LotteryWinnerAttribute.LotteryId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    lotteryIdWithCount.Add(rdr.GetInt32(0), rdr.GetInt32(1));
                }
                rdr.Close();
            }

            DataProviderWx.LotteryDao.UpdateUserCount(publishmentSystemId, lotteryIdWithCount);

        }

        public void Delete(int publishmentSystemId, List<int> winnerIdList)
        {

            var awardIdList = new List<int>();

            if (winnerIdList != null && winnerIdList.Count > 0)
            {

                for (var i = 0; i < winnerIdList.Count; i++)
                {
                    var getAwardId = GetWinnerInfo(winnerIdList[i]).AwardId;
                    awardIdList.Add(getAwardId);
                }

                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(winnerIdList)})";
                ExecuteNonQuery(sqlString);

                UpdateUserCount(publishmentSystemId);

                for (var j = 0; j < awardIdList.Count; j++)
                {
                    DataProviderWx.LotteryAwardDao.UpdateWonNum(awardIdList[j]);
                }
            }

        }

        public void DeleteAll(int lotteryId)
        {
            if (lotteryId > 0)
            {
                string sqlString = $"DELETE FROM {TableName} WHERE {LotteryWinnerAttribute.LotteryId} = {lotteryId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public LotteryWinnerInfo GetWinnerInfo(int winnerId)
        {
            LotteryWinnerInfo winnerInfo = null;

            string sqlWhere = $"WHERE ID = {winnerId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    winnerInfo = new LotteryWinnerInfo(rdr);
                }
                rdr.Close();
            }

            return winnerInfo;
        }

        public LotteryWinnerInfo GetWinnerInfo(int publishmentSystemId, int lotteryId, string cookieSn, string wxOpenId, string userName)
        {

            ///改成 wxopenID

            var sqlWhere = "";

            LotteryWinnerInfo winnerInfo = null;

            if (string.IsNullOrEmpty(wxOpenId))
            {
                sqlWhere =
                    $"WHERE {LotteryWinnerAttribute.PublishmentSystemId} = {publishmentSystemId} AND {LotteryWinnerAttribute.LotteryId} = {lotteryId} AND {LotteryWinnerAttribute.CookieSn}= '{cookieSn}'";
            }
            else
            {
                sqlWhere =
                    $"WHERE {LotteryWinnerAttribute.PublishmentSystemId} = {publishmentSystemId} AND {LotteryWinnerAttribute.LotteryId} = {lotteryId} AND {LotteryWinnerAttribute.WxOpenId}= '{wxOpenId}'";
            }
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
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
                if (!string.IsNullOrEmpty(wxOpenId))
                {
                    sqlWhere += $" AND {LotteryWinnerAttribute.WxOpenId} = '{wxOpenId}'";
                }
                else if (!string.IsNullOrEmpty(userName))
                {
                    sqlWhere += $" AND {LotteryWinnerAttribute.UserName} = '{userName}'";
                }

                sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

                using (var rdr = ExecuteReader(sqlSelect))
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

        public List<LotteryWinnerInfo> GetWinnerInfoList(int publishmentSystemId, ELotteryType lotteryType, int lotteryId)
        {
            var winnerInfoList = new List<LotteryWinnerInfo>();

            string sqlWhere =
                $"WHERE {LotteryWinnerAttribute.PublishmentSystemId} = {publishmentSystemId} AND {LotteryWinnerAttribute.LotteryType} = '{ELotteryTypeUtils.GetValue(lotteryType)}'";
            if (lotteryId > 0)
            {
                sqlWhere =
                    $"WHERE {LotteryWinnerAttribute.PublishmentSystemId} = {publishmentSystemId} AND {LotteryWinnerAttribute.LotteryId} = {lotteryId}";
            }

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, "ORDER BY ID DESC");

            using (var rdr = ExecuteReader(sqlSelect))
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

        public int GetTotalNum(int publishmentSystemId, ELotteryType lotteryType, int lotteryId)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TableName} WHERE {LotteryWinnerAttribute.PublishmentSystemId} = {publishmentSystemId} AND {LotteryWinnerAttribute.LotteryType} = '{ELotteryTypeUtils.GetValue(lotteryType)}'";

            if (lotteryId > 0)
            {
                sqlString =
                    $"SELECT COUNT(*) FROM {TableName} WHERE {LotteryWinnerAttribute.PublishmentSystemId} = {publishmentSystemId} AND {LotteryWinnerAttribute.LotteryId} = {lotteryId}";
            }

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetTotalNum(int awardId)
        {
            string sqlString = $"SELECT COUNT(*) FROM {TableName} WHERE {LotteryWinnerAttribute.AwardId} = {awardId}";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetCashNum(int publishmentSystemId, int lotteryId)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TableName} WHERE {LotteryWinnerAttribute.PublishmentSystemId} = {publishmentSystemId} AND {LotteryWinnerAttribute.LotteryId} = {lotteryId} AND {LotteryWinnerAttribute.Status} = '{EWinStatusUtils.GetValue(EWinStatus.Cashed)}'";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetSelectString(int publishmentSystemId, ELotteryType lotteryType, int lotteryId, int awardId)
        {
            string whereString =
                $"WHERE {LotteryWinnerAttribute.PublishmentSystemId} = {publishmentSystemId} AND {LotteryWinnerAttribute.LotteryType} = '{ELotteryTypeUtils.GetValue(lotteryType)}'";
            if (lotteryId > 0)
            {
                whereString =
                    $"WHERE {LotteryWinnerAttribute.PublishmentSystemId} = {publishmentSystemId} AND {LotteryWinnerAttribute.LotteryId} = {lotteryId}";
            }
            if (awardId > 0)
            {
                whereString = $" AND {LotteryWinnerAttribute.AwardId} = {awardId}";
            }
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public List<LotteryWinnerInfo> GetLotteryWinnerInfoList(int publishmentSystemId, int lotteryId, int awardId)
        {
            var lotteryWinnerInfoList = new List<LotteryWinnerInfo>();

            string sqlWhere =
                $"WHERE {LotteryWinnerAttribute.PublishmentSystemId} = {publishmentSystemId} AND {LotteryWinnerAttribute.LotteryId} = {lotteryId} AND {LotteryWinnerAttribute.AwardId} = {awardId}";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
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
