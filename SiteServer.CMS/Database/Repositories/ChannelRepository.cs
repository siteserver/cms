using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SiteServer.CMS.Apis;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Attributes;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Database.Repositories
{
    public class ChannelRepository : GenericRepository<ChannelInfo>
    {
        private static readonly List<string> SqlColumns = new List<string>
        {
            ChannelAttribute.Id,
            ChannelAttribute.AddDate,
            ChannelAttribute.Taxis
        };

        //public override string TableName => "siteserver_Channel";

        //public override List<TableColumn> TableColumns => new List<TableColumn>
        //{
        //    new TableColumn
        //    {
        //        AttributeName = nameof(ChannelInfo.Id),
        //        DataType = DataType.Integer,
        //        IsIdentity = true,
        //        IsPrimaryKey = true
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(ChannelInfo.ChannelName),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(ChannelInfo.SiteId),
        //        DataType = DataType.Integer
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(ChannelInfo.ContentModelPluginId),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(ChannelInfo.ContentRelatedPluginIds),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(ChannelInfo.ParentId),
        //        DataType = DataType.Integer
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(ChannelInfo.ParentsPath),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(ChannelInfo.ParentsCount),
        //        DataType = DataType.Integer
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(ChannelInfo.ChildrenCount),
        //        DataType = DataType.Integer
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(ChannelInfo.IsLastNode),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(ChannelInfo.IndexName),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(ChannelInfo.GroupNameCollection),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(ChannelInfo.Taxis),
        //        DataType = DataType.Integer
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(ChannelInfo.AddDate),
        //        DataType = DataType.DateTime
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(ChannelInfo.ImageUrl),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(ChannelInfo.Content),
        //        DataType = DataType.Text
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(ChannelInfo.FilePath),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(ChannelInfo.ChannelFilePathRule),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(ChannelInfo.ContentFilePathRule),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(ChannelInfo.LinkUrl),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(ChannelInfo.LinkType),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(ChannelInfo.ChannelTemplateId),
        //        DataType = DataType.Integer
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(ChannelInfo.ContentTemplateId),
        //        DataType = DataType.Integer
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(ChannelInfo.Keywords),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(ChannelInfo.Description),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = "ExtendValues",
        //        DataType = DataType.Text
        //    }
        //};

        //private const string SqlSelectByLastAddDate = "SELECT Id, ChannelName, SiteId, ContentModelPluginId, ContentRelatedPluginIds, ParentId, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, IndexName, GroupNameCollection, Taxis, AddDate, ImageUrl, Content, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateId, ContentTemplateId, Keywords, Description, ExtendValues FROM siteserver_Channel WHERE ParentId = @ParentId ORDER BY AddDate Desc";

        //private const string SqlSelectByTaxis = "SELECT Id, ChannelName, SiteId, ContentModelPluginId, ContentRelatedPluginIds, ParentId, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, IndexName, GroupNameCollection, Taxis, AddDate, ImageUrl, Content, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateId, ContentTemplateId, Keywords, Description, ExtendValues FROM siteserver_Channel WHERE ParentId = @ParentId ORDER BY Taxis";

        //private const string SqlSelectParentId = "SELECT ParentId FROM siteserver_Channel WHERE Id = @Id";

        //private const string SqlSelectCount = "SELECT COUNT(*) FROM siteserver_Channel WHERE ParentId = @ParentId";

        //private const string SqlSelectSiteIdById = "SELECT SiteId FROM siteserver_Channel WHERE Id = @Id";

        //private const string SqlSelectIndexNameCollection = "SELECT DISTINCT IndexName FROM siteserver_Channel WHERE SiteId = @SiteId";

        //private const string SqlUpdate = "UPDATE siteserver_Channel SET ChannelName = @ChannelName, ContentModelPluginId = @ContentModelPluginId, ContentRelatedPluginIds = @ContentRelatedPluginIds, ParentsPath = @ParentsPath, ParentsCount = @ParentsCount, ChildrenCount = @ChildrenCount, IsLastNode = @IsLastNode, IndexName = @IndexName, GroupNameCollection = @GroupNameCollection, ImageUrl = @ImageUrl, Content = @Content, FilePath = @FilePath, ChannelFilePathRule = @ChannelFilePathRule, ContentFilePathRule = @ContentFilePathRule, LinkUrl = @LinkUrl,LinkType = @LinkType, ChannelTemplateId = @ChannelTemplateId, ContentTemplateId = @ContentTemplateId, Keywords = @Keywords, Description = @Description, ExtendValues = @ExtendValues WHERE Id = @Id";

        //private const string SqlUpdateExtendValues = "UPDATE siteserver_Channel SET ExtendValues = @ExtendValues WHERE Id = @Id";

        //private const string SqlUpdateGroupNameCollection = "UPDATE siteserver_Channel SET GroupNameCollection = @GroupNameCollection WHERE Id = @Id";

        //private const string ParamId = "@Id";
        //private const string ParamChannelName = "@ChannelName";
        //private const string ParamSiteId = "@SiteId";
        //private const string ParamContentModelPluginId = "@ContentModelPluginId";
        //private const string ParamContentRelatedPluginIds = "@ContentRelatedPluginIds";
        //private const string ParamParentId = "@ParentId";
        //private const string ParamParentsPath = "@ParentsPath";
        //private const string ParamParentsCount = "@ParentsCount";
        //private const string ParamChildrenCount = "@ChildrenCount";
        //private const string ParamIsLastNode = "@IsLastNode";
        //private const string ParamIndexName = "@IndexName";
        //private const string ParamGroupNameCollection = "@GroupNameCollection";
        //private const string ParamTaxis = "@Taxis";
        //private const string ParamAddDate = "@AddDate";
        //private const string ParamImageUrl = "@ImageUrl";
        //private const string ParamContent = "@Content";
        //private const string ParamFilePath = "@FilePath";
        //private const string ParamChannelFilePathRule = "@ChannelFilePathRule";
        //private const string ParamContentFilePathRule = "@ContentFilePathRule";
        //private const string ParamLinkUrl = "@LinkUrl";
        //private const string ParamLinkType = "@LinkType";
        //private const string ParamChannelTemplateId = "@ChannelTemplateId";
        //private const string ParamContentTemplateId = "@ContentTemplateId";
        //private const string ParamKeywords = "@Keywords";
        //private const string ParamDescription = "@Description";
        //private const string ParamExtendValues = "@ExtendValues";

        private static class Attr
        {
            public const string Id = nameof(ChannelInfo.Id);
            public const string SiteId = nameof(ChannelInfo.SiteId);
            public const string ContentModelPluginId = nameof(ChannelInfo.ContentModelPluginId);
            public const string ParentId = nameof(ChannelInfo.ParentId);
            public const string ParentsPath = nameof(ChannelInfo.ParentsPath);
            public const string ChildrenCount = nameof(ChannelInfo.ChildrenCount);
            public const string IsLastNode = "IsLastNode";
            public const string IndexName = nameof(ChannelInfo.IndexName);
            public const string GroupNameCollection = nameof(ChannelInfo.GroupNameCollection);
            public const string Taxis = nameof(ChannelInfo.Taxis);
            public const string AddDate = nameof(ChannelInfo.AddDate);
            public const string FilePath = nameof(ChannelInfo.FilePath);
            public const string ChannelTemplateId = nameof(ChannelInfo.ChannelTemplateId);
            public const string ContentTemplateId = nameof(ChannelInfo.ContentTemplateId);
            public const string ExtendValues = "ExtendValues";
        }

        private void InsertChannelInfo(ChannelInfo parentChannelInfo, ChannelInfo channelInfo)
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

            channelInfo.ChildrenCount = 0;
            channelInfo.LastNode = true;

            //const string sqlInsertNode = "INSERT INTO siteserver_Channel (ChannelName, SiteId, ContentModelPluginId, ContentRelatedPluginIds, ParentId, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, IndexName, GroupNameCollection, Taxis, AddDate, ImageUrl, Content, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateId, ContentTemplateId, Keywords, Description, ExtendValues) VALUES (@ChannelName, @SiteId, @ContentModelPluginId, @ContentRelatedPluginIds, @ParentId, @ParentsPath, @ParentsCount, @ChildrenCount, @IsLastNode, @IndexName, @GroupNameCollection, @Taxis, @AddDate, @ImageUrl, @Content, @FilePath, @ChannelFilePathRule, @ContentFilePathRule, @LinkUrl, @LinkType, @ChannelTemplateId, @ContentTemplateId, @Keywords, @Description, @ExtendValues)";

            //IDataParameter[] insertParams =
            //{
            //    GetParameter(ParamChannelName, channelInfo.ChannelName),
            //    GetParameter(ParamSiteId, channelInfo.SiteId),
            //    GetParameter(ParamContentModelPluginId, channelInfo.ContentModelPluginId),
            //    GetParameter(ParamContentRelatedPluginIds, channelInfo.ContentRelatedPluginIds),
            //    GetParameter(ParamParentId, channelInfo.ParentId),
            //    GetParameter(ParamParentsPath, channelInfo.ParentsPath),
            //    GetParameter(ParamParentsCount, channelInfo.ParentsCount),
            //    GetParameter(ParamChildrenCount, 0),
            //    GetParameter(ParamIsLastNode, true.ToString()),
            //    GetParameter(ParamIndexName, channelInfo.IndexName),
            //    GetParameter(ParamGroupNameCollection, channelInfo.GroupNameCollection),
            //    GetParameter(ParamTaxis, channelInfo.Taxis),
            //    GetParameter(ParamAddDate, channelInfo.AddDate),
            //    GetParameter(ParamImageUrl, channelInfo.ImageUrl),
            //    GetParameter(ParamContent, channelInfo.Content),
            //    GetParameter(ParamFilePath, channelInfo.FilePath),
            //    GetParameter(ParamChannelFilePathRule, channelInfo.ChannelFilePathRule),
            //    GetParameter(ParamContentFilePathRule, channelInfo.ContentFilePathRule),
            //    GetParameter(ParamLinkUrl, channelInfo.LinkUrl),
            //    GetParameter(ParamLinkType, channelInfo.LinkType),
            //    GetParameter(ParamChannelTemplateId, channelInfo.ChannelTemplateId),
            //    GetParameter(ParamContentTemplateId, channelInfo.ContentTemplateId),
            //    GetParameter(ParamKeywords, channelInfo.Keywords),
            //    GetParameter(ParamDescription, channelInfo.Description),
            //    GetParameter(ParamExtendValues, channelInfo.Attributes.ToString())
            //};

            if (channelInfo.SiteId != 0)
            {
                //var sqlString =
                //    $"UPDATE siteserver_Channel SET Taxis = {SqlDifferences.ColumnIncrement("Taxis")} WHERE (Taxis >= {channelInfo.Taxis}) AND (SiteId = {channelInfo.SiteId})";
                //DatabaseApi.ExecuteNonQuery(trans, sqlString);

                IncrementAll(Attr.Taxis, Q
                    .Where(Attr.Taxis, ">=", channelInfo.Taxis)
                    .Where(Attr.SiteId, channelInfo.SiteId));
            }
            //channelInfo.Id = DatabaseApi.ExecuteNonQueryAndReturnId(TableName, nameof(ChannelInfo.Id), trans, sqlInsertNode, insertParams);
            InsertObject(channelInfo);

            if (!string.IsNullOrEmpty(channelInfo.ParentsPath))
            {
                //var sqlString = $"UPDATE siteserver_Channel SET ChildrenCount = {SqlDifferences.ColumnIncrement("ChildrenCount")} WHERE Id IN ({channelInfo.ParentsPath})";

                //DatabaseApi.ExecuteNonQuery(trans, sqlString);

                IncrementAll(Attr.ChildrenCount, Q
                    .WhereIn(Attr.ParentsPath,
                        TranslateUtils.StringCollectionToIntList(channelInfo.ParentsPath)));
            }

            //var sqlUpdateIsLastNode = "UPDATE siteserver_Channel SET IsLastNode = @IsLastNode WHERE ParentId = @ParentId";
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamIsLastNode, false.ToString()),
            //    GetParameter(ParamParentId, channelInfo.ParentId)
            //};
            //DatabaseApi.ExecuteNonQuery(trans, sqlUpdateIsLastNode, parameters);

            UpdateAll(Q
                .Set(Attr.IsLastNode, false.ToString())
                .Where(Attr.ParentId, channelInfo.ParentId)
            );

            //sqlUpdateIsLastNode =
            //    $"UPDATE siteserver_Channel SET IsLastNode = '{true}' WHERE (Id IN ({SqlUtils.ToInTopSqlString(TableName, new List<string>{ nameof(ChannelInfo.Id) }, $"WHERE ParentId = {channelInfo.ParentId}", "ORDER BY Taxis DESC", 1)}))";
            //DatabaseApi.ExecuteNonQuery(trans, sqlUpdateIsLastNode);
            var topId = GetValue<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ParentId, channelInfo.ParentId)
                .OrderByDesc(Attr.Taxis));

            if (topId > 0)
            {
                UpdateAll(Q
                    .Set(Attr.IsLastNode, true.ToString())
                    .Where(nameof(Attr.Id), topId)
                );
            }

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
            if (string.IsNullOrEmpty(parentsPath)) return;

            //var sqlString = string.Concat("UPDATE siteserver_Channel SET ChildrenCount = ChildrenCount - ", subtractNum, " WHERE Id in (", parentsPath, ")");
            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
            DecrementAll(Attr.ChildrenCount, Q
                    .WhereIn(Attr.Id, TranslateUtils.StringCollectionToIntList(parentsPath)),
                subtractNum);
        }

        /// <summary>
        /// Change The Taxis To Lower Level
        /// </summary>
        private void TaxisSubtract(int siteId, int selectedId)
        {
            var channelInfo = ChannelManager.GetChannelInfo(siteId, selectedId);
            if (channelInfo == null || channelInfo.ParentId == 0 || channelInfo.SiteId == 0) return;
            //UpdateWholeTaxisBySiteId(channelInfo.SiteId);
            //Get Lower Taxis and Id
            //int lowerId;
            //int lowerChildrenCount;
            //string lowerParentsPath;

            //var sqlString = SqlDifferences.GetSqlString(TableName, new List<string>
            //{
            //    nameof(ChannelInfo.Id),
            //    nameof(ChannelInfo.ChildrenCount),
            //    nameof(ChannelInfo.ParentsPath)
            //}, "WHERE (ParentId = @ParentId) AND (Id <> @Id) AND (Taxis < @Taxis) AND (SiteId = @SiteId)", "ORDER BY Taxis DESC", 1);
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamParentId, channelInfo.ParentId),
            //    GetParameter(ParamId, channelInfo.Id),
            //    GetParameter(ParamTaxis, channelInfo.Taxis),
            //    GetParameter(ParamSiteId, channelInfo.SiteId)
            //};
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        lowerId = DatabaseApi.GetInt(rdr, 0);
            //        lowerChildrenCount = DatabaseApi.GetInt(rdr, 1);
            //        lowerParentsPath = DatabaseApi.GetString(rdr, 2);
            //    }
            //    else
            //    {
            //        return;
            //    }
            //    rdr.Close();
            //}
            var dataInfo = GetObject(Q
                .Where(Attr.ParentId, channelInfo.ParentId)
                .WhereNot(Attr.Id, channelInfo.Id)
                .Where(Attr.Taxis, "<", channelInfo.Taxis)
                .Where(Attr.SiteId, channelInfo.SiteId)
                .OrderByDesc(Attr.Taxis));
            if (dataInfo == null) return;

            var lowerId = dataInfo.Id;
            var lowerChildrenCount = dataInfo.ChildrenCount;
            var lowerParentsPath = dataInfo.ParentsPath;

            var lowerPath = lowerParentsPath == "" ? lowerId.ToString() : string.Concat(lowerParentsPath, ",", lowerId);
            var selectedPath = channelInfo.ParentsPath == "" ? channelInfo.Id.ToString() : string.Concat(channelInfo.ParentsPath, ",", channelInfo.Id);

            SetTaxisSubtract(selectedId, selectedPath, lowerChildrenCount + 1);
            SetTaxisAdd(lowerId, lowerPath, channelInfo.ChildrenCount + 1);

            UpdateIsLastNode(channelInfo.ParentId);
        }

        /// <summary>
        /// Change The Taxis To Higher Level
        /// </summary>
        private void TaxisAdd(int siteId, int selectedId)
        {
            var channelInfo = ChannelManager.GetChannelInfo(siteId, selectedId);
            if (channelInfo == null || channelInfo.ParentId == 0 || channelInfo.SiteId == 0) return;
            //UpdateWholeTaxisBySiteId(channelInfo.SiteId);
            //Get Higher Taxis and Id
            //int higherId;
            //int higherChildrenCount;
            //string higherParentsPath;

            //var sqlString = SqlDifferences.GetSqlString(TableName, new List<string>
            //{
            //    nameof(ChannelInfo.Id),
            //    nameof(ChannelInfo.ChildrenCount),
            //    nameof(ChannelInfo.ParentsPath)
            //}, "WHERE (ParentId = @ParentId) AND (Id <> @Id) AND (Taxis > @Taxis) AND (SiteId = @SiteId)", "ORDER BY Taxis", 1);
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamParentId, channelInfo.ParentId),
            //    GetParameter(ParamId, channelInfo.Id),
            //    GetParameter(ParamTaxis, channelInfo.Taxis),
            //    GetParameter(ParamSiteId, channelInfo.SiteId)
            //};
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        higherId = DatabaseApi.GetInt(rdr, 0);
            //        higherChildrenCount = DatabaseApi.GetInt(rdr, 1);
            //        higherParentsPath = DatabaseApi.GetString(rdr, 2);
            //    }
            //    else
            //    {
            //        return;
            //    }
            //    rdr.Close();
            //}
            var dataInfo = GetObject(Q
                .Where(Attr.ParentId, channelInfo.ParentId)
                .WhereNot(Attr.Id, channelInfo.Id)
                .Where(Attr.Taxis, ">", channelInfo.Taxis)
                .Where(Attr.SiteId, channelInfo.SiteId)
                .OrderBy(Attr.Taxis));

            if (dataInfo == null) return;

            var higherId = dataInfo.Id;
            var higherChildrenCount = dataInfo.ChildrenCount;
            var higherParentsPath = dataInfo.ParentsPath;

            var higherPath = higherParentsPath == string.Empty ? higherId.ToString() : string.Concat(higherParentsPath, ",", higherId);
            var selectedPath = channelInfo.ParentsPath == string.Empty ? channelInfo.Id.ToString() : String.Concat(channelInfo.ParentsPath, ",", channelInfo.Id);

            SetTaxisAdd(selectedId, selectedPath, higherChildrenCount + 1);
            SetTaxisSubtract(higherId, higherPath, channelInfo.ChildrenCount + 1);

            UpdateIsLastNode(channelInfo.ParentId);
        }

        private void SetTaxisAdd(int channelId, string parentsPath, int addNum)
        {
            //var sqlString =
            //    $"UPDATE siteserver_Channel SET Taxis = {SqlDifferences.ColumnIncrement("Taxis", addNum)} WHERE Id = {channelId} OR ParentsPath = '{parentsPath}' OR ParentsPath like '{parentsPath},%'";

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
            IncrementAll(Attr.Taxis, Q
                .Where(Attr.Id, channelId)
                .OrWhere(Attr.ParentsPath, parentsPath)
                .OrWhereStarts(Attr.ParentsPath, $"{parentsPath},"), addNum);
        }

        private void SetTaxisSubtract(int channelId, string parentsPath, int subtractNum)
        {
            //string sqlString =
            //    $"UPDATE siteserver_Channel SET Taxis = {SqlDifferences.ColumnDecrement("Taxis", subtractNum)} WHERE  Id = {channelId} OR ParentsPath = '{parentsPath}' OR ParentsPath like '{parentsPath},%'";

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
            DecrementAll(Attr.Taxis, Q
                .Where(Attr.Id, channelId)
                .OrWhere(Attr.ParentsPath, parentsPath)
                .OrWhereStarts(Attr.ParentsPath, $"{parentsPath},"), subtractNum);
        }

        private void UpdateIsLastNode(int parentId)
        {
            if (parentId <= 0) return;

            //var sqlString = "UPDATE siteserver_Channel SET IsLastNode = @IsLastNode WHERE  ParentId = @ParentId";
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamIsLastNode, false.ToString()),
            //    GetParameter(ParamParentId, parentId)
            //};
            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            UpdateAll(Q
                .Set(Attr.IsLastNode, false.ToString())
                .Where(Attr.ParentId, parentId)
            );

            //sqlString =
            //    $"UPDATE siteserver_Channel SET IsLastNode = '{true}' WHERE (Id IN ({SqlUtils.ToInTopSqlString(TableName, new List<string> { Attr.Id }, $"WHERE ParentId = {parentId}", "ORDER BY Taxis DESC", 1)}))";
            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
            var topId = GetValue<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ParentId, parentId)
                .OrderByDesc(Attr.Taxis));

            if (topId > 0)
            {
                UpdateAll(Q
                    .Set(Attr.IsLastNode, true.ToString())
                    .Where(Attr.Id, topId)
                );
            }
        }

        private int GetMaxTaxisByParentPath(string parentPath)
        {
            //var cmd = string.Concat("SELECT MAX(Taxis) AS MaxTaxis FROM siteserver_Channel WHERE (ParentsPath = '", parentPath, "') OR (ParentsPath like '", parentPath, ",%')");
            //var maxTaxis = 0;
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, cmd))
            //{
            //    if (rdr.Read())
            //    {
            //        maxTaxis = DatabaseApi.GetInt(rdr, 0);
            //    }
            //    rdr.Close();
            //}
            //return maxTaxis;
            return Max(Attr.Taxis, Q
                .Where(Attr.ParentsPath, parentPath)
                .OrWhereStarts(Attr.ParentsPath, $"{parentPath},"));
        }

        private int GetParentId(int channelId)
        {
            //var parentId = 0;

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamId, channelId)
            //};

            //"SELECT ParentId FROM siteserver_Channel WHERE Id = @Id"
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectParentId, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        parentId = DatabaseApi.GetInt(rdr, 0);
            //    }
            //    rdr.Close();
            //}
            //return parentId;

            return GetValue<int>(Q
                .Select(Attr.ParentId)
                .Where(Attr.Id, channelId));
        }

        private int GetIdByParentIdAndOrder(int parentId, int order)
        {
            //var channelId = parentId;

            //string cmd = $"SELECT Id FROM siteserver_Channel WHERE (ParentId = {parentId}) ORDER BY Taxis";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, cmd))
            //{
            //    var index = 1;
            //    while (rdr.Read())
            //    {
            //        channelId = DatabaseApi.GetInt(rdr, 0);
            //        if (index == order)
            //            break;
            //        index++;
            //    }
            //    rdr.Close();
            //}
            //return channelId;

            var idList = GetValueList<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ParentId, parentId)
                .OrderBy(Attr.Taxis));
            
            for (var i = 0; i < idList.Count; i++)
            {
                if (i + 1 == order)
                {
                    return idList[i];
                }
            }

            return parentId;
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

            InsertChannelInfo(parentChannelInfo, channelInfo);

            return channelInfo.Id;

        }

        public int Insert(ChannelInfo channelInfo)
        {
            if (channelInfo.SiteId > 0 && channelInfo.ParentId == 0) return 0;

            var parentChannelInfo = ChannelManager.GetChannelInfo(channelInfo.SiteId, channelInfo.ParentId);

            InsertChannelInfo(parentChannelInfo, channelInfo);

            return channelInfo.Id;
        }

        /// <summary>
        /// 添加后台发布节点
        /// </summary>
        public int InsertSiteInfo(ChannelInfo channelInfo, SiteInfo siteInfo, string administratorName)
        {
            InsertChannelInfo(null, channelInfo);

            siteInfo.Id = channelInfo.Id;

            DataProvider.Site.Insert(siteInfo);

            var adminInfo = AdminManager.GetAdminInfoByUserName(administratorName);
            DataProvider.Administrator.UpdateSiteId(adminInfo, channelInfo.Id);

            //var sqlString =
            //    $"UPDATE siteserver_Channel SET SiteId = {channelInfo.Id} WHERE Id = {channelInfo.Id}";
            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

            UpdateAll(Q
                .Set(Attr.SiteId, channelInfo.Id)
                .Where(Attr.Id, channelInfo.Id)
            );

            DataProvider.Template.CreateDefaultTemplateInfo(channelInfo.Id, administratorName);
            return channelInfo.Id;
        }

        public void Update(ChannelInfo channelInfo)
        {
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamChannelName, channelInfo.ChannelName),
            //    GetParameter(ParamContentModelPluginId, channelInfo.ContentModelPluginId),
            //    GetParameter(ParamContentRelatedPluginIds, channelInfo.ContentRelatedPluginIds),
            //    GetParameter(ParamParentsPath, channelInfo.ParentsPath),
            //    GetParameter(ParamParentsCount, channelInfo.ParentsCount),
            //    GetParameter(ParamChildrenCount, channelInfo.ChildrenCount),
            //    GetParameter(ParamIsLastNode, channelInfo.IsLastNode.ToString()),
            //    GetParameter(ParamIndexName, channelInfo.IndexName),
            //    GetParameter(ParamGroupNameCollection, channelInfo.GroupNameCollection),
            //    GetParameter(ParamImageUrl, channelInfo.ImageUrl),
            //    GetParameter(ParamContent,channelInfo.Content),
            //    GetParameter(ParamFilePath, channelInfo.FilePath),
            //    GetParameter(ParamChannelFilePathRule, channelInfo.ChannelFilePathRule),
            //    GetParameter(ParamContentFilePathRule, channelInfo.ContentFilePathRule),
            //    GetParameter(ParamLinkUrl, channelInfo.LinkUrl),
            //    GetParameter(ParamLinkType, channelInfo.LinkType),
            //    GetParameter(ParamChannelTemplateId, channelInfo.ChannelTemplateId),
            //    GetParameter(ParamContentTemplateId, channelInfo.ContentTemplateId),
            //    GetParameter(ParamKeywords, channelInfo.Keywords),
            //    GetParameter(ParamDescription, channelInfo.Description),
            //    GetParameter(ParamExtendValues,channelInfo.Attributes.ToString()),
            //    GetParameter(ParamId, channelInfo.Id)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, SqlUpdate, parameters);
            UpdateObject(channelInfo);

            ChannelManager.UpdateCache(channelInfo.SiteId, channelInfo);

            //ChannelManager.RemoveCache(channelInfo.ParentId == 0
            //    ? channelInfo.Id
            //    : channelInfo.SiteId);
        }

        public void UpdateChannelTemplateId(ChannelInfo channelInfo)
        {
            //string sqlString =
            //    $"UPDATE siteserver_Channel SET ChannelTemplateId = {channelInfo.ChannelTemplateId} WHERE Id = {channelInfo.Id}";
            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

            UpdateAll(Q
                .Set(Attr.ChannelTemplateId, channelInfo.ChannelTemplateId)
                .Where(Attr.Id, channelInfo.Id)
            );

            ChannelManager.UpdateCache(channelInfo.SiteId, channelInfo);
        }

        public void UpdateContentTemplateId(ChannelInfo channelInfo)
        {
            //string sqlString =
            //    $"UPDATE siteserver_Channel SET ContentTemplateId = {channelInfo.ContentTemplateId} WHERE Id = {channelInfo.Id}";
            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

            UpdateAll(Q
                .Set(Attr.ContentTemplateId, channelInfo.ContentTemplateId)
                .Where(Attr.Id, channelInfo.Id)
            );

            ChannelManager.UpdateCache(channelInfo.SiteId, channelInfo);
        }

        public void UpdateExtend(ChannelInfo channelInfo)
        {
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamExtendValues,channelInfo.ToString()),
            //    GetParameter(ParamId, channelInfo.Id)
            //};
            //"UPDATE siteserver_Channel SET ExtendValues = @ExtendValues WHERE Id = @Id"
            //DatabaseApi.ExecuteNonQuery(ConnectionString, SqlUpdateExtendValues, parameters);

            UpdateAll(Q
                .Set(Attr.ExtendValues, channelInfo.ExtendValues)
                .Where(Attr.Id, channelInfo.Id)
            );

            ChannelManager.UpdateCache(channelInfo.SiteId, channelInfo);
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

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamGroupNameCollection, channelInfo.GroupNameCollection),
            //    GetParameter(ParamId, channelId)
            //};
            //"UPDATE siteserver_Channel SET GroupNameCollection = @GroupNameCollection WHERE Id = @Id"
            //DatabaseApi.ExecuteNonQuery(ConnectionString, SqlUpdateGroupNameCollection, parameters);

            UpdateAll(Q
                .Set(Attr.GroupNameCollection, channelInfo.GroupNameCollection)
                .Where(Attr.Id, channelId)
            );

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

            //var siteInfo = SiteManager.GetSiteInfo(siteId);

            foreach (var i in idList)
            {
                var cInfo = ChannelManager.GetChannelInfo(siteId, i);
                cInfo.ContentRepository.UpdateTrashContentsByChannelId(siteId, i);
            }

            //DataProvider.ContentRepository.DeleteContentsByDeletedChannelIdList(siteInfo, idList);

            //var deleteCmd =
            //    $"DELETE FROM siteserver_Channel WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";
            //deletedNum = DatabaseApi.ExecuteNonQuery(trans, deleteCmd);
            var deletedNum = DeleteAll(Q
                .WhereIn(Attr.Id, idList));

            if (channelInfo.ParentId != 0)
            {
                //var taxisCmd =
                //    $"UPDATE siteserver_Channel SET Taxis = Taxis - {deletedNum} WHERE (Taxis > {channelInfo.Taxis}) AND (SiteId = {channelInfo.SiteId})";
                //DatabaseApi.ExecuteNonQuery(trans, taxisCmd);
                DecrementAll(Attr.Taxis, Q
                    .Where(Attr.SiteId, channelInfo.SiteId)
                    .Where(Attr.Taxis, ">", channelInfo.Taxis), deletedNum);
            }

            UpdateIsLastNode(channelInfo.ParentId);
            UpdateSubtractChildrenCount(channelInfo.ParentsPath, deletedNum);

            if (channelInfo.ParentId == 0)
            {
                DataProvider.Site.Delete(channelInfo.Id);
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
            //ChannelInfo channelInfo = null;

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamParentId, channelId)
            //};
            //"SELECT Id, ChannelName, SiteId, ContentModelPluginId, ContentRelatedPluginIds, ParentId, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, IndexName, GroupNameCollection, Taxis, AddDate, ImageUrl, Content, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateId, ContentTemplateId, Keywords, Description, ExtendValues FROM siteserver_Channel WHERE ParentId = @ParentId ORDER BY AddDate Desc"
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectByLastAddDate, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        channelInfo = GetChannelInfo(rdr);
            //    }
            //    rdr.Close();
            //}
            //return channelInfo;
            return GetObject(Q
                .Where(Attr.ParentId, channelId)
                .OrderByDesc(Attr.AddDate));
        }

        /// <summary>
        /// 得到第一个子节点
        /// </summary>
        public ChannelInfo GetChannelInfoByTaxis(int channelId)
        {
            //ChannelInfo channelInfo = null;

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamParentId, channelId)
            //};
            //"SELECT Id, ChannelName, SiteId, ContentModelPluginId, ContentRelatedPluginIds, ParentId, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, IndexName, GroupNameCollection, Taxis, AddDate, ImageUrl, Content, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateId, ContentTemplateId, Keywords, Description, ExtendValues FROM siteserver_Channel WHERE ParentId = @ParentId ORDER BY Taxis"
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectByTaxis, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        channelInfo = GetChannelInfo(rdr);
            //    }
            //    rdr.Close();
            //}
            //return channelInfo;

            return GetObject(Q
                .Where(Attr.ParentId, channelId)
                .OrderBy(Attr.Taxis));
        }

        public int GetIdByParentIdAndTaxis(int parentId, int taxis, bool isNextChannel)
        {
            //var channelId = 0;

            //var sqlString = isNextChannel ? $"SELECT TOP 1 Id FROM siteserver_Channel WHERE (ParentId = {parentId} AND Taxis > {taxis}) ORDER BY Taxis" : $"SELECT TOP 1 Id FROM siteserver_Channel WHERE (ParentId = {parentId} AND Taxis < {taxis}) ORDER BY Taxis DESC";
            //var sqlString = isNextChannel
            //    ? SqlDifferences.GetSqlString(TableName, new List<string>
            //        {
            //            Attr.Id
            //        },
            //        $"WHERE (ParentId = {parentId} AND Taxis > {taxis})", "ORDER BY Taxis", 1)
            //    : SqlDifferences.GetSqlString(TableName, new List<string>
            //        {
            //            Attr.Id
            //        },
            //        $"WHERE (ParentId = {parentId} AND Taxis < {taxis})", "ORDER BY Taxis DESC", 1);

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    if (rdr.Read())
            //    {
            //        channelId = DatabaseApi.GetInt(rdr, 0);
            //    }
            //    rdr.Close();
            //}
            //return channelId;

            int channelId;
            if (isNextChannel)
            {
                channelId = GetValue<int>(Q
                    .Select(Attr.Id)
                    .Where(Attr.ParentId, parentId)
                    .Where(Attr.Taxis, ">", taxis)
                    .OrderBy(Attr.Taxis));
            }
            else
            {
                channelId = GetValue<int>(Q
                    .Select(Attr.Id)
                    .Where(Attr.ParentId, parentId)
                    .Where(Attr.Taxis, "<", taxis)
                    .OrderByDesc(Attr.Taxis));
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
            //var siteId = 0;

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamId, channelId)
            //};
            //"SELECT SiteId FROM siteserver_Channel WHERE Id = @Id"
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectSiteIdById, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        siteId = DatabaseApi.GetInt(rdr, 0);
            //        if (siteId == 0)
            //        {
            //            siteId = channelId;
            //        }
            //    }
            //    rdr.Close();
            //}
            //return siteId;

            var siteId = GetValue<int>(Q
                .Select(Attr.SiteId)
                .Where(Attr.Id, channelId));

            if (siteId == 0)
            {
                siteId = channelId;
            }

            return siteId;
        }

        /// <summary>
        /// 在节点树中得到此节点的排序号，以“1_2_5_2”的形式表示
        /// </summary>
        public string GetOrderStringInSite(int channelId)
        {
            var retVal = "";
            if (channelId != 0)
            {
                var parentId = GetParentId(channelId);
                if (parentId != 0)
                {
                    var orderString = GetOrderStringInSite(parentId);
                    retVal = orderString + "_" + GetOrderInSibling(channelId, parentId);
                }
                else
                {
                    retVal = "1";
                }
            }
            return retVal;
        }

        private int GetOrderInSibling(int channelId, int parentId)
        {
            //string cmd = $"SELECT Id FROM siteserver_Channel WHERE (ParentId = {parentId}) ORDER BY Taxis";
            //var idList = new List<int>();
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, cmd))
            //{
            //    while (rdr.Read())
            //    {
            //        idList.Add(DatabaseApi.GetInt(rdr, 0));
            //    }
            //    rdr.Close();
            //}
            var idList = GetValueList<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ParentId, parentId)
                .OrderBy(Attr.Taxis));

            return idList.IndexOf(channelId) + 1;
        }

        public IList<string> GetIndexNameList(int siteId)
        {
            //var list = new List<string>();

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamSiteId, siteId)
            //};
            //"SELECT DISTINCT IndexName FROM siteserver_Channel WHERE SiteId = @SiteId"
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectIndexNameCollection, parameters))
            //{
            //    while (rdr.Read())
            //    {
            //        list.Add(DatabaseApi.GetString(rdr, 0));
            //    }
            //    rdr.Close();
            //}

            //return list;

            return GetValueList<string>(Q
                .Select(Attr.IndexName)
                .Where(Attr.SiteId, siteId)
                .Distinct());
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
            //var count = 0;

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamParentId, channelId)
            //};
            //"SELECT COUNT(*) FROM siteserver_Channel WHERE ParentId = @ParentId"
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectCount, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        count = DatabaseApi.GetInt(rdr, 0);
            //    }
            //    rdr.Close();
            //}
            //return count;

            return Count(Q
                .Where(Attr.ParentId, channelId));
        }

        public int GetSequence(int siteId, int channelId)
        {
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);

            //string sqlString =
            //    $"SELECT COUNT(*) AS TotalNum FROM siteserver_Channel WHERE SiteId = {siteId} AND ParentId = {channelInfo.ParentId} AND Taxis > (SELECT Taxis FROM siteserver_Channel WHERE Id = {channelId})";

            //return DatabaseApi.Instance.GetIntResult(sqlString) + 1;

            var taxis = GetValue<int>(Q
                .Select(Attr.Taxis)
                .Where(Attr.Id, channelId));
            return Count(Q
                       .Where(Attr.SiteId, siteId)
                       .Where(Attr.ParentId, channelInfo.ParentId)
                       .Where(Attr.Taxis, ">", taxis)
                   ) + 1;
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
                sqlString = SqlDifferences.GetSqlString(TableName, new List<string>
                    {
                        Attr.Id
                    },
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

            using (var rdr = DatabaseApi.Instance.ExecuteReader(WebConfigUtils.ConnectionString, sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(DatabaseApi.Instance.GetInt(rdr, 0));
                }
                rdr.Close();
            }
            return list;
        }

        public Dictionary<int, ChannelInfo> GetChannelInfoDictionaryBySiteId(int siteId)
        {
//            string sqlString =
//                $@"SELECT Id, ChannelName, SiteId, ContentModelPluginId, ContentRelatedPluginIds, ParentId, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, IndexName, GroupNameCollection, Taxis, AddDate, ImageUrl, Content, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateId, ContentTemplateId, Keywords, Description, ExtendValues
//FROM siteserver_Channel 
//WHERE (SiteId = {siteId} AND (Id = {siteId} OR ParentId > 0))
//ORDER BY Taxis";

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                while (rdr.Read())
//                {
//                    var channelInfo = GetChannelInfo(rdr);
//                    dict.Add(channelInfo.Id, channelInfo);
//                }
//                rdr.Close();
//            }

            var channelInfoList = GetObjectList(Q
                .Where(Attr.SiteId, siteId)
                .Where(q => q
                    .Where(Attr.Id, siteId)
                    .OrWhere(Attr.ParentId, ">", 0))
                .OrderBy(Attr.Taxis));

            return channelInfoList.ToDictionary(channelInfo => channelInfo.Id);
        }

        public DataSet GetStlDataSourceBySiteId(int siteId, int startNum, int totalNum, string whereString, string orderByString)
        {
            var sqlWhereString = $"WHERE (SiteId = {siteId} {whereString})";

            //var sqlSelect = DatabaseApi.Instance.GetSelectSqlString(TableName, startNum, totalNum, SqlColumns, sqlWhereString, orderByString);
            var sqlSelect = SqlDifferences.GetSqlString(TableName, SqlColumns, sqlWhereString, orderByString, startNum - 1, totalNum);

            return DatabaseApi.Instance.ExecuteDataset(WebConfigUtils.ConnectionString, sqlSelect);
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

            //var sqlSelect = DatabaseApi.Instance.GetSelectSqlString(TableName, startNum, totalNum, SqlColumns, sqlWhereString, orderByString);
            var sqlSelect = SqlDifferences.GetSqlString(TableName, SqlColumns, sqlWhereString, orderByString, startNum - 1, totalNum);

            return DatabaseApi.Instance.ExecuteDataset(WebConfigUtils.ConnectionString, sqlSelect);
        }

        public DataSet GetStlDataSetBySiteId(int siteId, int startNum, int totalNum, string whereString, string orderByString)
        {
            var sqlWhereString = $"WHERE (SiteId = {siteId} {whereString})";

            //var sqlSelect = DatabaseApi.Instance.GetSelectSqlString(TableName, startNum, totalNum, SqlColumns, sqlWhereString, orderByString);
            var sqlSelect = SqlDifferences.GetSqlString(TableName, SqlColumns, sqlWhereString, orderByString, startNum - 1, totalNum);

            return DatabaseApi.Instance.ExecuteDataset(WebConfigUtils.ConnectionString, sqlSelect);
        }

        public IList<string> GetContentModelPluginIdList()
        {
            //var list = new List<string>();

            //const string sqlString = "SELECT DISTINCT ContentModelPluginId FROM siteserver_Channel";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        list.Add(DatabaseApi.GetString(rdr, 0));
            //    }
            //    rdr.Close();
            //}
            //return list;

            return GetValueList<string>(Q
                .Select(Attr.ContentModelPluginId)
                .Distinct());
        }

        public IList<string> GetAllFilePathBySiteId(int siteId)
        {
            //var list = new List<string>();

            //var sqlString =
            //    $"SELECT FilePath FROM siteserver_Channel WHERE FilePath <> '' AND SiteId = {siteId}";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        list.Add(DatabaseApi.GetString(rdr, 0));
            //    }
            //    rdr.Close();
            //}
            //return list;

            return GetValueList<string>(Q
                .Select(Attr.FilePath)
                .Where(Attr.SiteId, siteId)
                .WhereNotNull(Attr.FilePath)
                .WhereNot(Attr.FilePath, string.Empty));
        }

        public int GetTemplateUseCount(int siteId, int templateId, TemplateType templateType, bool isDefault)
        {
            //var sqlString = string.Empty;

            if (templateType == TemplateType.IndexPageTemplate)
            {
                return isDefault ? 1 : 0;
            }

            if (templateType == TemplateType.FileTemplate)
            {
                return 1;
            }

            if (templateType == TemplateType.ChannelTemplate)
            {
                //sqlString = isDefault
                //    ? $"SELECT COUNT(*) FROM {TableName} WHERE ({Attr.ChannelTemplateId} = {templateId} OR {Attr.ChannelTemplateId} = 0) AND {Attr.SiteId} = {siteId}"
                //    : $"SELECT COUNT(*) FROM {TableName} WHERE {Attr.ChannelTemplateId} = {templateId} AND {Attr.SiteId} = {siteId}";

                if (isDefault)
                {
                    return Count(Q
                        .Where(Attr.SiteId, siteId)
                        .Where(q => q
                            .Where(Attr.ChannelTemplateId, templateId)
                            .OrWhere(Attr.ChannelTemplateId, 0)
                            .OrWhereNull(Attr.ChannelTemplateId)
                        ));
                }

                return Count(Q
                    .Where(Attr.SiteId, siteId)
                    .Where(Attr.ChannelTemplateId, templateId));
            }

            if (templateType == TemplateType.ContentTemplate)
            {
                //sqlString = isDefault
                //    ? $"SELECT COUNT(*) FROM {TableName} WHERE ({Attr.ContentTemplateId} = {templateId} OR {Attr.ContentTemplateId} = 0) AND {Attr.SiteId} = {siteId}"
                //    : $"SELECT COUNT(*) FROM {TableName} WHERE {Attr.ContentTemplateId} = {templateId} AND {Attr.SiteId} = {siteId}";

                if (isDefault)
                {
                    return Count(Q
                        .Where(Attr.SiteId, siteId)
                        .Where(q => q.Where(Attr.ContentTemplateId, templateId)
                            .OrWhere(Attr.ContentTemplateId, 0)
                            .OrWhereNull(Attr.ContentTemplateId))
                        );
                }

                return Count(Q
                    .Where(Attr.SiteId, siteId)
                    .Where(Attr.ContentTemplateId, templateId));
            }

            return 0;

            //return DatabaseApi.Instance.GetIntResult(sqlString);
        }

        public IList<int> GetChannelIdList(TemplateInfo templateInfo)
        {
            var list = new List<int>();
            //var sqlString = string.Empty;

            if (templateInfo.Type == TemplateType.ChannelTemplate)
            {
                //sqlString = templateInfo.IsDefault
                //    ? $"SELECT {Attr.Id} FROM {TableName} WHERE ({Attr.ChannelTemplateId} = {templateInfo.Id} OR {Attr.ChannelTemplateId} = 0) AND {Attr.SiteId} = {templateInfo.SiteId}"
                //    : $"SELECT {Attr.Id} FROM {TableName} WHERE {Attr.ChannelTemplateId} = {templateInfo.Id} AND {Attr.SiteId} = {templateInfo.SiteId}";

                if (templateInfo.Default)
                {
                    return GetValueList<int>(Q
                        .Select(Attr.Id)
                        .Where(Attr.SiteId, templateInfo.SiteId)
                        .OrWhere(Attr.ChannelTemplateId, templateInfo.Id)
                        .OrWhere(Attr.ChannelTemplateId, 0)
                        .OrWhereNull(Attr.ChannelTemplateId));
                }

                return GetValueList<int>(Q
                    .Select(Attr.Id)
                    .Where(Attr.SiteId, templateInfo.SiteId)
                    .Where(Attr.ChannelTemplateId, templateInfo.Id));
            }

            if (templateInfo.Type == TemplateType.ContentTemplate)
            {
                //sqlString = templateInfo.IsDefault
                //    ? $"SELECT {Attr.Id} FROM {TableName} WHERE ({Attr.ContentTemplateId} = {templateInfo.Id} OR {Attr.ContentTemplateId} = 0) AND {Attr.SiteId} = {templateInfo.SiteId}"
                //    : $"SELECT {Attr.Id} FROM {TableName} WHERE {Attr.ContentTemplateId} = {templateInfo.Id} AND {Attr.SiteId} = {templateInfo.SiteId}";

                if (templateInfo.Default)
                {
                    return GetValueList<int>(Q
                        .Select(Attr.Id)
                        .Where(Attr.SiteId, templateInfo.SiteId)
                        .OrWhere(Attr.ContentTemplateId, templateInfo.Id)
                        .OrWhere(Attr.ContentTemplateId, 0)
                        .OrWhereNull(Attr.ContentTemplateId));
                }

                return GetValueList<int>(Q
                    .Select(Attr.Id)
                    .Where(Attr.SiteId, templateInfo.SiteId)
                    .Where(Attr.ContentTemplateId, templateInfo.Id));
            }

            //if (!string.IsNullOrEmpty(sqlString))
            //{
            //    list = DatabaseApi.Instance.GetIntList(sqlString);
            //}

            return list;
        }

        //private ChannelInfo GetChannelInfo(IDataReader rdr)
        //{
        //    var i = 0;
        //    return new ChannelInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++),
        //        DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++),
        //        TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i++)), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetDateTime(rdr, i++),
        //        DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++),
        //        DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), ELinkTypeUtils.GetEnumType(DatabaseApi.GetString(rdr, i++)),
        //        DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i));
        //}
    }
}

