using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationUploadVideo : BasePageCms
    {
		public TextBox tbVideoUploadDirectoryName;
		public RadioButtonList rblVideoUploadDateFormatString;
		public RadioButtonList rblIsVideoUploadChangeFileName;
        public TextBox tbVideoUploadTypeCollection;
        public DropDownList ddlVideoUploadTypeUnit;
        public TextBox tbVideoUploadTypeMaxSize;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if (!IsPostBack)
			{
                BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, AppManager.Cms.LeftMenu.Configuration.IdConfigurationUpload, "视频上传设置", AppManager.Cms.Permission.WebSite.Configration);

                tbVideoUploadDirectoryName.Text = PublishmentSystemInfo.Additional.VideoUploadDirectoryName;

				rblVideoUploadDateFormatString.Items.Add(new ListItem("按年存入不同目录(不推荐)", EDateFormatTypeUtils.GetValue(EDateFormatType.Year)));
				rblVideoUploadDateFormatString.Items.Add(new ListItem("按年/月存入不同目录", EDateFormatTypeUtils.GetValue(EDateFormatType.Month)));
				rblVideoUploadDateFormatString.Items.Add(new ListItem("按年/月/日存入不同目录", EDateFormatTypeUtils.GetValue(EDateFormatType.Day)));
                ControlUtils.SelectListItemsIgnoreCase(rblVideoUploadDateFormatString, PublishmentSystemInfo.Additional.VideoUploadDateFormatString);

				EBooleanUtils.AddListItems(rblIsVideoUploadChangeFileName, "自动修改文件名", "保持文件名不变");
                ControlUtils.SelectListItemsIgnoreCase(rblIsVideoUploadChangeFileName, PublishmentSystemInfo.Additional.IsVideoUploadChangeFileName.ToString());

                tbVideoUploadTypeCollection.Text = PublishmentSystemInfo.Additional.VideoUploadTypeCollection.Replace("|", ",");
                var mbSize = GetMBSize(PublishmentSystemInfo.Additional.VideoUploadTypeMaxSize);
				if (mbSize == 0)
				{
                    ddlVideoUploadTypeUnit.SelectedIndex = 0;
                    tbVideoUploadTypeMaxSize.Text = PublishmentSystemInfo.Additional.VideoUploadTypeMaxSize.ToString();
				}
				else
				{
                    ddlVideoUploadTypeUnit.SelectedIndex = 1;
                    tbVideoUploadTypeMaxSize.Text = mbSize.ToString();
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
                PublishmentSystemInfo.Additional.VideoUploadDirectoryName = tbVideoUploadDirectoryName.Text;

                PublishmentSystemInfo.Additional.VideoUploadDateFormatString = EDateFormatTypeUtils.GetValue(EDateFormatTypeUtils.GetEnumType(rblVideoUploadDateFormatString.SelectedValue));
                PublishmentSystemInfo.Additional.IsVideoUploadChangeFileName = TranslateUtils.ToBool(rblIsVideoUploadChangeFileName.SelectedValue);

                PublishmentSystemInfo.Additional.VideoUploadTypeCollection = tbVideoUploadTypeCollection.Text.Replace(",", "|");
                var kbSize = int.Parse(tbVideoUploadTypeMaxSize.Text);
                PublishmentSystemInfo.Additional.VideoUploadTypeMaxSize = (ddlVideoUploadTypeUnit.SelectedIndex == 0) ? kbSize : 1024 * kbSize;
				
				try
				{
                    DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);

                    Body.AddSiteLog(PublishmentSystemId, "修改视频上传设置");

                    SuccessMessage("上传视频设置修改成功！");
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "上传视频设置修改失败！");
				}
			}
		}

	}
}
