using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Provider
{
    public class ThirdLoginDao : DataProviderBase
    {
        private const string SqlUpdateThirdlogin = "UPDATE bairong_ThirdLogin SET ThirdLoginName = @ThirdLoginName, IsEnabled = @IsEnabled, Description = @Description, SettingsXML = @SettingsXML WHERE ID = @ID";

        private const string SqlDeleteThirdlogin = "DELETE FROM bairong_ThirdLogin WHERE ID = @ID";

        private const string SqlSelectThirdlogin = "SELECT ID, ThirdLoginType, ThirdLoginName, IsEnabled, Taxis, Description, SettingsXML FROM bairong_ThirdLogin WHERE ID = @ID";

        private const string SqlSelectByName = "SELECT ID,  ThirdLoginType, ThirdLoginName, IsEnabled, Taxis, Description, SettingsXML FROM bairong_ThirdLogin WHERE ThirdLoginName = @ThirdLoginName";

        private const string SqlSelectByType = "SELECT ID, ThirdLoginType, ThirdLoginName, IsEnabled,  Taxis, Description, SettingsXML FROM bairong_ThirdLogin WHERE ThirdLoginType = @ThirdLoginType";

        private const string SqlSelectAllThirdlogin = "SELECT ID, ThirdLoginType, ThirdLoginName, IsEnabled,  Taxis, Description, SettingsXML FROM bairong_ThirdLogin ORDER BY Taxis DESC";

        private const string ParmId = "@ID";
        private const string ParmType = "@ThirdLoginType";
        private const string ParmName = "@ThirdLoginName";
        private const string ParmIsEnabled = "@IsEnabled";
        private const string ParmTaxis = "@Taxis";
        private const string ParmDescription = "@Description";
        private const string ParmSettingsXml = "@SettingsXML";

        public int Insert(ThirdLoginInfo thirdLoginInfo)
        {
            int thirdLoginId;

            var sqlString = "INSERT INTO bairong_ThirdLogin (ThirdLoginType, ThirdLoginName, IsEnabled,  Taxis, Description, SettingsXML) VALUES ( @ThirdLoginType, @ThirdLoginName, @IsEnabled,@Taxis, @Description, @SettingsXML)";

            var taxis = GetMaxTaxis() + 1;

            IDataParameter[] parms = {
				GetParameter(ParmType, EDataType.VarChar, 50, EThirdLoginTypeUtils.GetValue(thirdLoginInfo.ThirdLoginType)),
                GetParameter(ParmName, EDataType.NVarChar, 50, thirdLoginInfo.ThirdLoginName),
                GetParameter(ParmIsEnabled, EDataType.VarChar, 18, thirdLoginInfo.IsEnabled.ToString()),                 
                GetParameter(ParmTaxis, EDataType.Integer, taxis),
                GetParameter(ParmDescription, EDataType.NText, thirdLoginInfo.Description),
                GetParameter(ParmSettingsXml, EDataType.NText, thirdLoginInfo.SettingsXml)
			};

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        thirdLoginId = ExecuteNonQueryAndReturnId(trans, sqlString, parms);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return thirdLoginId;
        }

        public void Update(ThirdLoginInfo thirdLoginInfo)
        {
            var parms = new IDataParameter[]
			{
                GetParameter(ParmName, EDataType.NVarChar, 50, thirdLoginInfo.ThirdLoginName),
                GetParameter(ParmIsEnabled, EDataType.VarChar, 18, thirdLoginInfo.IsEnabled.ToString()),                
                GetParameter(ParmDescription, EDataType.NText, thirdLoginInfo.Description),
                GetParameter(ParmSettingsXml, EDataType.NText, thirdLoginInfo.SettingsXml),
				GetParameter(ParmId, EDataType.Integer, thirdLoginInfo.Id)
			};

            ExecuteNonQuery(SqlUpdateThirdlogin, parms);
        }

        public void Delete(int thirdLoginId)
        {
            var deleteParms = new IDataParameter[]
			{
				GetParameter(ParmId, EDataType.Integer, thirdLoginId)
			};

            ExecuteNonQuery(SqlDeleteThirdlogin, deleteParms);
        }

        public ThirdLoginInfo GetThirdLoginInfo(int thirdLoginId)
        {
            ThirdLoginInfo thirdLoginInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmId, EDataType.Integer, thirdLoginId)
			};

            using (var rdr = ExecuteReader(SqlSelectThirdlogin, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    thirdLoginInfo = new ThirdLoginInfo(GetInt(rdr, i++), EThirdLoginTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

            return thirdLoginInfo;
        }

        public bool IsExists(string thirdLoginName)
        {
            var exists = false;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmName, EDataType.NVarChar, 50, thirdLoginName)
			};

            using (var rdr = ExecuteReader(SqlSelectByName, parms))
            {
                if (rdr.Read())
                {
                    exists = true;
                }
                rdr.Close();
            }

            return exists;
        }

        public bool IsExists(EThirdLoginType thirdLoginType)
        {
            var exists = false;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmType, EDataType.VarChar, 50, EThirdLoginTypeUtils.GetValue(thirdLoginType))
			};

            using (var rdr = ExecuteReader(SqlSelectByType, parms))
            {
                if (rdr.Read())
                {
                    exists = true;
                }
                rdr.Close();
            }

            return exists;
        }

        public IEnumerable GetDataSource()
        {
            var parms = new IDataParameter[]
			{

			};

            var enumerable = (IEnumerable)ExecuteReader(SqlSelectAllThirdlogin, parms);
            return enumerable;
        }

        public List<ThirdLoginInfo> GetThirdLoginInfoList()
        {
            var list = new List<ThirdLoginInfo>();

            var parms = new IDataParameter[]
			{
			};

            using (var rdr = ExecuteReader(SqlSelectAllThirdlogin, parms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var thirdLoginInfo = new ThirdLoginInfo(GetInt(rdr, i++), EThirdLoginTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                    list.Add(thirdLoginInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public bool UpdateTaxisToUp(int thirdLoginId)
        {
            //string sqlString =
            //    $"SELECT TOP 1 ID, Taxis FROM bairong_ThirdLogin WHERE Taxis > (SELECT Taxis FROM bairong_ThirdLogin WHERE ID = {thirdLoginId}) ORDER BY Taxis";
            string sqlString = SqlUtils.GetTopSqlString("bairong_ThirdLogin", "ID, Taxis", $"WHERE Taxis > (SELECT Taxis FROM bairong_ThirdLogin WHERE ID = {thirdLoginId}) ORDER BY Taxis", 1);

            var higherId = 0;
            var higherTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    higherId = GetInt(rdr, 0);
                    higherTaxis = GetInt(rdr, 1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(thirdLoginId);

            if (higherId != 0)
            {
                SetTaxis(thirdLoginId, higherTaxis);
                SetTaxis(higherId, selectedTaxis);
                return true;
            }
            return false;
        }

        public bool UpdateTaxisToDown(int thirdLoginId)
        {
            //string sqlString =
            //    $"SELECT TOP 1 ID, Taxis FROM bairong_ThirdLogin WHERE Taxis < (SELECT Taxis FROM bairong_ThirdLogin WHERE ID = {thirdLoginId}) ORDER BY Taxis DESC";
            string sqlString = SqlUtils.GetTopSqlString("bairong_ThirdLogin", "ID, Taxis", $"WHERE Taxis < (SELECT Taxis FROM bairong_ThirdLogin WHERE ID = {thirdLoginId}) ORDER BY Taxis DESC", 1);

            var lowerId = 0;
            var lowerTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    lowerId = GetInt(rdr, 0);
                    lowerTaxis = GetInt(rdr, 1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(thirdLoginId);

            if (lowerId != 0)
            {
                SetTaxis(thirdLoginId, lowerTaxis);
                SetTaxis(lowerId, selectedTaxis);
                return true;
            }
            return false;
        }

        private int GetMaxTaxis()
        {
            var sqlString = "SELECT MAX(Taxis) FROM bairong_ThirdLogin";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        private int GetTaxis(int thirdLoginId)
        {
            string sqlString = $"SELECT Taxis FROM bairong_ThirdLogin WHERE (ID = {thirdLoginId})";
            var taxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    taxis = GetInt(rdr, 0);
                }
                rdr.Close();
            }

            return taxis;
        }

        private void SetTaxis(int thirdLoginId, int taxis)
        {
            string sqlString = $"UPDATE bairong_ThirdLogin SET Taxis = {taxis} WHERE ID = {thirdLoginId}";
            ExecuteNonQuery(sqlString);
        }

        public void InsertUserBinding(int userId, string thirdLoginType, string thirdLoginUserId)
        {
            var sqlString = "INSERT INTO bairong_UserBinding (UserID, ThirdLoginType, ThirdLoginUserID) VALUES (@UserID, @ThirdLoginType, @ThirdLoginUserID)";

            var parms = new IDataParameter[]
			{
                GetParameter("@UserID", EDataType.Integer,userId),
				GetParameter("@ThirdLoginType", EDataType.VarChar, 50, thirdLoginType),
                GetParameter("@ThirdLoginUserID", EDataType.NVarChar, 200, thirdLoginUserId),
 
			};

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, sqlString, parms);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public int GetUserBindingCount(string thirdLoginUserId)
        {
            string sqlString = $"SELECT UserID FROM bairong_UserBinding WHERE (ThirdLoginUserID = '{thirdLoginUserId}')";
            var bindingUserId = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    bindingUserId = GetInt(rdr, 0);
                }
                rdr.Close();
            }

            return bindingUserId;
        }

        public void SetDefaultThirdLogin()
        {
            ExecuteNonQuery("DELETE FROM bairong_ThirdLogin;");
            var sbSql = new StringBuilder();
            sbSql.AppendFormat("INSERT INTO bairong_ThirdLogin (ThirdLoginType, ThirdLoginName, IsEnabled,  Taxis, Description, SettingsXML) VALUES ( '{0}', '{1}', 'False', 1, '{2}', '');", EThirdLoginTypeUtils.GetValue(EThirdLoginType.QQ), EThirdLoginTypeUtils.GetText(EThirdLoginType.QQ), EThirdLoginTypeUtils.GetDescription(EThirdLoginType.QQ));
            sbSql.AppendFormat("INSERT INTO bairong_ThirdLogin (ThirdLoginType, ThirdLoginName, IsEnabled,  Taxis, Description, SettingsXML) VALUES ( '{0}', '{1}', 'False', 2, '{2}', '');", EThirdLoginTypeUtils.GetValue(EThirdLoginType.Weibo), EThirdLoginTypeUtils.GetText(EThirdLoginType.Weibo), EThirdLoginTypeUtils.GetDescription(EThirdLoginType.Weibo));
            sbSql.AppendFormat("INSERT INTO bairong_ThirdLogin (ThirdLoginType, ThirdLoginName, IsEnabled,  Taxis, Description, SettingsXML) VALUES ( '{0}', '{1}', 'False', 3, '{2}', '');", EThirdLoginTypeUtils.GetValue(EThirdLoginType.WeixinPC), EThirdLoginTypeUtils.GetText(EThirdLoginType.WeixinPC), EThirdLoginTypeUtils.GetDescription(EThirdLoginType.WeixinPC));
            ExecuteNonQuery(sbSql.ToString());
        }
    }
}