//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Text;
//using SiteServer.CMS.Core.Attributes;
//using SiteServer.CMS.Core.Enumerations;
//using SiteServer.CMS.Database.Caches;
//using SiteServer.CMS.Database.Core;
//using SiteServer.CMS.Database.Models;
//using SiteServer.CMS.Plugin.Impl;
//using SiteServer.Plugin;
//using SiteServer.Utils;
//using SiteServer.Utils.Enumerations;

//namespace SiteServer.CMS.Database.Repositories
//{
//    public class ChannelRepository : DataProviderBase
//    {
//        private static readonly List<string> SqlColumns = new List<string>
//        {
//            ChannelAttribute.Id,
//            ChannelAttribute.AddDate,
//            ChannelAttribute.Taxis
//        };

//        public override string TableName => "siteserver_Channel";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelInfo.Id),
//                DataType = DataType.Integer,
//                IsIdentity = true,
//                IsPrimaryKey = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelInfo.ChannelName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelInfo.SiteId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelInfo.ContentModelPluginId),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelInfo.ContentRelatedPluginIds),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelInfo.ParentId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelInfo.ParentsPath),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelInfo.ParentsCount),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelInfo.ChildrenCount),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelInfo.IsLastNode),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelInfo.IndexName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelInfo.GroupNameCollection),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelInfo.Taxis),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelInfo.AddDate),
//                DataType = DataType.DateTime
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelInfo.ImageUrl),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelInfo.Content),
//                DataType = DataType.Text
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelInfo.FilePath),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelInfo.ChannelFilePathRule),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelInfo.ContentFilePathRule),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelInfo.LinkUrl),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelInfo.LinkType),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelInfo.ChannelTemplateId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelInfo.ContentTemplateId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelInfo.Keywords),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ChannelInfo.Description),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = "ExtendValues",
//                DataType = DataType.Text
//            }
//        };

