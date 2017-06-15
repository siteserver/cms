using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class AlbumDao : DataProviderBase
    {
        private const string TableName = "wx_Album";
         
        public int Insert(AlbumInfo albumInfo)
        {
            var albumId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(albumInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);
             
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        albumId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return albumId;
        }

        public void Update(AlbumInfo albumInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(albumInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);
             
            ExecuteNonQuery(sqlUpdate, parms);
        }
  
        public void AddPvCount(int albumId)
        {
            if (albumId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {AlbumAttribute.PvCount} = {AlbumAttribute.PvCount} + 1 WHERE ID = {albumId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemId, int albumId)
        {
            if (albumId > 0)
            {
                var albumIdList = new List<int>();
                albumIdList.Add(albumId);
                DataProviderWx.KeywordDao.Delete(GetKeywordIdList(albumIdList));

                string sqlString = $"DELETE FROM {TableName} WHERE ID = {albumId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemId, List<int> albumIdList)
        {
            if (albumIdList != null && albumIdList.Count > 0)
            {
                DataProviderWx.KeywordDao.Delete(GetKeywordIdList(albumIdList));

                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(albumIdList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetKeywordIdList(List<int> albumIdList)
        {
            var keywordIdList = new List<int>();

            string sqlString =
                $"SELECT {AlbumAttribute.KeywordId} FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(albumIdList)})";

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

        public AlbumInfo GetAlbumInfo(int albumId)
        {
            AlbumInfo albumInfo = null;

            string sqlWhere = $"WHERE ID = {albumId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    albumInfo = new AlbumInfo(rdr);
                }
                rdr.Close();
            }

            return albumInfo;
        }

        public List<AlbumInfo> GetAlbumInfoListByKeywordId(int publishmentSystemId, int keywordId)
        {
            var albumInfoList = new List<AlbumInfo>();

            string sqlWhere =
                $"WHERE {AlbumAttribute.PublishmentSystemId} = {publishmentSystemId} AND {AlbumAttribute.IsDisabled} <> '{true}'";
            if (keywordId > 0)
            {
                sqlWhere += $" AND {AlbumAttribute.KeywordId} = {keywordId}";
            }

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var albumInfo = new AlbumInfo(rdr);
                    albumInfoList.Add(albumInfo);
                }
                rdr.Close();
            }

            return albumInfoList;
        }

        public int GetFirstIdByKeywordId(int publishmentSystemId, int keywordId)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TableName} WHERE {AlbumAttribute.PublishmentSystemId} = {publishmentSystemId} AND {AlbumAttribute.IsDisabled} <> '{true}' AND {AlbumAttribute.KeywordId} = {keywordId}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetTitle(int albumId)
        {
            var title = string.Empty;

            string sqlWhere = $"WHERE ID = {albumId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, AlbumAttribute.Title, sqlWhere, null);

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
            string whereString = $"WHERE {AlbumAttribute.PublishmentSystemId} = {publishmentSystemId}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public List<AlbumInfo> GetAlbumInfoList(int publishmentSystemId)
        {
            var albumInfoList = new List<AlbumInfo>();

            string sqlWhere = $" AND {AlbumAttribute.PublishmentSystemId} = {publishmentSystemId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var albumInfo = new AlbumInfo(rdr);
                    albumInfoList.Add(albumInfo);
                }
                rdr.Close();
            }

            return albumInfoList;
        }
        
    }
}
