using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class LotteryDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_Lottery";

        public int Insert(LotteryInfo lotteryInfo)
        {
            var lotteryID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(lotteryInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        lotteryID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return lotteryID;
        }

        public void Update(LotteryInfo lotteryInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(lotteryInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);


            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        public List<int> GetLotteryIDList(int publishmentSystemID, ELotteryType lotteryType)
        {
            var lotteryIDList = new List<int>();

            string SQL_WHERE =
                $"WHERE {LotteryAttribute.PublishmentSystemID} = {publishmentSystemID} AND {LotteryAttribute.LotteryType} = '{ELotteryTypeUtils.GetValue(lotteryType)}'";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, LotteryAttribute.ID, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    lotteryIDList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return lotteryIDList;
        }

        private List<int> GetLotteryIDList(int publishmentSystemID)
        {
            var lotteryIDList = new List<int>();

            string SQL_WHERE = $"WHERE {LotteryAttribute.PublishmentSystemID} = {publishmentSystemID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, LotteryAttribute.ID, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    lotteryIDList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return lotteryIDList;
        }

        public void UpdateUserCount(int publishmentSystemID, Dictionary<int, int> lotteryIDWithCount)
        {
            if (lotteryIDWithCount.Count == 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {LotteryAttribute.UserCount} = 0 WHERE {LotteryAttribute.PublishmentSystemID} = {publishmentSystemID}";
                ExecuteNonQuery(sqlString);
            }
            else
            {
                var lotteryIDList = GetLotteryIDList(publishmentSystemID);
                foreach (var lotteryID in lotteryIDList)
                {
                    if (lotteryIDWithCount.ContainsKey(lotteryID))
                    {
                        string sqlString =
                            $"UPDATE {TABLE_NAME} SET {LotteryAttribute.UserCount} = {lotteryIDWithCount[lotteryID]} WHERE ID = {lotteryID}";
                        ExecuteNonQuery(sqlString);
                    }
                    else
                    {
                        string sqlString =
                            $"UPDATE {TABLE_NAME} SET {LotteryAttribute.UserCount} = 0 WHERE ID = {lotteryID}";
                        ExecuteNonQuery(sqlString);
                    }
                }
            }
        }

        public void Delete(int publishmentSystemID, List<int> lotteryIDList)
        {
            if (lotteryIDList != null && lotteryIDList.Count > 0)
            {
                DataProviderWX.KeywordDAO.Delete(GetKeywordIDList(lotteryIDList));

                foreach (var lotteryID in lotteryIDList)
                {
                    DataProviderWX.LotteryAwardDAO.DeleteAll(lotteryID);
                    DataProviderWX.LotteryWinnerDAO.DeleteAll(lotteryID);
                    DataProviderWX.LotteryLogDAO.DeleteAll(lotteryID);
                }

                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(lotteryIDList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetKeywordIDList(List<int> lotteryIDList)
        {
            var keywordIDList = new List<int>();

            string sqlString =
                $"SELECT {LotteryAttribute.KeywordID} FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(lotteryIDList)})";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    keywordIDList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return keywordIDList;
        }

        public LotteryInfo GetLotteryInfo(int lotteryID)
        {
            LotteryInfo lotteryInfo = null;

            string SQL_WHERE = $"WHERE ID = {lotteryID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    lotteryInfo = new LotteryInfo(rdr);
                }
                rdr.Close();
            }

            return lotteryInfo;
        }

        public List<LotteryInfo> GetLotteryInfoListByKeywordID(int publishmentSystemID, ELotteryType lotteryType, int keywordID)
        {
            var lotteryInfoList = new List<LotteryInfo>();

            string SQL_WHERE =
                $"WHERE {LotteryAttribute.PublishmentSystemID} = {publishmentSystemID} AND {LotteryAttribute.LotteryType} = '{ELotteryTypeUtils.GetValue(lotteryType)}' AND {LotteryAttribute.IsDisabled} <> '{true}'";
            if (keywordID > 0)
            {
                SQL_WHERE += $" AND {LotteryAttribute.KeywordID} = {keywordID}";
            }
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
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

        public int GetFirstIDByKeywordID(int publishmentSystemID, ELotteryType lotteryType, int keywordID)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TABLE_NAME} WHERE {LotteryAttribute.PublishmentSystemID} = {publishmentSystemID} AND {LotteryAttribute.LotteryType} = '{ELotteryTypeUtils.GetValue(lotteryType)}' AND {LotteryAttribute.IsDisabled} <> '{true}' AND {LotteryAttribute.KeywordID} = {keywordID}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetKeywordID(int lotteryID)
        {
            var keywordID = 0;

            string SQL_WHERE = $"WHERE ID = {lotteryID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, LotteryAttribute.KeywordID, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    keywordID = rdr.GetInt32(0);
                }
                rdr.Close();
            }

            return keywordID;
        }

        public string GetSelectString(int publishmentSystemID, ELotteryType lotteryType)
        {
            string whereString =
                $"WHERE {LotteryAttribute.PublishmentSystemID} = {publishmentSystemID} AND {LotteryAttribute.LotteryType} = '{ELotteryTypeUtils.GetValue(lotteryType)}'";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public void AddUserCount(int lotteryID)
        {
            if (lotteryID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {LotteryAttribute.UserCount} = {LotteryAttribute.UserCount} + 1 WHERE ID = {lotteryID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void AddPVCount(int lotteryID)
        {
            if (lotteryID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {LotteryAttribute.PVCount} = {LotteryAttribute.PVCount} + 1 WHERE ID = {lotteryID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public List<LotteryInfo> GetLotteryInfoList(int publishmentSystemID)
        {
            var lotteryInfoList = new List<LotteryInfo>();

            string SQL_WHERE = $"WHERE {LotteryAttribute.PublishmentSystemID} = {publishmentSystemID}";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
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
