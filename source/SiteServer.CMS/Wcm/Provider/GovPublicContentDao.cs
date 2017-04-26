using System;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Wcm.GovPublic;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.CMS.Wcm.Provider
{
    public class GovPublicContentDao : DataProviderBase
	{
        public GovPublicContentInfo GetContentInfo(PublishmentSystemInfo publishmentSystemInfo, int contentId)
        {
            GovPublicContentInfo info = null;
            if (contentId > 0)
            {
                if (!string.IsNullOrEmpty(publishmentSystemInfo.AuxiliaryTableForGovPublic))
                {
                    string sqlWhere = $"WHERE ID = {contentId}";
                    var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(publishmentSystemInfo.AuxiliaryTableForGovPublic, SqlUtils.Asterisk, sqlWhere);

                    using (var rdr = ExecuteReader(sqlSelect))
                    {
                        if (rdr.Read())
                        {
                            info = new GovPublicContentInfo();
                            BaiRongDataProvider.DatabaseDao.ReadResultsToExtendedAttributes(rdr, info);
                        }
                        rdr.Close();
                    }
                }
            }

            info?.AfterExecuteReader();
            return info;
        }

        public int GetContentNum(PublishmentSystemInfo publishmentSystemInfo)
        {
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM {publishmentSystemInfo.AuxiliaryTableForGovPublic} WHERE PublishmentSystemID = {publishmentSystemInfo.PublishmentSystemId}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetCountByDepartmentId(PublishmentSystemInfo publishmentSystemInfo, int departmentId, DateTime begin, DateTime end)
        {
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM {publishmentSystemInfo.AuxiliaryTableForGovPublic} WHERE PublishmentSystemID = {publishmentSystemInfo.PublishmentSystemId} AND DepartmentID = {departmentId} AND (AddDate BETWEEN '{begin.ToShortDateString()}' AND '{end.AddDays(1).ToShortDateString()}')";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetSelectCommendByNodeId(PublishmentSystemInfo publishmentSystemInfo, int nodeId)
        {
            return BaiRongDataProvider.ContentDao.GetSelectCommend(publishmentSystemInfo.AuxiliaryTableForGovPublic, nodeId, ETriState.All);
        }

        public string GetSelectCommendByDepartmentId(PublishmentSystemInfo publishmentSystemInfo, int departmentId)
        {
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListForDescendant(publishmentSystemInfo.Additional.GovPublicNodeId);
            nodeIdList.Add(publishmentSystemInfo.Additional.GovPublicNodeId);

            string whereString = $"DepartmentID = {departmentId}";

            return BaiRongDataProvider.ContentDao.GetSelectCommendByWhere(publishmentSystemInfo.AuxiliaryTableForGovPublic, publishmentSystemInfo.PublishmentSystemId, nodeIdList, whereString, ETriState.All);
        }

        public string GetSelectCommendByCategoryId(PublishmentSystemInfo publishmentSystemInfo, string classCode, int categoryId)
        {
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListForDescendant(publishmentSystemInfo.Additional.GovPublicNodeId);
            nodeIdList.Add(publishmentSystemInfo.Additional.GovPublicNodeId);

            var attributeName = DataProvider.GovPublicCategoryClassDao.GetContentAttributeName(classCode, publishmentSystemInfo.PublishmentSystemId);

            string whereString = $"{attributeName} = {categoryId}";

            return BaiRongDataProvider.ContentDao.GetSelectCommendByWhere(publishmentSystemInfo.AuxiliaryTableForGovPublic, publishmentSystemInfo.PublishmentSystemId, nodeIdList, whereString, ETriState.All);
        }

        public void CreateIdentifier(PublishmentSystemInfo publishmentSystemInfo, int parentNodeId, bool isAll)
        {
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListForDescendant(parentNodeId);
            nodeIdList.Add(parentNodeId);
            foreach (int nodeId in nodeIdList)
            {
                var contentIdList = BaiRongDataProvider.ContentDao.GetContentIdList(publishmentSystemInfo.AuxiliaryTableForGovPublic, nodeId);
                foreach (int contentId in contentIdList)
                {
                    var contentInfo = GetContentInfo(publishmentSystemInfo, contentId);
                    if (isAll || string.IsNullOrEmpty(contentInfo.Identifier))
                    {
                        var identifier = GovPublicManager.GetIdentifier(publishmentSystemInfo, contentInfo.NodeId, contentInfo.DepartmentId, contentInfo);
                        contentInfo.Identifier = identifier;
                        DataProvider.ContentDao.Update(publishmentSystemInfo.AuxiliaryTableForGovPublic, publishmentSystemInfo, contentInfo);
                    }
                }
            }
        }
	}
}
