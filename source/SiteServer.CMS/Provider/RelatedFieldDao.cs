using System;
using System.Collections;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Provider
{
    public class RelatedFieldDao : DataProviderBase
	{
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
            int relatedFieldId;

            var sqlString = "INSERT INTO siteserver_RelatedField (RelatedFieldName, PublishmentSystemID, TotalLevel, Prefixes, Suffixes) VALUES (@RelatedFieldName, @PublishmentSystemID, @TotalLevel, @Prefixes, @Suffixes)";

			var insertParms = new IDataParameter[]
			{
				GetParameter(ParmRelatedFieldName, EDataType.NVarChar, 50, relatedFieldInfo.RelatedFieldName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, relatedFieldInfo.PublishmentSystemID),
                GetParameter(ParmTotalLevel, EDataType.Integer, relatedFieldInfo.TotalLevel),
                GetParameter(ParmPrefixes, EDataType.NVarChar, 255, relatedFieldInfo.Prefixes),
                GetParameter(ParmSuffixes, EDataType.NVarChar, 255, relatedFieldInfo.Suffixes),
			};

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        relatedFieldId = ExecuteNonQueryAndReturnId(trans, sqlString, insertParms);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return relatedFieldId;
		}

        public void Update(RelatedFieldInfo relatedFieldInfo) 
		{
			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmRelatedFieldName, EDataType.NVarChar, 50, relatedFieldInfo.RelatedFieldName),
                GetParameter(ParmTotalLevel, EDataType.Integer, relatedFieldInfo.TotalLevel),
                GetParameter(ParmPrefixes, EDataType.NVarChar, 255, relatedFieldInfo.Prefixes),
                GetParameter(ParmSuffixes, EDataType.NVarChar, 255, relatedFieldInfo.Suffixes),
				GetParameter(ParmRelatedFieldId, EDataType.Integer, relatedFieldInfo.RelatedFieldID)
			};

            ExecuteNonQuery(SqlUpdate, updateParms);
		}

		public void Delete(int relatedFieldId)
		{
			var relatedFieldInfoParms = new IDataParameter[]
			{
				GetParameter(ParmRelatedFieldId, EDataType.Integer, relatedFieldId)
			};

            ExecuteNonQuery(SqlDelete, relatedFieldInfoParms);
		}

        public RelatedFieldInfo GetRelatedFieldInfo(int relatedFieldId)
		{
			RelatedFieldInfo relatedFieldInfo = null;

            if (relatedFieldId > 0)
            {
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
				GetParameter(ParmRelatedFieldName, EDataType.NVarChar, 255, relatedFieldName)			 
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

		public IEnumerable GetDataSource(int publishmentSystemId)
		{
            string sqlString =
                $"SELECT RelatedFieldID, RelatedFieldName, PublishmentSystemID, TotalLevel, Prefixes, Suffixes FROM siteserver_RelatedField WHERE PublishmentSystemID = {publishmentSystemId} ORDER BY RelatedFieldID";
			var enumerable = (IEnumerable)ExecuteReader(sqlString);
			return enumerable;
		}

		public ArrayList GetRelatedFieldInfoArrayList(int publishmentSystemId)
		{
			var arraylist = new ArrayList();
            string sqlString =
                $"SELECT RelatedFieldID, RelatedFieldName, PublishmentSystemID, TotalLevel, Prefixes, Suffixes FROM siteserver_RelatedField WHERE PublishmentSystemID = {publishmentSystemId} ORDER BY RelatedFieldID";

			using (var rdr = ExecuteReader(sqlString)) 
			{
				while (rdr.Read())
				{
				    var i = 0;
                    arraylist.Add(new RelatedFieldInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i)));
				}
				rdr.Close();
			}

			return arraylist;
		}

		public ArrayList GetRelatedFieldNameArrayList(int publishmentSystemId)
		{
			var arraylist = new ArrayList();
            string sqlString =
                $"SELECT RelatedFieldName FROM siteserver_RelatedField WHERE PublishmentSystemID = {publishmentSystemId} ORDER BY RelatedFieldID";
			
			using (var rdr = ExecuteReader(sqlString)) 
			{
				while (rdr.Read()) 
				{
                    arraylist.Add(GetString(rdr, 0));
				}
				rdr.Close();
			}

			return arraylist;
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