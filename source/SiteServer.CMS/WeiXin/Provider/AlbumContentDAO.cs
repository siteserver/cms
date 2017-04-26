using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class AlbumContentDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_AlbumContent";
         
        private const string SQL_SELECT_ALL = "SELECT ID, PublishmentSystemID, AlbumID, ParentID, Title, ImageUrl, LargeImageUrl FROM wx_AlbumContent WHERE PublishmentSystemID = @PublishmentSystemID AND AlbumID = @AlbumID AND ParentID = 0  ORDER BY ID ASC";

        private const string SQL_SELECT_ALL_BY_PARENTID = "SELECT ID, PublishmentSystemID, AlbumID, ParentID, Title, ImageUrl, LargeImageUrl FROM wx_AlbumContent WHERE PublishmentSystemID = @PublishmentSystemID AND AlbumID = @AlbumID  AND  ParentID=@ParentID AND ParentID <> 0  ORDER BY ID ASC";
         
        private const string PARM_ID = "@ID";
        private const string PARM_PUBLISHMENT_SYSTEM_ID = "@PublishmentSystemID";
        private const string PARM_ALBUM_ID = "@AlbumID";
        private const string PARM_PARENT_ID = "@ParentID";
        private const string PARM_TITLE = "@Title";
        private const string PARM_IMAGEURL = "@ImageUrl";
        private const string PARM_LARGEIMAGEURL = "@LargeImageUrl";
         
        public int Insert(AlbumContentInfo albumContentInfo)
        {
            var albumID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(albumContentInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);
             
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

        public void Update(AlbumContentInfo albumContentInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(albumContentInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);
             
            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        public void Delete(int publishmentSystemID, int albumContentID)
        {
            if (albumContentID > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE {AlbumContentAttribute.PublishmentSystemID}= {publishmentSystemID} AND {AlbumContentAttribute.ID}= {albumContentID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemID, List<int> albumContentIDList)
        {
            if (albumContentIDList != null && albumContentIDList.Count > 0)
            {
               string sqlString =
                   $"DELETE FROM {TABLE_NAME} WHERE {AlbumContentAttribute.PublishmentSystemID} = {publishmentSystemID} AND ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(albumContentIDList)})";
               ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteByParentID(int publishmentSystemID, int parentID)
        {
            if (parentID > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE {AlbumContentAttribute.PublishmentSystemID} = {publishmentSystemID} AND {AlbumContentAttribute.ParentID} = {parentID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public AlbumContentInfo GetAlbumContentInfo(int albumContentID)
        {
            AlbumContentInfo albumContentInfo = null;

            string SQL_WHERE = $"WHERE ID = {albumContentID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    albumContentInfo = new AlbumContentInfo(rdr);
                }
                rdr.Close();
            }

            return albumContentInfo;
        }

        public List<AlbumContentInfo> GetAlbumContentInfoList(int publishmentSystemID, int albumID, int parentID)
        {
            var list = new List<AlbumContentInfo>();

            string SQL_WHERE =
                $"WHERE {AlbumContentAttribute.PublishmentSystemID} = {publishmentSystemID} AND {AlbumContentAttribute.AlbumID} = {albumID} AND {AlbumContentAttribute.ParentID} = {parentID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var albumContentInfo = new AlbumContentInfo(rdr);
                    list.Add(albumContentInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public List<int> GetAlbumContentIDList(int publishmentSystemID, int albumID, int parentID)
        {
            var list = new List<int>();
            string sqlString =
                $"SELECT ID FROM {TABLE_NAME} WHERE {AlbumContentAttribute.PublishmentSystemID} = {publishmentSystemID} AND {AlbumContentAttribute.AlbumID} = {albumID} AND {AlbumContentAttribute.ParentID} = {parentID} ORDER BY ID";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }

            return list;
        }
  
        public string GetTitle(int albumContentID)
        {
            var title = string.Empty;

            string SQL_WHERE = $"WHERE ID = {albumContentID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, AlbumContentAttribute.Title, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    title = GetString(rdr, 0);
                }
                rdr.Close();
            }

            return title;
        }

        public int GetCount(int publishmentSystemID, int parentID)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TABLE_NAME} WHERE {AlbumContentAttribute.PublishmentSystemID} = {publishmentSystemID} AND {AlbumContentAttribute.ParentID} = {parentID}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }
         
        public IEnumerable GetDataSource(int publishmentSystemID, int albumID)
        {
            
            var parms = new IDataParameter[]
			{
                GetParameter(PARM_PUBLISHMENT_SYSTEM_ID, EDataType.Integer, publishmentSystemID),
				GetParameter(PARM_ALBUM_ID,EDataType.Integer,albumID)
			};

            var enumerable = (IEnumerable)ExecuteReader(SQL_SELECT_ALL, parms);
            return enumerable;
        }

        public IEnumerable GetDataSource(int publishmentSystemID, int albumID,int parentID)
        {

            var parms = new IDataParameter[]
			{
                GetParameter(PARM_PUBLISHMENT_SYSTEM_ID, EDataType.Integer, publishmentSystemID),
				GetParameter(PARM_ALBUM_ID,EDataType.Integer,albumID),
                GetParameter(PARM_PARENT_ID,EDataType.Integer,parentID)
			};

            var enumerable = (IEnumerable)ExecuteReader(SQL_SELECT_ALL_BY_PARENTID, parms);
            return enumerable;
        }

        public List<AlbumContentInfo> GetAlbumContentInfoList(int publishmentSystemID)
        {
            var list = new List<AlbumContentInfo>();

            string SQL_WHERE = $"WHERE {AlbumContentAttribute.PublishmentSystemID} = {publishmentSystemID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var albumContentInfo = new AlbumContentInfo(rdr);
                    list.Add(albumContentInfo);
                }
                rdr.Close();
            }

            return list;
        }
        
    }
}
