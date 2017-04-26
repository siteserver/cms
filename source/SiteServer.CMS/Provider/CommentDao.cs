using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Provider
{
    public class CommentDao : DataProviderBase
    {
        private const string TableName = "siteserver_Comment";

        private const string ParmId = "@ID";
        private const string ParmPublishmentSystemId = "@PublishmentSystemID";
        private const string ParmNodeId = "@NodeID";
        private const string ParmContentId = "@ContentID";
        private const string ParmGoodCount = "@GoodCount";
        private const string ParmUserName = "@UserName";
        private const string ParmIsChecked = "@IsChecked";
        private const string ParmAddDate = "@AddDate";
        private const string ParmContent = "@Content";

        public int Insert(CommentInfo commentInfo)
        {
            int commentId;

            const string sqlString = "INSERT INTO siteserver_Comment(PublishmentSystemID, NodeID, ContentID, GoodCount, UserName, IsChecked, AddDate, Content) VALUES (@PublishmentSystemID, @NodeID, @ContentID, @GoodCount, @UserName, @IsChecked, @AddDate, @Content)";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, commentInfo.PublishmentSystemId),
                GetParameter(ParmNodeId, EDataType.Integer, commentInfo.NodeId),
                GetParameter(ParmContentId, EDataType.Integer, commentInfo.ContentId),
                GetParameter(ParmGoodCount, EDataType.Integer, commentInfo.GoodCount),
                GetParameter(ParmUserName, EDataType.NVarChar, 50, commentInfo.UserName),
                GetParameter(ParmIsChecked, EDataType.VarChar, 18, commentInfo.IsChecked.ToString()),
                GetParameter(ParmAddDate, EDataType.DateTime, commentInfo.AddDate),
				GetParameter(ParmContent, EDataType.NVarChar, 500, commentInfo.Content)
			};

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        commentId = ExecuteNonQueryAndReturnId(trans, sqlString, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            if (commentInfo.IsChecked)
            {
                UpdateCommentNum(commentInfo.PublishmentSystemId, commentInfo.NodeId, commentInfo.ContentId);
            }

            return commentId;
        }

        public void Delete(int publishmentSystemId, int nodeId, int contentId, int commentId)
        {
            const string sqlString = "DELETE FROM siteserver_Comment WHERE ID = @ID";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmId, EDataType.Integer, commentId)
            };

            ExecuteNonQuery(sqlString, parms);

            UpdateCommentNum(publishmentSystemId, nodeId, contentId);
        }

        public void DeleteUnChecked(List<int> commentIdList)
        {
            var sqlString = $"DELETE FROM siteserver_Comment WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(commentIdList)})";
            ExecuteNonQuery(sqlString);
        }

        public void Check(int publishmentSystemId, List<int> commentIdList)
        {
            var sqlString = $"UPDATE siteserver_Comment SET IsChecked = '{true}' WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(commentIdList)})";
            ExecuteNonQuery(sqlString);

            sqlString =
                $"SELECT NodeID, ContentID FROM siteserver_Comment WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(commentIdList)})";

            var dic = new Dictionary<int, List<int>>();

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var nodeId = GetInt(rdr, 0);
                    var contentId = GetInt(rdr, 1);
                    if (dic.ContainsKey(nodeId))
                    {
                        if (!dic[nodeId].Contains(contentId))
                        {
                            dic[nodeId].Add(contentId);
                        }
                    }
                    else
                    {
                        dic[nodeId] = new List<int>
                        {
                            contentId
                        };
                    }
                }
                rdr.Close();
            }

            foreach (var nodeId in dic.Keys)
            {
                var contentIdList = dic[nodeId];
                foreach (var contentId in contentIdList)
                {
                    UpdateCommentNum(publishmentSystemId, nodeId, contentId);
                }
            }
        }

        private void UpdateCommentNum(int publishmentSystemId, int nodeId, int contentId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            if (publishmentSystemInfo == null || nodeId == 0 || contentId == 0) return;

            var comments = GetCountChecked(publishmentSystemId, nodeId, contentId);
            
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeId);
            BaiRongDataProvider.ContentDao.UpdateComments(tableName, contentId, comments);
        }

        public void AddGoodCount(int commentId)
        {
            string sqlString = $"UPDATE siteserver_Comment SET {SqlUtils.GetAddOne("GoodCount")} WHERE ID = {commentId}";
            ExecuteNonQuery(sqlString);
        }

        public List<CommentInfo> GetCommentInfoListChecked(int publishmentSystemId, int nodeId, int contentId, int requestCount, int requestOffset)
        {
            if (requestCount == 0) return new List<CommentInfo>();

            var commentInfoList = new List<CommentInfo>();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
                GetParameter(ParmNodeId, EDataType.Integer, nodeId),
                GetParameter(ParmContentId, EDataType.Integer, contentId),
                GetParameter(ParmIsChecked, EDataType.VarChar, 18, true.ToString())
			};

            var offsetWhereString = string.Empty;
            if (requestOffset > 0)
            {
                //offsetWhereString = $"AND ID NOT IN (SELECT TOP {requestOffset} ID FROM siteserver_Comment WHERE PublishmentSystemID = @PublishmentSystemID AND NodeID = @NodeID AND ContentID = @ContentID AND IsChecked = @IsChecked ORDER BY ID DESC)";
                offsetWhereString = $"AND ID NOT IN ({SqlUtils.GetInTopSqlString("siteserver_Comment", "ID", "WHERE PublishmentSystemID = @PublishmentSystemID AND NodeID = @NodeID AND ContentID = @ContentID AND IsChecked = @IsChecked ORDER BY ID DESC", requestOffset)})";
            }
            //var sqlString =
            //    $"SELECT TOP {requestCount} ID, PublishmentSystemID, NodeID, ContentID, GoodCount, UserName, IsChecked, AddDate, Content FROM siteserver_Comment WHERE PublishmentSystemID = @PublishmentSystemID AND NodeID = @NodeID AND ContentID = @ContentID AND IsChecked = @IsChecked {offsetWhereString} ORDER BY ID DESC";
            var sqlString = SqlUtils.GetTopSqlString("siteserver_Comment", "ID, PublishmentSystemID, NodeID, ContentID, GoodCount, UserName, IsChecked, AddDate, Content", $"WHERE PublishmentSystemID = @PublishmentSystemID AND NodeID = @NodeID AND ContentID = @ContentID AND IsChecked = @IsChecked {offsetWhereString} ORDER BY ID DESC", requestCount);

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var commentInfo = new CommentInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i));

                    commentInfoList.Add(commentInfo);
                }
                rdr.Close();
            }

            return commentInfoList;
        }

        public void GetUserNameAndContent(int commentId, out string userName, out string content)
        {
            userName = content = string.Empty;
            string sqlString =
                $"SELECT UserName, Content FROM siteserver_Comment WHERE ID = {commentId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    userName = GetString(rdr, 0);
                    content = GetString(rdr, 1);
                }
                rdr.Close();
            }
        }

        public int GetCountChecked(int publishmentSystemId, int nodeId, int contentId)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM siteserver_Comment WHERE PublishmentSystemID = {publishmentSystemId} AND NodeID = {nodeId} AND ContentID = {contentId} AND IsChecked = '{true}'";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetCountChecked(int publishmentSystemId, DateTime begin, DateTime end)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM siteserver_Comment WHERE PublishmentSystemID = {publishmentSystemId} AND (AddDate BETWEEN '{begin.ToShortDateString()}' AND '{end.ToShortDateString()}') AND IsChecked = '{true}'";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetSortFieldName()
        {
            return "ID";
        }


        public string GetSelectSqlStringWithChecked(int publishmentSystemId, int nodeId, int contentId, int startNum, int totalNum, bool isRecommend, string whereString, string orderByString)
        {
            if (!string.IsNullOrEmpty(whereString) && !StringUtils.StartsWithIgnoreCase(whereString.Trim(), "AND "))
            {
                whereString = "AND " + whereString.Trim();
            }
            if (isRecommend)
            {
                whereString += $"AND IsRecommend = '{true}'";
            }
            string sqlWhereString =
                $"WHERE PublishmentSystemID = {publishmentSystemId} AND NodeID = {nodeId} AND ContentID = {contentId} AND IsChecked = '{true}' {whereString}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, startNum, totalNum, SqlUtils.Asterisk, sqlWhereString, orderByString);
        }

        public string GetSelectedCommendByCheck(int publishmentSystemId)
        {
            string whereString =
                        $"WHERE PublishmentSystemID = {publishmentSystemId} AND IsChecked='{false}'";

            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public int GetTotalCountWithChecked(int publishmentSystemId, int nodeId, int contentId)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM siteserver_Comment WHERE PublishmentSystemID = {publishmentSystemId} AND NodeID = {nodeId} AND ContentID = {contentId} AND IsChecked = '{true}'";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public ArrayList GetContentIdArrayListByCount(int publishmentSystemId)
        {
            var list = new ArrayList();

            string sqlString = $@"
SELECT ContentID, COUNT(ContentID) AS TotalNum FROM siteserver_Comment WHERE PublishmentSystemID = {publishmentSystemId} AND ContentID > 0 GROUP BY ContentID ORDER BY TotalNum DESC
";

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
    }
}