using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Text;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageChannelTranslate : BasePageCms
    {
        public ListBox NodeIDFrom;
		public DropDownList NodeIDTo;
		public DropDownList TranslateType;
        public RadioButtonList IsIncludeDesendents;
		public RadioButtonList IsDeleteAfterTranslate;

		public DropDownList PublishmentSystemIDDropDownList;

        public PlaceHolder phReturn;
        public Button Submit;

		private bool[] isLastNodeArray;
		public string SystemKeyword;
        private string returnUrl;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageChannelTranslate), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetCmsUrl(nameof(PageChannelTranslate), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()}
            });
        }

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(nameof(PageChannelTranslate), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public static string GetRedirectUrl(int publishmentSystemId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(nameof(PageChannelTranslate), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			PageUtils.CheckRequestParameter("PublishmentSystemID");
            returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));

            if (!HasChannelPermissions(PublishmentSystemId, AppManager.Cms.Permission.Channel.ContentDelete))
			{
				IsDeleteAfterTranslate.Visible = false;
			}
			
			if (!IsPostBack)
			{
                BreadCrumb(AppManager.Cms.LeftMenu.IdContent, "批量转移", string.Empty);

                phReturn.Visible = !string.IsNullOrEmpty(returnUrl);
				ETranslateTypeUtils.AddListItems(TranslateType);
                if (Body.IsQueryExists("ChannelIDCollection"))
                {
                    ControlUtils.SelectListItems(TranslateType, ETranslateTypeUtils.GetValue(ETranslateType.All));
                }
                else
                {
                    ControlUtils.SelectListItems(TranslateType, ETranslateTypeUtils.GetValue(ETranslateType.Content));
                }

				IsDeleteAfterTranslate.Items[0].Value = true.ToString();
				IsDeleteAfterTranslate.Items[1].Value = false.ToString();

                var publishmentSystemIDList = ProductPermissionsManager.Current.PublishmentSystemIdList;
                foreach (var psID in publishmentSystemIDList)
				{
					var psInfo = PublishmentSystemManager.GetPublishmentSystemInfo(psID);
                    var listitem = new ListItem(psInfo.PublishmentSystemName, psID.ToString());
                    if (psID == PublishmentSystemId) listitem.Selected = true;
                    PublishmentSystemIDDropDownList.Items.Add(listitem);
				}

                var nodeIDStrArrayList = new List<string>();
                if (Body.IsQueryExists("ChannelIDCollection"))
                {
                    nodeIDStrArrayList = TranslateUtils.StringCollectionToStringList(Body.GetQueryString("ChannelIDCollection"));
                }

				var nodeIdList = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(PublishmentSystemId);
                var nodeCount = nodeIdList.Count;
				isLastNodeArray = new bool[nodeCount];
                foreach (int theNodeID in nodeIdList)
				{
                    var enabled = IsOwningNodeId(theNodeID);
                    if (!enabled)
                    {
                        if (!IsHasChildOwningNodeId(theNodeID)) continue;
                    }
                    var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, theNodeID);

                    var value = enabled ? nodeInfo.NodeId.ToString() : string.Empty;
                    value = (nodeInfo.Additional.IsContentAddable) ? value : string.Empty;

                    var text = GetTitle(nodeInfo);
					var listItem = new ListItem(text, value);
                    if (nodeIDStrArrayList.Contains(value))
                    {
                        listItem.Selected = true;
                    }
                    NodeIDFrom.Items.Add(listItem);
                    listItem = new ListItem(text, value);
                    NodeIDTo.Items.Add(listItem);
				}
			}
		}

		public string GetTitle(NodeInfo nodeInfo)
		{
			var str = "";
            if (nodeInfo.NodeId == PublishmentSystemId)
			{
                nodeInfo.IsLastNode = true;
			}
            if (nodeInfo.IsLastNode == false)
			{
                isLastNodeArray[nodeInfo.ParentsCount] = false;
			}
			else
			{
                isLastNodeArray[nodeInfo.ParentsCount] = true;
			}
            for (var i = 0; i < nodeInfo.ParentsCount; i++)
			{
				if (isLastNodeArray[i])
				{
					str = string.Concat(str, "　");
				}
				else
				{
					str = string.Concat(str, "│");
				}
			}
            if (nodeInfo.IsLastNode)
			{
				str = string.Concat(str, "└");
			}
			else
			{
				str = string.Concat(str, "├");
			}
            str = string.Concat(str, nodeInfo.NodeName);
            if (nodeInfo.ContentNum != 0)
            {
                str = $"{str} ({nodeInfo.ContentNum})";
            }
			return str;
		}

        public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack)
			{
				var targetNodeID = int.Parse(NodeIDTo.SelectedValue);

				var targetPublishmentSystemID = int.Parse(PublishmentSystemIDDropDownList.SelectedValue);
				var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemID);
				var isChecked = false;
				var checkedLevel = 0;
                if (targetPublishmentSystemInfo.CheckContentLevel == 0 || AdminUtility.HasChannelPermissions(Body.AdministratorName, targetPublishmentSystemID, targetNodeID, AppManager.Cms.Permission.Channel.ContentAdd, AppManager.Cms.Permission.Channel.ContentCheck))
				{
					isChecked = true;
					checkedLevel = 0;
				}
				else
				{
					var UserCheckLevel = 0;
					var OwnHighestLevel = false;

                    if (AdminUtility.HasChannelPermissions(Body.AdministratorName, targetPublishmentSystemID, targetNodeID, AppManager.Cms.Permission.Channel.ContentCheckLevel1))
                    {
                        UserCheckLevel = 1;
                        if (AdminUtility.HasChannelPermissions(Body.AdministratorName, targetPublishmentSystemID, targetNodeID, AppManager.Cms.Permission.Channel.ContentCheckLevel2))
                        {
                            UserCheckLevel = 2;
                            if (AdminUtility.HasChannelPermissions(Body.AdministratorName, targetPublishmentSystemID, targetNodeID, AppManager.Cms.Permission.Channel.ContentCheckLevel3))
                            {
                                UserCheckLevel = 3;
                                if (AdminUtility.HasChannelPermissions(Body.AdministratorName, targetPublishmentSystemID, targetNodeID, AppManager.Cms.Permission.Channel.ContentCheckLevel4))
                                {
                                    UserCheckLevel = 4;
                                    if (AdminUtility.HasChannelPermissions(Body.AdministratorName, targetPublishmentSystemID, targetNodeID, AppManager.Cms.Permission.Channel.ContentCheckLevel5))
                                    {
                                        UserCheckLevel = 5;
                                    }
                                }
                            }
                        }
                    }

                    if (UserCheckLevel >= targetPublishmentSystemInfo.CheckContentLevel)
					{
						OwnHighestLevel = true;
					}
					if (OwnHighestLevel)
					{
						isChecked = true;
						checkedLevel = 0;
					}
					else
					{
						isChecked = false;
						checkedLevel = UserCheckLevel;
					}
				}

				try
				{
                    var translateType = ETranslateTypeUtils.GetEnumType(TranslateType.SelectedValue);

                    var nodeIDStrArrayList = ControlUtils.GetSelectedListControlValueArrayList(NodeIDFrom);

                    var nodeIDArrayList = new ArrayList();//需要转移的栏目ID
                    foreach (string nodeIDStr in nodeIDStrArrayList)
                    {
                        var nodeID = int.Parse(nodeIDStr);
                        if (translateType != ETranslateType.Content)//需要转移栏目
                        {
                            if (!NodeManager.IsAncestorOrSelf(PublishmentSystemId, nodeID, targetNodeID))
                            {
                                nodeIDArrayList.Add(nodeID);
                            }
                        }

                        if (translateType == ETranslateType.Content)//转移内容
                        {
                            TranslateContent(targetPublishmentSystemInfo, nodeID, targetNodeID, isChecked, checkedLevel);
                        }
                    }

                    if (translateType != ETranslateType.Content)//需要转移栏目
                    {
                        var nodeIDArrayListToTranslate = new ArrayList(nodeIDArrayList);
                        foreach (int nodeID in nodeIDArrayList)
                        {
                            var subNodeIDArrayList = DataProvider.NodeDao.GetNodeIdListForDescendant(nodeID);
                            if (subNodeIDArrayList != null && subNodeIDArrayList.Count > 0)
                            {
                                foreach (int nodeIDToDelete in subNodeIDArrayList)
                                {
                                    if (nodeIDArrayListToTranslate.Contains(nodeIDToDelete))
                                    {
                                        nodeIDArrayListToTranslate.Remove(nodeIDToDelete);
                                    }
                                }
                            }
                        }

                        var nodeInfoList = new List<NodeInfo>();
                        foreach (int nodeID in nodeIDArrayListToTranslate)
                        {
                            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeID);
                            nodeInfoList.Add(nodeInfo);
                        }

                        TranslateChannelAndContent(nodeInfoList, targetPublishmentSystemID, targetNodeID, translateType, isChecked, checkedLevel, null, null);

                        if (IsDeleteAfterTranslate.Visible && EBooleanUtils.Equals(IsDeleteAfterTranslate.SelectedValue, EBoolean.True))
                        {
                            foreach (int nodeID in nodeIDArrayListToTranslate)
                            {
                                try
                                {
                                    DataProvider.NodeDao.Delete(nodeID);
                                }
                                catch { }
                            }
                        }
                    }
                    Submit.Enabled = false;

                    var builder = new StringBuilder();
                    foreach (ListItem listItem in NodeIDFrom.Items)
                    {
                        if (listItem.Selected)
                        {
                            builder.Append(listItem.Text).Append(",");
                        }
                    }
                    if (builder.Length > 0)
                    {
                        builder.Length = builder.Length - 1;
                    }
                    Body.AddSiteLog(PublishmentSystemId, "批量转移", $"栏目:{builder},转移后删除:{IsDeleteAfterTranslate.SelectedValue}");

					SuccessMessage("批量转移成功！");
                    if (Body.IsQueryExists("ChannelIDCollection"))
                    {
                        PageUtils.Redirect(returnUrl);
                    }
                    else
                    {
                        PageUtils.Redirect(GetRedirectUrl(PublishmentSystemId));
                    }
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "批量转移失败！");
                    LogUtils.AddErrorLog(ex);
				}

			}
		}

		private void TranslateChannelAndContent(List<NodeInfo> nodeInfoList, int targetPublishmentSystemID, int parentID, ETranslateType translateType, bool isChecked, int checkedLevel, List<string> nodeIndexNameList, List<string> filePathList)
		{
			if (nodeInfoList == null || nodeInfoList.Count == 0)
			{
				return;
			}

			if (nodeIndexNameList == null)
			{
                nodeIndexNameList = DataProvider.NodeDao.GetNodeIndexNameList(targetPublishmentSystemID);
			}

            if (filePathList == null)
			{
                filePathList = DataProvider.NodeDao.GetAllFilePathByPublishmentSystemId(targetPublishmentSystemID);
			}

            var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemID);

			foreach (NodeInfo oldNodeInfo in nodeInfoList)
			{
                var nodeInfo = new NodeInfo(oldNodeInfo);
				nodeInfo.PublishmentSystemId = targetPublishmentSystemID;
				nodeInfo.ParentId = parentID;
				nodeInfo.ContentNum = 0;
				nodeInfo.ChildrenCount = 0;
                
				nodeInfo.AddDate = DateTime.Now;
                if (IsDeleteAfterTranslate.Visible && EBooleanUtils.Equals(IsDeleteAfterTranslate.SelectedValue, EBoolean.True))
                {
                    nodeIndexNameList.Add(nodeInfo.NodeIndexName);
                }
               
                else if (!string.IsNullOrEmpty(nodeInfo.NodeIndexName) && nodeIndexNameList.IndexOf(nodeInfo.NodeIndexName) == -1)
                {
                    nodeIndexNameList.Add(nodeInfo.NodeIndexName);
                }
                else
                {
                    nodeInfo.NodeIndexName = string.Empty;
                }

                if (!string.IsNullOrEmpty(nodeInfo.FilePath) && filePathList.IndexOf(nodeInfo.FilePath) == -1)
                {
                    filePathList.Add(nodeInfo.FilePath);
                }
                else
                {
                    nodeInfo.FilePath = string.Empty;
                }

                var insertedNodeID = DataProvider.NodeDao.InsertNodeInfo(nodeInfo);

                if (translateType == ETranslateType.All)
                {
                    TranslateContent(targetPublishmentSystemInfo, oldNodeInfo.NodeId, insertedNodeID, isChecked, checkedLevel);
                }

                if (insertedNodeID != 0)
                {
                    var orderByString = ETaxisTypeUtils.GetChannelOrderByString(ETaxisType.OrderByTaxis);
                    var childrenNodeInfoList = DataProvider.NodeDao.GetNodeInfoList(oldNodeInfo, 0, "", EScopeType.Children, orderByString);
                    if (childrenNodeInfoList != null && childrenNodeInfoList.Count > 0)
                    {
                        TranslateChannelAndContent(childrenNodeInfoList, targetPublishmentSystemID, insertedNodeID, translateType, isChecked, checkedLevel, nodeIndexNameList, filePathList);
                    }

                    if (isChecked)
                    {
                        CreateManager.CreateChannel(targetPublishmentSystemInfo.PublishmentSystemId, insertedNodeID);
                    }
                }
			}
		}

		private void TranslateContent(PublishmentSystemInfo targetPublishmentSystemInfo, int nodeID, int targetNodeID, bool isChecked, int checkedLevel)
		{
            var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, nodeID);
            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeID);

            var orderByString = ETaxisTypeUtils.GetOrderByString(tableStyle, ETaxisType.OrderByTaxis);

            var targetTableName = NodeManager.GetTableName(targetPublishmentSystemInfo, targetNodeID);
            var contentIdList = DataProvider.ContentDao.GetContentIdListChecked(tableName, nodeID, orderByString);

            foreach (var contentId in contentIdList)
			{
                var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);
                FileUtility.MoveFileByContentInfo(PublishmentSystemInfo, targetPublishmentSystemInfo, contentInfo);
                contentInfo.PublishmentSystemId = TranslateUtils.ToInt(PublishmentSystemIDDropDownList.SelectedValue);
                contentInfo.SourceId = contentInfo.NodeId;
				contentInfo.NodeId = targetNodeID;
				contentInfo.IsChecked = isChecked;
				contentInfo.CheckedLevel = checkedLevel;

                var theContentID = DataProvider.ContentDao.Insert(targetTableName, targetPublishmentSystemInfo, contentInfo);
				if (contentInfo.IsChecked)
				{
                    CreateManager.CreateContentAndTrigger(targetPublishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, theContentID);
				}
			}

			if (IsDeleteAfterTranslate.Visible && EBooleanUtils.Equals(IsDeleteAfterTranslate.SelectedValue, EBoolean.True))
			{
                try
                {
                    DataProvider.ContentDao.TrashContents(PublishmentSystemId, tableName, contentIdList, nodeID);
                }
                catch { }
			}
		}


		public void PublishmentSystemID_OnSelectedIndexChanged(object sender, EventArgs e)
		{
			var psID = int.Parse(PublishmentSystemIDDropDownList.SelectedValue);

			NodeIDTo.Items.Clear();

			var nodeIdList = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(psID);
            var nodeCount = nodeIdList.Count;
			isLastNodeArray = new bool[nodeCount];
            foreach (int theNodeID in nodeIdList)
			{
                var nodeInfo = NodeManager.GetNodeInfo(psID, theNodeID);
                var value = IsOwningNodeId(nodeInfo.NodeId) ? nodeInfo.NodeId.ToString() : "";
                value = (nodeInfo.Additional.IsContentAddable) ? value : "";
                var listitem = new ListItem(GetTitle(nodeInfo), value);
				NodeIDTo.Items.Add(listitem);
			}
		}

        public string ReturnUrl => returnUrl;
	}
}
