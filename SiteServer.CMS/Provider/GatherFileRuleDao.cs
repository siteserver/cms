using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Provider
{
	public class GatherFileRuleDao : DataProviderBase
	{
        public override string TableName => "siteserver_GatherFileRule";

        public override List<TableColumnInfo> TableColumns => new List<TableColumnInfo>
        {
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.GatherRuleName),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.PublishmentSystemId),
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.GatherUrl),
                DataType = DataType.VarChar,
                Length = 255
            },
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.Charset),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.LastGatherDate),
                DataType = DataType.DateTime
            },
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.IsToFile),
                DataType = DataType.VarChar,
                Length = 18
            },
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.FilePath),
                DataType = DataType.VarChar,
                Length = 255
            },
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.IsSaveRelatedFiles),
                DataType = DataType.VarChar,
                Length = 18
            },
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.IsRemoveScripts),
                DataType = DataType.VarChar,
                Length = 18
            },
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.StyleDirectoryPath),
                DataType = DataType.VarChar,
                Length = 255
            },
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.ScriptDirectoryPath),
                DataType = DataType.VarChar,
                Length = 255
            },
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.ImageDirectoryPath),
                DataType = DataType.VarChar,
                Length = 255
            },
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.NodeId),
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.IsSaveImage),
                DataType = DataType.VarChar,
                Length = 18
            },
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.IsChecked),
                DataType = DataType.VarChar,
                Length = 18
            },
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.IsAutoCreate),
                DataType = DataType.VarChar,
                Length = 18
            },
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.ContentExclude),
                DataType = DataType.Text
            },
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.ContentHtmlClearCollection),
                DataType = DataType.VarChar,
                Length = 255
            },
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.ContentHtmlClearTagCollection),
                DataType = DataType.VarChar,
                Length = 255
            },
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.ContentTitleStart),
                DataType = DataType.Text
            },
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.ContentTitleEnd),
                DataType = DataType.Text
            },
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.ContentContentStart),
                DataType = DataType.Text
            },
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.ContentContentEnd),
                DataType = DataType.Text
            },
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.ContentAttributes),
                DataType = DataType.Text
            },
            new TableColumnInfo
            {
                ColumnName = nameof(GatherFileRuleInfo.ContentAttributesXml),
                DataType = DataType.Text
            }
        };

        private const string SqlSelectGatherFileRule = "SELECT GatherRuleName, PublishmentSystemID, GatherUrl, Charset, LastGatherDate, IsToFile, FilePath, IsSaveRelatedFiles, IsRemoveScripts, StyleDirectoryPath, ScriptDirectoryPath, ImageDirectoryPath, NodeID, IsSaveImage, IsChecked, ContentExclude, ContentHtmlClearCollection, ContentHtmlClearTagCollection, ContentTitleStart, ContentTitleEnd, ContentContentStart, ContentContentEnd, ContentAttributes, ContentAttributesXML, IsAutoCreate FROM siteserver_GatherFileRule WHERE GatherRuleName = @GatherRuleName AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectAllGatherFileRuleByPsId = "SELECT GatherRuleName, PublishmentSystemID, GatherUrl, Charset, LastGatherDate, IsToFile, FilePath, IsSaveRelatedFiles, IsRemoveScripts, StyleDirectoryPath, ScriptDirectoryPath, ImageDirectoryPath, NodeID, IsSaveImage, IsChecked, ContentExclude, ContentHtmlClearCollection, ContentHtmlClearTagCollection, ContentTitleStart, ContentTitleEnd, ContentContentStart, ContentContentEnd, ContentAttributes, ContentAttributesXML, IsAutoCreate FROM siteserver_GatherFileRule WHERE PublishmentSystemID = @PublishmentSystemID";

		private const string SqlSelectGatherFileRuleNameByPsId = "SELECT GatherRuleName FROM siteserver_GatherFileRule WHERE PublishmentSystemID = @PublishmentSystemID";

		private const string SqlInsertGatherFileRule = @"
