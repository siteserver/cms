using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
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

		private bool[] _isLastNodeArray;
		public string SystemKeyword;

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
            ReturnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));

            if (!HasChannelPermissions(PublishmentSystemId, AppManager.Permissions.Channel.ContentDelete))
			{
				IsDeleteAfterTranslate.Visible = false;
			}
			
			if (!IsPostBack)
			{
                BreadCrumb(AppManager.Cms.LeftMenu.IdContent, "批量转移", string.Empty);

                phReturn.Visible = !string.IsNullOrEmpty(ReturnUrl);
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

                var publishmentSystemIdList = ProductPermissionsManager.Current.PublishmentSystemIdList;
                foreach (var psId in publishmentSystemIdList)
				{
					var psInfo = PublishmentSystemManager.GetPublishmentSystemInfo(psId);
                    var listitem = new ListItem(psInfo.PublishmentSystemName, psId.ToString());
                    if (psId == PublishmentSystemId) listitem.Selected = true;
                    PublishmentSystemIDDropDownList.Items.Add(listitem);
				}

                var nodeIdStrArrayList = new List<string>();
                if (Body.IsQueryExists("ChannelIDCollection"))
                {
                    nodeIdStrArrayList = TranslateUtils.StringCollectionToStringList(Body.GetQueryString("ChannelIDCollection"));
                }

				var nodeIdList = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(PublishmentSystemId);
                var nodeCount = nodeIdList.Count;
				_isLastNodeArray = new bool[nodeCount];
                foreach (var theNodeId in nodeIdList)
				{
                    var enabled = IsOwningNodeId(theNodeId);
                    if (!enabled)
                    {
                        if (!IsHasChildOwningNodeId(theNodeId)) continue;
                    }
                    var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, theNodeId);

                    var value = enabled ? nodeInfo.NodeId.ToString() : string.Empty;
                    value = (nodeInfo.Additional.IsContentAddable) ? value : string.Empty;

                    var text = GetTitle(nodeInfo);
					var listItem = new ListItem(text, value);
                    if (nodeIdStrArrayList.Contains(value))
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
                _isLastNodeArray[nodeInfo.ParentsCount] = false;
			}
			else
			{
                _isLastNodeArray[nodeInfo.ParentsCount] = true;
			}
            for (var i = 0; i < nodeInfo.ParentsCount; i++)
			{
				if (_isLastNodeArray[i])
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
				var targetNodeId = int.Parse(NodeIDTo.SelectedValue);

				var targetPublishmentSystemId = int.Parse(PublishmentSystemIDDropDownList.SelectedValue);
				var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemId);
				bool isChecked;
				int checkedLevel;
                if (targetPublishmentSystemInfo.CheckContentLevel == 0 || AdminUtility.HasChannelPermissions(Body.AdministratorName, targetPublishmentSystemId, targetNodeId, AppManager.Permissions.Channel.ContentAdd, AppManager.Permissions.Channel.ContentCheck))
				{
					isChecked = true;
					checkedLevel = 0;
				}
				else
				{
					var userCheckLevel = 0;
					var ownHighestLevel = false;

                    if (AdminUtility.HasChannelPermissions(Body.AdministratorName, targetPublishmentSystemId, targetNodeId, AppManager.Permissions.Channel.ContentCheckLevel1))
                    {
                        userCheckLevel = 1;
                        if (AdminUtility.HasChannelPermissions(Body.AdministratorName, targetPublishmentSystemId, targetNodeId, AppManager.Permissions.Channel.ContentCheckLevel2))
                        {
                            userCheckLevel = 2;
                            if (AdminUtility.HasChannelPermissions(Body.AdministratorName, targetPublishmentSystemId, targetNodeId, AppManager.Permissions.Channel.ContentCheckLevel3))
                            {
                                userCheckLevel = 3;
                                if (AdminUtility.HasChannelPermissions(Body.AdministratorName, targetPublishmentSystemId, targetNodeId, AppManager.Permissions.Channel.ContentCheckLevel4))
                                {
                                    userCheckLevel = 4;
                                    if (AdminUtility.HasChannelPermissions(Body.AdministratorName, targetPublishmentSystemId, targetNodeId, AppManager.Permissions.Channel.ContentCheckLevel5))
                                    {
                                        userCheckLevel = 5;
                                    }
                                }
                            }
                        }
                    }

                    if (userCheckLevel >= targetPublishmentSystemInfo.CheckContentLevel)
					{
						ownHighestLevel = true;
					}
					if (ownHighestLevel)
					{
						isChecked = true;
						checkedLevel = 0;
					}
					else
					{
						isChecked = false;
						checkedLevel = userCheckLevel;
					}
				}

				try
				{
                    var translateType = ETranslateTypeUtils.GetEnumType(TranslateType.SelectedValue);

                    var nodeIdStrArrayList = ControlUtils.GetSelectedListControlValueArrayList(NodeIDFrom);

                    var nodeIdArrayList = new ArrayList();//需要转移的栏目ID
                    foreach (string nodeIdStr in nodeIdStrArrayList)
                    {
                        var nodeId = int.Parse(nodeIdStr);
                        if (translateType != ETranslateType.Content)//需要转移栏目
                        {
                            if (!NodeManager.IsAncestorOrSelf(PublishmentSystemId, nodeId, targetNodeId))
                            {
                                nodeIdArrayList.Add(nodeId);
                            }
                        }

                        if (translateType == ETranslateType.Content)//转移内容
                        {
                            TranslateContent(targetPublishmentSystemInfo, nodeId, targetNodeId, isChecked, checkedLevel);
                        }
                    }

                    if (translateType != ETranslateType.Content)//需要转移栏目
                    {
                        var nodeIdArrayListToTranslate = new ArrayList(nodeIdArrayList);
                        foreach (int nodeId in nodeIdArrayList)
                        {
                            var subNodeIdArrayList = DataProvider.NodeDao.GetNodeIdListForDescendant(nodeId);
                            if (subNodeIdArrayList != null && subNodeIdArrayList.Count > 0)
                            {
                                foreach (int nodeIdToDelete in subNodeIdArrayList)
                                {
                                    if (nodeIdArrayListToTranslate.Contains(nodeIdToDelete))
                                    {
                                        nodeIdArrayListToTranslate.Remove(nodeIdToDelete);
                                    }
                                }
                            }
                        }

                        var nodeInfoList = new List<NodeInfo>();
                        foreach (int nodeId in nodeIdArrayListToTranslate)
                        {
                            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);
                            nodeInfoList.Add(nodeInfo);
                        }

                        TranslateChannelAndContent(nodeInfoList, targetPublishmentSystemId, targetNodeId, translateType, isChecked, checkedLevel, null, null);

                        if (IsDeleteAfterTranslate.Visible && EBooleanUtils.Equals(IsDeleteAfterTranslate.SelectedValue, EBoolean.True))
                        {
                            foreach (int nodeId in nodeIdArrayListToTranslate)
                            {
                                try
                                {
                                    DataProvider.NodeDao.Delete(nodeId);
                                }
                                catch
                                {
                                    // ignored
                                }
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
                        PageUtils.Redirect(ReturnUrl);
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

		private void TranslateChannelAndContent(List<NodeInfo> nodeInfoList, int targetPublishmentSystemId, int parentId, ETranslateType translateType, bool isChecked, int checkedLevel, List<string> nodeIndexNameList, List<string> filePathList)
		{
			if (nodeInfoList == null || nodeInfoList.Count == 0)
			{
				return;
			}

			if (nodeIndexNameList == null)
			{
                nodeIndexNameList = DataProvider.NodeDao.GetNodeIndexNameList(targetPublishmentSystemId);
			}

            if (filePathList == null)
			{
                filePathList = DataProvider.NodeDao.GetAllFilePathByPublishmentSystemId(targetPublishmentSystemId);
			}

            var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemId);

			foreach (var oldNodeInfo in nodeInfoList)
			{
			    var nodeInfo = new NodeInfo(oldNodeInfo)
			    {
			        PublishmentSystemId = targetPublishmentSystemId,
			        ParentId = parentId,
			        ContentNum = 0,
			        ChildrenCount = 0,
			        AddDate = DateTime.Now
			    };

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

                var insertedNodeId = DataProvider.NodeDao.InsertNodeInfo(nodeInfo);

                if (translateType == ETranslateType.All)
                {
                    TranslateContent(targetPublishmentSystemInfo, oldNodeInfo.NodeId, insertedNodeId, isChecked, checkedLevel);
                }

                if (insertedNodeId != 0)
                {
                    var orderByString = ETaxisTypeUtils.GetChannelOrderByString(ETaxisType.OrderByTaxis);
                    var childrenNodeInfoList = DataProvider.NodeDao.GetNodeInfoList(oldNodeInfo.NodeId, oldNodeInfo.ChildrenCount, 0, "", EScopeType.Children, orderByString);
                    if (childrenNodeInfoList != null && childrenNodeInfoList.Count > 0)
                    {
                        TranslateChannelAndContent(childrenNodeInfoList, targetPublishmentSystemId, insertedNodeId, translateType, isChecked, checkedLevel, nodeIndexNameList, filePathList);
                    }

                    if (isChecked)
                    {
                        CreateManager.CreateChannel(targetPublishmentSystemInfo.PublishmentSystemId, insertedNodeId);
                    }
                }
			}
		}

		private void TranslateContent(PublishmentSystemInfo targetPublishmentSystemInfo, int nodeId, int targetNodeId, bool isChecked, int checkedLevel)
		{
            var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, nodeId);
            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeId);

            var orderByString = ETaxisTypeUtils.GetOrderByString(tableStyle, ETaxisType.OrderByTaxis);

            var targetTableName = NodeManager.GetTableName(targetPublishmentSystemInfo, targetNodeId);
            var contentIdList = DataProvider.ContentDao.GetContentIdListChecked(tableName, nodeId, orderByString);

            foreach (var contentId in contentIdList)
			{
                var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);
                FileUtility.MoveFileByContentInfo(PublishmentSystemInfo, targetPublishmentSystemInfo, contentInfo);
                contentInfo.PublishmentSystemId = TranslateUtils.ToInt(PublishmentSystemIDDropDownList.SelectedValue);
                contentInfo.SourceId = contentInfo.NodeId;
				contentInfo.NodeId = targetNodeId;
				contentInfo.IsChecked = isChecked;
				contentInfo.CheckedLevel = checkedLevel;

                var theContentId = DataProvider.ContentDao.Insert(targetTableName, targetPublishmentSystemInfo, contentInfo);
				if (contentInfo.IsChecked)
				{
                    CreateManager.CreateContentAndTrigger(targetPublishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, theContentId);
				}
			}

			if (IsDeleteAfterTranslate.Visible && EBooleanUtils.Equals(IsDeleteAfterTranslate.SelectedValue, EBoolean.True))
			{
                try
                {
                    DataProvider.ContentDao.TrashContents(PublishmentSystemId, tableName, contentIdList, nodeId);
                }
			    catch
			    {
			        // ignored
			    }
			}
		}


		public void PublishmentSystemID_OnSelectedIndexChanged(object sender, EventArgs e)
		{
			var psId = int.Parse(PublishmentSystemIDDropDownList.SelectedValue);

			NodeIDTo.Items.Clear();

			var nodeIdList = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(psId);
            var nodeCount = nodeIdList.Count;
			_isLastNodeArray = new bool[nodeCount];
            foreach (var theNodeId in nodeIdList)
			{
                var nodeInfo = NodeManager.GetNodeInfo(psId, theNodeId);
                var value = IsOwningNodeId(nodeInfo.NodeId) ? nodeInfo.NodeId.ToString() : "";
                value = (nodeInfo.Additional.IsContentAddable) ? value : "";
                var listitem = new ListItem(GetTitle(nodeInfo), value);
				NodeIDTo.Items.Add(listitem);
			}
		}

        public string ReturnUrl { get; private set; }
    }
}
