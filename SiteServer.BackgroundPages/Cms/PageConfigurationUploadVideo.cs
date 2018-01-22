using System;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.Utils.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationUploadVideo : BasePageCms
    {
		public TextBox TbVideoUploadDirectoryName;
		public DropDownList DdlVideoUploadDateFormatString;
		public DropDownList DdlIsVideoUploadChangeFileName;
        public TextBox TbVideoUploadTypeCollection;
        public DropDownList DdlVideoUploadTypeUnit;
        public TextBox TbVideoUploadTypeMaxSize;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (IsPostBack) return;

            VerifySitePermissions(AppManager.Permissions.WebSite.Configration);

            TbVideoUploadDirectoryName.Text = PublishmentSystemInfo.Additional.VideoUploadDirectoryName;

            DdlVideoUploadDateFormatString.Items.Add(new ListItem("按年存入不同目录(不推荐)", EDateFormatTypeUtils.GetValue(EDateFormatType.Year)));
            DdlVideoUploadDateFormatString.Items.Add(new ListItem("按年/月存入不同目录", EDateFormatTypeUtils.GetValue(EDateFormatType.Month)));
            DdlVideoUploadDateFormatString.Items.Add(new ListItem("按年/月/日存入不同目录", EDateFormatTypeUtils.GetValue(EDateFormatType.Day)));
            ControlUtils.SelectSingleItemIgnoreCase(DdlVideoUploadDateFormatString, PublishmentSystemInfo.Additional.VideoUploadDateFormatString);

            EBooleanUtils.AddListItems(DdlIsVideoUploadChangeFileName, "自动修改文件名", "保持文件名不变");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsVideoUploadChangeFileName, PublishmentSystemInfo.Additional.IsVideoUploadChangeFileName.ToString());

            TbVideoUploadTypeCollection.Text = PublishmentSystemInfo.Additional.VideoUploadTypeCollection.Replace("|", ",");
            var mbSize = GetMbSize(PublishmentSystemInfo.Additional.VideoUploadTypeMaxSize);
            if (mbSize == 0)
            {
                DdlVideoUploadTypeUnit.SelectedIndex = 0;
                TbVideoUploadTypeMaxSize.Text = PublishmentSystemInfo.Additional.VideoUploadTypeMaxSize.ToString();
            }
            else
            {
                DdlVideoUploadTypeUnit.SelectedIndex = 1;
                TbVideoUploadTypeMaxSize.Text = mbSize.ToString();
            }
        }

		private static int GetMbSize(int kbSize)
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
                PublishmentSystemInfo.Additional.VideoUploadDirectoryName = TbVideoUploadDirectoryName.Text;

                PublishmentSystemInfo.Additional.VideoUploadDateFormatString = EDateFormatTypeUtils.GetValue(EDateFormatTypeUtils.GetEnumType(DdlVideoUploadDateFormatString.SelectedValue));
                PublishmentSystemInfo.Additional.IsVideoUploadChangeFileName = TranslateUtils.ToBool(DdlIsVideoUploadChangeFileName.SelectedValue);

                PublishmentSystemInfo.Additional.VideoUploadTypeCollection = TbVideoUploadTypeCollection.Text.Replace(",", "|");
                var kbSize = int.Parse(TbVideoUploadTypeMaxSize.Text);
                PublishmentSystemInfo.Additional.VideoUploadTypeMaxSize = (DdlVideoUploadTypeUnit.SelectedIndex == 0) ? kbSize : 1024 * kbSize;
				
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
