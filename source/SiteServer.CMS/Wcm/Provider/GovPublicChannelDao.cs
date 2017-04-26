using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.CMS.Wcm.Provider
{
    public class GovPublicChannelDao : DataProviderBase
	{
        private const string SqlInsert = "INSERT INTO wcm_GovPublicChannel (NodeID, PublishmentSystemID, Code, Summary) VALUES (@NodeID, @PublishmentSystemID, @Code, @Summary)";

        private const string SqlDelete = "DELETE FROM wcm_GovPublicChannel WHERE NodeID = @NodeID";

        private const string SqlSelect = "SELECT NodeID, PublishmentSystemID, Code, Summary FROM wcm_GovPublicChannel WHERE NodeID = @NodeID";

        private const string SqlSelectCode = "SELECT Code FROM wcm_GovPublicChannel WHERE NodeID = @NodeID";

        private const string SqlSelectId = "SELECT NodeID FROM wcm_GovPublicChannel WHERE NodeID = @NodeID";

        private const string SqlUpdate = "UPDATE wcm_GovPublicChannel SET Code = @Code, Summary = @Summary WHERE NodeID = @NodeID";

        private const string ParmNodeId = "@NodeID";
        private const string ParmPublishmentsystemid = "@PublishmentSystemID";
        private const string ParmCode = "@Code";
        private const string ParmSummary = "@Summary";

        public void Insert(GovPublicChannelInfo channelInfo)
        {
            var parms = new IDataParameter[]
			{
                GetParameter(ParmNodeId, EDataType.Integer, channelInfo.NodeID),
                GetParameter(ParmPublishmentsystemid, EDataType.Integer, channelInfo.PublishmentSystemID),
                GetParameter(ParmCode, EDataType.VarChar, 50, channelInfo.Code),
				GetParameter(ParmSummary, EDataType.NVarChar, 255, channelInfo.Summary)
			};

            ExecuteNonQuery(SqlInsert, parms);
        }

        public void Update(GovPublicChannelInfo channelInfo)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmCode, EDataType.VarChar, 50, channelInfo.Code),
				GetParameter(ParmSummary, EDataType.NVarChar, 255, channelInfo.Summary),
				GetParameter(ParmNodeId, EDataType.Integer, channelInfo.NodeID)
			};

            ExecuteNonQuery(SqlUpdate, parms);
        }

        public void Delete(int nodeId)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmNodeId, EDataType.Integer, nodeId)
			};

            ExecuteNonQuery(SqlDelete, parms);
        }

        public GovPublicChannelInfo GetChannelInfo(int nodeId)
		{
            GovPublicChannelInfo channelInfo = null;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmNodeId, EDataType.Integer, nodeId)
			};

            using (var rdr = ExecuteReader(SqlSelect, parms)) 
			{
				if (rdr.Read())
				{
				    var i = 0;
                    channelInfo = new GovPublicChannelInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
				}
				rdr.Close();
			}
            return channelInfo;
		}

        public string GetCode(int nodeId)
        {
            var code = string.Empty;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmNodeId, EDataType.Integer, nodeId)
			};

            using (var rdr = ExecuteReader(SqlSelectCode, parms))
            {
                if (rdr.Read())
                {
                    code = GetString(rdr, 0);
                }
                rdr.Close();
            }
            return code;
        }

		public bool IsExists(int nodeId)
		{
			var exists = false;

			var nodeParms = new IDataParameter[]
			{
				GetParameter(ParmNodeId, EDataType.Integer, nodeId)
			};

			using (var rdr = ExecuteReader(SqlSelectId, nodeParms))
			{
				if (rdr.Read())
				{
					if (!rdr.IsDBNull(0))
					{
						exists = true;
					}
				}
				rdr.Close();
			}
			return exists;
		}
	}
}
