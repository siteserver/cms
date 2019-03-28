using System;
using System.Web.UI.WebControls;
using SiteServer.CMS.Caches;
using SiteServer.Utils;
using SiteServer.CMS.Database.Core;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationUploadImage : BasePageCms
    {
		public TextBox TbImageUploadDirectoryName;
		public DropDownList DdlImageUploadDateFormatString;
		public DropDownList DdlIsImageUploadChangeFileName;
        public TextBox TbImageUploadTypeCollection;
        public DropDownList DdlImageUploadTypeUnit;
        public TextBox TbImageUploadTypeMaxSize;

        public TextBox TbPhotoSmallWidth;
        public TextBox TbPhotoMiddleWidth;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            PageUtils.CheckRequestParameter("siteId");

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Configration);

            TbImageUploadDirectoryName.Text = SiteInfo.ImageUploadDirectoryName;

            DdlImageUploadDateFormatString.Items.Add(new ListItem("按年存入不同目录(不推荐)", EDateFormatTypeUtils.GetValue(EDateFormatType.Year)));
            DdlImageUploadDateFormatString.Items.Add(new ListItem("按年/月存入不同目录", EDateFormatTypeUtils.GetValue(EDateFormatType.Month)));
            DdlImageUploadDateFormatString.Items.Add(new ListItem("按年/月/日存入不同目录", EDateFormatTypeUtils.GetValue(EDateFormatType.Day)));
            ControlUtils.SelectSingleItemIgnoreCase(DdlImageUploadDateFormatString, SiteInfo.ImageUploadDateFormatString);

            EBooleanUtils.AddListItems(DdlIsImageUploadChangeFileName, "自动修改文件名", "保持文件名不变");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsImageUploadChangeFileName, SiteInfo.IsImageUploadChangeFileName.ToString());

            TbImageUploadTypeCollection.Text = SiteInfo.ImageUploadTypeCollection.Replace("|", ",");
            var mbSize = GetMbSize(SiteInfo.ImageUploadTypeMaxSize);
            if (mbSize == 0)
            {
                DdlImageUploadTypeUnit.SelectedIndex = 0;
                TbImageUploadTypeMaxSize.Text = SiteInfo.ImageUploadTypeMaxSize.ToString();
            }
            else
            {
                DdlImageUploadTypeUnit.SelectedIndex = 1;
                TbImageUploadTypeMaxSize.Text = mbSize.ToString();
            }

            TbPhotoSmallWidth.Text = SiteInfo.PhotoSmallWidth.ToString();
            TbPhotoMiddleWidth.Text = SiteInfo.PhotoMiddleWidth.ToString();
        }

		private static int GetMbSize(int kbSize)
		{
			var retval = 0;
			if (kbSize >= 1024 && kbSize % 1024 == 0)
			{
				retval = kbSize / 1024;
			}
			return retval;
		}

		public override void Submit_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid) return;

		    SiteInfo.ImageUploadDirectoryName = TbImageUploadDirectoryName.Text;

		    SiteInfo.ImageUploadDateFormatString = EDateFormatTypeUtils.GetValue(EDateFormatTypeUtils.GetEnumType(DdlImageUploadDateFormatString.SelectedValue));
		    SiteInfo.IsImageUploadChangeFileName = TranslateUtils.ToBool(DdlIsImageUploadChangeFileName.SelectedValue);

		    SiteInfo.ImageUploadTypeCollection = TbImageUploadTypeCollection.Text.Replace(",", "|");
		    var kbSize = int.Parse(TbImageUploadTypeMaxSize.Text);
		    SiteInfo.ImageUploadTypeMaxSize = DdlImageUploadTypeUnit.SelectedIndex == 0 ? kbSize : 1024 * kbSize;

            SiteInfo.PhotoSmallWidth = TranslateUtils.ToInt(TbPhotoSmallWidth.Text, SiteInfo.PhotoSmallWidth);
            SiteInfo.PhotoMiddleWidth = TranslateUtils.ToInt(TbPhotoMiddleWidth.Text, SiteInfo.PhotoMiddleWidth);

            try
		    {
		        DataProvider.Site.Update(SiteInfo);

		        AuthRequest.AddSiteLog(SiteId, "修改图片上传设置");

		        SuccessMessage("上传图片设置修改成功！");
		    }
		    catch(Exception ex)
		    {
		        FailMessage(ex, "上传图片设置修改失败！");
		    }
		}

	}
}
