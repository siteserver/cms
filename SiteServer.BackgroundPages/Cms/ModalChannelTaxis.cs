using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalChannelTaxis : BasePageCms
    {
        protected DropDownList DdlTaxisType;
        protected TextBox TbTaxisNum;

        private int _channelId;
        private string _returnUrl;
        private List<int> _contentIdList;
        private string _tableName;

        public static string GetOpenWindowString(int siteId)
        {
            return LayerUtils.GetOpenScriptWithCheckBoxValue("栏目排序", PageUtils.GetCmsUrl(siteId, nameof(ModalChannelTaxis), null), "ChannelIDCollection", "请选择需要排序的内容！", 400, 280);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "ChannelIDCollection");

            _channelId = AuthRequest.GetQueryInt("ChannelIDCollection");
            _contentIdList = TranslateUtils.StringCollectionToIntList(AuthRequest.GetQueryString("ChannelIDCollection"));
            _tableName = ChannelManager.GetTableName(SiteInfo, _channelId);

            if (IsPostBack) return;

            DdlTaxisType.Items.Add(new ListItem("上升", "Up"));
            DdlTaxisType.Items.Add(new ListItem("下降", "Down"));
            ControlUtils.SelectSingleItem(DdlTaxisType, "Up");
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isUp = DdlTaxisType.SelectedValue == "Up";
            var taxisNum = TranslateUtils.ToInt(TbTaxisNum.Text);

            foreach (var contentId in _contentIdList)
            {
                DataProvider.ChannelDao.UpdateTaxis(SiteId, _channelId, isUp);

                AuthRequest.AddSiteLog(SiteId, _channelId, 0, "栏目排序" + (isUp ? "上升" : "下降"),
                    $"栏目:{ChannelManager.GetChannelName(SiteId, _channelId)}");
            }
            LayerUtils.CloseAndRedirect(Page, PageChannel.GetRedirectUrl(SiteId, _channelId));
        }

    }
}
