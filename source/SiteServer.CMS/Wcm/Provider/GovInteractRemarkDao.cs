using System.Collections;
using System.Data;
using BaiRong.Core.Data;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.Model;
using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Wcm.Provider
{
    public class GovInteractRemarkDao : DataProviderBase
    {
        private const string SqlSelect = "SELECT RemarkID, PublishmentSystemID, NodeID, ContentID, RemarkType, Remark, DepartmentID, UserName, AddDate FROM wcm_GovInteractRemark WHERE RemarkID = @RemarkID";

        private const string SqlSelectAll = "SELECT RemarkID, PublishmentSystemID, NodeID, ContentID, RemarkType, Remark, DepartmentID, UserName, AddDate FROM wcm_GovInteractRemark WHERE PublishmentSystemID = @PublishmentSystemID AND ContentID = @ContentID";

        private const string ParmRemarkId = "@RemarkID";
        private const string ParmPublishmentsystemid = "@PublishmentSystemID";
        private const string ParmNodeId = "@NodeID";
        private const string ParmContentId = "@ContentID";
        private const string ParmRemarkType = "@RemarkType";
        private const string ParmRemark = "@Remark";
        private const string ParmDepartmentId = "@DepartmentID";
        private const string ParmUserName = "@UserName";
        private const string ParmAddDate = "@AddDate";

        public void Insert(GovInteractRemarkInfo remarkInfo)
        {
            var sqlString = "INSERT INTO wcm_GovInteractRemark(PublishmentSystemID, NodeID, ContentID, RemarkType, Remark, DepartmentID, UserName, AddDate) VALUES (@PublishmentSystemID, @NodeID, @ContentID, @RemarkType, @Remark, @DepartmentID, @UserName, @AddDate)";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishmentsystemid, DataType.Integer, remarkInfo.PublishmentSystemID),
                GetParameter(ParmNodeId, DataType.Integer, remarkInfo.NodeID),
                GetParameter(ParmContentId, DataType.Integer, remarkInfo.ContentID),
                GetParameter(ParmRemarkType, DataType.VarChar, 50, EGovInteractRemarkTypeUtils.GetValue(remarkInfo.RemarkType)),
                GetParameter(ParmRemark, DataType.NVarChar, 255, remarkInfo.Remark),
                GetParameter(ParmDepartmentId, DataType.Integer, remarkInfo.DepartmentID),
				GetParameter(ParmUserName, DataType.VarChar, 50, remarkInfo.UserName),
                GetParameter(ParmAddDate, DataType.DateTime, remarkInfo.AddDate)
			};

            ExecuteNonQuery(sqlString, parms);
        }

        public void Delete(int remarkId)
        {
            string sqlString = $"DELETE FROM wcm_GovInteractRemark WHERE RemarkID = {remarkId}";
            ExecuteNonQuery(sqlString);
        }

        public GovInteractRemarkInfo GetRemarkInfo(int remarkId)
        {
            GovInteractRemarkInfo remarkInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmRemarkId, DataType.Integer, remarkId)
			};

            using (var rdr = ExecuteReader(SqlSelect, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    remarkInfo = new GovInteractRemarkInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), EGovInteractRemarkTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i));
                }
                rdr.Close();
            }

            return remarkInfo;
        }

        public ArrayList GetRemarkInfoArrayList(int publishmentSystemId, int contentId)
        {
            var arraylist = new ArrayList();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentsystemid, DataType.Integer, publishmentSystemId),
                GetParameter(ParmContentId, DataType.Integer, contentId)
			};

            using (var rdr = ExecuteReader(SqlSelectAll, parms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var remarkInfo = new GovInteractRemarkInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), EGovInteractRemarkTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i));

                    arraylist.Add(remarkInfo);
                }
                rdr.Close();
            }
            return arraylist;
        }

        public IEnumerable GetDataSourceByContentId(int publishmentSystemId, int contentId)
        {
            string sqlString =
                $"SELECT RemarkID, PublishmentSystemID, NodeID, ContentID, RemarkType, Remark, DepartmentID, UserName, AddDate FROM wcm_GovInteractRemark WHERE PublishmentSystemID = {publishmentSystemId} AND ContentID = {contentId}";

            var enumerable = (IEnumerable)ExecuteReader(sqlString);
            return enumerable;
        }
    }
}
