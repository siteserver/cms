using System.Data;
using BaiRong.Core.Data;
using SiteServer.CMS.Wcm.Model;
using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Wcm.Provider
{
    public class GovInteractReplyDao : DataProviderBase
    {
        private const string SqlSelect = "SELECT ReplyID, PublishmentSystemID, NodeID, ContentID, Reply, FileUrl, DepartmentID, UserName, AddDate FROM wcm_GovInteractReply WHERE ReplyID = @ReplyID";

        private const string SqlSelectByContentId = "SELECT ReplyID, PublishmentSystemID, NodeID, ContentID, Reply, FileUrl, DepartmentID, UserName, AddDate FROM wcm_GovInteractReply WHERE PublishmentSystemID = @PublishmentSystemID AND ContentID = @ContentID";

        private const string ParmReplyId = "@ReplyID";
        private const string ParmPublishmentsystemid = "@PublishmentSystemID";
        private const string ParmNodeId = "@NodeID";
        private const string ParmContentId = "@ContentID";
        private const string ParmReply = "@Reply";
        private const string ParmFileUrl = "@FileUrl";
        private const string ParmDepartmentId = "@DepartmentID";
        private const string ParmUserName = "@UserName";
        private const string ParmAddDate = "@AddDate";

        public void Insert(GovInteractReplyInfo replyInfo)
        {
            var sqlString = "INSERT INTO wcm_GovInteractReply(PublishmentSystemID, NodeID, ContentID, Reply, FileUrl, DepartmentID, UserName, AddDate) VALUES (@PublishmentSystemID, @NodeID, @ContentID, @Reply, @FileUrl, @DepartmentID, @UserName, @AddDate)";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishmentsystemid, DataType.Integer, replyInfo.PublishmentSystemID),
                GetParameter(ParmNodeId, DataType.Integer, replyInfo.NodeID),
                GetParameter(ParmContentId, DataType.Integer, replyInfo.ContentID),
                GetParameter(ParmReply, DataType.NText, replyInfo.Reply),
                GetParameter(ParmFileUrl, DataType.NVarChar, 255, replyInfo.FileUrl),
                GetParameter(ParmDepartmentId, DataType.Integer, replyInfo.DepartmentID),
				GetParameter(ParmUserName, DataType.VarChar, 50, replyInfo.UserName),
                GetParameter(ParmAddDate, DataType.DateTime, replyInfo.AddDate)
			};

            ExecuteNonQuery(sqlString, parms);
        }

        public void Delete(int replyId)
        {
            string sqlString = $"DELETE FROM wcm_GovInteractReply WHERE ReplyID = {replyId}";
            ExecuteNonQuery(sqlString);
        }

        public void DeleteByContentId(int publishmentSystemId, int contentId)
        {
            string sqlString =
                $"DELETE FROM wcm_GovInteractReply WHERE PublishmentSystemID = {publishmentSystemId} AND ContentID = {contentId}";
            ExecuteNonQuery(sqlString);
        }

        public GovInteractReplyInfo GetReplyInfo(int replayId)
        {
            GovInteractReplyInfo replyInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmReplyId, DataType.Integer, replayId)
			};

            using (var rdr = ExecuteReader(SqlSelect, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    replyInfo = new GovInteractReplyInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i));
                }
                rdr.Close();
            }

            return replyInfo;
        }

        public GovInteractReplyInfo GetReplyInfoByContentId(int publishmentSystemId, int contentId)
        {
            GovInteractReplyInfo replyInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentsystemid, DataType.Integer, publishmentSystemId),
                GetParameter(ParmContentId, DataType.Integer, contentId)
			};

            using (var rdr = ExecuteReader(SqlSelectByContentId, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    replyInfo = new GovInteractReplyInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i));
                }
                rdr.Close();
            }

            return replyInfo;
        }
    }
}
