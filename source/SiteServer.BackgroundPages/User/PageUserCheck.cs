using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Text;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;

namespace SiteServer.BackgroundPages.User
{
    public class PageUserCheck : BasePage
    {
        public Repeater rptContents;
        public SqlPager spContents;

        public Button Check;
        public Button Delete;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetUserUrl(nameof(PageUserCheck), null);
        }

        public static string GetViewHtml(int userID, string userName)
        {
            var showPopWinString = ModalUserView.GetOpenWindowString(userName);
            return $@"<a href=""javascript:;"" onclick=""{showPopWinString}"">{userName}</a>";
        }

        public string GetDateTime(DateTime datetime)
        {
            var retval = string.Empty;
            if (datetime > DateUtils.SqlMinValue)
            {
                retval = DateUtils.GetDateString(datetime);
            }
            return retval;
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Body.IsQueryExists("Delete"))
            {
                var userIdList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("UserIDCollection"));
                try
                {
                    foreach (var userId in userIdList)
                    {
                        BaiRongDataProvider.UserDao.Delete(userId);
                    }

                    Body.AddAdminLog("删除用户", string.Empty);

                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }
            else if (Body.IsQueryExists("Check"))
            {
                var userIdList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("UserIDCollection"));
                try
                {
                    BaiRongDataProvider.UserDao.Check(userIdList);

                    SuccessCheckMessage();
                }
                catch (Exception ex)
                {
                    FailCheckMessage(ex);
                }
            }

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = 25;
            spContents.SelectCommand = BaiRongDataProvider.UserDao.GetSelectCommand(false);
            rptContents.ItemDataBound += rptContents_ItemDataBound;
            spContents.SortField = BaiRongDataProvider.UserDao.GetSortFieldName();
            spContents.SortMode = SortMode.DESC;

            if (!IsPostBack)
            {
                BreadCrumbUser(AppManager.User.LeftMenu.UserManagement, "审核新用户", AppManager.User.Permission.UserManagement);

                spContents.DataBind();

                Check.Attributes.Add("onclick",
                    PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                        PageUtils.GetUserUrl(nameof(PageUserCheck), new NameValueCollection
                        {
                            {"Check", "True"}
                        }), "UserIDCollection", "UserIDCollection", "请选择需要审核的会员！", "此操作将审核通过所选会员，确认吗？"));

                Delete.Attributes.Add("onclick",
                    PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                        PageUtils.GetUserUrl(nameof(PageUserCheck), new NameValueCollection
                        {
                            {"Delete", "True"}
                        }), "UserIDCollection", "UserIDCollection", "请选择需要删除的会员！", "此操作将删除所选会员，确认吗？"));
            }
        }

        private string GetUserNameHtml(string userName, bool isLockedOut)
        {
            var showPopWinString = ModalUserView.GetOpenWindowString(userName);
            var state = string.Empty;
            if (isLockedOut)
            {
                state = @"<span style=""color:red;"">[已被锁定]</span>";
            }
            return $@"<a href=""javascript:;"" onclick=""{showPopWinString}"">{userName}</a>{state}";
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var userInfo = new UserInfo(e.Item.DataItem);

                var ltlUserName = (Literal)e.Item.FindControl("ltlUserName");
                var ltlDisplayName = (Literal)e.Item.FindControl("ltlDisplayName");
                var ltlUserGroupName = (Literal)e.Item.FindControl("ltlUserGroupName");
                var ltlCreateDate = (Literal)e.Item.FindControl("ltlCreateDate");
                var ltlSelect = (Literal)e.Item.FindControl("ltlSelect");
                var hlEditLink = (HyperLink)e.Item.FindControl("hlEditLink");

                ltlUserName.Text = GetUserNameHtml(userInfo.UserName, userInfo.IsLockedOut);
                ltlDisplayName.Text = userInfo.DisplayName;

                ltlCreateDate.Text = DateUtils.GetDateAndTimeString(userInfo.CreateDate);

                var userAddUrl = PageUserAdd.GetRedirectUrlToEdit(userInfo.UserId, GetRedirectUrl());
                hlEditLink.NavigateUrl = userAddUrl;

                ltlSelect.Text = $@"<input type=""checkbox"" name=""UserIDCollection"" value=""{userInfo.UserId}"" />";
            }
        }
    }
}
