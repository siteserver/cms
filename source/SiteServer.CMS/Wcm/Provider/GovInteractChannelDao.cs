using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.Wcm.GovInteract;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.CMS.Wcm.Provider
{
    public class GovInteractChannelDao : DataProviderBase
    {
        private const string SqlInsert = "INSERT INTO wcm_GovInteractChannel (NodeID, PublishmentSystemID, ApplyStyleID, QueryStyleID, DepartmentIDCollection, Summary) VALUES (@NodeID, @PublishmentSystemID, @ApplyStyleID, @QueryStyleID, @DepartmentIDCollection, @Summary)";

        private const string SqlDelete = "DELETE FROM wcm_GovInteractChannel WHERE NodeID = @NodeID";

        private const string SqlSelect = "SELECT NodeID, PublishmentSystemID, ApplyStyleID, QueryStyleID, DepartmentIDCollection, Summary FROM wcm_GovInteractChannel WHERE NodeID = @NodeID";

        private const string SqlSelectId = "SELECT NodeID FROM wcm_GovInteractChannel WHERE NodeID = @NodeID";

        private const string SqlUpdate = "UPDATE wcm_GovInteractChannel SET DepartmentIDCollection = @DepartmentIDCollection, Summary = @Summary WHERE NodeID = @NodeID";

        private const string ParmNodeId = "@NodeID";
        private const string ParmPublishmentsystemid = "@PublishmentSystemID";
        private const string ParmApplyStyleId = "@ApplyStyleID";
        private const string ParmQueryStyleId = "@QueryStyleID";
        private const string ParmDepartmentidCollection = "@DepartmentIDCollection";
        private const string ParmSummary = "@Summary";

        public void Insert(GovInteractChannelInfo channelInfo)
        {
            channelInfo.ApplyStyleID = DataProvider.TagStyleDao.Insert(new TagStyleInfo(0, channelInfo.NodeID.ToString(), StlGovInteractApply.ElementName, channelInfo.PublishmentSystemID, false, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
            channelInfo.QueryStyleID = DataProvider.TagStyleDao.Insert(new TagStyleInfo(0, channelInfo.NodeID.ToString(), StlGovInteractQuery.ElementName, channelInfo.PublishmentSystemID, false, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));

            var parms = new IDataParameter[]
			{
                GetParameter(ParmNodeId, EDataType.Integer, channelInfo.NodeID),
                GetParameter(ParmPublishmentsystemid, EDataType.Integer, channelInfo.PublishmentSystemID),
                GetParameter(ParmApplyStyleId, EDataType.Integer, channelInfo.ApplyStyleID),
                GetParameter(ParmQueryStyleId, EDataType.Integer, channelInfo.QueryStyleID),
                GetParameter(ParmDepartmentidCollection, EDataType.NVarChar, 255, channelInfo.DepartmentIDCollection),
				GetParameter(ParmSummary, EDataType.NVarChar, 255, channelInfo.Summary)
			};

            ExecuteNonQuery(SqlInsert, parms);

            GovInteractManager.AddDefaultTypeInfos(channelInfo.PublishmentSystemID, channelInfo.NodeID);
        }

        public void Update(GovInteractChannelInfo channelInfo)
        {
            var parms = new IDataParameter[]
			{
                GetParameter(ParmDepartmentidCollection, EDataType.NVarChar, 255, channelInfo.DepartmentIDCollection),
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

        public GovInteractChannelInfo GetChannelInfo(int publishmentSystemId, int nodeId)
        {
            GovInteractChannelInfo channelInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmNodeId, EDataType.Integer, nodeId)
			};

            using (var rdr = ExecuteReader(SqlSelect, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    channelInfo = new GovInteractChannelInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

            if (channelInfo == null)
            {
                var theChannelInfo = new GovInteractChannelInfo(nodeId, publishmentSystemId, 0, 0, string.Empty, string.Empty);
                DataProvider.GovInteractChannelDao.Insert(theChannelInfo);

                using (var rdr = ExecuteReader(SqlSelect, parms))
                {
                    if (rdr.Read())
                    {
                        var i = 0;
                        channelInfo = new GovInteractChannelInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                    }
                    rdr.Close();
                }
            }

            return channelInfo;
        }

        public string GetSummary(int nodeId)
        {
            string sqlString = $"SELECT Summary FROM wcm_GovInteractChannel WHERE NodeID = {nodeId}";
            return BaiRongDataProvider.DatabaseDao.GetString(sqlString);
        }

        public int GetApplyStyleId(int publishmentSystemId, int nodeId)
        {
            string sqlString = $"SELECT ApplyStyleID FROM wcm_GovInteractChannel WHERE NodeID = {nodeId}";
            var styleId = BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);

            if (styleId == 0)
            {
                var theChannelInfo = new GovInteractChannelInfo(nodeId, publishmentSystemId, 0, 0, string.Empty, string.Empty);
                DataProvider.GovInteractChannelDao.Insert(theChannelInfo);

                styleId = BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
            }

            return styleId;
        }

        public int GetQueryStyleId(int publishmentSystemId, int nodeId)
        {
            string sqlString = $"SELECT QueryStyleID FROM wcm_GovInteractChannel WHERE NodeID = {nodeId}";
            var styleId = BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
            if (styleId == 0)
            {
                var theChannelInfo = new GovInteractChannelInfo(nodeId, publishmentSystemId, 0, 0, string.Empty, string.Empty);
                DataProvider.GovInteractChannelDao.Insert(theChannelInfo);

                styleId = BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
            }
            return styleId;
        }

        public int GetNodeIdByInteractName(int publishmentSystemId, string interactName)
        {
            string sqlString =
                $"SELECT wcm_GovInteractChannel.NodeID FROM wcm_GovInteractChannel INNER JOIN siteserver_Node ON wcm_GovInteractChannel.NodeID = siteserver_Node.NodeID and siteserver_Node.PublishmentSystemID = {publishmentSystemId} AND siteserver_Node.ContentModelID = '{EContentModelTypeUtils.GetValue(EContentModelType.GovInteract)}' AND siteserver_Node.NodeName = @NodeName";

            var selectParms = new IDataParameter[]
			{
				GetParameter("@NodeName", EDataType.NVarChar,255, interactName)
			};

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString, selectParms);
        }

        public int GetNodeIdByApplyStyleId(int applyStyleId)
        {
            string sqlString = $"SELECT NodeID FROM wcm_GovInteractChannel WHERE ApplyStyleID = {applyStyleId}";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetNodeIdByQueryStyleId(int queryStyleId)
        {
            string sqlString = $"SELECT NodeID FROM wcm_GovInteractChannel WHERE QueryStyleID = {queryStyleId}";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
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
