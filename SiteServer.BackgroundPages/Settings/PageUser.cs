using System;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageUser : BasePage
    {
        public DropDownList DdlGroupId;
        public DropDownList DdlPageNum;
        public DropDownList DdlLoginCount;

        public DropDownList DdlSearchType;
        public TextBox TbKeyword;
        public DropDownList DdlCreationDate;
        public DropDownList DdlLastActivityDate;

        public Repeater RptContents;
        public SqlPager SpContents;

        public Button BtnCheck;
        public Button BtnAdd;
        public Button BtnLock;
        public Button BtnUnLock;
        public Button BtnDelete;
        public Button BtnExport;

        private EUserLockType _lockType = EUserLockType.Forever;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageUser), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (AuthRequest.IsQueryExists("Check"))
            {
                var userIdList = TranslateUtils.StringCollectionToIntList(AuthRequest.GetQueryString("UserIDCollection"));
                DataProvider.UserDao.Check(userIdList);

                SuccessCheckMessage();
            }
            else if (AuthRequest.IsQueryExists("Delete"))
            {
                var userIdList = TranslateUtils.StringCollectionToIntList(AuthRequest.GetQueryString("UserIDCollection"));
                try
                {
                    foreach (var userId in userIdList)
                    {
                        var userInfo = UserManager.GetUserInfoByUserId(userId);
                        DataProvider.UserDao.Delete(userInfo);
                    }

                    AuthRequest.AddAdminLog("删除用户", string.Empty);

                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }
            else if (AuthRequest.IsQueryExists("Lock"))
            {
                var userIdList = TranslateUtils.StringCollectionToIntList(AuthRequest.GetQueryString("UserIDCollection"));
                try
                {
                    DataProvider.UserDao.Lock(userIdList);

                    AuthRequest.AddAdminLog("锁定用户", string.Empty);

                    SuccessMessage("成功锁定所选会员！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "锁定所选会员失败！");
                }
            }
            else if (AuthRequest.IsQueryExists("UnLock"))
            {
                var userIdList = TranslateUtils.StringCollectionToIntList(AuthRequest.GetQueryString("UserIDCollection"));
                try
                {
                    DataProvider.UserDao.UnLock(userIdList);

                    AuthRequest.AddAdminLog("解除锁定用户", string.Empty);

                    SuccessMessage("成功解除锁定所选会员！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "解除锁定所选会员失败！");
                }
            }

            SpContents.ControlToPaginate = RptContents;

            if (string.IsNullOrEmpty(AuthRequest.GetQueryString("PageNum")))
            {
                SpContents.ItemsPerPage = TranslateUtils.ToInt(DdlPageNum.SelectedValue) == 0 ? 25 : TranslateUtils.ToInt(DdlPageNum.SelectedValue);

                SpContents.SelectCommand = DataProvider.UserDao.GetSelectCommand();
            }
            else
            {
                SpContents.ItemsPerPage = AuthRequest.GetQueryInt("PageNum") == 0 ? StringUtils.Constants.PageSize : AuthRequest.GetQueryInt("PageNum");

                SpContents.SelectCommand = DataProvider.UserDao.GetSelectCommand(AuthRequest.GetQueryInt("groupId"), AuthRequest.GetQueryString("keyword"), AuthRequest.GetQueryInt("creationDate"), AuthRequest.GetQueryInt("lastActivityDate"), AuthRequest.GetQueryInt("loginCount"), AuthRequest.GetQueryString("searchType"));
            }

            RptContents.ItemDataBound += rptContents_ItemDataBound;
            SpContents.OrderByString = "ORDER BY IsChecked, Id DESC";

            _lockType = EUserLockTypeUtils.GetEnumType(ConfigManager.SystemConfigInfo.UserLockLoginType);

            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.User);

            DdlGroupId.Items.Add(new ListItem("<全部用户组>", "-1"));
            foreach (var groupInfo in UserGroupManager.GetUserGroupInfoList())
            {
                DdlGroupId.Items.Add(new ListItem(groupInfo.GroupName, groupInfo.Id.ToString()));
            }
            
            //添加隐藏属性
            DdlSearchType.Items.Add(new ListItem("用户Id", UserAttribute.Id));
            DdlSearchType.Items.Add(new ListItem("用户名", UserAttribute.UserName));
            DdlSearchType.Items.Add(new ListItem("邮箱", UserAttribute.Email));
            DdlSearchType.Items.Add(new ListItem("手机", UserAttribute.Mobile));

            //默认选择用户名
            DdlSearchType.SelectedValue = UserAttribute.UserName;

            if (!string.IsNullOrEmpty(AuthRequest.GetQueryString("groupId")))
            {
                ControlUtils.SelectSingleItem(DdlGroupId, AuthRequest.GetQueryString("groupId"));
            }
            if (!string.IsNullOrEmpty(AuthRequest.GetQueryString("searchType")))
            {
                ControlUtils.SelectSingleItem(DdlSearchType, AuthRequest.GetQueryString("searchType"));
            }
            if (!string.IsNullOrEmpty(AuthRequest.GetQueryString("pageNum")))
            {
                ControlUtils.SelectSingleItem(DdlPageNum, AuthRequest.GetQueryString("pageNum"));
            }
            if (!string.IsNullOrEmpty(AuthRequest.GetQueryString("loginCount")))
            {
                ControlUtils.SelectSingleItem(DdlLoginCount, AuthRequest.GetQueryString("loginCount"));
            }
            if (!string.IsNullOrEmpty(AuthRequest.GetQueryString("keyword")))
            {
                TbKeyword.Text = AuthRequest.GetQueryString("keyword");
            }
            if (!string.IsNullOrEmpty(AuthRequest.GetQueryString("creationDate")))
            {
                ControlUtils.SelectSingleItem(DdlCreationDate, AuthRequest.GetQueryString("creationDate"));
            }
            if (!string.IsNullOrEmpty(AuthRequest.GetQueryString("lastActivityDate")))
            {
                ControlUtils.SelectSingleItem(DdlLastActivityDate, AuthRequest.GetQueryString("lastActivityDate"));
            }

            var backgroundUrl = GetRedirectUrl();

            BtnCheck.Attributes.Add("onclick",
                PageUtils.GetRedirectStringWithCheckBoxValueAndAlert($"{backgroundUrl}?Check=True", "UserIDCollection",
                    "UserIDCollection", "请选择需要审核的会员！", "此操作将审核通过所选会员，确认吗？"));

            BtnAdd.Attributes.Add("onclick",
                $"location.href='{PageUserAdd.GetRedirectUrlToAdd(PageUrl)}';return false;");

            BtnLock.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                $"{backgroundUrl}?Lock=True", "UserIDCollection", "UserIDCollection", "请选择需要锁定的会员！", "此操作将锁定所选会员，确认吗？"));

            BtnUnLock.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                $"{backgroundUrl}?UnLock=True", "UserIDCollection", "UserIDCollection", "请选择需要解除锁定的会员！", "此操作将解除锁定所选会员，确认吗？"));

            BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                $"{backgroundUrl}?Delete=True", "UserIDCollection", "UserIDCollection", "请选择需要删除的会员！", "此操作将删除所选会员，确认吗？"));

            BtnExport.Attributes.Add("onclick", ModalUserExport.GetOpenWindowString());

            SpContents.DataBind();
        }

        private void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            if (e.Item.DataItem == null) return;

            var id = SqlUtils.EvalInt(e.Item.DataItem, nameof(UserInfo.Id));
            var userName = SqlUtils.EvalString(e.Item.DataItem, nameof(UserInfo.UserName));
            var createDate = SqlUtils.EvalDateTime(e.Item.DataItem, nameof(UserInfo.CreateDate));
            var lastActivityDate = SqlUtils.EvalDateTime(e.Item.DataItem, nameof(UserInfo.LastActivityDate));
            var countOfLogin = SqlUtils.EvalInt(e.Item.DataItem, nameof(UserInfo.CountOfLogin));
            var countOfFailedLogin = SqlUtils.EvalInt(e.Item.DataItem, nameof(UserInfo.CountOfFailedLogin));
            var groupId = SqlUtils.EvalInt(e.Item.DataItem, nameof(UserInfo.GroupId));
            var isChecked = SqlUtils.EvalBool(e.Item.DataItem, nameof(UserInfo.IsChecked));
            var isLockedOut = SqlUtils.EvalBool(e.Item.DataItem, nameof(UserInfo.IsLockedOut));
            var displayName = SqlUtils.EvalString(e.Item.DataItem, nameof(UserInfo.DisplayName));
            var email = SqlUtils.EvalString(e.Item.DataItem, nameof(UserInfo.Email));
            var mobile = SqlUtils.EvalString(e.Item.DataItem, nameof(UserInfo.Mobile));

            var ltlUserName = (Literal)e.Item.FindControl("ltlUserName");
            var ltlEmail = (Literal)e.Item.FindControl("ltlEmail");
            var ltlMobile = (Literal)e.Item.FindControl("ltlMobile");
            var ltlGroupName = (Literal)e.Item.FindControl("ltlGroupName");
            var ltlLoginCount = (Literal)e.Item.FindControl("ltlLoginCount");
            var ltlCreationDate = (Literal)e.Item.FindControl("ltlCreationDate");
            var ltlSelect = (Literal)e.Item.FindControl("ltlSelect");
            var hlChangePassword = (HyperLink)e.Item.FindControl("hlChangePassword");
            var hlEditLink = (HyperLink)e.Item.FindControl("hlEditLink");

            var showPopWinString = ModalUserView.GetOpenWindowString(userName);
            var state = isChecked ? string.Empty : @"<span style=""color:red;"">[待审核]</span>";
            if (isLockedOut)
            {
                state += @"<span style=""color:red;"">[已锁定]</span>";
            }
            else if (ConfigManager.SystemConfigInfo.IsUserLockLogin &&
                     ConfigManager.SystemConfigInfo.UserLockLoginCount <= countOfFailedLogin)
            {
                if (_lockType == EUserLockType.Forever)
                {
                    state += @"<span style=""color:red;"">[已锁定]</span>";
                }
                else
                {
                    var ts = new TimeSpan(DateTime.Now.Ticks - lastActivityDate.Ticks);
                    var hours = Convert.ToInt32(ConfigManager.SystemConfigInfo.UserLockLoginHours - ts.TotalHours);
                    if (hours > 0)
                    {
                        state += $@"<span style=""color:red;"">[已锁定{hours}小时]</span>";
                    }
                }
            }

            ltlUserName.Text = $@"<a href=""javascript:;"" onclick=""{showPopWinString}"">{userName}</a> {state}";

            if (!string.IsNullOrEmpty(displayName))
            {
                ltlUserName.Text += $"({displayName})";
            }
            ltlEmail.Text = email;
            ltlMobile.Text = mobile;
            ltlGroupName.Text = UserGroupManager.GetUserGroupInfo(groupId).GroupName;
            ltlLoginCount.Text = countOfLogin.ToString();
            ltlCreationDate.Text = DateUtils.GetDateAndTimeString(createDate);

            hlEditLink.NavigateUrl = PageUserAdd.GetRedirectUrlToEdit(id, GetRedirectUrl());
            hlChangePassword.Attributes.Add("onclick", ModalUserPassword.GetOpenWindowString(userName));
            ltlSelect.Text = $@"<input type=""checkbox"" name=""UserIDCollection"" value=""{id}"" />";
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageUrl);
        }

        private string _pageUrl;
        private string PageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_pageUrl))
                {
                    _pageUrl =
                        $"{GetRedirectUrl()}?groupId={DdlGroupId.SelectedValue}&pageNum={DdlPageNum.SelectedValue}&keyword={TbKeyword.Text}&creationDate={DdlCreationDate.SelectedValue}&lastActivityDate={DdlLastActivityDate.SelectedValue}&loginCount={DdlLoginCount.SelectedValue}&searchType={DdlSearchType.SelectedValue}";
                }
                return _pageUrl;
            }
        }
    }
}
