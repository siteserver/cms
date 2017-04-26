using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationUploadFile : BasePageCms
    {
		public TextBox tbFileUploadDirectoryName;
		public RadioButtonList rblFileUploadDateFormatString;
		public RadioButtonList rblIsFileUploadChangeFileName;
        public TextBox tbFileUploadTypeCollection;
        public DropDownList ddlFileUploadTypeUnit;
        public TextBox tbFileUploadTypeMaxSize;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if (!IsPostBack)
			{
                BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, AppManager.Cms.LeftMenu.Configuration.IdConfigurationUpload, "附件上传设置", AppManager.Cms.Permission.WebSite.Configration);

                tbFileUploadDirectoryName.Text = PublishmentSystemInfo.Additional.FileUploadDirectoryName;

				rblFileUploadDateFormatString.Items.Add(new ListItem("按年存入不同目录(不推荐)", EDateFormatTypeUtils.GetValue(EDateFormatType.Year)));
				rblFileUploadDateFormatString.Items.Add(new ListItem("按年/月存入不同目录", EDateFormatTypeUtils.GetValue(EDateFormatType.Month)));
				rblFileUploadDateFormatString.Items.Add(new ListItem("按年/月/日存入不同目录", EDateFormatTypeUtils.GetValue(EDateFormatType.Day)));
                ControlUtils.SelectListItemsIgnoreCase(rblFileUploadDateFormatString, PublishmentSystemInfo.Additional.FileUploadDateFormatString);

				EBooleanUtils.AddListItems(rblIsFileUploadChangeFileName, "自动修改文件名", "保持文件名不变");
                ControlUtils.SelectListItemsIgnoreCase(rblIsFileUploadChangeFileName, PublishmentSystemInfo.Additional.IsFileUploadChangeFileName.ToString());

                tbFileUploadTypeCollection.Text = PublishmentSystemInfo.Additional.FileUploadTypeCollection.Replace("|", ",");
                var mbSize = GetMBSize(PublishmentSystemInfo.Additional.FileUploadTypeMaxSize);
				if (mbSize == 0)
				{
                    ddlFileUploadTypeUnit.SelectedIndex = 0;
                    tbFileUploadTypeMaxSize.Text = PublishmentSystemInfo.Additional.FileUploadTypeMaxSize.ToString();
				}
				else
				{
                    ddlFileUploadTypeUnit.SelectedIndex = 1;
                    tbFileUploadTypeMaxSize.Text = mbSize.ToString();
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
                PublishmentSystemInfo.Additional.FileUploadDirectoryName = tbFileUploadDirectoryName.Text;

                PublishmentSystemInfo.Additional.FileUploadDateFormatString = EDateFormatTypeUtils.GetValue(EDateFormatTypeUtils.GetEnumType(rblFileUploadDateFormatString.SelectedValue));
                PublishmentSystemInfo.Additional.IsFileUploadChangeFileName = TranslateUtils.ToBool(rblIsFileUploadChangeFileName.SelectedValue);

                PublishmentSystemInfo.Additional.FileUploadTypeCollection = tbFileUploadTypeCollection.Text.Replace(",", "|");
                var kbSize = int.Parse(tbFileUploadTypeMaxSize.Text);
                PublishmentSystemInfo.Additional.FileUploadTypeMaxSize = (ddlFileUploadTypeUnit.SelectedIndex == 0) ? kbSize : 1024 * kbSize;
				
				try
				{
                    DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);

                    Body.AddSiteLog(PublishmentSystemId, "修改附件上传设置");

                    SuccessMessage("上传附件设置修改成功！");
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "上传附件设置修改失败！");
				}
			}
		}

	}
}
