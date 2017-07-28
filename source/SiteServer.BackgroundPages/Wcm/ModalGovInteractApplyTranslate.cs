using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.GovInteract;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Wcm
{
    public class ModalGovInteractApplyTranslate : BasePageCms
	{
        public DropDownList ddlNodeID;
        protected TextBox tbTranslateRemark;
        public Literal ltlDepartmentName;
        public Literal ltlUserName;

        private int _nodeId;
        private List<int> _idArrayList;

	    public static string GetOpenWindowString(int publishmentSystemId, int nodeId)
	    {
	        return PageUtils.GetOpenWindowStringWithCheckBoxValue("转移办件",
	            PageUtils.GetWcmUrl(nameof(ModalGovInteractApplyTranslate), new NameValueCollection
	            {
	                {"PublishmentSystemID", publishmentSystemId.ToString()},
	                {"NodeID", nodeId.ToString()}
	            }), "IDCollection", "请选择需要转移的办件！", 580, 400);
	    }

	    public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _nodeId = TranslateUtils.ToInt(Request.QueryString["NodeID"]);
            _idArrayList = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);

            if (!IsPostBack)
            {
                var nodeInfoList = GovInteractManager.GetNodeInfoList(PublishmentSystemInfo);
                foreach (var nodeInfo in nodeInfoList)
                {
                    if (nodeInfo.NodeId != _nodeId)
                    {
                        var listItem = new ListItem(nodeInfo.NodeName, nodeInfo.NodeId.ToString());
                        ddlNodeID.Items.Add(listItem);
                    }
                }

                ltlDepartmentName.Text = DepartmentManager.GetDepartmentName(Body.AdministratorInfo.DepartmentId);
                ltlUserName.Text = Body.AdministratorInfo.DisplayName;
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {
                var translateNodeID = TranslateUtils.ToInt(ddlNodeID.SelectedValue);
                if (translateNodeID == 0)
                {
                    FailMessage("转移失败，必须选择转移目标");
                    return;
                }

                foreach (int contentID in _idArrayList)
                {
                    var contentInfo = DataProvider.GovInteractContentDao.GetContentInfo(PublishmentSystemInfo, contentID);
                    contentInfo.SetExtendedAttribute(GovInteractContentAttribute.TranslateFromNodeId, contentInfo.NodeId.ToString());
                    contentInfo.NodeId = translateNodeID;
                    
                    DataProvider.ContentDao.Update(PublishmentSystemInfo.AuxiliaryTableForGovInteract, PublishmentSystemInfo, contentInfo);

                    if (!string.IsNullOrEmpty(tbTranslateRemark.Text))
                    {
                        var remarkInfo = new GovInteractRemarkInfo(0, PublishmentSystemId, contentInfo.NodeId, contentID, EGovInteractRemarkType.Translate, tbTranslateRemark.Text, Body.AdministratorInfo.DepartmentId, Body.AdministratorName, DateTime.Now);
                        DataProvider.GovInteractRemarkDao.Insert(remarkInfo);
                    }

                    GovInteractApplyManager.LogTranslate(PublishmentSystemId, contentInfo.NodeId, contentID, NodeManager.GetNodeName(PublishmentSystemId, _nodeId), Body.AdministratorName, Body.AdministratorInfo.DepartmentId);
                }

                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
                isChanged = false;
            }

            if (isChanged)
            {
                PageUtils.CloseModalPage(Page, "alert(\'办件转移成功!\');");
            }
        }
	}
}
