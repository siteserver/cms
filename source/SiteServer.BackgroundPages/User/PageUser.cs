using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;

namespace SiteServer.BackgroundPages.User
{
    public class PageUser : BasePage
    {
        public DropDownList DdlGroup;
        public DropDownList DdlPageNum;
        public DropDownList DdlLoginCount;

        public DropDownList DdlSearchType;
        public TextBox TbKeyword;
        public DropDownList DdlCreationDate;
        public DropDownList DdlLastActivityDate;

        public Repeater RptContents;
        public SqlCountPager SpContents;

        public Button BtnAdd;
        public Button BtnAddToGroup;
        public Button BtnLock;
        public Button BtnUnLock;
        public Button BtnDelete;
        public Button BtnImport;
        public Button BtnExport;

        private EUserLockType _lockType = EUserLockType.Forever;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetUserUrl(nameof(PageUser), null);
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
            else if (Body.IsQueryExists("Lock"))
            {
                var userIdList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("UserIDCollection"));
                try
                {
                    BaiRongDataProvider.UserDao.Lock(userIdList);

                    Body.AddAdminLog("锁定用户", string.Empty);

                    SuccessMessage("成功锁定所选会员！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "锁定所选会员失败！");
                }
            }
            else if (Body.IsQueryExists("UnLock"))
            {
                var userIdList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("UserIDCollection"));
                try
                {
                    BaiRongDataProvider.UserDao.UnLock(userIdList);

                    Body.AddAdminLog("解除锁定用户", string.Empty);

                    SuccessMessage("成功解除锁定所选会员！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "解除锁定所选会员失败！");
                }
            }

            SpContents.ControlToPaginate = RptContents;

            if (string.IsNullOrEmpty(Body.GetQueryString("GroupID")))
            {
                SpContents.ItemsPerPage = TranslateUtils.ToInt(DdlPageNum.SelectedValue) == 0 ? 25 : TranslateUtils.ToInt(DdlPageNum.SelectedValue);

                SpContents.SelectCommand = BaiRongDataProvider.UserDao.GetSelectCommand(true);
            }
            else
            {
                SpContents.ItemsPerPage = Body.GetQueryInt("PageNum") == 0 ? StringUtils.Constants.PageSize : Body.GetQueryInt("PageNum");
                SpContents.SelectCommand = BaiRongDataProvider.UserDao.GetSelectCommand(Body.GetQueryString("Keyword"), Body.GetQueryInt("CreationDate"), Body.GetQueryInt("LastActivityDate"), true, Body.GetQueryInt("GroupID"), Body.GetQueryInt("LoginCount"), Body.GetQueryString("SearchType"));
            }

            RptContents.ItemDataBound += rptContents_ItemDataBound;
            SpContents.SortField = BaiRongDataProvider.UserDao.GetSortFieldName();
            SpContents.SortMode = SortMode.DESC;

            _lockType = EUserLockTypeUtils.GetEnumType(ConfigManager.UserConfigInfo.LoginLockingType);

            if (IsPostBack) return;

            BreadCrumbUser(AppManager.User.LeftMenu.UserManagement, "用户管理", AppManager.User.Permission.UserManagement);

            var theListItem = new ListItem("全部", "0")
            {
                Selected = true
            };
            DdlGroup.Items.Add(theListItem);
            var groupInfoList = UserGroupManager.GetGroupInfoList();
            foreach (var userGroupInfo in groupInfoList)
            {
                var listitem = new ListItem(userGroupInfo.GroupName, userGroupInfo.GroupId.ToString());
                DdlGroup.Items.Add(listitem);
            }

            //添加隐藏属性
            DdlSearchType.Items.Add(new ListItem("用户ID", "userID"));
            DdlSearchType.Items.Add(new ListItem("用户名", "userName"));
            DdlSearchType.Items.Add(new ListItem("邮箱", "email"));
            DdlSearchType.Items.Add(new ListItem("手机", "mobile"));

            //默认选择用户名
            DdlSearchType.SelectedValue = "userName";

