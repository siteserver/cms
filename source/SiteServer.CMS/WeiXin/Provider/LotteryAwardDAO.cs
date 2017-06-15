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
    public class LotteryAwardDao : DataProviderBase
    {
        private const string TableName = "wx_LotteryAward";
          
        public int Insert(LotteryAwardInfo awardInfo)
        {
            var awardId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(awardInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);
             
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        awardId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return awardId;
        }

        public void Update(LotteryAwardInfo awardInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(awardInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);


            ExecuteNonQuery(sqlUpdate, parms);
        }

        public void UpdateTotalNum(int awardId, int totalNum)
        {
            string sqlString =
                $"UPDATE {TableName} SET {LotteryAwardAttribute.TotalNum} = {totalNum} WHERE ID = {awardId}";

            ExecuteNonQuery(sqlString);
        }

        public void UpdateWonNum(int awardId)
        {
            var wonNum = DataProviderWx.LotteryWinnerDao.GetTotalNum(awardId);
            string sqlString = $"UPDATE {TableName} SET {LotteryAwardAttribute.WonNum} = {wonNum} WHERE ID = {awardId}";

            ExecuteNonQuery(sqlString);
        }

        public void UpdateLotteryId(int publishmentSystemId, int lotteryId)
        {
            if (lotteryId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {LotteryAwardAttribute.LotteryId} = {lotteryId} WHERE {LotteryAwardAttribute.LotteryId} = 0 AND {LotteryAwardAttribute.PublishmentSystemId} = {publishmentSystemId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int awardId)
        {
            if (awardId > 0)
            {
                string sqlString = $"DELETE FROM {TableName} WHERE ID = {awardId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(List<int> awardIdList)
        {
            if (awardIdList != null  && awardIdList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(awardIdList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteAll(int lotteryId)
        {
            if (lotteryId > 0)
            {
                string sqlString = $"DELETE FROM {TableName} WHERE {LotteryAwardAttribute.LotteryId} = {lotteryId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteAllNotInIdList(int publishmentSystemId, int lotteryId, List<int> idList)
        {
            if (lotteryId > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE {LotteryAwardAttribute.PublishmentSystemId} = {publishmentSystemId} AND {LotteryAwardAttribute.LotteryId} = {lotteryId}";
                if (idList != null && idList.Count > 0)
                {
                    sqlString =
                        $"DELETE FROM {TableName} WHERE {LotteryAwardAttribute.PublishmentSystemId} = {publishmentSystemId} AND {LotteryAwardAttribute.LotteryId} = {lotteryId} AND ID NOT IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";
                }
                ExecuteNonQuery(sqlString);
            }
        }

        public LotteryAwardInfo GetAwardInfo(int awardId)
        {
            LotteryAwardInfo awardInfo = null;

            string sqlWhere = $"WHERE ID = {awardId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    awardInfo = new LotteryAwardInfo(rdr);
                }
                rdr.Close();
            }

            return awardInfo;
        }

        public string GetSelectString(int publishmentSystemId)
        {
            string whereString = $"WHERE {LotteryAwardAttribute.PublishmentSystemId} = {publishmentSystemId}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public Dictionary<string, int> GetLotteryAwardDictionary(int lotteryId)
        {
            var dictionary = new Dictionary<string, int>();

            string sqlWhere = $"WHERE {LotteryAwardAttribute.LotteryId} = {lotteryId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, LotteryAwardAttribute.Title + "," + LotteryAwardAttribute.TotalNum, sqlWhere);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    dictionary.Add(rdr.GetValue(0).ToString(), rdr.GetInt32(1));
                }
                rdr.Close();
            }

            return dictionary;
        }

        public List<LotteryAwardInfo> GetLotteryAwardInfoList(int publishmentSystemId, int lotteryId)
        {
            var list = new List<LotteryAwardInfo>();

            var builder = new StringBuilder(
                $"WHERE {LotteryAwardAttribute.PublishmentSystemId} = {publishmentSystemId} AND {LotteryAwardAttribute.LotteryId} = {lotteryId}");
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, builder.ToString(), "ORDER BY ID");

            using (var rdr = ExecuteReader(sqlSelect))
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

        public void GetCount(int publishmentSystemId, ELotteryType lotteryType, int lotteryId, out int totalNum, out int wonNum)
        {
            totalNum = 0;
            wonNum = 0;

            var lotteryIdList = new List<int>();
            if (lotteryId == 0)
            {
                lotteryIdList = DataProviderWx.LotteryDao.GetLotteryIdList(publishmentSystemId, lotteryType);
            }
            else
            {
                lotteryIdList.Add(lotteryId);
            }

            string sqlString =
                $"SELECT SUM(TotalNum), SUM(WonNum) FROM {TableName} WHERE {LotteryWinnerAttribute.PublishmentSystemId} = {publishmentSystemId} AND {LotteryWinnerAttribute.LotteryId} IN ({TranslateUtils.ToSqlInStringWithoutQuote(lotteryIdList)})";

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

        public List<LotteryAwardInfo> GetLotteryAwardInfoList(int publishmentSystemId)
        {
            var list = new List<LotteryAwardInfo>();

            var builder = new StringBuilder(
                $"WHERE {LotteryAwardAttribute.PublishmentSystemId} = {publishmentSystemId}");
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, builder.ToString(), "ORDER BY ID");

            using (var rdr = ExecuteReader(sqlSelect))
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
