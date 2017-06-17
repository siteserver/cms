using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class VoteDao : DataProviderBase
    {
        private const string TableName = "wx_Vote";

        public int Insert(VoteInfo voteInfo)
        {
            var voteId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(voteInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        voteId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return voteId;
        }

        public void Update(VoteInfo voteInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(voteInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);
        }

        public void AddUserCount(int voteId)
        {
            if (voteId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {VoteAttribute.UserCount} = {VoteAttribute.UserCount} + 1 WHERE ID = {voteId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void AddPvCount(int voteId)
        {
            if (voteId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {VoteAttribute.PvCount} = {VoteAttribute.PvCount} + 1 WHERE ID = {voteId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemId, int voteId)
        {
            if (voteId > 0)
            {
                var voteIdList = new List<int>();
                voteIdList.Add(voteId);
                DataProviderWx.KeywordDao.Delete(GetKeywordIdList(voteIdList));

                DataProviderWx.VoteLogDao.DeleteAll(voteId);
                DataProviderWx.VoteItemDao.DeleteAll(publishmentSystemId, voteId);

                string sqlString = $"DELETE FROM {TableName} WHERE ID = {voteId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemId, List<int> voteIdList)
        {
            if (voteIdList != null  && voteIdList.Count > 0)
            {
                DataProviderWx.KeywordDao.Delete(GetKeywordIdList(voteIdList));

                foreach (var voteId in voteIdList)
                {
                    DataProviderWx.VoteLogDao.DeleteAll(voteId);
                    DataProviderWx.VoteItemDao.DeleteAll(publishmentSystemId, voteId);
                }

                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(voteIdList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetKeywordIdList(List<int> voteIdList)
        {
            var keywordIdList = new List<int>();

            string sqlString =
                $"SELECT {VoteAttribute.KeywordId} FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(voteIdList)})";

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

        public VoteInfo GetVoteInfo(int voteId)
        {
            VoteInfo voteInfo = null;

            string sqlWhere = $"WHERE ID = {voteId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    voteInfo = new VoteInfo(rdr);
                }
                rdr.Close();
            }

            return voteInfo;
        }

        public List<VoteInfo> GetVoteInfoListByKeywordId(int publishmentSystemId, int keywordId)
        {
            var voteInfoList = new List<VoteInfo>();

            string sqlWhere =
                $"WHERE {VoteAttribute.PublishmentSystemId} = {publishmentSystemId} AND {VoteAttribute.IsDisabled} <> '{true}'";
            if (keywordId > 0)
            {
                sqlWhere += $" AND {VoteAttribute.KeywordId} = {keywordId}";
            }

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
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

        public int GetFirstIdByKeywordId(int publishmentSystemId, int keywordId)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TableName} WHERE {VoteAttribute.PublishmentSystemId} = {publishmentSystemId} AND {VoteAttribute.IsDisabled} <> '{true}' AND {VoteAttribute.KeywordId} = {keywordId}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetTitle(int voteId)
        {
            var title = string.Empty;

            string sqlWhere = $"WHERE ID = {voteId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, VoteAttribute.Title, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    title = rdr.GetValue(0).ToString();
                }
                rdr.Close();
            }

            return title;
        }

        public string GetSelectString(int publishmentSystemId)
        {
            string whereString = $"WHERE {VoteAttribute.PublishmentSystemId} = {publishmentSystemId}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public void UpdateUserCountById(int cNum, int voteId)
        {
            if (voteId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {VoteAttribute.UserCount} = {cNum} WHERE ID = {voteId} ";
                ExecuteNonQuery(sqlString);
            }
        }

        public List<VoteInfo> GetVoteInfoList(int publishmentSystemId)
        {
            var voteInfoList = new List<VoteInfo>();

            string sqlWhere = $"WHERE {LotteryAttribute.PublishmentSystemId} = {publishmentSystemId}";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
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
