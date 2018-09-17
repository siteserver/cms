using System;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationUploadFile : BasePageCms
    {
		public TextBox TbFileUploadDirectoryName;
		public DropDownList DdlFileUploadDateFormatString;
		public DropDownList DdlIsFileUploadChangeFileName;
        public TextBox TbFileUploadTypeCollection;
        public DropDownList DdlFileUploadTypeUnit;
        public TextBox TbFileUploadTypeMaxSize;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

			if (!IsPostBack)
			{
                VerifySitePermissions(ConfigManager.WebSitePermissions.Configration);

                TbFileUploadDirectoryName.Text = SiteInfo.Additional.FileUploadDirectoryName;

                DdlFileUploadDateFormatString.Items.Add(new ListItem("按年存入不同目录(不推荐)", EDateFormatTypeUtils.GetValue(EDateFormatType.Year)));
                DdlFileUploadDateFormatString.Items.Add(new ListItem("按年/月存入不同目录", EDateFormatTypeUtils.GetValue(EDateFormatType.Month)));
                DdlFileUploadDateFormatString.Items.Add(new ListItem("按年/月/日存入不同目录", EDateFormatTypeUtils.GetValue(EDateFormatType.Day)));
                ControlUtils.SelectSingleItemIgnoreCase(DdlFileUploadDateFormatString, SiteInfo.Additional.FileUploadDateFormatString);

				EBooleanUtils.AddListItems(DdlIsFileUploadChangeFileName, "自动修改文件名", "保持文件名不变");
                ControlUtils.SelectSingleItemIgnoreCase(DdlIsFileUploadChangeFileName, SiteInfo.Additional.IsFileUploadChangeFileName.ToString());

                TbFileUploadTypeCollection.Text = SiteInfo.Additional.FileUploadTypeCollection.Replace("|", ",");
                var mbSize = GetMbSize(SiteInfo.Additional.FileUploadTypeMaxSize);
				if (mbSize == 0)
				{
                    DdlFileUploadTypeUnit.SelectedIndex = 0;
                    TbFileUploadTypeMaxSize.Text = SiteInfo.Additional.FileUploadTypeMaxSize.ToString();
				}
				else
				{
                    DdlFileUploadTypeUnit.SelectedIndex = 1;
                    TbFileUploadTypeMaxSize.Text = mbSize.ToString();
				}
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
                SiteInfo.Additional.FileUploadDirectoryName = TbFileUploadDirectoryName.Text;

                SiteInfo.Additional.FileUploadDateFormatString = EDateFormatTypeUtils.GetValue(EDateFormatTypeUtils.GetEnumType(DdlFileUploadDateFormatString.SelectedValue));
                SiteInfo.Additional.IsFileUploadChangeFileName = TranslateUtils.ToBool(DdlIsFileUploadChangeFileName.SelectedValue);

                SiteInfo.Additional.FileUploadTypeCollection = TbFileUploadTypeCollection.Text.Replace(",", "|");
                var kbSize = int.Parse(TbFileUploadTypeMaxSize.Text);
                SiteInfo.Additional.FileUploadTypeMaxSize = (DdlFileUploadTypeUnit.SelectedIndex == 0) ? kbSize : 1024 * kbSize;
				
				try
				{
                    DataProvider.SiteDao.Update(SiteInfo);

                    AuthRequest.AddSiteLog(SiteId, "修改附件上传设置");

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
