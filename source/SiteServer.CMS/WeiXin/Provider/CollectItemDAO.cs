using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class CollectItemDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_CollectItem";

        public int Insert(CollectItemInfo itemInfo)
        {
            var itemID = 0;

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        IDataParameter[] parms = null;
                        var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(itemInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);


                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        itemID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return itemID;
        }

        public void Update(CollectItemInfo itemInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(itemInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);


            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        public void Delete(int publishmentSystemID, List<int> collectItemIDList)
        {
            if (collectItemIDList != null && collectItemIDList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(collectItemIDList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteAll(int publishmentSystemID, int collectID)
        {
            if (collectID > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE {CollectItemAttribute.PublishmentSystemID} = {publishmentSystemID} AND {CollectItemAttribute.CollectID} = {collectID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public CollectItemInfo GetCollectItemInfo(int itemID)
        {
            CollectItemInfo collectItemInfo = null;

            string SQL_WHERE = $"WHERE ID = {itemID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    collectItemInfo = new CollectItemInfo(rdr);
                }
                rdr.Close();
            }

            return collectItemInfo;
        }

        public List<CollectItemInfo> GetCollectItemInfoList(int collectID)
        {
            var list = new List<CollectItemInfo>();

            string SQL_WHERE =
                $"WHERE {CollectItemAttribute.CollectID} = {collectID} AND IsChecked = 'True' order by id desc";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
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

        public void AddVoteNum(int collectID, int itemID)
        {
            if (collectID > 0 && itemID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {CollectItemAttribute.VoteNum} = {CollectItemAttribute.VoteNum} + 1 WHERE ID = {itemID} AND CollectID = {collectID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public Dictionary<string, int> GetItemIDCollectionWithRank(int collectID)
        {
            var rankWithItemIDCollection = new Dictionary<int, string>();

            string sqlString =
                $"SELECT ID, VoteNum FROM {TABLE_NAME} WHERE {CollectLogAttribute.CollectID} = {collectID} ORDER BY VoteNum DESC";
            using (var rdr = ExecuteReader(sqlString))
            {
                var rank = 1;
                while (rdr.Read())
                {
                    var itemID = rdr.GetInt32(0);
                    var voteNum = rdr.GetInt32(1);

                    if (rankWithItemIDCollection.ContainsKey(rank))
                    {
                        var itemIDCollection = rankWithItemIDCollection[rank];
                        itemIDCollection += "," + itemID;
                        rankWithItemIDCollection[rank] = itemIDCollection;
                    }
                    else
                    {
                        rankWithItemIDCollection.Add(rank, itemID.ToString());
                    }

                    rank++;
                }
                rdr.Close();
            }

            var itemIDCollectionWithRank = new Dictionary<string, int>();
            foreach (var item in rankWithItemIDCollection)
            {
                itemIDCollectionWithRank.Add(item.Value, item.Key);
            }

            return itemIDCollectionWithRank;
        }

        public string GetSelectString(int publishmentSystemID, int collectID)
        {
            string whereString =
                $"WHERE {CollectItemAttribute.PublishmentSystemID} = {publishmentSystemID} AND {CollectItemAttribute.CollectID} = {collectID}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public void Audit(int publishmentSystemID, int collectItemID)
        {
            if (collectItemID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET IsChecked='True' WHERE PublishmentSystemID = {publishmentSystemID} AND ID = {collectItemID}";
                ExecuteNonQuery(sqlString);
            }
        }

    }
}
