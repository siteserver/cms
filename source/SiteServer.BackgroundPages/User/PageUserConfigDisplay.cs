using System;
using System.Collections;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Text.LitJson;

namespace SiteServer.BackgroundPages.User
{
    public class PageUserConfigDisplay : BasePage
    {
        public DropDownList DdlIsEnable;
        public PlaceHolder PhOpen;
        public TextBox TbTitle;
        public TextBox TbCopyright;
        public TextBox TbBeianNo;
        public Literal LtlLogoUrl;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Body.IsQueryExists("uploadLogo"))
            {
                var attributes = UploadLogo();
                var json = JsonMapper.ToJson(attributes);
                Response.Write(json);
                Response.End();
                return;
            }
            if (IsPostBack) return;

            BreadCrumbUser(AppManager.User.LeftMenu.UserConfiguration, "基本配置", AppManager.User.Permission.UserConfiguration);

            EBooleanUtils.AddListItems(DdlIsEnable, "开启", "关闭");
            ControlUtils.SelectListItemsIgnoreCase(DdlIsEnable, ConfigManager.UserConfigInfo.IsEnable.ToString());

            TbTitle.Text = ConfigManager.UserConfigInfo.Title;
            TbCopyright.Text = ConfigManager.UserConfigInfo.Copyright;
            TbBeianNo.Text = ConfigManager.UserConfigInfo.BeianNo;

            LtlLogoUrl.Text = $@"<img id=""logoUrl"" src=""{ConfigManager.UserConfigInfo.LogoUrl}"" />";
            PhOpen.Visible = ConfigManager.UserConfigInfo.IsEnable;
        }

        public void DdlIsEnable_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhOpen.Visible = TranslateUtils.ToBool(DdlIsEnable.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                ConfigManager.UserConfigInfo.Title = TbTitle.Text;
                ConfigManager.UserConfigInfo.Copyright = TbCopyright.Text;
                ConfigManager.UserConfigInfo.BeianNo = TbBeianNo.Text;

                ConfigManager.UserConfigInfo.IsEnable = TranslateUtils.ToBool(DdlIsEnable.SelectedValue);

                try
                {
                    BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);
                    Body.AddAdminLog("修改用户中心显示设置");
                    SuccessMessage("显示设置修改成功！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "显示设置修改失败！");
                }
            }
        }

        private Hashtable UploadLogo()
        {
            var success = false;
            var message = string.Empty;

            if (Request.Files["Filedata"] != null)
            {
                var postedFile = Request.Files["Filedata"];
                try
                {
                    if (!string.IsNullOrEmpty(postedFile?.FileName))
                    {
                        var filePath = postedFile.FileName;
                        var fileExtName = filePath.ToLower().Substring(filePath.LastIndexOf(".", StringComparison.Ordinal) + 1);
                        var imageType = EImageTypeUtils.GetEnumType(fileExtName);
                        if (imageType != EImageType.Unknown)
                        {
                            string fileName = $"home_logo.{EImageTypeUtils.GetValue(imageType)}";
                            var logoPath = PathUtils.GetUserFilesPath(string.Empty, fileName);
                            postedFile.SaveAs(logoPath);
                            ConfigManager.UserConfigInfo.LogoUrl = PageUtils.AddProtocolToUrl(PageUtils.GetUserFilesUrl(string.Empty, fileName));
                            BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);

                            success = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }

            var jsonAttributes = new Hashtable();
            if (success)
            {
                jsonAttributes.Add("success", "true");
                jsonAttributes.Add("logoUrl", ConfigManager.UserConfigInfo.LogoUrl);
            }
            else
            {
                jsonAttributes.Add("success", "false");
                if (string.IsNullOrEmpty(message))
                {
                    message = "图标上传失败";
                }
                jsonAttributes.Add("message", message);
            }

            return jsonAttributes;
        }
    }
}
