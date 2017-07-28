using System.Collections.Generic;
using System.Data;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class LotteryAwardDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_LotteryAward";
          
        public int Insert(LotteryAwardInfo awardInfo)
        {
            var awardID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(awardInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);
             
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        awardID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return awardID;
        }

        public void Update(LotteryAwardInfo awardInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(awardInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);


            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        public void UpdateTotalNum(int awardID, int totalNum)
        {
            string sqlString =
                $"UPDATE {TABLE_NAME} SET {LotteryAwardAttribute.TotalNum} = {totalNum} WHERE ID = {awardID}";

            ExecuteNonQuery(sqlString);
        }

        public void UpdateWonNum(int awardID)
        {
            var wonNum = DataProviderWX.LotteryWinnerDAO.GetTotalNum(awardID);
            string sqlString = $"UPDATE {TABLE_NAME} SET {LotteryAwardAttribute.WonNum} = {wonNum} WHERE ID = {awardID}";

            ExecuteNonQuery(sqlString);
        }

        public void UpdateLotteryID(int publishmentSystemID, int lotteryID)
        {
            if (lotteryID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {LotteryAwardAttribute.LotteryID} = {lotteryID} WHERE {LotteryAwardAttribute.LotteryID} = 0 AND {LotteryAwardAttribute.PublishmentSystemID} = {publishmentSystemID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int awardID)
        {
            if (awardID > 0)
            {
                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE ID = {awardID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(List<int> awardIDList)
        {
            if (awardIDList != null  && awardIDList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(awardIDList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteAll(int lotteryID)
        {
            if (lotteryID > 0)
            {
                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE {LotteryAwardAttribute.LotteryID} = {lotteryID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteAllNotInIDList(int publishmentSystemID, int lotteryID, List<int> idList)
        {
            if (lotteryID > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE {LotteryAwardAttribute.PublishmentSystemID} = {publishmentSystemID} AND {LotteryAwardAttribute.LotteryID} = {lotteryID}";
                if (idList != null && idList.Count > 0)
                {
                    sqlString =
                        $"DELETE FROM {TABLE_NAME} WHERE {LotteryAwardAttribute.PublishmentSystemID} = {publishmentSystemID} AND {LotteryAwardAttribute.LotteryID} = {lotteryID} AND ID NOT IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";
                }
                ExecuteNonQuery(sqlString);
            }
        }

        public LotteryAwardInfo GetAwardInfo(int awardID)
        {
            LotteryAwardInfo awardInfo = null;

            string SQL_WHERE = $"WHERE ID = {awardID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    awardInfo = new LotteryAwardInfo(rdr);
                }
                rdr.Close();
            }

            return awardInfo;
        }

        public string GetSelectString(int publishmentSystemID)
        {
            string whereString = $"WHERE {LotteryAwardAttribute.PublishmentSystemID} = {publishmentSystemID}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public Dictionary<string, int> GetLotteryAwardDictionary(int lotteryID)
        {
            var dictionary = new Dictionary<string, int>();

            string SQL_WHERE = $"WHERE {LotteryAwardAttribute.LotteryID} = {lotteryID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, LotteryAwardAttribute.Title + "," + LotteryAwardAttribute.TotalNum, SQL_WHERE);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    dictionary.Add(rdr.GetValue(0).ToString(), rdr.GetInt32(1));
                }
                rdr.Close();
            }

            return dictionary;
        }

        public List<LotteryAwardInfo> GetLotteryAwardInfoList(int publishmentSystemID, int lotteryID)
        {
            var list = new List<LotteryAwardInfo>();

            var builder = new StringBuilder(
                $"WHERE {LotteryAwardAttribute.PublishmentSystemID} = {publishmentSystemID} AND {LotteryAwardAttribute.LotteryID} = {lotteryID}");
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, builder.ToString(), "ORDER BY ID");

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var awardInfo = new LotteryAwardInfo(rdr);
                    list.Add(awardInfo);
                }
                rdr.Close();
            }

            return list;
        }        

        public void GetCount(int publishmentSystemID, ELotteryType lotteryType, int lotteryID, out int totalNum, out int wonNum)
        {
            totalNum = 0;
            wonNum = 0;

            var lotteryIDList = new List<int>();
            if (lotteryID == 0)
            {
                lotteryIDList = DataProviderWX.LotteryDAO.GetLotteryIDList(publishmentSystemID, lotteryType);
            }
            else
            {
                lotteryIDList.Add(lotteryID);
            }

            string sqlString =
                $"SELECT SUM(TotalNum), SUM(WonNum) FROM {TABLE_NAME} WHERE {LotteryWinnerAttribute.PublishmentSystemID} = {publishmentSystemID} AND {LotteryWinnerAttribute.LotteryID} IN ({TranslateUtils.ToSqlInStringWithoutQuote(lotteryIDList)})";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    totalNum = rdr.GetInt32(0);
                    wonNum = rdr.GetInt32(1);
                }
                rdr.Close();
            }
        }

        public List<LotteryAwardInfo> GetLotteryAwardInfoList(int publishmentSystemID)
        {
            var list = new List<LotteryAwardInfo>();

            var builder = new StringBuilder(
                $"WHERE {LotteryAwardAttribute.PublishmentSystemID} = {publishmentSystemID}");
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, builder.ToString(), "ORDER BY ID");

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var awardInfo = new LotteryAwardInfo(rdr);
                    list.Add(awardInfo);
                }
                rdr.Close();
            }

            return list;
        }
    }
}
