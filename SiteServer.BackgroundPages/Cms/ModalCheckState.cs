using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalCheckState : BasePageCms
    {
        protected override bool IsSinglePage => true;
        public Literal LtlTitle;
        public Literal LtlState;
        public PlaceHolder PhCheckReasons;
        public Repeater RptContents;
        public Button BtnCheck;

        private int _channelId;
        private string _tableName;
        private int _contentId;
        private string _returnUrl;

        public static string GetOpenWindowString(int siteId, ContentInfo contentInfo, string returnUrl)
        {
            return LayerUtils.GetOpenScript("审核状态",
                PageUtils.GetCmsUrl(siteId, nameof(ModalCheckState), new NameValueCollection
                {
                    {"channelId", contentInfo.ChannelId.ToString()},
                    {"contentID", contentInfo.Id.ToString()},
                    {"returnUrl", StringUtils.ValueToUrl(returnUrl)}
                }), 560, 500);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "channelId", "contentID", "returnUrl");

            _channelId = AuthRequest.GetQueryInt("channelId");
            _tableName = ChannelManager.GetTableName(SiteInfo, _channelId);
            _contentId = AuthRequest.GetQueryInt("contentID");
            _returnUrl = StringUtils.ValueFromUrl(AuthRequest.GetQueryString("returnUrl"));

            var contentInfo = ContentManager.GetContentInfo(SiteInfo, _channelId, _contentId);

            int checkedLevel;
            var isChecked = CheckManager.GetUserCheckLevel(AuthRequest.AdminPermissionsImpl, SiteInfo, SiteId, out checkedLevel);
            BtnCheck.Visible = CheckManager.IsCheckable(contentInfo.IsChecked, contentInfo.CheckedLevel, isChecked, checkedLevel);

            LtlTitle.Text = contentInfo.Title;
            LtlState.Text = CheckManager.GetCheckState(SiteInfo, contentInfo);

            var checkInfoList = DataProvider.ContentCheckDao.GetCheckInfoList(_tableName, _contentId);
            if (checkInfoList.Count > 0)
            {
                PhCheckReasons.Visible = true;
                RptContents.DataSource = checkInfoList;
                RptContents.ItemDataBound += RptContents_ItemDataBound;
                RptContents.DataBind();
            }
        }

        private static void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var checkInfo = (ContentCheckInfo)e.Item.DataItem;

            var ltlUserName = (Literal)e.Item.FindControl("ltlUserName");
            var ltlCheckDate = (Literal)e.Item.FindControl("ltlCheckDate");
            var ltlReasons = (Literal)e.Item.FindControl("ltlReasons");

            ltlUserName.Text = AdminManager.GetDisplayName(checkInfo.UserName, true);
            ltlCheckDate.Text = DateUtils.GetDateAndTimeString(checkInfo.CheckDate);
            ltlReasons.Text = checkInfo.Reasons;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var redirectUrl = ModalContentCheck.GetRedirectUrl(SiteId, _channelId, _contentId, _returnUrl);
            PageUtils.Redirect(redirectUrl);
        }

    }
}
