using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Settings
{
    public class ModalUserView : BasePage
    {
        protected Literal LtlUserId;
        protected Literal LtlUserName;
        protected Literal LtlCreateDate;
        protected Literal LtlLoginCount;
        protected Literal LtlLastResetPasswordDate;
        protected Literal LtlLastActivityDate;
        protected Literal LtlAttributes;

        private UserInfo _userInfo;

        public static string GetOpenWindowString(string userName)
        {
            return LayerUtils.GetOpenScript("查看用户信息", PageUtils.GetSettingsUrl(nameof(ModalUserView), new NameValueCollection
            {
                {"UserName", userName}
            }));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var userName = Request.QueryString["UserName"];
            _userInfo = UserManager.GetUserInfoByUserName(userName);

            LtlUserId.Text = _userInfo.Id.ToString();
            LtlUserName.Text = _userInfo.UserName;
            LtlCreateDate.Text = DateUtils.GetDateAndTimeString(_userInfo.CreateDate);
            LtlLastActivityDate.Text = DateUtils.GetDateAndTimeString(_userInfo.LastActivityDate);
            LtlLastResetPasswordDate.Text = DateUtils.GetDateAndTimeString(_userInfo.LastResetPasswordDate);
            LtlLoginCount.Text = _userInfo.CountOfLogin.ToString();

            var builder = new StringBuilder();
            var sep = true;
            foreach (var styleInfo in TableStyleManager.GetUserStyleInfoList())
            {
                if (sep)
                {
                    builder.Append(@"<div class=""form-group form-row"">");
                }

                builder.Append($@"
<label class=""col-2 text-right col-form-label"">{styleInfo.DisplayName}</label>
<div class=""col-4 form-control-plaintext"">
    {_userInfo.Get(styleInfo.AttributeName)}
</div>
");

                if (!sep)
                {
                    builder.Append("</div>");
                }

                sep = !sep;
            }

            LtlAttributes.Text = builder.ToString();
        }
    }
}
