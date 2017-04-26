using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Provider
{
	public class InnerLinkDao : DataProviderBase
	{
		private const string SqlInsertInnerLink = "INSERT INTO siteserver_InnerLink VALUES (@InnerLinkName, @PublishmentSystemID, @LinkUrl)";
		private const string SqlUpdateInnerLink = "UPDATE siteserver_InnerLink SET LinkUrl = @LinkUrl WHERE InnerLinkName = @InnerLinkName AND PublishmentSystemID = @PublishmentSystemID";
		private const string SqlDeleteInnerLink = "DELETE FROM siteserver_InnerLink WHERE InnerLinkName = @InnerLinkName AND PublishmentSystemID = @PublishmentSystemID";

		private const string ParmInnerLinkName = "@InnerLinkName";
		private const string ParmPublishmentsystemid = "@PublishmentSystemID";
		private const string ParmLinkUrl = "@LinkUrl";

		public void Insert(InnerLinkInfo innerLinkInfo) 
		{
			var insertParms = new IDataParameter[]
			{
				GetParameter(ParmInnerLinkName, EDataType.NVarChar, 255, innerLinkInfo.InnerLinkName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, innerLinkInfo.PublishmentSystemID),
				GetParameter(ParmLinkUrl, EDataType.VarChar, 200, innerLinkInfo.LinkUrl)
			};

            ExecuteNonQuery(SqlInsertInnerLink, insertParms);
		}

        public void Update(InnerLinkInfo innerLinkInfo) 
		{
			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmLinkUrl, EDataType.VarChar, 200, innerLinkInfo.LinkUrl),
				GetParameter(ParmInnerLinkName, EDataType.NVarChar, 255, innerLinkInfo.InnerLinkName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, innerLinkInfo.PublishmentSystemID)
			};

            ExecuteNonQuery(SqlUpdateInnerLink, updateParms);
		}

		public void Delete(string innerLinkName, int publishmentSystemId)
		{
			var innerLinkParms = new IDataParameter[]
			{
				GetParameter(ParmInnerLinkName, EDataType.NVarChar, 255, innerLinkName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};

            ExecuteNonQuery(SqlDeleteInnerLink, innerLinkParms);
		}

		public InnerLinkInfo GetInnerLinkInfo(string innerLinkName, int publishmentSystemId)
		{
			InnerLinkInfo innerLinkInfo = null;

            var sqlString = "SELECT InnerLinkName, PublishmentSystemID, LinkUrl FROM siteserver_InnerLink WHERE InnerLinkName = @InnerLinkName AND PublishmentSystemID = 0";
			if (publishmentSystemId != 0)
			{
                sqlString = "SELECT InnerLinkName, PublishmentSystemID, LinkUrl FROM siteserver_InnerLink WHERE InnerLinkName = @InnerLinkName AND (PublishmentSystemID = 0 OR PublishmentSystemID = @PublishmentSystemID)";
			}
            var selectParms = new IDataParameter[]
			{
				GetParameter(ParmInnerLinkName, EDataType.NVarChar, 255, innerLinkName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)				 
			};
            using (var rdr = ExecuteReader(sqlString, selectParms)) 
			{
				if (rdr.Read())
				{
				    var i = 0;
                    innerLinkInfo = new InnerLinkInfo(GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i));
				}
				rdr.Close();
			}

            return innerLinkInfo;
		}

        public bool IsExists(string innerLinkName, int publishmentSystemId)
        {
            var exists = false;

            var sqlString = "SELECT InnerLinkName, PublishmentSystemID, LinkUrl FROM siteserver_InnerLink WHERE InnerLinkName = @InnerLinkName AND PublishmentSystemID = 0";
            if (publishmentSystemId != 0)
            {
                sqlString = "SELECT InnerLinkName, PublishmentSystemID, LinkUrl FROM siteserver_InnerLink WHERE InnerLinkName = @InnerLinkName AND (PublishmentSystemID = 0 OR PublishmentSystemID = @PublishmentSystemID)";
            }
            var selectParms = new IDataParameter[]
			{
				GetParameter(ParmInnerLinkName, EDataType.NVarChar, 255, innerLinkName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)				 
			};
            using (var rdr = ExecuteReader(sqlString, selectParms))
            {
                if (rdr.Read())
                {
                    exists = true;
                }
                rdr.Close();
            }

            return exists;
        }

		public bool IsExactExists(string innerLinkName, int publishmentSystemId)
		{
			var exists = false;

            var sqlString = "SELECT InnerLinkName, PublishmentSystemID, LinkUrl FROM siteserver_InnerLink WHERE InnerLinkName = @InnerLinkName AND PublishmentSystemID = @PublishmentSystemID";
            var selectParms = new IDataParameter[]
			{
				GetParameter(ParmInnerLinkName, EDataType.NVarChar, 255, innerLinkName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)				 
			};
			using (var rdr = ExecuteReader(sqlString,selectParms)) 
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
			var sqlString = "SELECT InnerLinkName, PublishmentSystemID, LinkUrl FROM siteserver_InnerLink WHERE PublishmentSystemID = 0 ORDER BY PublishmentSystemID, InnerLinkName";
			if (publishmentSystemId != 0)
			{
				sqlString =
				    $"SELECT InnerLinkName, PublishmentSystemID, LinkUrl FROM siteserver_InnerLink WHERE  PublishmentSystemID = 0 OR PublishmentSystemID = {publishmentSystemId} ORDER BY PublishmentSystemID, InnerLinkName";
			}
			var enumerable = (IEnumerable)ExecuteReader(sqlString);
			return enumerable;
		}

		public List<InnerLinkInfo> GetInnerLinkInfoList(int publishmentSystemId)
		{
			var list = new List<InnerLinkInfo>();
			var sqlString = "SELECT InnerLinkName, PublishmentSystemID, LinkUrl FROM siteserver_InnerLink WHERE PublishmentSystemID = 0 ORDER BY PublishmentSystemID, InnerLinkName";
			if (publishmentSystemId != 0)
			{
				sqlString =
				    $"SELECT InnerLinkName, PublishmentSystemID, LinkUrl FROM siteserver_InnerLink WHERE PublishmentSystemID = 0 OR PublishmentSystemID = {publishmentSystemId} ORDER BY PublishmentSystemID, InnerLinkName";
			}

			using (var rdr = ExecuteReader(sqlString)) 
			{
				while (rdr.Read())
				{
				    var i = 0;
                    list.Add(new InnerLinkInfo(GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i)));
                }
				rdr.Close();
			}

			return list;
		}

		public List<string> GetInnerLinkNameList(int publishmentSystemId)
		{
			var list = new List<string>();
			var sqlString = "SELECT InnerLinkName FROM siteserver_InnerLink WHERE PublishmentSystemID = 0";
			if (publishmentSystemId != 0)
			{
				sqlString =
				    $"SELECT InnerLinkName FROM siteserver_InnerLink WHERE PublishmentSystemID = 0 OR PublishmentSystemID = {publishmentSystemId}";
			}
			
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
	}
}