//        private const string SqlSelectByLastAddDate = "SELECT Id, ChannelName, SiteId, ContentModelPluginId, ContentRelatedPluginIds, ParentId, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, IndexName, GroupNameCollection, Taxis, AddDate, ImageUrl, Content, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateId, ContentTemplateId, Keywords, Description, ExtendValues FROM siteserver_Channel WHERE ParentId = @ParentId ORDER BY AddDate Desc";

//        private const string SqlSelectByTaxis = "SELECT Id, ChannelName, SiteId, ContentModelPluginId, ContentRelatedPluginIds, ParentId, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, IndexName, GroupNameCollection, Taxis, AddDate, ImageUrl, Content, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateId, ContentTemplateId, Keywords, Description, ExtendValues FROM siteserver_Channel WHERE ParentId = @ParentId ORDER BY Taxis";

//        private const string SqlSelectParentId = "SELECT ParentId FROM siteserver_Channel WHERE Id = @Id";

//        private const string SqlSelectCount = "SELECT COUNT(*) FROM siteserver_Channel WHERE ParentId = @ParentId";

//        private const string SqlSelectSiteIdById = "SELECT SiteId FROM siteserver_Channel WHERE Id = @Id";

//        private const string SqlSelectIndexNameCollection = "SELECT DISTINCT IndexName FROM siteserver_Channel WHERE SiteId = @SiteId";

