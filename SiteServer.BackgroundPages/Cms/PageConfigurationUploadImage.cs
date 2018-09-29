using System;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
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

            TbImageUploadDirectoryName.Text = SiteInfo.Additional.ImageUploadDirectoryName;

            DdlImageUploadDateFormatString.Items.Add(new ListItem("按年存入不同目录(不推荐)", EDateFormatTypeUtils.GetValue(EDateFormatType.Year)));
            DdlImageUploadDateFormatString.Items.Add(new ListItem("按年/月存入不同目录", EDateFormatTypeUtils.GetValue(EDateFormatType.Month)));
            DdlImageUploadDateFormatString.Items.Add(new ListItem("按年/月/日存入不同目录", EDateFormatTypeUtils.GetValue(EDateFormatType.Day)));
            ControlUtils.SelectSingleItemIgnoreCase(DdlImageUploadDateFormatString, SiteInfo.Additional.ImageUploadDateFormatString);

            EBooleanUtils.AddListItems(DdlIsImageUploadChangeFileName, "自动修改文件名", "保持文件名不变");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsImageUploadChangeFileName, SiteInfo.Additional.IsImageUploadChangeFileName.ToString());

            TbImageUploadTypeCollection.Text = SiteInfo.Additional.ImageUploadTypeCollection.Replace("|", ",");
            var mbSize = GetMbSize(SiteInfo.Additional.ImageUploadTypeMaxSize);
            if (mbSize == 0)
            {
                DdlImageUploadTypeUnit.SelectedIndex = 0;
                TbImageUploadTypeMaxSize.Text = SiteInfo.Additional.ImageUploadTypeMaxSize.ToString();
            }
            else
            {
                DdlImageUploadTypeUnit.SelectedIndex = 1;
                TbImageUploadTypeMaxSize.Text = mbSize.ToString();
            }

            TbPhotoSmallWidth.Text = SiteInfo.Additional.PhotoSmallWidth.ToString();
            TbPhotoMiddleWidth.Text = SiteInfo.Additional.PhotoMiddleWidth.ToString();
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

		    SiteInfo.Additional.ImageUploadDirectoryName = TbImageUploadDirectoryName.Text;

		    SiteInfo.Additional.ImageUploadDateFormatString = EDateFormatTypeUtils.GetValue(EDateFormatTypeUtils.GetEnumType(DdlImageUploadDateFormatString.SelectedValue));
		    SiteInfo.Additional.IsImageUploadChangeFileName = TranslateUtils.ToBool(DdlIsImageUploadChangeFileName.SelectedValue);

		    SiteInfo.Additional.ImageUploadTypeCollection = TbImageUploadTypeCollection.Text.Replace(",", "|");
		    var kbSize = int.Parse(TbImageUploadTypeMaxSize.Text);
		    SiteInfo.Additional.ImageUploadTypeMaxSize = DdlImageUploadTypeUnit.SelectedIndex == 0 ? kbSize : 1024 * kbSize;

            SiteInfo.Additional.PhotoSmallWidth = TranslateUtils.ToInt(TbPhotoSmallWidth.Text, SiteInfo.Additional.PhotoSmallWidth);
            SiteInfo.Additional.PhotoMiddleWidth = TranslateUtils.ToInt(TbPhotoMiddleWidth.Text, SiteInfo.Additional.PhotoMiddleWidth);

            try
		    {
		        DataProvider.SiteDao.Update(SiteInfo);

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
