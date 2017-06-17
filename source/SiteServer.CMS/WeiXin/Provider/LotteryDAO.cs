using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class LotteryDao : DataProviderBase
    {
        private const string TableName = "wx_Lottery";

        public int Insert(LotteryInfo lotteryInfo)
        {
            var lotteryId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(lotteryInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        lotteryId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return lotteryId;
        }

        public void Update(LotteryInfo lotteryInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(lotteryInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);


            ExecuteNonQuery(sqlUpdate, parms);
        }

        public List<int> GetLotteryIdList(int publishmentSystemId, ELotteryType lotteryType)
        {
            var lotteryIdList = new List<int>();

            string sqlWhere =
                $"WHERE {LotteryAttribute.PublishmentSystemId} = {publishmentSystemId} AND {LotteryAttribute.LotteryType} = '{ELotteryTypeUtils.GetValue(lotteryType)}'";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, LotteryAttribute.Id, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    lotteryIdList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return lotteryIdList;
        }

        private List<int> GetLotteryIdList(int publishmentSystemId)
        {
            var lotteryIdList = new List<int>();

            string sqlWhere = $"WHERE {LotteryAttribute.PublishmentSystemId} = {publishmentSystemId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, LotteryAttribute.Id, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    lotteryIdList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return lotteryIdList;
        }

        public void UpdateUserCount(int publishmentSystemId, Dictionary<int, int> lotteryIdWithCount)
        {
            if (lotteryIdWithCount.Count == 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {LotteryAttribute.UserCount} = 0 WHERE {LotteryAttribute.PublishmentSystemId} = {publishmentSystemId}";
                ExecuteNonQuery(sqlString);
            }
            else
            {
                var lotteryIdList = GetLotteryIdList(publishmentSystemId);
                foreach (var lotteryId in lotteryIdList)
                {
                    if (lotteryIdWithCount.ContainsKey(lotteryId))
                    {
                        string sqlString =
                            $"UPDATE {TableName} SET {LotteryAttribute.UserCount} = {lotteryIdWithCount[lotteryId]} WHERE ID = {lotteryId}";
                        ExecuteNonQuery(sqlString);
                    }
                    else
                    {
                        string sqlString =
                            $"UPDATE {TableName} SET {LotteryAttribute.UserCount} = 0 WHERE ID = {lotteryId}";
                        ExecuteNonQuery(sqlString);
                    }
                }
            }
        }

        public void Delete(int publishmentSystemId, List<int> lotteryIdList)
        {
            if (lotteryIdList != null && lotteryIdList.Count > 0)
            {
                DataProviderWx.KeywordDao.Delete(GetKeywordIdList(lotteryIdList));

                foreach (var lotteryId in lotteryIdList)
                {
                    DataProviderWx.LotteryAwardDao.DeleteAll(lotteryId);
                    DataProviderWx.LotteryWinnerDao.DeleteAll(lotteryId);
                    DataProviderWx.LotteryLogDao.DeleteAll(lotteryId);
                }

                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(lotteryIdList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetKeywordIdList(List<int> lotteryIdList)
        {
            var keywordIdList = new List<int>();

            string sqlString =
                $"SELECT {LotteryAttribute.KeywordId} FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(lotteryIdList)})";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    keywordIdList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return keywordIdList;
        }

        public LotteryInfo GetLotteryInfo(int lotteryId)
        {
            LotteryInfo lotteryInfo = null;

            string sqlWhere = $"WHERE ID = {lotteryId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    lotteryInfo = new LotteryInfo(rdr);
                }
                rdr.Close();
            }

            return lotteryInfo;
        }

        public List<LotteryInfo> GetLotteryInfoListByKeywordId(int publishmentSystemId, ELotteryType lotteryType, int keywordId)
        {
            var lotteryInfoList = new List<LotteryInfo>();

            string sqlWhere =
                $"WHERE {LotteryAttribute.PublishmentSystemId} = {publishmentSystemId} AND {LotteryAttribute.LotteryType} = '{ELotteryTypeUtils.GetValue(lotteryType)}' AND {LotteryAttribute.IsDisabled} <> '{true}'";
            if (keywordId > 0)
            {
                sqlWhere += $" AND {LotteryAttribute.KeywordId} = {keywordId}";
            }
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var lotteryInfo = new LotteryInfo(rdr);
                    lotteryInfoList.Add(lotteryInfo);
                }
                rdr.Close();
            }

            return lotteryInfoList;
        }

        public int GetFirstIdByKeywordId(int publishmentSystemId, ELotteryType lotteryType, int keywordId)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TableName} WHERE {LotteryAttribute.PublishmentSystemId} = {publishmentSystemId} AND {LotteryAttribute.LotteryType} = '{ELotteryTypeUtils.GetValue(lotteryType)}' AND {LotteryAttribute.IsDisabled} <> '{true}' AND {LotteryAttribute.KeywordId} = {keywordId}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetKeywordId(int lotteryId)
        {
            var keywordId = 0;

            string sqlWhere = $"WHERE ID = {lotteryId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, LotteryAttribute.KeywordId, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    keywordId = rdr.GetInt32(0);
                }
                rdr.Close();
            }

            return keywordId;
        }

        public string GetSelectString(int publishmentSystemId, ELotteryType lotteryType)
        {
            string whereString =
                $"WHERE {LotteryAttribute.PublishmentSystemId} = {publishmentSystemId} AND {LotteryAttribute.LotteryType} = '{ELotteryTypeUtils.GetValue(lotteryType)}'";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public void AddUserCount(int lotteryId)
        {
            if (lotteryId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {LotteryAttribute.UserCount} = {LotteryAttribute.UserCount} + 1 WHERE ID = {lotteryId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void AddPvCount(int lotteryId)
        {
            if (lotteryId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {LotteryAttribute.PvCount} = {LotteryAttribute.PvCount} + 1 WHERE ID = {lotteryId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public List<LotteryInfo> GetLotteryInfoList(int publishmentSystemId)
        {
            var lotteryInfoList = new List<LotteryInfo>();

            string sqlWhere = $"WHERE {LotteryAttribute.PublishmentSystemId} = {publishmentSystemId}";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var lotteryInfo = new LotteryInfo(rdr);
                    lotteryInfoList.Add(lotteryInfo);
                }
                rdr.Close();
            }

            return lotteryInfoList;
        }
    }
}
