using System;
using System.Collections.Generic;
using System.Data;
using SiteServer.CMS.Data;
using SiteServer.Utils.Model;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.CMS.Provider
{
    public class RelatedFieldDao : DataProviderBase
	{
        public override string TableName => "siteserver_RelatedField";

        public override List<TableColumnInfo> TableColumns => new List<TableColumnInfo>
        {
            new TableColumnInfo
            {
                ColumnName = nameof(RelatedFieldInfo.RelatedFieldId),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumnInfo
            {
                ColumnName = nameof(RelatedFieldInfo.RelatedFieldName),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(RelatedFieldInfo.PublishmentSystemId),
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = nameof(RelatedFieldInfo.TotalLevel),
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = nameof(RelatedFieldInfo.Prefixes),
                DataType = DataType.VarChar,
                Length = 255
            },
            new TableColumnInfo
            {
                ColumnName = nameof(RelatedFieldInfo.Suffixes),
                DataType = DataType.VarChar,
                Length = 255
            }
        };

        private const string SqlUpdate = "UPDATE siteserver_RelatedField SET RelatedFieldName = @RelatedFieldName, TotalLevel = @TotalLevel, Prefixes = @Prefixes, Suffixes = @Suffixes WHERE RelatedFieldID = @RelatedFieldID";
        private const string SqlDelete = "DELETE FROM siteserver_RelatedField WHERE RelatedFieldID = @RelatedFieldID";

        private const string ParmRelatedFieldId = "@RelatedFieldID";
        private const string ParmRelatedFieldName = "@RelatedFieldName";
		private const string ParmPublishmentsystemid = "@PublishmentSystemID";
        private const string ParmTotalLevel = "@TotalLevel";
        private const string ParmPrefixes = "@Prefixes";
        private const string ParmSuffixes = "@Suffixes";

		public int Insert(RelatedFieldInfo relatedFieldInfo) 
		{
            const string sqlString = "INSERT INTO siteserver_RelatedField (RelatedFieldName, PublishmentSystemID, TotalLevel, Prefixes, Suffixes) VALUES (@RelatedFieldName, @PublishmentSystemID, @TotalLevel, @Prefixes, @Suffixes)";

			var insertParms = new IDataParameter[]
			{
				GetParameter(ParmRelatedFieldName, DataType.VarChar, 50, relatedFieldInfo.RelatedFieldName),
				GetParameter(ParmPublishmentsystemid, DataType.Integer, relatedFieldInfo.PublishmentSystemId),
                GetParameter(ParmTotalLevel, DataType.Integer, relatedFieldInfo.TotalLevel),
                GetParameter(ParmPrefixes, DataType.VarChar, 255, relatedFieldInfo.Prefixes),
                GetParameter(ParmSuffixes, DataType.VarChar, 255, relatedFieldInfo.Suffixes),
			};

            return ExecuteNonQueryAndReturnId(TableName, nameof(RelatedFieldInfo.RelatedFieldId), sqlString, insertParms);
		}

        public void Update(RelatedFieldInfo relatedFieldInfo) 
		{
			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmRelatedFieldName, DataType.VarChar, 50, relatedFieldInfo.RelatedFieldName),
                GetParameter(ParmTotalLevel, DataType.Integer, relatedFieldInfo.TotalLevel),
                GetParameter(ParmPrefixes, DataType.VarChar, 255, relatedFieldInfo.Prefixes),
                GetParameter(ParmSuffixes, DataType.VarChar, 255, relatedFieldInfo.Suffixes),
				GetParameter(ParmRelatedFieldId, DataType.Integer, relatedFieldInfo.RelatedFieldId)
			};

            ExecuteNonQuery(SqlUpdate, updateParms);
		}

		public void Delete(int relatedFieldId)
		{
			var relatedFieldInfoParms = new IDataParameter[]
			{
				GetParameter(ParmRelatedFieldId, DataType.Integer, relatedFieldId)
			};

            ExecuteNonQuery(SqlDelete, relatedFieldInfoParms);
		}

        public RelatedFieldInfo GetRelatedFieldInfo(int relatedFieldId)
		{
            if (relatedFieldId <= 0) return null;

            RelatedFieldInfo relatedFieldInfo = null;

		    string sqlString =
		        $"SELECT RelatedFieldID, RelatedFieldName, PublishmentSystemID, TotalLevel, Prefixes, Suffixes FROM siteserver_RelatedField WHERE RelatedFieldID = {relatedFieldId}";

		    using (var rdr = ExecuteReader(sqlString))
		    {
		        if (rdr.Read())
		        {
		            var i = 0;
		            relatedFieldInfo = new RelatedFieldInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
		        }
		        rdr.Close();
		    }

		    return relatedFieldInfo;
		}

        public RelatedFieldInfo GetRelatedFieldInfo(int publishmentSystemId, string relatedFieldName)
        {
            RelatedFieldInfo relatedFieldInfo = null;

            string sqlString =
                $"SELECT RelatedFieldID, RelatedFieldName, PublishmentSystemID, TotalLevel, Prefixes, Suffixes FROM siteserver_RelatedField WHERE PublishmentSystemID = {publishmentSystemId} AND RelatedFieldName = @RelatedFieldName";

            var selectParms = new IDataParameter[]
			{
				GetParameter(ParmRelatedFieldName, DataType.VarChar, 255, relatedFieldName)			 
			};

            using (var rdr = ExecuteReader(sqlString, selectParms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    relatedFieldInfo = new RelatedFieldInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

            return relatedFieldInfo;
        }

        public string GetRelatedFieldName(int relatedFieldId)
        {
            var relatedFieldName = string.Empty;

            string sqlString =
                $"SELECT RelatedFieldName FROM siteserver_RelatedField WHERE RelatedFieldID = {relatedFieldId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    relatedFieldName = GetString(rdr, 0);
                }
                rdr.Close();
            }

            return relatedFieldName;
        }

		public List<RelatedFieldInfo> GetRelatedFieldInfoList(int publishmentSystemId)
		{
			var list = new List<RelatedFieldInfo>();
            string sqlString =
                $"SELECT RelatedFieldID, RelatedFieldName, PublishmentSystemID, TotalLevel, Prefixes, Suffixes FROM siteserver_RelatedField WHERE PublishmentSystemID = {publishmentSystemId} ORDER BY RelatedFieldID";

			using (var rdr = ExecuteReader(sqlString)) 
			{
				while (rdr.Read())
				{
				    var i = 0;
                    list.Add(new RelatedFieldInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i)));
				}
				rdr.Close();
			}

			return list;
		}

		public List<string> GetRelatedFieldNameList(int publishmentSystemId)
		{
			var list = new List<string>();
            string sqlString =
                $"SELECT RelatedFieldName FROM siteserver_RelatedField WHERE PublishmentSystemID = {publishmentSystemId} ORDER BY RelatedFieldID";
			
			using (var rdr = ExecuteReader(sqlString)) 
			{
				while (rdr.Read()) 
				{
                    list.Add(GetString(rdr, 0));
				}
				rdr.Close();
			}

			return list;
		}

        public string GetImportRelatedFieldName(int publishmentSystemId, string relatedFieldName)
        {
            string importName;
            if (relatedFieldName.IndexOf("_", StringComparison.Ordinal) != -1)
            {
                var relatedFieldNameCount = 0;
                var lastName = relatedFieldName.Substring(relatedFieldName.LastIndexOf("_", StringComparison.Ordinal) + 1);
                var firstName = relatedFieldName.Substring(0, relatedFieldName.Length - lastName.Length);
                try
                {
                    relatedFieldNameCount = int.Parse(lastName);
                }
                catch
                {
                    // ignored
                }
                relatedFieldNameCount++;
                importName = firstName + relatedFieldNameCount;
            }
            else
            {
                importName = relatedFieldName + "_1";
            }

            var relatedFieldInfo = GetRelatedFieldInfo(publishmentSystemId, relatedFieldName);
            if (relatedFieldInfo != null)
            {
                importName = GetImportRelatedFieldName(publishmentSystemId, importName);
            }

            return importName;
        }
	}
}