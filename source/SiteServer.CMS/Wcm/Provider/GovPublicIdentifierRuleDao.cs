using System;
using System.Collections;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.CMS.Wcm.Provider
{
    public class GovPublicIdentifierRuleDao : DataProviderBase
	{
        private const string SqlUpdate = "UPDATE wcm_GovPublicIdentifierRule SET RuleName = @RuleName, IdentifierType = @IdentifierType, MinLength = @MinLength, Suffix = @Suffix, FormatString = @FormatString, AttributeName = @AttributeName, Sequence = @Sequence, SettingsXML = @SettingsXML WHERE RuleID = @RuleID";

        private const string SqlDelete = "DELETE FROM wcm_GovPublicIdentifierRule WHERE RuleID = @RuleID";

        private const string SqlSelect = "SELECT RuleID, RuleName, PublishmentSystemID, IdentifierType, MinLength, Suffix, FormatString, AttributeName, Sequence, Taxis, SettingsXML FROM wcm_GovPublicIdentifierRule WHERE RuleID = @RuleID";

        private const string SqlSelectAll = "SELECT RuleID, RuleName, PublishmentSystemID, IdentifierType, MinLength, Suffix, FormatString, AttributeName, Sequence, Taxis, SettingsXML FROM wcm_GovPublicIdentifierRule WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY Taxis";

        private const string ParmRuleId = "@RuleID";
        private const string ParmRuleName = "@RuleName";
        private const string ParmPublishmentsystemid = "@PublishmentSystemID";
        private const string ParmIdentifierType = "@IdentifierType";
        private const string ParmMinLength = "@MinLength";
        private const string ParmSuffix = "@Suffix";
        private const string ParmFormatString = "@FormatString";
        private const string ParmAttributeName = "@AttributeName";
        private const string ParmSequence = "@Sequence";
        private const string ParmTaxis = "@Taxis";
        private const string ParmSettingsXml = "@SettingsXML";

        public void Insert(GovPublicIdentifierRuleInfo ruleInfo)
        {
            var taxis = GetMaxTaxis(ruleInfo.PublishmentSystemID) + 1;

            var sqlInsert = "INSERT INTO wcm_GovPublicIdentifierRule (RuleName, PublishmentSystemID, IdentifierType, MinLength, Suffix, FormatString, AttributeName, Sequence, Taxis, SettingsXML) VALUES (@RuleName, @PublishmentSystemID, @IdentifierType, @MinLength, @Suffix, @FormatString, @AttributeName, @Sequence, @Taxis, @SettingsXML)";

			var parms = new IDataParameter[]
			{
				GetParameter(ParmRuleName, EDataType.NVarChar, 255, ruleInfo.RuleName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, ruleInfo.PublishmentSystemID),
                GetParameter(ParmIdentifierType, EDataType.VarChar, 50, EGovPublicIdentifierTypeUtils.GetValue(ruleInfo.IdentifierType)),
                GetParameter(ParmMinLength, EDataType.Integer, ruleInfo.MinLength),
                GetParameter(ParmSuffix, EDataType.VarChar, 50, ruleInfo.Suffix),
                GetParameter(ParmFormatString, EDataType.VarChar, 50, ruleInfo.FormatString),
                GetParameter(ParmAttributeName, EDataType.VarChar, 50, ruleInfo.AttributeName),
                GetParameter(ParmSequence, EDataType.Integer, ruleInfo.Sequence),
                GetParameter(ParmTaxis, EDataType.Integer, taxis),
                GetParameter(ParmSettingsXml, EDataType.NText, ruleInfo.Additional.ToString())
			};

            ExecuteNonQuery(sqlInsert, parms);
		}

        public void Update(GovPublicIdentifierRuleInfo ruleInfo) 
		{
			var parms = new IDataParameter[]
			{
                GetParameter(ParmRuleName, EDataType.NVarChar, 255, ruleInfo.RuleName),
                GetParameter(ParmIdentifierType, EDataType.VarChar, 50, EGovPublicIdentifierTypeUtils.GetValue(ruleInfo.IdentifierType)),
                GetParameter(ParmMinLength, EDataType.Integer, ruleInfo.MinLength),
                GetParameter(ParmSuffix, EDataType.VarChar, 50, ruleInfo.Suffix),
                GetParameter(ParmFormatString, EDataType.VarChar, 50, ruleInfo.FormatString),
                GetParameter(ParmAttributeName, EDataType.VarChar, 50, ruleInfo.AttributeName),
                GetParameter(ParmSequence, EDataType.Integer, ruleInfo.Sequence),
                GetParameter(ParmSettingsXml, EDataType.NText, ruleInfo.Additional.ToString()),
                GetParameter(ParmRuleId, EDataType.Integer, ruleInfo.RuleID)
			};

            ExecuteNonQuery(SqlUpdate, parms);
		}

		public void Delete(int ruleId)
		{
            var parms = new IDataParameter[]
			{
				GetParameter(ParmRuleId, EDataType.Integer, ruleId)
			};

            ExecuteNonQuery(SqlDelete, parms);
		}

        public int GetCount(int publishmentSystemId)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM wcm_GovPublicIdentifierRule WHERE PublishmentSystemID = {publishmentSystemId}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public GovPublicIdentifierRuleInfo GetIdentifierRuleInfo(int ruleId)
		{
            GovPublicIdentifierRuleInfo ruleInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmRuleId, EDataType.Integer, ruleId)
			};

            using (var rdr = ExecuteReader(SqlSelect, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    ruleInfo = new GovPublicIdentifierRuleInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), EGovPublicIdentifierTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

            return ruleInfo;
		}

        public ArrayList GetRuleInfoArrayList(int publishmentSystemId)
        {
            var arraylist = new ArrayList();

            var selectParms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(SqlSelectAll, selectParms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var ruleInfo = new GovPublicIdentifierRuleInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), EGovPublicIdentifierTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i));
                    arraylist.Add(ruleInfo);
                }
                rdr.Close();
            }

            return arraylist;
        }

        public bool UpdateTaxisToUp(int ruleId, int publishmentSystemId)
        {
            //string sqlString =
            //    $"SELECT TOP 1 RuleID, Taxis FROM wcm_GovPublicIdentifierRule WHERE ((Taxis > (SELECT Taxis FROM wcm_GovPublicIdentifierRule WHERE RuleID = {ruleId})) AND PublishmentSystemID ={publishmentSystemId}) ORDER BY Taxis";
            var sqlString = SqlUtils.GetTopSqlString("wcm_GovPublicIdentifierRule", "RuleID, Taxis", $"WHERE ((Taxis > (SELECT Taxis FROM wcm_GovPublicIdentifierRule WHERE RuleID = {ruleId})) AND PublishmentSystemID ={publishmentSystemId}) ORDER BY Taxis", 1);

            var higherRuleId = 0;
            var higherTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    higherRuleId = GetInt(rdr, 0);
                    higherTaxis = GetInt(rdr, 1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(ruleId);

            if (higherRuleId > 0)
            {
                SetTaxis(ruleId, higherTaxis);
                SetTaxis(higherRuleId, selectedTaxis);
                return true;
            }
            return false;
        }

        public bool UpdateTaxisToDown(int ruleId, int publishmentSystemId)
        {
            //string sqlString =
            //    $"SELECT TOP 1 RuleID, Taxis FROM wcm_GovPublicIdentifierRule WHERE ((Taxis < (SELECT Taxis FROM wcm_GovPublicIdentifierRule WHERE RuleID = {ruleId})) AND PublishmentSystemID = {publishmentSystemId}) ORDER BY Taxis DESC";
            var sqlString = SqlUtils.GetTopSqlString("wcm_GovPublicIdentifierRule", "RuleID, Taxis", $"WHERE ((Taxis < (SELECT Taxis FROM wcm_GovPublicIdentifierRule WHERE RuleID = {ruleId})) AND PublishmentSystemID = {publishmentSystemId}) ORDER BY Taxis DESC", 1);

            var lowerRuleId = 0;
            var lowerTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    lowerRuleId = GetInt(rdr, 0);
                    lowerTaxis = GetInt(rdr, 1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(ruleId);

            if (lowerRuleId > 0)
            {
                SetTaxis(ruleId, lowerTaxis);
                SetTaxis(lowerRuleId, selectedTaxis);
                return true;
            }
            return false;
        }

        private int GetMaxTaxis(int publishmentSystemId)
        {
            string sqlString =
                $"SELECT MAX(Taxis) FROM wcm_GovPublicIdentifierRule WHERE PublishmentSystemID = {publishmentSystemId}";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        private int GetTaxis(int ruleId)
        {
            string sqlString = $"SELECT Taxis FROM wcm_GovPublicIdentifierRule WHERE RuleID = {ruleId}";
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

        private void SetTaxis(int ruleId, int taxis)
        {
            string sqlString = $"UPDATE wcm_GovPublicIdentifierRule SET Taxis = {taxis} WHERE RuleID = {ruleId}";
            ExecuteNonQuery(sqlString);
        }
	}
}