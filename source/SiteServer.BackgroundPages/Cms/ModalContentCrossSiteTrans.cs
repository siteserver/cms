using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalContentCrossSiteTrans : BasePageCms
    {
	    protected DropDownList PublishmentSystemIDDropDownList;
        protected ListBox NodeIDListBox;

        private int _nodeId;
	    private List<int> _contentIdArrayList;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetOpenWindowStringWithCheckBoxValue("转发所选内容", PageUtils.GetCmsUrl(nameof(ModalContentCrossSiteTrans), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()}
            }), "ContentIDCollection", "请选择需要转发的内容！", 400, 390);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "NodeID", "ContentIDCollection");

            _nodeId = Body.GetQueryInt("NodeID");
            _contentIdArrayList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ContentIDCollection"));

			if (!IsPostBack)
			{
                CrossSiteTransUtility.LoadPublishmentSystemIDDropDownList(PublishmentSystemIDDropDownList, PublishmentSystemInfo, _nodeId);

                if (PublishmentSystemIDDropDownList.Items.Count > 0)
                {
                    PublishmentSystemID_SelectedIndexChanged(null, EventArgs.Empty);
                }

				
			}
		}

        public void PublishmentSystemID_SelectedIndexChanged(object sender, EventArgs e)
        {
            var psID = int.Parse(PublishmentSystemIDDropDownList.SelectedValue);
            CrossSiteTransUtility.LoadNodeIDListBox(NodeIDListBox, PublishmentSystemInfo, psID, NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId), Body.AdministratorName);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var targetPublishmentSystemID = int.Parse(PublishmentSystemIDDropDownList.SelectedValue);
            var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemID);
            try
            {
                foreach (ListItem listItem in NodeIDListBox.Items)
                {
                    if (listItem.Selected)
                    {
                        var targetNodeID = TranslateUtils.ToInt(listItem.Value);
                        if (targetNodeID != 0)
                        {
                            var targetTableStyle = NodeManager.GetTableStyle(targetPublishmentSystemInfo, targetNodeID);
                            var targetTableName = NodeManager.GetTableName(targetPublishmentSystemInfo, targetNodeID);
                            var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, _nodeId);
                            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeId);
                            foreach (int contentID in _contentIdArrayList)
                            {
                                var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentID);
                                FileUtility.MoveFileByContentInfo(PublishmentSystemInfo, targetPublishmentSystemInfo, contentInfo as BackgroundContentInfo);
                                contentInfo.PublishmentSystemId = targetPublishmentSystemID;
                                contentInfo.SourceId = contentInfo.NodeId;
                                contentInfo.NodeId = targetNodeID;
                                
                                if (targetPublishmentSystemInfo.Additional.IsCrossSiteTransChecked)
                                {
                                    contentInfo.IsChecked = true;
                                }
                                else
                                {
                                    contentInfo.IsChecked = false;
                                }
                                contentInfo.CheckedLevel = 0;

                                DataProvider.ContentDao.Insert(targetTableName, targetPublishmentSystemInfo, contentInfo);
                            }
                        }
                    }
                }

                Body.AddSiteLog(PublishmentSystemId, _nodeId, 0, "跨站转发内容", string.Empty);

                SuccessMessage("内容转发成功，请选择后续操作。");
            }
            catch (Exception ex)
            {
                FailMessage(ex, "内容转发失败！");
            }
            
            PageUtils.CloseModalPage(Page);
		}

	}
}
