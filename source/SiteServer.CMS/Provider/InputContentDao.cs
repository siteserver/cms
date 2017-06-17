using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Provider
{
    public class InputContentDao : DataProviderBase
    {
        public string TableName => "siteserver_InputContent";

        public int Insert(InputContentInfo info)
        {
            int contentId;

            info.Taxis = GetMaxTaxis(info.InputId) + 1;
            info.BeforeExecuteNonQuery();
            IDataParameter[] parms;
            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(info.Attributes, TableName, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        contentId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return contentId;
        }

        public void Update(InputContentInfo info)
        {
            info.BeforeExecuteNonQuery();
            IDataParameter[] parms;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(info.Attributes, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);
        }

        public void UpdateIsChecked(List<int> contentIdList)
        {
            string sqlString =
                $"UPDATE {TableName} SET IsChecked = '{true}' WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";

            ExecuteNonQuery(sqlString);
        }

        public bool UpdateTaxisToUp(int inputId, int contentId)
        {
            //string sqlString =
            //    $"SELECT TOP 1 ID, Taxis FROM {TableName} WHERE ((Taxis > (SELECT Taxis FROM {TableName} WHERE ID = {contentId})) AND InputID ={inputId}) ORDER BY Taxis";
            string sqlString = SqlUtils.GetTopSqlString(TableName, "ID, Taxis", $"WHERE ((Taxis > (SELECT Taxis FROM {TableName} WHERE ID = {contentId})) AND InputID ={inputId}) ORDER BY Taxis", 1);

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

            var selectedTaxis = GetTaxis(contentId);

            if (higherId != 0)
            {
                SetTaxis(contentId, higherTaxis);
                SetTaxis(higherId, selectedTaxis);
                return true;
            }
            return false;
        }

        public bool UpdateTaxisToDown(int inputId, int contentId)
        {
            //string sqlString =
            //    $"SELECT TOP 1 ID, Taxis FROM {TableName} WHERE ((Taxis < (SELECT Taxis FROM {TableName} WHERE (ID = {contentId}))) AND InputID = {inputId}) ORDER BY Taxis DESC";
            string sqlString = SqlUtils.GetTopSqlString(TableName, "ID, Taxis", $"WHERE ((Taxis < (SELECT Taxis FROM {TableName} WHERE (ID = {contentId}))) AND InputID = {inputId}) ORDER BY Taxis DESC", 1);

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

            var selectedTaxis = GetTaxis(contentId);

            if (lowerId != 0)
            {
                SetTaxis(contentId, lowerTaxis);
                SetTaxis(lowerId, selectedTaxis);
                return true;
            }
            return false;
        }

        public void Delete(List<int> deleteIdList)
        {
            string sqlString =
                $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(deleteIdList)})";
            ExecuteNonQuery(sqlString);
        }

        public void Check(List<int> contentIdList)
        {
            string sqlString =
                $"UPDATE {TableName} SET IsChecked = '{true}' WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
            ExecuteNonQuery(sqlString);
        }

        public void Delete(int inputId)
        {
            string sqlString = $"DELETE FROM {TableName} WHERE InputID ={inputId}";
            ExecuteNonQuery(sqlString);
        }

        public InputContentInfo GetContentInfo(int contentId)
        {
            InputContentInfo info = null;
            string sqlWhere = $"WHERE ID = {contentId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, sqlWhere);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    info = new InputContentInfo();
                    BaiRongDataProvider.DatabaseDao.ReadResultsToExtendedAttributes(rdr, info);
                }
                rdr.Close();
            }

            info?.AfterExecuteReader();
            return info;
        }

        public int GetCountChecked(int inputId)
        {
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM {TableName} WHERE (InputID = {inputId} AND {InputContentAttribute.IsChecked} = '{true}')";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetCountUnChecked(int inputId)
        {
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM {TableName} WHERE (InputID = {inputId} AND {InputContentAttribute.IsChecked} = '{false}')";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public DataSet GetDataSetNotChecked(int inputId)
        {
            string whereString = $"WHERE (InputID = {inputId} AND IsChecked = '{false}') ORDER BY Taxis DESC";
            return GetDataSetByWhereString(whereString);
        }

        public DataSet GetDataSetWithChecked(int inputId)
        {
            return GetDataSetWithChecked(inputId, ETaxisTypeUtils.GetInputContentOrderByString(ETaxisType.OrderByTaxisDesc), "");
        }

        private DataSet GetDataSetByWhereString(string whereString)
        {
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
            return ExecuteDataset(sqlSelect);
        }

        private DataSet GetDataSetWithChecked(int inputId, string orderByString, string whereString)
        {
            string where =
                $"WHERE (InputID = {inputId} AND IsChecked = '{true}' {whereString}) {orderByString}";
            return GetDataSetByWhereString(where);
        }

        public IEnumerable GetStlDataSourceChecked(int inputId, int totalNum, string orderByString, string whereString)
        {
            string sqlWhereString = $"WHERE (InputID = {inputId} AND IsChecked = '{true}' {whereString})";
            return GetDataSourceByContentNumAndWhereString(totalNum, sqlWhereString, orderByString);
        }

        private IEnumerable GetDataSourceByContentNumAndWhereString(int totalNum, string whereString, string orderByString)
        {
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, totalNum, SqlUtils.Asterisk, whereString, orderByString);
            return (IEnumerable)ExecuteReader(sqlSelect);
        }

        public List<int> GetContentIdListWithChecked(int inputId)
        {
            var list = new List<int>();
            var taxisString = ETaxisTypeUtils.GetInputContentOrderByString(ETaxisType.OrderByTaxisDesc);
            string sqlString =
                $"SELECT ID FROM {TableName} WHERE (InputID = {inputId} AND IsChecked = '{true}') {taxisString}";
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

        public List<int> GetContentIdListWithChecked(int inputId, List<string> searchFields, string keyword)
        {
            var contentIdList = new List<int>();
            var taxisString = ETaxisTypeUtils.GetInputContentOrderByString(ETaxisType.OrderByTaxisDesc);
            var whereStringBuilder = new StringBuilder();
            foreach (var field in searchFields)
            {
                if (!string.IsNullOrEmpty(field))
                {
                    whereStringBuilder.Append($" {field} LIKE '%{PageUtils.FilterSql(keyword)}%' OR");
                }
            }
            if (whereStringBuilder.Length > 0)
            {
                whereStringBuilder.Remove(whereStringBuilder.Length - 3, 3);
            }

            string sqlString =
                $"SELECT ID FROM {TableName} WHERE (InputID = {inputId} AND IsChecked = '{true}' AND ({whereStringBuilder})) {taxisString}";
            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    contentIdList.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }
            return contentIdList;
        }

        public List<int> GetContentIdListByUserName(string userName)
        {
            var contentIdList = new List<int>();

            string sqlString = $"SELECT ID FROM {TableName} WHERE UserName = @UserName ORDER BY AddDate DESC, ID DESC";

            var selectParms = new IDataParameter[]
			{
				GetParameter("@UserName", EDataType.NVarChar, 255,userName)
			};
            using (var rdr = ExecuteReader(sqlString, selectParms))
            {
                while (rdr.Read())
                {
                    contentIdList.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }
            return contentIdList;
        }

        public string GetValue(int contentId, string attributeName)
        {
            string sqlString = $"SELECT [{attributeName}] FROM [{TableName}] WHERE ([ID] = {contentId})";
            return BaiRongDataProvider.DatabaseDao.GetString(sqlString);
        }

        private int GetTaxis(int contentId)
        {
            string sqlString = $"SELECT Taxis FROM {TableName} WHERE (ID = {contentId})";
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

        private void SetTaxis(int contentId, int taxis)
        {
            string sqlString = $"UPDATE {TableName} SET Taxis = {taxis} WHERE  ID = {contentId}";
            ExecuteNonQuery(sqlString);
        }

        private int GetMaxTaxis(int inputId)
        {
            string sqlString = $"SELECT MAX(Taxis) FROM {TableName} WHERE InputID = {inputId}";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetSelectStringOfContentId(int inputId, string whereString)
        {
            var orderByString = ETaxisTypeUtils.GetInputContentOrderByString(ETaxisType.OrderByTaxisDesc);
            string where = $"WHERE (InputID = {inputId} {whereString}) {orderByString}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, "ID, Taxis", where);
        }

        public string GetSelectSqlStringWithChecked(int publishmentSystemId, int inputId, bool isReplyExists, bool isReply, int startNum, int totalNum, string whereString, string orderByString, LowerNameValueCollection others)
        {
            if (!string.IsNullOrEmpty(whereString) && !StringUtils.StartsWithIgnoreCase(whereString.Trim(), "AND "))
            {
                whereString = "AND " + whereString.Trim();
            }
            string sqlWhereString = $"WHERE InputID = {inputId} AND IsChecked = '{true}' {whereString}";
            if (isReplyExists)
            {
                if (isReply)
                {
                    sqlWhereString += " AND " + SqlUtils.GetNotNullAndEmpty("Reply");
                }
                else
                {
                    sqlWhereString += " AND " + SqlUtils.GetNullOrEmpty("Reply");
                }
            }
            if (others != null && others.Count > 0)
            {
                var relatedIdentities = RelatedIdentities.GetRelatedIdentities(ETableStyle.InputContent, publishmentSystemId, inputId);
                var styleInfoList = TableStyleManager.GetTableStyleInfoList(ETableStyle.InputContent, TableName, relatedIdentities);
                foreach (var tableStyleInfo in styleInfoList)
                {
                    if (!string.IsNullOrEmpty(others.Get(tableStyleInfo.AttributeName)))
                    {
                        sqlWhereString +=
                            $" AND ({InputContentAttribute.SettingsXml} LIKE '%{tableStyleInfo.AttributeName}={others.Get(tableStyleInfo.AttributeName)}%')";
                    }
                }
            }

            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, startNum, totalNum, SqlUtils.Asterisk, sqlWhereString, orderByString);
        }

        public string GetSortFieldName()
        {
            return "Taxis";
        }
    }
}
