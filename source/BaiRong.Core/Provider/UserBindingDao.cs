using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Provider
{
    public class UserBindingDao : DataProviderBase
    {
        private const string SqlDeleteByUserid = "DELETE FROM bairong_UserBinding WHERE ThirdLoginType = @ThirdLoginType AND UserID = @UserID";

        private const string SqlDeleteByThirdUserid = "DELETE FROM bairong_UserBinding WHERE ThirdLoginUserID = @ThirdLoginUserID";

        private const string SqlGetBindidByUseridType = "SELECT ThirdLoginUserID FROM bairong_UserBinding WHERE ThirdLoginType = @ThirdLoginType AND UserID = @UserID";

        private const string ParmUserid = "@UserID";
        private const string ParmThirdLoginType = "@ThirdLoginType";
        private const string ParmThirdLoginUserId = "@ThirdLoginUserID";

        public void DeleteByUserId(int userId, string thirdLoginType)
        {
            var parms = new IDataParameter[]
            {
				GetParameter(ParmUserid, EDataType.Integer, userId),
                GetParameter(ParmThirdLoginType,EDataType.VarChar,50,thirdLoginType)
			};

            ExecuteNonQuery(SqlDeleteByUserid, parms);
        }

        public void DeleteByThirdUserId(string thirdUserId)
        {
            var parms = new IDataParameter[]
            {
				GetParameter(ParmThirdLoginUserId, EDataType.NVarChar,200, thirdUserId)
			};

            ExecuteNonQuery(SqlDeleteByThirdUserid, parms);
        }

        public string GetUserBindByUserAndType(int userId, string thirdLoginType)
        {
            var parms = new IDataParameter[]
            {
                GetParameter(ParmUserid,EDataType.Integer,userId),
                GetParameter(ParmThirdLoginType,EDataType.VarChar,50,thirdLoginType)
            };

            var result = ExecuteScalar(SqlGetBindidByUseridType, parms);
            return result != null ? result.ToString() : string.Empty;
        }
    }
}