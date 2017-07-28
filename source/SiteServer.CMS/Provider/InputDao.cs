using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Provider
{
    public class InputDao : DataProviderBase
    {
        private const string SqlUpdateInput = "UPDATE siteserver_Input SET InputName = @InputName, IsChecked = @IsChecked, IsReply = @IsReply, SettingsXML = @SettingsXML WHERE InputID = @InputID";

        private const string SqlDeleteInput = "DELETE FROM siteserver_Input WHERE InputID = @InputID";

        private const string SqlSelectInputByWhere = "SELECT InputID, InputName, PublishmentSystemID, AddDate, IsChecked, IsReply, Taxis, SettingsXML FROM siteserver_Input WHERE InputName = @InputName AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectInputIdByWhere = "SELECT InputID FROM siteserver_Input WHERE InputName = @InputName AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectInputByInputId = "SELECT InputID, InputName, PublishmentSystemID, AddDate, IsChecked, IsReply, Taxis, SettingsXML FROM siteserver_Input WHERE InputID = @InputID";

        private const string SqlSelectAllInput = "SELECT InputID, InputName, PublishmentSystemID, AddDate, IsChecked, IsReply, Taxis, SettingsXML FROM siteserver_Input WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY Taxis DESC, AddDate DESC";

        private const string SqlSelectInputId = "SELECT InputID FROM siteserver_Input WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY Taxis DESC, AddDate DESC";

        private const string SqlSelectInputName = "SELECT InputName FROM siteserver_Input WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY Taxis DESC, AddDate DESC";

        private const string SqlSelectAll = "SELECT InputID, InputName, PublishmentSystemID, AddDate, IsChecked, IsReply, Taxis, SettingsXML FROM siteserver_Input";

        private const string ParmInputId = "@InputID";
        private const string ParmInputName = "@InputName";
        private const string ParmPublishmentsystemid = "@PublishmentSystemID";
        private const string ParmAddDate = "@AddDate";
        private const string ParmIsChecked = "@IsChecked";
        private const string ParmIsReply = "@IsReply";
        private const string ParmTaxis = "@Taxis";
        private const string ParmSettingsXml = "@SettingsXML";

        public int Insert(InputInfo inputInfo)
        {
            int inputId;

            var sqlString = "INSERT INTO siteserver_Input (InputName, PublishmentSystemID, AddDate, IsChecked, IsReply, Taxis, SettingsXML) VALUES (@InputName, @PublishmentSystemID, @AddDate, @IsChecked, @IsReply, @Taxis, @SettingsXML)";

            var taxis = GetMaxTaxis(inputInfo.PublishmentSystemId) + 1;
            var insertParms = new IDataParameter[]
            {
                GetParameter(ParmInputName, EDataType.NVarChar, 50, inputInfo.InputName),
                GetParameter(ParmPublishmentsystemid, EDataType.Integer, inputInfo.PublishmentSystemId),
                GetParameter(ParmAddDate, EDataType.DateTime, inputInfo.AddDate),
                GetParameter(ParmIsChecked, EDataType.VarChar, 18, inputInfo.IsChecked.ToString()),
                GetParameter(ParmIsReply, EDataType.VarChar, 18, inputInfo.IsReply.ToString()),
                GetParameter(ParmTaxis, EDataType.Integer, taxis),
                GetParameter(ParmSettingsXml, EDataType.NText, inputInfo.Additional.ToString())
            };

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        inputId = ExecuteNonQueryAndReturnId(trans, sqlString, insertParms);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return inputId;
        }

        public void Update(InputInfo inputInfo)
        {
            var updateParms = new IDataParameter[]
            {
                GetParameter(ParmInputName, EDataType.NVarChar, 50, inputInfo.InputName),
                GetParameter(ParmIsChecked, EDataType.VarChar, 18, inputInfo.IsChecked.ToString()),
                GetParameter(ParmIsReply, EDataType.VarChar, 18, inputInfo.IsReply.ToString()),
                GetParameter(ParmSettingsXml, EDataType.NText, inputInfo.Additional.ToString()),
                GetParameter(ParmInputId, EDataType.Integer, inputInfo.InputId)
            };

            ExecuteNonQuery(SqlUpdateInput, updateParms);
        }

        public void Delete(int inputId)
        {
            var deleteParms = new IDataParameter[]
            {
                GetParameter(ParmInputId, EDataType.Integer, inputId)
            };

            ExecuteNonQuery(SqlDeleteInput, deleteParms);
        }

        public InputInfo GetInputInfo(string inputName, int publishmentSystemId)
        {
            InputInfo inputInfo = null;

            var parms = new IDataParameter[]
            {
                GetParameter(ParmInputName, EDataType.NVarChar, 50, inputName),
                GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
            };

            using (var rdr = ExecuteReader(SqlSelectInputByWhere, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    inputInfo = new InputInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetDateTime(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

            return inputInfo;
        }

        public InputInfo GetInputInfoAsPossible(int inputId, int publishmentSystemId)
        {
            var inputInfo = GetInputInfo(inputId) ?? GetLastAddInputInfo(publishmentSystemId);
            return inputInfo;
        }

        public int GetInputIdAsPossible(string inputName, int publishmentSystemId)
        {
            var inputId = 0;

            if (!string.IsNullOrEmpty(inputName))
            {
                var selectParms = new IDataParameter[]
                {
                    GetParameter(ParmInputName, EDataType.NVarChar, 50, inputName),
                    GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
                };

                using (var rdr = ExecuteReader(SqlSelectInputIdByWhere, selectParms))
                {
                    if (rdr.Read())
                    {
                        inputId = GetInt(rdr, 0);
                    }
                }
            }

            if (inputId == 0)
            {
                var inputInfo = GetLastAddInputInfo(publishmentSystemId);
                if (inputInfo != null)
                {
                    inputId = inputInfo.InputId;
                }
            }

            return inputId;
        }

        public InputInfo GetInputInfo(int inputId)
        {
            InputInfo inputInfo = null;

            var parms = new IDataParameter[]
            {
                GetParameter(ParmInputId, EDataType.Integer, inputId)
            };

            using (var rdr = ExecuteReader(SqlSelectInputByInputId, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    inputInfo = new InputInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetDateTime(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

            return inputInfo;
        }

        public InputInfo GetLastAddInputInfo(int publishmentSystemId)
        {
            InputInfo inputInfo = null;

            //var sqlString = "SELECT TOP 1 InputID, InputName, PublishmentSystemID, AddDate, IsChecked, IsReply, Taxis, SettingsXML FROM siteserver_Input WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY Taxis DESC, AddDate DESC";
            var sqlString = SqlUtils.GetTopSqlString("siteserver_Input", "InputID, InputName, PublishmentSystemID, AddDate, IsChecked, IsReply, Taxis, SettingsXML", "WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY Taxis DESC, AddDate DESC", 1);

            var parms = new IDataParameter[]
            {
                GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    inputInfo = new InputInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetDateTime(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

            return inputInfo;
        }

        public bool IsExists(string inputName, int publishmentSystemId)
        {
            var exists = false;

            var selectParms = new IDataParameter[]
            {
                GetParameter(ParmInputName, EDataType.NVarChar, 50, inputName),
                GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
            };

            using (var rdr = ExecuteReader(SqlSelectInputByWhere, selectParms))
            {
                if (rdr.Read())
                {
                    exists = true;
                }
                rdr.Close();
            }

            return exists;
        }

        public IEnumerable GetDataSource(int publishmentSystemId)
        {
            var selectParms = new IDataParameter[]
            {
                GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
            };
            var enumerable = (IEnumerable)ExecuteReader(SqlSelectAllInput, selectParms);
            return enumerable;
        }

        public ArrayList GetInputIdArrayList(int publishmentSystemId)
        {
            var arraylist = new ArrayList();

            var selectParms = new IDataParameter[]
            {
                GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
            };

            using (var rdr = ExecuteReader(SqlSelectInputId, selectParms))
            {
                while (rdr.Read())
                {
                    arraylist.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }

            return arraylist;
        }

        public ArrayList GetInputNameArrayList(int publishmentSystemId)
        {
            var arraylist = new ArrayList();

            var selectParms = new IDataParameter[]
            {
                GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
            };

            using (var rdr = ExecuteReader(SqlSelectInputName, selectParms))
            {
                while (rdr.Read())
                {
                    arraylist.Add(GetString(rdr, 0));
                }
                rdr.Close();
            }

            return arraylist;
        }

        public string GetImportInputName(string inputName, int publishmentSystemId)
        {
            string importInputName;
            if (inputName.IndexOf("_", StringComparison.Ordinal) != -1)
            {
                var inputNameCount = 0;
                var lastInputName = inputName.Substring(inputName.LastIndexOf("_", StringComparison.Ordinal) + 1);
                var firstInputName = inputName.Substring(0, inputName.Length - lastInputName.Length);
                try
                {
                    inputNameCount = int.Parse(lastInputName);
                }
                catch
                {
                    // ignored
                }
                inputNameCount++;
                importInputName = firstInputName + inputNameCount;
            }
            else
            {
                importInputName = inputName + "_1";
            }

            var inputInfo = GetInputInfo(inputName, publishmentSystemId);
            if (inputInfo != null)
            {
                importInputName = GetImportInputName(importInputName, publishmentSystemId);
            }

            return importInputName;
        }

        public bool UpdateTaxisToUp(int publishmentSystemId, int inputId)
        {
            //string sqlString =
            //    $"SELECT TOP 1 InputID, Taxis FROM siteserver_Input WHERE ((Taxis > (SELECT Taxis FROM siteserver_Input WHERE InputID = {inputId})) AND PublishmentSystemID ={publishmentSystemId}) ORDER BY Taxis";
            var sqlString = SqlUtils.GetTopSqlString("siteserver_Input", "InputID, Taxis", $"WHERE ((Taxis > (SELECT Taxis FROM siteserver_Input WHERE InputID = {inputId})) AND PublishmentSystemID ={publishmentSystemId}) ORDER BY Taxis", 1);

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

            var selectedTaxis = GetTaxis(inputId);

            if (higherId != 0)
            {
                SetTaxis(inputId, higherTaxis);
                SetTaxis(higherId, selectedTaxis);
                return true;
            }
            return false;
        }

        public bool UpdateTaxisToDown(int publishmentSystemId, int inputId)
        {
            //string sqlString =
            //    $"SELECT TOP 1 InputID, Taxis FROM siteserver_Input WHERE ((Taxis < (SELECT Taxis FROM siteserver_Input WHERE (InputID = {inputId}))) AND PublishmentSystemID = {publishmentSystemId}) ORDER BY Taxis DESC";
            var sqlString = SqlUtils.GetTopSqlString("siteserver_Input", "InputID, Taxis", $"WHERE ((Taxis < (SELECT Taxis FROM siteserver_Input WHERE (InputID = {inputId}))) AND PublishmentSystemID = {publishmentSystemId}) ORDER BY Taxis DESC", 1);

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

            var selectedTaxis = GetTaxis(inputId);

            if (lowerId != 0)
            {
                SetTaxis(inputId, lowerTaxis);
                SetTaxis(lowerId, selectedTaxis);
                return true;
            }
            return false;
        }

        private int GetMaxTaxis(int publishmentSystemId)
        {
            string sqlString =
                $"SELECT MAX(Taxis) FROM siteserver_Input WHERE PublishmentSystemID = {publishmentSystemId}";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        private int GetTaxis(int inputId)
        {
            string sqlString = $"SELECT Taxis FROM siteserver_Input WHERE (InputID = {inputId})";
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

        private void SetTaxis(int inputId, int taxis)
        {
            string sqlString = $"UPDATE siteserver_Input SET Taxis = {taxis} WHERE InputID = {inputId}";
            ExecuteNonQuery(sqlString);
        }

        public int GetCount()
        {
            string sqlString = "SELECT COUNT(*) AS TotalNum FROM siteserver_Input";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<InputInfo> GetKeywordInfoList()
        {
            var list = new List<InputInfo>();
            string sql = SqlSelectAll;
            using (var rdr = ExecuteReader(sql))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var inputInfo = new InputInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetDateTime(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), GetString(rdr, i));
                    list.Add(inputInfo);
                }
                rdr.Close();
            }
            return list;
        }

        public string GetSelectCommand()
        {
            return SqlSelectAll;
        }

        public IEnumerable GetDataSource(int publishmentSystemId, string inputName, string dateFrom, string dateTo)
        {

            var dateString = string.Empty;
            if (!string.IsNullOrEmpty(dateFrom))
            {
                dateString = $" AND AddDate >= '{dateFrom}' ";
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                dateTo = DateUtils.GetDateString(TranslateUtils.ToDateTime(dateTo).AddDays(1));
                dateString += $" AND AddDate <= '{dateTo}' ";
            }

            var whereStr = new StringBuilder();
            whereStr.AppendFormat(" where PublishmentSystemID={0}", publishmentSystemId);
            whereStr.Append(dateString);

            if (!string.IsNullOrEmpty(inputName))
            {
                whereStr.AppendFormat(" AND (InputName LIKE '%{0}%')  ", inputName);
            }

            whereStr.Append(" ORDER BY Taxis DESC");

            var enumerable = (IEnumerable)ExecuteReader(SqlSelectAll + whereStr);
            return enumerable;
        }

    }
}