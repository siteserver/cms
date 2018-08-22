using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageUserCheck : BasePage
    {
        public Repeater RptContents;
        public SqlPager SpContents;

        public Button BtnCheck;
        public Button BtnDelete;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageUserCheck), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (AuthRequest.IsQueryExists("Delete"))
            {
                var userIdList = TranslateUtils.StringCollectionToIntList(AuthRequest.GetQueryString("UserIDCollection"));
                foreach (var userId in userIdList)
                {
                    DataProvider.UserDao.Delete(userId);
                }

                AuthRequest.AddAdminLog("删除用户", string.Empty);

                SuccessDeleteMessage();
            }
            else if (AuthRequest.IsQueryExists("Check"))
            {
                var userIdList = TranslateUtils.StringCollectionToIntList(AuthRequest.GetQueryString("UserIDCollection"));
                DataProvider.UserDao.Check(userIdList);

                SuccessCheckMessage();
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = 25;
            SpContents.SelectCommand = DataProvider.UserDao.GetSelectCommand(false);
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            SpContents.SortField = DataProvider.UserDao.GetSortFieldName();
            SpContents.SortMode = SortMode.DESC;

            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.User);

            SpContents.DataBind();

            BtnCheck.Attributes.Add("onclick",
                PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                    PageUtils.GetSettingsUrl(nameof(PageUserCheck), new NameValueCollection
                    {
                        {"Check", "True"}
                    }), "UserIDCollection", "UserIDCollection", "请选择需要审核的会员！", "此操作将审核通过所选会员，确认吗？"));

            BtnDelete.Attributes.Add("onclick",
                PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                    PageUtils.GetSettingsUrl(nameof(PageUserCheck), new NameValueCollection
                    {
                        {"Delete", "True"}
                    }), "UserIDCollection", "UserIDCollection", "请选择需要删除的会员！", "此操作将删除所选会员，确认吗？"));
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var userInfo = new UserInfo(e.Item.DataItem);

            var ltlUserName = (Literal)e.Item.FindControl("ltlUserName");
            var ltlDisplayName = (Literal)e.Item.FindControl("ltlDisplayName");
            var ltlCreateDate = (Literal)e.Item.FindControl("ltlCreateDate");
            var ltlSelect = (Literal)e.Item.FindControl("ltlSelect");
            var hlEditLink = (HyperLink)e.Item.FindControl("hlEditLink");

            var state = string.Empty;
            if (userInfo.IsLockedOut)
            {
                state = @"<span style=""color:red;"">[已被锁定]</span>";
            }
            ltlUserName.Text = $@"<a href=""javascript:;"" onclick=""{ModalUserView.GetOpenWindowString(userInfo.UserName)}"">{userInfo.UserName}</a>{state}";

            ltlDisplayName.Text = userInfo.DisplayName;

            ltlCreateDate.Text = DateUtils.GetDateAndTimeString(userInfo.CreateDate);

            var userAddUrl = PageUserAdd.GetRedirectUrlToEdit(userInfo.Id, GetRedirectUrl());
            hlEditLink.NavigateUrl = userAddUrl;

            ltlSelect.Text = $@"<input type=""checkbox"" name=""UserIDCollection"" value=""{userInfo.Id}"" />";
        }
    }
}