//        private const string SqlUpdate = "UPDATE siteserver_Channel SET ChannelName = @ChannelName, ContentModelPluginId = @ContentModelPluginId, ContentRelatedPluginIds = @ContentRelatedPluginIds, ParentsPath = @ParentsPath, ParentsCount = @ParentsCount, ChildrenCount = @ChildrenCount, IsLastNode = @IsLastNode, IndexName = @IndexName, GroupNameCollection = @GroupNameCollection, ImageUrl = @ImageUrl, Content = @Content, FilePath = @FilePath, ChannelFilePathRule = @ChannelFilePathRule, ContentFilePathRule = @ContentFilePathRule, LinkUrl = @LinkUrl,LinkType = @LinkType, ChannelTemplateId = @ChannelTemplateId, ContentTemplateId = @ContentTemplateId, Keywords = @Keywords, Description = @Description, ExtendValues = @ExtendValues WHERE Id = @Id";

//        private const string SqlUpdateExtendValues = "UPDATE siteserver_Channel SET ExtendValues = @ExtendValues WHERE Id = @Id";

//        private const string SqlUpdateGroupNameCollection = "UPDATE siteserver_Channel SET GroupNameCollection = @GroupNameCollection WHERE Id = @Id";

//        private const string ParamId = "@Id";
//        private const string ParamChannelName = "@ChannelName";
//        private const string ParamSiteId = "@SiteId";
//        private const string ParamContentModelPluginId = "@ContentModelPluginId";
//        private const string ParamContentRelatedPluginIds = "@ContentRelatedPluginIds";
//        private const string ParamParentId = "@ParentId";
//        private const string ParamParentsPath = "@ParentsPath";
//        private const string ParamParentsCount = "@ParentsCount";
//        private const string ParamChildrenCount = "@ChildrenCount";
//        private const string ParamIsLastNode = "@IsLastNode";
//        private const string ParamIndexName = "@IndexName";
//        private const string ParamGroupNameCollection = "@GroupNameCollection";
//        private const string ParamTaxis = "@Taxis";
//        private const string ParamAddDate = "@AddDate";
//        private const string ParamImageUrl = "@ImageUrl";
//        private const string ParamContent = "@Content";
//        private const string ParamFilePath = "@FilePath";
//        private const string ParamChannelFilePathRule = "@ChannelFilePathRule";
//        private const string ParamContentFilePathRule = "@ContentFilePathRule";
//        private const string ParamLinkUrl = "@LinkUrl";
//        private const string ParamLinkType = "@LinkType";
//        private const string ParamChannelTemplateId = "@ChannelTemplateId";
//        private const string ParamContentTemplateId = "@ContentTemplateId";
//        private const string ParamKeywords = "@Keywords";
//        private const string ParamDescription = "@Description";
//        private const string ParamExtendValues = "@ExtendValues";

