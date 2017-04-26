using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Provider
{
    public class ResumeContentDao : DataProviderBase
	{
        public string TableName => "siteserver_ResumeContent";

	    public int Insert(ResumeContentInfo info)
		{
			int contentId;

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

        public void Update(ResumeContentInfo info)
		{
            info.BeforeExecuteNonQuery();
            IDataParameter[] parms;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(info.Attributes, TableName, out parms);
            ExecuteNonQuery(sqlUpdate, parms);
		}

        public void SetIsView(List<int> idCollection, bool isView)
        {
            string sqlString =
                $"UPDATE {TableName} SET IsView = '{isView}' WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(idCollection)})";

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

        public ResumeContentInfo GetContentInfo(int publishmentSystemId, NameValueCollection form)
        {
            var contentInfo = new ResumeContentInfo(0, publishmentSystemId, TranslateUtils.ToInt(form[ResumeContentAttribute.JobContentId]), string.Empty, DateTime.Now);

            foreach (var name in form.AllKeys)
            {
                if (ResumeContentAttribute.BasicAttributes.Contains(name.ToLower()))
                {
                    var value = form[name];
                    if (!string.IsNullOrEmpty(value))
                    {
                        contentInfo.SetExtendedAttribute(name, value);
                    }
                }
            }

            var count = TranslateUtils.ToInt(form[ResumeContentAttribute.ExpCount]);
            contentInfo.SetExtendedAttribute(ResumeContentAttribute.ExpCount, count.ToString());
            for (var index = 1; index <= count; index++)
            {
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.ExpFromYear, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.ExpFromMonth, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.ExpToYear, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.ExpToMonth, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.ExpEmployerName, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.ExpDepartment, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.ExpEmployerPhone, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.ExpWorkPlace, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.ExpPositionTitle, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.ExpIndustry, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.ExpSummary, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.ExpScore, index);
            }

            count = TranslateUtils.ToInt(form[ResumeContentAttribute.ProCount]);
            contentInfo.SetExtendedAttribute(ResumeContentAttribute.ProCount, count.ToString());
            for (var index = 1; index <= count; index++)
            {
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.ProFromYear, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.ProFromMonth, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.ProToYear, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.ProToMonth, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.ProProjectName, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.ProSummary, index);
            }

            count = TranslateUtils.ToInt(form[ResumeContentAttribute.EduCount]);
            contentInfo.SetExtendedAttribute(ResumeContentAttribute.EduCount, count.ToString());
            for (var index = 1; index <= count; index++)
            {
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.EduFromYear, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.EduFromMonth, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.EduToYear, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.EduToMonth, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.EduSchoolName, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.EduEducation, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.EduProfession, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.EduSummary, index);
            }

            count = TranslateUtils.ToInt(form[ResumeContentAttribute.TraCount]);
            contentInfo.SetExtendedAttribute(ResumeContentAttribute.TraCount, count.ToString());
            for (var index = 1; index <= count; index++)
            {
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.TraFromYear, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.TraFromMonth, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.TraToYear, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.TraToMonth, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.TraTrainerName, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.TraTrainerAddress, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.TraLesson, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.TraCentification, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.TraSummary, index);
            }

            count = TranslateUtils.ToInt(form[ResumeContentAttribute.LanCount]);
            contentInfo.SetExtendedAttribute(ResumeContentAttribute.LanCount, count.ToString());
            for (var index = 1; index <= count; index++)
            {
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.LanLanguage, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.LanLevel, index);
            }

            count = TranslateUtils.ToInt(form[ResumeContentAttribute.SkiCount]);
            contentInfo.SetExtendedAttribute(ResumeContentAttribute.SkiCount, count.ToString());
            for (var index = 1; index <= count; index++)
            {
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.SkiSkillName, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.SkiUsedTimes, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.SkiAbility, index);
            }

            count = TranslateUtils.ToInt(form[ResumeContentAttribute.CerCount]);
            contentInfo.SetExtendedAttribute(ResumeContentAttribute.CerCount, count.ToString());
            for (var index = 1; index <= count; index++)
            {
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.CerCertificationName, index);
                SetValueByIndex(contentInfo, form, ResumeContentAttribute.CerEffectiveDate, index);
            }

            return contentInfo;
        }

        private void SetValueByIndex(ResumeContentInfo contentInfo, NameValueCollection form, string attributeName, int index)
        {
            var value = form[ResumeContentAttribute.GetAttributeName(attributeName, index)];
            if (value == null)
            {
                value = string.Empty;
            }
            value = value.Replace("&", string.Empty);
            if (index == 1)
            {
                contentInfo.SetExtendedAttribute(attributeName, value);
            }
            else
            {
                contentInfo.SetExtendedAttribute(attributeName, contentInfo.GetExtendedAttribute(attributeName) + "&" + value);
            }
        }

        public ResumeContentInfo GetContentInfo(int contentId)
		{
			ResumeContentInfo info = null;
            string sqlWhere = $"WHERE ID = {contentId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, sqlWhere);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    info = new ResumeContentInfo();
                    BaiRongDataProvider.DatabaseDao.ReadResultsToExtendedAttributes(rdr, info);
                }
                rdr.Close();
            }

		    info?.AfterExecuteReader();
		    return info;
		}

        public int GetCount(int publishmentSystemId, int jobContentId)
        {
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM {TableName} WHERE (PublishmentSystemID = {publishmentSystemId} AND JobContentID = {jobContentId})";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetSelectStringOfId(int publishmentSystemId, int jobContentId, string whereString)
        {
            string where =
                $"WHERE (PublishmentSystemID = {publishmentSystemId} AND JobContentID = {jobContentId} {whereString}) ORDER BY ID DESC";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, "ID", where);
        }

        public string GetSelectSqlString(int styleId, int startNum, int totalNum, string whereString, string orderByString)
        {
            if (!string.IsNullOrEmpty(whereString) && !StringUtils.StartsWithIgnoreCase(whereString.Trim(), "AND "))
            {
                whereString = "AND " + whereString.Trim();
            }
            string sqlWhereString = $"WHERE StyleID = {styleId} {whereString}";

            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, startNum, totalNum, SqlUtils.Asterisk, sqlWhereString, orderByString);
        }

        public string GetSortFieldName()
        {
            return "ID";
        }
	}
}