            if (!string.IsNullOrEmpty(Body.GetQueryString("SearchType")))
            {
                ControlUtils.SelectListItems(DdlSearchType, Body.GetQueryString("SearchType"));
            }
            if (!string.IsNullOrEmpty(Body.GetQueryString("GroupID")))
            {
                ControlUtils.SelectListItems(DdlGroup, Body.GetQueryString("GroupID"));
            }
            if (!string.IsNullOrEmpty(Body.GetQueryString("PageNum")))
            {
                ControlUtils.SelectListItems(DdlPageNum, Body.GetQueryString("PageNum"));
            }
            if (!string.IsNullOrEmpty(Body.GetQueryString("LoginCount")))
            {
                ControlUtils.SelectListItems(DdlLoginCount, Body.GetQueryString("LoginCount"));
            }
            if (!string.IsNullOrEmpty(Body.GetQueryString("Keyword")))
            {
                TbKeyword.Text = Body.GetQueryString("Keyword");
            }
            if (!string.IsNullOrEmpty(Body.GetQueryString("CreationDate")))
            {
                ControlUtils.SelectListItems(DdlCreationDate, Body.GetQueryString("CreationDate"));
            }
            if (!string.IsNullOrEmpty(Body.GetQueryString("LastActivityDate")))
            {
                ControlUtils.SelectListItems(DdlLastActivityDate, Body.GetQueryString("LastActivityDate"));
            }

            var showPopWinString = ModalAddToUserGroup.GetOpenWindowString();
            BtnAddToGroup.Attributes.Add("onclick", showPopWinString);

            var backgroundUrl = GetRedirectUrl();

            BtnAdd.Attributes.Add("onclick",
                $"location.href='{PageUserAdd.GetRedirectUrlToAdd(PageUrl)}';return false;");

            BtnLock.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                $"{backgroundUrl}?Lock=True", "UserIDCollection", "UserIDCollection", "请选择需要锁定的会员！", "此操作将锁定所选会员，确认吗？"));

            BtnUnLock.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                $"{backgroundUrl}?UnLock=True", "UserIDCollection", "UserIDCollection", "请选择需要解除锁定的会员！", "此操作将解除锁定所选会员，确认吗？"));

            BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                $"{backgroundUrl}?Delete=True", "UserIDCollection", "UserIDCollection", "请选择需要删除的会员！", "此操作将删除所选会员，确认吗？"));

            BtnImport.Attributes.Add("onclick", ModalUserImport.GetOpenWindowString());

            BtnExport.Attributes.Add("onclick", ModalUserExport.GetOpenWindowString());

            SpContents.DataBind();
        }

        public void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var userInfo = new UserInfo(e.Item.DataItem);

            var ltlUserName = (Literal)e.Item.FindControl("ltlUserName");
            var ltlDisplayName = (Literal)e.Item.FindControl("ltlDisplayName");
            var ltlGroupName = (Literal)e.Item.FindControl("ltlGroupName");
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
            ltlGroupName.Text = UserGroupManager.GetGroupName(userInfo.GroupId);
            ltlLastActivityDate.Text = DateUtils.GetDateAndTimeString(userInfo.LastActivityDate);
            ltlLoginCount.Text = userInfo.CountOfLogin.ToString();
            ltlCreationDate.Text = DateUtils.GetDateAndTimeString(userInfo.CreateDate);

            hlEditLink.NavigateUrl = PageUserAdd.GetRedirectUrlToEdit(userInfo.UserId, GetRedirectUrl());
            hlChangePassword.Attributes.Add("onclick", ModalUserPassword.GetOpenWindowString(userInfo.UserName));
            ltlSelect.Text = $@"<input type=""checkbox"" name=""UserIDCollection"" value=""{userInfo.UserId}"" />";

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
            else if (ConfigManager.UserConfigInfo.IsLoginFailToLock &&
                       ConfigManager.UserConfigInfo.LoginFailToLockCount <= userInfo.CountOfFailedLogin)
            {
                if (_lockType == EUserLockType.Forever)
                {
                    state = @"<span style=""color:red;"">[已被锁定]</span>";
                }
                else
                {
                    var ts = new TimeSpan(DateTime.Now.Ticks - userInfo.LastActivityDate.Ticks);
                    var hours = Convert.ToInt32(ConfigManager.UserConfigInfo.LoginLockingHours - ts.TotalHours);
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
                        $"{GetRedirectUrl()}?GroupID={DdlGroup.SelectedValue}&PageNum={DdlPageNum.SelectedValue}&Keyword={TbKeyword.Text}&CreationDate={DdlCreationDate.SelectedValue}&LastActivityDate={DdlLastActivityDate.SelectedValue}&loginCount={DdlLoginCount.SelectedValue}&SearchType={DdlSearchType.SelectedValue}";
                }
                return _pageUrl;
            }
        }
    }
}
