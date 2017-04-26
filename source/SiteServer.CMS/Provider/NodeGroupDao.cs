using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Provider
{
	public class NodeGroupDao : DataProviderBase
	{
        private const string SqlInsertNodegroup = "INSERT INTO siteserver_NodeGroup (NodeGroupName, PublishmentSystemID, Taxis, Description) VALUES (@NodeGroupName, @PublishmentSystemID, @Taxis, @Description)";
		private const string SqlUpdateNodegroup = "UPDATE siteserver_NodeGroup SET Description = @Description WHERE NodeGroupName = @NodeGroupName AND PublishmentSystemID = @PublishmentSystemID";
		private const string SqlDeleteNodegroup = "DELETE FROM siteserver_NodeGroup WHERE NodeGroupName = @NodeGroupName AND PublishmentSystemID = @PublishmentSystemID";

		private const string ParmGroupName = "@NodeGroupName";
		private const string ParmPublishmentsystemid = "@PublishmentSystemID";
        private const string ParmTaxis = "@Taxis";
		private const string ParmDescription = "@Description";


		public void Insert(NodeGroupInfo nodeGroup) 
		{
            var maxTaxis = GetMaxTaxis(nodeGroup.PublishmentSystemID);
            nodeGroup.Taxis = maxTaxis + 1;

			var insertParms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, EDataType.NVarChar, 255, nodeGroup.NodeGroupName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, nodeGroup.PublishmentSystemID),
                GetParameter(ParmTaxis, EDataType.Integer, nodeGroup.Taxis),
				GetParameter(ParmDescription, EDataType.NText, nodeGroup.Description)
			};

            ExecuteNonQuery(SqlInsertNodegroup, insertParms);
		}

		public void Update(NodeGroupInfo nodeGroup) 
		{
			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmDescription, EDataType.NText, nodeGroup.Description),
				GetParameter(ParmGroupName, EDataType.NVarChar, 255, nodeGroup.NodeGroupName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, nodeGroup.PublishmentSystemID)
			};

            ExecuteNonQuery(SqlUpdateNodegroup, updateParms);
		}

        public void Delete(int publishmentSystemId, string groupName)
		{
			var nodeGroupParms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, EDataType.NVarChar, 255, groupName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};

            ExecuteNonQuery(SqlDeleteNodegroup, nodeGroupParms);

		    var nodeIdList = DataProvider.NodeDao.GetNodeIdListByGroupName(publishmentSystemId, groupName);
		    foreach (var nodeId in nodeIdList)
		    {
		        var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, nodeId);
		        var groupNameList = TranslateUtils.StringCollectionToStringList(nodeInfo.NodeGroupNameCollection);
		        groupNameList.Remove(groupName);
		        nodeInfo.NodeGroupNameCollection = TranslateUtils.ObjectCollectionToString(groupNameList);
                DataProvider.NodeDao.UpdateNodeInfo(nodeInfo);
		    }
		}

        public NodeGroupInfo GetNodeGroupInfo(int publishmentSystemId, string groupName)
		{
			NodeGroupInfo nodeGroup = null;

            const string sqlString = "SELECT NodeGroupName, PublishmentSystemID, Taxis, Description FROM siteserver_NodeGroup WHERE NodeGroupName = @NodeGroupName AND PublishmentSystemID = @PublishmentSystemID";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, EDataType.NVarChar, 255, groupName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};
			
			using (var rdr = ExecuteReader(sqlString, parms))
			{
				if (rdr.Read())
				{
				    var i = 0;
                    nodeGroup = new NodeGroupInfo(GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i));
				}
				rdr.Close();
			}

			return nodeGroup;
		}

        public bool IsExists(int publishmentSystemId, string groupName)
		{
			var exists = false;

            var sqlString = "SELECT NodeGroupName FROM siteserver_NodeGroup WHERE NodeGroupName = @NodeGroupName AND PublishmentSystemID = @PublishmentSystemID";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, EDataType.NVarChar, 255, groupName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};
			
			using (var rdr = ExecuteReader(sqlString, parms)) 
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
            string sqlString =
                $"SELECT NodeGroupName, PublishmentSystemID, Taxis, Description FROM siteserver_NodeGroup WHERE PublishmentSystemID = {publishmentSystemId} ORDER BY Taxis DESC, NodeGroupName";
			var enumerable = (IEnumerable)ExecuteReader(sqlString);
			return enumerable;
		}

		public List<NodeGroupInfo> GetNodeGroupInfoList(int publishmentSystemId)
		{
			var list = new List<NodeGroupInfo>();
            string sqlString =
                $"SELECT NodeGroupName, PublishmentSystemID, Taxis, Description FROM siteserver_NodeGroup WHERE PublishmentSystemID = {publishmentSystemId} ORDER BY Taxis DESC, NodeGroupName";

			using (var rdr = ExecuteReader(sqlString)) 
			{
				while (rdr.Read())
				{
				    var i = 0;
                    list.Add(new NodeGroupInfo(GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i)));
				}
				rdr.Close();
			}

			return list;
		}

		public List<string> GetNodeGroupNameList(int publishmentSystemId)
		{
			var list = new List<string>();
            string sqlString =
                $"SELECT NodeGroupName FROM siteserver_NodeGroup WHERE PublishmentSystemID = {publishmentSystemId} ORDER BY Taxis DESC, NodeGroupName";
			
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

		//得到属于此发布系统和栏目组的所有栏目
		public List<NodeInfo> GetNodeInfoListChecked(int publishmentSystemId, string nodeGroupName)
		{
		    nodeGroupName = PageUtils.FilterSql(nodeGroupName);

            string whereString =
                $" AND (siteserver_Node.NodeGroupNameCollection = '{nodeGroupName}' OR siteserver_Node.NodeGroupNameCollection LIKE '{nodeGroupName},%' OR siteserver_Node.NodeGroupNameCollection LIKE '%,{nodeGroupName},%' OR siteserver_Node.NodeGroupNameCollection LIKE '%,{nodeGroupName}')";
			return DataProvider.NodeDao.GetNodeInfoListByPublishmentSystemId(publishmentSystemId, whereString);
		}

        private int GetTaxis(int publishmentSystemId, string groupName)
        {
            var sqlString = "SELECT Taxis FROM siteserver_NodeGroup WHERE (NodeGroupName = @NodeGroupName AND PublishmentSystemID = @PublishmentSystemID)";
            var parms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, EDataType.NVarChar, 255, groupName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString, parms);
        }

        private void SetTaxis(int publishmentSystemId, string groupName, int taxis)
        {
            string sqlString =
                $"UPDATE siteserver_NodeGroup SET Taxis = {taxis} WHERE (NodeGroupName = @NodeGroupName AND PublishmentSystemID = @PublishmentSystemID)";
            var parms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, EDataType.NVarChar, 255, groupName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};
            ExecuteNonQuery(sqlString, parms);
        }

        private int GetMaxTaxis(int publishmentSystemId)
        {
            string sqlString =
                $"SELECT MAX(Taxis) FROM siteserver_NodeGroup WHERE (PublishmentSystemID = {publishmentSystemId})";
            var maxTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    maxTaxis = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return maxTaxis;
        }

        public bool UpdateTaxisToUp(int publishmentSystemId, string groupName)
        {
            //Get Higher Taxis and ID
            //var sqlString = "SELECT TOP 1 NodeGroupName, Taxis FROM siteserver_NodeGroup WHERE (Taxis > (SELECT Taxis FROM siteserver_NodeGroup WHERE NodeGroupName = @NodeGroupName AND PublishmentSystemID = @PublishmentSystemID) AND PublishmentSystemID = @PublishmentSystemID) ORDER BY Taxis";
            var sqlString = SqlUtils.GetTopSqlString("siteserver_NodeGroup", "NodeGroupName, Taxis", "WHERE (Taxis > (SELECT Taxis FROM siteserver_NodeGroup WHERE NodeGroupName = @NodeGroupName AND PublishmentSystemID = @PublishmentSystemID) AND PublishmentSystemID = @PublishmentSystemID) ORDER BY Taxis", 1);

            var higherGroupName = string.Empty;
            var higherTaxis = 0;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, EDataType.NVarChar, 255, groupName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    higherGroupName = GetString(rdr, 0);
                    higherTaxis = GetInt(rdr, 1);
                }
                rdr.Close();
            }

            if (!string.IsNullOrEmpty(higherGroupName))
            {
                //Get Taxis Of Selected ID
                var selectedTaxis = GetTaxis(publishmentSystemId, groupName);

                //Set The Selected Class Taxis To Higher Level
                SetTaxis(publishmentSystemId, groupName, higherTaxis);
                //Set The Higher Class Taxis To Lower Level
                SetTaxis(publishmentSystemId, higherGroupName, selectedTaxis);
                return true;
            }
            return false;
        }

        public bool UpdateTaxisToDown(int publishmentSystemId, string groupName)
        {
            //Get Lower Taxis and ID
            //var sqlString = "SELECT TOP 1 NodeGroupName, Taxis FROM siteserver_NodeGroup WHERE (Taxis < (SELECT Taxis FROM siteserver_NodeGroup WHERE NodeGroupName = @NodeGroupName AND PublishmentSystemID = @PublishmentSystemID) AND PublishmentSystemID = @PublishmentSystemID) ORDER BY Taxis DESC";
            var sqlString = SqlUtils.GetTopSqlString("siteserver_NodeGroup", "NodeGroupName, Taxis", "WHERE (Taxis < (SELECT Taxis FROM siteserver_NodeGroup WHERE NodeGroupName = @NodeGroupName AND PublishmentSystemID = @PublishmentSystemID) AND PublishmentSystemID = @PublishmentSystemID) ORDER BY Taxis DESC", 1);

            var lowerGroupName = string.Empty;
            var lowerTaxis = 0;
            var parms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, EDataType.NVarChar, 255, groupName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};
            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    lowerGroupName = GetString(rdr, 0);
                    lowerTaxis = GetInt(rdr, 1);
                }
                rdr.Close();
            }

            if (!string.IsNullOrEmpty(lowerGroupName))
            {
                //Get Taxis Of Selected Class
                var selectedTaxis = GetTaxis(publishmentSystemId, groupName);

                //Set The Selected Class Taxis To Lower Level
                SetTaxis(publishmentSystemId, groupName, lowerTaxis);
                //Set The Lower Class Taxis To Higher Level
                SetTaxis(publishmentSystemId, lowerGroupName, selectedTaxis);
                return true;
            }
            return false;
        }
	}
}
