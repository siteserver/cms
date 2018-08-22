using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Settings
{
    public static class PageAdminAccessTokens
    {
        public static readonly string PageUrl = PageUtils.GetSettingsUrl(nameof(PageAdminAccessTokens));

        //public Repeater RptContents;

        //public void Page_Load(object sender, EventArgs e)
        //{
        //    if (IsForbidden) return;

        //    if (IsPostBack) return;

        //    VerifyAdministratorPermissions(ConfigManager.SettingsPermissions.Admin);

        //    if (AuthRequest.IsQueryExists("delete") && AuthRequest.IsQueryExists("id"))
        //    {
        //        DataProvider.AccessTokenDao.Delete(AuthRequest.GetQueryInt("id"));

        //        AuthRequest.AddAdminLog("删除API密钥");
        //        SuccessMessage("API密钥删除成功！");
        //    }

        //    //RptContents.DataSource = DataProvider.AccessTokenDao.GetAccessTokenInfoList();
        //    //RptContents.ItemDataBound += RptContents_ItemDataBound;
        //    //RptContents.DataBind();
        //}

        //        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        //        {
        //            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

        //            var tokenInfo = (AccessTokenInfo) e.Item.DataItem;

        //            var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
        //            var ltlAdminName= (Literal)e.Item.FindControl("ltlAdminName");
        //            var ltlScopes = (Literal)e.Item.FindControl("ltlScopes");
        //            var ltlActions = (Literal)e.Item.FindControl("ltlActions");

        //            ltlTitle.Text = tokenInfo.Title;
        //            ltlAdminName.Text = tokenInfo.AdminName;
        //            ltlScopes.Text = tokenInfo.Scopes;

        //            ltlActions.Text = $@"
        //<a href=""javascript:;"" onclick=""{ModalAdminAccessToken.GetOpenWindowString(tokenInfo.Id)}"" class=""btn btn-primary m-r-5"">获取密钥</a>
        //<a href=""{PageAdminAccessTokensAdd.GetRedirectUrlToEdit(tokenInfo.Id)}"" class=""btn btn-success m-r-5"">编 辑</a>
        //<a href=""javascript:;"" class=""btn btn-danger m-r-5"" onclick=""{AlertUtils.ConfirmDelete("删除API密钥", $"此操作将删除API密钥 {tokenInfo.Title}，确定吗？", PageUrl + "?delete=true&id=" + tokenInfo.Id)}"">删 除</a>";
        //        }
    }
}
