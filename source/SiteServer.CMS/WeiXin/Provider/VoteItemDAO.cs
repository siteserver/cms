using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class VoteItemDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_VoteItem";

        
        public int Insert(VoteItemInfo itemInfo)
        {
            var voteItemID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(itemInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        voteItemID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return voteItemID;
        }

        public void Update(VoteItemInfo itemInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(itemInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        public void UpdateVoteID(int publishmentSystemID, int voteID)
        {
            if (voteID > 0)
            {
                var sqlString =
                    $"UPDATE {TABLE_NAME} SET {VoteItemAttribute.VoteID} = {voteID} WHERE {VoteItemAttribute.VoteID} = 0 AND {VoteItemAttribute.PublishmentSystemID} = {publishmentSystemID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteAll(int publishmentSystemID, int voteID)
        {
            if (voteID > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE {VoteItemAttribute.PublishmentSystemID} = {publishmentSystemID} AND {VoteItemAttribute.VoteID} = {voteID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public VoteItemInfo GetVoteItemInfo(int itemID)
        {
            VoteItemInfo voteItemInfo = null;

            string SQL_WHERE = $"WHERE ID = {itemID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    voteItemInfo = new VoteItemInfo(rdr);
                }
                rdr.Close();
            }

            return voteItemInfo;
        }


        public List<VoteItemInfo> GetVoteItemInfoList(int voteID)
        {
            var list = new List<VoteItemInfo>();

            string SQL_WHERE = $"WHERE {VoteItemAttribute.VoteID} = {voteID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
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

        public void AddVoteNum(int voteID, List<int> itemIDList)
        {
            if (voteID > 0 && itemIDList != null && itemIDList.Count > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {VoteItemAttribute.VoteNum} = {VoteItemAttribute.VoteNum} + 1 WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(itemIDList)}) AND VoteID = {voteID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void UpdateVoteNumByID(int VNum, int voteItemID)
        {
            if (voteItemID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {VoteItemAttribute.VoteNum} = {VNum} WHERE ID = {voteItemID} ";
                ExecuteNonQuery(sqlString);
            }
        }

        public void UpdateAllVoteNumByVoteID(int VNum, int voteID)
        {
            if (voteID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {VoteItemAttribute.VoteNum} = {VNum} WHERE VoteID = {voteID} ";
                ExecuteNonQuery(sqlString);
            }
        }

        public void UpdateOtherVoteNumByIDList(List<int> logIDList, int VNum, int VoteID)
        {
            if (logIDList != null && logIDList.Count > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {VoteItemAttribute.VoteNum} = {VNum} WHERE VoteID = {VoteID} AND ID NOT IN ({TranslateUtils.ToSqlInStringWithoutQuote(logIDList)}) ";
                ExecuteNonQuery(sqlString);
            }
        }

        public List<VoteItemInfo> GetVoteItemInfoList(int publishmentSystemID, int voteID)
        {
            var list = new List<VoteItemInfo>();

            string SQL_WHERE =
                $"WHERE {VoteItemAttribute.PublishmentSystemID} = {publishmentSystemID} AND {VoteItemAttribute.VoteID} = {voteID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
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
