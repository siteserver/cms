using System;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageUtilityAccessTokens : BasePage
    {
        public Repeater RptContents;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageUtilityAccessTokens), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (IsPostBack) return;

            VerifyAdministratorPermissions(ConfigManager.SettingsPermissions.Utility);

            if (AuthRequest.IsQueryExists("delete") && AuthRequest.IsQueryExists("id"))
            {
                DataProvider.AccessTokenDao.Delete(AuthRequest.GetQueryInt("id"));

                AuthRequest.AddAdminLog("删除API密钥");
                SuccessMessage("API密钥删除成功！");
            }

            RptContents.DataSource = DataProvider.AccessTokenDao.GetAccessTokenInfoList();
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var tokenInfo = (AccessTokenInfo) e.Item.DataItem;

            var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
            var ltlScopes = (Literal)e.Item.FindControl("ltlScopes");
            var ltlActions = (Literal)e.Item.FindControl("ltlActions");

            ltlTitle.Text = $@"<a href=""javascript:;"" onclick=""{ModalUtilityAccessToken.GetOpenWindowString(tokenInfo.Id)}"">{tokenInfo.Title}</a>";
            ltlScopes.Text = tokenInfo.Scopes;

            ltlActions.Text = $@"
<a href=""{PageUtilityAccessTokensAdd.GetRedirectUrlToEdit(tokenInfo.Id)}"" class=""btn btn-success m-r-5"">编 辑</a>
<a href=""javascript:;"" class=""btn btn-danger m-r-5"" onclick=""{AlertUtils.ConfirmDelete("删除API密钥", $"此操作将删除API密钥 {tokenInfo.Title}，确定吗？", GetRedirectUrl() + "?delete=true&id=" + tokenInfo.Id)}"">删 除</a>";
        }
    }
}
