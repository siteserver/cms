using System.Text;
using BaiRong.Core;
using System.Collections.Specialized;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Cms;
using SiteServer.BackgroundPages.Wcm;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Core
{
    public class ChannelLoading
    {
        public static string GetChannelRowHtml(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, bool enabled, ELoadingType loadingType, NameValueCollection additional, string administratorName)
        {
            var nodeTreeItem = NodeTreeItem.CreateInstance(nodeInfo, enabled, administratorName);
            var title = nodeTreeItem.GetItemHtml(loadingType, PageChannel.GetRedirectUrl(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId), additional);

            var rowHtml = string.Empty;

            if (loadingType == ELoadingType.ContentTree)
            {
                rowHtml = $@"
<tr treeItemLevel=""{nodeInfo.ParentsCount + 1}"">
	<td align=""left"" nowrap>
		{title}
	</td>
</tr>
";
            }
            else if (loadingType == ELoadingType.Channel)
            {
                var upLink = string.Empty;
                var downLink = string.Empty;
                var editUrl = string.Empty;
                var checkBoxHtml = string.Empty;

                if (enabled)
                {
                    if (AdminUtility.HasChannelPermissions(administratorName, nodeInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Cms.Permission.Channel.ChannelEdit))
                    {
                        var urlEdit = PageChannelEdit.GetRedirectUrl(nodeInfo.PublishmentSystemId, nodeInfo.NodeId, PageChannel.GetRedirectUrl(nodeInfo.PublishmentSystemId, nodeInfo.NodeId));
                        editUrl = $"<a href=\"{urlEdit}\">编辑</a>";
                        var urlSubtract = PageUtils.GetCmsUrl(nameof(PageChannel), new NameValueCollection
                        {
                            {"PublishmentSystemID", nodeInfo.PublishmentSystemId.ToString()},
                            {"Subtract", true.ToString()},
                            {"NodeID", nodeInfo.NodeId.ToString()}
                        });
                        upLink =
                            $@"<a href=""{urlSubtract}""><img src=""../Pic/icon/up.gif"" border=""0"" alt=""上升"" /></a>";
                        var urlAdd = PageUtils.GetCmsUrl(nameof(PageChannel), new NameValueCollection
                        {
                            {"PublishmentSystemID", nodeInfo.PublishmentSystemId.ToString()},
                            {"Add", true.ToString()},
                            {"NodeID", nodeInfo.NodeId.ToString()}
                        });
                        downLink =
                            $@"<a href=""{urlAdd}""><img src=""../Pic/icon/down.gif"" border=""0"" alt=""下降"" /></a>";
                    }
                    checkBoxHtml = $"<input type='checkbox' name='ChannelIDCollection' value='{nodeInfo.NodeId}' />";
                }

                rowHtml = $@"
<tr treeItemLevel=""{nodeInfo.ParentsCount + 1}"">
    <td>{title}</td>
    <td>{nodeInfo.NodeGroupNameCollection}</td>
    <td><nobr>{nodeInfo.NodeIndexName}</nobr></td>
    <td class=""center"">
	    {upLink}
    </td>
    <td class=""center"">
	    {downLink}
    </td>
    <td class=""center"">
	    {editUrl}
    </td>
    <td class=""center"">
	    {checkBoxHtml}
    </td>
</tr>
";
            }
            else if (loadingType == ELoadingType.SiteAnalysis)
            {
                var contentAddNum = string.Empty;
                var contentUpdateNum = string.Empty;

                var startDate = TranslateUtils.ToDateTime(additional["StartDate"]);
                var endDate = TranslateUtils.ToDateTime(additional["EndDate"]);

                var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
                var num = DataProvider.ContentDao.GetCountOfContentAdd(tableName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, startDate, endDate, string.Empty);
                contentAddNum = (num == 0) ? "0" : $"<strong>{num}</strong>";

                num = DataProvider.ContentDao.GetCountOfContentUpdate(tableName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, startDate, endDate, string.Empty);
                contentUpdateNum = (num == 0) ? "0" : $"<strong>{num}</strong>";

                rowHtml = $@"
<tr treeItemLevel=""{nodeInfo.ParentsCount + 1}"">
	<td>
		<nobr>{title}</nobr>
	</td>
	<td>
		{contentAddNum}
	</td>
	<td>
		{contentUpdateNum}
	</td>
</tr>
";
            }
            else if (loadingType == ELoadingType.TemplateFilePathRule)
            {
                var editLink = string.Empty;

                var filePath = string.Empty;

                if (enabled)
                {
                    var showPopWinString = ModalTemplateFilePathRule.GetOpenWindowString(nodeInfo.PublishmentSystemId, nodeInfo.NodeId);
                    editLink = $"<a href=\"javascript:;\" onclick=\"{showPopWinString}\">更改</a>";
                }
                filePath = PageUtility.GetInputChannelUrl(publishmentSystemInfo, nodeInfo);

                rowHtml = $@"
<tr treeItemLevel=""{nodeInfo.ParentsCount + 1}"">
	<td>
		<nobr>{title}</nobr>
	</td>
	<td>
		<nobr>{filePath}</nobr>
	</td>
	<td class=""center"">
		{editLink}
	</td>
</tr>
";
            }
            else if (loadingType == ELoadingType.ConfigurationCreateDetails)
            {
                var editChannelLink = string.Empty;

                var nodeNames = string.Empty;

                if (enabled)
                {
                    var showPopWinString = ModalConfigurationCreateChannel.GetOpenWindowString(nodeInfo.PublishmentSystemId, nodeInfo.NodeId);
                    editChannelLink = $"<a href=\"javascript:;\" onclick=\"{showPopWinString}\">触发栏目</a>";
                }

                if (nodeInfo.Additional.Attributes.Count > 0)
                {
                    var nodeNameBuilder = new StringBuilder();
                    var nodeIDArrayList = TranslateUtils.StringCollectionToIntList(nodeInfo.Additional.CreateChannelIDsIfContentChanged);
                    foreach (int theNodeID in nodeIDArrayList)
                    {
                        var theNodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, theNodeID);
                        if (theNodeInfo != null)
                        {
                            nodeNameBuilder.Append(theNodeInfo.NodeName).Append(",");
                        }
                    }
                    if (nodeNameBuilder.Length > 0)
                    {
                        nodeNameBuilder.Length--;
                        nodeNames = nodeNameBuilder.ToString();
                    }
                }

                rowHtml = $@"
<tr treeItemLevel=""{nodeInfo.ParentsCount + 1}"">
	<td>
		<nobr>{title}</nobr>
	</td>
	<td>
		{nodeNames}
	</td>
	<td class=""center"">
		{editChannelLink}
	</td>
</tr>
";
            }
            else if (loadingType == ELoadingType.ConfigurationCrossSiteTrans)
            {
                var editLink = string.Empty;

                var contribute = string.Empty;

                if (enabled)
                {
                    var showPopWinString = ModalCrossSiteTransEdit.GetOpenWindowString(nodeInfo.PublishmentSystemId, nodeInfo.NodeId);
                    editLink = $"<a href=\"javascript:;\" onclick=\"{showPopWinString}\">更改</a>";
                }

                contribute = CrossSiteTransUtility.GetDescription(nodeInfo.PublishmentSystemId, nodeInfo);

                rowHtml = $@"
<tr treeItemLevel=""{nodeInfo.ParentsCount + 1}"">
	<td>{title}</td>
	<td>{contribute}</td>
	<td class=""center"" width=""50"">{editLink}</td>
</tr>
";
            }
            else if (loadingType == ELoadingType.ConfigurationSignin)
            {
                var editLink = string.Empty;

                if (enabled)
                {
                    var showPopWinString = ModalConfigurationSignin.GetOpenWindowString(nodeInfo.PublishmentSystemId, nodeInfo.NodeId);
                    editLink = $"<a href=\"javascript:;\" onclick=\"{showPopWinString}\">更改</a>";
                }

                //string contribute = CrossSiteTransUtility.GetDescription(nodeInfo.PublishmentSystemID, nodeInfo);
                var isSign = "";
                var SignUser = "";
                if (nodeInfo.Additional.IsSignin)
                {
                    isSign = "是";
                }
                else
                {
                    isSign = "否";
                }
                //if (!string.IsNullOrEmpty(nodeInfo.Additional.SigninUserGroupCollection))
                //{
                //    ArrayList groupIDlist = TranslateUtils.StringCollectionToIntList(nodeInfo.Additional.SigninUserGroupCollection);
                //    UserGroupInfo userGroupInfo = null;
                //    foreach (int groupID in groupIDlist)
                //    {
                //        userGroupInfo = DataProvider.UserGroupDAO.GetUserGroupMessage(groupID);
                //        SignUser += userGroupInfo.GroupName + ',';
                //    }
                //    SignUser = SignUser.TrimEnd(',');
                //}
                //else
                //{
                SignUser = nodeInfo.Additional.SigninUserNameCollection;
                //}

                rowHtml = $@"
<tr treeItemLevel=""{nodeInfo.ParentsCount + 1}"">
	<td>{title}</td>
    <td>{SignUser}</td>
	<td class=""center"">{isSign}</td>
	<td class=""center"">{editLink}</td>
</tr>
";
            }
            else if (loadingType == ELoadingType.ChannelSelect || loadingType == ELoadingType.GovPublicChannelAdd || loadingType == ELoadingType.GovPublicChannelTree)
            {
                rowHtml = $@"
<tr treeItemLevel=""{nodeInfo.ParentsCount + 1}"">
	<td nowrap>{title}</td>
</tr>
";
            }
            else if (loadingType == ELoadingType.GovPublicChannel)
            {
                var editUrl = string.Empty;
                var upLink = string.Empty;
                var downLink = string.Empty;
                var checkBoxHtml = string.Empty;

                if (!EContentModelTypeUtils.Equals(EContentModelType.GovPublic, nodeInfo.ContentModelId))
                {
                    enabled = false;
                }

                if (enabled)
                {
                    editUrl =
                        $@"<a href=""javascript:;"" onclick=""{ModalGovPublicChannelAdd
                            .GetOpenWindowStringToEdit(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId,
                                string.Empty)}"">编辑</a>";

                    var urlUp = PageUtils.GetWcmUrl(nameof(PageGovPublicChannel), new NameValueCollection
                    {
                        {"PublishmentSystemID", nodeInfo.PublishmentSystemId.ToString()},
                        {"NodeID", nodeInfo.NodeId.ToString()},
                        {"Subtract", true.ToString()}
                    });
                    upLink = $@"<a href=""{urlUp}""><img src=""../Pic/icon/up.gif"" border=""0"" alt=""上升"" /></a>";

                    var urlDown = PageUtils.GetWcmUrl(nameof(PageGovPublicChannel), new NameValueCollection
                    {
                        {"PublishmentSystemID", nodeInfo.PublishmentSystemId.ToString()},
                        {"NodeID", nodeInfo.NodeId.ToString()},
                        {"Add", true.ToString()}
                    });
                    downLink =
                        $@"<a href=""{urlDown}""><img src=""../Pic/icon/down.gif"" border=""0"" alt=""下降"" /></a>";

                    checkBoxHtml = $"<input type='checkbox' name='ChannelIDCollection' value='{nodeInfo.NodeId}' />";
                }

                var channelCode = DataProvider.GovPublicChannelDao.GetCode(nodeInfo.NodeId);

                rowHtml = $@"
<tr treeItemLevel=""{nodeInfo.ParentsCount + 1}"">
    <td>{title}</td>
    <td>{channelCode}</td>
    <td class=""center"">{upLink}</td>
    <td class=""center"">{downLink}</td>
    <td class=""center"">{editUrl}</td>
    <td class=""center"">{checkBoxHtml}</td>
</tr>
";
            }
            else if (loadingType == ELoadingType.GovInteractChannel)
            {
                var editUrl = string.Empty;
                var upLink = string.Empty;
                var downLink = string.Empty;
                var styleAddUrl = string.Empty;
                var checkBoxHtml = string.Empty;

                if (enabled)
                {
                    var applyStyleId = DataProvider.GovInteractChannelDao.GetApplyStyleId(nodeInfo.PublishmentSystemId, nodeInfo.NodeId);
                    editUrl =
                        $@"<a href=""javascript:;"" onclick=""{ModalGovInteractChannelAdd
                            .GetOpenWindowStringToEdit(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId,
                                string.Empty)}"">编辑</a>";

                    var urlUp = PageUtils.GetWcmUrl(nameof(PageGovInteractChannel), new NameValueCollection
                    {
                        {"PublishmentSystemID", nodeInfo.PublishmentSystemId.ToString()},
                        {"NodeID", nodeInfo.NodeId.ToString()},
                        {"Subtract", true.ToString()}
                    });
                    upLink = $@"<a href=""{urlUp}""><img src=""../Pic/icon/up.gif"" border=""0"" alt=""上升"" /></a>";

                    var urlDown = PageUtils.GetWcmUrl(nameof(PageGovInteractChannel), new NameValueCollection
                    {
                        {"PublishmentSystemID", nodeInfo.PublishmentSystemId.ToString()},
                        {"NodeID", nodeInfo.NodeId.ToString()},
                        {"Add", true.ToString()}
                    });
                    downLink =
                        $@"<a href=""{urlDown}""><img src=""../Pic/icon/down.gif"" border=""0"" alt=""下降"" /></a>";

                    styleAddUrl =
                        $@"<a href=""javascript:;"" onclick=""{ModalTagStyleGovInteractApplyAdd.GetOpenWindowStringToEdit(publishmentSystemInfo.PublishmentSystemId, applyStyleId)}"">提交设置</a>";
                    checkBoxHtml = $"<input type='checkbox' name='ChannelIDCollection' value='{nodeInfo.NodeId}' />";
                }

                var summary = DataProvider.GovInteractChannelDao.GetSummary(nodeInfo.NodeId);

                rowHtml = $@"
<tr treeItemLevel=""{nodeInfo.ParentsCount + 1}"">
    <td>{title}</td>
    <td>{summary}</td>
    <td class=""center"">{upLink}</td>
    <td class=""center"">{downLink}</td>
    <td class=""center"">{styleAddUrl}</td>
    <td class=""center"">{editUrl}</td>
    <td class=""center"">{checkBoxHtml}</td>
</tr>
";
            }
            else if (loadingType == ELoadingType.GovPublicChannelAdd || loadingType == ELoadingType.GovPublicChannelTree)
            {
                rowHtml = $@"
<tr treeItemLevel=""{nodeInfo.ParentsCount + 1}"">
	<td nowrap>{title}</td>
</tr>
";
            }

            return rowHtml;
        }

        public static string GetScript(PublishmentSystemInfo publishmentSystemInfo, ELoadingType loadingType, NameValueCollection additional)
        {
            return NodeTreeItem.GetScript(publishmentSystemInfo, loadingType, additional);
        }

        public static string GetScriptOnLoad(int publishmentSystemID, int currentNodeID)
        {
            if (currentNodeID != 0 && currentNodeID != publishmentSystemID)
            {
                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemID, currentNodeID);
                if (nodeInfo != null)
                {
                    var path = string.Empty;
                    if (nodeInfo.ParentId == publishmentSystemID)
                    {
                        path = currentNodeID.ToString();
                    }
                    else
                    {
                        path = nodeInfo.ParentsPath.Substring(nodeInfo.ParentsPath.IndexOf(",") + 1) + "," + currentNodeID.ToString();
                    }
                    return NodeTreeItem.GetScriptOnLoad(path);
                }
            }
            return string.Empty;
        }
    }
}
