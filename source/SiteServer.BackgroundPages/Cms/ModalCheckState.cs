using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Permissions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.User;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalCheckState : BasePageCms
    {
        public Literal ltlTitle;
        public Literal ltlState;
        public PlaceHolder phCheckReasons;
        public Repeater rpContents;
        public PlaceHolder phCheck;

        private int _nodeId;
        private ETableStyle _tableStyle;
        private string _tableName;
        private int _contentId;
        private string _returnUrl;

        public static string GetOpenWindowString(int publishmentSystemId, ContentInfo contentInfo, string returnUrl)
        {
            return PageUtils.GetOpenWindowString("审核状态",
                PageUtils.GetCmsUrl(nameof(ModalCheckState), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"NodeID", contentInfo.NodeId.ToString()},
                    {"ContentID", contentInfo.Id.ToString()},
                    {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
                }), 560, 500);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "NodeID", "ContentID", "ReturnUrl");

            _nodeId = Body.GetQueryInt("NodeID");
            _tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, _nodeId);
            _tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeId);
            _contentId = Body.GetQueryInt("ContentID");
            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));

            var contentInfo = DataProvider.ContentDao.GetContentInfo(_tableStyle, _tableName, _contentId);

            var checkedLevel = 0;
            var isChecked = CheckManager.GetUserCheckLevel(Body.AdministratorName, PublishmentSystemInfo, PublishmentSystemId, out checkedLevel);
            phCheck.Visible = LevelManager.IsCheckable(PublishmentSystemInfo, _nodeId, contentInfo.IsChecked, contentInfo.CheckedLevel, isChecked, checkedLevel);

            ltlTitle.Text = contentInfo.Title;
            ltlState.Text = LevelManager.GetCheckState(PublishmentSystemInfo, contentInfo.IsChecked, contentInfo.CheckedLevel);

            var checkInfoArrayList = BaiRongDataProvider.ContentCheckDao.GetCheckInfoArrayList(_tableName, _contentId);
            if (checkInfoArrayList.Count > 0)
            {
                phCheckReasons.Visible = true;
                rpContents.DataSource = checkInfoArrayList;
                rpContents.ItemDataBound += rpContents_ItemDataBound;
                rpContents.DataBind();
            }
        }

        void rpContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var checkInfo = e.Item.DataItem as ContentCheckInfo;

            var ltlUserName = e.Item.FindControl("ltlUserName") as Literal;
            var ltlCheckDate = e.Item.FindControl("ltlCheckDate") as Literal;
            var ltlReasons = e.Item.FindControl("ltlReasons") as Literal;

            ltlUserName.Text = AdminManager.GetDisplayName(checkInfo.UserName, true);
            ltlCheckDate.Text = DateUtils.GetDateAndTimeString(checkInfo.CheckDate);
            ltlReasons.Text = checkInfo.Reasons;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var redirectUrl = ModalContentCheck.GetRedirectUrl(PublishmentSystemId, _nodeId, _contentId, _returnUrl);
            PageUtils.Redirect(redirectUrl);
        }

    }
}
