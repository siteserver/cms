using System;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageUser : BasePage
    {
        public DropDownList DdlPageNum;
        public DropDownList DdlLoginCount;

        public DropDownList DdlSearchType;
        public TextBox TbKeyword;
        public DropDownList DdlCreationDate;
        public DropDownList DdlLastActivityDate;

        public Repeater RptContents;
        public SqlPager SpContents;

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

            if (AuthRequest.IsQueryExists("Delete"))
            {
                var userIdList = TranslateUtils.StringCollectionToIntList(AuthRequest.GetQueryString("UserIDCollection"));
                try
                {
                    foreach (var userId in userIdList)
                    {
                        DataProvider.UserDao.Delete(userId);
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

                SpContents.SelectCommand = DataProvider.UserDao.GetSelectCommand(true);
            }
            else
            {
                SpContents.ItemsPerPage = AuthRequest.GetQueryInt("PageNum") == 0 ? StringUtils.Constants.PageSize : AuthRequest.GetQueryInt("PageNum");
                SpContents.SelectCommand = DataProvider.UserDao.GetSelectCommand(AuthRequest.GetQueryString("Keyword"), AuthRequest.GetQueryInt("CreationDate"), AuthRequest.GetQueryInt("LastActivityDate"), true, AuthRequest.GetQueryInt("LoginCount"), AuthRequest.GetQueryString("SearchType"));
            }

            RptContents.ItemDataBound += rptContents_ItemDataBound;
            SpContents.SortField = DataProvider.UserDao.GetSortFieldName();
            SpContents.SortMode = SortMode.DESC;

            _lockType = EUserLockTypeUtils.GetEnumType(ConfigManager.SystemConfigInfo.UserLockLoginType);

            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.User);

            //添加隐藏属性
            DdlSearchType.Items.Add(new ListItem("用户ID", "userID"));
            DdlSearchType.Items.Add(new ListItem("用户名", "userName"));
            DdlSearchType.Items.Add(new ListItem("邮箱", "email"));
            DdlSearchType.Items.Add(new ListItem("手机", "mobile"));

            //默认选择用户名
            DdlSearchType.SelectedValue = "userName";

            if (!string.IsNullOrEmpty(AuthRequest.GetQueryString("SearchType")))
            {
                ControlUtils.SelectSingleItem(DdlSearchType, AuthRequest.GetQueryString("SearchType"));
            }
            if (!string.IsNullOrEmpty(AuthRequest.GetQueryString("PageNum")))
            {
                ControlUtils.SelectSingleItem(DdlPageNum, AuthRequest.GetQueryString("PageNum"));
            }
            if (!string.IsNullOrEmpty(AuthRequest.GetQueryString("LoginCount")))
            {
                ControlUtils.SelectSingleItem(DdlLoginCount, AuthRequest.GetQueryString("LoginCount"));
            }
            if (!string.IsNullOrEmpty(AuthRequest.GetQueryString("Keyword")))
            {
                TbKeyword.Text = AuthRequest.GetQueryString("Keyword");
            }
            if (!string.IsNullOrEmpty(AuthRequest.GetQueryString("CreationDate")))
            {
                ControlUtils.SelectSingleItem(DdlCreationDate, AuthRequest.GetQueryString("CreationDate"));
            }
            if (!string.IsNullOrEmpty(AuthRequest.GetQueryString("LastActivityDate")))
            {
                ControlUtils.SelectSingleItem(DdlLastActivityDate, AuthRequest.GetQueryString("LastActivityDate"));
            }

            var backgroundUrl = GetRedirectUrl();

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

        public void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var userInfo = new UserInfo(e.Item.DataItem);

            var ltlUserName = (Literal)e.Item.FindControl("ltlUserName");
            var ltlDisplayName = (Literal)e.Item.FindControl("ltlDisplayName");
            var ltlEmail = (Literal)e.Item.FindControl("ltlEmail");
            var ltlMobile = (Literal)e.Item.FindControl("ltlMobile");
            var ltlLastActivityDate = (Literal)e.Item.FindControl("ltlLastActivityDate");
            var ltlLoginCount = (Literal)e.Item.FindControl("ltlLoginCount");
            var ltlCreationDate = (Literal)e.Item.FindControl("ltlCreationDate");
            var ltlWritingCount = (Literal)e.Item.FindControl("ltlWritingCount");
            var ltlSelect = (Literal)e.Item.FindControl("ltlSelect");
            var hlChangePassword = (HyperLink)e.Item.FindControl("hlChangePassword");
            var hlEditLink = (HyperLink)e.Item.FindControl("hlEditLink");

            ltlUserName.Text = GetUserNameHtml(userInfo);
            ltlDisplayName.Text = userInfo.DisplayName;
            ltlEmail.Text = userInfo.Email;
            ltlMobile.Text = userInfo.Mobile;
            ltlLastActivityDate.Text = DateUtils.GetDateAndTimeString(userInfo.LastActivityDate);
            ltlLoginCount.Text = userInfo.CountOfLogin.ToString();
            ltlCreationDate.Text = DateUtils.GetDateAndTimeString(userInfo.CreateDate);

            hlEditLink.NavigateUrl = PageUserAdd.GetRedirectUrlToEdit(userInfo.Id, GetRedirectUrl());
            hlChangePassword.Attributes.Add("onclick", ModalUserPassword.GetOpenWindowString(userInfo.UserName));
            ltlSelect.Text = $@"<input type=""checkbox"" name=""UserIDCollection"" value=""{userInfo.Id}"" />";

            ltlWritingCount.Text = userInfo.CountOfWriting.ToString();
        }

        private string GetUserNameHtml(UserInfo userInfo)
        {
            var showPopWinString = ModalUserView.GetOpenWindowString(userInfo.UserName);
            var state = string.Empty;
            if (userInfo.IsLockedOut)
            {
                state = @"<span style=""color:red;"">[已被锁定]</span>";
            }
            else if (ConfigManager.SystemConfigInfo.IsUserLockLogin &&
                       ConfigManager.SystemConfigInfo.UserLockLoginCount <= userInfo.CountOfFailedLogin)
            {
                if (_lockType == EUserLockType.Forever)
                {
                    state = @"<span style=""color:red;"">[已被锁定]</span>";
                }
                else
                {
                    var ts = new TimeSpan(DateTime.Now.Ticks - userInfo.LastActivityDate.Ticks);
                    var hours = Convert.ToInt32(ConfigManager.SystemConfigInfo.UserLockLoginHours - ts.TotalHours);
                    if (hours > 0)
                    {
                        state = $@"<span style=""color:red;"">[错误登录次数过多，已被锁定{hours}小时]</span>";
                    }
                }
            }
            return $@"<a href=""javascript:;"" onclick=""{showPopWinString}"">{userInfo.UserName}</a> {state}";
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
                        $"{GetRedirectUrl()}?PageNum={DdlPageNum.SelectedValue}&Keyword={TbKeyword.Text}&CreationDate={DdlCreationDate.SelectedValue}&LastActivityDate={DdlLastActivityDate.SelectedValue}&loginCount={DdlLoginCount.SelectedValue}&SearchType={DdlSearchType.SelectedValue}";
                }
                return _pageUrl;
            }
        }
    }
}
