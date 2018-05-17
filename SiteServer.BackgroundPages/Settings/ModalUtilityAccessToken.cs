using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Settings
{
	public class ModalUtilityAccessToken : BasePage
	{
		public Literal LtlTitle;
		public Literal LtlToken;
	    public Literal LtlAddDate;
	    public Literal LtlUpdatedDate;

        private int _id;

        public static string GetOpenWindowString(int id)
        {
            return LayerUtils.GetOpenScript("Access Token", PageUtils.GetSettingsUrl(nameof(ModalUtilityAccessToken), new NameValueCollection
            {
                {"id", id.ToString()}
            }), 0, 420);
        }
        
		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _id = AuthRequest.GetQueryInt("id");

            if (IsPostBack) return;

            var tokenInfo = DataProvider.AccessTokenDao.GetAccessTokenInfo(_id);

            LtlTitle.Text = tokenInfo.Title;
            LtlToken.Text = TranslateUtils.DecryptStringBySecretKey(tokenInfo.Token);
            LtlAddDate.Text = DateUtils.GetDateAndTimeString(tokenInfo.AddDate);
            LtlUpdatedDate.Text = DateUtils.GetDateAndTimeString(tokenInfo.UpdatedDate);
        }

        public void Regenerate_OnClick(object sender, EventArgs e)
        {
            if (!IsPostBack || !IsValid) return;

            try
            {
                LtlToken.Text = TranslateUtils.DecryptStringBySecretKey(DataProvider.AccessTokenDao.Regenerate(_id));
                LtlUpdatedDate.Text = DateUtils.GetDateAndTimeString(DateTime.Now);

                AuthRequest.AddAdminLog("重设 Access Token");

                SuccessMessage("Access Token 重新设置成功！");
            }
            catch(Exception ex)
            {
                FailMessage(ex, "Access Token 重新设置失败！");
            }
        }
    }
}
