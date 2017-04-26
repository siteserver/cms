using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.GovPublic;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.CMS.Wcm.Provider
{
    public class GovPublicApplyDao : DataProviderBase
	{
        public string TableName => "wcm_GovPublicApply";

	    public int Insert(GovPublicApplyInfo info)
        {
            int applyId;

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
                        applyId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return applyId;
        }

        public void Update(GovPublicApplyInfo info)
        {
            info.BeforeExecuteNonQuery();
            IDataParameter[] parms;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(info.Attributes, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);
        }

        public void UpdateState(int applyId, EGovPublicApplyState state)
        {
            string sqlString =
                $"UPDATE {TableName} SET State = '{EGovPublicApplyStateUtils.GetValue(state)}' WHERE ID = {applyId}";
            ExecuteNonQuery(sqlString);
        }

        public void UpdateDepartmentId(int applyId, int departmentId)
        {
            string sqlString = $"UPDATE {TableName} SET DepartmentID = {departmentId} WHERE ID = {applyId}";
            ExecuteNonQuery(sqlString);
        }

        public void UpdateDepartmentId(ArrayList idCollection, int departmentId)
        {
            string sqlString =
                $"UPDATE {TableName} SET DepartmentID = {departmentId} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(idCollection)})";

            ExecuteNonQuery(sqlString);
        }

        public void Delete(List<int> deleteIdArrayList)
        {
            string sqlString =
                $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(deleteIdArrayList)})";
            ExecuteNonQuery(sqlString);
        }

        public void Delete(int styleId)
        {
            string sqlString = $"DELETE FROM {TableName} WHERE StyleID ={styleId}";
            ExecuteNonQuery(sqlString);
        }

        public GovPublicApplyInfo GetApplyInfo(int publishmentSystemId, int styleId, NameValueCollection form)
        {
            var queryCode = GovPublicApplyManager.GetQueryCode();
            var departmentId = TranslateUtils.ToInt(form[GovPublicApplyAttribute.DepartmentId]);
            var departmentName = string.Empty;
            if (departmentId > 0)
            {
                departmentName = DepartmentManager.GetDepartmentName(departmentId);
            }
            var applyInfo = new GovPublicApplyInfo(0, styleId, publishmentSystemId, TranslateUtils.ToBool(form[GovPublicApplyAttribute.IsOrganization], false), form[GovPublicApplyAttribute.Title], departmentName, departmentId, DateTime.Now, queryCode, EGovPublicApplyState.New);

            foreach (var name in form.AllKeys)
            {
                if (GovPublicApplyAttribute.BasicAttributes.Contains(name.ToLower()))
                {
                    var value = form[name];
                    if (!string.IsNullOrEmpty(value))
                    {
                        applyInfo.SetExtendedAttribute(name, value);
                    }
                }
            }

            return applyInfo;
        }

        public GovPublicApplyInfo GetApplyInfo(int applyId)
        {
            GovPublicApplyInfo info = null;
            string sqlWhere = $"WHERE ID = {applyId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, sqlWhere);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    info = new GovPublicApplyInfo();
                    BaiRongDataProvider.DatabaseDao.ReadResultsToExtendedAttributes(rdr, info);
                }
                rdr.Close();
            }

            info?.AfterExecuteReader();
            return info;
        }

        public GovPublicApplyInfo GetApplyInfo(int publishmentSystemId, bool isOrganization, string queryName, string queryCode)
        {
            GovPublicApplyInfo info = null;
            var nameAttribute = GovPublicApplyAttribute.CivicName;
            if (isOrganization)
            {
                nameAttribute = GovPublicApplyAttribute.OrgName;
            }
            string sqlWhere =
                $"WHERE PublishmentSystemID = {publishmentSystemId} AND {nameAttribute} = '{queryName}' AND QueryCode = '{queryCode}'";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, sqlWhere);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    info = new GovPublicApplyInfo();
                    BaiRongDataProvider.DatabaseDao.ReadResultsToExtendedAttributes(rdr, info);
                }
                rdr.Close();
            }

            info?.AfterExecuteReader();
            return info;
        }

        public EGovPublicApplyState GetState(int applyId)
        {
            var state = EGovPublicApplyState.New;
            string sqlString = $"SELECT State FROM {TableName} WHERE ID = {applyId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    state = EGovPublicApplyStateUtils.GetEnumType(GetString(rdr, 0));
                }
                rdr.Close();
            }
            return state;
        }

        public int GetCountByStyleId(int styleId)
        {
            string sqlString = $"SELECT COUNT(*) AS TotalNum FROM {TableName} WHERE (StyleID = {styleId})";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetCountByPublishmentSystemId(int publishmentSystemId)
        {
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM {TableName} WHERE PublishmentSystemID = {publishmentSystemId}";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetCountByDepartmentId(int publishmentSystemId, int departmentId)
        {
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM {TableName} WHERE PublishmentSystemID = {publishmentSystemId} AND DepartmentID = {departmentId}";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetCountByDepartmentId(int publishmentSystemId, int departmentId, DateTime begin, DateTime end)
        {
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM {TableName} WHERE PublishmentSystemID = {publishmentSystemId} AND DepartmentID = {departmentId} AND (AddDate BETWEEN '{begin.ToShortDateString()}' AND '{end.AddDays(1).ToShortDateString()}')";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetCountByDepartmentIdAndState(int publishmentSystemId, int departmentId, EGovPublicApplyState state)
        {
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM {TableName} WHERE PublishmentSystemID = {publishmentSystemId} AND DepartmentID = {departmentId} AND State = '{EGovPublicApplyStateUtils.GetValue(state)}'";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetCountByDepartmentIdAndState(int publishmentSystemId, int departmentId, EGovPublicApplyState state, DateTime begin, DateTime end)
        {
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM {TableName} WHERE PublishmentSystemID = {publishmentSystemId} AND DepartmentID = {departmentId} AND State = '{EGovPublicApplyStateUtils.GetValue(state)}' AND (AddDate BETWEEN '{begin.ToShortDateString()}' AND '{end.AddDays(1).ToShortDateString()}')";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetSelectStringByState(int publishmentSystemId, params EGovPublicApplyState[] states)
        {
            var whereBuilder = new StringBuilder();
            whereBuilder.Append($"WHERE PublishmentSystemID = {publishmentSystemId} AND (");
            foreach (var state in states)
            {
                whereBuilder.Append($" State = '{EGovPublicApplyStateUtils.GetValue(state)}' OR");
            }
            whereBuilder.Length -= 2;
            whereBuilder.Append(") ORDER BY ID DESC");
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereBuilder.ToString());
        }

        public string GetSelectString(int publishmentSystemId)
        {
            string whereString = $"WHERE PublishmentSystemID = {publishmentSystemId}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public string GetSelectString(int publishmentSystemId, string state, string keyword)
        {
            string whereString = $"WHERE PublishmentSystemID = {publishmentSystemId}";
            if (!string.IsNullOrEmpty(state))
            {
                whereString += $" AND (State = '{state}')";
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                whereString += $" AND (Title LIKE '{keyword}' OR Content LIKE '{keyword}')";
            }

            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public string GetSortFieldName()
        {
            return "ID";
        }
	}
}
