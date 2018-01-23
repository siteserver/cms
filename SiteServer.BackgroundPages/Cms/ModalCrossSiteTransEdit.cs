using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalCrossSiteTransEdit : BasePageCms
    {
        public DropDownList DdlTransType;
        public PlaceHolder PhSite;
        public DropDownList DdlSiteId;
        public ListBox LbNodeId;
        public PlaceHolder PhNodeNames;
        public TextBox TbNodeNames;
        public PlaceHolder PhIsAutomatic;
        public DropDownList DdlIsAutomatic;
        public DropDownList DdlTranslateDoneType;

        private ChannelInfo _nodeInfo;

        public static string GetOpenWindowString(int siteId, int nodeId)
        {
            return LayerUtils.GetOpenScript("跨站转发设置", PageUtils.GetCmsUrl(siteId, nameof(ModalCrossSiteTransEdit), new NameValueCollection
            {
                {"NodeID", nodeId.ToString()}
            }));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "NodeID");
            var nodeId = int.Parse(Body.GetQueryString("NodeID"));
            _nodeInfo = ChannelManager.GetChannelInfo(SiteId, nodeId);

            if (IsPostBack) return;

            ECrossSiteTransTypeUtils.AddAllListItems(DdlTransType, SiteInfo.ParentId > 0);

            ControlUtils.SelectSingleItem(DdlTransType, ECrossSiteTransTypeUtils.GetValue(_nodeInfo.Additional.TransType));

            DdlTransType_OnSelectedIndexChanged(null, EventArgs.Empty);
            ControlUtils.SelectSingleItem(DdlSiteId, _nodeInfo.Additional.TransSiteId.ToString());


            DdlSiteId_OnSelectedIndexChanged(null, EventArgs.Empty);
            ControlUtils.SelectMultiItems(LbNodeId, TranslateUtils.StringCollectionToStringList(_nodeInfo.Additional.TransChannelIds));
            TbNodeNames.Text = _nodeInfo.Additional.TransChannelNames;

            EBooleanUtils.AddListItems(DdlIsAutomatic, "自动", "提示");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsAutomatic, _nodeInfo.Additional.TransIsAutomatic.ToString());

            ETranslateContentTypeUtils.AddListItems(DdlTranslateDoneType, false);
            ControlUtils.SelectSingleItem(DdlTranslateDoneType, ETranslateContentTypeUtils.GetValue(_nodeInfo.Additional.TransDoneType));
        }

        protected void DdlTransType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            DdlSiteId.Items.Clear();
            DdlSiteId.Enabled = true;

            PhIsAutomatic.Visible = false;

            var contributeType = ECrossSiteTransTypeUtils.GetEnumType(DdlTransType.SelectedValue);
            if (contributeType == ECrossSiteTransType.None)
            {
                PhSite.Visible = PhNodeNames.Visible = false;
            }
            else if (contributeType == ECrossSiteTransType.SelfSite || contributeType == ECrossSiteTransType.SpecifiedSite)
            {
                PhSite.Visible = true;
                PhNodeNames.Visible = false;

                PhIsAutomatic.Visible = true;
            }
            else if (contributeType == ECrossSiteTransType.ParentSite)
            {
                PhSite.Visible = true;
                PhNodeNames.Visible = false;
                DdlSiteId.Enabled = false;

                PhIsAutomatic.Visible = true;
            }
            else if (contributeType == ECrossSiteTransType.AllParentSite || contributeType == ECrossSiteTransType.AllSite)
            {
                PhSite.Visible = false;
                PhNodeNames.Visible = true;
            }

            if (PhSite.Visible)
            {
                var siteIdList = SiteManager.GetSiteIdList();

                var allParentSiteIdList = new List<int>();
                if (contributeType == ECrossSiteTransType.AllParentSite)
                {
                    SiteManager.GetAllParentSiteIdList(allParentSiteIdList, siteIdList, SiteId);
                }
                else if (contributeType == ECrossSiteTransType.SelfSite)
                {
                    siteIdList = new List<int>
                    {
                        SiteId
                    };
                }

                foreach (var psId in siteIdList)
                {
                    var psInfo = SiteManager.GetSiteInfo(psId);
                    var show = false;
                    if (contributeType == ECrossSiteTransType.SpecifiedSite)
                    {
                        show = true;
                    }
                    else if (contributeType == ECrossSiteTransType.SelfSite)
                    {
                        if (psId == SiteId)
                        {
                            show = true;
                        }
                    }
                    else if (contributeType == ECrossSiteTransType.ParentSite)
                    {
                        if (psInfo.Id == SiteInfo.ParentId || (SiteInfo.ParentId == 0 && psInfo.IsRoot))
                        {
                            show = true;
                        }
                    }
                    if (!show) continue;

                    var listitem = new ListItem(psInfo.SiteName, psId.ToString());
                    if (psInfo.IsRoot) listitem.Selected = true;
                    DdlSiteId.Items.Add(listitem);
                }
            }
            DdlSiteId_OnSelectedIndexChanged(sender, e);
        }

        protected void DdlSiteId_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            LbNodeId.Items.Clear();
            if (PhSite.Visible && DdlSiteId.Items.Count > 0)
            {
                ChannelManager.AddListItemsForAddContent(LbNodeId.Items, SiteManager.GetSiteInfo(int.Parse(DdlSiteId.SelectedValue)), false, Body.AdminName);
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isSuccess = false;

            try
            {
                _nodeInfo.Additional.TransType = ECrossSiteTransTypeUtils.GetEnumType(DdlTransType.SelectedValue);
                _nodeInfo.Additional.TransSiteId = _nodeInfo.Additional.TransType == ECrossSiteTransType.SpecifiedSite ? TranslateUtils.ToInt(DdlSiteId.SelectedValue) : 0;
                _nodeInfo.Additional.TransChannelIds = ControlUtils.GetSelectedListControlValueCollection(LbNodeId);
                _nodeInfo.Additional.TransChannelNames = TbNodeNames.Text;

                _nodeInfo.Additional.TransIsAutomatic = TranslateUtils.ToBool(DdlIsAutomatic.SelectedValue);

                var translateDoneType = ETranslateContentTypeUtils.GetEnumType(DdlTranslateDoneType.SelectedValue);
                _nodeInfo.Additional.TransDoneType = translateDoneType;

                DataProvider.ChannelDao.Update(_nodeInfo);

                Body.AddSiteLog(SiteId, "修改跨站转发设置");

                isSuccess = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }

            if (isSuccess)
            {
                LayerUtils.CloseAndRedirect(Page, PageConfigurationCrossSiteTrans.GetRedirectUrl(SiteId, _nodeInfo.Id));
            }
        }
    }
}