//        private void InsertChannelInfoWithTrans(IChannelInfo parentChannelInfo, IChannelInfo channelInfo, IDbTransaction trans)
//        {
//            if (parentChannelInfo != null)
//            {
//                channelInfo.SiteId = parentChannelInfo.SiteId;
//                if (parentChannelInfo.ParentsPath.Length == 0)
//                {
//                    channelInfo.ParentsPath = parentChannelInfo.Id.ToString();
//                }
//                else
//                {
//                    channelInfo.ParentsPath = parentChannelInfo.ParentsPath + "," + parentChannelInfo.Id;
//                }
//                channelInfo.ParentsCount = parentChannelInfo.ParentsCount + 1;

//                var maxTaxis = GetMaxTaxisByParentPath(channelInfo.ParentsPath);
//                if (maxTaxis == 0)
//                {
//                    maxTaxis = parentChannelInfo.Taxis;
//                }
//                channelInfo.Taxis = maxTaxis + 1;
//            }
//            else
//            {
//                channelInfo.Taxis = 1;
//            }

//            const string sqlInsertNode = "INSERT INTO siteserver_Channel (ChannelName, SiteId, ContentModelPluginId, ContentRelatedPluginIds, ParentId, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, IndexName, GroupNameCollection, Taxis, AddDate, ImageUrl, Content, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateId, ContentTemplateId, Keywords, Description, ExtendValues) VALUES (@ChannelName, @SiteId, @ContentModelPluginId, @ContentRelatedPluginIds, @ParentId, @ParentsPath, @ParentsCount, @ChildrenCount, @IsLastNode, @IndexName, @GroupNameCollection, @Taxis, @AddDate, @ImageUrl, @Content, @FilePath, @ChannelFilePathRule, @ContentFilePathRule, @LinkUrl, @LinkType, @ChannelTemplateId, @ContentTemplateId, @Keywords, @Description, @ExtendValues)";

//            IDataParameter[] insertParams =
//            {
//                GetParameter(ParamChannelName, channelInfo.ChannelName),
//                GetParameter(ParamSiteId, channelInfo.SiteId),
//                GetParameter(ParamContentModelPluginId, channelInfo.ContentModelPluginId),
//                GetParameter(ParamContentRelatedPluginIds, channelInfo.ContentRelatedPluginIds),
//                GetParameter(ParamParentId, channelInfo.ParentId),
//                GetParameter(ParamParentsPath, channelInfo.ParentsPath),
//                GetParameter(ParamParentsCount, channelInfo.ParentsCount),
//                GetParameter(ParamChildrenCount, 0),
//                GetParameter(ParamIsLastNode, true.ToString()),
//                GetParameter(ParamIndexName, channelInfo.IndexName),
//                GetParameter(ParamGroupNameCollection, channelInfo.GroupNameCollection),
//                GetParameter(ParamTaxis, channelInfo.Taxis),
//                GetParameter(ParamAddDate, channelInfo.AddDate),
//                GetParameter(ParamImageUrl, channelInfo.ImageUrl),
//                GetParameter(ParamContent, channelInfo.Content),
//                GetParameter(ParamFilePath, channelInfo.FilePath),
//                GetParameter(ParamChannelFilePathRule, channelInfo.ChannelFilePathRule),
//                GetParameter(ParamContentFilePathRule, channelInfo.ContentFilePathRule),
//                GetParameter(ParamLinkUrl, channelInfo.LinkUrl),
//                GetParameter(ParamLinkType, channelInfo.LinkType),
//                GetParameter(ParamChannelTemplateId, channelInfo.ChannelTemplateId),
//                GetParameter(ParamContentTemplateId, channelInfo.ContentTemplateId),
//                GetParameter(ParamKeywords, channelInfo.Keywords),
//                GetParameter(ParamDescription, channelInfo.Description),
//                GetParameter(ParamExtendValues, channelInfo.Attributes.ToString())
//            };

//            if (channelInfo.SiteId != 0)
//            {
//                string sqlString =
//                    $"UPDATE siteserver_Channel SET Taxis = {SqlDifferences.ColumnIncrement("Taxis")} WHERE (Taxis >= {channelInfo.Taxis}) AND (SiteId = {channelInfo.SiteId})";
//                DatabaseApi.ExecuteNonQuery(trans, sqlString);
//            }
//            channelInfo.Id = DatabaseApi.ExecuteNonQueryAndReturnId(TableName, nameof(ChannelInfo.Id), trans, sqlInsertNode, insertParams);

//            if (!string.IsNullOrEmpty(channelInfo.ParentsPath))
//            {
//                var sqlString = $"UPDATE siteserver_Channel SET ChildrenCount = {SqlDifferences.ColumnIncrement("ChildrenCount")} WHERE Id IN ({channelInfo.ParentsPath})";

//                DatabaseApi.ExecuteNonQuery(trans, sqlString);
//            }

//            var sqlUpdateIsLastNode = "UPDATE siteserver_Channel SET IsLastNode = @IsLastNode WHERE ParentId = @ParentId";

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamIsLastNode, false.ToString()),
//                GetParameter(ParamParentId, channelInfo.ParentId)
//            };

//            DatabaseApi.ExecuteNonQuery(trans, sqlUpdateIsLastNode, parameters);

//            sqlUpdateIsLastNode =
//                $"UPDATE siteserver_Channel SET IsLastNode = '{true}' WHERE (Id IN ({SqlUtils.ToInTopSqlString(TableName, new List<string> { nameof(ChannelInfo.Id) }, $"WHERE ParentId = {channelInfo.ParentId}", "ORDER BY Taxis DESC", 1)}))";

