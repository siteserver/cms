using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageAdminAccessTokensAdd : BasePage
    {
        public static readonly string PageUrl = PageUtils.GetSettingsUrl(nameof(PageAdminAccessTokensAdd));

        public Literal LtlPageTitle;
        public TextBox TbTitle;
        public DropDownList DdlAdministrators;
        public CheckBoxList CblScopes;

        private int _id;

        public static string GetRedirectUrlToAdd()
        {
            return PageUtils.GetSettingsUrl(nameof(PageAdminAccessTokensAdd), null);
        }

        public static string GetRedirectUrlToEdit(int id)
        {
            return PageUtils.GetSettingsUrl(nameof(PageAdminAccessTokensAdd), new NameValueCollection
            {
                {"id", id.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _id = AuthRequest.GetQueryInt("id");

            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Admin);

            LtlPageTitle.Text = _id > 0 ? "修改API密钥" : "新增API密钥";

            if (AuthRequest.AdminPermissions.IsConsoleAdministrator)
            {
                var userNameList = DataProvider.AdministratorDao.GetUserNameList();

                foreach (var userName in userNameList)
                {
                    DdlAdministrators.Items.Add(new ListItem(userName, userName));
                }

                ControlUtils.SelectSingleItem(DdlAdministrators, AuthRequest.AdminName);
            }
            else
            {
                DdlAdministrators.Items.Add(new ListItem(AuthRequest.AdminName, AuthRequest.AdminName));
            }

            foreach (var scope in AccessTokenManager.ScopeList)
            {
                CblScopes.Items.Add(new ListItem(scope, scope));
            }

            if (_id > 0)
            {
                var tokenInfo = DataProvider.AccessTokenDao.GetAccessTokenInfo(_id);

                TbTitle.Text = tokenInfo.Title;

                ControlUtils.SelectSingleItem(DdlAdministrators, tokenInfo.AdminName);

                var scopes = TranslateUtils.StringCollectionToStringList(tokenInfo.Scopes);
                ControlUtils.SelectMultiItemsIgnoreCase(CblScopes, scopes);
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            if (_id > 0)
            {
                var tokenInfo = DataProvider.AccessTokenDao.GetAccessTokenInfo(_id);

                if (tokenInfo.Title != TbTitle.Text && DataProvider.AccessTokenDao.IsTitleExists(TbTitle.Text))
                {
                    FailMessage("保存失败，已存在相同标题的API密钥！");
                    return;
                }

                tokenInfo.Title = TbTitle.Text;

                tokenInfo.AdminName = DdlAdministrators.SelectedValue;

                var scopes = ControlUtils.GetSelectedListControlValueStringList(CblScopes);
                tokenInfo.Scopes = TranslateUtils.ObjectCollectionToString(scopes);

                DataProvider.AccessTokenDao.Update(tokenInfo);

                AuthRequest.AddAdminLog("修改API密钥", $"Access Token:{tokenInfo.Title}");

                SuccessMessage("API密钥修改成功！");
                //AddWaitAndRedirectScript(PageAdminAccessTokens.PageUrl);
            }
            else
            {
                if (DataProvider.AccessTokenDao.IsTitleExists(TbTitle.Text))
                {
                    FailMessage("保存失败，已存在相同标题的API密钥！");
                    return;
                }

                var scopes = ControlUtils.GetSelectedListControlValueStringList(CblScopes);

                var tokenInfo = new AccessTokenInfo
                {
                    Title = TbTitle.Text,
                    AdminName = DdlAdministrators.SelectedValue,
                    Scopes = TranslateUtils.ObjectCollectionToString(scopes)
                };

                DataProvider.AccessTokenDao.Insert(tokenInfo);

                AuthRequest.AddAdminLog("新增API密钥", $"Access Token:{tokenInfo.Title}");

                SuccessMessage("API密钥新增成功！");
                //AddWaitAndRedirectScript(PageAdminAccessTokens.PageUrl);
            }
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            //PageUtils.Redirect(PageAdminAccessTokens.PageUrl);
        }
    }
}
