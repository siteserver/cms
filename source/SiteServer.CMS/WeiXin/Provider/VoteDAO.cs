using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class VoteDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_Vote";

        public int Insert(VoteInfo voteInfo)
        {
            var voteID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(voteInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        voteID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return voteID;
        }

        public void Update(VoteInfo voteInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(voteInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        public void AddUserCount(int voteID)
        {
            if (voteID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {VoteAttribute.UserCount} = {VoteAttribute.UserCount} + 1 WHERE ID = {voteID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void AddPVCount(int voteID)
        {
            if (voteID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {VoteAttribute.PVCount} = {VoteAttribute.PVCount} + 1 WHERE ID = {voteID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemID, int voteID)
        {
            if (voteID > 0)
            {
                var voteIDList = new List<int>();
                voteIDList.Add(voteID);
                DataProviderWX.KeywordDAO.Delete(GetKeywordIDList(voteIDList));

                DataProviderWX.VoteLogDAO.DeleteAll(voteID);
                DataProviderWX.VoteItemDAO.DeleteAll(publishmentSystemID, voteID);

                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE ID = {voteID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemID, List<int> voteIDList)
        {
            if (voteIDList != null  && voteIDList.Count > 0)
            {
                DataProviderWX.KeywordDAO.Delete(GetKeywordIDList(voteIDList));

                foreach (var voteID in voteIDList)
                {
                    DataProviderWX.VoteLogDAO.DeleteAll(voteID);
                    DataProviderWX.VoteItemDAO.DeleteAll(publishmentSystemID, voteID);
                }

                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(voteIDList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetKeywordIDList(List<int> voteIDList)
        {
            var keywordIDList = new List<int>();

            string sqlString =
                $"SELECT {VoteAttribute.KeywordID} FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(voteIDList)})";

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

        public VoteInfo GetVoteInfo(int voteID)
        {
            VoteInfo voteInfo = null;

            string SQL_WHERE = $"WHERE ID = {voteID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    voteInfo = new VoteInfo(rdr);
                }
                rdr.Close();
            }

            return voteInfo;
        }

        public List<VoteInfo> GetVoteInfoListByKeywordID(int publishmentSystemID, int keywordID)
        {
            var voteInfoList = new List<VoteInfo>();

            string SQL_WHERE =
                $"WHERE {VoteAttribute.PublishmentSystemID} = {publishmentSystemID} AND {VoteAttribute.IsDisabled} <> '{true}'";
            if (keywordID > 0)
            {
                SQL_WHERE += $" AND {VoteAttribute.KeywordID} = {keywordID}";
            }

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var voteInfo = new VoteInfo(rdr);
                    voteInfoList.Add(voteInfo);
                }
                rdr.Close();
            }

            return voteInfoList;
        }

        public int GetFirstIDByKeywordID(int publishmentSystemID, int keywordID)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TABLE_NAME} WHERE {VoteAttribute.PublishmentSystemID} = {publishmentSystemID} AND {VoteAttribute.IsDisabled} <> '{true}' AND {VoteAttribute.KeywordID} = {keywordID}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetTitle(int voteID)
        {
            var title = string.Empty;

            string SQL_WHERE = $"WHERE ID = {voteID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, VoteAttribute.Title, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    title = rdr.GetValue(0).ToString();
                }
                rdr.Close();
            }

            return title;
        }

        public string GetSelectString(int publishmentSystemID)
        {
            string whereString = $"WHERE {VoteAttribute.PublishmentSystemID} = {publishmentSystemID}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public void UpdateUserCountByID(int CNum, int voteID)
        {
            if (voteID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {VoteAttribute.UserCount} = {CNum} WHERE ID = {voteID} ";
                ExecuteNonQuery(sqlString);
            }
        }

        public List<VoteInfo> GetVoteInfoList(int publishmentSystemID)
        {
            var voteInfoList = new List<VoteInfo>();

            string SQL_WHERE = $"WHERE {LotteryAttribute.PublishmentSystemID} = {publishmentSystemID}";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var voteInfo = new VoteInfo(rdr);
                    voteInfoList.Add(voteInfo);
                }
                rdr.Close();
            }

            return voteInfoList;
        }

    }
}
