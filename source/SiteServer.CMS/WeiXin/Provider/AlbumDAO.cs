using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class AlbumDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_Album";
         
        public int Insert(AlbumInfo albumInfo)
        {
            var albumID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(albumInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);
             
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        albumID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return albumID;
        }

        public void Update(AlbumInfo albumInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(albumInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);
             
            ExecuteNonQuery(SQL_UPDATE, parms);
        }
  
        public void AddPVCount(int albumID)
        {
            if (albumID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {AlbumAttribute.PVCount} = {AlbumAttribute.PVCount} + 1 WHERE ID = {albumID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemID, int albumID)
        {
            if (albumID > 0)
            {
                var albumIDList = new List<int>();
                albumIDList.Add(albumID);
                DataProviderWX.KeywordDAO.Delete(GetKeywordIDList(albumIDList));

                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE ID = {albumID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemID, List<int> albumIDList)
        {
            if (albumIDList != null && albumIDList.Count > 0)
            {
                DataProviderWX.KeywordDAO.Delete(GetKeywordIDList(albumIDList));

                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(albumIDList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetKeywordIDList(List<int> albumIDList)
        {
            var keywordIDList = new List<int>();

            string sqlString =
                $"SELECT {AlbumAttribute.KeywordID} FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(albumIDList)})";

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

        public AlbumInfo GetAlbumInfo(int albumID)
        {
            AlbumInfo albumInfo = null;

            string SQL_WHERE = $"WHERE ID = {albumID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    albumInfo = new AlbumInfo(rdr);
                }
                rdr.Close();
            }

            return albumInfo;
        }

        public List<AlbumInfo> GetAlbumInfoListByKeywordID(int publishmentSystemID, int keywordID)
        {
            var albumInfoList = new List<AlbumInfo>();

            string SQL_WHERE =
                $"WHERE {AlbumAttribute.PublishmentSystemID} = {publishmentSystemID} AND {AlbumAttribute.IsDisabled} <> '{true}'";
            if (keywordID > 0)
            {
                SQL_WHERE += $" AND {AlbumAttribute.KeywordID} = {keywordID}";
            }

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
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

        public int GetFirstIDByKeywordID(int publishmentSystemID, int keywordID)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TABLE_NAME} WHERE {AlbumAttribute.PublishmentSystemID} = {publishmentSystemID} AND {AlbumAttribute.IsDisabled} <> '{true}' AND {AlbumAttribute.KeywordID} = {keywordID}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetTitle(int albumID)
        {
            var title = string.Empty;

            string SQL_WHERE = $"WHERE ID = {albumID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, AlbumAttribute.Title, SQL_WHERE, null);

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
            string whereString = $"WHERE {AlbumAttribute.PublishmentSystemID} = {publishmentSystemID}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public List<AlbumInfo> GetAlbumInfoList(int publishmentSystemID)
        {
            var albumInfoList = new List<AlbumInfo>();

            string SQL_WHERE = $" AND {AlbumAttribute.PublishmentSystemID} = {publishmentSystemID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
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
