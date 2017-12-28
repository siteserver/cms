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
        public DropDownList DdlTransType;
        public PlaceHolder PhPublishmentSystem;
        public DropDownList DdlPublishmentSystemId;
        public ListBox LbNodeId;
        public PlaceHolder PhNodeNames;
        public TextBox TbNodeNames;
        public PlaceHolder PhIsAutomatic;
        public DropDownList DdlIsAutomatic;
        public DropDownList DdlTranslateDoneType;

        private NodeInfo _nodeInfo;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId)
        {
            return LayerUtils.GetOpenScript("跨站转发设置", PageUtils.GetCmsUrl(nameof(ModalCrossSiteTransEdit), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()}
            }));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "NodeID");
            var nodeId = int.Parse(Body.GetQueryString("NodeID"));
            _nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);

            if (IsPostBack) return;

            ECrossSiteTransTypeUtils.AddAllListItems(DdlTransType, PublishmentSystemInfo.ParentPublishmentSystemId > 0);

            ControlUtils.SelectSingleItem(DdlTransType, ECrossSiteTransTypeUtils.GetValue(_nodeInfo.Additional.TransType));

            DdlTransType_OnSelectedIndexChanged(null, EventArgs.Empty);
            ControlUtils.SelectSingleItem(DdlPublishmentSystemId, _nodeInfo.Additional.TransPublishmentSystemId.ToString());


            DdlPublishmentSystemId_OnSelectedIndexChanged(null, EventArgs.Empty);
            ControlUtils.SelectMultiItems(LbNodeId, TranslateUtils.StringCollectionToStringList(_nodeInfo.Additional.TransNodeIds));
            TbNodeNames.Text = _nodeInfo.Additional.TransNodeNames;

            EBooleanUtils.AddListItems(DdlIsAutomatic, "自动", "提示");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsAutomatic, _nodeInfo.Additional.TransIsAutomatic.ToString());

            ETranslateContentTypeUtils.AddListItems(DdlTranslateDoneType, false);
            ControlUtils.SelectSingleItem(DdlTranslateDoneType, ETranslateContentTypeUtils.GetValue(_nodeInfo.Additional.TransDoneType));
        }

        protected void DdlTransType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            DdlPublishmentSystemId.Items.Clear();
            DdlPublishmentSystemId.Enabled = true;

            PhIsAutomatic.Visible = false;

            var contributeType = ECrossSiteTransTypeUtils.GetEnumType(DdlTransType.SelectedValue);
            if (contributeType == ECrossSiteTransType.None)
            {
                PhPublishmentSystem.Visible = PhNodeNames.Visible = false;
            }
            else if (contributeType == ECrossSiteTransType.SelfSite || contributeType == ECrossSiteTransType.SpecifiedSite)
            {
                PhPublishmentSystem.Visible = true;
                PhNodeNames.Visible = false;

                PhIsAutomatic.Visible = true;
            }
            else if (contributeType == ECrossSiteTransType.ParentSite)
            {
                PhPublishmentSystem.Visible = true;
                PhNodeNames.Visible = false;
                DdlPublishmentSystemId.Enabled = false;

                PhIsAutomatic.Visible = true;
            }
            else if (contributeType == ECrossSiteTransType.AllParentSite || contributeType == ECrossSiteTransType.AllSite)
            {
                PhPublishmentSystem.Visible = false;
                PhNodeNames.Visible = true;
            }

            if (PhPublishmentSystem.Visible)
            {
                var publishmentSystemIdList = PublishmentSystemManager.GetPublishmentSystemIdList();

                var allParentPublishmentSystemIdList = new List<int>();
                if (contributeType == ECrossSiteTransType.AllParentSite)
                {
                    PublishmentSystemManager.GetAllParentPublishmentSystemIdList(allParentPublishmentSystemIdList, publishmentSystemIdList, PublishmentSystemId);
                }
                else if (contributeType == ECrossSiteTransType.SelfSite)
                {
                    publishmentSystemIdList = new List<int>
                    {
                        PublishmentSystemId
                    };
                }

                foreach (var psId in publishmentSystemIdList)
                {
                    var psInfo = PublishmentSystemManager.GetPublishmentSystemInfo(psId);
                    var show = false;
                    if (contributeType == ECrossSiteTransType.SpecifiedSite)
                    {
                        show = true;
                    }
                    else if (contributeType == ECrossSiteTransType.SelfSite)
                    {
                        if (psId == PublishmentSystemId)
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
                    if (!show) continue;

                    var listitem = new ListItem(psInfo.PublishmentSystemName, psId.ToString());
                    if (psInfo.IsHeadquarters) listitem.Selected = true;
                    DdlPublishmentSystemId.Items.Add(listitem);
                }
            }
            DdlPublishmentSystemId_OnSelectedIndexChanged(sender, e);
        }

        protected void DdlPublishmentSystemId_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            LbNodeId.Items.Clear();
            if (PhPublishmentSystem.Visible && DdlPublishmentSystemId.Items.Count > 0)
            {
                NodeManager.AddListItemsForAddContent(LbNodeId.Items, PublishmentSystemManager.GetPublishmentSystemInfo(int.Parse(DdlPublishmentSystemId.SelectedValue)), false, Body.AdminName);
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isSuccess = false;

            try
            {
                _nodeInfo.Additional.TransType = ECrossSiteTransTypeUtils.GetEnumType(DdlTransType.SelectedValue);
                _nodeInfo.Additional.TransPublishmentSystemId = _nodeInfo.Additional.TransType == ECrossSiteTransType.SpecifiedSite ? TranslateUtils.ToInt(DdlPublishmentSystemId.SelectedValue) : 0;
                _nodeInfo.Additional.TransNodeIds = ControlUtils.GetSelectedListControlValueCollection(LbNodeId);
                _nodeInfo.Additional.TransNodeNames = TbNodeNames.Text;

                _nodeInfo.Additional.TransIsAutomatic = TranslateUtils.ToBool(DdlIsAutomatic.SelectedValue);


                var translateDoneType = ETranslateContentTypeUtils.GetEnumType(DdlTranslateDoneType.SelectedValue);
                _nodeInfo.Additional.TransDoneType = translateDoneType;

                DataProvider.NodeDao.UpdateNodeInfo(_nodeInfo);

                Body.AddSiteLog(PublishmentSystemId, "修改跨站转发设置");

                isSuccess = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }

            if (isSuccess)
            {
                LayerUtils.CloseAndRedirect(Page, PageConfigurationCrossSiteTrans.GetRedirectUrl(PublishmentSystemId, _nodeInfo.NodeId));
            }
        }
    }
}
