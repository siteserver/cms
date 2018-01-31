using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Settings
{
    public class ModalUserView : BasePage
    {
        protected Literal LtlUserName;
        protected Literal LtlDisplayName;
        protected Literal LtlCreateDate;
        protected Literal LtlLastResetPasswordDate;
        protected Literal LtlLastActivityDate;
        protected Literal LtlEmail;
        protected Literal LtlMobile;
        protected Literal LtlLoginCount;
        protected Literal LtlWritingCount;
        protected Literal LtlOrganization;
        protected Literal LtlDepartment;
        protected Literal LtlPosition;
        protected Literal LtlGender;
        protected Literal LtlBirthday;
        protected Literal LtlEducation;
        protected Literal LtlGraduation;
        protected Literal LtlAddress;
        protected Literal LtlWeiXin;
        protected Literal LtlQq;
        protected Literal LtlWeiBo;
        protected Literal LtlInterests;
        protected Literal LtlSignature;

        private UserInfo _userInfo;

        public static string GetOpenWindowString(string userName)
        {
            return LayerUtils.GetOpenScript("查看用户信息", PageUtils.GetSettingsUrl(nameof(ModalUserView), new NameValueCollection
            {
                {"UserName", userName}
            }), 700, 560);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var userName = Request.QueryString["UserName"];
            _userInfo = DataProvider.UserDao.GetUserInfoByAccount(userName);

            LtlUserName.Text = _userInfo.UserName;
            LtlDisplayName.Text = _userInfo.DisplayName;
            LtlCreateDate.Text = DateUtils.GetDateAndTimeString(_userInfo.CreateDate);
            LtlLastActivityDate.Text = DateUtils.GetDateAndTimeString(_userInfo.LastActivityDate);
            LtlLastResetPasswordDate.Text = DateUtils.GetDateAndTimeString(_userInfo.LastResetPasswordDate);
            LtlEmail.Text = _userInfo.Email;
            LtlMobile.Text = _userInfo.Mobile;
            LtlLoginCount.Text = _userInfo.CountOfLogin.ToString();
            LtlWritingCount.Text = _userInfo.CountOfWriting.ToString();
            LtlOrganization.Text = _userInfo.Organization;
            LtlDepartment.Text = _userInfo.Department;
            LtlPosition.Text = _userInfo.Position;
            LtlGender.Text = _userInfo.Gender;
            LtlBirthday.Text = _userInfo.Birthday;
            LtlEducation.Text = _userInfo.Education;
            LtlGraduation.Text = _userInfo.Graduation;
            LtlAddress.Text = _userInfo.Address;
            LtlWeiXin.Text = _userInfo.WeiXin;
            LtlQq.Text = _userInfo.Qq;
            LtlWeiBo.Text = _userInfo.WeiBo;
            LtlInterests.Text = _userInfo.Interests;
            LtlSignature.Text = _userInfo.Signature;
        }
    }
}
