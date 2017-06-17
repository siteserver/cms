using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class CollectItemDao : DataProviderBase
    {
        private const string TableName = "wx_CollectItem";

        public int Insert(CollectItemInfo itemInfo)
        {
            var itemId = 0;

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        IDataParameter[] parms = null;
                        var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(itemInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

                        itemId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return itemId;
        }

        public void Update(CollectItemInfo itemInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(itemInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);


            ExecuteNonQuery(sqlUpdate, parms);
        }

        public void Delete(int publishmentSystemId, List<int> collectItemIdList)
        {
            if (collectItemIdList != null && collectItemIdList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(collectItemIdList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteAll(int publishmentSystemId, int collectId)
        {
            if (collectId > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE {CollectItemAttribute.PublishmentSystemId} = {publishmentSystemId} AND {CollectItemAttribute.CollectId} = {collectId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public CollectItemInfo GetCollectItemInfo(int itemId)
        {
            CollectItemInfo collectItemInfo = null;

            string sqlWhere = $"WHERE ID = {itemId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    collectItemInfo = new CollectItemInfo(rdr);
                }
                rdr.Close();
            }

            return collectItemInfo;
        }

        public List<CollectItemInfo> GetCollectItemInfoList(int collectId)
        {
            var list = new List<CollectItemInfo>();

            string sqlWhere =
                $"WHERE {CollectItemAttribute.CollectId} = {collectId} AND IsChecked = 'True' order by id desc";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var itemInfo = new CollectItemInfo(rdr);
                    list.Add(itemInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public void AddVoteNum(int collectId, int itemId)
        {
            if (collectId > 0 && itemId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {CollectItemAttribute.VoteNum} = {CollectItemAttribute.VoteNum} + 1 WHERE ID = {itemId} AND CollectID = {collectId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public Dictionary<string, int> GetItemIdCollectionWithRank(int collectId)
        {
            var rankWithItemIdCollection = new Dictionary<int, string>();

            string sqlString =
                $"SELECT ID, VoteNum FROM {TableName} WHERE {CollectLogAttribute.CollectId} = {collectId} ORDER BY VoteNum DESC";
            using (var rdr = ExecuteReader(sqlString))
            {
                var rank = 1;
                while (rdr.Read())
                {
                    var itemId = rdr.GetInt32(0);
                    var voteNum = rdr.GetInt32(1);

                    if (rankWithItemIdCollection.ContainsKey(rank))
                    {
                        var itemIdCollection = rankWithItemIdCollection[rank];
                        itemIdCollection += "," + itemId;
                        rankWithItemIdCollection[rank] = itemIdCollection;
                    }
                    else
                    {
                        rankWithItemIdCollection.Add(rank, itemId.ToString());
                    }

                    rank++;
                }
                rdr.Close();
            }

            var itemIdCollectionWithRank = new Dictionary<string, int>();
            foreach (var item in rankWithItemIdCollection)
            {
                itemIdCollectionWithRank.Add(item.Value, item.Key);
            }

            return itemIdCollectionWithRank;
        }

        public string GetSelectString(int publishmentSystemId, int collectId)
        {
            string whereString =
                $"WHERE {CollectItemAttribute.PublishmentSystemId} = {publishmentSystemId} AND {CollectItemAttribute.CollectId} = {collectId}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public void Audit(int publishmentSystemId, int collectItemId)
        {
            if (collectItemId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET IsChecked='True' WHERE PublishmentSystemID = {publishmentSystemId} AND ID = {collectItemId}";
                ExecuteNonQuery(sqlString);
            }
        }

    }
}
