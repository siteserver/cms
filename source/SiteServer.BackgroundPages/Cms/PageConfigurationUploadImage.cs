using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationUploadImage : BasePageCms
    {
		public TextBox tbImageUploadDirectoryName;
		public RadioButtonList rblImageUploadDateFormatString;
		public RadioButtonList rblIsImageUploadChangeFileName;
        public TextBox tbImageUploadTypeCollection;
        public DropDownList ddlImageUploadTypeUnit;
        public TextBox tbImageUploadTypeMaxSize;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if (!IsPostBack)
			{
                BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, AppManager.Cms.LeftMenu.Configuration.IdConfigurationUpload, "图片上传设置", AppManager.Cms.Permission.WebSite.Configration);

                tbImageUploadDirectoryName.Text = PublishmentSystemInfo.Additional.ImageUploadDirectoryName;

				rblImageUploadDateFormatString.Items.Add(new ListItem("按年存入不同目录(不推荐)", EDateFormatTypeUtils.GetValue(EDateFormatType.Year)));
				rblImageUploadDateFormatString.Items.Add(new ListItem("按年/月存入不同目录", EDateFormatTypeUtils.GetValue(EDateFormatType.Month)));
				rblImageUploadDateFormatString.Items.Add(new ListItem("按年/月/日存入不同目录", EDateFormatTypeUtils.GetValue(EDateFormatType.Day)));
                ControlUtils.SelectListItemsIgnoreCase(rblImageUploadDateFormatString, PublishmentSystemInfo.Additional.ImageUploadDateFormatString);

				EBooleanUtils.AddListItems(rblIsImageUploadChangeFileName, "自动修改文件名", "保持文件名不变");
                ControlUtils.SelectListItemsIgnoreCase(rblIsImageUploadChangeFileName, PublishmentSystemInfo.Additional.IsImageUploadChangeFileName.ToString());

                tbImageUploadTypeCollection.Text = PublishmentSystemInfo.Additional.ImageUploadTypeCollection.Replace("|", ",");
                var mbSize = GetMBSize(PublishmentSystemInfo.Additional.ImageUploadTypeMaxSize);
				if (mbSize == 0)
				{
                    ddlImageUploadTypeUnit.SelectedIndex = 0;
                    tbImageUploadTypeMaxSize.Text = PublishmentSystemInfo.Additional.ImageUploadTypeMaxSize.ToString();
				}
				else
				{
                    ddlImageUploadTypeUnit.SelectedIndex = 1;
                    tbImageUploadTypeMaxSize.Text = mbSize.ToString();
				}
			}
		}


		private static int GetMBSize(int kbSize)
		{
			var retval = 0;
			if (kbSize >= 1024 && ((kbSize % 1024) == 0))
			{
				retval = kbSize / 1024;
			}
			return retval;
		}


		public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                PublishmentSystemInfo.Additional.ImageUploadDirectoryName = tbImageUploadDirectoryName.Text;

                PublishmentSystemInfo.Additional.ImageUploadDateFormatString = EDateFormatTypeUtils.GetValue(EDateFormatTypeUtils.GetEnumType(rblImageUploadDateFormatString.SelectedValue));
                PublishmentSystemInfo.Additional.IsImageUploadChangeFileName = TranslateUtils.ToBool(rblIsImageUploadChangeFileName.SelectedValue);

                PublishmentSystemInfo.Additional.ImageUploadTypeCollection = tbImageUploadTypeCollection.Text.Replace(",", "|");
                var kbSize = int.Parse(tbImageUploadTypeMaxSize.Text);
                PublishmentSystemInfo.Additional.ImageUploadTypeMaxSize = (ddlImageUploadTypeUnit.SelectedIndex == 0) ? kbSize : 1024 * kbSize;
				
				try
				{
                    DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);

                    Body.AddSiteLog(PublishmentSystemId, "修改图片上传设置");

                    SuccessMessage("上传图片设置修改成功！");
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "上传图片设置修改失败！");
				}
			}
		}

	}
}
