using System;
using System.Web.UI.WebControls;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Repositories;

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

            PageUtils.CheckRequestParameter("siteId");

            if (IsPostBack) return;

            VerifySitePermissions(Constants.WebSitePermissions.Configuration);

            TbVideoUploadDirectoryName.Text = Site.VideoUploadDirectoryName;

            DdlVideoUploadDateFormatString.Items.Add(new ListItem("按年存入不同目录(不推荐)", EDateFormatTypeUtils.GetValue(EDateFormatType.Year)));
            DdlVideoUploadDateFormatString.Items.Add(new ListItem("按年/月存入不同目录", EDateFormatTypeUtils.GetValue(EDateFormatType.Month)));
            DdlVideoUploadDateFormatString.Items.Add(new ListItem("按年/月/日存入不同目录", EDateFormatTypeUtils.GetValue(EDateFormatType.Day)));
            ControlUtils.SelectSingleItemIgnoreCase(DdlVideoUploadDateFormatString, Site.VideoUploadDateFormatString);

            EBooleanUtils.AddListItems(DdlIsVideoUploadChangeFileName, "自动修改文件名", "保持文件名不变");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsVideoUploadChangeFileName, Site.IsVideoUploadChangeFileName.ToString());

            TbVideoUploadTypeCollection.Text = Site.VideoUploadTypeCollection.Replace("|", ",");
            var mbSize = GetMbSize(Site.VideoUploadTypeMaxSize);
            if (mbSize == 0)
            {
                DdlVideoUploadTypeUnit.SelectedIndex = 0;
                TbVideoUploadTypeMaxSize.Text = Site.VideoUploadTypeMaxSize.ToString();
            }
            else
            {
                DdlVideoUploadTypeUnit.SelectedIndex = 1;
                TbVideoUploadTypeMaxSize.Text = mbSize.ToString();
            }
        }

		private static int GetMbSize(int kbSize)
		{
			var retVal = 0;
			if (kbSize >= 1024 && ((kbSize % 1024) == 0))
			{
				retVal = kbSize / 1024;
			}
			return retVal;
		}

		public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                Site.VideoUploadDirectoryName = TbVideoUploadDirectoryName.Text;

                Site.VideoUploadDateFormatString = EDateFormatTypeUtils.GetValue(EDateFormatTypeUtils.GetEnumType(DdlVideoUploadDateFormatString.SelectedValue));
                Site.IsVideoUploadChangeFileName = TranslateUtils.ToBool(DdlIsVideoUploadChangeFileName.SelectedValue);

                Site.VideoUploadTypeCollection = TbVideoUploadTypeCollection.Text.Replace(",", "|");
                var kbSize = int.Parse(TbVideoUploadTypeMaxSize.Text);
                Site.VideoUploadTypeMaxSize = (DdlVideoUploadTypeUnit.SelectedIndex == 0) ? kbSize : 1024 * kbSize;
				
				try
				{
                    DataProvider.SiteRepository.UpdateAsync(Site).GetAwaiter().GetResult();

                    AuthRequest.AddSiteLogAsync(SiteId, "修改视频上传设置").GetAwaiter().GetResult();

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
