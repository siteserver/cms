using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.GovInteract;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.CMS.Wcm.Provider
{
    public class GovInteractContentDao : DataProviderBase
    {
        public void UpdateState(PublishmentSystemInfo publishmentSystemInfo, int contentId, EGovInteractState state)
        {
            string sqlString;
            if (state == EGovInteractState.Checked)
            {
                sqlString =
                    $"UPDATE {publishmentSystemInfo.AuxiliaryTableForGovInteract} SET State = '{EGovInteractStateUtils.GetValue(state)}', IsChecked='{true}', CheckedLevel = 0 WHERE ID = {contentId}";
            }
            else
            {
                sqlString =
                    $"UPDATE {publishmentSystemInfo.AuxiliaryTableForGovInteract} SET State = '{EGovInteractStateUtils.GetValue(state)}', IsChecked='{false}', CheckedLevel = 0 WHERE ID = {contentId}";
            }
            ExecuteNonQuery(sqlString);
        }

        public void UpdateDepartmentId(PublishmentSystemInfo publishmentSystemInfo, int contentId, int departmentId)
        {
            string sqlString =
                $"UPDATE {publishmentSystemInfo.AuxiliaryTableForGovInteract} SET DepartmentID = {departmentId} WHERE ID = {contentId}";
            ExecuteNonQuery(sqlString);
        }

        public void UpdateDepartmentId(PublishmentSystemInfo publishmentSystemInfo, ArrayList idCollection, int departmentId)
        {
            string sqlString =
                $"UPDATE {publishmentSystemInfo.AuxiliaryTableForGovInteract} SET DepartmentID = {departmentId} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(idCollection)})";

            ExecuteNonQuery(sqlString);
        }

        public GovInteractContentInfo GetContentInfo(PublishmentSystemInfo publishmentSystemInfo, int nodeId, string queryCode)
        {
            GovInteractContentInfo info = null;
            string sqlWhere =
                $"WHERE PublishmentSystemID = {publishmentSystemInfo.PublishmentSystemId} AND NodeID = {nodeId} AND QueryCode = '{queryCode}'";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(publishmentSystemInfo.AuxiliaryTableForGovInteract, SqlUtils.Asterisk, sqlWhere);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    info = new GovInteractContentInfo();
                    BaiRongDataProvider.DatabaseDao.ReadResultsToExtendedAttributes(rdr, info);
                }
                rdr.Close();
            }

            if (info != null) info.AfterExecuteReader();
            return info;
        }

        public GovInteractContentInfo GetContentInfo(PublishmentSystemInfo publishmentSystemInfo, int nodeId, NameValueCollection form)
        {
            var queryCode = GovInteractApplyManager.GetQueryCode();
            var departmentId = TranslateUtils.ToInt(form[GovInteractContentAttribute.DepartmentId]);
            var departmentName = string.Empty;
            if (departmentId > 0)
            {
                departmentName = DepartmentManager.GetDepartmentName(departmentId);
            }

            var ipAddress = PageUtils.GetIpAddress();

            var contentInfo = new GovInteractContentInfo();
            contentInfo.PublishmentSystemId = publishmentSystemInfo.PublishmentSystemId;
            contentInfo.NodeId = nodeId;
            contentInfo.DepartmentName = departmentName;
            contentInfo.QueryCode = queryCode;
            contentInfo.State = EGovInteractState.New;
            contentInfo.AddUserName = string.Empty;
            contentInfo.IpAddress = ipAddress;
            contentInfo.AddDate = DateTime.Now;

            var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(publishmentSystemInfo.PublishmentSystemId, nodeId);
            InputTypeParser.AddValuesToAttributes(ETableStyle.GovInteractContent, publishmentSystemInfo.AuxiliaryTableForGovInteract, publishmentSystemInfo, relatedIdentities, form, contentInfo.Attributes);

            //foreach (string name in form.AllKeys)
            //{
            //    if (!GovInteractContentAttribute.HiddenAttributes.Contains(name.ToLower()))
            //    {
            //        string value = form[name];
            //        if (!string.IsNullOrEmpty(value))
            //        {
            //            applyInfo.SetExtendedAttribute(name, value);
            //        }
            //    }
            //}

            return contentInfo;
        }

        public GovInteractContentInfo GetContentInfo(PublishmentSystemInfo publishmentSystemInfo, int contentId)
        {
            GovInteractContentInfo info = null;
            if (contentId > 0)
            {
                if (!string.IsNullOrEmpty(publishmentSystemInfo.AuxiliaryTableForGovInteract))
                {
                    string sqlWhere = $"WHERE ID = {contentId}";
                    var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(publishmentSystemInfo.AuxiliaryTableForGovInteract, SqlUtils.Asterisk, sqlWhere);

                    using (var rdr = ExecuteReader(sqlSelect))
                    {
                        if (rdr.Read())
                        {
                            info = new GovInteractContentInfo();
                            BaiRongDataProvider.DatabaseDao.ReadResultsToExtendedAttributes(rdr, info);
                        }
                        rdr.Close();
                    }
                }
            }

            if (info != null) info.AfterExecuteReader();
            return info;
        }

        public int GetNodeId(PublishmentSystemInfo publishmentSystemInfo, int contentId)
        {
            string sqlString =
                $"SELECT NodeID FROM {publishmentSystemInfo.AuxiliaryTableForGovInteract} WHERE ID = {contentId}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetContentNum(PublishmentSystemInfo publishmentSystemInfo)
        {
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM {publishmentSystemInfo.AuxiliaryTableForGovInteract} WHERE PublishmentSystemID = {publishmentSystemInfo.PublishmentSystemId}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetCountByDepartmentId(PublishmentSystemInfo publishmentSystemInfo, int departmentId, DateTime begin, DateTime end)
        {
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM {publishmentSystemInfo.AuxiliaryTableForGovInteract} WHERE PublishmentSystemID = {publishmentSystemInfo.PublishmentSystemId} AND DepartmentID = {departmentId} AND (AddDate BETWEEN '{begin.ToShortDateString()}' AND '{end.AddDays(1).ToShortDateString()}')";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetSelectCommendByNodeId(PublishmentSystemInfo publishmentSystemInfo, int nodeId)
        {
            return BaiRongDataProvider.ContentDao.GetSelectCommend(publishmentSystemInfo.AuxiliaryTableForGovInteract, nodeId, ETriState.All);
        }

        public string GetSelectCommendByDepartmentId(PublishmentSystemInfo publishmentSystemInfo, int departmentId)
        {
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListForDescendant(publishmentSystemInfo.Additional.GovInteractNodeId);
            nodeIdList.Add(publishmentSystemInfo.Additional.GovInteractNodeId);

            string whereString = $"DepartmentID = {departmentId}";

            return BaiRongDataProvider.ContentDao.GetSelectCommendByWhere(publishmentSystemInfo.AuxiliaryTableForGovInteract, publishmentSystemInfo.PublishmentSystemId, nodeIdList, whereString, ETriState.All);
        }

        public string GetSelectStringByState(PublishmentSystemInfo publishmentSystemInfo, int nodeId, params EGovInteractState[] states)
        {
            var whereBuilder = new StringBuilder();
            whereBuilder.Append(
                $"WHERE PublishmentSystemID = {publishmentSystemInfo.PublishmentSystemId} AND NodeID = {nodeId} AND (");
            foreach (var state in states)
            {
                whereBuilder.Append($" State = '{EGovInteractStateUtils.GetValue(state)}' OR");
            }
            whereBuilder.Length -= 2;
            whereBuilder.Append(")");
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(publishmentSystemInfo.AuxiliaryTableForGovInteract, SqlUtils.Asterisk, whereBuilder.ToString());
        }

        public string GetSelectSqlStringWithChecked(PublishmentSystemInfo publishmentSystemInfo, int nodeId, bool isReplyExists, bool isReply, int startNum, int totalNum, string whereString, string orderByString, NameValueCollection otherAttributes)
        {
            if (!string.IsNullOrEmpty(whereString) && !StringUtils.StartsWithIgnoreCase(whereString.Trim(), "AND "))
            {
                whereString = "AND " + whereString.Trim();
            }
            string sqlWhereString = $"WHERE NodeID = {nodeId} AND IsPublic = '{true.ToString()}' {whereString}";
            if (isReplyExists)
            {
                if (isReply)
                {
                    sqlWhereString += $" AND State = '{EGovInteractStateUtils.GetValue(EGovInteractState.Checked)}'";
                }
                else
                {
                    sqlWhereString +=
                        $" AND State <> '{EGovInteractStateUtils.GetValue(EGovInteractState.Checked)}' AND State <> '{EGovInteractStateUtils.GetValue(EGovInteractState.Denied)}'";
                }
            }
            if (otherAttributes != null && otherAttributes.Count > 0)
            {
                var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(publishmentSystemInfo.PublishmentSystemId, nodeId);
                var styleInfoList = TableStyleManager.GetTableStyleInfoList(ETableStyle.GovInteractContent, publishmentSystemInfo.AuxiliaryTableForGovInteract, relatedIdentities);
                foreach (var tableStyleInfo in styleInfoList)
                {
                    if (!string.IsNullOrEmpty(otherAttributes[tableStyleInfo.AttributeName.ToLower()]))
                    {
                        sqlWhereString +=
                            $" AND ({ContentAttribute.SettingsXml} like '%{tableStyleInfo.AttributeName}={otherAttributes[tableStyleInfo.AttributeName.ToLower()]}%')";
                    }
                }
            }

            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(publishmentSystemInfo.AuxiliaryTableForGovInteract, startNum, totalNum, SqlUtils.Asterisk, sqlWhereString, orderByString);
        }

        public string GetSelectString(PublishmentSystemInfo publishmentSystemInfo, int nodeId)
        {
            string whereString =
                $"WHERE PublishmentSystemID = {publishmentSystemInfo.PublishmentSystemId} AND NodeID = {nodeId}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(publishmentSystemInfo.AuxiliaryTableForGovInteract, SqlUtils.Asterisk, whereString);
        }

        public string GetSelectString(PublishmentSystemInfo publishmentSystemInfo, int nodeId, string state, string dateFrom, string dateTo, string keyword)
        {
            var whereBuilder = new StringBuilder();
            whereBuilder.Append(
                $"WHERE PublishmentSystemID = {publishmentSystemInfo.PublishmentSystemId} AND NodeID = {nodeId}");

            if (!string.IsNullOrEmpty(state))
            {
                whereBuilder.Append($" AND (State = '{state}')");
            }
            if (!string.IsNullOrEmpty(dateFrom))
            {
                whereBuilder.Append($" AND ({ContentAttribute.AddDate} >= '{dateFrom}')");
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                whereBuilder.Append($" AND ({ContentAttribute.AddDate} <= '{dateTo}')");
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                var filterKeyword = PageUtils.FilterSql(keyword);
                whereBuilder.Append($" AND (Title LIKE '{filterKeyword}' OR Content LIKE '{filterKeyword}')");
            }

            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(publishmentSystemInfo.AuxiliaryTableForGovInteract, SqlUtils.Asterisk, whereBuilder.ToString());
        }

        public int GetCountByNodeId(PublishmentSystemInfo publishmentSystemInfo, int nodeId)
        {
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM {publishmentSystemInfo.AuxiliaryTableForGovInteract} WHERE (NodeID = {nodeId})";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetCountByPublishmentSystemId(PublishmentSystemInfo publishmentSystemInfo)
        {
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM {publishmentSystemInfo.AuxiliaryTableForGovInteract} WHERE PublishmentSystemID = {publishmentSystemInfo.PublishmentSystemId}";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetCountByDepartmentId(PublishmentSystemInfo publishmentSystemInfo, int departmentId)
        {
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM {publishmentSystemInfo.AuxiliaryTableForGovInteract} WHERE PublishmentSystemID = {publishmentSystemInfo.PublishmentSystemId} AND DepartmentID = {departmentId}";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetCountByDepartmentId(PublishmentSystemInfo publishmentSystemInfo, int departmentId, int nodeId, DateTime begin, DateTime end)
        {
            string sqlString;
            if (nodeId == 0)
            {
                sqlString =
                    $"SELECT COUNT(*) AS TotalNum FROM {publishmentSystemInfo.AuxiliaryTableForGovInteract} WHERE PublishmentSystemID = {publishmentSystemInfo.PublishmentSystemId} AND DepartmentID = {departmentId} AND (AddDate BETWEEN '{begin.ToShortDateString()}' AND '{end.AddDays(1).ToShortDateString()}')";
            }
            else
            {
                sqlString =
                    $"SELECT COUNT(*) AS TotalNum FROM {publishmentSystemInfo.AuxiliaryTableForGovInteract} WHERE PublishmentSystemID = {publishmentSystemInfo.PublishmentSystemId} AND NodeID = {nodeId} AND DepartmentID = {departmentId} AND (AddDate BETWEEN '{begin.ToShortDateString()}' AND '{end.AddDays(1).ToShortDateString()}')";
            }
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetCountByDepartmentIdAndState(PublishmentSystemInfo publishmentSystemInfo, int departmentId, EGovInteractState state)
        {
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM {publishmentSystemInfo.AuxiliaryTableForGovInteract} WHERE PublishmentSystemID = {publishmentSystemInfo.PublishmentSystemId} AND DepartmentID = {departmentId} AND State = '{EGovInteractStateUtils.GetValue(state)}'";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetCountByDepartmentIdAndState(PublishmentSystemInfo publishmentSystemInfo, int departmentId, EGovInteractState state, DateTime begin, DateTime end)
        {
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM {publishmentSystemInfo.AuxiliaryTableForGovInteract} WHERE PublishmentSystemID = {publishmentSystemInfo.PublishmentSystemId} AND DepartmentID = {departmentId} AND State = '{EGovInteractStateUtils.GetValue(state)}' AND (AddDate BETWEEN '{begin.ToShortDateString()}' AND '{end.AddDays(1).ToShortDateString()}')";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetCountByDepartmentIdAndState(PublishmentSystemInfo publishmentSystemInfo, int departmentId, int nodeId, EGovInteractState state, DateTime begin, DateTime end)
        {
            string sqlString;
            if (nodeId == 0)
            {
                sqlString =
                    $"SELECT COUNT(*) AS TotalNum FROM {publishmentSystemInfo.AuxiliaryTableForGovInteract} WHERE PublishmentSystemID = {publishmentSystemInfo.PublishmentSystemId} AND DepartmentID = {departmentId} AND State = '{EGovInteractStateUtils.GetValue(state)}' AND (AddDate BETWEEN '{begin.ToShortDateString()}' AND '{end.AddDays(1).ToShortDateString()}')";
            }
            else
            {
                sqlString =
                    $"SELECT COUNT(*) AS TotalNum FROM {publishmentSystemInfo.AuxiliaryTableForGovInteract} WHERE PublishmentSystemID = {publishmentSystemInfo.PublishmentSystemId} AND DepartmentID = {departmentId} AND NodeID = {nodeId} AND State = '{EGovInteractStateUtils.GetValue(state)}' AND (AddDate BETWEEN '{begin.ToShortDateString()}' AND '{end.AddDays(1).ToShortDateString()}')";
            }
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }
    }
}