//            DatabaseApi.ExecuteNonQuery(trans, sqlUpdateIsLastNode);

//            //OwningIdCache.IsChanged = true;
//            ChannelManager.RemoveCacheBySiteId(channelInfo.SiteId);
//            PermissionsImpl.ClearAllCache();
//        }

//        /// <summary>
//        /// 将节点数减1
//        /// </summary>
//        /// <param name="parentsPath"></param>
//        /// <param name="subtractNum"></param>
//        private void UpdateSubtractChildrenCount(string parentsPath, int subtractNum)
//        {
//            if (!string.IsNullOrEmpty(parentsPath))
//            {
//                var sqlString = string.Concat("UPDATE siteserver_Channel SET ChildrenCount = ChildrenCount - ", subtractNum, " WHERE Id in (", parentsPath, ")");
//                DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
//            }
//        }

//        /// <summary>
//        /// Change The Taxis To Lower Level
//        /// </summary>
//        private void TaxisSubtract(int siteId, int selectedId)
//        {
//            var channelInfo = ChannelManager.GetChannelInfo(siteId, selectedId);
//            if (channelInfo == null || channelInfo.ParentId == 0 || channelInfo.SiteId == 0) return;
//            //UpdateWholeTaxisBySiteId(channelInfo.SiteId);
//            //Get Lower Taxis and Id
//            int lowerId;
//            int lowerChildrenCount;
//            string lowerParentsPath;

//            var sqlString = SqlDifferences.GetSqlString(TableName, new List<string>
//            {
//                nameof(ChannelInfo.Id),
//                nameof(ChannelInfo.ChildrenCount),
//                nameof(ChannelInfo.ParentsPath)
//            }, "WHERE (ParentId = @ParentId) AND (Id <> @Id) AND (Taxis < @Taxis) AND (SiteId = @SiteId)", "ORDER BY Taxis DESC", 1);

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamParentId, channelInfo.ParentId),
//                GetParameter(ParamId, channelInfo.Id),
//                GetParameter(ParamTaxis, channelInfo.Taxis),
//                GetParameter(ParamSiteId, channelInfo.SiteId)
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read())
//                {
//                    lowerId = DatabaseApi.GetInt(rdr, 0);
//                    lowerChildrenCount = DatabaseApi.GetInt(rdr, 1);
//                    lowerParentsPath = DatabaseApi.GetString(rdr, 2);
//                }
//                else
//                {
//                    return;
//                }
//                rdr.Close();
//            }

//            var lowerPath = lowerParentsPath == "" ? lowerId.ToString() : string.Concat(lowerParentsPath, ",", lowerId);
//            var selectedPath = channelInfo.ParentsPath == "" ? channelInfo.Id.ToString() : string.Concat(channelInfo.ParentsPath, ",", channelInfo.Id);

//            SetTaxisSubtract(selectedId, selectedPath, lowerChildrenCount + 1);
//            SetTaxisAdd(lowerId, lowerPath, channelInfo.ChildrenCount + 1);

//            UpdateIsLastNode(channelInfo.ParentId);
//        }

//        /// <summary>
//        /// Change The Taxis To Higher Level
//        /// </summary>
//        private void TaxisAdd(int siteId, int selectedId)
//        {
//            var channelInfo = ChannelManager.GetChannelInfo(siteId, selectedId);
//            if (channelInfo == null || channelInfo.ParentId == 0 || channelInfo.SiteId == 0) return;
//            //UpdateWholeTaxisBySiteId(channelInfo.SiteId);
//            //Get Higher Taxis and Id
//            int higherId;
//            int higherChildrenCount;
//            string higherParentsPath;

//            var sqlString = SqlDifferences.GetSqlString(TableName, new List<string>
//            {
//                nameof(ChannelInfo.Id),
//                nameof(ChannelInfo.ChildrenCount),
//                nameof(ChannelInfo.ParentsPath)
//            }, "WHERE (ParentId = @ParentId) AND (Id <> @Id) AND (Taxis > @Taxis) AND (SiteId = @SiteId)", "ORDER BY Taxis", 1);

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamParentId, channelInfo.ParentId),
//                GetParameter(ParamId, channelInfo.Id),
//                GetParameter(ParamTaxis, channelInfo.Taxis),
//                GetParameter(ParamSiteId, channelInfo.SiteId)
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read())
//                {
//                    higherId = DatabaseApi.GetInt(rdr, 0);
//                    higherChildrenCount = DatabaseApi.GetInt(rdr, 1);
//                    higherParentsPath = DatabaseApi.GetString(rdr, 2);
//                }
//                else
//                {
//                    return;
//                }
//                rdr.Close();
//            }


//            var higherPath = higherParentsPath == string.Empty ? higherId.ToString() : string.Concat(higherParentsPath, ",", higherId);
//            var selectedPath = channelInfo.ParentsPath == string.Empty ? channelInfo.Id.ToString() : String.Concat(channelInfo.ParentsPath, ",", channelInfo.Id);

//            SetTaxisAdd(selectedId, selectedPath, higherChildrenCount + 1);
//            SetTaxisSubtract(higherId, higherPath, channelInfo.ChildrenCount + 1);

//            UpdateIsLastNode(channelInfo.ParentId);
//        }

//        private void SetTaxisAdd(int channelId, string parentsPath, int addNum)
//        {
//            string sqlString =
//                $"UPDATE siteserver_Channel SET Taxis = {SqlDifferences.ColumnIncrement("Taxis", addNum)} WHERE Id = {channelId} OR ParentsPath = '{parentsPath}' OR ParentsPath like '{parentsPath},%'";

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
//        }

//        private void SetTaxisSubtract(int channelId, string parentsPath, int subtractNum)
//        {
//            string sqlString =
//                $"UPDATE siteserver_Channel SET Taxis = {SqlDifferences.ColumnDecrement("Taxis", subtractNum)} WHERE  Id = {channelId} OR ParentsPath = '{parentsPath}' OR ParentsPath like '{parentsPath},%'";

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
//        }

//        private void UpdateIsLastNode(int parentId)
//        {
//            if (parentId <= 0) return;

//            var sqlString = "UPDATE siteserver_Channel SET IsLastNode = @IsLastNode WHERE  ParentId = @ParentId";

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamIsLastNode, false.ToString()),
//                GetParameter(ParamParentId, parentId)
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

//            sqlString =
//                $"UPDATE siteserver_Channel SET IsLastNode = '{true}' WHERE (Id IN ({SqlUtils.ToInTopSqlString(TableName, new List<string> { nameof(ChannelInfo.Id) }, $"WHERE ParentId = {parentId}", "ORDER BY Taxis DESC", 1)}))";

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
//        }

//        private int GetMaxTaxisByParentPath(string parentPath)
//        {
//            var cmd = string.Concat("SELECT MAX(Taxis) AS MaxTaxis FROM siteserver_Channel WHERE (ParentsPath = '", parentPath, "') OR (ParentsPath like '", parentPath, ",%')");
//            var maxTaxis = 0;

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, cmd))
//            {
//                if (rdr.Read())
//                {
//                    maxTaxis = DatabaseApi.GetInt(rdr, 0);
//                }
//                rdr.Close();
//            }
//            return maxTaxis;
//        }

//        private int GetParentId(int channelId)
//        {
//            var parentId = 0;

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamId, channelId)
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectParentId, parameters))
//            {
//                if (rdr.Read())
//                {
//                    parentId = DatabaseApi.GetInt(rdr, 0);
//                }
//                rdr.Close();
//            }
//            return parentId;
//        }

//        private int GetIdByParentIdAndOrder(int parentId, int order)
//        {
//            var channelId = parentId;

//            string cmd = $"SELECT Id FROM siteserver_Channel WHERE (ParentId = {parentId}) ORDER BY Taxis";

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, cmd))
//            {
//                var index = 1;
//                while (rdr.Read())
//                {
//                    channelId = DatabaseApi.GetInt(rdr, 0);
//                    if (index == order)
//                        break;
//                    index++;
//                }
//                rdr.Close();
//            }
//            return channelId;
//        }

//        public int InsertObject(int siteId, int parentId, string channelName, string indexName, string contentModelPluginId, string contentRelatedPluginIds, int channelTemplateId, int contentTemplateId)
//        {
//            if (siteId > 0 && parentId == 0) return 0;

//            var defaultChannelTemplateInfo = TemplateManager.GetDefaultTemplateInfo(siteId, TemplateType.ChannelTemplate);
//            var defaultContentTemplateInfo = TemplateManager.GetDefaultTemplateInfo(siteId, TemplateType.ContentTemplate);

//            var channelInfo = new ChannelInfo
//            {
//                ParentId = parentId,
//                SiteId = siteId,
//                ChannelName = channelName,
//                IndexName = indexName,
//                ContentModelPluginId = contentModelPluginId,
//                ContentRelatedPluginIds = contentRelatedPluginIds,
//                AddDate = DateTime.Now,
//                ChannelTemplateId = channelTemplateId > 0 ? channelTemplateId : defaultChannelTemplateInfo.Id,
//                ContentTemplateId = contentTemplateId > 0 ? contentTemplateId : defaultContentTemplateInfo.Id
//            };

//            var parentChannelInfo = ChannelManager.GetChannelInfo(siteId, parentId);

//            using (var conn = GetConnection())
//            {
//                conn.Open();
//                using (var trans = conn.BeginTransaction())
//                {
//                    try
//                    {
//                        InsertChannelInfoWithTrans(parentChannelInfo, channelInfo, trans);

//                        trans.Commit();
//                    }
//                    catch
//                    {
//                        trans.Rollback();
//                        throw;
//                    }
//                }
//            }

//            return channelInfo.Id;

//        }

//        public int InsertObject(IChannelInfo channelInfo)
//        {
//            if (channelInfo.SiteId > 0 && channelInfo.ParentId == 0) return 0;

//            using (var conn = GetConnection())
//            {
//                conn.Open();
//                using (var trans = conn.BeginTransaction())
//                {
//                    try
//                    {
//                        var parentChannelInfo = ChannelManager.GetChannelInfo(channelInfo.SiteId, channelInfo.ParentId);

//                        InsertChannelInfoWithTrans(parentChannelInfo, channelInfo, trans);

//                        trans.Commit();
//                    }
//                    catch
//                    {
//                        trans.Rollback();
//                        throw;
//                    }
//                }
//            }

//            return channelInfo.Id;
//        }

//        /// <summary>
//        /// 添加后台发布节点
//        /// </summary>
//        public int InsertSiteInfo(ChannelInfo channelInfo, SiteInfo siteInfo, string administratorName)
//        {
//            using (var conn = GetConnection())
//            {
//                conn.Open();
//                using (var trans = conn.BeginTransaction())
//                {
//                    try
//                    {
//                        InsertChannelInfoWithTrans(null, channelInfo, trans);

//                        siteInfo.Id = channelInfo.Id;

//                        DataProvider.Site.InsertWithTrans(siteInfo, trans);

//                        trans.Commit();
//                    }
//                    catch
//                    {
//                        trans.Rollback();
//                        throw;
//                    }
//                }
//            }

//            var adminInfo = AdminManager.GetAdminInfoByUserName(administratorName);
//            DataProvider.Administrator.UpdateSiteId(adminInfo, channelInfo.Id);

//            var sqlString =
//                $"UPDATE siteserver_Channel SET SiteId = {channelInfo.Id} WHERE Id = {channelInfo.Id}";
//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

//            DataProvider.Template.CreateDefaultTemplateInfo(channelInfo.Id, administratorName);
//            return channelInfo.Id;
//        }

