using System;
using System.Web.UI.WebControls;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Repositories;

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
                VerifySitePermissions(Constants.SitePermissions.ConfigUpload);

                TbFileUploadDirectoryName.Text = Site.FileUploadDirectoryName;

                DdlFileUploadDateFormatString.Items.Add(new ListItem("按年存入不同目录", DateFormatType.Year.GetValue()));
                DdlFileUploadDateFormatString.Items.Add(new ListItem("按年/月存入不同目录", DateFormatType.Month.GetValue()));
                DdlFileUploadDateFormatString.Items.Add(new ListItem("按年/月/日存入不同目录", DateFormatType.Day.GetValue()));
                ControlUtils.SelectSingleItemIgnoreCase(DdlFileUploadDateFormatString, Site.FileUploadDateFormatString.GetValue());

				EBooleanUtils.AddListItems(DdlIsFileUploadChangeFileName, "自动修改文件名", "保持文件名不变");
                ControlUtils.SelectSingleItemIgnoreCase(DdlIsFileUploadChangeFileName, Site.IsFileUploadChangeFileName.ToString());

                TbFileUploadTypeCollection.Text = Site.FileUploadTypeCollection.Replace("|", ",");
                var mbSize = GetMbSize(Site.FileUploadTypeMaxSize);
				if (mbSize == 0)
				{
                    DdlFileUploadTypeUnit.SelectedIndex = 0;
                    TbFileUploadTypeMaxSize.Text = Site.FileUploadTypeMaxSize.ToString();
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
                Site.FileUploadDirectoryName = TbFileUploadDirectoryName.Text;

                Site.FileUploadDateFormatString = TranslateUtils.ToEnum(DdlFileUploadDateFormatString.SelectedValue, DateFormatType.Month);
                Site.IsFileUploadChangeFileName = TranslateUtils.ToBool(DdlIsFileUploadChangeFileName.SelectedValue);

                Site.FileUploadTypeCollection = TbFileUploadTypeCollection.Text.Replace(",", "|");
                var kbSize = int.Parse(TbFileUploadTypeMaxSize.Text);
                Site.FileUploadTypeMaxSize = (DdlFileUploadTypeUnit.SelectedIndex == 0) ? kbSize : 1024 * kbSize;
				
				try
				{
                    DataProvider.SiteRepository.UpdateAsync(Site).GetAwaiter().GetResult();

                    AuthRequest.AddSiteLogAsync(SiteId, "修改附件上传设置").GetAwaiter().GetResult();

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
