using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class MapDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_Map";
          
         
        public int Insert(MapInfo mapInfo)
        {
            var mapID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(mapInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);
             
            
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        mapID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return mapID;
        }

        public void Update(MapInfo mapInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(mapInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        public void AddPVCount(int mapID)
        {
            if (mapID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {MapAttribute.PVCount} = {MapAttribute.PVCount} + 1 WHERE ID = {mapID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int mapID)
        {
            if (mapID > 0)
            {
                var mapIDList = new List<int>();
                mapIDList.Add(mapID);
                DataProviderWX.KeywordDAO.Delete(GetKeywordIDList(mapIDList));

                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE ID = {mapID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(List<int> mapIDList)
        {
            if (mapIDList != null  && mapIDList.Count > 0)
            {
                DataProviderWX.KeywordDAO.Delete(GetKeywordIDList(mapIDList));

                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(mapIDList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetKeywordIDList(List<int> mapIDList)
        {
            var keywordIDList = new List<int>();

            string sqlString =
                $"SELECT {MapAttribute.KeywordID} FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(mapIDList)})";

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

        public MapInfo GetMapInfo(int mapID)
        {
            MapInfo mapInfo = null;

            string SQL_WHERE = $"WHERE ID = {mapID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    mapInfo = new MapInfo(rdr);
                }
                rdr.Close();
            }

            return mapInfo;
        }

        public List<MapInfo> GetMapInfoListByKeywordID(int publishmentSystemID, int keywordID)
        {
            var mapInfoList = new List<MapInfo>();

            string SQL_WHERE =
                $"WHERE {MapAttribute.PublishmentSystemID} = {publishmentSystemID} AND {MapAttribute.IsDisabled} <> '{true}'";
            if (keywordID > 0)
            {
                SQL_WHERE += $" AND {MapAttribute.KeywordID} = {keywordID}";
            }

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
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

        public List<MapInfo> GetMapInfoList(int publishmentSystemID)
        {
            var mapInfoList = new List<MapInfo>();

            string SQL_WHERE = $"WHERE {MapAttribute.PublishmentSystemID} = {publishmentSystemID}";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
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

        public int GetFirstIDByKeywordID(int publishmentSystemID, int keywordID)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TABLE_NAME} WHERE {MapAttribute.PublishmentSystemID} = {publishmentSystemID} AND {MapAttribute.IsDisabled} <> '{true}' AND {MapAttribute.KeywordID} = {keywordID}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetTitle(int mapID)
        {
            var title = string.Empty;

            string SQL_WHERE = $"WHERE ID = {mapID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, MapAttribute.Title, SQL_WHERE, null);

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
            string whereString = $"WHERE {MapAttribute.PublishmentSystemID} = {publishmentSystemID}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }
    }
}
