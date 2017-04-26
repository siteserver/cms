using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;

namespace SiteServer.BackgroundPages.User
{
    public class ModalUserView : BasePage
    {
        protected Literal ltlUserID;
        protected Literal ltlUserName;
        protected Literal ltlGroup;
        protected Literal ltlDisplayName;
        protected Literal ltlCreateDate;
        protected Literal ltlLastResetPasswordDate;
        protected Literal ltlLastActivityDate;
        protected Literal ltlEmail;
        protected Literal ltlMobile;
        protected Literal ltlLoginCount;
        protected Literal ltlWritingCount;
        protected Literal ltlOrganization;
        protected Literal ltlDepartment;
        protected Literal ltlPosition;
        protected Literal ltlGender;
        protected Literal ltlBirthday;
        protected Literal ltlEducation;
        protected Literal ltlGraduation;
        protected Literal ltlAddress;
        protected Literal ltlWeiXin;
        protected Literal ltlQQ;
        protected Literal ltlWeiBo;
        protected Literal ltlInterests;
        protected Literal ltlSignature;

        private UserInfo _userInfo;

        public static string GetOpenWindowString(string userName)
        {
            return PageUtils.GetOpenWindowString("查看用户信息", PageUtils.GetUserUrl(nameof(ModalUserView), new NameValueCollection
            {
                {"UserName", userName}
            }), 700, 560, true);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var userName = Request.QueryString["UserName"];
            _userInfo = BaiRongDataProvider.UserDao.GetUserInfoByAccount(userName);

            ltlUserID.Text = _userInfo.UserId.ToString();
            ltlUserName.Text = _userInfo.UserName;
            ltlGroup.Text = UserGroupManager.GetGroupName(_userInfo.GroupId);
            ltlDisplayName.Text = _userInfo.DisplayName;
            ltlCreateDate.Text = DateUtils.GetDateAndTimeString(_userInfo.CreateDate);
            ltlLastActivityDate.Text = DateUtils.GetDateAndTimeString(_userInfo.LastActivityDate);
            ltlLastResetPasswordDate.Text = DateUtils.GetDateAndTimeString(_userInfo.LastResetPasswordDate);
            ltlEmail.Text = _userInfo.Email;
            ltlMobile.Text = _userInfo.Mobile;
            ltlLoginCount.Text = _userInfo.CountOfLogin.ToString();
            ltlWritingCount.Text = _userInfo.CountOfWriting.ToString();
            ltlOrganization.Text = _userInfo.Organization;
            ltlDepartment.Text = _userInfo.Department;
            ltlPosition.Text = _userInfo.Position;
            ltlGender.Text = _userInfo.Gender;
            ltlBirthday.Text = _userInfo.Birthday;
            ltlEducation.Text = _userInfo.Education;
            ltlGraduation.Text = _userInfo.Graduation;
            ltlAddress.Text = _userInfo.Address;
            ltlWeiXin.Text = _userInfo.WeiXin;
            ltlQQ.Text = _userInfo.Qq;
            ltlWeiBo.Text = _userInfo.WeiBo;
            ltlInterests.Text = _userInfo.Interests;
            ltlSignature.Text = _userInfo.Signature;
        }
    }
}
