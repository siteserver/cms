using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using Datory;
using SiteServer.CMS.Context;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Repositories;
using SiteServer.Abstractions;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalCrossSiteTransEdit : BasePageCms
    {
        public DropDownList DdlTransType;
        public PlaceHolder PhSite;
        public DropDownList DdlSiteId;
        public ListBox LbChannelId;
        public PlaceHolder PhNodeNames;
        public TextBox TbNodeNames;
        public PlaceHolder PhIsAutomatic;
        public DropDownList DdlIsAutomatic;
        public DropDownList DdlTranslateDoneType;

        private Channel _channel;

        public static string GetOpenWindowString(int siteId, int channelId)
        {
            return LayerUtils.GetOpenScript("跨站转发设置", PageUtils.GetCmsUrl(siteId, nameof(ModalCrossSiteTransEdit), new NameValueCollection
            {
                {"channelId", channelId.ToString()}
            }));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "channelId");
            var channelId = int.Parse(AuthRequest.GetQueryString("channelId"));
            _channel = ChannelManager.GetChannelAsync(SiteId, channelId).GetAwaiter().GetResult();

            if (IsPostBack) return;

            ECrossSiteTransTypeUtilsExtensions.AddAllListItems(DdlTransType, Site.ParentId > 0);

            ControlUtils.SelectSingleItem(DdlTransType, _channel.TransType.GetValue());

            DdlTransType_OnSelectedIndexChanged(null, EventArgs.Empty);
            ControlUtils.SelectSingleItem(DdlSiteId, _channel.TransSiteId.ToString());


            DdlSiteId_OnSelectedIndexChanged(null, EventArgs.Empty);
            ControlUtils.SelectMultiItems(LbChannelId, StringUtils.GetStringList(_channel.TransChannelIds));
            TbNodeNames.Text = _channel.TransChannelNames;

            EBooleanUtils.AddListItems(DdlIsAutomatic, "系统自动转发", "需手动操作");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsAutomatic, _channel.TransIsAutomatic.ToString());

            ETranslateContentTypeUtilsExtensions.AddListItems(DdlTranslateDoneType, false);
            ControlUtils.SelectSingleItem(DdlTranslateDoneType, _channel.TransDoneType.GetValue());
        }

        protected void DdlTransType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            DdlSiteId.Items.Clear();
            DdlSiteId.Enabled = true;

            PhIsAutomatic.Visible = false;

            var contributeType = TranslateUtils.ToEnum(DdlTransType.SelectedValue, TransType.None);
            if (contributeType == TransType.None)
            {
                PhSite.Visible = PhNodeNames.Visible = false;
            }
            else if (contributeType == TransType.SelfSite || contributeType == TransType.SpecifiedSite)
            {
                PhSite.Visible = true;
                PhNodeNames.Visible = false;

                PhIsAutomatic.Visible = true;
            }
            else if (contributeType == TransType.ParentSite)
            {
                PhSite.Visible = true;
                PhNodeNames.Visible = false;
                DdlSiteId.Enabled = false;

                PhIsAutomatic.Visible = true;
            }
            else if (contributeType == TransType.AllParentSite || contributeType == TransType.AllSite)
            {
                PhSite.Visible = false;
                PhNodeNames.Visible = true;
            }

            if (PhSite.Visible)
            {
                var siteIdList = DataProvider.SiteRepository.GetSiteIdListAsync().GetAwaiter().GetResult();

                var allParentSiteIdList = new List<int>();
                if (contributeType == TransType.AllParentSite)
                {
                    DataProvider.SiteRepository.GetAllParentSiteIdListAsync(allParentSiteIdList, siteIdList, SiteId).GetAwaiter().GetResult();
                }
                else if (contributeType == TransType.SelfSite)
                {
                    siteIdList = new List<int>
                    {
                        SiteId
                    };
                }

                foreach (var psId in siteIdList)
                {
                    var psInfo = DataProvider.SiteRepository.GetAsync(psId).GetAwaiter().GetResult();
                    var show = false;
                    if (contributeType == TransType.SpecifiedSite)
                    {
                        show = true;
                    }
                    else if (contributeType == TransType.SelfSite)
                    {
                        if (psId == SiteId)
                        {
                            show = true;
                        }
                    }
                    else if (contributeType == TransType.ParentSite)
                    {
                        if (psInfo.Id == Site.ParentId || (Site.ParentId == 0 && psInfo.Root))
                        {
                            show = true;
                        }
                    }
                    if (!show) continue;

                    var listitem = new ListItem(psInfo.SiteName, psId.ToString());
                    if (psInfo.Root) listitem.Selected = true;
                    DdlSiteId.Items.Add(listitem);
                }
            }
            DdlSiteId_OnSelectedIndexChanged(sender, e);
        }

        protected void DdlSiteId_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            LbChannelId.Items.Clear();
            if (PhSite.Visible && DdlSiteId.Items.Count > 0)
            {
                ChannelManager.AddListItemsForAddContentAsync(LbChannelId.Items, DataProvider.SiteRepository.GetAsync(int.Parse(DdlSiteId.SelectedValue)).GetAwaiter().GetResult(), false, AuthRequest.AdminPermissionsImpl).GetAwaiter().GetResult();
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isSuccess = false;

            try
            {
                _channel.TransType = TranslateUtils.ToEnum(DdlTransType.SelectedValue, TransType.None);
                _channel.TransSiteId = _channel.TransType == TransType.SpecifiedSite ? TranslateUtils.ToInt(DdlSiteId.SelectedValue) : 0;
                _channel.TransChannelIds = ControlUtils.GetSelectedListControlValueCollection(LbChannelId);
                _channel.TransChannelNames = TbNodeNames.Text;

                _channel.TransIsAutomatic = TranslateUtils.ToBool(DdlIsAutomatic.SelectedValue);

                var translateDoneType = TranslateUtils.ToEnum(DdlTranslateDoneType.SelectedValue, TranslateContentType.Copy);
                _channel.TransDoneType = translateDoneType;

                DataProvider.ChannelRepository.UpdateAsync(_channel).GetAwaiter().GetResult();

                AuthRequest.AddSiteLogAsync(SiteId, "修改跨站转发设置").GetAwaiter().GetResult();

                isSuccess = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }

            if (isSuccess)
            {
                LayerUtils.Close(Page);
            }
        }
    }
}