INSERT INTO siteserver_GatherFileRule 
(GatherRuleName, PublishmentSystemID, GatherUrl, Charset, LastGatherDate, IsToFile, FilePath, IsSaveRelatedFiles, IsRemoveScripts, StyleDirectoryPath, ScriptDirectoryPath, ImageDirectoryPath, NodeID, IsSaveImage, IsChecked, ContentExclude, ContentHtmlClearCollection, ContentHtmlClearTagCollection, ContentTitleStart, ContentTitleEnd, ContentContentStart, ContentContentEnd, ContentAttributes, ContentAttributesXML, IsAutoCreate) VALUES (@GatherRuleName, @PublishmentSystemID, @GatherUrl, @Charset, @LastGatherDate, @IsToFile, @FilePath, @IsSaveRelatedFiles, @IsRemoveScripts, @StyleDirectoryPath, @ScriptDirectoryPath, @ImageDirectoryPath, @NodeID, @IsSaveImage, @IsChecked, @ContentExclude, @ContentHtmlClearCollection, @ContentHtmlClearTagCollection, @ContentTitleStart, @ContentTitleEnd, @ContentContentStart, @ContentContentEnd, @ContentAttributes, @ContentAttributesXML, @IsAutoCreate)";

		private const string SqlUpdateGatherFileRule = @"