//        public void UpdateObject(ChannelInfo channelInfo)
//        {
//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamChannelName, channelInfo.ChannelName),
//                GetParameter(ParamContentModelPluginId, channelInfo.ContentModelPluginId),
//                GetParameter(ParamContentRelatedPluginIds, channelInfo.ContentRelatedPluginIds),
//                GetParameter(ParamParentsPath, channelInfo.ParentsPath),
//                GetParameter(ParamParentsCount, channelInfo.ParentsCount),
//                GetParameter(ParamChildrenCount, channelInfo.ChildrenCount),
//                GetParameter(ParamIsLastNode, channelInfo.IsLastNode.ToString()),
//                GetParameter(ParamIndexName, channelInfo.IndexName),
//                GetParameter(ParamGroupNameCollection, channelInfo.GroupNameCollection),
//                GetParameter(ParamImageUrl, channelInfo.ImageUrl),
//                GetParameter(ParamContent,channelInfo.Content),
//                GetParameter(ParamFilePath, channelInfo.FilePath),
//                GetParameter(ParamChannelFilePathRule, channelInfo.ChannelFilePathRule),
//                GetParameter(ParamContentFilePathRule, channelInfo.ContentFilePathRule),
//                GetParameter(ParamLinkUrl, channelInfo.LinkUrl),
//                GetParameter(ParamLinkType, channelInfo.LinkType),
//                GetParameter(ParamChannelTemplateId, channelInfo.ChannelTemplateId),
//                GetParameter(ParamContentTemplateId, channelInfo.ContentTemplateId),
//                GetParameter(ParamKeywords, channelInfo.Keywords),
//                GetParameter(ParamDescription, channelInfo.Description),
//                GetParameter(ParamExtendValues,channelInfo.Attributes.ToString()),
//                GetParameter(ParamId, channelInfo.Id)
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, SqlUpdate, parameters);

//            ChannelManager.UpdateCache(channelInfo.SiteId, channelInfo);

//            //ChannelManager.RemoveCache(channelInfo.ParentId == 0
//            //    ? channelInfo.Id
//            //    : channelInfo.SiteId);
//        }

//        public void UpdateChannelTemplateId(ChannelInfo channelInfo)
//        {
//            string sqlString =
//                $"UPDATE siteserver_Channel SET ChannelTemplateId = {channelInfo.ChannelTemplateId} WHERE Id = {channelInfo.Id}";
//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

//            ChannelManager.UpdateCache(channelInfo.SiteId, channelInfo);
//        }

//        public void UpdateContentTemplateId(ChannelInfo channelInfo)
//        {
//            string sqlString =
//                $"UPDATE siteserver_Channel SET ContentTemplateId = {channelInfo.ContentTemplateId} WHERE Id = {channelInfo.Id}";
//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

//            ChannelManager.UpdateCache(channelInfo.SiteId, channelInfo);
//        }

//        public void UpdateAdditional(ChannelInfo channelInfo)
//        {
//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamExtendValues,channelInfo.ToString()),
//                GetParameter(ParamId, channelInfo.Id)
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, SqlUpdateExtendValues, parameters);

//            ChannelManager.UpdateCache(channelInfo.SiteId, channelInfo);
//        }

//        /// <summary>
//        /// 修改排序
//        /// </summary>
//        public void UpdateTaxis(int siteId, int selectedId, bool isSubtract)
//        {
//            if (isSubtract)
//            {
//                TaxisSubtract(siteId, selectedId);
//            }
//            else
//            {
//                TaxisAdd(siteId, selectedId);
//            }
//            ChannelManager.RemoveCacheBySiteId(siteId);
//        }

//        public void AddGroupNameList(int siteId, int channelId, List<string> groupList)
//        {
//            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
//            if (channelInfo == null) return;

//            var list = TranslateUtils.StringCollectionToStringList(channelInfo.GroupNameCollection);
//            foreach (var groupName in groupList)
//            {
//                if (!list.Contains(groupName)) list.Add(groupName);
//            }

//            channelInfo.GroupNameCollection = TranslateUtils.ObjectCollectionToString(list);

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamGroupNameCollection, channelInfo.GroupNameCollection),
//                GetParameter(ParamId, channelId)
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, SqlUpdateGroupNameCollection, parameters);

//            ChannelManager.UpdateCache(siteId, channelInfo);
//        }

//        public void DeleteById(int siteId, int channelId)
//        {
//            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
//            if (channelInfo == null) return;

//            var idList = new List<int>();
//            if (channelInfo.ChildrenCount > 0)
//            {
//                idList = ChannelManager.GetChannelIdList(channelInfo, EScopeType.Descendant, string.Empty, string.Empty, string.Empty);
//            }
//            idList.Add(channelId);

//            var deleteCmd =
//                $"DELETE FROM siteserver_Channel WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

//            int deletedNum;

//            var siteInfo = SiteManager.GetSiteInfo(siteId);

//            using (var conn = GetConnection())
//            {
//                conn.Open();
//                using (var trans = conn.BeginTransaction())
//                {
//                    try
//                    {
//                        DataProvider.ContentRepository.DeleteContentsByDeletedChannelIdList(trans, siteInfo, idList);

//                        deletedNum = DatabaseApi.ExecuteNonQuery(trans, deleteCmd);

//                        if (channelInfo.ParentId != 0)
//                        {
//                            string taxisCmd =
//                                $"UPDATE siteserver_Channel SET Taxis = Taxis - {deletedNum} WHERE (Taxis > {channelInfo.Taxis}) AND (SiteId = {channelInfo.SiteId})";
//                            DatabaseApi.ExecuteNonQuery(trans, taxisCmd);
//                        }

//                        trans.Commit();
//                    }
//                    catch
//                    {
//                        trans.Rollback();
//                        throw;
//                    }
//                }
//            }
//            UpdateIsLastNode(channelInfo.ParentId);
//            UpdateSubtractChildrenCount(channelInfo.ParentsPath, deletedNum);

//            if (channelInfo.ParentId == 0)
//            {
//                DataProvider.Site.DeleteById(channelInfo.Id);
//            }
//            else
//            {
//                ChannelManager.RemoveCacheBySiteId(channelInfo.SiteId);
//            }
//        }

//        /// <summary>
//        /// 得到最后一个添加的子节点
//        /// </summary>
//        public ChannelInfo GetChannelInfoByLastAddDate(int channelId)
//        {
//            ChannelInfo channelInfo = null;

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamParentId, channelId)
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectByLastAddDate, parameters))
//            {
//                if (rdr.Read())
//                {
//                    channelInfo = GetChannelInfo(rdr);
//                }
//                rdr.Close();
//            }
//            return channelInfo;
//        }

//        /// <summary>
//        /// 得到第一个子节点
//        /// </summary>
//        public ChannelInfo GetChannelInfoByTaxis(int channelId)
//        {
//            ChannelInfo channelInfo = null;

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamParentId, channelId)
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectByTaxis, parameters))
//            {
//                if (rdr.Read())
//                {
//                    channelInfo = GetChannelInfo(rdr);
//                }
//                rdr.Close();
//            }
//            return channelInfo;
//        }

//        public int GetIdByParentIdAndTaxis(int parentId, int taxis, bool isNextChannel)
//        {
//            var channelId = 0;

//            //var sqlString = isNextChannel ? $"SELECT TOP 1 Id FROM siteserver_Channel WHERE (ParentId = {parentId} AND Taxis > {taxis}) ORDER BY Taxis" : $"SELECT TOP 1 Id FROM siteserver_Channel WHERE (ParentId = {parentId} AND Taxis < {taxis}) ORDER BY Taxis DESC";
//            var sqlString = isNextChannel
//                ? SqlDifferences.GetSqlString(TableName, new List<string>
//                    {
//                        nameof(ChannelInfo.Id)
//                    },
//                    $"WHERE (ParentId = {parentId} AND Taxis > {taxis})", "ORDER BY Taxis", 1)
//                : SqlDifferences.GetSqlString(TableName, new List<string>
//                    {
//                        nameof(ChannelInfo.Id)
//                    },
//                    $"WHERE (ParentId = {parentId} AND Taxis < {taxis})", "ORDER BY Taxis DESC", 1);

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                if (rdr.Read())
//                {
//                    channelId = DatabaseApi.GetInt(rdr, 0);
//                }
//                rdr.Close();
//            }
//            return channelId;
//        }

//        public int GetId(int siteId, string orderString)
//        {
//            if (orderString == "1")
//                return siteId;

//            var channelId = siteId;

//            var orderArr = orderString.Split('_');
//            for (var index = 1; index < orderArr.Length; index++)
//            {
//                var order = int.Parse(orderArr[index]);
//                channelId = GetIdByParentIdAndOrder(channelId, order);
//            }
//            return channelId;
//        }

//        public int GetSiteId(int channelId)
//        {
//            var siteId = 0;

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamId, channelId)
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectSiteIdById, parameters))
//            {
//                if (rdr.Read())
//                {
//                    siteId = DatabaseApi.GetInt(rdr, 0);
//                    if (siteId == 0)
//                    {
//                        siteId = channelId;
//                    }
//                }
//                rdr.Close();
//            }
//            return siteId;
//        }

//        /// <summary>
//        /// 在节点树中得到此节点的排序号，以“1_2_5_2”的形式表示
//        /// </summary>
//        public string GetOrderStringInSite(int channelId)
//        {
//            var retVal = "";
//            if (channelId != 0)
//            {
//                var parentId = GetParentId(channelId);
//                if (parentId != 0)
//                {
//                    var orderString = GetOrderStringInSite(parentId);
//                    retVal = orderString + "_" + GetOrderInSibling(channelId, parentId);
//                }
//                else
//                {
//                    retVal = "1";
//                }
//            }
//            return retVal;
//        }

//        private int GetOrderInSibling(int channelId, int parentId)
//        {
//            string cmd = $"SELECT Id FROM siteserver_Channel WHERE (ParentId = {parentId}) ORDER BY Taxis";
//            var idList = new List<int>();
//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, cmd))
//            {
//                while (rdr.Read())
//                {
//                    idList.Add(DatabaseApi.GetInt(rdr, 0));
//                }
//                rdr.Close();
//            }
//            return idList.IndexOf(channelId) + 1;
//        }

//        public List<string> GetIndexNameList(int siteId)
//        {
//            var list = new List<string>();

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamSiteId, siteId)
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectIndexNameCollection, parameters))
//            {
//                while (rdr.Read())
//                {
//                    list.Add(DatabaseApi.GetString(rdr, 0));
//                }
//                rdr.Close();
//            }

//            return list;
//        }

//        private static string GetGroupWhereString(string group, string groupNot)
//        {
//            var whereStringBuilder = new StringBuilder();
//            if (!string.IsNullOrEmpty(group))
//            {
//                group = group.Trim().Trim(',');
//                var groupArr = group.Split(',');
//                if (groupArr.Length > 0)
//                {
//                    whereStringBuilder.Append(" AND (");
//                    foreach (var theGroup in groupArr)
//                    {
//                        var trimGroup = theGroup.Trim();

//                        whereStringBuilder.Append(
//                                $" (siteserver_Channel.GroupNameCollection = '{trimGroup}' OR {SqlUtils.GetInStr("siteserver_Channel.GroupNameCollection", trimGroup + ",")} OR {SqlUtils.GetInStr("siteserver_Channel.GroupNameCollection", "," + trimGroup + ",")} OR {SqlUtils.GetInStr("siteserver_Channel.GroupNameCollection", "," + trimGroup)}) OR ");
//                    }
//                    if (groupArr.Length > 0)
//                    {
//                        whereStringBuilder.Length = whereStringBuilder.Length - 3;
//                    }
//                    whereStringBuilder.Append(") ");
//                }
//            }

