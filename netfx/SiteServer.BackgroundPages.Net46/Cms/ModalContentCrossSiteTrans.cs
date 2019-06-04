using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalContentCrossSiteTrans : BasePageCms
    {
        protected DropDownList DdlSiteId;
        protected ListBox LbChannelId;

        private ChannelInfo _channelInfo;
        private List<int> _contentIdList;

        public static string GetOpenWindowString(int siteId, int channelId)
        {
            return LayerUtils.GetOpenScriptWithCheckBoxValue("跨站转发", PageUtilsEx.GetCmsUrl(siteId, nameof(ModalContentCrossSiteTrans), new NameValueCollection
            {
                {"channelId", channelId.ToString()}
            }), "contentIdCollection", "请选择需要转发的内容！", 400, 410);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            FxUtils.CheckRequestParameter("siteId", "channelId", "contentIdCollection");

            _channelInfo = ChannelManager.GetChannelInfo(SiteId, AuthRequest.GetQueryInt("channelId"));
            _contentIdList = TranslateUtils.StringCollectionToIntList(AuthRequest.GetQueryString("contentIdCollection"));

            if (IsPostBack) return;

            ControlUtils.CrossSiteTransUI.LoadSiteIdDropDownList(DdlSiteId, SiteInfo, _channelInfo.Id);

            if (DdlSiteId.Items.Count > 0)
            {
                DdlSiteId_SelectedIndexChanged(null, EventArgs.Empty);
            }
        }

        public void DdlSiteId_SelectedIndexChanged(object sender, EventArgs e)
        {
            var psId = int.Parse(DdlSiteId.SelectedValue);
            ControlUtils.CrossSiteTransUI.LoadChannelIdListBox(LbChannelId, SiteInfo, psId, _channelInfo, AuthRequest.AdminPermissionsImpl);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var targetSiteId = int.Parse(DdlSiteId.SelectedValue);
            var targetSiteInfo = SiteManager.GetSiteInfo(targetSiteId);
            try
            {
                foreach (ListItem listItem in LbChannelId.Items)
                {
                    if (listItem.Selected)
                    {
                        var targetChannelId = TranslateUtils.ToInt(listItem.Value);
                        if (targetChannelId != 0)
                        {
                            var targetChannelInfo = ChannelManager.GetChannelInfo(targetSiteId, targetChannelId);
                            foreach (var contentId in _contentIdList)
                            {
                                var contentInfo = ContentManager.GetContentInfo(SiteInfo, _channelInfo, contentId);
                                FileUtility.MoveFileByContentInfo(SiteInfo, targetSiteInfo, contentInfo);
                                contentInfo.SiteId = targetSiteId;
                                contentInfo.SourceId = contentInfo.ChannelId;
                                contentInfo.ChannelId = targetChannelId;

                                contentInfo.Checked = targetSiteInfo.IsCrossSiteTransChecked;
                                contentInfo.CheckedLevel = 0;

                                targetChannelInfo.ContentDao.Insert(targetSiteInfo, targetChannelInfo, contentInfo);
                            }
                        }
                    }
                }

                AuthRequest.AddChannelLog(SiteId, _channelInfo.Id, "跨站转发", string.Empty);

                SuccessMessage("内容转发成功，请选择后续操作。");
            }
            catch (Exception ex)
            {
                FailMessage(ex, "内容转发失败！");
            }

            LayerUtils.Close(Page);
        }

    }
}
