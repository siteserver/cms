using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.Model;
using SiteServer.Utils.Model;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Provider
{
    public class TagDao : DataProviderBase
	{
        public override string TableName => "bairong_Tags";

        public override List<TableColumnInfo> TableColumns => new List<TableColumnInfo>
        {
            new TableColumnInfo
            {
                ColumnName = nameof(TagInfo.TagId),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TagInfo.PublishmentSystemId),
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TagInfo.ContentIdCollection),
                DataType = DataType.VarChar,
                Length = 255
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TagInfo.Tag),
                DataType = DataType.VarChar,
                Length = 255
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TagInfo.UseNum),
                DataType = DataType.Integer
            }
        };

        private const string ParmTagId = "@TagID";
        private const string ParmPublishmentSystemId = "@PublishmentSystemID";
        private const string ParmContentIdCollection = "@ContentIDCollection";
        private const string ParmTag = "@Tag";
        private const string ParmUseNum = "@UseNum";
        
        public int Insert(TagInfo tagInfo)
        {
            const string sqlString = "INSERT INTO bairong_Tags (PublishmentSystemID, ContentIDCollection, Tag, UseNum) VALUES (@PublishmentSystemID, @ContentIDCollection, @Tag, @UseNum)";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, tagInfo.PublishmentSystemId),
				GetParameter(ParmContentIdCollection, DataType.VarChar, 255, tagInfo.ContentIdCollection),
                GetParameter(ParmTag, DataType.VarChar, 255, tagInfo.Tag),
                GetParameter(ParmUseNum, DataType.Integer, tagInfo.UseNum)
			};

            return ExecuteNonQueryAndReturnId(TableName, nameof(TagInfo.TagId), sqlString, parms);
        }

        public void Update(TagInfo tagInfo)
        {
            var sqlString = "UPDATE bairong_Tags SET ContentIDCollection = @ContentIDCollection, UseNum = @UseNum WHERE TagID = @TagID";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmContentIdCollection, DataType.VarChar, 255, tagInfo.ContentIdCollection),
                GetParameter(ParmUseNum, DataType.Integer, tagInfo.UseNum),
                GetParameter(ParmTagId, DataType.Integer, tagInfo.TagId)
			};

            ExecuteNonQuery(sqlString, parms);
        }

        public TagInfo GetTagInfo(int publishmentSystemId, string tag)
        {
            TagInfo tagInfo = null;

            var sqlString = "SELECT TagID, PublishmentSystemID, ContentIDCollection, Tag, UseNum FROM bairong_Tags WHERE PublishmentSystemID = @PublishmentSystemID AND Tag = @Tag";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
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

        public List<TagInfo> GetTagInfoList(int publishmentSystemId, int contentId)
        {
            var list = new List<TagInfo>();

            var whereString = GetWhereString(null, publishmentSystemId, contentId);
            string sqlString =
                $"SELECT TagID, PublishmentSystemID, ContentIDCollection, Tag, UseNum FROM bairong_Tags {whereString}";

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

        public string GetSqlString(int publishmentSystemId, int contentId, bool isOrderByCount, int totalNum)
        {
            var whereString = GetWhereString(null, publishmentSystemId, contentId);
            var orderString = string.Empty;
            if (isOrderByCount)
            {
                orderString = "ORDER BY UseNum DESC";
            }

            return SqlUtils.ToTopSqlString("bairong_Tags", "TagID, PublishmentSystemID, ContentIDCollection, Tag, UseNum", whereString, orderString, totalNum);
        }

        public List<TagInfo> GetTagInfoList(int publishmentSystemId, int contentId, bool isOrderByCount, int totalNum)
        {
            var list = new List<TagInfo>();

            var whereString = GetWhereString(null, publishmentSystemId, contentId);
            var orderString = string.Empty;
            if (isOrderByCount)
            {
                orderString = "ORDER BY UseNum DESC";
            }

            var sqlString = SqlUtils.ToTopSqlString("bairong_Tags", "TagID, PublishmentSystemID, ContentIDCollection, Tag, UseNum", whereString, orderString, totalNum);

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

        public List<string> GetTagListByStartString(int publishmentSystemId, string startString, int totalNum)
        {
            var sqlString = SqlUtils.GetDistinctTopSqlString("bairong_Tags", "Tag, UseNum",
                $"WHERE PublishmentSystemID = {publishmentSystemId} AND {SqlUtils.GetInStr("Tag", PageUtils.FilterSql(startString))}",
                "ORDER BY UseNum DESC", totalNum);
            return DataProvider.DatabaseDao.GetStringList(sqlString);
        }

        public List<string> GetTagList(int publishmentSystemId)
        {
            string sqlString =
                $"SELECT Tag FROM bairong_Tags WHERE PublishmentSystemID = {publishmentSystemId} ORDER BY UseNum DESC";
            return DataProvider.DatabaseDao.GetStringList(sqlString);
        }

        public void DeleteTags(int publishmentSystemId)
        {
            var whereString = GetWhereString(null, publishmentSystemId, 0);
            string sqlString = $"DELETE FROM bairong_Tags {whereString}";
            ExecuteNonQuery(sqlString);
        }

        public void DeleteTag(string tag, int publishmentSystemId)
        {
            var whereString = GetWhereString(tag, publishmentSystemId, 0);
            string sqlString = $"DELETE FROM bairong_Tags {whereString}";
            ExecuteNonQuery(sqlString);
        }

        public int GetTagCount(string tag, int publishmentSystemId)
        {
            var contentIdList = GetContentIdListByTag(tag, publishmentSystemId);
            return contentIdList.Count;
        }

        private string GetWhereString(string tag, int publishmentSystemId, int contentId)
        {
            var builder = new StringBuilder();
            builder.Append($" WHERE PublishmentSystemID = {publishmentSystemId} ");
            if (!string.IsNullOrEmpty(tag))
            {
                builder.Append($"AND Tag = '{PageUtils.FilterSql(tag)}' ");
            }
            if (contentId > 0)
            {
                builder.Append(
                    $"AND (ContentIDCollection = '{contentId}' OR ContentIDCollection LIKE '{contentId},%' OR ContentIDCollection LIKE '%,{contentId},%' OR ContentIDCollection LIKE '%,{contentId}')");
            }

            return builder.ToString();
        }

        public List<int> GetContentIdListByTag(string tag, int publishmentSystemId)
        {
            var idList = new List<int>();
            if (string.IsNullOrEmpty(tag)) return idList;

            var whereString = GetWhereString(tag, publishmentSystemId, 0);
            var sqlString = "SELECT ContentIDCollection FROM bairong_Tags" + whereString;

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

        public List<int> GetContentIdListByTagCollection(StringCollection tagCollection, int publishmentSystemId)
        {
            var contentIdList = new List<int>();
            if (tagCollection.Count > 0)
            {
                string parameterNameList;
                var parameterList = GetInParameterList(ParmTag, DataType.VarChar, 255, tagCollection, out parameterNameList);

                string sqlString =
                    $"SELECT ContentIDCollection FROM bairong_Tags WHERE Tag IN ({parameterNameList}) AND PublishmentSystemID = @PublishmentSystemID";

                var paramList = new List<IDataParameter>();
                paramList.AddRange(parameterList);
                paramList.Add(GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId));

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
