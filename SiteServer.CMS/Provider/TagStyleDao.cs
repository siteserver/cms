using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Provider
{
    public class TagStyleDao : DataProviderBase
	{
        public override string TableName => "siteserver_TagStyle";

        public override List<TableColumnInfo> TableColumns => new List<TableColumnInfo>
        {
            new TableColumnInfo
            {
                ColumnName = nameof(TagStyleInfo.StyleId),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TagStyleInfo.StyleName),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TagStyleInfo.ElementName),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TagStyleInfo.PublishmentSystemId),
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TagStyleInfo.IsTemplate),
                DataType = DataType.VarChar,
                Length = 18
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TagStyleInfo.StyleTemplate),
                DataType = DataType.Text
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TagStyleInfo.ScriptTemplate),
                DataType = DataType.Text
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TagStyleInfo.ContentTemplate),
                DataType = DataType.Text
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TagStyleInfo.SuccessTemplate),
                DataType = DataType.Text
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TagStyleInfo.FailureTemplate),
                DataType = DataType.Text
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TagStyleInfo.SettingsXml),
                DataType = DataType.Text
            }
        };

        private const string SqlUpdate = "UPDATE siteserver_TagStyle SET StyleName = @StyleName, IsTemplate = @IsTemplate, StyleTemplate = @StyleTemplate, ScriptTemplate = @ScriptTemplate, ContentTemplate = @ContentTemplate, SuccessTemplate = @SuccessTemplate, FailureTemplate = @FailureTemplate, SettingsXML = @SettingsXML WHERE StyleID = @StyleID";

        private const string SqlDelete = "DELETE FROM siteserver_TagStyle WHERE StyleID = @StyleID";

        private const string SqlSelect = "SELECT StyleID, StyleName, ElementName, PublishmentSystemID, IsTemplate, StyleTemplate, ScriptTemplate, ContentTemplate, SuccessTemplate, FailureTemplate, SettingsXML FROM siteserver_TagStyle WHERE StyleID = @StyleID";

        private const string SqlSelectAllByElementName = "SELECT StyleID, StyleName, ElementName, PublishmentSystemID, IsTemplate, StyleTemplate, ScriptTemplate, ContentTemplate, SuccessTemplate, FailureTemplate, SettingsXML FROM siteserver_TagStyle WHERE PublishmentSystemID = @PublishmentSystemID AND ElementName = @ElementName ORDER BY StyleID";

        private const string SqlSelectAll = "SELECT StyleID, StyleName, ElementName, PublishmentSystemID, IsTemplate, StyleTemplate, ScriptTemplate, ContentTemplate, SuccessTemplate, FailureTemplate, SettingsXML FROM siteserver_TagStyle WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY StyleID";

        private const string SqlSelectStyleName = "SELECT StyleName FROM siteserver_TagStyle WHERE PublishmentSystemID = @PublishmentSystemID AND ElementName = @ElementName";

        private const string ParmStyleId = "@StyleID";
        private const string ParmStyleName = "@StyleName";
        private const string ParmElementName = "@ElementName";
		private const string ParmPublishmentsystemid = "@PublishmentSystemID";
        private const string ParmIstemplate = "@IsTemplate";
        private const string ParmStyleTemplate = "@StyleTemplate";
        private const string ParmScriptTemplate = "@ScriptTemplate";
        private const string ParmContentTemplate = "@ContentTemplate";
        private const string ParmSuccessTemplate = "@SuccessTemplate";
        private const string ParmFailureTemplate = "@FailureTemplate";
        private const string ParmSettingsXml = "@SettingsXML";

		public int Insert(TagStyleInfo tagStyleInfo) 
		{
            const string sqlString = "INSERT INTO siteserver_TagStyle (StyleName, ElementName, PublishmentSystemID, IsTemplate, StyleTemplate, ScriptTemplate, ContentTemplate, SuccessTemplate, FailureTemplate, SettingsXML) VALUES (@StyleName, @ElementName, @PublishmentSystemID, @IsTemplate, @StyleTemplate, @ScriptTemplate, @ContentTemplate, @SuccessTemplate, @FailureTemplate, @SettingsXML)";

			var parms = new IDataParameter[]
			{
				GetParameter(ParmStyleName, DataType.VarChar, 50, tagStyleInfo.StyleName),
                GetParameter(ParmElementName, DataType.VarChar, 50, tagStyleInfo.ElementName),
				GetParameter(ParmPublishmentsystemid, DataType.Integer, tagStyleInfo.PublishmentSystemId),
                GetParameter(ParmIstemplate, DataType.VarChar, 18, tagStyleInfo.IsTemplate.ToString()),
                GetParameter(ParmStyleTemplate, DataType.Text, tagStyleInfo.StyleTemplate),
                GetParameter(ParmScriptTemplate, DataType.Text, tagStyleInfo.ScriptTemplate),
                GetParameter(ParmContentTemplate, DataType.Text, tagStyleInfo.ContentTemplate),
                GetParameter(ParmSuccessTemplate, DataType.Text, tagStyleInfo.SuccessTemplate),
                GetParameter(ParmFailureTemplate, DataType.Text, tagStyleInfo.FailureTemplate),
                GetParameter(ParmSettingsXml, DataType.Text, tagStyleInfo.SettingsXml)
			};

            return ExecuteNonQueryAndReturnId(TableName, nameof(TagStyleInfo.StyleId), sqlString, parms);
		}

        public void Update(TagStyleInfo tagStyleInfo) 
		{
			var parms = new IDataParameter[]
			{
				GetParameter(ParmStyleName, DataType.VarChar, 50, tagStyleInfo.StyleName),
                GetParameter(ParmIstemplate, DataType.VarChar, 18, tagStyleInfo.IsTemplate.ToString()),
                GetParameter(ParmStyleTemplate, DataType.Text, tagStyleInfo.StyleTemplate),
                GetParameter(ParmScriptTemplate, DataType.Text, tagStyleInfo.ScriptTemplate),
                GetParameter(ParmContentTemplate, DataType.Text, tagStyleInfo.ContentTemplate),
                GetParameter(ParmSuccessTemplate, DataType.Text, tagStyleInfo.SuccessTemplate),
                GetParameter(ParmFailureTemplate, DataType.Text, tagStyleInfo.FailureTemplate),
                GetParameter(ParmSettingsXml, DataType.Text, tagStyleInfo.SettingsXml),
				GetParameter(ParmStyleId, DataType.Integer, tagStyleInfo.StyleId)
			};

            ExecuteNonQuery(SqlUpdate, parms);

            TagStyleManager.RemoveCache(tagStyleInfo.PublishmentSystemId, tagStyleInfo.ElementName, tagStyleInfo.StyleName);
		}

		public void Delete(int styleId)
		{
			var parms = new IDataParameter[]
			{
				GetParameter(ParmStyleId, DataType.Integer, styleId)
			};

            ExecuteNonQuery(SqlDelete, parms);
		}

        public TagStyleInfo GetTagStyleInfo(int styleId)
		{
            TagStyleInfo tagStyleInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmStyleId, DataType.Integer, styleId)
			};

            using (var rdr = ExecuteReader(SqlSelect, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    tagStyleInfo = new TagStyleInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

            return tagStyleInfo;
		}

        public TagStyleInfo GetTagStyleInfo(int publishmentSystemId, string elementName, string styleName)
        {
            TagStyleInfo tagStyleInfo = null;

            var sqlString = "SELECT StyleID, StyleName, ElementName, PublishmentSystemID, IsTemplate, StyleTemplate, ScriptTemplate, ContentTemplate, SuccessTemplate, FailureTemplate, SettingsXML FROM siteserver_TagStyle WHERE PublishmentSystemID = @PublishmentSystemID AND ElementName = @ElementName AND StyleName = @StyleName";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishmentsystemid, DataType.Integer, publishmentSystemId),
                GetParameter(ParmElementName, DataType.VarChar, 50, elementName),
                GetParameter(ParmStyleName, DataType.VarChar, 50, styleName)
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    tagStyleInfo = new TagStyleInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

            return tagStyleInfo;
        }

        public ArrayList GetTagStyleInfoArrayList(int publishmentSystemId)
        {
            var arraylist = new ArrayList();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentsystemid, DataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(SqlSelectAll, parms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var tagStyleInfo = new TagStyleInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                    arraylist.Add(tagStyleInfo);
                }
                rdr.Close();
            }

            return arraylist;
        }

        public IEnumerable GetDataSource(int publishmentSystemId, string elementName)
		{
            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentsystemid, DataType.Integer, publishmentSystemId),
                GetParameter(ParmElementName, DataType.VarChar, 50, elementName)
			};
            return (IEnumerable)ExecuteReader(SqlSelectAllByElementName, parms);
		}

        public ArrayList GetStyleNameArrayList(int publishmentSystemId, string elementName)
		{
			var arraylist = new ArrayList();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentsystemid, DataType.Integer, publishmentSystemId),
                GetParameter(ParmElementName, DataType.VarChar, 50, elementName)
			};

            using (var rdr = ExecuteReader(SqlSelectStyleName, parms)) 
			{
				while (rdr.Read()) 
				{
                    arraylist.Add(GetString(rdr, 0));
				}
				rdr.Close();
			}

			return arraylist;
		}

        public string GetImportStyleName(int publishmentSystemId, string elementName, string styleName)
        {
            string importStyleName;
            if (styleName.IndexOf("_", StringComparison.Ordinal) != -1)
            {
                var styleNameCount = 0;
                var lastStyleName = styleName.Substring(styleName.LastIndexOf("_", StringComparison.Ordinal) + 1);
                var firstStyleName = styleName.Substring(0, styleName.Length - lastStyleName.Length);
                try
                {
                    styleNameCount = int.Parse(lastStyleName);
                }
                catch
                {
                    // ignored
                }
                styleNameCount++;
                importStyleName = firstStyleName + styleNameCount;
            }
            else
            {
                importStyleName = styleName + "_1";
            }

            var styleInfo = GetTagStyleInfo(publishmentSystemId, elementName, importStyleName);

            if (styleInfo != null)
            {
                importStyleName = GetImportStyleName(publishmentSystemId, elementName, importStyleName);
            }

            return importStyleName;
        }
	}
}