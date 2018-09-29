using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Provider
{
    public class TagDao : DataProviderBase
	{
        public override string TableName => "siteserver_Tag";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(TagInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(TagInfo.SiteId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(TagInfo.ContentIdCollection),
                DataType = DataType.VarChar,
                DataLength = 2000
            },
            new TableColumn
            {
                AttributeName = nameof(TagInfo.Tag),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(TagInfo.UseNum),
                DataType = DataType.Integer
            }
        };

        private const string ParmId = "@Id";
        private const string ParmSiteId = "@SiteId";
        private const string ParmContentIdCollection = "@ContentIdCollection";
        private const string ParmTag = "@Tag";
        private const string ParmUseNum = "@UseNum";
        
        public int Insert(TagInfo tagInfo)
        {
            const string sqlString = "INSERT INTO siteserver_Tag (SiteId, ContentIdCollection, Tag, UseNum) VALUES (@SiteId, @ContentIdCollection, @Tag, @UseNum)";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmSiteId, DataType.Integer, tagInfo.SiteId),
				GetParameter(ParmContentIdCollection, DataType.VarChar, 2000, tagInfo.ContentIdCollection),
                GetParameter(ParmTag, DataType.VarChar, 255, tagInfo.Tag),
                GetParameter(ParmUseNum, DataType.Integer, tagInfo.UseNum)
			};

            return ExecuteNonQueryAndReturnId(TableName, nameof(TagInfo.Id), sqlString, parms);
        }

        public void Update(TagInfo tagInfo)
        {
            var sqlString = "UPDATE siteserver_Tag SET ContentIdCollection = @ContentIdCollection, UseNum = @UseNum WHERE Id = @Id";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmContentIdCollection, DataType.VarChar, 2000, tagInfo.ContentIdCollection),
                GetParameter(ParmUseNum, DataType.Integer, tagInfo.UseNum),
                GetParameter(ParmId, DataType.Integer, tagInfo.Id)
			};

            ExecuteNonQuery(sqlString, parms);
        }

        public TagInfo GetTagInfo(int siteId, string tag)
        {
            TagInfo tagInfo = null;

            var sqlString = "SELECT Id, SiteId, ContentIdCollection, Tag, UseNum FROM siteserver_Tag WHERE SiteId = @SiteId AND Tag = @Tag";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmSiteId, DataType.Integer, siteId),
                GetParameter(ParmTag, DataType.VarChar, 255, tag)
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    tagInfo = new TagInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i));
                }
                rdr.Close();
            }
            return tagInfo;
        }

        public List<TagInfo> GetTagInfoList(int siteId, int contentId)
        {
            var list = new List<TagInfo>();

            var whereString = GetWhereString(null, siteId, contentId);
            string sqlString =
                $"SELECT Id, SiteId, ContentIdCollection, Tag, UseNum FROM siteserver_Tag {whereString}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var tagInfo = new TagInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i));
                    list.Add(tagInfo);
                }
                rdr.Close();
            }
            return list;
        }

        public string GetSqlString(int siteId, int contentId, bool isOrderByCount, int totalNum)
        {
            var whereString = GetWhereString(null, siteId, contentId);
            var orderString = string.Empty;
            if (isOrderByCount)
            {
                orderString = "ORDER BY UseNum DESC";
            }

            return SqlUtils.ToTopSqlString("siteserver_Tag", "Id, SiteId, ContentIdCollection, Tag, UseNum", whereString, orderString, totalNum);
        }

        public List<TagInfo> GetTagInfoList(int siteId, int contentId, bool isOrderByCount, int totalNum)
        {
            var list = new List<TagInfo>();

            var whereString = GetWhereString(null, siteId, contentId);
            var orderString = string.Empty;
            if (isOrderByCount)
            {
                orderString = "ORDER BY UseNum DESC";
            }

            var sqlString = SqlUtils.ToTopSqlString("siteserver_Tag", "Id, SiteId, ContentIdCollection, Tag, UseNum", whereString, orderString, totalNum);

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var tagInfo = new TagInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i));
                    list.Add(tagInfo);
                }
                rdr.Close();
            }
            return list;
        }

        public List<string> GetTagListByStartString(int siteId, string startString, int totalNum)
        {
            var sqlString = SqlUtils.GetDistinctTopSqlString("siteserver_Tag", "Tag, UseNum",
                $"WHERE SiteId = {siteId} AND {SqlUtils.GetInStr("Tag", AttackUtils.FilterSql(startString))}",
                "ORDER BY UseNum DESC", totalNum);
            return DataProvider.DatabaseDao.GetStringList(sqlString);
        }

        public List<string> GetTagList(int siteId)
        {
            string sqlString =
                $"SELECT Tag FROM siteserver_Tag WHERE SiteId = {siteId} ORDER BY UseNum DESC";
            return DataProvider.DatabaseDao.GetStringList(sqlString);
        }

        public void DeleteTags(int siteId)
        {
            var whereString = GetWhereString(null, siteId, 0);
            string sqlString = $"DELETE FROM siteserver_Tag {whereString}";
            ExecuteNonQuery(sqlString);
        }

        public void DeleteTag(string tag, int siteId)
        {
            var whereString = GetWhereString(tag, siteId, 0);
            string sqlString = $"DELETE FROM siteserver_Tag {whereString}";
            ExecuteNonQuery(sqlString);
        }

        public int GetTagCount(string tag, int siteId)
        {
            var contentIdList = GetContentIdListByTag(tag, siteId);
            return contentIdList.Count;
        }

        private string GetWhereString(string tag, int siteId, int contentId)
        {
            var builder = new StringBuilder();
            builder.Append($" WHERE SiteId = {siteId} ");
            if (!string.IsNullOrEmpty(tag))
            {
                builder.Append($"AND Tag = '{AttackUtils.FilterSql(tag)}' ");
            }
            if (contentId > 0)
            {
                builder.Append(
                    $"AND (ContentIdCollection = '{contentId}' OR ContentIdCollection LIKE '{contentId},%' OR ContentIdCollection LIKE '%,{contentId},%' OR ContentIdCollection LIKE '%,{contentId}')");
            }

            return builder.ToString();
        }

        public List<int> GetContentIdListByTag(string tag, int siteId)
        {
            var idList = new List<int>();
            if (string.IsNullOrEmpty(tag)) return idList;

            var whereString = GetWhereString(tag, siteId, 0);
            var sqlString = "SELECT ContentIdCollection FROM siteserver_Tag" + whereString;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    var contentIdCollection = GetString(rdr, 0);
                    var contentIdList = TranslateUtils.StringCollectionToIntList(contentIdCollection);
                    foreach (var contentId in contentIdList)
                    {
                        if (contentId > 0 && !idList.Contains(contentId))
                        {
                            idList.Add(contentId);
                        }
                    }
                }
                rdr.Close();
            }
            return idList;
        }

        public List<int> GetContentIdListByTagCollection(StringCollection tagCollection, int siteId)
        {
            var contentIdList = new List<int>();
            if (tagCollection.Count > 0)
            {
                string parameterNameList;
                var parameterList = GetInParameterList(ParmTag, DataType.VarChar, 255, tagCollection, out parameterNameList);

                string sqlString =
                    $"SELECT ContentIdCollection FROM siteserver_Tag WHERE Tag IN ({parameterNameList}) AND SiteId = @SiteId";

                var paramList = new List<IDataParameter>();
                paramList.AddRange(parameterList);
                paramList.Add(GetParameter(ParmSiteId, DataType.Integer, siteId));

                using (var rdr = ExecuteReader(sqlString, paramList.ToArray()))
                {
                    while (rdr.Read())
                    {
                        var contentIdCollection = GetString(rdr, 0);
                        var list = TranslateUtils.StringCollectionToIntList(contentIdCollection);
                        foreach (var contentId in list)
                        {
                            if (contentId > 0 && !contentIdList.Contains(contentId))
                            {
                                contentIdList.Add(contentId);
                            }
                        }
                    }
                    rdr.Close();
                }
            }
            return contentIdList;
        }
	}
}