UPDATE siteserver_GatherFileRule SET 
GatherUrl = @GatherUrl, Charset = @Charset, LastGatherDate = @LastGatherDate, IsToFile = @IsToFile, FilePath = @FilePath, IsSaveRelatedFiles = @IsSaveRelatedFiles, IsRemoveScripts = @IsRemoveScripts, StyleDirectoryPath = @StyleDirectoryPath, ScriptDirectoryPath = @ScriptDirectoryPath, ImageDirectoryPath = @ImageDirectoryPath, NodeID = @NodeID, IsSaveImage = @IsSaveImage, IsChecked = @IsChecked, ContentExclude = @ContentExclude, ContentHtmlClearCollection = @ContentHtmlClearCollection, ContentHtmlClearTagCollection = @ContentHtmlClearTagCollection, ContentTitleStart = @ContentTitleStart, ContentTitleEnd = @ContentTitleEnd, ContentContentStart = @ContentContentStart, ContentContentEnd = @ContentContentEnd, ContentAttributes = @ContentAttributes, ContentAttributesXML = @ContentAttributesXML, IsAutoCreate = @IsAutoCreate WHERE GatherRuleName = @GatherRuleName AND PublishmentSystemID = @PublishmentSystemID";

		private const string SqlUpdateLastGatherDate = "UPDATE siteserver_GatherFileRule SET LastGatherDate = @LastGatherDate WHERE GatherRuleName = @GatherRuleName AND PublishmentSystemID = @PublishmentSystemID";

		private const string SqlDeleteGatherFileRule = "DELETE FROM siteserver_GatherFileRule WHERE GatherRuleName = @GatherRuleName AND PublishmentSystemID = @PublishmentSystemID";

		private const string ParmGatherFileRuleName = "@GatherRuleName";
		private const string ParmPublishmentSystemId = "@PublishmentSystemID";
        private const string ParmGatherUrl = "@GatherUrl";
        private const string ParmCharset = "@Charset";
        private const string ParmLastGatherDate = "@LastGatherDate";
        private const string ParmIsToFile = "@IsToFile";
        private const string ParmFilePath = "@FilePath";
        private const string ParmIsSaveRelatedFiles = "@IsSaveRelatedFiles";
        private const string ParmIsRemoveScripts = "@IsRemoveScripts";
        private const string ParmStyleDirectoryPath = "@StyleDirectoryPath";
        private const string ParmScriptDirectoryPath = "@ScriptDirectoryPath";
        private const string ParmImageDirectoryPath = "@ImageDirectoryPath";

        private const string ParmNodeId = "@NodeID";
        private const string ParmIsSaveImage = "@IsSaveImage";
        private const string ParmIsChecked = "@IsChecked";
        private const string ParmContentExclude = "@ContentExclude";
        private const string ParmContentHtmlClearCollection = "@ContentHtmlClearCollection";
        private const string ParmContentHtmlClearTagCollection = "@ContentHtmlClearTagCollection";
		private const string ParmContentTitleStart = "@ContentTitleStart";
		private const string ParmContentTitleEnd = "@ContentTitleEnd";
		private const string ParmContentContentStart = "@ContentContentStart";
		private const string ParmContentContentEnd = "@ContentContentEnd";
        private const string ParmContentAttributes = "@ContentAttributes";
        private const string ParmContentAttributesXml = "@ContentAttributesXML";
        private const string ParmIsAutoCreate = "@IsAutoCreate";

        public void Insert(GatherFileRuleInfo gatherFileRuleInfo) 
		{
			var insertParms = new IDataParameter[]
			{
				GetParameter(ParmGatherFileRuleName, DataType.VarChar, 50, gatherFileRuleInfo.GatherRuleName),
				GetParameter(ParmPublishmentSystemId, DataType.Integer, gatherFileRuleInfo.PublishmentSystemId),
				GetParameter(ParmGatherUrl, DataType.VarChar, 255, gatherFileRuleInfo.GatherUrl),
				GetParameter(ParmCharset, DataType.VarChar, 50, ECharsetUtils.GetValue(gatherFileRuleInfo.Charset)),
                GetParameter(ParmLastGatherDate, DataType.DateTime, gatherFileRuleInfo.LastGatherDate),
                GetParameter(ParmIsToFile, DataType.VarChar, 18, gatherFileRuleInfo.IsToFile.ToString()),
                GetParameter(ParmFilePath, DataType.VarChar, 255, gatherFileRuleInfo.FilePath),
                GetParameter(ParmIsSaveRelatedFiles, DataType.VarChar, 18, gatherFileRuleInfo.IsSaveRelatedFiles.ToString()),
                GetParameter(ParmIsRemoveScripts, DataType.VarChar, 18, gatherFileRuleInfo.IsRemoveScripts.ToString()),
                GetParameter(ParmStyleDirectoryPath, DataType.VarChar, 255, gatherFileRuleInfo.StyleDirectoryPath),
                GetParameter(ParmScriptDirectoryPath, DataType.VarChar, 255, gatherFileRuleInfo.ScriptDirectoryPath),
                GetParameter(ParmImageDirectoryPath, DataType.VarChar, 255, gatherFileRuleInfo.ImageDirectoryPath),

                GetParameter(ParmNodeId, DataType.Integer, gatherFileRuleInfo.NodeId),
				GetParameter(ParmIsSaveImage, DataType.VarChar, 18, gatherFileRuleInfo.IsSaveImage.ToString()),
                GetParameter(ParmIsChecked, DataType.VarChar, 18, gatherFileRuleInfo.IsChecked.ToString()),
                GetParameter(ParmContentExclude, DataType.Text, gatherFileRuleInfo.ContentExclude),
				GetParameter(ParmContentHtmlClearCollection, DataType.VarChar, 255, gatherFileRuleInfo.ContentHtmlClearCollection),
                GetParameter(ParmContentHtmlClearTagCollection, DataType.VarChar, 255, gatherFileRuleInfo.ContentHtmlClearTagCollection),
				GetParameter(ParmContentTitleStart, DataType.Text, gatherFileRuleInfo.ContentTitleStart),
				GetParameter(ParmContentTitleEnd, DataType.Text, gatherFileRuleInfo.ContentTitleEnd),
				GetParameter(ParmContentContentStart, DataType.Text, gatherFileRuleInfo.ContentContentStart),
				GetParameter(ParmContentContentEnd, DataType.Text, gatherFileRuleInfo.ContentContentEnd),
                GetParameter(ParmContentAttributes, DataType.Text, gatherFileRuleInfo.ContentAttributes),
                GetParameter(ParmContentAttributesXml, DataType.Text, gatherFileRuleInfo.ContentAttributesXml),
                GetParameter(ParmIsAutoCreate, DataType.VarChar, 18, gatherFileRuleInfo.IsAutoCreate.ToString())
            };

            ExecuteNonQuery(SqlInsertGatherFileRule, insertParms);
		}

		public void UpdateLastGatherDate(string gatherRuleName, int publishmentSystemId)
		{
			var parms = new IDataParameter[]
			{
				GetParameter(ParmLastGatherDate, DataType.DateTime, DateTime.Now),
				GetParameter(ParmGatherFileRuleName, DataType.VarChar, 50, gatherRuleName),
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
			};
							
			ExecuteNonQuery(SqlUpdateLastGatherDate, parms);
		}

		public void Update(GatherFileRuleInfo gatherFileRuleInfo) 
		{

			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmGatherUrl, DataType.VarChar, 255, gatherFileRuleInfo.GatherUrl),
				GetParameter(ParmCharset, DataType.VarChar, 50, ECharsetUtils.GetValue(gatherFileRuleInfo.Charset)),
                GetParameter(ParmLastGatherDate, DataType.DateTime, gatherFileRuleInfo.LastGatherDate),
                GetParameter(ParmIsToFile, DataType.VarChar, 18, gatherFileRuleInfo.IsToFile.ToString()),
                GetParameter(ParmFilePath, DataType.VarChar, 255, gatherFileRuleInfo.FilePath),
                GetParameter(ParmIsSaveRelatedFiles, DataType.VarChar, 18, gatherFileRuleInfo.IsSaveRelatedFiles.ToString()),
                GetParameter(ParmIsRemoveScripts, DataType.VarChar, 18, gatherFileRuleInfo.IsRemoveScripts.ToString()),
                GetParameter(ParmStyleDirectoryPath, DataType.VarChar, 255, gatherFileRuleInfo.StyleDirectoryPath),
                GetParameter(ParmScriptDirectoryPath, DataType.VarChar, 255, gatherFileRuleInfo.ScriptDirectoryPath),
                GetParameter(ParmImageDirectoryPath, DataType.VarChar, 255, gatherFileRuleInfo.ImageDirectoryPath),

                GetParameter(ParmNodeId, DataType.Integer, gatherFileRuleInfo.NodeId),
				GetParameter(ParmIsSaveImage, DataType.VarChar, 18, gatherFileRuleInfo.IsSaveImage.ToString()),
                GetParameter(ParmIsChecked, DataType.VarChar, 18, gatherFileRuleInfo.IsChecked.ToString()),
                GetParameter(ParmContentExclude, DataType.Text, gatherFileRuleInfo.ContentExclude),
				GetParameter(ParmContentHtmlClearCollection, DataType.VarChar, 255, gatherFileRuleInfo.ContentHtmlClearCollection),
                GetParameter(ParmContentHtmlClearTagCollection, DataType.VarChar, 255, gatherFileRuleInfo.ContentHtmlClearTagCollection),
				GetParameter(ParmContentTitleStart, DataType.Text, gatherFileRuleInfo.ContentTitleStart),
				GetParameter(ParmContentTitleEnd, DataType.Text, gatherFileRuleInfo.ContentTitleEnd),
				GetParameter(ParmContentContentStart, DataType.Text, gatherFileRuleInfo.ContentContentStart),
				GetParameter(ParmContentContentEnd, DataType.Text, gatherFileRuleInfo.ContentContentEnd),
                GetParameter(ParmContentAttributes, DataType.Text, gatherFileRuleInfo.ContentAttributes),
                GetParameter(ParmContentAttributesXml, DataType.Text, gatherFileRuleInfo.ContentAttributesXml),
				GetParameter(ParmGatherFileRuleName, DataType.VarChar, 50, gatherFileRuleInfo.GatherRuleName),
                GetParameter(ParmIsAutoCreate, DataType.VarChar, 18, gatherFileRuleInfo.IsAutoCreate.ToString()),
                GetParameter(ParmPublishmentSystemId, DataType.Integer, gatherFileRuleInfo.PublishmentSystemId)
			};

            ExecuteNonQuery(SqlUpdateGatherFileRule, updateParms);
		}

		public void Delete(string gatherRuleName, int publishmentSystemId)
		{
			var parms = new IDataParameter[]
			{
				GetParameter(ParmGatherFileRuleName, DataType.VarChar, 50, gatherRuleName),
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
			};
							
			ExecuteNonQuery(SqlDeleteGatherFileRule, parms);
		}

		public GatherFileRuleInfo GetGatherFileRuleInfo(string gatherRuleName, int publishmentSystemId)
		{
			GatherFileRuleInfo gatherFileRuleInfo = null;
			
			var parms = new IDataParameter[]
			{
				GetParameter(ParmGatherFileRuleName, DataType.VarChar, 50, gatherRuleName),
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
			};
			
			using (var rdr = ExecuteReader(SqlSelectGatherFileRule, parms)) 
			{
				if (rdr.Read())
				{
				    var i = 0;
                    gatherFileRuleInfo = new GatherFileRuleInfo(GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), ECharsetUtils.GetEnumType(GetString(rdr, i++)), GetDateTime(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetBool(rdr, i));
				}
				rdr.Close();
			}

			return gatherFileRuleInfo;
		}

		public string GetImportGatherRuleName(int publishmentSystemId, string gatherRuleName)
		{
			string importGatherRuleName;
			if (gatherRuleName.IndexOf("_", StringComparison.Ordinal) != -1)
			{
				var gatherRuleNameCount = 0;
				var lastGatherRuleName = gatherRuleName.Substring(gatherRuleName.LastIndexOf("_", StringComparison.Ordinal) + 1);
				var firstGatherRuleName = gatherRuleName.Substring(0, gatherRuleName.Length - lastGatherRuleName.Length);
				try
				{
					gatherRuleNameCount = int.Parse(lastGatherRuleName);
				}
			    catch
			    {
			        // ignored
			    }
			    gatherRuleNameCount++;
				importGatherRuleName = firstGatherRuleName + gatherRuleNameCount;
			}
			else
			{
				importGatherRuleName = gatherRuleName + "_1";
			}

			var parms = new IDataParameter[]
			{
				GetParameter(ParmGatherFileRuleName, DataType.VarChar, 50, importGatherRuleName),
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
			};

			using (var rdr = ExecuteReader(SqlSelectGatherFileRule, parms))
			{
				if (rdr.Read())
				{
					importGatherRuleName = GetImportGatherRuleName(publishmentSystemId, importGatherRuleName);
				}
				rdr.Close();
			}

			return importGatherRuleName;
		}

		public IEnumerable GetDataSource(int publishmentSystemId)
		{
			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
			};

			var enumerable = (IEnumerable)ExecuteReader(SqlSelectAllGatherFileRuleByPsId, parms);
			return enumerable;
		}

		public List<GatherFileRuleInfo> GetGatherFileRuleInfoList(int publishmentSystemId)
		{
			var list = new List<GatherFileRuleInfo>();

			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
			};

			using (var rdr = ExecuteReader(SqlSelectAllGatherFileRuleByPsId, parms))
			{
				while (rdr.Read())
				{
				    var i = 0;
                    var gatherFileRuleInfo = new GatherFileRuleInfo(GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), ECharsetUtils.GetEnumType(GetString(rdr, i++)), GetDateTime(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetBool(rdr, i));

                    list.Add(gatherFileRuleInfo);
				}
				rdr.Close();
			}

			return list;
		}

		public List<string> GetGatherRuleNameList(int publishmentSystemId)
		{
			var list = new List<string>();

			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
			};

			using (var rdr = ExecuteReader(SqlSelectGatherFileRuleNameByPsId, parms)) 
			{
				while (rdr.Read()) 
				{					
					list.Add(GetString(rdr, 0));
				}
				rdr.Close();
			}

			return list;
		}

        public void OpenAuto(int publishmentSystemId, List<string> gatherRuleNameCollection)
        {
            string sql =
                $"UPDATE siteserver_GatherFileRule SET IsAutoCreate = 'True' WHERE PublishmentSystemID = {publishmentSystemId} AND GatherRuleName in ({TranslateUtils.ToSqlInStringWithQuote(gatherRuleNameCollection)})";
            ExecuteNonQuery(sql);
        }

        public void CloseAuto(int publishmentSystemId, List<string> gatherRuleNameCollection)
        {
            string sql =
                $"UPDATE siteserver_GatherFileRule SET IsAutoCreate = 'False' WHERE PublishmentSystemID = {publishmentSystemId} AND GatherRuleName in ({TranslateUtils.ToSqlInStringWithQuote(gatherRuleNameCollection)})";
            ExecuteNonQuery(sql);
        }

    }
}
