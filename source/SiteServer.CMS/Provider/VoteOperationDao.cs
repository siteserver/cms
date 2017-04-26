using System;
using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Provider
{
    public class VoteOperationDao : DataProviderBase
	{
        private const string SqlSelectCount = "SELECT Count(OperationID) AS TotalNum FROM siteserver_VoteOperation WHERE PublishmentSystemID = @PublishmentSystemID AND NodeID = @NodeID AND ContentID = @ContentID";

        private const string SqlSelectAll = "SELECT OperationID, PublishmentSystemID, NodeID, ContentID, IPAddress, UserName, AddDate FROM siteserver_VoteOperation WHERE PublishmentSystemID = @PublishmentSystemID AND NodeID = @NodeID AND ContentID = @ContentID ORDER BY AddDate DESC";

        private const string SqlSelectByUserName = "SELECT OperationID FROM siteserver_VoteOperation WHERE PublishmentSystemID = @PublishmentSystemID AND NodeID = @NodeID AND ContentID = @ContentID AND UserName = @UserName";

        private const string SqlSelectByIpaddress = "SELECT OperationID FROM siteserver_VoteOperation WHERE PublishmentSystemID = @PublishmentSystemID AND NodeID = @NodeID AND ContentID = @ContentID AND IPAddress = @IPAddress";

        private const string ParmPublishmentsystemId = "@PublishmentSystemID";
        private const string ParmNodeId = "@NodeID";
        private const string ParmContentId = "@ContentID";
		private const string ParmIpAddress = "@IPAddress";
        private const string ParmUserName = "@UserName";
        private const string ParmAddDate = "@AddDate";

		public void Insert(VoteOperationInfo operationInfo)
		{
            var sqlString = "INSERT INTO siteserver_VoteOperation (PublishmentSystemID, NodeID, ContentID, IPAddress, UserName, AddDate) VALUES (@PublishmentSystemID, @NodeID, @ContentID, @IPAddress, @UserName, @AddDate)";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentsystemId, EDataType.Integer, operationInfo.PublishmentSystemID),
                GetParameter(ParmNodeId, EDataType.Integer, operationInfo.NodeID),
                GetParameter(ParmContentId, EDataType.Integer, operationInfo.ContentID),
				GetParameter(ParmIpAddress, EDataType.VarChar, 50, operationInfo.IPAddress),
                GetParameter(ParmUserName, EDataType.NVarChar, 255, operationInfo.UserName),
				GetParameter(ParmAddDate, EDataType.DateTime, operationInfo.AddDate)
			};

            ExecuteNonQuery(sqlString, parms);
		}

		public int GetCount(int publishmentSystemId, int nodeId, int contentId)
		{
			var count = 0;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentsystemId, EDataType.Integer, publishmentSystemId),
                GetParameter(ParmNodeId, EDataType.Integer, nodeId),
                GetParameter(ParmContentId, EDataType.Integer, contentId)
			};

            using (var rdr = ExecuteReader(SqlSelectCount, parms))
			{
				if (rdr.Read())
				{
                    count = GetInt(rdr, 0);
                }
				rdr.Close();
			}
            return count;
		}

        public bool IsUserExists(int publishmentSystemId, int nodeId, int contentId, string userName)
        {
            var isExists = false;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentsystemId, EDataType.Integer, publishmentSystemId),
                GetParameter(ParmNodeId, EDataType.Integer, nodeId),
                GetParameter(ParmContentId, EDataType.Integer, contentId),
                GetParameter(ParmUserName, EDataType.NVarChar, 255, userName)
			};

            using (var rdr = ExecuteReader(SqlSelectByUserName, parms))
            {
                if (rdr.Read())
                {
                    if (!rdr.IsDBNull(0))
                    {
                        isExists = true;
                    }
                }
                rdr.Close();
            }
            return isExists;
        }

        public bool IsIpAddressExists(int publishmentSystemId, int nodeId, int contentId, string ipAddress)
        {
            var isExists = false;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentsystemId, EDataType.Integer, publishmentSystemId),
                GetParameter(ParmNodeId, EDataType.Integer, nodeId),
                GetParameter(ParmContentId, EDataType.Integer, contentId),
                GetParameter(ParmIpAddress, EDataType.VarChar, 50, ipAddress)
			};

            using (var rdr = ExecuteReader(SqlSelectByIpaddress, parms))
            {
                if (rdr.Read())
                {
                    if (!rdr.IsDBNull(0))
                    {
                        isExists = true;
                    }
                }
                rdr.Close();
            }
            return isExists;
        }

        public DataSet GetDataSet(int publishmentSystemId, int nodeId, int contentId)
		{
			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentsystemId, EDataType.Integer, publishmentSystemId),
                GetParameter(ParmNodeId, EDataType.Integer, nodeId),
                GetParameter(ParmContentId, EDataType.Integer, contentId)
			};

			var dataset = ExecuteDataset(SqlSelectAll, parms);
			return dataset;
		}

        public string GetCookieName(int publishmentSystemId, int nodeId, int contentId)
        {
            return $"SiteServer_CMS_Vote_{publishmentSystemId}_{nodeId}_{contentId}";
        }
	}
}
