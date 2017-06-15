using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class VoteItemDao : DataProviderBase
    {
        private const string TableName = "wx_VoteItem";

        
        public int Insert(VoteItemInfo itemInfo)
        {
            var voteItemId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(itemInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        voteItemId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return voteItemId;
        }

        public void Update(VoteItemInfo itemInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(itemInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);
        }

        public void UpdateVoteId(int publishmentSystemId, int voteId)
        {
            if (voteId > 0)
            {
                var sqlString =
                    $"UPDATE {TableName} SET {VoteItemAttribute.VoteId} = {voteId} WHERE {VoteItemAttribute.VoteId} = 0 AND {VoteItemAttribute.PublishmentSystemId} = {publishmentSystemId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteAll(int publishmentSystemId, int voteId)
        {
            if (voteId > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE {VoteItemAttribute.PublishmentSystemId} = {publishmentSystemId} AND {VoteItemAttribute.VoteId} = {voteId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public VoteItemInfo GetVoteItemInfo(int itemId)
        {
            VoteItemInfo voteItemInfo = null;

            string sqlWhere = $"WHERE ID = {itemId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    voteItemInfo = new VoteItemInfo(rdr);
                }
                rdr.Close();
            }

            return voteItemInfo;
        }


        public List<VoteItemInfo> GetVoteItemInfoList(int voteId)
        {
            var list = new List<VoteItemInfo>();

            string sqlWhere = $"WHERE {VoteItemAttribute.VoteId} = {voteId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var itemInfo = new VoteItemInfo(rdr);
                    list.Add(itemInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public void AddVoteNum(int voteId, List<int> itemIdList)
        {
            if (voteId > 0 && itemIdList != null && itemIdList.Count > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {VoteItemAttribute.VoteNum} = {VoteItemAttribute.VoteNum} + 1 WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(itemIdList)}) AND VoteID = {voteId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void UpdateVoteNumById(int vNum, int voteItemId)
        {
            if (voteItemId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {VoteItemAttribute.VoteNum} = {vNum} WHERE ID = {voteItemId} ";
                ExecuteNonQuery(sqlString);
            }
        }

        public void UpdateAllVoteNumByVoteId(int vNum, int voteId)
        {
            if (voteId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {VoteItemAttribute.VoteNum} = {vNum} WHERE VoteID = {voteId} ";
                ExecuteNonQuery(sqlString);
            }
        }

        public void UpdateOtherVoteNumByIdList(List<int> logIdList, int vNum, int voteId)
        {
            if (logIdList != null && logIdList.Count > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {VoteItemAttribute.VoteNum} = {vNum} WHERE VoteID = {voteId} AND ID NOT IN ({TranslateUtils.ToSqlInStringWithoutQuote(logIdList)}) ";
                ExecuteNonQuery(sqlString);
            }
        }

        public List<VoteItemInfo> GetVoteItemInfoList(int publishmentSystemId, int voteId)
        {
            var list = new List<VoteItemInfo>();

            string sqlWhere =
                $"WHERE {VoteItemAttribute.PublishmentSystemId} = {publishmentSystemId} AND {VoteItemAttribute.VoteId} = {voteId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var itemInfo = new VoteItemInfo(rdr);
                    list.Add(itemInfo);
                }
                rdr.Close();
            }

            return list;
        }         
    }
}