//            if (!string.IsNullOrEmpty(groupNot))
//            {
//                groupNot = groupNot.Trim().Trim(',');
//                var groupNotArr = groupNot.Split(',');
//                if (groupNotArr.Length > 0)
//                {
//                    whereStringBuilder.Append(" AND (");
//                    foreach (var theGroupNot in groupNotArr)
//                    {
//                        var trimGroupNot = theGroupNot.Trim();

//                        whereStringBuilder.Append(
//                                $" (siteserver_Channel.GroupNameCollection <> '{trimGroupNot}' AND {SqlUtils.GetNotInStr("siteserver_Channel.GroupNameCollection", trimGroupNot + ",")} AND {SqlUtils.GetNotInStr("siteserver_Channel.GroupNameCollection", "," + trimGroupNot + ",")} AND {SqlUtils.GetNotInStr("siteserver_Channel.GroupNameCollection", "," + trimGroupNot)}) AND ");
//                    }
//                    if (groupNotArr.Length > 0)
//                    {
//                        whereStringBuilder.Length = whereStringBuilder.Length - 4;
//                    }
//                    whereStringBuilder.Append(") ");
//                }
//            }
//            return whereStringBuilder.ToString();
//        }

//        public string GetWhereString(string group, string groupNot, bool isImageExists, bool isImage, string where)
//        {
//            var whereStringBuilder = new StringBuilder();
//            if (isImageExists)
//            {
//                whereStringBuilder.Append(isImage
//                    ? " AND siteserver_Channel.ImageUrl <> '' "
//                    : " AND siteserver_Channel.ImageUrl = '' ");
//            }

//            whereStringBuilder.Append(GetGroupWhereString(group, groupNot));

//            if (!string.IsNullOrEmpty(where))
//            {
//                whereStringBuilder.Append($" AND {where} ");
//            }

//            return whereStringBuilder.ToString();
//        }

//        public int GetCount(int channelId)
//        {
//            var count = 0;

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamParentId, channelId)
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectCount, parameters))
//            {
//                if (rdr.Read())
//                {
//                    count = DatabaseApi.GetInt(rdr, 0);
//                }
//                rdr.Close();
//            }
//            return count;
//        }

//        public int GetSequence(int siteId, int channelId)
//        {
//            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
//            string sqlString =
//                $"SELECT COUNT(*) AS TotalNum FROM siteserver_Channel WHERE SiteId = {siteId} AND ParentId = {channelInfo.ParentId} AND Taxis > (SELECT Taxis FROM siteserver_Channel WHERE Id = {channelId})";

//            return DatabaseApi.Instance.GetIntResult(sqlString) + 1;
//        }

//        public List<int> GetIdListByTotalNum(List<int> channelIdList, int totalNum, string orderByString, string whereString)
//        {
//            if (channelIdList == null || channelIdList.Count == 0)
//            {
//                return channelIdList;
//            }

//            string sqlString;
//            if (totalNum > 0)
//            {
//                //                sqlString = $@"SELECT TOP {totalNum} Id
//                //FROM siteserver_Channel 
//                //WHERE (Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) {whereString}) {orderByString}
//                //";
//                //var where =
//                //    $"WHERE (Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) {whereString})";
//                var where =
//                    $"WHERE {SqlUtils.GetSqlColumnInList("Id", channelIdList)} {whereString})";
//                sqlString = SqlDifferences.GetSqlString(TableName, new List<string>
//                    {
//                        nameof(ChannelInfo.Id)
//                    },
//                    where,
//                    orderByString,
//                    totalNum);
//            }
//            else
//            {
//                //                sqlString = $@"SELECT Id
//                //FROM siteserver_Channel 
//                //WHERE (Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) {whereString}) {orderByString}
//                //";
//                sqlString = $@"SELECT Id
//FROM siteserver_Channel 
//WHERE {SqlUtils.GetSqlColumnInList("Id", channelIdList)} {whereString} {orderByString}
//";
//            }

//            var list = new List<int>();

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                while (rdr.Read())
//                {
//                    list.Add(DatabaseApi.GetInt(rdr, 0));
//                }
//                rdr.Close();
//            }
//            return list;
//        }

//        public Dictionary<int, ChannelInfo> GetChannelInfoDictionaryBySiteId(int siteId)
//        {
//            var dic = new Dictionary<int, ChannelInfo>();
//            string sqlString =
//                $@"SELECT Id, ChannelName, SiteId, ContentModelPluginId, ContentRelatedPluginIds, ParentId, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, IndexName, GroupNameCollection, Taxis, AddDate, ImageUrl, Content, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateId, ContentTemplateId, Keywords, Description, ExtendValues
//FROM siteserver_Channel 
//WHERE (SiteId = {siteId} AND (Id = {siteId} OR ParentId > 0))
//ORDER BY Taxis";

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                while (rdr.Read())
//                {
//                    var channelInfo = GetChannelInfo(rdr);
//                    dic.Add(channelInfo.Id, channelInfo);
//                }
//                rdr.Close();
//            }

//            return dic;
//        }

//        public DataSet GetStlDataSourceBySiteId(int siteId, int startNum, int totalNum, string whereString, string orderByString)
//        {
//            var sqlWhereString = $"WHERE (SiteId = {siteId} {whereString})";

//            //var sqlSelect = DatabaseApi.Instance.GetSelectSqlString(TableName, startNum, totalNum, SqlColumns, sqlWhereString, orderByString);
//            var sqlSelect = SqlDifferences.GetSqlString(TableName, SqlColumns, sqlWhereString, orderByString, startNum - 1, totalNum);

//            return DatabaseApi.ExecuteDataset(ConnectionString, sqlSelect);
//        }

//        public DataSet GetStlDataSet(List<int> channelIdList, int startNum, int totalNum, string whereString, string orderByString)
//        {
//            if (channelIdList == null || channelIdList.Count == 0)
//            {
//                return null;
//            }

//            //var sqlWhereString =
//            //    $"WHERE (Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) {whereString})";

//            var sqlWhereString =
//                $"WHERE {SqlUtils.GetSqlColumnInList("Id", channelIdList)} {whereString}";

//            //var sqlSelect = DatabaseApi.Instance.GetSelectSqlString(TableName, startNum, totalNum, SqlColumns, sqlWhereString, orderByString);
//            var sqlSelect = SqlDifferences.GetSqlString(TableName, SqlColumns, sqlWhereString, orderByString, startNum - 1, totalNum);

//            return DatabaseApi.ExecuteDataset(ConnectionString, sqlSelect);
//        }

//        public DataSet GetStlDataSetBySiteId(int siteId, int startNum, int totalNum, string whereString, string orderByString)
//        {
//            var sqlWhereString = $"WHERE (SiteId = {siteId} {whereString})";

//            //var sqlSelect = DatabaseApi.Instance.GetSelectSqlString(TableName, startNum, totalNum, SqlColumns, sqlWhereString, orderByString);
//            var sqlSelect = SqlDifferences.GetSqlString(TableName, SqlColumns, sqlWhereString, orderByString, startNum - 1, totalNum);

//            return DatabaseApi.ExecuteDataset(ConnectionString, sqlSelect);
//        }

//        public List<string> GetContentModelPluginIdList()
//        {
//            var list = new List<string>();

//            const string sqlString = "SELECT DISTINCT ContentModelPluginId FROM siteserver_Channel";

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                while (rdr.Read())
//                {
//                    list.Add(DatabaseApi.GetString(rdr, 0));
//                }
//                rdr.Close();
//            }
//            return list;
//        }

//        public List<string> GetAllFilePathBySiteId(int siteId)
//        {
//            var list = new List<string>();

//            var sqlString =
//                $"SELECT FilePath FROM siteserver_Channel WHERE FilePath <> '' AND SiteId = {siteId}";

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                while (rdr.Read())
//                {
//                    list.Add(DatabaseApi.GetString(rdr, 0));
//                }
//                rdr.Close();
//            }
//            return list;
//        }

//        public int GetTemplateUseCount(int siteId, int templateId, TemplateType templateType, bool isDefault)
//        {
//            var sqlString = string.Empty;

//            if (templateType == TemplateType.IndexPageTemplate)
//            {
//                if (isDefault)
//                {
//                    return 1;
//                }
//                return 0;
//            }
//            if (templateType == TemplateType.FileTemplate)
//            {
//                return 1;
//            }
//            if (templateType == TemplateType.ChannelTemplate)
//            {
//                sqlString = isDefault
//                    ? $"SELECT COUNT(*) FROM {TableName} WHERE ({nameof(ChannelInfo.ChannelTemplateId)} = {templateId} OR {nameof(ChannelInfo.ChannelTemplateId)} = 0) AND {nameof(ChannelInfo.SiteId)} = {siteId}"
//                    : $"SELECT COUNT(*) FROM {TableName} WHERE {nameof(ChannelInfo.ChannelTemplateId)} = {templateId} AND {nameof(ChannelInfo.SiteId)} = {siteId}";
//            }
//            else if (templateType == TemplateType.ContentTemplate)
//            {
//                sqlString = isDefault
//                    ? $"SELECT COUNT(*) FROM {TableName} WHERE ({nameof(ChannelInfo.ContentTemplateId)} = {templateId} OR {nameof(ChannelInfo.ContentTemplateId)} = 0) AND {nameof(ChannelInfo.SiteId)} = {siteId}"
//                    : $"SELECT COUNT(*) FROM {TableName} WHERE {nameof(ChannelInfo.ContentTemplateId)} = {templateId} AND {nameof(ChannelInfo.SiteId)} = {siteId}";
//            }

//            return DatabaseApi.Instance.GetIntResult(sqlString);
//        }

//        public List<int> GetChannelIdList(TemplateInfo templateInfo)
//        {
//            var list = new List<int>();
//            var sqlString = string.Empty;

//            if (templateInfo.TemplateType == TemplateType.ChannelTemplate)
//            {
//                sqlString = templateInfo.IsDefault
//                    ? $"SELECT {nameof(ChannelInfo.Id)} FROM {TableName} WHERE ({nameof(ChannelInfo.ChannelTemplateId)} = {templateInfo.Id} OR {nameof(ChannelInfo.ChannelTemplateId)} = 0) AND {nameof(ChannelInfo.SiteId)} = {templateInfo.SiteId}"
//                    : $"SELECT {nameof(ChannelInfo.Id)} FROM {TableName} WHERE {nameof(ChannelInfo.ChannelTemplateId)} = {templateInfo.Id} AND {nameof(ChannelInfo.SiteId)} = {templateInfo.SiteId}";
//            }
//            else if (templateInfo.TemplateType == TemplateType.ContentTemplate)
//            {
//                sqlString = templateInfo.IsDefault
//                    ? $"SELECT {nameof(ChannelInfo.Id)} FROM {TableName} WHERE ({nameof(ChannelInfo.ContentTemplateId)} = {templateInfo.Id} OR {nameof(ChannelInfo.ContentTemplateId)} = 0) AND {nameof(ChannelInfo.SiteId)} = {templateInfo.SiteId}"
//                    : $"SELECT {nameof(ChannelInfo.Id)} FROM {TableName} WHERE {nameof(ChannelInfo.ContentTemplateId)} = {templateInfo.Id} AND {nameof(ChannelInfo.SiteId)} = {templateInfo.SiteId}";
//            }

//            if (!string.IsNullOrEmpty(sqlString))
//            {
//                list = DatabaseApi.Instance.GetIntList(sqlString);
//            }

//            return list;
//        }

//        private ChannelInfo GetChannelInfo(IDataReader rdr)
//        {
//            var i = 0;
//            return new ChannelInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++),
//                DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++),
//                TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i++)), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetDateTime(rdr, i++),
//                DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++),
//                DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), ELinkTypeUtils.GetEnumType(DatabaseApi.GetString(rdr, i++)),
//                DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i));
//        }
//    }
//}
