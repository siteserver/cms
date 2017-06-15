using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class MapDao : DataProviderBase
    {
        private const string TableName = "wx_Map";
          
         
        public int Insert(MapInfo mapInfo)
        {
            var mapId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(mapInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);
             
            
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        mapId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return mapId;
        }

        public void Update(MapInfo mapInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(mapInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);
        }

        public void AddPvCount(int mapId)
        {
            if (mapId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {MapAttribute.PvCount} = {MapAttribute.PvCount} + 1 WHERE ID = {mapId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int mapId)
        {
            if (mapId > 0)
            {
                var mapIdList = new List<int>();
                mapIdList.Add(mapId);
                DataProviderWx.KeywordDao.Delete(GetKeywordIdList(mapIdList));

                string sqlString = $"DELETE FROM {TableName} WHERE ID = {mapId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(List<int> mapIdList)
        {
            if (mapIdList != null  && mapIdList.Count > 0)
            {
                DataProviderWx.KeywordDao.Delete(GetKeywordIdList(mapIdList));

                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(mapIdList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetKeywordIdList(List<int> mapIdList)
        {
            var keywordIdList = new List<int>();

            string sqlString =
                $"SELECT {MapAttribute.KeywordId} FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(mapIdList)})";

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

        public MapInfo GetMapInfo(int mapId)
        {
            MapInfo mapInfo = null;

            string sqlWhere = $"WHERE ID = {mapId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    mapInfo = new MapInfo(rdr);
                }
                rdr.Close();
            }

            return mapInfo;
        }

        public List<MapInfo> GetMapInfoListByKeywordId(int publishmentSystemId, int keywordId)
        {
            var mapInfoList = new List<MapInfo>();

            string sqlWhere =
                $"WHERE {MapAttribute.PublishmentSystemId} = {publishmentSystemId} AND {MapAttribute.IsDisabled} <> '{true}'";
            if (keywordId > 0)
            {
                sqlWhere += $" AND {MapAttribute.KeywordId} = {keywordId}";
            }

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var mapInfo = new MapInfo(rdr);
                    mapInfoList.Add(mapInfo);
                }
                rdr.Close();
            }

            return mapInfoList;
        }

        public List<MapInfo> GetMapInfoList(int publishmentSystemId)
        {
            var mapInfoList = new List<MapInfo>();

            string sqlWhere = $"WHERE {MapAttribute.PublishmentSystemId} = {publishmentSystemId}";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var mapInfo = new MapInfo(rdr);
                    mapInfoList.Add(mapInfo);
                }
                rdr.Close();
            }

            return mapInfoList;
        }

        public int GetFirstIdByKeywordId(int publishmentSystemId, int keywordId)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TableName} WHERE {MapAttribute.PublishmentSystemId} = {publishmentSystemId} AND {MapAttribute.IsDisabled} <> '{true}' AND {MapAttribute.KeywordId} = {keywordId}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetTitle(int mapId)
        {
            var title = string.Empty;

            string sqlWhere = $"WHERE ID = {mapId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, MapAttribute.Title, sqlWhere, null);

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
            string whereString = $"WHERE {MapAttribute.PublishmentSystemId} = {publishmentSystemId}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }
    }
}
