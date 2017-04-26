using System.Collections;
using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.CMS.Wcm.Provider
{
    public class GovPublicApplyRemarkDao : DataProviderBase
    {
        private const string SqlSelect = "SELECT RemarkID, PublishmentSystemID, ApplyID, RemarkType, Remark, DepartmentID, UserName, AddDate FROM wcm_GovPublicApplyRemark WHERE RemarkID = @RemarkID";

        private const string SqlSelectAll = "SELECT RemarkID, PublishmentSystemID, ApplyID, RemarkType, Remark, DepartmentID, UserName, AddDate FROM wcm_GovPublicApplyRemark WHERE ApplyID = @ApplyID";

        private const string ParmRemarkId = "@RemarkID";
        private const string ParmPublishmentsystemid = "@PublishmentSystemID";
        private const string ParmApplyId = "@ApplyID";
        private const string ParmRemarkType = "@RemarkType";
        private const string ParmRemark = "@Remark";
        private const string ParmDepartmentId = "@DepartmentID";
        private const string ParmUserName = "@UserName";
        private const string ParmAddDate = "@AddDate";

        public void Insert(GovPublicApplyRemarkInfo remarkInfo)
        {
            var sqlString = "INSERT INTO wcm_GovPublicApplyRemark(PublishmentSystemID, ApplyID, RemarkType, Remark, DepartmentID, UserName, AddDate) VALUES (@PublishmentSystemID, @ApplyID, @RemarkType, @Remark, @DepartmentID, @UserName, @AddDate)";
            
            var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishmentsystemid, EDataType.Integer, remarkInfo.PublishmentSystemID),
                GetParameter(ParmApplyId, EDataType.Integer, remarkInfo.ApplyID),
                GetParameter(ParmRemarkType, EDataType.VarChar, 50, EGovPublicApplyRemarkTypeUtils.GetValue(remarkInfo.RemarkType)),
                GetParameter(ParmRemark, EDataType.NVarChar, 255, remarkInfo.Remark),
                GetParameter(ParmDepartmentId, EDataType.Integer, remarkInfo.DepartmentID),
				GetParameter(ParmUserName, EDataType.VarChar, 50, remarkInfo.UserName),
                GetParameter(ParmAddDate, EDataType.DateTime, remarkInfo.AddDate)
			};

            ExecuteNonQuery(sqlString, parms);
        }

        public void Delete(int remarkId)
        {
            string sqlString = $"DELETE FROM wcm_GovPublicApplyRemark WHERE RemarkID = {remarkId}";
            ExecuteNonQuery(sqlString);
        }

        public GovPublicApplyRemarkInfo GetRemarkInfo(int remarkId)
        {
            GovPublicApplyRemarkInfo remarkInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmRemarkId, EDataType.Integer, remarkId)
			};

            using (var rdr = ExecuteReader(SqlSelect, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    remarkInfo = new GovPublicApplyRemarkInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), EGovPublicApplyRemarkTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i));
                }
                rdr.Close();
            }

            return remarkInfo;
        }

        public ArrayList GetRemarkInfoArrayList(int applyId)
        {
            var arraylist = new ArrayList();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmApplyId, EDataType.Integer, applyId)
			};

            using (var rdr = ExecuteReader(SqlSelectAll, parms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var remarkInfo = new GovPublicApplyRemarkInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), EGovPublicApplyRemarkTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i));

                    arraylist.Add(remarkInfo);
                }
                rdr.Close();
            }
            return arraylist;
        }

        public IEnumerable GetDataSourceByApplyId(int applyId)
        {
            string sqlString =
                $"SELECT RemarkID, PublishmentSystemID, ApplyID, RemarkType, Remark, DepartmentID, UserName, AddDate FROM wcm_GovPublicApplyRemark WHERE ApplyID = {applyId}";

            var enumerable = (IEnumerable)ExecuteReader(sqlString);
            return enumerable;
        }
    }
}
