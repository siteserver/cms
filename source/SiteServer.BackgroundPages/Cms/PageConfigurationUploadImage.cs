using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationUploadImage : BasePageCms
    {
		public TextBox TbImageUploadDirectoryName;
		public RadioButtonList RblImageUploadDateFormatString;
		public RadioButtonList RblIsImageUploadChangeFileName;
        public TextBox TbImageUploadTypeCollection;
        public DropDownList DdlImageUploadTypeUnit;
        public TextBox TbImageUploadTypeMaxSize;

        public TextBox TbPhotoSmallWidth;
        public TextBox TbPhotoMiddleWidth;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            PageUtils.CheckRequestParameter("PublishmentSystemID");
            if (IsPostBack) return;

            BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, "图片上传设置", AppManager.Permissions.WebSite.Configration);

            TbImageUploadDirectoryName.Text = PublishmentSystemInfo.Additional.ImageUploadDirectoryName;

            RblImageUploadDateFormatString.Items.Add(new ListItem("按年存入不同目录(不推荐)", EDateFormatTypeUtils.GetValue(EDateFormatType.Year)));
            RblImageUploadDateFormatString.Items.Add(new ListItem("按年/月存入不同目录", EDateFormatTypeUtils.GetValue(EDateFormatType.Month)));
            RblImageUploadDateFormatString.Items.Add(new ListItem("按年/月/日存入不同目录", EDateFormatTypeUtils.GetValue(EDateFormatType.Day)));
            ControlUtils.SelectListItemsIgnoreCase(RblImageUploadDateFormatString, PublishmentSystemInfo.Additional.ImageUploadDateFormatString);

            EBooleanUtils.AddListItems(RblIsImageUploadChangeFileName, "自动修改文件名", "保持文件名不变");
            ControlUtils.SelectListItemsIgnoreCase(RblIsImageUploadChangeFileName, PublishmentSystemInfo.Additional.IsImageUploadChangeFileName.ToString());

            TbImageUploadTypeCollection.Text = PublishmentSystemInfo.Additional.ImageUploadTypeCollection.Replace("|", ",");
            var mbSize = GetMbSize(PublishmentSystemInfo.Additional.ImageUploadTypeMaxSize);
            if (mbSize == 0)
            {
                DdlImageUploadTypeUnit.SelectedIndex = 0;
                TbImageUploadTypeMaxSize.Text = PublishmentSystemInfo.Additional.ImageUploadTypeMaxSize.ToString();
            }
            else
            {
                DdlImageUploadTypeUnit.SelectedIndex = 1;
                TbImageUploadTypeMaxSize.Text = mbSize.ToString();
            }

            TbPhotoSmallWidth.Text = PublishmentSystemInfo.Additional.PhotoSmallWidth.ToString();
            TbPhotoMiddleWidth.Text = PublishmentSystemInfo.Additional.PhotoMiddleWidth.ToString();
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

		    PublishmentSystemInfo.Additional.ImageUploadDirectoryName = TbImageUploadDirectoryName.Text;

		    PublishmentSystemInfo.Additional.ImageUploadDateFormatString = EDateFormatTypeUtils.GetValue(EDateFormatTypeUtils.GetEnumType(RblImageUploadDateFormatString.SelectedValue));
		    PublishmentSystemInfo.Additional.IsImageUploadChangeFileName = TranslateUtils.ToBool(RblIsImageUploadChangeFileName.SelectedValue);

		    PublishmentSystemInfo.Additional.ImageUploadTypeCollection = TbImageUploadTypeCollection.Text.Replace(",", "|");
		    var kbSize = int.Parse(TbImageUploadTypeMaxSize.Text);
		    PublishmentSystemInfo.Additional.ImageUploadTypeMaxSize = DdlImageUploadTypeUnit.SelectedIndex == 0 ? kbSize : 1024 * kbSize;

            PublishmentSystemInfo.Additional.PhotoSmallWidth = TranslateUtils.ToInt(TbPhotoSmallWidth.Text, PublishmentSystemInfo.Additional.PhotoSmallWidth);
            PublishmentSystemInfo.Additional.PhotoMiddleWidth = TranslateUtils.ToInt(TbPhotoMiddleWidth.Text, PublishmentSystemInfo.Additional.PhotoMiddleWidth);

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
