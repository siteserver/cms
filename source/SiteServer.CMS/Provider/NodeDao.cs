using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Provider
{
    public class NodeDao : DataProviderBase
    {
        public string TableName => "siteserver_Node";

        private const string SqlSelectNode = "SELECT NodeID, NodeName, NodeType, PublishmentSystemID, ContentModelID, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, NodeIndexName, NodeGroupNameCollection, Taxis, AddDate, ImageUrl, Content, ContentNum, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateID, ContentTemplateID, Keywords, Description, ExtendValues FROM siteserver_Node WHERE NodeID = @NodeID";

        private const string SqlSelectNodeByLastAddDate = "SELECT NodeID, NodeName, NodeType, PublishmentSystemID, ContentModelID, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, NodeIndexName, NodeGroupNameCollection, Taxis, AddDate, ImageUrl, Content, ContentNum, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateID, ContentTemplateID, Keywords, Description, ExtendValues FROM siteserver_Node WHERE ParentID = @ParentID ORDER BY AddDate Desc";

        private const string SqlSelectNodeId = "SELECT NodeID FROM siteserver_Node WHERE NodeID = @NodeID";

        private const string SqlSelectNodeByTaxis = "SELECT NodeID, NodeName, NodeType, PublishmentSystemID, ContentModelID, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, NodeIndexName, NodeGroupNameCollection, Taxis, AddDate, ImageUrl, Content, ContentNum, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateID, ContentTemplateID, Keywords, Description, ExtendValues FROM siteserver_Node WHERE ParentID = @ParentID ORDER BY Taxis";

        private const string SqlSelectNodeByParentIdAndContentModelId = "SELECT NodeID, NodeName, NodeType, PublishmentSystemID, ContentModelID, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, NodeIndexName, NodeGroupNameCollection, Taxis, AddDate, ImageUrl, Content, ContentNum, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateID, ContentTemplateID, Keywords, Description, ExtendValues FROM siteserver_Node WHERE (PublishmentSystemID = @PublishmentSystemID OR NodeID = @PublishmentSystemID) AND ContentModelID = @ContentModelID";

        private const string SqlSelectNodeGroupNameCollection = "SELECT NodeGroupNameCollection FROM siteserver_Node WHERE NodeID = @NodeID";

        private const string SqlSelectParentId = "SELECT ParentID FROM siteserver_Node WHERE NodeID = @NodeID";

        private const string SqlSelectNodeCount = "SELECT COUNT(*) FROM siteserver_Node WHERE ParentID = @ParentID";

        private const string SqlSelectPublishmentSystemIdById = "SELECT PublishmentSystemID FROM siteserver_Node WHERE NodeID = @NodeID";

        private const string SqlSelectNodeIndexNameCollection = "SELECT DISTINCT NodeIndexName FROM siteserver_Node WHERE PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectNodeIdByIndex = "SELECT NodeID FROM siteserver_Node WHERE (PublishmentSystemID = @PublishmentSystemID OR NodeID = @PublishmentSystemID) AND NodeIndexName = @NodeIndexName";

        private const string SqlSelectNodeIdByContentModelId = "SELECT NodeID FROM siteserver_Node WHERE (PublishmentSystemID = @PublishmentSystemID OR NodeID = @PublishmentSystemID) AND ContentModelID = @ContentModelID";

        private const string SqlUpdateNode = "UPDATE siteserver_Node SET NodeName = @NodeName, NodeType = @NodeType, ContentModelID = @ContentModelID, ParentsPath = @ParentsPath, ParentsCount = @ParentsCount, ChildrenCount = @ChildrenCount, IsLastNode = @IsLastNode, NodeIndexName = @NodeIndexName, NodeGroupNameCollection = @NodeGroupNameCollection, ImageUrl = @ImageUrl, Content = @Content, ContentNum = @ContentNum, FilePath = @FilePath, ChannelFilePathRule = @ChannelFilePathRule, ContentFilePathRule = @ContentFilePathRule, LinkUrl = @LinkUrl,LinkType = @LinkType, ChannelTemplateID = @ChannelTemplateID, ContentTemplateID = @ContentTemplateID, Keywords = @Keywords, Description = @Description, ExtendValues = @ExtendValues WHERE NodeID = @NodeID";

        private const string SqlUpdateExtendValues = "UPDATE siteserver_Node SET ExtendValues = @ExtendValues WHERE NodeID = @NodeID";

        private const string SqlUpdateNodeGroupNameCollection = "UPDATE siteserver_Node SET NodeGroupNameCollection = @NodeGroupNameCollection WHERE NodeID = @NodeID";

        private const string ParmNodeId = "@NodeID";
        private const string ParmNodeName = "@NodeName";
        private const string ParmNodeType = "@NodeType";
        private const string ParmPublishmentSystemId = "@PublishmentSystemID";
        private const string ParmContentModelId = "@ContentModelID";
        private const string ParmParentId = "@ParentID";
        private const string ParmParentsPath = "@ParentsPath";
        private const string ParmParentsCount = "@ParentsCount";
        private const string ParmChildrenCount = "@ChildrenCount";
        private const string ParmIsLastNode = "@IsLastNode";
        private const string ParmNodeIndexName = "@NodeIndexName";
        private const string ParmNodeGroupNameCollection = "@NodeGroupNameCollection";
        private const string ParmTaxis = "@Taxis";
        private const string ParmAddDate = "@AddDate";
        private const string ParmImageUrl = "@ImageUrl";
        private const string ParmContent = "@Content";
        private const string ParmContentNum = "@ContentNum";
        private const string ParmFilePath = "@FilePath";
        private const string ParmChannelFilePathRule = "@ChannelFilePathRule";
        private const string ParmContentFilePathRule = "@ContentFilePathRule";
        private const string ParmLinkUrl = "@LinkUrl";
        private const string ParmLinkType = "@LinkType";
        private const string ParmChannelTemplateId = "@ChannelTemplateID";
        private const string ParmContentTemplateId = "@ContentTemplateID";
        private const string ParmKeywords = "@Keywords";
        private const string ParmDescription = "@Description";
        private const string ParmExtendValues = "@ExtendValues";

        /// <summary>
        /// 使用事务添加节点信息到Node表中
        /// </summary>
        /// <param name="parentNodeInfo">父节点</param>
        /// <param name="nodeInfo">需要添加的节点</param>
        /// <param name="trans"></param>
        private void InsertNodeInfoWithTrans(NodeInfo parentNodeInfo, NodeInfo nodeInfo, IDbTransaction trans)
        {
            if (parentNodeInfo != null)
            {
                nodeInfo.PublishmentSystemId = parentNodeInfo.NodeType == ENodeType.BackgroundPublishNode ? parentNodeInfo.NodeId : parentNodeInfo.PublishmentSystemId;
                if (parentNodeInfo.ParentsPath.Length == 0)
                {
                    nodeInfo.ParentsPath = parentNodeInfo.NodeId.ToString();
                }
                else
                {
                    nodeInfo.ParentsPath = parentNodeInfo.ParentsPath + "," + parentNodeInfo.NodeId;
                }
                nodeInfo.ParentsCount = parentNodeInfo.ParentsCount + 1;

                var maxTaxis = GetMaxTaxisByParentPath(nodeInfo.ParentsPath);
                if (maxTaxis == 0)
                {
                    maxTaxis = parentNodeInfo.Taxis;
                }
                nodeInfo.Taxis = maxTaxis + 1;
            }
            else
            {
                nodeInfo.Taxis = 1;
            }

            const string sqlInsertNode = "INSERT INTO siteserver_Node (NodeName, NodeType, PublishmentSystemID, ContentModelID, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, NodeIndexName, NodeGroupNameCollection, Taxis, AddDate, ImageUrl, Content, ContentNum, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateID, ContentTemplateID, Keywords, Description, ExtendValues) VALUES (@NodeName, @NodeType, @PublishmentSystemID, @ContentModelID, @ParentID, @ParentsPath, @ParentsCount, @ChildrenCount, @IsLastNode, @NodeIndexName, @NodeGroupNameCollection, @Taxis, @AddDate, @ImageUrl, @Content, @ContentNum, @FilePath, @ChannelFilePathRule, @ContentFilePathRule, @LinkUrl, @LinkType, @ChannelTemplateID, @ContentTemplateID, @Keywords, @Description, @ExtendValues)";

            var insertParms = new IDataParameter[]
            {
                GetParameter(ParmNodeName, EDataType.NVarChar, 255, nodeInfo.NodeName),
                GetParameter(ParmNodeType, EDataType.VarChar, 50, ENodeTypeUtils.GetValue(nodeInfo.NodeType)),
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, nodeInfo.PublishmentSystemId),
                GetParameter(ParmContentModelId, EDataType.VarChar, 50, nodeInfo.ContentModelId),
                GetParameter(ParmParentId, EDataType.Integer, nodeInfo.ParentId),
                GetParameter(ParmParentsPath, EDataType.NVarChar, 255, nodeInfo.ParentsPath),
                GetParameter(ParmParentsCount, EDataType.Integer, nodeInfo.ParentsCount),
                GetParameter(ParmChildrenCount, EDataType.Integer, 0),
                GetParameter(ParmIsLastNode, EDataType.VarChar, 18, true.ToString()),
                GetParameter(ParmNodeIndexName, EDataType.NVarChar, 255, nodeInfo.NodeIndexName),
                GetParameter(ParmNodeGroupNameCollection, EDataType.NVarChar, 255, nodeInfo.NodeGroupNameCollection),
                GetParameter(ParmTaxis, EDataType.Integer, nodeInfo.Taxis),
                GetParameter(ParmAddDate, EDataType.DateTime, nodeInfo.AddDate),
                GetParameter(ParmImageUrl, EDataType.VarChar, 200, nodeInfo.ImageUrl),
                GetParameter(ParmContent, EDataType.NText, nodeInfo.Content),
                GetParameter(ParmContentNum, EDataType.Integer, nodeInfo.ContentNum),
                GetParameter(ParmFilePath, EDataType.VarChar, 200, nodeInfo.FilePath),
                GetParameter(ParmChannelFilePathRule, EDataType.VarChar, 200, nodeInfo.ChannelFilePathRule),
                GetParameter(ParmContentFilePathRule, EDataType.VarChar, 200, nodeInfo.ContentFilePathRule),
                GetParameter(ParmLinkUrl, EDataType.VarChar, 200, nodeInfo.LinkUrl),
                GetParameter(ParmLinkType, EDataType.VarChar, 200, ELinkTypeUtils.GetValue(nodeInfo.LinkType)),
                GetParameter(ParmChannelTemplateId, EDataType.Integer, nodeInfo.ChannelTemplateId),
                GetParameter(ParmContentTemplateId, EDataType.Integer, nodeInfo.ContentTemplateId),
                GetParameter(ParmKeywords, EDataType.NVarChar, 255, nodeInfo.Keywords),
                GetParameter(ParmDescription, EDataType.NVarChar, 255, nodeInfo.Description),
                GetParameter(ParmExtendValues, EDataType.NText, nodeInfo.Additional.ToString())
            };

            if (nodeInfo.PublishmentSystemId != 0)
            {
                string sqlString =
                    $"UPDATE siteserver_Node SET {SqlUtils.GetAddOne("Taxis")} WHERE (Taxis >= {nodeInfo.Taxis}) AND (PublishmentSystemID = {nodeInfo.PublishmentSystemId})";
                ExecuteNonQuery(trans, sqlString);
            }
            nodeInfo.NodeId = ExecuteNonQueryAndReturnId(trans, sqlInsertNode, insertParms);

            if (!string.IsNullOrEmpty(nodeInfo.ParentsPath))
            {
                var sqlString = $"UPDATE siteserver_Node SET {SqlUtils.GetAddOne("ChildrenCount")} WHERE NodeID IN ({nodeInfo.ParentsPath})";

                ExecuteNonQuery(trans, sqlString);
            }

            var sqlUpdateIsLastNode = "UPDATE siteserver_Node SET IsLastNode = @IsLastNode WHERE ParentID = @ParentID";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmIsLastNode, EDataType.VarChar, 18, false.ToString()),
                GetParameter(ParmParentId, EDataType.Integer, nodeInfo.ParentId)
            };

            ExecuteNonQuery(trans, sqlUpdateIsLastNode, parms);

            //sqlUpdateIsLastNode =
            //    $"UPDATE siteserver_Node SET IsLastNode = '{true}' WHERE (NodeID IN (SELECT TOP 1 NodeID FROM siteserver_Node WHERE ParentID = {nodeInfo.ParentId} ORDER BY Taxis DESC))";
            sqlUpdateIsLastNode =
                $"UPDATE siteserver_Node SET IsLastNode = '{true}' WHERE (NodeID IN ({SqlUtils.GetInTopSqlString(TableName, "NodeID", $"WHERE ParentID = {nodeInfo.ParentId} ORDER BY Taxis DESC", 1)}))";


            ExecuteNonQuery(trans, sqlUpdateIsLastNode);

            //OwningNodeIDCache.IsChanged = true;
            NodeManager.RemoveCache(nodeInfo.PublishmentSystemId);
            ProductPermissionsManager.Current.ClearCache();
        }


        /// <summary>
        /// 将节点数减1
        /// </summary>
        /// <param name="parentsPath"></param>
        /// <param name="subtractNum"></param>
        private void UpdateSubtractChildrenCount(string parentsPath, int subtractNum)
        {
            if (!string.IsNullOrEmpty(parentsPath))
            {
                var sqlString = string.Concat("UPDATE siteserver_Node SET ChildrenCount = ChildrenCount - ", subtractNum, " WHERE NodeID in (", parentsPath, ")");
                ExecuteNonQuery(sqlString);
            }
        }


        /// <summary>
        /// 更新发布系统下的所有节点的排序号
        /// </summary>
        /// <param name="publishmentSystemId"></param>
        private void UpdateWholeTaxisByPublishmentSystemId(int publishmentSystemId)
        {
            if (publishmentSystemId <= 0) return;
            var nodeIdList = new List<int>
            {
                publishmentSystemId
            };
            var level = 0;
            string selectLevelCmd =
                $"SELECT MAX(ParentsCount) FROM siteserver_Node WHERE (NodeID = {publishmentSystemId}) OR (PublishmentSystemID = {publishmentSystemId})";
            using (var rdr = ExecuteReader(selectLevelCmd))
            {
                while (rdr.Read())
                {
                    var parentsCount = GetInt(rdr, 0);
                    level = parentsCount;
                }
                rdr.Close();
            }

            for (var i = 0; i < level; i++)
            {
                var list = new List<int>(nodeIdList);
                foreach (int savedNodeId in list)
                {
                    var lastChildNodeIdOfSavedNodeId = savedNodeId;
                    string selectNodeCmd =
                        $"SELECT NodeID, NodeName FROM siteserver_Node WHERE ParentID = {savedNodeId} ORDER BY Taxis, IsLastNode";
                    using (var rdr = ExecuteReader(selectNodeCmd))
                    {
                        while (rdr.Read())
                        {
                            var nodeId = GetInt(rdr, 0);
                            if (!nodeIdList.Contains(nodeId))
                            {
                                var index = nodeIdList.IndexOf(lastChildNodeIdOfSavedNodeId);
                                nodeIdList.Insert(index + 1, nodeId);
                                lastChildNodeIdOfSavedNodeId = nodeId;
                            }
                        }
                        rdr.Close();
                    }
                }
            }

            for (var i = 1; i <= nodeIdList.Count; i++)
            {
                var nodeId = nodeIdList[i - 1];
                string updateCmd = $"UPDATE siteserver_Node SET Taxis = {i} WHERE NodeID = {nodeId}";
                ExecuteNonQuery(updateCmd);
            }
        }


        /// <summary>
        /// Change The Texis To Lowerer Level
        /// </summary>
        private void TaxisSubtract(int publishmentSystemId, int selectedNodeId)
        {
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, selectedNodeId);
            if (nodeInfo == null || nodeInfo.NodeType == ENodeType.BackgroundPublishNode || nodeInfo.PublishmentSystemId == 0) return;
            UpdateWholeTaxisByPublishmentSystemId(nodeInfo.PublishmentSystemId);
            //Get Lower Taxis and NodeID
            int lowerNodeId;
            int lowerChildrenCount;
            string lowerParentsPath;
            //            var sqlString = @"SELECT TOP 1 NodeID, ChildrenCount, ParentsPath
            //FROM siteserver_Node
            //WHERE (ParentID = @ParentID) AND (NodeID <> @NodeID) AND (Taxis < @Taxis) AND (PublishmentSystemID = @PublishmentSystemID)
            //ORDER BY Taxis DESC";
            var sqlString = SqlUtils.GetTopSqlString(TableName, "NodeID, ChildrenCount, ParentsPath", "WHERE (ParentID = @ParentID) AND (NodeID <> @NodeID) AND (Taxis < @Taxis) AND (PublishmentSystemID = @PublishmentSystemID) ORDER BY Taxis DESC", 1);

            var parms = new IDataParameter[]
            {
                GetParameter(ParmParentId, EDataType.Integer, nodeInfo.ParentId),
                GetParameter(ParmNodeId, EDataType.Integer, nodeInfo.NodeId),
                GetParameter(ParmTaxis, EDataType.Integer, nodeInfo.Taxis),
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, nodeInfo.PublishmentSystemId)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    lowerNodeId = GetInt(rdr, 0);
                    lowerChildrenCount = GetInt(rdr, 1);
                    lowerParentsPath = GetString(rdr, 2);
                }
                else
                {
                    return;
                }
                rdr.Close();
            }


            var lowerNodePath = lowerParentsPath == "" ? lowerNodeId.ToString() : string.Concat(lowerParentsPath, ",", lowerNodeId);
            var selectedNodePath = nodeInfo.ParentsPath == "" ? nodeInfo.NodeId.ToString() : string.Concat(nodeInfo.ParentsPath, ",", nodeInfo.NodeId);

            SetTaxisSubtract(selectedNodeId, selectedNodePath, lowerChildrenCount + 1);
            SetTaxisAdd(lowerNodeId, lowerNodePath, nodeInfo.ChildrenCount + 1);

            UpdateIsLastNode(nodeInfo.ParentId);

        }

        /// <summary>
        /// Change The Texis To Higher Level
        /// </summary>
        private void TaxisAdd(int publishmentSystemId, int selectedNodeId)
        {
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, selectedNodeId);
            if (nodeInfo == null || nodeInfo.NodeType == ENodeType.BackgroundPublishNode || nodeInfo.PublishmentSystemId == 0) return;
            UpdateWholeTaxisByPublishmentSystemId(nodeInfo.PublishmentSystemId);
            //Get Higher Taxis and NodeID
            int higherNodeId;
            int higherChildrenCount;
            string higherParentsPath;
            //const string sqlString = @"SELECT TOP 1 NodeID, ChildrenCount, ParentsPath
            //FROM siteserver_Node
            //WHERE (ParentID = @ParentID) AND (NodeID <> @NodeID) AND (Taxis > @Taxis) AND (PublishmentSystemID = @PublishmentSystemID) ORDER BY Taxis";
            var sqlString = SqlUtils.GetTopSqlString(TableName, "NodeID, ChildrenCount, ParentsPath", "WHERE (ParentID = @ParentID) AND (NodeID <> @NodeID) AND (Taxis > @Taxis) AND (PublishmentSystemID = @PublishmentSystemID) ORDER BY Taxis", 1);

            var parms = new IDataParameter[]
            {
                GetParameter(ParmParentId, EDataType.Integer, nodeInfo.ParentId),
                GetParameter(ParmNodeId, EDataType.Integer, nodeInfo.NodeId),
                GetParameter(ParmTaxis, EDataType.Integer, nodeInfo.Taxis),
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, nodeInfo.PublishmentSystemId)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    higherNodeId = GetInt(rdr, 0);
                    higherChildrenCount = GetInt(rdr, 1);
                    higherParentsPath = GetString(rdr, 2);
                }
                else
                {
                    return;
                }
                rdr.Close();
            }


            var higherNodePath = higherParentsPath == string.Empty ? higherNodeId.ToString() : string.Concat(higherParentsPath, ",", higherNodeId);
            var selectedNodePath = nodeInfo.ParentsPath == string.Empty ? nodeInfo.NodeId.ToString() : String.Concat(nodeInfo.ParentsPath, ",", nodeInfo.NodeId);

            SetTaxisAdd(selectedNodeId, selectedNodePath, higherChildrenCount + 1);
            SetTaxisSubtract(higherNodeId, higherNodePath, nodeInfo.ChildrenCount + 1);

            UpdateIsLastNode(nodeInfo.ParentId);
        }

        private void SetTaxisAdd(int nodeId, string parentsPath, int addNum)
        {
            string sqlString =
                $"UPDATE siteserver_Node SET {SqlUtils.GetAddNum("Taxis", addNum)} WHERE NodeID = {nodeId} OR ParentsPath = '{parentsPath}' OR ParentsPath like '{parentsPath},%'";

            ExecuteNonQuery(sqlString);
        }

        private void SetTaxisSubtract(int nodeId, string parentsPath, int subtractNum)
        {
            string sqlString =
                $"UPDATE siteserver_Node SET {SqlUtils.GetMinusNum("Taxis", subtractNum)} WHERE  NodeID = {nodeId} OR ParentsPath = '{parentsPath}' OR ParentsPath like '{parentsPath},%'";

            ExecuteNonQuery(sqlString);
        }


        private void UpdateIsLastNode(int parentId)
        {
            if (parentId <= 0) return;

            var sqlString = "UPDATE siteserver_Node SET IsLastNode = @IsLastNode WHERE  ParentID = @ParentID";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmIsLastNode, EDataType.VarChar, 18, false.ToString()),
                GetParameter(ParmParentId, EDataType.Integer, parentId)
            };

            ExecuteNonQuery(sqlString, parms);

            //sqlString =
            //    $"UPDATE siteserver_Node SET IsLastNode = '{true}' WHERE (NodeID IN (SELECT TOP 1 NodeID FROM siteserver_Node WHERE ParentID = {parentId} ORDER BY Taxis DESC))";
            sqlString =
                $"UPDATE siteserver_Node SET IsLastNode = '{true}' WHERE (NodeID IN ({SqlUtils.GetInTopSqlString(TableName, "NodeID", $"WHERE ParentID = {parentId} ORDER BY Taxis DESC", 1)}))";

            ExecuteNonQuery(sqlString);
        }


        private int GetMaxTaxisByParentPath(string parentPath)
        {
            var cmd = string.Concat("SELECT MAX(Taxis) AS MaxTaxis FROM siteserver_Node WHERE (ParentsPath = '", parentPath, "') OR (ParentsPath like '", parentPath, ",%')");
            var maxTaxis = 0;

            using (var rdr = ExecuteReader(cmd))
            {
                if (rdr.Read())
                {
                    maxTaxis = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return maxTaxis;
        }

        private string GetNodeGroupNameCollection(int nodeId)
        {
            var groupNameCollection = string.Empty;

            var nodeParms = new IDataParameter[]
            {
                GetParameter(ParmNodeId, EDataType.Integer, nodeId)
            };

            using (var rdr = ExecuteReader(SqlSelectNodeGroupNameCollection, nodeParms))
            {
                if (rdr.Read())
                {
                    groupNameCollection = GetString(rdr, 0);
                }
                rdr.Close();
            }
            return groupNameCollection;
        }

        private int GetParentId(int nodeId)
        {
            var parentId = 0;

            var nodeParms = new IDataParameter[]
            {
                GetParameter(ParmNodeId, EDataType.Integer, nodeId)
            };

            using (var rdr = ExecuteReader(SqlSelectParentId, nodeParms))
            {
                if (rdr.Read())
                {
                    parentId = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return parentId;
        }

        private int GetNodeIdByParentIdAndOrder(int parentId, int order)
        {
            var nodeId = parentId;

            string cmd = $"SELECT NodeID FROM siteserver_Node WHERE (ParentID = {parentId}) ORDER BY Taxis";

            using (var rdr = ExecuteReader(cmd))
            {
                var index = 1;
                while (rdr.Read())
                {
                    nodeId = GetInt(rdr, 0);
                    if (index == order)
                        break;
                    index++;
                }
                rdr.Close();
            }
            return nodeId;
        }

        public int InsertNodeInfo(int publishmentSystemId, int parentId, string nodeName, string nodeIndex, string contentModelId)
        {
            if (publishmentSystemId > 0 && parentId == 0) return 0;

            var defaultChannelTemplateInfo = TemplateManager.GetDefaultTemplateInfo(publishmentSystemId, ETemplateType.ChannelTemplate);
            var defaultContentTemplateInfo = TemplateManager.GetDefaultTemplateInfo(publishmentSystemId, ETemplateType.ContentTemplate);

            var nodeInfo = new NodeInfo
            {
                ParentId = parentId,
                PublishmentSystemId = publishmentSystemId,
                NodeName = nodeName,
                NodeIndexName = nodeIndex,
                ContentModelId = contentModelId,
                AddDate = DateTime.Now,
                ChannelTemplateId = defaultChannelTemplateInfo.TemplateId,
                ContentTemplateId = defaultContentTemplateInfo.TemplateId
            };

            var parentNodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, parentId);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        InsertNodeInfoWithTrans(parentNodeInfo, nodeInfo, trans);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return nodeInfo.NodeId;
        }

        public int InsertNodeInfo(int publishmentSystemId, int parentId, string nodeName, string nodeIndex, string contentModelId, int channelTemplateId, int contentTemplateId)
        {
            if (publishmentSystemId > 0 && parentId == 0) return 0;

            var defaultChannelTemplateInfo = TemplateManager.GetDefaultTemplateInfo(publishmentSystemId, ETemplateType.ChannelTemplate);
            var defaultContentTemplateInfo = TemplateManager.GetDefaultTemplateInfo(publishmentSystemId, ETemplateType.ContentTemplate);

            var nodeInfo = new NodeInfo
            {
                ParentId = parentId,
                PublishmentSystemId = publishmentSystemId,
                NodeName = nodeName,
                NodeIndexName = nodeIndex,
                ContentModelId = contentModelId,
                AddDate = DateTime.Now,
                ChannelTemplateId = channelTemplateId > 0 ? channelTemplateId : defaultChannelTemplateInfo.TemplateId,
                ContentTemplateId = contentTemplateId > 0 ? contentTemplateId : defaultContentTemplateInfo.TemplateId
            };

            var parentNodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, parentId);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        InsertNodeInfoWithTrans(parentNodeInfo, nodeInfo, trans);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return nodeInfo.NodeId;

        }

        public int InsertNodeInfo(NodeInfo nodeInfo)
        {
            if (nodeInfo.PublishmentSystemId > 0 && nodeInfo.ParentId == 0) return 0;

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var parentNodeInfo = GetNodeInfo(nodeInfo.ParentId);

                        InsertNodeInfoWithTrans(parentNodeInfo, nodeInfo, trans);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return nodeInfo.NodeId;
        }

        /// <summary>
        /// 添加后台发布节点
        /// </summary>
        public int InsertPublishmentSystemInfo(NodeInfo nodeInfo, PublishmentSystemInfo psInfo, string administratorName)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        InsertNodeInfoWithTrans(null, nodeInfo, trans);

                        psInfo.PublishmentSystemId = nodeInfo.NodeId;

                        DataProvider.PublishmentSystemDao.InsertWithTrans(psInfo, trans);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            BaiRongDataProvider.AdministratorDao.UpdatePublishmentSystemId(administratorName, nodeInfo.NodeId);

            string updateNodeSqlString =
                $"UPDATE siteserver_Node SET PublishmentSystemID = {nodeInfo.NodeId} WHERE NodeID = {nodeInfo.NodeId}";
            ExecuteNonQuery(updateNodeSqlString);

            DataProvider.TemplateDao.CreateDefaultTemplateInfo(nodeInfo.NodeId, administratorName);
            DataProvider.MenuDisplayDao.CreateDefaultMenuDisplayInfo(nodeInfo.NodeId);
            return nodeInfo.NodeId;
        }


        /// <summary>
        /// 修改节点信息到Node表中
        /// </summary>
        /// <param name="nodeInfo"></param>
        public void UpdateNodeInfo(NodeInfo nodeInfo)
        {
            var updateParms = new IDataParameter[]
            {
                GetParameter(ParmNodeName, EDataType.NVarChar, 255, nodeInfo.NodeName),
                GetParameter(ParmNodeType, EDataType.VarChar, 50, ENodeTypeUtils.GetValue(nodeInfo.NodeType)),
                GetParameter(ParmContentModelId, EDataType.VarChar, 50, nodeInfo.ContentModelId),
                GetParameter(ParmParentsPath, EDataType.NVarChar, 255, nodeInfo.ParentsPath),
                GetParameter(ParmParentsCount, EDataType.Integer, nodeInfo.ParentsCount),
                GetParameter(ParmChildrenCount, EDataType.Integer, nodeInfo.ChildrenCount),
                GetParameter(ParmIsLastNode, EDataType.VarChar, 18, nodeInfo.IsLastNode.ToString()),
                GetParameter(ParmNodeIndexName, EDataType.NVarChar, 255, nodeInfo.NodeIndexName),
                GetParameter(ParmNodeGroupNameCollection, EDataType.NVarChar, 255, nodeInfo.NodeGroupNameCollection),
                GetParameter(ParmImageUrl, EDataType.VarChar, 200, nodeInfo.ImageUrl),
                GetParameter(ParmContent, EDataType.NText, nodeInfo.Content),
                GetParameter(ParmContentNum, EDataType.Integer, nodeInfo.ContentNum),
                GetParameter(ParmFilePath, EDataType.VarChar, 200, nodeInfo.FilePath),
                GetParameter(ParmChannelFilePathRule, EDataType.VarChar, 200, nodeInfo.ChannelFilePathRule),
                GetParameter(ParmContentFilePathRule, EDataType.VarChar, 200, nodeInfo.ContentFilePathRule),
                GetParameter(ParmLinkUrl, EDataType.VarChar, 200, nodeInfo.LinkUrl),
                GetParameter(ParmLinkType, EDataType.VarChar, 200, ELinkTypeUtils.GetValue(nodeInfo.LinkType)),
                GetParameter(ParmChannelTemplateId, EDataType.Integer, nodeInfo.ChannelTemplateId),
                GetParameter(ParmContentTemplateId, EDataType.Integer, nodeInfo.ContentTemplateId),
                GetParameter(ParmKeywords, EDataType.NVarChar, 255, nodeInfo.Keywords),
                GetParameter(ParmDescription, EDataType.NVarChar, 255, nodeInfo.Description),
                GetParameter(ParmExtendValues, EDataType.NText, nodeInfo.Additional.ToString()),
                GetParameter(ParmNodeId, EDataType.Integer, nodeInfo.NodeId)
            };

            ExecuteNonQuery(SqlUpdateNode, updateParms);

            NodeManager.RemoveCache(nodeInfo.NodeType == ENodeType.BackgroundPublishNode
                ? nodeInfo.NodeId
                : nodeInfo.PublishmentSystemId);
        }

        public void UpdateAdditional(NodeInfo nodeInfo)
        {
            var updateParms = new IDataParameter[]
            {
                GetParameter(ParmExtendValues, EDataType.NText, nodeInfo.Additional.ToString()),
                GetParameter(ParmNodeId, EDataType.Integer, nodeInfo.NodeId)
            };

            ExecuteNonQuery(SqlUpdateExtendValues, updateParms);

            NodeManager.RemoveCache(nodeInfo.NodeType == ENodeType.BackgroundPublishNode
                ? nodeInfo.NodeId
                : nodeInfo.PublishmentSystemId);
        }

        /// <summary>
        /// 修改排序
        /// </summary>
        public void UpdateTaxis(int publishmentSystemId, int selectedNodeId, bool isSubtract)
        {
            if (isSubtract)
            {
                TaxisSubtract(publishmentSystemId, selectedNodeId);
            }
            else
            {
                TaxisAdd(publishmentSystemId, selectedNodeId);
            }
            NodeManager.RemoveCache(publishmentSystemId);
        }

        private void UpdateNodeGroupNameCollection(int publishmentSystemId, int nodeId, string nodeGroupNameCollection)
        {
            var parms = new IDataParameter[]
            {
                GetParameter(ParmNodeGroupNameCollection, EDataType.NVarChar, 255, nodeGroupNameCollection),
                GetParameter(ParmNodeId, EDataType.Integer, nodeId)
            };

            ExecuteNonQuery(SqlUpdateNodeGroupNameCollection, parms);
            NodeManager.RemoveCache(publishmentSystemId);
        }

        public void AddNodeGroupList(int publishmentSystemId, int nodeId, List<string> nodeGroupList)
        {
            var list = TranslateUtils.StringCollectionToStringList(GetNodeGroupNameCollection(nodeId));
            foreach (string groupName in nodeGroupList)
            {
                if (!list.Contains(groupName)) list.Add(groupName);
            }
            UpdateNodeGroupNameCollection(publishmentSystemId, nodeId, TranslateUtils.ObjectCollectionToString(list));
        }

        public void UpdateContentNum(PublishmentSystemInfo publishmentSystemInfo)
        {
            var nodeIdList = GetNodeIdListByPublishmentSystemId(publishmentSystemInfo.PublishmentSystemId);
            foreach (var nodeId in nodeIdList)
            {
                UpdateContentNum(publishmentSystemInfo, nodeId, false);
            }

            NodeManager.RemoveCache(publishmentSystemInfo.PublishmentSystemId);
        }

        public virtual void UpdateContentNum(PublishmentSystemInfo publishmentSystemInfo, int nodeId, bool isRemoveCache)
        {
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);
            var sqlString = string.Empty;
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
            if (!string.IsNullOrEmpty(tableName))
            {
                sqlString =
                    $"UPDATE siteserver_Node SET ContentNum = (SELECT COUNT(*) AS ContentNum FROM {tableName} WHERE (NodeID = {nodeId})) WHERE (NodeID = {nodeId})";
            }
            if (!string.IsNullOrEmpty(sqlString))
            {
                ExecuteNonQuery(sqlString);
            }

            if (isRemoveCache)
            {
                NodeManager.RemoveCache(publishmentSystemInfo.PublishmentSystemId);
            }
        }

        public void UpdateContentNumToZero(string tableName, EAuxiliaryTableType tableType)
        {
            if (tableType != EAuxiliaryTableType.BackgroundContent) return;

            var publishmentSystemIdList = new List<int>();
            var psIdList = PublishmentSystemManager.GetPublishmentSystemIdList();

            foreach (var publishmentSystemId in psIdList)
            {
                var psInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                if (tableType == EAuxiliaryTableType.BackgroundContent && psInfo.AuxiliaryTableForContent == tableName)
                {
                    publishmentSystemIdList.Add(publishmentSystemId);
                    NodeManager.RemoveCache(publishmentSystemId);
                }
                else if (tableType == EAuxiliaryTableType.GovPublicContent && psInfo.AuxiliaryTableForGovPublic == tableName)
                {
                    publishmentSystemIdList.Add(publishmentSystemId);
                    NodeManager.RemoveCache(publishmentSystemId);
                }
                else if (tableType == EAuxiliaryTableType.GovInteractContent && psInfo.AuxiliaryTableForGovInteract == tableName)
                {
                    publishmentSystemIdList.Add(publishmentSystemId);
                    NodeManager.RemoveCache(publishmentSystemId);
                }
                else if (tableType == EAuxiliaryTableType.VoteContent && psInfo.AuxiliaryTableForVote == tableName)
                {
                    publishmentSystemIdList.Add(publishmentSystemId);
                    NodeManager.RemoveCache(publishmentSystemId);
                }
                else if (tableType == EAuxiliaryTableType.JobContent && psInfo.AuxiliaryTableForJob == tableName)
                {
                    publishmentSystemIdList.Add(publishmentSystemId);
                    NodeManager.RemoveCache(publishmentSystemId);
                }
            }
            if (publishmentSystemIdList.Count == 0) return;

            var inString = TranslateUtils.ToSqlInStringWithoutQuote(publishmentSystemIdList);
            string sqlString =
                $"UPDATE siteserver_Node SET ContentNum = 0 WHERE PublishmentSystemID IN ({inString}) OR NodeID IN ({inString})";
            ExecuteNonQuery(sqlString);
        }

        public void Delete(int nodeId)
        {
            var nodeInfo = GetNodeInfo(nodeId);
            if (nodeInfo == null) return;

            var nodeIdList = new List<int>();
            if (nodeInfo.ChildrenCount > 0)
            {
                nodeIdList = DataProvider.NodeDao.GetNodeIdListForDescendant(nodeId);
            }
            nodeIdList.Add(nodeId);

            string deleteCmd =
                $"DELETE FROM siteserver_Node WHERE NodeID IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)})";

            var tableName = NodeManager.GetTableName(PublishmentSystemManager.GetPublishmentSystemInfo(nodeInfo.PublishmentSystemId), nodeInfo);
            var deleteContentCmd = string.Empty;
            if (!string.IsNullOrEmpty(tableName))
            {
                deleteContentCmd =
                    $"DELETE FROM {tableName} WHERE NodeID IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)})";
            }

            int deletedNum;

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(deleteContentCmd))
                        {
                            ExecuteNonQuery(trans, deleteContentCmd);
                        }
                        deletedNum = ExecuteNonQuery(trans, deleteCmd);

                        if (nodeInfo.NodeType != ENodeType.BackgroundPublishNode)
                        {
                            string taxisCmd =
                                $"UPDATE siteserver_Node SET Taxis = Taxis - {deletedNum} WHERE (Taxis > {nodeInfo.Taxis}) AND (PublishmentSystemID = {nodeInfo.PublishmentSystemId})";
                            ExecuteNonQuery(trans, taxisCmd);
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
            UpdateIsLastNode(nodeInfo.ParentId);
            UpdateSubtractChildrenCount(nodeInfo.ParentsPath, deletedNum);

            if (nodeInfo.NodeType == ENodeType.BackgroundPublishNode)
            {
                DataProvider.PublishmentSystemDao.Delete(nodeInfo.NodeId);
            }
            else
            {
                NodeManager.RemoveCache(nodeInfo.PublishmentSystemId);
            }
        }

        public NodeInfo GetNodeInfo(int nodeId)
        {
            NodeInfo node = null;

            var nodeParms = new IDataParameter[]
            {
                GetParameter(ParmNodeId, EDataType.Integer, nodeId)
            };

            using (var rdr = ExecuteReader(SqlSelectNode, nodeParms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    node = new NodeInfo(GetInt(rdr, i++), GetString(rdr, i++), ENodeTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), ELinkTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }
            return node;
        }

        /// <summary>
        /// 得到最后一个添加的子节点
        /// </summary>
        public NodeInfo GetNodeInfoByLastAddDate(int nodeId)
        {
            NodeInfo node = null;

            var nodeParms = new IDataParameter[]
            {
                GetParameter(ParmParentId, EDataType.Integer, nodeId)
            };

            using (var rdr = ExecuteReader(SqlSelectNodeByLastAddDate, nodeParms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    node = new NodeInfo(GetInt(rdr, i++), GetString(rdr, i++), ENodeTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), ELinkTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }
            return node;
        }

        /// <summary>
        /// 得到第一个子节点
        /// </summary>
        public NodeInfo GetNodeInfoByTaxis(int nodeId)
        {
            NodeInfo node = null;

            var nodeParms = new IDataParameter[]
            {
                GetParameter(ParmParentId, EDataType.Integer, nodeId)
            };

            using (var rdr = ExecuteReader(SqlSelectNodeByTaxis, nodeParms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    node = new NodeInfo(GetInt(rdr, i++), GetString(rdr, i++), ENodeTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), ELinkTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }
            return node;
        }

        public NodeInfo GetNodeInfoByParentId(int publishmentSystemId, int parentId, EContentModelType contentModelType)
        {
            NodeInfo nodeInfo = null;
            var nodeParms = new IDataParameter[] {
                GetParameter(ParmPublishmentSystemId,EDataType.Integer,publishmentSystemId),
                GetParameter(ParmParentId,EDataType.Integer,parentId),
                GetParameter(ParmContentModelId,EDataType.VarChar,50,EContentModelTypeUtils.GetValue(contentModelType))
            };
            using (var rdr = ExecuteReader(SqlSelectNodeByParentIdAndContentModelId, nodeParms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    nodeInfo = new NodeInfo(GetInt(rdr, i++), GetString(rdr, i++), ENodeTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), ELinkTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }
            return nodeInfo;
        }

        public int GetNodeIdByParentIdAndNodeName(int publishmentSystemId, int parentId, string nodeName, bool recursive)
        {
            var nodeId = 0;
            string sqlString;

            if (recursive)
            {
                if (publishmentSystemId == parentId)
                {
                    sqlString =
                        $"SELECT NodeID FROM siteserver_Node WHERE (PublishmentSystemID = {publishmentSystemId} AND NodeName = '{nodeName}') ORDER BY Taxis";
                }
                else
                {
                    sqlString = $@"SELECT NodeID
FROM siteserver_Node 
WHERE ((ParentID = {parentId}) OR
      (ParentsPath = '{parentId}') OR
      (ParentsPath LIKE '{parentId},%') OR
      (ParentsPath LIKE '%,{parentId},%') OR
      (ParentsPath LIKE '%,{parentId}')) AND NodeName = '{PageUtils.FilterSql(nodeName)}'
ORDER BY Taxis";
                }
            }
            else
            {
                sqlString =
                    $"SELECT NodeID FROM siteserver_Node WHERE (ParentID = {parentId} AND NodeName = '{nodeName}') ORDER BY Taxis";
            }

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    nodeId = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return nodeId;
        }

        public int GetNodeIdByParentIdAndTaxis(int parentId, int taxis, bool isNextChannel)
        {
            var nodeId = 0;

            //var sqlString = isNextChannel ? $"SELECT TOP 1 NodeID FROM siteserver_Node WHERE (ParentID = {parentId} AND Taxis > {taxis}) ORDER BY Taxis" : $"SELECT TOP 1 NodeID FROM siteserver_Node WHERE (ParentID = {parentId} AND Taxis < {taxis}) ORDER BY Taxis DESC";
            var sqlString = isNextChannel
                ? SqlUtils.GetTopSqlString(TableName, "NodeID",
                    $"WHERE (ParentID = {parentId} AND Taxis > {taxis}) ORDER BY Taxis", 1)
                : SqlUtils.GetTopSqlString(TableName, "NodeID",
                    $"WHERE (ParentID = {parentId} AND Taxis < {taxis}) ORDER BY Taxis DESC", 1);

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    nodeId = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return nodeId;
        }

        public int GetNodeId(int publishmentSystemId, string orderString)
        {
            if (orderString == "1")
                return publishmentSystemId;

            var nodeId = publishmentSystemId;

            var orderArr = orderString.Split('_');
            for (var index = 1; index < orderArr.Length; index++)
            {
                var order = int.Parse(orderArr[index]);
                nodeId = GetNodeIdByParentIdAndOrder(nodeId, order);
            }
            return nodeId;
        }

        public int GetPublishmentSystemId(int nodeId)
        {
            var publishmentSystemId = 0;

            var nodeParms = new IDataParameter[]
            {
                GetParameter(ParmNodeId, EDataType.Integer, nodeId)
            };

            using (var rdr = ExecuteReader(SqlSelectPublishmentSystemIdById, nodeParms))
            {
                if (rdr.Read())
                {
                    publishmentSystemId = GetInt(rdr, 0);
                    if (publishmentSystemId == 0)
                    {
                        publishmentSystemId = nodeId;
                    }
                }
                rdr.Close();
            }
            return publishmentSystemId;
        }

        /// <summary>
        /// 在节点树中得到此节点的排序号，以“1_2_5_2”的形式表示
        /// </summary>
        public string GetOrderStringInPublishmentSystem(int nodeId)
        {
            var retval = "";
            if (nodeId != 0)
            {
                var parentId = GetParentId(nodeId);
                if (parentId != 0)
                {
                    var orderString = GetOrderStringInPublishmentSystem(parentId);
                    retval = orderString + "_" + GetOrderInSiblingNode(nodeId, parentId);
                }
                else
                {
                    retval = "1";
                }
            }
            return retval;
        }

        public List<int> GetNodeIdListByGroupName(int publishmentSystemId, string nodeGroupName)
        {
            var nodeIdList = new List<int>();
            string sqlString =
                $"SELECT NodeId FROM siteserver_Node WHERE PublishmentSystemID={publishmentSystemId} AND (NodeGroupNameCollection LIKE '{PageUtils.FilterSql(nodeGroupName)},%' OR NodeGroupNameCollection LIKE '%,{PageUtils.FilterSql(nodeGroupName)}' OR NodeGroupNameCollection LIKE '%,{PageUtils.FilterSql(nodeGroupName)},%' OR NodeGroupNameCollection='{PageUtils.FilterSql(nodeGroupName)}') ORDER By NodeId";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    nodeIdList.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }

            return nodeIdList;
        }

        public int GetOrderInSiblingNode(int nodeId, int parentId)
        {
            string cmd = $"SELECT NodeID FROM siteserver_Node WHERE (ParentID = {parentId}) ORDER BY Taxis";
            var nodeIdList = new List<int>();
            using (var rdr = ExecuteReader(cmd))
            {
                while (rdr.Read())
                {
                    nodeIdList.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }
            return nodeIdList.IndexOf(nodeId) + 1;
        }

        public List<string> GetNodeIndexNameList(int publishmentSystemId)
        {
            var list = new List<string>();

            var nodeParms = new IDataParameter[]
            {
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
            };

            using (var rdr = ExecuteReader(SqlSelectNodeIndexNameCollection, nodeParms))
            {
                while (rdr.Read())
                {
                    list.Add(GetString(rdr, 0));
                }
                rdr.Close();
            }

            return list;
        }

        private static string GetGroupWhereString(string group, string groupNot)
        {
            var whereStringBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(group))
            {
                group = group.Trim().Trim(',');
                var groupArr = group.Split(',');
                if (groupArr.Length > 0)
                {
                    whereStringBuilder.Append(" AND (");
                    foreach (var theGroup in groupArr)
                    {
                        var trimGroup = theGroup.Trim();
                        //whereStringBuilder.Append(
                        //    $" (siteserver_Node.NodeGroupNameCollection = '{trimGroup}' OR CHARINDEX('{trimGroup},',siteserver_Node.NodeGroupNameCollection) > 0 OR CHARINDEX(',{trimGroup},', siteserver_Node.NodeGroupNameCollection) > 0 OR CHARINDEX(',{trimGroup}', siteserver_Node.NodeGroupNameCollection) > 0) OR ");

                        whereStringBuilder.Append(
                                $" (siteserver_Node.NodeGroupNameCollection = '{trimGroup}' OR {SqlUtils.GetInStr("siteserver_Node.NodeGroupNameCollection", trimGroup + ",")} OR {SqlUtils.GetInStr("siteserver_Node.NodeGroupNameCollection", "," + trimGroup + ",")} OR {SqlUtils.GetInStr("siteserver_Node.NodeGroupNameCollection", "," + trimGroup)}) OR ");
                    }
                    if (groupArr.Length > 0)
                    {
                        whereStringBuilder.Length = whereStringBuilder.Length - 3;
                    }
                    whereStringBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(groupNot))
            {
                groupNot = groupNot.Trim().Trim(',');
                var groupNotArr = groupNot.Split(',');
                if (groupNotArr.Length > 0)
                {
                    whereStringBuilder.Append(" AND (");
                    foreach (var theGroupNot in groupNotArr)
                    {
                        var trimGroupNot = theGroupNot.Trim();
                        //whereStringBuilder.Append(
                        //    $" (siteserver_Node.NodeGroupNameCollection <> '{trimGroupNot}' AND CHARINDEX('{trimGroupNot},',siteserver_Node.NodeGroupNameCollection) = 0 AND CHARINDEX(',{trimGroupNot},',siteserver_Node.NodeGroupNameCollection) = 0 AND CHARINDEX(',{trimGroupNot}',siteserver_Node.NodeGroupNameCollection) = 0) AND ");

                        whereStringBuilder.Append(
                                $" (siteserver_Node.NodeGroupNameCollection <> '{trimGroupNot}' AND {SqlUtils.GetNotInStr("siteserver_Node.NodeGroupNameCollection", trimGroupNot + ",")} AND {SqlUtils.GetNotInStr("siteserver_Node.NodeGroupNameCollection", "," + trimGroupNot + ",")} AND {SqlUtils.GetNotInStr("siteserver_Node.NodeGroupNameCollection", "," + trimGroupNot)}) AND ");
                    }
                    if (groupNotArr.Length > 0)
                    {
                        whereStringBuilder.Length = whereStringBuilder.Length - 4;
                    }
                    whereStringBuilder.Append(") ");
                }
            }
            return whereStringBuilder.ToString();
        }

        public string GetWhereString(int publishmentSystemId, string group, string groupNot, bool isImageExists, bool isImage, string where)
        {
            var whereStringBuilder = new StringBuilder();
            if (isImageExists)
            {
                whereStringBuilder.Append(isImage
                    ? " AND siteserver_Node.ImageUrl <> '' "
                    : " AND siteserver_Node.ImageUrl = '' ");
            }

            whereStringBuilder.Append(GetGroupWhereString(group, groupNot));

            if (!string.IsNullOrEmpty(where))
            {
                whereStringBuilder.Append($" AND {where} ");
            }

            return whereStringBuilder.ToString();
        }

        public int GetNodeCountByPublishmentSystemId(int publishmentSystemId)
        {
            var nodeCount = 0;
            string cmd =
                $"SELECT COUNT(*) AS TotalNum FROM siteserver_Node WHERE (PublishmentSystemID = {publishmentSystemId} AND (NodeID = {publishmentSystemId} OR ParentID > 0))";

            using (var rdr = ExecuteReader(cmd))
            {
                if (rdr.Read())
                {
                    nodeCount = GetInt(rdr, 0);
                }
                rdr.Close();
            }

            return nodeCount;
        }

        public int GetNodeCount(int nodeId)
        {
            var nodeCount = 0;

            var nodeParms = new IDataParameter[]
            {
                GetParameter(ParmParentId, EDataType.Integer, nodeId)
            };

            using (var rdr = ExecuteReader(SqlSelectNodeCount, nodeParms))
            {
                if (rdr.Read())
                {
                    nodeCount = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return nodeCount;
        }

        public int GetSequence(int publishmentSystemId, int nodeId)
        {
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, nodeId);
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM siteserver_Node WHERE PublishmentSystemID = {publishmentSystemId} AND ParentID = {nodeInfo.ParentId} AND Taxis > (SELECT Taxis FROM siteserver_Node WHERE NodeID = {nodeId})";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString) + 1;
        }

        public int GetNodeIdByNodeIndexName(int publishmentSystemId, string nodeIndexName)
        {
            var nodeId = 0;
            if (string.IsNullOrEmpty(nodeIndexName)) return nodeId;

            var nodeParms = new IDataParameter[]
            {
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
                GetParameter(ParmNodeIndexName, EDataType.NVarChar, 255, nodeIndexName)
            };

            using (var rdr = ExecuteReader(SqlSelectNodeIdByIndex, nodeParms))
            {
                if (rdr.Read())
                {
                    nodeId = GetInt(rdr, 0);
                }
                rdr.Close();
            }

            return nodeId;
        }

        public int GetNodeIdByContentModelType(int publishmentSystemId, EContentModelType contentModelType)
        {
            var nodeId = 0;

            var nodeParms = new IDataParameter[]
            {
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
                GetParameter(ParmContentModelId, EDataType.VarChar, 50, EContentModelTypeUtils.GetValue(contentModelType))
            };

            using (var rdr = ExecuteReader(SqlSelectNodeIdByContentModelId, nodeParms))
            {
                if (rdr.Read())
                {
                    nodeId = GetInt(rdr, 0);
                }
                rdr.Close();
            }

            return nodeId;
        }

        public bool IsExists(int nodeId)
        {
            var exists = false;

            var nodeParms = new IDataParameter[]
            {
                GetParameter(ParmNodeId, EDataType.Integer, nodeId)
            };

            using (var rdr = ExecuteReader(SqlSelectNodeId, nodeParms))
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

        public void UpdateChannelTemplateId(int nodeId, int channelTemplateId)
        {
            string sqlString =
                $"UPDATE siteserver_Node SET ChannelTemplateID = {channelTemplateId} WHERE NodeID = {nodeId}";
            ExecuteNonQuery(sqlString);

            NodeManager.RemoveCache(nodeId);
        }

        public void UpdateContentTemplateId(int nodeId, int contentTemplateId)
        {
            string sqlString =
                $"UPDATE siteserver_Node SET ContentTemplateID = {contentTemplateId} WHERE NodeID = {nodeId}";
            ExecuteNonQuery(sqlString);

            NodeManager.RemoveCache(nodeId);
        }

        public List<int> GetNodeIdList()
        {
            var list = new List<int>();
            const string sqlString = "SELECT NodeID FROM siteserver_Node";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }

            return list;
        }

        public List<int> GetNodeIdList(int nodeId, int totalNum, string orderByString, string whereString, EScopeType scopeType, string group, string groupNot)
        {
            var nodeIdList = GetNodeIdListByScopeType(nodeId, scopeType, group, groupNot);
            if (nodeIdList == null || nodeIdList.Count == 0)
            {
                return nodeIdList;
            }

            string sqlString;
            if (totalNum > 0)
            {
//                sqlString = $@"SELECT TOP {totalNum} NodeID
//FROM siteserver_Node 
//WHERE (NodeID IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)}) {whereString}) {orderByString}
//";
                sqlString = SqlUtils.GetTopSqlString(TableName, "NodeID",
                    $"WHERE (NodeID IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)}) {whereString}) {orderByString}",
                    totalNum);
            }
            else
            {
                sqlString = $@"SELECT NodeID
FROM siteserver_Node 
WHERE (NodeID IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)}) {whereString}) {orderByString}
";
            }

            var list = new List<int>();

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }
            return list;
        }

        public List<int> GetNodeIdListByNodeType(params ENodeType[] eNodeTypeArray)
        {
            if (eNodeTypeArray == null || eNodeTypeArray.Length <= 0) return new List<int>();

            var list = new List<int>();
            var nodeTypeStringList = new List<string>();
            foreach (var nodeType in eNodeTypeArray)
            {
                nodeTypeStringList.Add(ENodeTypeUtils.GetValue(nodeType));
            }
            string sqlString =
                $"SELECT NodeID FROM siteserver_Node WHERE NodeType IN ({TranslateUtils.ToSqlInStringWithQuote(nodeTypeStringList)})";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }
            return list;
        }

        public List<int> GetNodeIdListByScopeType(int nodeId, EScopeType scopeType, string group, string groupNot)
        {
            var list = new List<int>();
            string sqlString = null;
            var groupWhereString = GetGroupWhereString(group, groupNot);
            if (scopeType == EScopeType.All)
            {
                sqlString = $@"SELECT NodeID
FROM siteserver_Node 
WHERE ((NodeID = {nodeId}) OR
      (ParentID = {nodeId}) OR
      (ParentsPath = '{nodeId}') OR
      (ParentsPath LIKE '{nodeId},%') OR
      (ParentsPath LIKE '%,{nodeId},%') OR
      (ParentsPath LIKE '%,{nodeId}')) {groupWhereString}
ORDER BY Taxis";
            }
            else if (scopeType == EScopeType.Self)
            {
                list.Add(nodeId);
                return list;
            }
            else if (scopeType == EScopeType.Children)
            {
                sqlString = $@"SELECT NodeID
FROM siteserver_Node 
WHERE (ParentID = {nodeId}) {groupWhereString}
ORDER BY Taxis";
            }
            else if (scopeType == EScopeType.Descendant)
            {
                sqlString = $@"SELECT NodeID
FROM siteserver_Node 
WHERE ((ParentID = {nodeId}) OR
      (ParentsPath = '{nodeId}') OR
      (ParentsPath LIKE '{nodeId},%') OR
      (ParentsPath LIKE '%,{nodeId},%') OR
      (ParentsPath LIKE '%,{nodeId}')) {groupWhereString}
ORDER BY Taxis";
            }
            else if (scopeType == EScopeType.SelfAndChildren)
            {
                sqlString = $@"SELECT NodeID
FROM siteserver_Node 
WHERE ((NodeID = {nodeId}) OR
      (ParentID = {nodeId})) {groupWhereString}
ORDER BY Taxis";
            }

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }

            return list;
        }

        private List<int> GetNodeIdListByScopeType(NodeInfo nodeInfo, EScopeType scopeType)
        {
            return GetNodeIdListByScopeType(nodeInfo, scopeType, string.Empty, string.Empty, string.Empty);
        }

        public List<int> GetNodeIdListByScopeType(NodeInfo nodeInfo, EScopeType scopeType, string group, string groupNot)
        {
            return GetNodeIdListByScopeType(nodeInfo, scopeType, group, groupNot, string.Empty);
        }

        public List<int> GetNodeIdListByScopeType(NodeInfo nodeInfo, EScopeType scopeType, string group, string groupNot, string contentModelId)
        {
            if (nodeInfo == null) return new List<int>();

            var list = new List<int>();

            if (nodeInfo.ChildrenCount == 0)
            {
                if (scopeType != EScopeType.Children && scopeType != EScopeType.Descendant)
                {
                    list.Add(nodeInfo.NodeId);
                }

                return list;
            }

            string sqlString = null;
            var whereString = GetGroupWhereString(group, groupNot);
            if (!string.IsNullOrEmpty(contentModelId))
            {
                whereString += $" AND ContentModelID = '{contentModelId}'";
            }
            if (scopeType == EScopeType.All)
            {
                sqlString = $@"SELECT NodeID
FROM siteserver_Node 
WHERE ((NodeID = {nodeInfo.NodeId}) OR
      (ParentID = {nodeInfo.NodeId}) OR
      (ParentsPath = '{nodeInfo.NodeId}') OR
      (ParentsPath LIKE '{nodeInfo.NodeId},%') OR
      (ParentsPath LIKE '%,{nodeInfo.NodeId},%') OR
      (ParentsPath LIKE '%,{nodeInfo.NodeId}')) {whereString}
ORDER BY Taxis";
            }
            else if (scopeType == EScopeType.Self)
            {
                list.Add(nodeInfo.NodeId);
                return list;
            }
            else if (scopeType == EScopeType.Children)
            {
                sqlString = $@"SELECT NodeID
FROM siteserver_Node 
WHERE (ParentID = {nodeInfo.NodeId}) {whereString}
ORDER BY Taxis";
            }
            else if (scopeType == EScopeType.Descendant)
            {
                sqlString = $@"SELECT NodeID
FROM siteserver_Node 
WHERE ((ParentID = {nodeInfo.NodeId}) OR
      (ParentsPath = '{nodeInfo.NodeId}') OR
      (ParentsPath LIKE '{nodeInfo.NodeId},%') OR
      (ParentsPath LIKE '%,{nodeInfo.NodeId},%') OR
      (ParentsPath LIKE '%,{nodeInfo.NodeId}')) {whereString}
ORDER BY Taxis";
            }
            else if (scopeType == EScopeType.SelfAndChildren)
            {
                sqlString = $@"SELECT NodeID
FROM siteserver_Node 
WHERE ((NodeID = {nodeInfo.NodeId}) OR
      (ParentID = {nodeInfo.NodeId})) {whereString}
ORDER BY Taxis";
            }

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }

            return list;
        }

        public List<int> GetNodeIdListForDescendant(int nodeId)
        {
            string sqlString = $@"SELECT NodeID
FROM siteserver_Node
WHERE (ParentsPath LIKE '{nodeId},%') OR
      (ParentsPath LIKE '%,{nodeId},%') OR
      (ParentsPath LIKE '%,{nodeId}') OR
      (ParentID = {nodeId})
";
            var list = new List<int>();

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }

            return list;
        }

        public List<int> GetNodeIdListByParentId(int publishmentSystemId, int parentId)
        {
            string sqlString;
            if (parentId == 0)
            {
                sqlString = $@"SELECT NodeID
FROM siteserver_Node
WHERE (NodeID = {publishmentSystemId} OR ParentID = {publishmentSystemId})
ORDER BY Taxis";
            }
            else
            {
                sqlString = $@"SELECT NodeID
FROM siteserver_Node
WHERE (ParentID = {parentId})
ORDER BY Taxis";
            }

            var list = new List<int>();

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }

            return list;
        }

        public List<NodeInfo> GetNodeInfoListByParentId(int publishmentSystemId, int parentId)
        {
            var sqlString = $@"SELECT NodeID, NodeName, NodeType, PublishmentSystemID, ContentModelID, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, NodeIndexName, NodeGroupNameCollection, Taxis, AddDate, ImageUrl, Content, ContentNum, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateID, ContentTemplateID, Keywords, Description, ExtendValues FROM siteserver_Node 
WHERE (PublishmentSystemID={publishmentSystemId} AND ParentID = {parentId})
ORDER BY Taxis";

            var list = new List<NodeInfo>();
            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var node = new NodeInfo(GetInt(rdr, i++), GetString(rdr, i++), ENodeTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), ELinkTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                    list.Add(node);
                }
                rdr.Close();
            }

            return list;
        }

        public List<int> GetNodeIdListByPublishmentSystemId(int publishmentSystemId)
        {
            string sqlString = $@"SELECT NodeID
FROM siteserver_Node
WHERE PublishmentSystemID = {publishmentSystemId} AND (NodeID = {publishmentSystemId} OR ParentID > 0)
ORDER BY Taxis";
            var list = new List<int>();

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }

            return list;
        }

        public Hashtable GetNodeInfoHashtableByPublishmentSystemId(int publishmentSystemId)
        {
            var ht = new Hashtable();
            string sqlString =
                $@"SELECT NodeID, NodeName, NodeType, PublishmentSystemID, ContentModelID, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, NodeIndexName, NodeGroupNameCollection, Taxis, AddDate, ImageUrl, Content, ContentNum, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateID, ContentTemplateID, Keywords, Description, ExtendValues
FROM siteserver_Node 
WHERE (PublishmentSystemID = {publishmentSystemId} AND (NodeID = {publishmentSystemId} OR ParentID > 0))
ORDER BY Taxis";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var nodeInfo = new NodeInfo(GetInt(rdr, i++), GetString(rdr, i++), ENodeTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), ELinkTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                    ht.Add(nodeInfo.NodeId, nodeInfo);
                }
                rdr.Close();
            }

            return ht;
        }

        public IEnumerable GetDataSource(List<int> nodeIdList, int totalNum, string whereString, string orderByString)
        {
            if (nodeIdList.Count == 0)
            {
                return null;
            }
            string sqlString;

            if (totalNum > 0)
            {
//                sqlString =
//                    $@"SELECT TOP {totalNum} NodeID, NodeName, NodeType, PublishmentSystemID, ContentModelID, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, NodeIndexName, NodeGroupNameCollection, Taxis, AddDate, ImageUrl, Content, ContentNum, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateID, ContentTemplateID, Keywords, Description, ExtendValues
//FROM siteserver_Node
//WHERE (NodeID IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)}) {whereString}) {orderByString}
//";
                sqlString = SqlUtils.GetTopSqlString(TableName,
                    "NodeID, NodeName, NodeType, PublishmentSystemID, ContentModelID, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, NodeIndexName, NodeGroupNameCollection, Taxis, AddDate, ImageUrl, Content, ContentNum, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateID, ContentTemplateID, Keywords, Description, ExtendValues",
                    $"WHERE (NodeID IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)}) {whereString}) {orderByString}",
                    totalNum);
            }
            else
            {
                sqlString =
                    $@"SELECT NodeID, NodeName, NodeType, PublishmentSystemID, ContentModelID, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, NodeIndexName, NodeGroupNameCollection, Taxis, AddDate, ImageUrl, Content, ContentNum, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateID, ContentTemplateID, Keywords, Description, ExtendValues
FROM siteserver_Node
WHERE (NodeID IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)}) {whereString}) {orderByString}
";
            }

            var enumerable = (IEnumerable)ExecuteReader(sqlString);
            return enumerable;
        }

        public DataSet GetDataSet(List<int> nodeIdList, int totalNum, string whereString, string orderByString)
        {
            if (nodeIdList.Count == 0)
            {
                return null;
            }
            string sqlString;

            if (totalNum > 0)
            {
//                sqlString =
//                    $@"SELECT TOP {totalNum} NodeID, NodeName, NodeType, PublishmentSystemID, ContentModelID, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, NodeIndexName, NodeGroupNameCollection, Taxis, AddDate, ImageUrl, Content, ContentNum, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateID, ContentTemplateID, Keywords, Description, ExtendValues
//FROM siteserver_Node
//WHERE (NodeID IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)}) {whereString}) {orderByString}
//";
                sqlString = SqlUtils.GetTopSqlString(TableName,
                    "NodeID, NodeName, NodeType, PublishmentSystemID, ContentModelID, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, NodeIndexName, NodeGroupNameCollection, Taxis, AddDate, ImageUrl, Content, ContentNum, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateID, ContentTemplateID, Keywords, Description, ExtendValues",
                    $"WHERE (NodeID IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)}) {whereString}) {orderByString}",
                    totalNum);
            }
            else
            {
                sqlString =
                    $@"SELECT NodeID, NodeName, NodeType, PublishmentSystemID, ContentModelID, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, NodeIndexName, NodeGroupNameCollection, Taxis, AddDate, ImageUrl, Content, ContentNum, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateID, ContentTemplateID, Keywords, Description, ExtendValues
FROM siteserver_Node
WHERE (NodeID IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)}) {whereString}) {orderByString}
";
            }

            var dataSet = ExecuteDataset(sqlString);
            return dataSet;
        }

        public IEnumerable GetStlDataSource(NodeInfo nodeInfo, int startNum, int totalNum, string whereString, EScopeType scopeType, string orderByString)
        {
            var nodeIdList = StlCacheManager.NodeIdList.GetNodeIdListByScopeType(nodeInfo, scopeType);

            if (nodeIdList == null || nodeIdList.Count == 0)
            {
                return null;
            }

            string sqlWhereString =
                $"WHERE (NodeID IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)}) {whereString})";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, startNum, totalNum, "NodeID, AddDate, Taxis", sqlWhereString, orderByString);

            return (IEnumerable)ExecuteReader(sqlSelect);
        }

        public IEnumerable GetStlDataSourceByPublishmentSystemId(int publishmentSystemId, int startNum, int totalNum, string whereString, string orderByString)
        {
            string sqlWhereString = $"WHERE (PublishmentSystemID = {publishmentSystemId} {whereString})";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, startNum, totalNum, "NodeID, AddDate, Taxis", sqlWhereString, orderByString);

            return (IEnumerable)ExecuteReader(sqlSelect);
        }

        public DataSet GetStlDataSet(NodeInfo nodeInfo, int startNum, int totalNum, string whereString, EScopeType scopeType, string orderByString)
        {
            var nodeIdList = StlCacheManager.NodeIdList.GetNodeIdListByScopeType(nodeInfo, scopeType);
            if (nodeIdList == null || nodeIdList.Count == 0)
            {
                return null;
            }

            string sqlWhereString =
                $"WHERE (NodeID IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)}) {whereString})";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, startNum, totalNum, "NodeID, AddDate, Taxis", sqlWhereString, orderByString);

            return ExecuteDataset(sqlSelect);
        }

        public DataSet GetStlDataSetByPublishmentSystemId(int publishmentSystemId, int startNum, int totalNum, string whereString, string orderByString)
        {
            string sqlWhereString = $"WHERE (PublishmentSystemID = {publishmentSystemId} {whereString})";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, startNum, totalNum, "NodeID, AddDate, Taxis", sqlWhereString, orderByString);

            return ExecuteDataset(sqlSelect);
        }

        public List<NodeInfo> GetNodeInfoListByPublishmentSystemId(int publishmentSystemId, string whereString)
        {
            var list = new List<NodeInfo>();
            string cmd =
                $@"SELECT NodeID, NodeName, NodeType, PublishmentSystemID, ContentModelID, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, NodeIndexName, NodeGroupNameCollection, Taxis, AddDate, ImageUrl, Content, ContentNum, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateID, ContentTemplateID, Keywords, Description, ExtendValues
FROM siteserver_Node 
WHERE (PublishmentSystemID = {publishmentSystemId} AND (NodeID = {publishmentSystemId} OR ParentID > 0))
{whereString}
ORDER BY Taxis";

            using (var rdr = ExecuteReader(cmd))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var nodeInfo = new NodeInfo(GetInt(rdr, i++), GetString(rdr, i++), ENodeTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), ELinkTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                    list.Add(nodeInfo);
                }
                rdr.Close();
            }
            return list;
        }

        public List<NodeInfo> GetNodeInfoList(NodeInfo nodeInfo, int totalNum, string whereString, EScopeType scopeType, string orderByString)
        {
            var nodeIdList = GetNodeIdListByScopeType(nodeInfo, scopeType);
            if (nodeIdList == null || nodeIdList.Count == 0)
            {
                return null;
            }

            string sqlString;
            if (totalNum > 0)
            {
//                sqlString =
//                    $@"SELECT TOP {totalNum} NodeID, NodeName, NodeType, PublishmentSystemID, ContentModelID, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, NodeIndexName, NodeGroupNameCollection, Taxis, AddDate, ImageUrl, Content, ContentNum, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateID, ContentTemplateID, Keywords, Description, ExtendValues
//FROM siteserver_Node 
//WHERE (NodeID IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)}) {whereString}) {orderByString}
//";
                sqlString = SqlUtils.GetTopSqlString(TableName,
                    "NodeID, NodeName, NodeType, PublishmentSystemID, ContentModelID, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, NodeIndexName, NodeGroupNameCollection, Taxis, AddDate, ImageUrl, Content, ContentNum, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateID, ContentTemplateID, Keywords, Description, ExtendValues",
                    $"WHERE (NodeID IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)}) {whereString}) {orderByString}",
                    totalNum);
            }
            else
            {
                sqlString =
                    $@"SELECT NodeID, NodeName, NodeType, PublishmentSystemID, ContentModelID, ParentID, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, NodeIndexName, NodeGroupNameCollection, Taxis, AddDate, ImageUrl, Content, ContentNum, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateID, ContentTemplateID, Keywords, Description, ExtendValues
FROM siteserver_Node 
WHERE (NodeID IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)}) {whereString}) {orderByString}
";
            }

            var list = new List<NodeInfo>();

            var nodeParms = new IDataParameter[]
            {
                GetParameter(ParmNodeId, EDataType.Integer, nodeInfo.NodeId)
            };

            using (var rdr = ExecuteReader(sqlString, nodeParms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var node = new NodeInfo(GetInt(rdr, i++), GetString(rdr, i++), ENodeTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), ELinkTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                    list.Add(node);
                }
                rdr.Close();
            }
            return list;
        }

        /// <summary>
        /// 得到所有系统文件夹的列表，以小写表示。
        /// </summary>
        public List<string> GetLowerSystemDirList(int parentPublishmentSystemId)
        {
            var list = new List<string>();
            var sqlString = "SELECT PublishmentSystemDir FROM siteserver_PublishmentSystem WHERE ParentPublishmentSystemID = " + parentPublishmentSystemId;

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetString(rdr, 0).ToLower());
                }
                rdr.Close();
            }

            return list;
        }

        public int GetContentNumByPublishmentSystemId(int publishmentSystemId)
        {
            string sqlString =
                $"SELECT SUM(ContentNum) FROM siteserver_Node WHERE (PublishmentSystemID = {publishmentSystemId})";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<string> GetAllFilePathByPublishmentSystemId(int publishmentSystemId)
        {
            var list = new List<string>();

            var sqlString =
                $"SELECT FilePath FROM siteserver_Node WHERE FilePath <> '' AND PublishmentSystemID = {publishmentSystemId}";

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

        public List<int> GetNodeIdListByChildNodeId(int publishmentSystemId, int childNodeId, List<int> nodeIdList)
        {
            var sqlString = $@"SELECT NodeID,ParentID FROM siteserver_Node 
WHERE (PublishmentSystemID={publishmentSystemId} AND NodeID={childNodeId})
ORDER BY Taxis";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var nodeId = GetInt(rdr, 0);
                    var parentNodeId = GetInt(rdr, 1);
                    nodeIdList.Add(nodeId);
                    GetNodeIdListByChildNodeId(publishmentSystemId, parentNodeId, nodeIdList);

                }
                rdr.Close();
            }
            return nodeIdList;
        }
    }
}
