using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class AlbumContentDao : DataProviderBase
    {
        private const string TableName = "wx_AlbumContent";
         
        private const string SqlSelectAll = "SELECT ID, PublishmentSystemID, AlbumID, ParentID, Title, ImageUrl, LargeImageUrl FROM wx_AlbumContent WHERE PublishmentSystemID = @PublishmentSystemID AND AlbumID = @AlbumID AND ParentID = 0  ORDER BY ID ASC";

        private const string SqlSelectAllByParentid = "SELECT ID, PublishmentSystemID, AlbumID, ParentID, Title, ImageUrl, LargeImageUrl FROM wx_AlbumContent WHERE PublishmentSystemID = @PublishmentSystemID AND AlbumID = @AlbumID  AND  ParentID=@ParentID AND ParentID <> 0  ORDER BY ID ASC";
         
        private const string ParmId = "@ID";
        private const string ParmPublishmentSystemId = "@PublishmentSystemID";
        private const string ParmAlbumId = "@AlbumID";
        private const string ParmParentId = "@ParentID";
        private const string ParmTitle = "@Title";
        private const string ParmImageurl = "@ImageUrl";
        private const string ParmLargeimageurl = "@LargeImageUrl";
         
        public int Insert(AlbumContentInfo albumContentInfo)
        {
            var albumId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(albumContentInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);
             
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

        public void Update(AlbumContentInfo albumContentInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(albumContentInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);
             
            ExecuteNonQuery(sqlUpdate, parms);
        }

        public void Delete(int publishmentSystemId, int albumContentId)
        {
            if (albumContentId > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE {AlbumContentAttribute.PublishmentSystemId}= {publishmentSystemId} AND {AlbumContentAttribute.Id}= {albumContentId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemId, List<int> albumContentIdList)
        {
            if (albumContentIdList != null && albumContentIdList.Count > 0)
            {
               string sqlString =
                   $"DELETE FROM {TableName} WHERE {AlbumContentAttribute.PublishmentSystemId} = {publishmentSystemId} AND ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(albumContentIdList)})";
               ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteByParentId(int publishmentSystemId, int parentId)
        {
            if (parentId > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE {AlbumContentAttribute.PublishmentSystemId} = {publishmentSystemId} AND {AlbumContentAttribute.ParentId} = {parentId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public AlbumContentInfo GetAlbumContentInfo(int albumContentId)
        {
            AlbumContentInfo albumContentInfo = null;

            string sqlWhere = $"WHERE ID = {albumContentId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    albumContentInfo = new AlbumContentInfo(rdr);
                }
                rdr.Close();
            }

            return albumContentInfo;
        }

        public List<AlbumContentInfo> GetAlbumContentInfoList(int publishmentSystemId, int albumId, int parentId)
        {
            var list = new List<AlbumContentInfo>();

            string sqlWhere =
                $"WHERE {AlbumContentAttribute.PublishmentSystemId} = {publishmentSystemId} AND {AlbumContentAttribute.AlbumId} = {albumId} AND {AlbumContentAttribute.ParentId} = {parentId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
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

        public List<int> GetAlbumContentIdList(int publishmentSystemId, int albumId, int parentId)
        {
            var list = new List<int>();
            string sqlString =
                $"SELECT ID FROM {TableName} WHERE {AlbumContentAttribute.PublishmentSystemId} = {publishmentSystemId} AND {AlbumContentAttribute.AlbumId} = {albumId} AND {AlbumContentAttribute.ParentId} = {parentId} ORDER BY ID";

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
  
        public string GetTitle(int albumContentId)
        {
            var title = string.Empty;

            string sqlWhere = $"WHERE ID = {albumContentId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, AlbumContentAttribute.Title, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    title = GetString(rdr, 0);
                }
                rdr.Close();
            }

            return title;
        }

        public int GetCount(int publishmentSystemId, int parentId)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TableName} WHERE {AlbumContentAttribute.PublishmentSystemId} = {publishmentSystemId} AND {AlbumContentAttribute.ParentId} = {parentId}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }
         
        public IEnumerable GetDataSource(int publishmentSystemId, int albumId)
        {
            
            var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
				GetParameter(ParmAlbumId,DataType.Integer,albumId)
			};

            var enumerable = (IEnumerable)ExecuteReader(SqlSelectAll, parms);
            return enumerable;
        }

        public IEnumerable GetDataSource(int publishmentSystemId, int albumId,int parentId)
        {

            var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
				GetParameter(ParmAlbumId,DataType.Integer,albumId),
                GetParameter(ParmParentId,DataType.Integer,parentId)
			};

            var enumerable = (IEnumerable)ExecuteReader(SqlSelectAllByParentid, parms);
            return enumerable;
        }

        public List<AlbumContentInfo> GetAlbumContentInfoList(int publishmentSystemId)
        {
            var list = new List<AlbumContentInfo>();

            string sqlWhere = $"WHERE {AlbumContentAttribute.PublishmentSystemId} = {publishmentSystemId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
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
