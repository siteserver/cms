using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Provider
{
    public class ContentModelDao : DataProviderBase
    {
        private const string SqlInsert = "INSERT INTO bairong_ContentModel (ModelID, SiteID, ModelName, IsSystem, TableName, TableType, IconUrl, Description) VALUES (@ModelID, @SiteID, @ModelName, @IsSystem, @TableName, @TableType, @IconUrl, @Description)";

        private const string SqlUpdate = "UPDATE bairong_ContentModel SET ModelName = @ModelName, TableName = @TableName, TableType = @TableType, IconUrl = @IconUrl, Description = @Description WHERE ModelID = @ModelID AND SiteID = @SiteID";

        private const string SqlDelete = "DELETE FROM bairong_ContentModel WHERE ModelID = @ModelID AND SiteID = @SiteID";

        private const string SqlSelect = "SELECT ModelID, SiteID, ModelName, IsSystem, TableName, TableType, IconUrl, Description FROM bairong_ContentModel WHERE ModelID = @ModelID AND SiteID = @SiteID";

        private const string ParmModelId = "@ModelID";
        private const string ParmSiteId = "@SiteID";
        private const string ParmModelName = "@ModelName";
        private const string ParmIsSystem = "@IsSystem";
        private const string ParmTableName = "@TableName";
        private const string ParmTableType = "@TableType";
        private const string ParmIconUrl = "@IconUrl";
        private const string ParmDescription = "@Description";

        public void Insert(ContentModelInfo contentModelInfo)
        {
            var parms = new IDataParameter[]
			{
                GetParameter(ParmModelId, EDataType.VarChar, 50, contentModelInfo.ModelId),
				GetParameter(ParmSiteId, EDataType.Integer, contentModelInfo.SiteId),
                GetParameter(ParmModelName, EDataType.NVarChar, 50, contentModelInfo.ModelName),
                GetParameter(ParmIsSystem, EDataType.VarChar, 18, contentModelInfo.IsSystem.ToString()),
				GetParameter(ParmTableName, EDataType.VarChar, 200, contentModelInfo.TableName),
                GetParameter(ParmTableType, EDataType.VarChar, 50, EAuxiliaryTableTypeUtils.GetValue(contentModelInfo.TableType)),
                GetParameter(ParmIconUrl, EDataType.VarChar, 50, contentModelInfo.IconUrl),
                GetParameter(ParmDescription, EDataType.NVarChar, 255, contentModelInfo.Description)
			};

            ExecuteNonQuery(SqlInsert, parms);

            ContentModelUtils.RemoveCache(contentModelInfo.SiteId);
        }

        public void Update(ContentModelInfo contentModelInfo)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmModelName, EDataType.NVarChar, 50, contentModelInfo.ModelName),
                GetParameter(ParmTableName, EDataType.VarChar, 200, contentModelInfo.TableName),
                GetParameter(ParmTableType, EDataType.VarChar, 50, EAuxiliaryTableTypeUtils.GetValue(contentModelInfo.TableType)),
                GetParameter(ParmIconUrl, EDataType.VarChar, 50, contentModelInfo.IconUrl),
                GetParameter(ParmDescription, EDataType.NVarChar, 255, contentModelInfo.Description),
                GetParameter(ParmModelId, EDataType.VarChar, 50, contentModelInfo.ModelId),
				GetParameter(ParmSiteId, EDataType.Integer, contentModelInfo.SiteId)
			};

            ExecuteNonQuery(SqlUpdate, parms);

            ContentModelUtils.RemoveCache(contentModelInfo.SiteId);
        }

        public void Delete(string modelId, int siteId)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmModelId, EDataType.VarChar, 50, modelId),
				GetParameter(ParmSiteId, EDataType.Integer, siteId)
			};

            ExecuteNonQuery(SqlDelete, parms);

            ContentModelUtils.RemoveCache(siteId);
        }

        public ContentModelInfo GetContentModelInfo(string modelId, int siteId)
        {
            ContentModelInfo contentModelInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmModelId, EDataType.VarChar, 50, modelId),
				GetParameter(ParmSiteId, EDataType.Integer, siteId)
			};

            using (var rdr = ExecuteReader(SqlSelect, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    contentModelInfo = new ContentModelInfo(GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), EAuxiliaryTableTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

            return contentModelInfo;
        }

        public IEnumerable GetDataSource(int siteId)
        {
            string sqlString =
                $"SELECT ModelID, SiteID, ModelName, IsSystem, TableName, TableType, IconUrl, Description FROM bairong_ContentModel WHERE SiteID = {siteId}";
            var enumerable = (IEnumerable)ExecuteReader(sqlString);
            return enumerable;
        }

        public List<ContentModelInfo> GetContentModelInfoList(int siteId)
        {
            var arraylist = new List<ContentModelInfo>();
            string sqlString =
                $"SELECT ModelID, SiteID, ModelName, IsSystem, TableName, TableType, IconUrl, Description FROM bairong_ContentModel WHERE SiteID = {siteId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    arraylist.Add(new ContentModelInfo(GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), EAuxiliaryTableTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetString(rdr, i)));
                }
                rdr.Close();
            }

            return arraylist;
        }

        public string GetImportContentModelId(int publishmentSystemId, string modelId)
        {
            string importModelId;
            if (modelId.Contains("_"))
            {
                var modelIdCount = 0;
                var lastModeId = modelId.Substring(modelId.LastIndexOf("_", StringComparison.Ordinal) + 1);
                var firstModeId = modelId.Substring(0, modelId.Length - lastModeId.Length);
                try
                {
                    modelIdCount = int.Parse(lastModeId);
                }
                catch
                {
                    // ignored
                }
                modelIdCount++;
                importModelId = firstModeId + modelIdCount;
            }
            else
            {
                importModelId = modelId + "_1";
            }

            var parms = new IDataParameter[]
			{
				GetParameter(ParmModelId, EDataType.NVarChar, 50, importModelId),
				GetParameter(ParmSiteId, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(SqlSelect, parms))
            {
                if (rdr.Read())
                {
                    importModelId = GetImportContentModelId(publishmentSystemId, importModelId);
                }
                rdr.Close();
            }

            return importModelId;
        }
    }
}
