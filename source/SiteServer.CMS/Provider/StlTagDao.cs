using System.Collections;
using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Provider
{
    public class StlTagDao : DataProviderBase
    {
        private const string SqlSelectStlTag = "SELECT TagName, PublishmentSystemID, TagDescription, TagContent FROM siteserver_StlTag WHERE TagName = @TagName AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectAllStlTag = "SELECT TagName, PublishmentSystemID, TagDescription, TagContent FROM siteserver_StlTag WHERE PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectStlTagNames = "SELECT TagName FROM siteserver_StlTag WHERE PublishmentSystemID = @PublishmentSystemID";

        private const string SqlInsertStlTag = "INSERT INTO siteserver_StlTag (TagName, PublishmentSystemID, TagDescription, TagContent) VALUES (@TagName, @PublishmentSystemID, @TagDescription, @TagContent)";

        private const string SqlUpdateStlTag = "UPDATE siteserver_StlTag SET TagDescription = @TagDescription, TagContent = @TagContent WHERE TagName = @TagName AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlDeleteStlTag = "DELETE FROM siteserver_StlTag WHERE TagName = @TagName AND PublishmentSystemID = @PublishmentSystemID";

        private const string ParmTagName = "@TagName";
        private const string ParmPublishmentSystemId = "@PublishmentSystemID";
        private const string ParmTagDescription = "@TagDescription";
        private const string ParmTagContent = "@TagContent";

        public void Insert(StlTagInfo info)
        {
            var insertParms = new IDataParameter[]
			{
                GetParameter(ParmTagName, EDataType.NVarChar, 50, info.TagName),
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, info.PublishmentSystemID),
				GetParameter(ParmTagDescription, EDataType.NVarChar, 255, info.TagDescription),
				GetParameter(ParmTagContent, EDataType.NText, info.TagContent)
			};

            ExecuteNonQuery(SqlInsertStlTag, insertParms);
        }

        public void Update(StlTagInfo info)
        {
            var updateParms = new IDataParameter[]
			{
                GetParameter(ParmTagDescription, EDataType.NVarChar, 255, info.TagDescription),
				GetParameter(ParmTagContent, EDataType.NText, info.TagContent),
				GetParameter(ParmTagName, EDataType.NVarChar, 50, info.TagName),
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, info.PublishmentSystemID)
			};

            ExecuteNonQuery(SqlUpdateStlTag, updateParms);
        }

        public void Delete(int publishmentSystemId, string tagName)
        {

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTagName, EDataType.NVarChar, 50, tagName),
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

            ExecuteNonQuery(SqlDeleteStlTag, parms);
        }

        public StlTagInfo GetStlTagInfo(int publishmentSystemId, string tagName)
        {
            StlTagInfo info = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTagName, EDataType.NVarChar, 50, tagName),
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(SqlSelectStlTag, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    info = new StlTagInfo(GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

            return info;
        }

        public ArrayList GetStlTagInfoArrayListByPublishmentSystemId(int publishmentSystemId)
        {
            var arraylist = new ArrayList();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(SqlSelectAllStlTag, parms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var info = new StlTagInfo(GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                    arraylist.Add(info);
                }
                rdr.Close();
            }
            return arraylist;
        }

        public ArrayList GetStlTagNameArrayList(int publishmentSystemId)
        {
            var list = new ArrayList();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(SqlSelectStlTagNames, parms))
            {
                while (rdr.Read())
                {
                    list.Add(GetString(rdr, 0));
                }
                rdr.Close();
            }

            return list;
        }


        public IEnumerable GetDataSource(int publishmentSystemId)
        {
            var sqlString = "SELECT TagName, PublishmentSystemID, TagDescription, TagContent FROM siteserver_StlTag WHERE PublishmentSystemID = 0 ORDER BY TagName";
            if (publishmentSystemId != 0)
            {
                sqlString =
                    $"SELECT TagName, PublishmentSystemID, TagDescription, TagContent FROM siteserver_StlTag WHERE PublishmentSystemID = 0 OR PublishmentSystemID = {publishmentSystemId} ORDER BY PublishmentSystemID, TagName";
            }
            var enumerable = (IEnumerable)ExecuteReader(sqlString);
            return enumerable;
        }


        public bool IsExactExists(int publishmentSystemId, string tagName)
        {
            var exists = false;

            var sqlString = "SELECT TagName FROM siteserver_StlTag WHERE TagName = @TagName AND PublishmentSystemID = @PublishmentSystemID";

            var selectParms = new IDataParameter[]
			{
				GetParameter(ParmTagName, EDataType.NVarChar, 50, tagName),
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(sqlString, selectParms))
            {
                if (rdr.Read())
                {
                    exists = true;
                }
                rdr.Close();
            }

            return exists;
        }

        public bool IsExists(int publishmentSystemId, string tagName)
        {
            var exists = false;

            var sqlString = "SELECT TagName FROM siteserver_StlTag WHERE TagName = @TagName AND PublishmentSystemID = 0";
            if (publishmentSystemId != 0)
            {
                sqlString = "SELECT TagName FROM siteserver_StlTag WHERE TagName = @TagName AND (PublishmentSystemID = 0 OR PublishmentSystemID = @PublishmentSystemID)";
            }
            var selectParms = new IDataParameter[]
			{
				GetParameter(ParmTagName, EDataType.NVarChar, 50, tagName),
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};
            using (var rdr = ExecuteReader(sqlString, selectParms))
            {
                if (rdr.Read())
                {
                    exists = true;
                }
                rdr.Close();
            }

            return exists;
        }
    }
}
