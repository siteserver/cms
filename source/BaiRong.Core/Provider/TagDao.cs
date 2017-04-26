using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Provider
{
    public class TagDao : DataProviderBase
	{
        private const string ParmTagId = "@TagID";
        private const string ParmPublishmentSystemId = "@PublishmentSystemID";
        private const string ParmContentIdCollection = "@ContentIDCollection";
        private const string ParmTag = "@Tag";
        private const string ParmUseNum = "@UseNum";
        
        public int Insert(TagInfo tagInfo)
        {
            int tagId;

            var sqlString = "INSERT INTO bairong_Tags (PublishmentSystemID, ContentIDCollection, Tag, UseNum) VALUES (@PublishmentSystemID, @ContentIDCollection, @Tag, @UseNum)";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, tagInfo.PublishmentSystemId),
				GetParameter(ParmContentIdCollection, EDataType.NVarChar, 255, tagInfo.ContentIdCollection),
                GetParameter(ParmTag, EDataType.NVarChar, 255, tagInfo.Tag),
                GetParameter(ParmUseNum, EDataType.Integer, tagInfo.UseNum)
			};

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        tagId = ExecuteNonQueryAndReturnId(trans, sqlString, parms);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return tagId;
        }

        public void Update(TagInfo tagInfo)
        {
            var sqlString = "UPDATE bairong_Tags SET ContentIDCollection = @ContentIDCollection, UseNum = @UseNum WHERE TagID = @TagID";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmContentIdCollection, EDataType.NVarChar, 255, tagInfo.ContentIdCollection),
                GetParameter(ParmUseNum, EDataType.Integer, tagInfo.UseNum),
                GetParameter(ParmTagId, EDataType.Integer, tagInfo.TagId)
			};

            ExecuteNonQuery(sqlString, parms);
        }

        public TagInfo GetTagInfo(int publishmentSystemId, string tag)
        {
            TagInfo tagInfo = null;

            var sqlString = "SELECT TagID, PublishmentSystemID, ContentIDCollection, Tag, UseNum FROM bairong_Tags WHERE PublishmentSystemID = @PublishmentSystemID AND Tag = @Tag";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
                GetParameter(ParmTag, EDataType.NVarChar, 255, tag)
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

        public List<TagInfo> GetTagInfoList(int publishmentSystemId, int contentId, bool isOrderByCount, int totalNum)
        {
            var list = new List<TagInfo>();
            string sqlString;

            var whereString = GetWhereString(null, publishmentSystemId, contentId);
            var orderString = string.Empty;
            if (isOrderByCount)
            {
                orderString = "ORDER BY UseNum DESC";
            }

            //            if (totalNum > 0)
            //            {
            //                sqlString = $@"
            //SELECT TOP {totalNum} TagID, PublishmentSystemID, ContentIDCollection, Tag, UseNum FROM bairong_Tags {whereString} {orderString}
            //            ";
            //            }
            //            else
            //            {
            //                sqlString = $@"
            //SELECT TagID, PublishmentSystemID, ContentIDCollection, Tag, UseNum FROM bairong_Tags {whereString} {orderString}
            //            ";
            //            }
            sqlString = SqlUtils.GetTopSqlString("bairong_Tags", "TagID, PublishmentSystemID, ContentIDCollection, Tag, UseNum", whereString + " " + orderString, totalNum);

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
            //var totalString = string.Empty;
            //if (totalNum > 0)
            //{
            //    totalString = " TOP " + totalNum + " ";
            //}

            //string sqlString =
            //    $"SELECT DISTINCT {totalString} Tag, UseNum FROM bairong_Tags WHERE PublishmentSystemID = {publishmentSystemId} AND CHARINDEX('{PageUtils.FilterSql(startString)}',Tag) > 0  ORDER BY UseNum DESC";

            var sqlString = SqlUtils.GetDistinctTopSqlString("bairong_Tags", "Tag, UseNum", $"WHERE PublishmentSystemID = {publishmentSystemId} AND {SqlUtils.GetInStr("Tag", PageUtils.FilterSql(startString))} ORDER BY UseNum DESC", totalNum);
            return BaiRongDataProvider.DatabaseDao.GetStringList(sqlString);
        }

        public List<string> GetTagList(int publishmentSystemId)
        {
            string sqlString =
                $"SELECT Tag FROM bairong_Tags WHERE PublishmentSystemID = {publishmentSystemId} ORDER BY UseNum DESC";
            return BaiRongDataProvider.DatabaseDao.GetStringList(sqlString);
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
            var contentIdList = new List<int>();
            if (!string.IsNullOrEmpty(tag))
            {
                var whereString = GetWhereString(tag, publishmentSystemId, 0);
                var sqlString = "SELECT ContentIDCollection FROM bairong_Tags" + whereString;

                using (var rdr = ExecuteReader(sqlString))
                {
                    if (rdr.Read())
                    {
                        var contentIdCollection = GetString(rdr, 0);
                        contentIdList = TranslateUtils.StringCollectionToIntList(contentIdCollection);
                    }
                    rdr.Close();
                }
            }
            return contentIdList;
        }

        public List<int> GetContentIdListByTagCollection(StringCollection tagCollection, int publishmentSystemId)
        {
            var contentIdList = new List<int>();
            if (tagCollection.Count > 0)
            {
                string parameterNameList;
                var parameterList = GetInParameterList(ParmTag, EDataType.NVarChar, 255, tagCollection, out parameterNameList);

                string sqlString =
                    $"SELECT ContentIDCollection FROM bairong_Tags WHERE Tag IN ({parameterNameList}) AND PublishmentSystemID = @PublishmentSystemID";

                var paramList = new List<IDataParameter>();
                paramList.AddRange(parameterList);
                paramList.Add(GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId));

                using (var rdr = ExecuteReader(sqlString, paramList.ToArray()))
                {
                    while (rdr.Read())
                    {
                        var contentIdCollection = GetString(rdr, 0);
                        var list = TranslateUtils.StringCollectionToIntList(contentIdCollection);
                        foreach (var contentId in list)
                        {
                            if (!contentIdList.Contains(contentId))
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
