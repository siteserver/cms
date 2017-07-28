using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Provider
{
    public class TemplateDao : DataProviderBase
    {
        private const string SqlSelectTemplateByTemplateName = "SELECT TemplateID, PublishmentSystemID, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE PublishmentSystemID = @PublishmentSystemID AND TemplateType = @TemplateType AND TemplateName = @TemplateName";

        private const string SqlSelectAllTemplateByType = "SELECT TemplateID, PublishmentSystemID, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE PublishmentSystemID = @PublishmentSystemID AND TemplateType = @TemplateType ORDER BY RelatedFileName";

        private const string SqlSelectAllTemplateIdByType = "SELECT TemplateID FROM siteserver_Template WHERE PublishmentSystemID = @PublishmentSystemID AND TemplateType = @TemplateType ORDER BY RelatedFileName";

        private const string SqlSelectAllTemplateByPublishmentSystemId = "SELECT TemplateID, PublishmentSystemID, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY TemplateType, RelatedFileName";

        private const string SqlSelectTemplateNames = "SELECT TemplateName FROM siteserver_Template WHERE PublishmentSystemID = @PublishmentSystemID AND TemplateType = @TemplateType";

        private const string SqlSelectTemplateCount = "SELECT TemplateType, COUNT(*) FROM siteserver_Template WHERE PublishmentSystemID = @PublishmentSystemID GROUP BY TemplateType";

        private const string SqlSelectRelatedFileNameByTemplateType = "SELECT RelatedFileName FROM siteserver_Template WHERE PublishmentSystemID = @PublishmentSystemID AND TemplateType = @TemplateType";

        private const string SqlUpdateTemplate = "UPDATE siteserver_Template SET TemplateName = @TemplateName, TemplateType = @TemplateType, RelatedFileName = @RelatedFileName, CreatedFileFullName = @CreatedFileFullName, CreatedFileExtName = @CreatedFileExtName, Charset = @Charset, IsDefault = @IsDefault WHERE  TemplateID = @TemplateID";

        private const string SqlDeleteTemplate = "DELETE FROM siteserver_Template WHERE  TemplateID = @TemplateID";

        //by 20151106 sofuny
        private const string SqlSelectTemplateByUrlType = "SELECT * FROM siteserver_Template WHERE PublishmentSystemID = @PublishmentSystemID AND TemplateType = @TemplateType and CreatedFileFullName=@CreatedFileFullName ";
        private const string SqlSelectTemplateByTemplateid = "SELECT * FROM siteserver_Template WHERE PublishmentSystemID = @PublishmentSystemID AND TemplateType = @TemplateType and TemplateID = @TemplateID ";

        private const string ParmTemplateId = "@TemplateID";
        private const string ParmPublishmentSystemId = "@PublishmentSystemID";
        private const string ParmTemplateName = "@TemplateName";
        private const string ParmTemplateType = "@TemplateType";
        private const string ParmRelatedFileName = "@RelatedFileName";
        private const string ParmCreatedFileFullName = "@CreatedFileFullName";
        private const string ParmCreatedFileExtName = "@CreatedFileExtName";
        private const string ParmCharset = "@Charset";
        private const string ParmIsDefault = "@IsDefault";

        public int Insert(TemplateInfo templateInfo, string templateContent, string administratorName)
        {
            int templateId;
            if (templateInfo.IsDefault)
            {
                SetAllTemplateDefaultToFalse(templateInfo.PublishmentSystemId, templateInfo.TemplateType);
            }

            var sqlInsertTemplate = "INSERT INTO siteserver_Template (PublishmentSystemID, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault) VALUES (@PublishmentSystemID, @TemplateName, @TemplateType, @RelatedFileName, @CreatedFileFullName, @CreatedFileExtName, @Charset, @IsDefault)";
            
            var insertParms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, templateInfo.PublishmentSystemId),
				GetParameter(ParmTemplateName, EDataType.NVarChar, 50, templateInfo.TemplateName),
				GetParameter(ParmTemplateType, EDataType.VarChar, 50, ETemplateTypeUtils.GetValue(templateInfo.TemplateType)),
				GetParameter(ParmRelatedFileName, EDataType.NVarChar, 50, templateInfo.RelatedFileName),
				GetParameter(ParmCreatedFileFullName, EDataType.NVarChar, 50, templateInfo.CreatedFileFullName),
				GetParameter(ParmCreatedFileExtName, EDataType.VarChar, 50, templateInfo.CreatedFileExtName),
                GetParameter(ParmCharset, EDataType.VarChar, 50, ECharsetUtils.GetValue(templateInfo.Charset)),
				GetParameter(ParmIsDefault, EDataType.VarChar, 18, templateInfo.IsDefault.ToString())
			};

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        templateId = ExecuteNonQueryAndReturnId(trans, sqlInsertTemplate, insertParms);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(templateInfo.PublishmentSystemId);
            TemplateManager.WriteContentToTemplateFile(publishmentSystemInfo, templateInfo, templateContent, administratorName);

            TemplateManager.RemoveCache(templateInfo.PublishmentSystemId);

            return templateId;
        }

        public void Update(PublishmentSystemInfo publishmentSystemInfo, TemplateInfo templateInfo, string templateContent, string administratorName)
        {
            if (templateInfo.IsDefault)
            {
                SetAllTemplateDefaultToFalse(publishmentSystemInfo.PublishmentSystemId, templateInfo.TemplateType);
            }

            var updateParms = new IDataParameter[]
			{
				GetParameter(ParmTemplateName, EDataType.NVarChar, 50, templateInfo.TemplateName),
				GetParameter(ParmTemplateType, EDataType.VarChar, 50, ETemplateTypeUtils.GetValue(templateInfo.TemplateType)),
				GetParameter(ParmRelatedFileName, EDataType.NVarChar, 50, templateInfo.RelatedFileName),
				GetParameter(ParmCreatedFileFullName, EDataType.NVarChar, 50, templateInfo.CreatedFileFullName),
				GetParameter(ParmCreatedFileExtName, EDataType.VarChar, 50, templateInfo.CreatedFileExtName),
				GetParameter(ParmCharset, EDataType.VarChar, 50, ECharsetUtils.GetValue(templateInfo.Charset)),
				GetParameter(ParmIsDefault, EDataType.VarChar, 18, templateInfo.IsDefault.ToString()),
				GetParameter(ParmTemplateId, EDataType.Integer, templateInfo.TemplateId)
			};

            ExecuteNonQuery(SqlUpdateTemplate, updateParms);

            TemplateManager.WriteContentToTemplateFile(publishmentSystemInfo, templateInfo, templateContent, administratorName);

            TemplateManager.RemoveCache(templateInfo.PublishmentSystemId);
        }

        private void SetAllTemplateDefaultToFalse(int publishmentSystemId, ETemplateType templateType)
        {
            var sqlString = "UPDATE siteserver_Template SET IsDefault = @IsDefault WHERE PublishmentSystemID = @PublishmentSystemID AND TemplateType = @TemplateType";

            var updateParms = new IDataParameter[]
			{
				GetParameter(ParmIsDefault, EDataType.VarChar, 18, false.ToString()),
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmTemplateType, EDataType.VarChar, 50, ETemplateTypeUtils.GetValue(templateType))
			};

            ExecuteNonQuery(sqlString, updateParms);

        }

        public void SetDefault(int publishmentSystemId, int templateId)
        {
            var info = TemplateManager.GetTemplateInfo(publishmentSystemId, templateId);
            SetAllTemplateDefaultToFalse(info.PublishmentSystemId, info.TemplateType);

            var sqlString = "UPDATE siteserver_Template SET IsDefault = @IsDefault WHERE TemplateID = @TemplateID";

            var updateParms = new IDataParameter[]
			{
				GetParameter(ParmIsDefault, EDataType.VarChar, 18, true.ToString()),
				GetParameter(ParmTemplateId, EDataType.Integer, templateId)
			};

            ExecuteNonQuery(sqlString, updateParms);

            TemplateManager.RemoveCache(publishmentSystemId);
        }

        public void Delete(int publishmentSystemId, int templateId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var templateInfo = TemplateManager.GetTemplateInfo(publishmentSystemId, templateId);
            var filePath = TemplateManager.GetTemplateFilePath(publishmentSystemInfo, templateInfo);

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTemplateId, EDataType.Integer, templateId)
			};

            ExecuteNonQuery(SqlDeleteTemplate, parms);
            FileUtils.DeleteFileIfExists(filePath);

            TemplateManager.RemoveCache(publishmentSystemId);
        }

        public string GetImportTemplateName(int publishmentSystemId, string templateName)
        {
            string importTemplateName;
            if (templateName.IndexOf("_", StringComparison.Ordinal) != -1)
            {
                var templateNameCount = 0;
                var lastTemplateName = templateName.Substring(templateName.LastIndexOf("_", StringComparison.Ordinal) + 1);
                var firstTemplateName = templateName.Substring(0, templateName.Length - lastTemplateName.Length);
                try
                {
                    templateNameCount = int.Parse(lastTemplateName);
                }
                catch
                {
                    // ignored
                }
                templateNameCount++;
                importTemplateName = firstTemplateName + templateNameCount;
            }
            else
            {
                importTemplateName = templateName + "_1";
            }

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmTemplateName, EDataType.NVarChar, 50, importTemplateName)
			};

            using (var rdr = ExecuteReader(SqlSelectTemplateByTemplateName, parms))
            {
                if (rdr.Read())
                {
                    importTemplateName = GetImportTemplateName(publishmentSystemId, importTemplateName);
                }
                rdr.Close();
            }

            return importTemplateName;
        }

        public Dictionary<ETemplateType, int> GetCountDictionary(int publishmentSystemId)
        {
            var dictionary = new Dictionary<ETemplateType, int>();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(SqlSelectTemplateCount, parms))
            {
                while (rdr.Read())
                {
                    var templateType = ETemplateTypeUtils.GetEnumType(GetString(rdr, 0));
                    var count = GetInt(rdr, 1);

                    dictionary.Add(templateType, count);
                }
                rdr.Close();
            }

            return dictionary;
        }

        public IEnumerable GetDataSourceByType(int publishmentSystemId, ETemplateType type)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmTemplateType, EDataType.VarChar, 50, ETemplateTypeUtils.GetValue(type))
			};

            var enumerable = (IEnumerable)ExecuteReader(SqlSelectAllTemplateByType, parms);
            return enumerable;
        }

        public IEnumerable GetDataSource(int publishmentSystemId, string searchText, string templateTypeString)
        {
            if (string.IsNullOrEmpty(searchText) && string.IsNullOrEmpty(templateTypeString))
            {
                var parms = new IDataParameter[]
				{
					GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
				};

                var enumerable = (IEnumerable)ExecuteReader(SqlSelectAllTemplateByPublishmentSystemId, parms);
                return enumerable;
            }
            else if (!string.IsNullOrEmpty(searchText))
            {
                var whereString = (string.IsNullOrEmpty(templateTypeString)) ? string.Empty :
                    $"AND TemplateType = '{templateTypeString}' ";
                searchText = PageUtils.FilterSql(searchText);
                whereString +=
                    $"AND (TemplateName LIKE '%{searchText}%' OR RelatedFileName LIKE '%{searchText}%' OR CreatedFileFullName LIKE '%{searchText}%' OR CreatedFileExtName LIKE '%{searchText}%')";
                string sqlString =
                    $"SELECT TemplateID, PublishmentSystemID, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE PublishmentSystemID = {publishmentSystemId} {whereString} ORDER BY TemplateType, RelatedFileName";

                var enumerable = (IEnumerable)ExecuteReader(sqlString);
                return enumerable;
            }
            else
            {
                return GetDataSourceByType(publishmentSystemId, ETemplateTypeUtils.GetEnumType(templateTypeString));
            }
        }

        public ArrayList GetTemplateIdArrayListByType(int publishmentSystemId, ETemplateType type)
        {
            var arraylist = new ArrayList();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmTemplateType, EDataType.VarChar, 50, ETemplateTypeUtils.GetValue(type))
			};

            using (var rdr = ExecuteReader(SqlSelectAllTemplateIdByType, parms))
            {
                while (rdr.Read())
                {
                    var templateId = GetInt(rdr, 0);
                    arraylist.Add(templateId);
                }
                rdr.Close();
            }
            return arraylist;
        }

        public ArrayList GetTemplateInfoArrayListByType(int publishmentSystemId, ETemplateType type)
        {
            var arraylist = new ArrayList();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmTemplateType, EDataType.VarChar, 50, ETemplateTypeUtils.GetValue(type))
			};

            using (var rdr = ExecuteReader(SqlSelectAllTemplateByType, parms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var info = new TemplateInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), ETemplateTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), ECharsetUtils.GetEnumType(GetString(rdr, i++)), GetBool(rdr, i));
                    arraylist.Add(info);
                }
                rdr.Close();
            }
            return arraylist;
        }

        public ArrayList GetTemplateInfoArrayListOfFile(int publishmentSystemId)
        {
            var arraylist = new ArrayList();

            string sqlString =
                $"SELECT TemplateID, PublishmentSystemID, TemplateName, TemplateType, RelatedFileName, CreatedFileFullName, CreatedFileExtName, Charset, IsDefault FROM siteserver_Template WHERE PublishmentSystemID = {publishmentSystemId} AND TemplateType = '{ETemplateTypeUtils.GetValue(ETemplateType.FileTemplate)}' ORDER BY RelatedFileName";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var info = new TemplateInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), ETemplateTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), ECharsetUtils.GetEnumType(GetString(rdr, i++)), GetBool(rdr, i));
                    arraylist.Add(info);
                }
                rdr.Close();
            }
            return arraylist;
        }

        public ArrayList GetTemplateInfoArrayListByPublishmentSystemId(int publishmentSystemId)
        {
            var arraylist = new ArrayList();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(SqlSelectAllTemplateByPublishmentSystemId, parms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var info = new TemplateInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), ETemplateTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), ECharsetUtils.GetEnumType(GetString(rdr, i++)), GetBool(rdr, i));
                    arraylist.Add(info);
                }
                rdr.Close();
            }
            return arraylist;
        }

        public ArrayList GetTemplateNameArrayList(int publishmentSystemId, ETemplateType templateType)
        {
            var list = new ArrayList();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmTemplateType, EDataType.VarChar, 50, ETemplateTypeUtils.GetValue(templateType))
			};

            using (var rdr = ExecuteReader(SqlSelectTemplateNames, parms))
            {
                while (rdr.Read())
                {
                    list.Add(GetString(rdr, 0));
                }
                rdr.Close();
            }

            return list;
        }

        public ArrayList GetLowerRelatedFileNameArrayList(int publishmentSystemId, ETemplateType templateType)
        {
            var list = new ArrayList();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmTemplateType, EDataType.VarChar, 50, ETemplateTypeUtils.GetValue(templateType))
			};

            using (var rdr = ExecuteReader(SqlSelectRelatedFileNameByTemplateType, parms))
            {
                while (rdr.Read())
                {
                    list.Add(GetString(rdr, 0).ToLower());
                }
                rdr.Close();
            }

            return list;
        }

        public void CreateDefaultTemplateInfo(int publishmentSystemId, string administratorName)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);

            var templateInfoList = new List<TemplateInfo>();
            var charset = ECharsetUtils.GetEnumType(publishmentSystemInfo.Additional.Charset);

            var templateInfo = new TemplateInfo(0, publishmentSystemInfo.PublishmentSystemId, "系统首页模板", ETemplateType.IndexPageTemplate, "T_系统首页模板.html", "@/index.html", ".html", charset, true);
            templateInfoList.Add(templateInfo);

            templateInfo = new TemplateInfo(0, publishmentSystemInfo.PublishmentSystemId, "系统栏目模板", ETemplateType.ChannelTemplate, "T_系统栏目模板.html", "index.html", ".html", charset, true);
            templateInfoList.Add(templateInfo);

            templateInfo = new TemplateInfo(0, publishmentSystemInfo.PublishmentSystemId, "系统内容模板", ETemplateType.ContentTemplate, "T_系统内容模板.html", "index.html", ".html", charset, true);
            templateInfoList.Add(templateInfo);

            foreach (var theTemplateInfo in templateInfoList)
            {
                Insert(theTemplateInfo, theTemplateInfo.Content, administratorName);
            }
        }

        public int GetTemplateUseCount(int publishmentSystemId, int templateId, ETemplateType templateType, bool isDefault)
        {
            var sqlString = string.Empty;

            if (templateType == ETemplateType.IndexPageTemplate)
            {
                if (isDefault)
                {
                    return 1;
                }
                return 0;
            }
            else if (templateType == ETemplateType.FileTemplate)
            {
                return 1;
            }
            else if (templateType == ETemplateType.ChannelTemplate)
            {
                if (isDefault)
                {
                    sqlString =
                        $"SELECT count(*) FROM siteserver_Node WHERE (ChannelTemplateID = {templateId} OR ChannelTemplateID = 0) AND PublishmentSystemID = {publishmentSystemId}";
                }
                else
                {
                    sqlString = $"SELECT count(*) FROM siteserver_Node WHERE ChannelTemplateID = {templateId}";
                }
            }
            else if (templateType == ETemplateType.ContentTemplate)
            {
                if (isDefault)
                {
                    sqlString =
                        $"SELECT count(*) FROM siteserver_Node WHERE (ContentTemplateID = {templateId} OR ContentTemplateID = 0) AND PublishmentSystemID = {publishmentSystemId}";
                }
                else
                {
                    sqlString = $"SELECT count(*) FROM siteserver_Node WHERE ContentTemplateID = {templateId}";
                }
            }

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<int> GetNodeIdArrayList(TemplateInfo templateInfo)
        {
            var list = new List<int>();
            var sqlString = string.Empty;

            if (templateInfo.TemplateType == ETemplateType.ChannelTemplate)
            {
                if (templateInfo.IsDefault)
                {
                    sqlString =
                        $"SELECT NodeID FROM siteserver_Node WHERE (ChannelTemplateID = {templateInfo.TemplateId} OR ChannelTemplateID = 0) AND PublishmentSystemID = {templateInfo.PublishmentSystemId}";
                }
                else
                {
                    sqlString =
                        $"SELECT NodeID FROM siteserver_Node WHERE ChannelTemplateID = {templateInfo.TemplateId} AND PublishmentSystemID = {templateInfo.PublishmentSystemId}";
                }
            }
            else if (templateInfo.TemplateType == ETemplateType.ContentTemplate)
            {
                if (templateInfo.IsDefault)
                {
                    sqlString =
                        $"SELECT NodeID FROM siteserver_Node WHERE (ContentTemplateID = {templateInfo.TemplateId} OR ContentTemplateID = 0) AND PublishmentSystemID = {templateInfo.PublishmentSystemId}";
                }
                else
                {
                    sqlString =
                        $"SELECT NodeID FROM siteserver_Node WHERE ContentTemplateID = {templateInfo.TemplateId} AND PublishmentSystemID = {templateInfo.PublishmentSystemId}";
                }
            }

            if (!string.IsNullOrEmpty(sqlString))
            {
                list = BaiRongDataProvider.DatabaseDao.GetIntList(sqlString);
            }

            return list;
        }

        public Dictionary<int, TemplateInfo> GetTemplateInfoDictionaryByPublishmentSystemId(int publishmentSystemId)
        {
            var dictionary = new Dictionary<int, TemplateInfo>();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(SqlSelectAllTemplateByPublishmentSystemId, parms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var info = new TemplateInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), ETemplateTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), ECharsetUtils.GetEnumType(GetString(rdr, i++)), GetBool(rdr, i));
                    dictionary.Add(info.TemplateId, info);
                }
                rdr.Close();
            }

            return dictionary;
        }


        public TemplateInfo GetTemplateByUrlType(int publishmentSystemId, ETemplateType type, string createdFileFullName)
        {
            TemplateInfo info = null;
            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmTemplateType, EDataType.VarChar, 50, ETemplateTypeUtils.GetValue(type)),
				GetParameter(ParmCreatedFileFullName, EDataType.VarChar, 50, createdFileFullName)
			};

            using (var rdr = ExecuteReader(SqlSelectTemplateByUrlType, parms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    info = new TemplateInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), ETemplateTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), ECharsetUtils.GetEnumType(GetString(rdr, i++)), GetBool(rdr, i));
                }
                rdr.Close();
            }
            return info;
        }

        public TemplateInfo GetTemplateByTemplateId(int publishmentSystemId, ETemplateType type, string tId)
        {
            TemplateInfo info = null;
            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmTemplateType, EDataType.VarChar, 50, ETemplateTypeUtils.GetValue(type)),
				GetParameter(ParmTemplateId, EDataType.Integer, tId)
			};

            using (var rdr = ExecuteReader(SqlSelectTemplateByTemplateid, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    info = new TemplateInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), ETemplateTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), ECharsetUtils.GetEnumType(GetString(rdr, i++)), GetBool(rdr, i));
                }
                rdr.Close();
            }
            return info;
        }

    }
}
