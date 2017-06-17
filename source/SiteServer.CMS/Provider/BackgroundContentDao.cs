using System.Collections.Generic;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Core.User;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Provider
{
	public class BackgroundContentDao : DataProviderBase
	{
        public List<KeyValuePair<int, int>> GetCountArrayListUnChecked(bool isSystemAdministrator, string administratorName, List<int> publishmentSystemIdList, List<int> owningNodeIdList, string tableName)
        {
            var list = new List<KeyValuePair<int, int>>();

            var publishmentSystemIdArrayList = PublishmentSystemManager.GetPublishmentSystemIdList();
            foreach (int publishmentSystemId in publishmentSystemIdArrayList)
            {
                if (!publishmentSystemIdList.Contains(publishmentSystemId)) continue;

                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                if (!isSystemAdministrator)
                {
                    //if (!owningNodeIDArrayList.Contains(psID)) continue;
                    //if (!AdminUtility.HasChannelPermissions(psID, psID, AppManager.CMS.Permission.Channel.ContentCheck)) continue;

                    var isContentCheck = false;
                    foreach (var theNodeId in owningNodeIdList)
                    {
                        if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemId, theNodeId, AppManager.Cms.Permission.Channel.ContentCheck))
                        {
                            isContentCheck = true;
                        }
                    }
                    if (!isContentCheck)
                    {
                        continue;
                    }
                }

                int checkedLevel;
                var isChecked = CheckManager.GetUserCheckLevel(administratorName, publishmentSystemInfo, publishmentSystemInfo.PublishmentSystemId, out checkedLevel);
                var checkLevelArrayList = LevelManager.LevelInt.GetCheckLevelArrayListOfNeedCheck(publishmentSystemInfo, isChecked, checkedLevel);
                string sqlString;
                if (isSystemAdministrator)
                {
                    sqlString =
                        $"SELECT COUNT(*) AS TotalNum FROM {tableName} WHERE (PublishmentSystemID = {publishmentSystemId} AND NodeID > 0 AND IsChecked = '{false}' AND CheckedLevel IN ({TranslateUtils.ToSqlInStringWithoutQuote(checkLevelArrayList)}))";
                }
                else
                {
                    sqlString =
                        $"SELECT COUNT(*) AS TotalNum FROM {tableName} WHERE (PublishmentSystemID = {publishmentSystemId} AND NodeID IN ({TranslateUtils.ToSqlInStringWithoutQuote(owningNodeIdList)}) AND IsChecked = '{false}' AND CheckedLevel IN ({TranslateUtils.ToSqlInStringWithoutQuote(checkLevelArrayList)}))";
                }

                var count = BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
                if (count > 0)
                {
                    list.Add(new KeyValuePair<int, int>(publishmentSystemId, count));
                }
            }
            return list;
        }

		public BackgroundContentInfo GetContentInfo(string tableName, int contentId)
		{
			BackgroundContentInfo info = null;
            if (contentId > 0)
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    string sqlWhere = $"WHERE ID = {contentId}";
                    var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, sqlWhere);

                    using (var rdr = ExecuteReader(sqlSelect))
                    {
                        if (rdr.Read())
                        {
                            info = new BackgroundContentInfo();
                            BaiRongDataProvider.DatabaseDao.ReadResultsToExtendedAttributes(rdr, info);
                        }
                        rdr.Close();
                    }
                }
            }

		    info?.AfterExecuteReader();
		    return info;
		}

        public int GetCountCheckedImage(int publishmentSystemId, int nodeId)
        {
            var tableName = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId).AuxiliaryTableForContent;
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM {tableName} WHERE (NodeID = {nodeId} AND ImageUrl <> '' AND {ContentAttribute.IsChecked} = '{true}')";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetStlWhereString(PublishmentSystemInfo publishmentSystemInfo, string tableName, string group, string groupNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where)
        {
            var whereBuilder = new StringBuilder();
            whereBuilder.Append($" AND PublishmentSystemID = {publishmentSystemInfo.PublishmentSystemId} ");

            if (isImageExists)
            {
                if (isImage)
                {
                    whereBuilder.Append($" AND {BackgroundContentAttribute.ImageUrl} <> '' ");
                }
                else
                {
                    whereBuilder.Append($" AND {BackgroundContentAttribute.ImageUrl} = '' ");
                }
            }

            if (isVideoExists)
            {
                if (isVideo)
                {
                    whereBuilder.Append($" AND {BackgroundContentAttribute.VideoUrl} <> '' ");
                }
                else
                {
                    whereBuilder.Append($" AND {BackgroundContentAttribute.VideoUrl} = '' ");
                }
            }

            if (isFileExists)
            {
                if (isFile)
                {
                    whereBuilder.Append($" AND {BackgroundContentAttribute.FileUrl} <> '' ");
                }
                else
                {
                    whereBuilder.Append($" AND {BackgroundContentAttribute.FileUrl} = '' ");
                }
            }

            if (isTopExists)
            {
                whereBuilder.Append($" AND IsTop = '{isTop}' ");
            }

            if (isRecommendExists)
            {
                whereBuilder.Append($" AND {BackgroundContentAttribute.IsRecommend} = '{isRecommend}' ");
            }

            if (isHotExists)
            {
                whereBuilder.Append($" AND {BackgroundContentAttribute.IsHot} = '{isHot}' ");
            }

            if (isColorExists)
            {
                whereBuilder.Append($" AND {BackgroundContentAttribute.IsColor} = '{isColor}' ");
            }

            if (!string.IsNullOrEmpty(group))
            {
                group = group.Trim().Trim(',');
                var groupArr = group.Split(',');
                if (groupArr != null && groupArr.Length > 0)
                {
                    whereBuilder.Append(" AND (");
                    foreach (var theGroup in groupArr)
                    {
                        var trimGroup = theGroup.Trim();
                        //whereBuilder.Append(
                        //    $" ({ContentAttribute.ContentGroupNameCollection} = '{trimGroup}' OR CHARINDEX('{trimGroup},',{ContentAttribute.ContentGroupNameCollection}) > 0 OR CHARINDEX(',{trimGroup},',{ContentAttribute.ContentGroupNameCollection}) > 0 OR CHARINDEX(',{trimGroup}',{ContentAttribute.ContentGroupNameCollection}) > 0) OR ");

                        whereBuilder.Append(
                                $" ({ContentAttribute.ContentGroupNameCollection} = '{trimGroup}' OR {SqlUtils.GetInStr(ContentAttribute.ContentGroupNameCollection, trimGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.ContentGroupNameCollection, "," + trimGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.ContentGroupNameCollection, "," + trimGroup)}) OR ");
                    }
                    if (groupArr.Length > 0)
                    {
                        whereBuilder.Length = whereBuilder.Length - 3;
                    }
                    whereBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(groupNot))
            {
                groupNot = groupNot.Trim().Trim(',');
                var groupNotArr = groupNot.Split(',');
                if (groupNotArr != null && groupNotArr.Length > 0)
                {
                    whereBuilder.Append(" AND (");
                    foreach (var theGroupNot in groupNotArr)
                    {
                        var trimGroup = theGroupNot.Trim();
                        //whereBuilder.Append(
                        //    $" ({ContentAttribute.ContentGroupNameCollection} <> '{trimGroup}' AND CHARINDEX('{trimGroup},',{ContentAttribute.ContentGroupNameCollection}) = 0 AND CHARINDEX(',{trimGroup},',{ContentAttribute.ContentGroupNameCollection}) = 0 AND CHARINDEX(',{trimGroup}',{ContentAttribute.ContentGroupNameCollection}) = 0) AND ");

                        whereBuilder.Append(
                                $" ({ContentAttribute.ContentGroupNameCollection} <> '{trimGroup}' AND {SqlUtils.GetNotInStr(ContentAttribute.ContentGroupNameCollection, trimGroup + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.ContentGroupNameCollection, "," + trimGroup + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.ContentGroupNameCollection, "," + trimGroup)}) AND ");
                    }
                    if (groupNotArr.Length > 0)
                    {
                        whereBuilder.Length = whereBuilder.Length - 4;
                    }
                    whereBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(tags))
            {
                var tagCollection = TagUtils.ParseTagsString(tags);
                var contentIdArrayList = BaiRongDataProvider.TagDao.GetContentIdListByTagCollection(tagCollection, publishmentSystemInfo.PublishmentSystemId);
                if (contentIdArrayList.Count > 0)
                {
                    whereBuilder.Append(
                        $" AND (ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdArrayList)}))");
                }
            }

            if (!string.IsNullOrEmpty(where))
            {
                whereBuilder.Append($" AND ({@where}) ");
            }

            if (!publishmentSystemInfo.Additional.IsCreateSearchDuplicate)
            {
                var sqlString = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(tableName, "MIN(ID)", whereBuilder + " GROUP BY Title");
                whereBuilder.Append($" AND ID IN ({sqlString}) ");
            }

            return whereBuilder.ToString();
        }

        public string GetStlWhereStringBySearch(string tableName, string group, string groupNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where)
        {
            var whereBuilder = new StringBuilder();

            if (isImageExists)
            {
                if (isImage)
                {
                    whereBuilder.Append($" AND {BackgroundContentAttribute.ImageUrl} <> '' ");
                }
                else
                {
                    whereBuilder.Append($" AND {BackgroundContentAttribute.ImageUrl} = '' ");
                }
            }

            if (isVideoExists)
            {
                if (isVideo)
                {
                    whereBuilder.Append($" AND {BackgroundContentAttribute.VideoUrl} <> '' ");
                }
                else
                {
                    whereBuilder.Append($" AND {BackgroundContentAttribute.VideoUrl} = '' ");
                }
            }

            if (isFileExists)
            {
                if (isFile)
                {
                    whereBuilder.Append($" AND {BackgroundContentAttribute.FileUrl} <> '' ");
                }
                else
                {
                    whereBuilder.Append($" AND {BackgroundContentAttribute.FileUrl} = '' ");
                }
            }

            if (isTopExists)
            {
                whereBuilder.Append($" AND IsTop = '{isTop}' ");
            }

            if (isRecommendExists)
            {
                whereBuilder.Append($" AND {BackgroundContentAttribute.IsRecommend} = '{isRecommend}' ");
            }

            if (isHotExists)
            {
                whereBuilder.Append($" AND {BackgroundContentAttribute.IsHot} = '{isHot}' ");
            }

            if (isColorExists)
            {
                whereBuilder.Append($" AND {BackgroundContentAttribute.IsColor} = '{isColor}' ");
            }

            if (!string.IsNullOrEmpty(group))
            {
                group = group.Trim().Trim(',');
                var groupArr = group.Split(',');
                if (groupArr != null && groupArr.Length > 0)
                {
                    whereBuilder.Append(" AND (");
                    foreach (var theGroup in groupArr)
                    {
                        var trimGroup = theGroup.Trim();
                        //whereBuilder.Append(
                        //    $" ({ContentAttribute.ContentGroupNameCollection} = '{trimGroup}' OR CHARINDEX('{trimGroup},',{ContentAttribute.ContentGroupNameCollection}) > 0 OR CHARINDEX(',{trimGroup},',{ContentAttribute.ContentGroupNameCollection}) > 0 OR CHARINDEX(',{trimGroup}',{ContentAttribute.ContentGroupNameCollection}) > 0) OR ");

                        whereBuilder.Append(
                                $" ({ContentAttribute.ContentGroupNameCollection} = '{trimGroup}' OR {SqlUtils.GetInStr(ContentAttribute.ContentGroupNameCollection, trimGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.ContentGroupNameCollection, "," + trimGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.ContentGroupNameCollection, "," + trimGroup)}) OR ");
                    }
                    if (groupArr.Length > 0)
                    {
                        whereBuilder.Length = whereBuilder.Length - 3;
                    }
                    whereBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(groupNot))
            {
                groupNot = groupNot.Trim().Trim(',');
                var groupNotArr = groupNot.Split(',');
                if (groupNotArr != null && groupNotArr.Length > 0)
                {
                    whereBuilder.Append(" AND (");
                    foreach (var theGroupNot in groupNotArr)
                    {
                        var trimGroup = theGroupNot.Trim();
                        //whereBuilder.Append(
                        //    $" ({ContentAttribute.ContentGroupNameCollection} <> '{trimGroup}' AND CHARINDEX('{trimGroup},',{ContentAttribute.ContentGroupNameCollection}) = 0 AND CHARINDEX(',{trimGroup},',{ContentAttribute.ContentGroupNameCollection}) = 0 AND CHARINDEX(',{trimGroup}',{ContentAttribute.ContentGroupNameCollection}) = 0) AND ");

                        whereBuilder.Append(
                                $" ({ContentAttribute.ContentGroupNameCollection} <> '{trimGroup}' AND {SqlUtils.GetNotInStr(ContentAttribute.ContentGroupNameCollection, trimGroup + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.ContentGroupNameCollection, "," + trimGroup + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.ContentGroupNameCollection, "," + trimGroup)}) AND ");
                    }
                    if (groupNotArr.Length > 0)
                    {
                        whereBuilder.Length = whereBuilder.Length - 4;
                    }
                    whereBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(where))
            {
                whereBuilder.Append($" AND ({where}) ");
            }

            return whereBuilder.ToString();
        }

        public string GetSelectCommendByDownloads(string tableName, int publishmentSystemId)
        {
            var whereString = new StringBuilder();
            whereString.Append(
                $"WHERE (PublishmentSystemID = {publishmentSystemId} AND IsChecked='True' AND FileUrl <> '') ");

            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, whereString.ToString());
        }
	}
}
