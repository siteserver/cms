using System.Collections.Generic;
using System.Data;
using SiteServer.CMS.Data;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.CMS.DataCache;

namespace SiteServer.CMS.Provider
{
    public class ContentTagDao : DataProviderBase
    {
        public override string TableName => "siteserver_ContentTag";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(ContentTagInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(ContentTagInfo.TagName),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(ContentTagInfo.SiteId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(ContentTagInfo.UseNum),
                DataType = DataType.Integer
            }
        };

        private const string SqlInsert = "INSERT INTO siteserver_ContentTag (TagName, SiteId, UseNum) VALUES (@TagName, @SiteId, @UseNum)";
        private const string SqlUpdate = "UPDATE siteserver_ContentTag SET UseNum = @UseNum WHERE TagName = @TagName AND SiteId = @SiteId";
        private const string SqlDelete = "DELETE FROM siteserver_ContentTag WHERE TagName = @TagName AND SiteId = @SiteId";

        private const string ParmTagName = "@TagName";
        private const string ParmSiteId = "@SiteId";
        private const string ParmUseNum = "@UseNum";

        public void Insert(ContentTagInfo contentTag)
        {
            var insertParms = new IDataParameter[]
			{
				GetParameter(ParmTagName, DataType.VarChar, 255, contentTag.TagName),
				GetParameter(ParmSiteId, DataType.Integer, contentTag.SiteId),
                GetParameter(ParmUseNum, DataType.Integer, contentTag.UseNum)
			};

            ExecuteNonQuery(SqlInsert, insertParms);

            ContentTagManager.ClearCache();
        }

        public void Update(ContentTagInfo contentTag)
        {
            var updateParms = new IDataParameter[]
			{
				GetParameter(ParmUseNum, DataType.Integer, contentTag.UseNum),
				GetParameter(ParmTagName, DataType.VarChar, 255, contentTag.TagName),
				GetParameter(ParmSiteId, DataType.Integer, contentTag.SiteId)
			};

            ExecuteNonQuery(SqlUpdate, updateParms);

            ContentTagManager.ClearCache();
        }

        public void Delete(string tagName, int siteId)
        {
            var contentTagParms = new IDataParameter[]
			{
				GetParameter(ParmTagName, DataType.VarChar, 255, tagName),
				GetParameter(ParmSiteId, DataType.Integer, siteId)
			};

            ExecuteNonQuery(SqlDelete, contentTagParms);

            ContentTagManager.ClearCache();
        }

        public Dictionary<int, List<ContentTagInfo>> GetAllContentTags()
        {
            var allDict = new Dictionary<int, List<ContentTagInfo>>();

            var sqlString =
                $"SELECT Id, TagName, SiteId, UseNum FROM {TableName} ORDER BY UseNum DESC, TagName";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var tag = new ContentTagInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i));

                    allDict.TryGetValue(tag.SiteId, out var list);

                    if (list == null)
                    {
                        list = new List<ContentTagInfo>();
                    }

                    list.Add(tag);

                    allDict[tag.SiteId] = list;
                }
                rdr.Close();
            }

            return allDict;
        }
    }
}