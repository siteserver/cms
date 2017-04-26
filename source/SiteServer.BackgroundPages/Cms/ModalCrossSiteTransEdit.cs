using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalCrossSiteTransEdit : BasePageCms
    {
        protected DropDownList TransType;

        protected PlaceHolder PlaceHolder_PublishmentSystem;
        protected DropDownList PublishmentSystemIDCollection;
        protected ListBox NodeIDCollection;

        protected PlaceHolder PlaceHolder_NodeNames;
        protected TextBox NodeNames;

        protected PlaceHolder PlaceHolder_IsAutomatic;
        protected RadioButtonList IsAutomatic;

        private NodeInfo nodeInfo;

        protected RadioButtonList ddlTranslateDoneType;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeID)
        {
            return PageUtils.GetOpenWindowString("跨站转发设置", PageUtils.GetCmsUrl(nameof(ModalCrossSiteTransEdit), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeID.ToString()}
            }), 650, 500);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "NodeID");
            var nodeID = int.Parse(Body.GetQueryString("NodeID"));
            nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeID);

            if (!IsPostBack)
            {
                ECrossSiteTransTypeUtils.AddAllListItems(TransType);

                ControlUtils.SelectListItems(TransType, ECrossSiteTransTypeUtils.GetValue(nodeInfo.Additional.TransType));

                TransType_OnSelectedIndexChanged(null, EventArgs.Empty);
                ControlUtils.SelectListItems(PublishmentSystemIDCollection, nodeInfo.Additional.TransPublishmentSystemID.ToString());


                PublishmentSystemIDCollection_OnSelectedIndexChanged(null, EventArgs.Empty);
                ControlUtils.SelectListItems(NodeIDCollection, TranslateUtils.StringCollectionToStringList(nodeInfo.Additional.TransNodeIDs));
                NodeNames.Text = nodeInfo.Additional.TransNodeNames;

                EBooleanUtils.AddListItems(IsAutomatic, "自动", "提示");
                ControlUtils.SelectListItemsIgnoreCase(IsAutomatic, nodeInfo.Additional.TransIsAutomatic.ToString());

                ETranslateContentTypeUtils.AddListItems(ddlTranslateDoneType, false);
                ControlUtils.SelectListItems(ddlTranslateDoneType, ETranslateContentTypeUtils.GetValue(nodeInfo.Additional.TransDoneType));
            }
        }

        protected void TransType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PublishmentSystemIDCollection.Items.Clear();
            PublishmentSystemIDCollection.Enabled = true;

            PlaceHolder_IsAutomatic.Visible = false;

            var contributeType = ECrossSiteTransTypeUtils.GetEnumType(TransType.SelectedValue);
            if (contributeType == ECrossSiteTransType.None)
            {
                PlaceHolder_PublishmentSystem.Visible = PlaceHolder_NodeNames.Visible = false;
            }
            else if (contributeType == ECrossSiteTransType.SelfSite || contributeType == ECrossSiteTransType.SpecifiedSite)
            {
                PlaceHolder_PublishmentSystem.Visible = true;
                PlaceHolder_NodeNames.Visible = false;

                PlaceHolder_IsAutomatic.Visible = true;
            }
            else if (contributeType == ECrossSiteTransType.ParentSite)
            {
                PlaceHolder_PublishmentSystem.Visible = true;
                PlaceHolder_NodeNames.Visible = false;
                PublishmentSystemIDCollection.Enabled = false;

                PlaceHolder_IsAutomatic.Visible = true;
            }
            else if (contributeType == ECrossSiteTransType.AllParentSite || contributeType == ECrossSiteTransType.AllSite)
            {
                PlaceHolder_PublishmentSystem.Visible = false;
                PlaceHolder_NodeNames.Visible = true;
            }

            if (PlaceHolder_PublishmentSystem.Visible)
            {
                var publishmentSystemIDArrayList = PublishmentSystemManager.GetPublishmentSystemIdList();

                var allParentPublishmentSystemIDArrayList = new List<int>();
                if (contributeType == ECrossSiteTransType.AllParentSite)
                {
                    PublishmentSystemManager.GetAllParentPublishmentSystemIdList(allParentPublishmentSystemIDArrayList, publishmentSystemIDArrayList, PublishmentSystemId);
                }
                else if (contributeType == ECrossSiteTransType.SelfSite)
                {
                    publishmentSystemIDArrayList = new List<int>();
                    publishmentSystemIDArrayList.Add(PublishmentSystemId);
                }

                foreach (int psID in publishmentSystemIDArrayList)
                {
                    var psInfo = PublishmentSystemManager.GetPublishmentSystemInfo(psID);
                    var show = false;
                    if (contributeType == ECrossSiteTransType.SpecifiedSite)
                    {
                        show = true;
                    }
                    else if (contributeType == ECrossSiteTransType.SelfSite)
                    {
                        if (psID == PublishmentSystemId)
                        {
                            show = true;
                        }
                    }
                    else if (contributeType == ECrossSiteTransType.ParentSite)
                    {
                        if (psInfo.PublishmentSystemId == PublishmentSystemInfo.ParentPublishmentSystemId || (PublishmentSystemInfo.ParentPublishmentSystemId == 0 && psInfo.IsHeadquarters))
                        {
                            show = true;
                        }
                    }
                    if (show)
                    {
                        var listitem = new ListItem(psInfo.PublishmentSystemName, psID.ToString());
                        if (psInfo.IsHeadquarters) listitem.Selected = true;
                        PublishmentSystemIDCollection.Items.Add(listitem);
                    }
                }
            }
            PublishmentSystemIDCollection_OnSelectedIndexChanged(sender, e);
        }

        protected void PublishmentSystemIDCollection_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            NodeIDCollection.Items.Clear();
            if (PlaceHolder_PublishmentSystem.Visible && PublishmentSystemIDCollection.Items.Count > 0)
            {
                NodeManager.AddListItemsForAddContent(NodeIDCollection.Items, PublishmentSystemManager.GetPublishmentSystemInfo(int.Parse(PublishmentSystemIDCollection.SelectedValue)), false, Body.AdministratorName);
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isSuccess = false;

            try
            {
                nodeInfo.Additional.TransType = ECrossSiteTransTypeUtils.GetEnumType(TransType.SelectedValue);
                if (nodeInfo.Additional.TransType == ECrossSiteTransType.SpecifiedSite)
                {
                    nodeInfo.Additional.TransPublishmentSystemID = TranslateUtils.ToInt(PublishmentSystemIDCollection.SelectedValue);
                }
                else
                {
                    nodeInfo.Additional.TransPublishmentSystemID = 0;
                }
                nodeInfo.Additional.TransNodeIDs = ControlUtils.GetSelectedListControlValueCollection(NodeIDCollection);
                nodeInfo.Additional.TransNodeNames = NodeNames.Text;

                nodeInfo.Additional.TransIsAutomatic = TranslateUtils.ToBool(IsAutomatic.SelectedValue);


                var translateDoneType = ETranslateContentTypeUtils.GetEnumType(ddlTranslateDoneType.SelectedValue);
                nodeInfo.Additional.TransDoneType = translateDoneType;

                DataProvider.NodeDao.UpdateNodeInfo(nodeInfo);

                Body.AddSiteLog(PublishmentSystemId, "修改跨站转发设置");

                isSuccess = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }

            if (isSuccess)
            {
                PageUtils.CloseModalPageAndRedirect(Page, PageConfigurationCrossSiteTrans.GetRedirectUrl(PublishmentSystemId, nodeInfo.NodeId));
            }
        }
    }
}
