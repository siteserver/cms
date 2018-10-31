using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Provider
{
    public class ChannelDao : DataProviderBase
    {
        private string SqlColumns => $"{ChannelAttribute.Id}, {ChannelAttribute.AddDate}, {ChannelAttribute.Taxis}";

        public override string TableName => "siteserver_Channel";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(ChannelInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelInfo.ChannelName),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelInfo.SiteId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelInfo.ContentModelPluginId),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelInfo.ContentRelatedPluginIds),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelInfo.ParentId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelInfo.ParentsPath),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelInfo.ParentsCount),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelInfo.ChildrenCount),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelInfo.IsLastNode),
                DataType = DataType.VarChar,
                DataLength = 18
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelInfo.IndexName),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelInfo.GroupNameCollection),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelInfo.Taxis),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelInfo.AddDate),
                DataType = DataType.DateTime
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelInfo.ImageUrl),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelInfo.Content),
                DataType = DataType.Text
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelInfo.FilePath),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelInfo.ChannelFilePathRule),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelInfo.ContentFilePathRule),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelInfo.LinkUrl),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelInfo.LinkType),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelInfo.ChannelTemplateId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelInfo.ContentTemplateId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelInfo.Keywords),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(ChannelInfo.Description),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = "ExtendValues",
                DataType = DataType.Text
            }
        };

        private const string SqlSelectByLastAddDate = "SELECT Id, ChannelName, SiteId, ContentModelPluginId, ContentRelatedPluginIds, ParentId, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, IndexName, GroupNameCollection, Taxis, AddDate, ImageUrl, Content, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateId, ContentTemplateId, Keywords, Description, ExtendValues FROM siteserver_Channel WHERE ParentId = @ParentId ORDER BY AddDate Desc";

        private const string SqlSelectByTaxis = "SELECT Id, ChannelName, SiteId, ContentModelPluginId, ContentRelatedPluginIds, ParentId, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, IndexName, GroupNameCollection, Taxis, AddDate, ImageUrl, Content, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateId, ContentTemplateId, Keywords, Description, ExtendValues FROM siteserver_Channel WHERE ParentId = @ParentId ORDER BY Taxis";

        private const string SqlSelectParentId = "SELECT ParentId FROM siteserver_Channel WHERE Id = @Id";

        private const string SqlSelectCount = "SELECT COUNT(*) FROM siteserver_Channel WHERE ParentId = @ParentId";

        private const string SqlSelectSiteIdById = "SELECT SiteId FROM siteserver_Channel WHERE Id = @Id";

        private const string SqlSelectIndexNameCollection = "SELECT DISTINCT IndexName FROM siteserver_Channel WHERE SiteId = @SiteId";

        private const string SqlUpdate = "UPDATE siteserver_Channel SET ChannelName = @ChannelName, ContentModelPluginId = @ContentModelPluginId, ContentRelatedPluginIds = @ContentRelatedPluginIds, ParentsPath = @ParentsPath, ParentsCount = @ParentsCount, ChildrenCount = @ChildrenCount, IsLastNode = @IsLastNode, IndexName = @IndexName, GroupNameCollection = @GroupNameCollection, ImageUrl = @ImageUrl, Content = @Content, FilePath = @FilePath, ChannelFilePathRule = @ChannelFilePathRule, ContentFilePathRule = @ContentFilePathRule, LinkUrl = @LinkUrl,LinkType = @LinkType, ChannelTemplateId = @ChannelTemplateId, ContentTemplateId = @ContentTemplateId, Keywords = @Keywords, Description = @Description, ExtendValues = @ExtendValues WHERE Id = @Id";

        private const string SqlUpdateExtendValues = "UPDATE siteserver_Channel SET ExtendValues = @ExtendValues WHERE Id = @Id";

        private const string SqlUpdateGroupNameCollection = "UPDATE siteserver_Channel SET GroupNameCollection = @GroupNameCollection WHERE Id = @Id";

        private const string ParmId = "@Id";
        private const string ParmChannelName = "@ChannelName";
        private const string ParmSiteId = "@SiteId";
        private const string ParmContentModelPluginId = "@ContentModelPluginId";
        private const string ParmContentRelatedPluginIds = "@ContentRelatedPluginIds";
        private const string ParmParentId = "@ParentId";
        private const string ParmParentsPath = "@ParentsPath";
        private const string ParmParentsCount = "@ParentsCount";
        private const string ParmChildrenCount = "@ChildrenCount";
        private const string ParmIsLastNode = "@IsLastNode";
        private const string ParmIndexName = "@IndexName";
        private const string ParmGroupNameCollection = "@GroupNameCollection";
        private const string ParmTaxis = "@Taxis";
        private const string ParmAddDate = "@AddDate";
        private const string ParmImageUrl = "@ImageUrl";
        private const string ParmContent = "@Content";
        private const string ParmFilePath = "@FilePath";
        private const string ParmChannelFilePathRule = "@ChannelFilePathRule";
        private const string ParmContentFilePathRule = "@ContentFilePathRule";
        private const string ParmLinkUrl = "@LinkUrl";
        private const string ParmLinkType = "@LinkType";
        private const string ParmChannelTemplateId = "@ChannelTemplateId";
        private const string ParmContentTemplateId = "@ContentTemplateId";
        private const string ParmKeywords = "@Keywords";
        private const string ParmDescription = "@Description";
        private const string ParmExtendValues = "@ExtendValues";

        private void InsertChannelInfoWithTrans(IChannelInfo parentChannelInfo, IChannelInfo channelInfo, IDbTransaction trans)
        {
            if (parentChannelInfo != null)
            {
                channelInfo.SiteId = parentChannelInfo.SiteId;
                if (parentChannelInfo.ParentsPath.Length == 0)
                {
                    channelInfo.ParentsPath = parentChannelInfo.Id.ToString();
                }
                else
                {
                    channelInfo.ParentsPath = parentChannelInfo.ParentsPath + "," + parentChannelInfo.Id;
                }
                channelInfo.ParentsCount = parentChannelInfo.ParentsCount + 1;

                var maxTaxis = GetMaxTaxisByParentPath(channelInfo.ParentsPath);
                if (maxTaxis == 0)
                {
                    maxTaxis = parentChannelInfo.Taxis;
                }
                channelInfo.Taxis = maxTaxis + 1;
            }
            else
            {
                channelInfo.Taxis = 1;
            }

            const string sqlInsertNode = "INSERT INTO siteserver_Channel (ChannelName, SiteId, ContentModelPluginId, ContentRelatedPluginIds, ParentId, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, IndexName, GroupNameCollection, Taxis, AddDate, ImageUrl, Content, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateId, ContentTemplateId, Keywords, Description, ExtendValues) VALUES (@ChannelName, @SiteId, @ContentModelPluginId, @ContentRelatedPluginIds, @ParentId, @ParentsPath, @ParentsCount, @ChildrenCount, @IsLastNode, @IndexName, @GroupNameCollection, @Taxis, @AddDate, @ImageUrl, @Content, @FilePath, @ChannelFilePathRule, @ContentFilePathRule, @LinkUrl, @LinkType, @ChannelTemplateId, @ContentTemplateId, @Keywords, @Description, @ExtendValues)";

            var insertParms = new IDataParameter[]
            {
                GetParameter(ParmChannelName, DataType.VarChar, 255, channelInfo.ChannelName),
                GetParameter(ParmSiteId, DataType.Integer, channelInfo.SiteId),
                GetParameter(ParmContentModelPluginId, DataType.VarChar, 50, channelInfo.ContentModelPluginId),
                GetParameter(ParmContentRelatedPluginIds, DataType.VarChar, 255, channelInfo.ContentRelatedPluginIds),
                GetParameter(ParmParentId, DataType.Integer, channelInfo.ParentId),
                GetParameter(ParmParentsPath, DataType.VarChar, 255, channelInfo.ParentsPath),
                GetParameter(ParmParentsCount, DataType.Integer, channelInfo.ParentsCount),
                GetParameter(ParmChildrenCount, DataType.Integer, 0),
                GetParameter(ParmIsLastNode, DataType.VarChar, 18, true.ToString()),
                GetParameter(ParmIndexName, DataType.VarChar, 255, channelInfo.IndexName),
                GetParameter(ParmGroupNameCollection, DataType.VarChar, 255, channelInfo.GroupNameCollection),
                GetParameter(ParmTaxis, DataType.Integer, channelInfo.Taxis),
                GetParameter(ParmAddDate, DataType.DateTime, channelInfo.AddDate),
                GetParameter(ParmImageUrl, DataType.VarChar, 200, channelInfo.ImageUrl),
                GetParameter(ParmContent, DataType.Text, channelInfo.Content),
                GetParameter(ParmFilePath, DataType.VarChar, 200, channelInfo.FilePath),
                GetParameter(ParmChannelFilePathRule, DataType.VarChar, 200, channelInfo.ChannelFilePathRule),
                GetParameter(ParmContentFilePathRule, DataType.VarChar, 200, channelInfo.ContentFilePathRule),
                GetParameter(ParmLinkUrl, DataType.VarChar, 200, channelInfo.LinkUrl),
                GetParameter(ParmLinkType, DataType.VarChar, 200, channelInfo.LinkType),
                GetParameter(ParmChannelTemplateId, DataType.Integer, channelInfo.ChannelTemplateId),
                GetParameter(ParmContentTemplateId, DataType.Integer, channelInfo.ContentTemplateId),
                GetParameter(ParmKeywords, DataType.VarChar, 255, channelInfo.Keywords),
                GetParameter(ParmDescription, DataType.VarChar, 255, channelInfo.Description),
                GetParameter(ParmExtendValues, DataType.Text, channelInfo.Attributes.ToString())
            };

            if (channelInfo.SiteId != 0)
            {
                string sqlString =
                    $"UPDATE siteserver_Channel SET {SqlUtils.ToPlusSqlString("Taxis")} WHERE (Taxis >= {channelInfo.Taxis}) AND (SiteId = {channelInfo.SiteId})";
                ExecuteNonQuery(trans, sqlString);
            }
            channelInfo.Id = ExecuteNonQueryAndReturnId(TableName, nameof(ChannelInfo.Id), trans, sqlInsertNode, insertParms);

            if (!string.IsNullOrEmpty(channelInfo.ParentsPath))
            {
                var sqlString = $"UPDATE siteserver_Channel SET {SqlUtils.ToPlusSqlString("ChildrenCount")} WHERE Id IN ({channelInfo.ParentsPath})";

                ExecuteNonQuery(trans, sqlString);
            }

            var sqlUpdateIsLastNode = "UPDATE siteserver_Channel SET IsLastNode = @IsLastNode WHERE ParentId = @ParentId";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmIsLastNode, DataType.VarChar, 18, false.ToString()),
                GetParameter(ParmParentId, DataType.Integer, channelInfo.ParentId)
            };

            ExecuteNonQuery(trans, sqlUpdateIsLastNode, parms);

            //sqlUpdateIsLastNode =
            //    $"UPDATE siteserver_Channel SET IsLastNode = '{true}' WHERE (Id IN (SELECT TOP 1 Id FROM siteserver_Channel WHERE ParentId = {channelInfo.ParentId} ORDER BY Taxis DESC))";
            sqlUpdateIsLastNode =
                $"UPDATE siteserver_Channel SET IsLastNode = '{true}' WHERE (Id IN ({SqlUtils.ToInTopSqlString(TableName, "Id", $"WHERE ParentId = {channelInfo.ParentId}", "ORDER BY Taxis DESC", 1)}))";


            ExecuteNonQuery(trans, sqlUpdateIsLastNode);

            //OwningIdCache.IsChanged = true;
            ChannelManager.RemoveCacheBySiteId(channelInfo.SiteId);
            PermissionsImpl.ClearAllCache();
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
                var sqlString = string.Concat("UPDATE siteserver_Channel SET ChildrenCount = ChildrenCount - ", subtractNum, " WHERE Id in (", parentsPath, ")");
                ExecuteNonQuery(sqlString);
            }
        }

        /// <summary>
        /// 更新发布系统下的所有节点的排序号
        /// </summary>
        /// <param name="siteId"></param>
        private void UpdateWholeTaxisBySiteId(int siteId)
        {
            if (siteId <= 0) return;
            var idList = new List<int>
            {
                siteId
            };
            var level = 0;
            string selectLevelCmd =
                $"SELECT MAX(ParentsCount) FROM siteserver_Channel WHERE (Id = {siteId}) OR (SiteId = {siteId})";
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
                var list = new List<int>(idList);
                foreach (var savedId in list)
                {
                    var lastChildIdOfSavedId = savedId;
                    var sqlString =
                        $"SELECT Id, ChannelName FROM siteserver_Channel WHERE ParentId = {savedId} ORDER BY Taxis, IsLastNode";
                    using (var rdr = ExecuteReader(sqlString))
                    {
                        while (rdr.Read())
                        {
                            var channelId = GetInt(rdr, 0);
                            if (!idList.Contains(channelId))
                            {
                                var index = idList.IndexOf(lastChildIdOfSavedId);
                                idList.Insert(index + 1, channelId);
                                lastChildIdOfSavedId = channelId;
                            }
                        }
                        rdr.Close();
                    }
                }
            }

            for (var i = 1; i <= idList.Count; i++)
            {
                var channelId = idList[i - 1];
                string updateCmd = $"UPDATE siteserver_Channel SET Taxis = {i} WHERE Id = {channelId}";
                ExecuteNonQuery(updateCmd);
            }
        }

        /// <summary>
        /// Change The Texis To Lowerer Level
        /// </summary>
        private void TaxisSubtract(int siteId, int selectedId)
        {
            var channelInfo = ChannelManager.GetChannelInfo(siteId, selectedId);
            if (channelInfo == null || channelInfo.ParentId == 0 || channelInfo.SiteId == 0) return;
            //UpdateWholeTaxisBySiteId(channelInfo.SiteId);
            //Get Lower Taxis and Id
            int lowerId;
            int lowerChildrenCount;
            string lowerParentsPath;
            //            var sqlString = @"SELECT TOP 1 Id, ChildrenCount, ParentsPath
            //FROM siteserver_Channel
            //WHERE (ParentId = @ParentId) AND (Id <> @Id) AND (Taxis < @Taxis) AND (SiteId = @SiteId)
            //ORDER BY Taxis DESC";
            var sqlString = SqlUtils.ToTopSqlString(TableName, "Id, ChildrenCount, ParentsPath", "WHERE (ParentId = @ParentId) AND (Id <> @Id) AND (Taxis < @Taxis) AND (SiteId = @SiteId)", "ORDER BY Taxis DESC", 1);

            var parms = new IDataParameter[]
            {
                GetParameter(ParmParentId, DataType.Integer, channelInfo.ParentId),
                GetParameter(ParmId, DataType.Integer, channelInfo.Id),
                GetParameter(ParmTaxis, DataType.Integer, channelInfo.Taxis),
                GetParameter(ParmSiteId, DataType.Integer, channelInfo.SiteId)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    lowerId = GetInt(rdr, 0);
                    lowerChildrenCount = GetInt(rdr, 1);
                    lowerParentsPath = GetString(rdr, 2);
                }
                else
                {
                    return;
                }
                rdr.Close();
            }

            var lowerPath = lowerParentsPath == "" ? lowerId.ToString() : string.Concat(lowerParentsPath, ",", lowerId);
            var selectedPath = channelInfo.ParentsPath == "" ? channelInfo.Id.ToString() : string.Concat(channelInfo.ParentsPath, ",", channelInfo.Id);

            SetTaxisSubtract(selectedId, selectedPath, lowerChildrenCount + 1);
            SetTaxisAdd(lowerId, lowerPath, channelInfo.ChildrenCount + 1);

            UpdateIsLastNode(channelInfo.ParentId);
        }

        /// <summary>
        /// Change The Texis To Higher Level
        /// </summary>
        private void TaxisAdd(int siteId, int selectedId)
        {
            var channelInfo = ChannelManager.GetChannelInfo(siteId, selectedId);
            if (channelInfo == null || channelInfo.ParentId == 0 || channelInfo.SiteId == 0) return;
            //UpdateWholeTaxisBySiteId(channelInfo.SiteId);
            //Get Higher Taxis and Id
            int higherId;
            int higherChildrenCount;
            string higherParentsPath;
            //const string sqlString = @"SELECT TOP 1 Id, ChildrenCount, ParentsPath
            //FROM siteserver_Channel
            //WHERE (ParentId = @ParentId) AND (Id <> @Id) AND (Taxis > @Taxis) AND (SiteId = @SiteId) ORDER BY Taxis";
            var sqlString = SqlUtils.ToTopSqlString(TableName, "Id, ChildrenCount, ParentsPath", "WHERE (ParentId = @ParentId) AND (Id <> @Id) AND (Taxis > @Taxis) AND (SiteId = @SiteId)", "ORDER BY Taxis", 1);

            var parms = new IDataParameter[]
            {
                GetParameter(ParmParentId, DataType.Integer, channelInfo.ParentId),
                GetParameter(ParmId, DataType.Integer, channelInfo.Id),
                GetParameter(ParmTaxis, DataType.Integer, channelInfo.Taxis),
                GetParameter(ParmSiteId, DataType.Integer, channelInfo.SiteId)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    higherId = GetInt(rdr, 0);
                    higherChildrenCount = GetInt(rdr, 1);
                    higherParentsPath = GetString(rdr, 2);
                }
                else
                {
                    return;
                }
                rdr.Close();
            }


            var higherPath = higherParentsPath == string.Empty ? higherId.ToString() : string.Concat(higherParentsPath, ",", higherId);
            var selectedPath = channelInfo.ParentsPath == string.Empty ? channelInfo.Id.ToString() : String.Concat(channelInfo.ParentsPath, ",", channelInfo.Id);

            SetTaxisAdd(selectedId, selectedPath, higherChildrenCount + 1);
            SetTaxisSubtract(higherId, higherPath, channelInfo.ChildrenCount + 1);

            UpdateIsLastNode(channelInfo.ParentId);
        }

        private void SetTaxisAdd(int channelId, string parentsPath, int addNum)
        {
            string sqlString =
                $"UPDATE siteserver_Channel SET {SqlUtils.ToPlusSqlString("Taxis", addNum)} WHERE Id = {channelId} OR ParentsPath = '{parentsPath}' OR ParentsPath like '{parentsPath},%'";

            ExecuteNonQuery(sqlString);
        }

        private void SetTaxisSubtract(int channelId, string parentsPath, int subtractNum)
        {
            string sqlString =
                $"UPDATE siteserver_Channel SET {SqlUtils.ToMinusSqlString("Taxis", subtractNum)} WHERE  Id = {channelId} OR ParentsPath = '{parentsPath}' OR ParentsPath like '{parentsPath},%'";

            ExecuteNonQuery(sqlString);
        }

        private void UpdateIsLastNode(int parentId)
        {
            if (parentId <= 0) return;

            var sqlString = "UPDATE siteserver_Channel SET IsLastNode = @IsLastNode WHERE  ParentId = @ParentId";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmIsLastNode, DataType.VarChar, 18, false.ToString()),
                GetParameter(ParmParentId, DataType.Integer, parentId)
            };

            ExecuteNonQuery(sqlString, parms);

            //sqlString =
            //    $"UPDATE siteserver_Channel SET IsLastNode = '{true}' WHERE (Id IN (SELECT TOP 1 Id FROM siteserver_Channel WHERE ParentId = {parentId} ORDER BY Taxis DESC))";
            sqlString =
                $"UPDATE siteserver_Channel SET IsLastNode = '{true}' WHERE (Id IN ({SqlUtils.ToInTopSqlString(TableName, "Id", $"WHERE ParentId = {parentId}", "ORDER BY Taxis DESC", 1)}))";

            ExecuteNonQuery(sqlString);
        }

        private int GetMaxTaxisByParentPath(string parentPath)
        {
            var cmd = string.Concat("SELECT MAX(Taxis) AS MaxTaxis FROM siteserver_Channel WHERE (ParentsPath = '", parentPath, "') OR (ParentsPath like '", parentPath, ",%')");
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

        private int GetParentId(int channelId)
        {
            var parentId = 0;

            var parms = new IDataParameter[]
            {
                GetParameter(ParmId, DataType.Integer, channelId)
            };

            using (var rdr = ExecuteReader(SqlSelectParentId, parms))
            {
                if (rdr.Read())
                {
                    parentId = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return parentId;
        }

        private int GetIdByParentIdAndOrder(int parentId, int order)
        {
            var channelId = parentId;

            string cmd = $"SELECT Id FROM siteserver_Channel WHERE (ParentId = {parentId}) ORDER BY Taxis";

            using (var rdr = ExecuteReader(cmd))
            {
                var index = 1;
                while (rdr.Read())
                {
                    channelId = GetInt(rdr, 0);
                    if (index == order)
                        break;
                    index++;
                }
                rdr.Close();
            }
            return channelId;
        }

        public int Insert(int siteId, int parentId, string channelName, string indexName, string contentModelPluginId, string contentRelatedPluginIds, int channelTemplateId, int contentTemplateId)
        {
            if (siteId > 0 && parentId == 0) return 0;

            var defaultChannelTemplateInfo = TemplateManager.GetDefaultTemplateInfo(siteId, TemplateType.ChannelTemplate);
            var defaultContentTemplateInfo = TemplateManager.GetDefaultTemplateInfo(siteId, TemplateType.ContentTemplate);

            var channelInfo = new ChannelInfo
            {
                ParentId = parentId,
                SiteId = siteId,
                ChannelName = channelName,
                IndexName = indexName,
                ContentModelPluginId = contentModelPluginId,
                ContentRelatedPluginIds = contentRelatedPluginIds,
                AddDate = DateTime.Now,
                ChannelTemplateId = channelTemplateId > 0 ? channelTemplateId : defaultChannelTemplateInfo.Id,
                ContentTemplateId = contentTemplateId > 0 ? contentTemplateId : defaultContentTemplateInfo.Id
            };

            var parentChannelInfo = ChannelManager.GetChannelInfo(siteId, parentId);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        InsertChannelInfoWithTrans(parentChannelInfo, channelInfo, trans);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return channelInfo.Id;

        }

        public int Insert(IChannelInfo channelInfo)
        {
            if (channelInfo.SiteId > 0 && channelInfo.ParentId == 0) return 0;

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var parentChannelInfo = ChannelManager.GetChannelInfo(channelInfo.SiteId, channelInfo.ParentId);

                        InsertChannelInfoWithTrans(parentChannelInfo, channelInfo, trans);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return channelInfo.Id;
        }

        /// <summary>
        /// 添加后台发布节点
        /// </summary>
        public int InsertSiteInfo(ChannelInfo channelInfo, SiteInfo siteInfo, string administratorName)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        InsertChannelInfoWithTrans(null, channelInfo, trans);

                        siteInfo.Id = channelInfo.Id;

                        DataProvider.SiteDao.InsertWithTrans(siteInfo, trans);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            var adminInfo = AdminManager.GetAdminInfoByUserName(administratorName);
            DataProvider.AdministratorDao.UpdateSiteId(adminInfo, channelInfo.Id);

            var sqlString =
                $"UPDATE siteserver_Channel SET SiteId = {channelInfo.Id} WHERE Id = {channelInfo.Id}";
            ExecuteNonQuery(sqlString);

            DataProvider.TemplateDao.CreateDefaultTemplateInfo(channelInfo.Id, administratorName);
            return channelInfo.Id;
        }

        public void Update(ChannelInfo channelInfo)
        {
            var updateParms = new IDataParameter[]
            {
                GetParameter(ParmChannelName, DataType.VarChar, 255, channelInfo.ChannelName),
                GetParameter(ParmContentModelPluginId, DataType.VarChar, 50, channelInfo.ContentModelPluginId),
                GetParameter(ParmContentRelatedPluginIds, DataType.VarChar, 255, channelInfo.ContentRelatedPluginIds),
                GetParameter(ParmParentsPath, DataType.VarChar, 255, channelInfo.ParentsPath),
                GetParameter(ParmParentsCount, DataType.Integer, channelInfo.ParentsCount),
                GetParameter(ParmChildrenCount, DataType.Integer, channelInfo.ChildrenCount),
                GetParameter(ParmIsLastNode, DataType.VarChar, 18, channelInfo.IsLastNode.ToString()),
                GetParameter(ParmIndexName, DataType.VarChar, 255, channelInfo.IndexName),
                GetParameter(ParmGroupNameCollection, DataType.VarChar, 255, channelInfo.GroupNameCollection),
                GetParameter(ParmImageUrl, DataType.VarChar, 200, channelInfo.ImageUrl),
                GetParameter(ParmContent, DataType.Text, channelInfo.Content),
                GetParameter(ParmFilePath, DataType.VarChar, 200, channelInfo.FilePath),
                GetParameter(ParmChannelFilePathRule, DataType.VarChar, 200, channelInfo.ChannelFilePathRule),
                GetParameter(ParmContentFilePathRule, DataType.VarChar, 200, channelInfo.ContentFilePathRule),
                GetParameter(ParmLinkUrl, DataType.VarChar, 200, channelInfo.LinkUrl),
                GetParameter(ParmLinkType, DataType.VarChar, 200, channelInfo.LinkType),
                GetParameter(ParmChannelTemplateId, DataType.Integer, channelInfo.ChannelTemplateId),
                GetParameter(ParmContentTemplateId, DataType.Integer, channelInfo.ContentTemplateId),
                GetParameter(ParmKeywords, DataType.VarChar, 255, channelInfo.Keywords),
                GetParameter(ParmDescription, DataType.VarChar, 255, channelInfo.Description),
                GetParameter(ParmExtendValues, DataType.Text, channelInfo.Attributes.ToString()),
                GetParameter(ParmId, DataType.Integer, channelInfo.Id)
            };

            ExecuteNonQuery(SqlUpdate, updateParms);

            ChannelManager.UpdateCache(channelInfo.SiteId, channelInfo);

            //ChannelManager.RemoveCache(channelInfo.ParentId == 0
            //    ? channelInfo.Id
            //    : channelInfo.SiteId);
        }

        public void UpdateChannelTemplateId(ChannelInfo channelInfo)
        {
            string sqlString =
                $"UPDATE siteserver_Channel SET ChannelTemplateId = {channelInfo.ChannelTemplateId} WHERE Id = {channelInfo.Id}";
            ExecuteNonQuery(sqlString);

            ChannelManager.UpdateCache(channelInfo.SiteId, channelInfo);
        }

        public void UpdateContentTemplateId(ChannelInfo channelInfo)
        {
            string sqlString =
                $"UPDATE siteserver_Channel SET ContentTemplateId = {channelInfo.ContentTemplateId} WHERE Id = {channelInfo.Id}";
            ExecuteNonQuery(sqlString);

            ChannelManager.UpdateCache(channelInfo.SiteId, channelInfo);
        }

        public void UpdateAdditional(ChannelInfo channelInfo)
        {
            var updateParms = new IDataParameter[]
            {
                GetParameter(ParmExtendValues, DataType.Text, channelInfo.Additional.ToString()),
                GetParameter(ParmId, DataType.Integer, channelInfo.Id)
            };

            ExecuteNonQuery(SqlUpdateExtendValues, updateParms);

            ChannelManager.UpdateCache(channelInfo.SiteId, channelInfo);

            //ChannelManager.RemoveCache(channelInfo.ParentId == 0
            //    ? channelInfo.Id
            //    : channelInfo.SiteId);
        }

        /// <summary>
        /// 修改排序
        /// </summary>
        public void UpdateTaxis(int siteId, int selectedId, bool isSubtract)
        {
            if (isSubtract)
            {
                TaxisSubtract(siteId, selectedId);
            }
            else
            {
                TaxisAdd(siteId, selectedId);
            }
            ChannelManager.RemoveCacheBySiteId(siteId);
        }

        public void AddGroupNameList(int siteId, int channelId, List<string> groupList)
        {
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            if (channelInfo == null) return;

            var list = TranslateUtils.StringCollectionToStringList(channelInfo.GroupNameCollection);
            foreach (var groupName in groupList)
            {
                if (!list.Contains(groupName)) list.Add(groupName);
            }

            channelInfo.GroupNameCollection = TranslateUtils.ObjectCollectionToString(list);

            var parms = new IDataParameter[]
            {
                GetParameter(ParmGroupNameCollection, DataType.VarChar, 255, channelInfo.GroupNameCollection),
                GetParameter(ParmId, DataType.Integer, channelId)
            };

            ExecuteNonQuery(SqlUpdateGroupNameCollection, parms);

            ChannelManager.UpdateCache(siteId, channelInfo);
        }

        public void Delete(int siteId, int channelId)
        {
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            if (channelInfo == null) return;

            var idList = new List<int>();
            if (channelInfo.ChildrenCount > 0)
            {
                idList = ChannelManager.GetChannelIdList(channelInfo, EScopeType.Descendant, string.Empty, string.Empty, string.Empty);
            }
            idList.Add(channelId);

            var deleteCmd =
                $"DELETE FROM siteserver_Channel WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

            int deletedNum;

            var siteInfo = SiteManager.GetSiteInfo(siteId);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        DataProvider.ContentDao.DeleteContentsByDeletedChannelIdList(trans, siteInfo, idList);

                        deletedNum = ExecuteNonQuery(trans, deleteCmd);

                        if (channelInfo.ParentId != 0)
                        {
                            string taxisCmd =
                                $"UPDATE siteserver_Channel SET Taxis = Taxis - {deletedNum} WHERE (Taxis > {channelInfo.Taxis}) AND (SiteId = {channelInfo.SiteId})";
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
            UpdateIsLastNode(channelInfo.ParentId);
            UpdateSubtractChildrenCount(channelInfo.ParentsPath, deletedNum);

            if (channelInfo.ParentId == 0)
            {
                DataProvider.SiteDao.Delete(channelInfo.Id);
            }
            else
            {
                ChannelManager.RemoveCacheBySiteId(channelInfo.SiteId);
            }
        }

        /// <summary>
        /// 得到最后一个添加的子节点
        /// </summary>
        public ChannelInfo GetChannelInfoByLastAddDate(int channelId)
        {
            ChannelInfo channelInfo = null;

            var parms = new IDataParameter[]
            {
                GetParameter(ParmParentId, DataType.Integer, channelId)
            };

            using (var rdr = ExecuteReader(SqlSelectByLastAddDate, parms))
            {
                if (rdr.Read())
                {
                    channelInfo = GetChannelInfo(rdr);
                }
                rdr.Close();
            }
            return channelInfo;
        }

        /// <summary>
        /// 得到第一个子节点
        /// </summary>
        public ChannelInfo GetChannelInfoByTaxis(int channelId)
        {
            ChannelInfo channelInfo = null;

            var parms = new IDataParameter[]
            {
                GetParameter(ParmParentId, DataType.Integer, channelId)
            };

            using (var rdr = ExecuteReader(SqlSelectByTaxis, parms))
            {
                if (rdr.Read())
                {
                    channelInfo = GetChannelInfo(rdr);
                }
                rdr.Close();
            }
            return channelInfo;
        }

        public int GetIdByParentIdAndTaxis(int parentId, int taxis, bool isNextChannel)
        {
            var channelId = 0;

            //var sqlString = isNextChannel ? $"SELECT TOP 1 Id FROM siteserver_Channel WHERE (ParentId = {parentId} AND Taxis > {taxis}) ORDER BY Taxis" : $"SELECT TOP 1 Id FROM siteserver_Channel WHERE (ParentId = {parentId} AND Taxis < {taxis}) ORDER BY Taxis DESC";
            var sqlString = isNextChannel
                ? SqlUtils.ToTopSqlString(TableName, "Id",
                    $"WHERE (ParentId = {parentId} AND Taxis > {taxis})", "ORDER BY Taxis", 1)
                : SqlUtils.ToTopSqlString(TableName, "Id",
                    $"WHERE (ParentId = {parentId} AND Taxis < {taxis})", "ORDER BY Taxis DESC", 1);

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    channelId = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return channelId;
        }

        public int GetId(int siteId, string orderString)
        {
            if (orderString == "1")
                return siteId;

            var channelId = siteId;

            var orderArr = orderString.Split('_');
            for (var index = 1; index < orderArr.Length; index++)
            {
                var order = int.Parse(orderArr[index]);
                channelId = GetIdByParentIdAndOrder(channelId, order);
            }
            return channelId;
        }

        public int GetSiteId(int channelId)
        {
            var siteId = 0;

            var parms = new IDataParameter[]
            {
                GetParameter(ParmId, DataType.Integer, channelId)
            };

            using (var rdr = ExecuteReader(SqlSelectSiteIdById, parms))
            {
                if (rdr.Read())
                {
                    siteId = GetInt(rdr, 0);
                    if (siteId == 0)
                    {
                        siteId = channelId;
                    }
                }
                rdr.Close();
            }
            return siteId;
        }

        /// <summary>
        /// 在节点树中得到此节点的排序号，以“1_2_5_2”的形式表示
        /// </summary>
        public string GetOrderStringInSite(int channelId)
        {
            var retval = "";
            if (channelId != 0)
            {
                var parentId = GetParentId(channelId);
                if (parentId != 0)
                {
                    var orderString = GetOrderStringInSite(parentId);
                    retval = orderString + "_" + GetOrderInSibling(channelId, parentId);
                }
                else
                {
                    retval = "1";
                }
            }
            return retval;
        }

        private int GetOrderInSibling(int channelId, int parentId)
        {
            string cmd = $"SELECT Id FROM siteserver_Channel WHERE (ParentId = {parentId}) ORDER BY Taxis";
            var idList = new List<int>();
            using (var rdr = ExecuteReader(cmd))
            {
                while (rdr.Read())
                {
                    idList.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }
            return idList.IndexOf(channelId) + 1;
        }

        public List<string> GetIndexNameList(int siteId)
        {
            var list = new List<string>();

            var parms = new IDataParameter[]
            {
                GetParameter(ParmSiteId, DataType.Integer, siteId)
            };

            using (var rdr = ExecuteReader(SqlSelectIndexNameCollection, parms))
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
                        //    $" (siteserver_Channel.GroupNameCollection = '{trimGroup}' OR CHARINDEX('{trimGroup},',siteserver_Channel.GroupNameCollection) > 0 OR CHARINDEX(',{trimGroup},', siteserver_Channel.GroupNameCollection) > 0 OR CHARINDEX(',{trimGroup}', siteserver_Channel.GroupNameCollection) > 0) OR ");

                        whereStringBuilder.Append(
                                $" (siteserver_Channel.GroupNameCollection = '{trimGroup}' OR {SqlUtils.GetInStr("siteserver_Channel.GroupNameCollection", trimGroup + ",")} OR {SqlUtils.GetInStr("siteserver_Channel.GroupNameCollection", "," + trimGroup + ",")} OR {SqlUtils.GetInStr("siteserver_Channel.GroupNameCollection", "," + trimGroup)}) OR ");
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
                        //    $" (siteserver_Channel.GroupNameCollection <> '{trimGroupNot}' AND CHARINDEX('{trimGroupNot},',siteserver_Channel.GroupNameCollection) = 0 AND CHARINDEX(',{trimGroupNot},',siteserver_Channel.GroupNameCollection) = 0 AND CHARINDEX(',{trimGroupNot}',siteserver_Channel.GroupNameCollection) = 0) AND ");

                        whereStringBuilder.Append(
                                $" (siteserver_Channel.GroupNameCollection <> '{trimGroupNot}' AND {SqlUtils.GetNotInStr("siteserver_Channel.GroupNameCollection", trimGroupNot + ",")} AND {SqlUtils.GetNotInStr("siteserver_Channel.GroupNameCollection", "," + trimGroupNot + ",")} AND {SqlUtils.GetNotInStr("siteserver_Channel.GroupNameCollection", "," + trimGroupNot)}) AND ");
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

        public string GetWhereString(string group, string groupNot, bool isImageExists, bool isImage, string where)
        {
            var whereStringBuilder = new StringBuilder();
            if (isImageExists)
            {
                whereStringBuilder.Append(isImage
                    ? " AND siteserver_Channel.ImageUrl <> '' "
                    : " AND siteserver_Channel.ImageUrl = '' ");
            }

            whereStringBuilder.Append(GetGroupWhereString(group, groupNot));

            if (!string.IsNullOrEmpty(where))
            {
                whereStringBuilder.Append($" AND {where} ");
            }

            return whereStringBuilder.ToString();
        }

        public int GetCount(int channelId)
        {
            var count = 0;

            var parms = new IDataParameter[]
            {
                GetParameter(ParmParentId, DataType.Integer, channelId)
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

        public int GetSequence(int siteId, int channelId)
        {
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM siteserver_Channel WHERE SiteId = {siteId} AND ParentId = {channelInfo.ParentId} AND Taxis > (SELECT Taxis FROM siteserver_Channel WHERE Id = {channelId})";

            return DataProvider.DatabaseDao.GetIntResult(sqlString) + 1;
        }

        public List<int> GetIdListByTotalNum(List<int> channelIdList, int totalNum, string orderByString, string whereString)
        {
            if (channelIdList == null || channelIdList.Count == 0)
            {
                return channelIdList;
            }

            string sqlString;
            if (totalNum > 0)
            {
                //                sqlString = $@"SELECT TOP {totalNum} Id
                //FROM siteserver_Channel 
                //WHERE (Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) {whereString}) {orderByString}
                //";
                //var where =
                //    $"WHERE (Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) {whereString})";
                var where =
                    $"WHERE {SqlUtils.GetSqlColumnInList("Id", channelIdList)} {whereString})";
                sqlString = SqlUtils.ToTopSqlString(TableName, "Id",
                    where,
                    orderByString,
                    totalNum);
            }
            else
            {
                //                sqlString = $@"SELECT Id
                //FROM siteserver_Channel 
                //WHERE (Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) {whereString}) {orderByString}
                //";
                sqlString = $@"SELECT Id
FROM siteserver_Channel 
WHERE {SqlUtils.GetSqlColumnInList("Id", channelIdList)} {whereString} {orderByString}
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

        public Dictionary<int, ChannelInfo> GetChannelInfoDictionaryBySiteId(int siteId)
        {
            var dic = new Dictionary<int, ChannelInfo>();
            string sqlString =
                $@"SELECT Id, ChannelName, SiteId, ContentModelPluginId, ContentRelatedPluginIds, ParentId, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, IndexName, GroupNameCollection, Taxis, AddDate, ImageUrl, Content, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateId, ContentTemplateId, Keywords, Description, ExtendValues
FROM siteserver_Channel 
WHERE (SiteId = {siteId} AND (Id = {siteId} OR ParentId > 0))
ORDER BY Taxis";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var channelInfo = GetChannelInfo(rdr);
                    dic.Add(channelInfo.Id, channelInfo);
                }
                rdr.Close();
            }

            return dic;
        }

        public DataSet GetStlDataSourceBySiteId(int siteId, int startNum, int totalNum, string whereString, string orderByString)
        {
            var sqlWhereString = $"WHERE (SiteId = {siteId} {whereString})";

            //var sqlSelect = DataProvider.DatabaseDao.GetSelectSqlString(TableName, startNum, totalNum, SqlColumns, sqlWhereString, orderByString);
            var sqlSelect = DataProvider.DatabaseDao.GetPageSqlString(TableName, SqlColumns, sqlWhereString, orderByString, startNum - 1, totalNum);

            return ExecuteDataset(sqlSelect);
        }

        public DataSet GetStlDataSet(List<int> channelIdList, int startNum, int totalNum, string whereString, string orderByString)
        {
            if (channelIdList == null || channelIdList.Count == 0)
            {
                return null;
            }

            //var sqlWhereString =
            //    $"WHERE (Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) {whereString})";

            var sqlWhereString =
                $"WHERE {SqlUtils.GetSqlColumnInList("Id", channelIdList)} {whereString}";

            //var sqlSelect = DataProvider.DatabaseDao.GetSelectSqlString(TableName, startNum, totalNum, SqlColumns, sqlWhereString, orderByString);
            var sqlSelect = DataProvider.DatabaseDao.GetPageSqlString(TableName, SqlColumns, sqlWhereString, orderByString, startNum - 1, totalNum);

            return ExecuteDataset(sqlSelect);
        }

        public DataSet GetStlDataSetBySiteId(int siteId, int startNum, int totalNum, string whereString, string orderByString)
        {
            var sqlWhereString = $"WHERE (SiteId = {siteId} {whereString})";

            //var sqlSelect = DataProvider.DatabaseDao.GetSelectSqlString(TableName, startNum, totalNum, SqlColumns, sqlWhereString, orderByString);
            var sqlSelect = DataProvider.DatabaseDao.GetPageSqlString(TableName, SqlColumns, sqlWhereString, orderByString, startNum - 1, totalNum);

            return ExecuteDataset(sqlSelect);
        }

        public List<string> GetContentModelPluginIdList()
        {
            var list = new List<string>();

            const string sqlString = "SELECT DISTINCT ContentModelPluginId FROM siteserver_Channel";

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

        public List<string> GetAllFilePathBySiteId(int siteId)
        {
            var list = new List<string>();

            var sqlString =
                $"SELECT FilePath FROM siteserver_Channel WHERE FilePath <> '' AND SiteId = {siteId}";

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

        public int GetTemplateUseCount(int siteId, int templateId, TemplateType templateType, bool isDefault)
        {
            var sqlString = string.Empty;

            if (templateType == TemplateType.IndexPageTemplate)
            {
                if (isDefault)
                {
                    return 1;
                }
                return 0;
            }
            if (templateType == TemplateType.FileTemplate)
            {
                return 1;
            }
            if (templateType == TemplateType.ChannelTemplate)
            {
                sqlString = isDefault
                    ? $"SELECT COUNT(*) FROM {TableName} WHERE ({nameof(ChannelInfo.ChannelTemplateId)} = {templateId} OR {nameof(ChannelInfo.ChannelTemplateId)} = 0) AND {nameof(ChannelInfo.SiteId)} = {siteId}"
                    : $"SELECT COUNT(*) FROM {TableName} WHERE {nameof(ChannelInfo.ChannelTemplateId)} = {templateId} AND {nameof(ChannelInfo.SiteId)} = {siteId}";
            }
            else if (templateType == TemplateType.ContentTemplate)
            {
                sqlString = isDefault
                    ? $"SELECT COUNT(*) FROM {TableName} WHERE ({nameof(ChannelInfo.ContentTemplateId)} = {templateId} OR {nameof(ChannelInfo.ContentTemplateId)} = 0) AND {nameof(ChannelInfo.SiteId)} = {siteId}"
                    : $"SELECT COUNT(*) FROM {TableName} WHERE {nameof(ChannelInfo.ContentTemplateId)} = {templateId} AND {nameof(ChannelInfo.SiteId)} = {siteId}";
            }

            return DataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<int> GetChannelIdList(TemplateInfo templateInfo)
        {
            var list = new List<int>();
            var sqlString = string.Empty;

            if (templateInfo.TemplateType == TemplateType.ChannelTemplate)
            {
                sqlString = templateInfo.IsDefault
                    ? $"SELECT {nameof(ChannelInfo.Id)} FROM {TableName} WHERE ({nameof(ChannelInfo.ChannelTemplateId)} = {templateInfo.Id} OR {nameof(ChannelInfo.ChannelTemplateId)} = 0) AND {nameof(ChannelInfo.SiteId)} = {templateInfo.SiteId}"
                    : $"SELECT {nameof(ChannelInfo.Id)} FROM {TableName} WHERE {nameof(ChannelInfo.ChannelTemplateId)} = {templateInfo.Id} AND {nameof(ChannelInfo.SiteId)} = {templateInfo.SiteId}";
            }
            else if (templateInfo.TemplateType == TemplateType.ContentTemplate)
            {
                sqlString = templateInfo.IsDefault
                    ? $"SELECT {nameof(ChannelInfo.Id)} FROM {TableName} WHERE ({nameof(ChannelInfo.ContentTemplateId)} = {templateInfo.Id} OR {nameof(ChannelInfo.ContentTemplateId)} = 0) AND {nameof(ChannelInfo.SiteId)} = {templateInfo.SiteId}"
                    : $"SELECT {nameof(ChannelInfo.Id)} FROM {TableName} WHERE {nameof(ChannelInfo.ContentTemplateId)} = {templateInfo.Id} AND {nameof(ChannelInfo.SiteId)} = {templateInfo.SiteId}";
            }

            if (!string.IsNullOrEmpty(sqlString))
            {
                list = DataProvider.DatabaseDao.GetIntList(sqlString);
            }

            return list;
        }

        private ChannelInfo GetChannelInfo(IDataReader rdr)
        {
            var i = 0;
            return new ChannelInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++),
                GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++),
                GetBool(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetDateTime(rdr, i++),
                GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++),
                GetString(rdr, i++), GetString(rdr, i++), ELinkTypeUtils.GetEnumType(GetString(rdr, i++)),
                GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
        }
    }
}
