using System;
using System.Drawing.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationWaterMark : BasePageCms
    {
		public RadioButtonList IsWaterMark;
		public Literal WaterMarkPosition;
		public Control WaterMarkPositionRow;
		public DropDownList WaterMarkTransparency;
		public Control WaterMarkTransparencyRow;
		public TextBox WaterMarkMinWidth;
		public TextBox WaterMarkMinHeight;
		public Control WaterMarkMinRow;
		public RadioButtonList IsImageWaterMark;
		public Control IsImageWaterMarkRow;
		public TextBox WaterMarkFormatString;
		public Control WaterMarkFormatStringRow;
		public DropDownList WaterMarkFontName;
		public Control WaterMarkFontNameRow;
		public TextBox WaterMarkFontSize;
		public Control WaterMarkFontSizeRow;
		public TextBox WaterMarkImagePath;
		public Control WaterMarkImagePathRow;
        public Button ImageUrlSelect;
        public Button ImageUrlUpload;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			PageUtils.CheckRequestParameter("PublishmentSystemID");
         
			if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdConfigration, "图片水印设置", AppManager.Cms.Permission.WebSite.Configration);

				EBooleanUtils.AddListItems(IsWaterMark);
				ControlUtils.SelectListItemsIgnoreCase(IsWaterMark, PublishmentSystemInfo.Additional.IsWaterMark.ToString());

                LoadWaterMarkPosition(PublishmentSystemInfo.Additional.WaterMarkPosition);

				for (var i = 1; i <= 10; i++)
				{
					WaterMarkTransparency.Items.Add(new ListItem(i + "0%", i.ToString()));
				}
                ControlUtils.SelectListItemsIgnoreCase(WaterMarkTransparency, PublishmentSystemInfo.Additional.WaterMarkTransparency.ToString());

                WaterMarkMinWidth.Text = PublishmentSystemInfo.Additional.WaterMarkMinWidth.ToString();
                WaterMarkMinHeight.Text = PublishmentSystemInfo.Additional.WaterMarkMinHeight.ToString();

				EBooleanUtils.AddListItems(IsImageWaterMark, "图片型", "文字型");
                ControlUtils.SelectListItemsIgnoreCase(IsImageWaterMark, PublishmentSystemInfo.Additional.IsImageWaterMark.ToString());

                WaterMarkFormatString.Text = PublishmentSystemInfo.Additional.WaterMarkFormatString;

				LoadSystemFont();
                ControlUtils.SelectListItemsIgnoreCase(WaterMarkFontName, PublishmentSystemInfo.Additional.WaterMarkFontName);

                WaterMarkFontSize.Text = PublishmentSystemInfo.Additional.WaterMarkFontSize.ToString();

                WaterMarkImagePath.Text = PublishmentSystemInfo.Additional.WaterMarkImagePath;
               
				IsWaterMark_SelectedIndexChanged(null, null);
                WaterMarkImagePath.Attributes.Add("onchange", GetShowImageScript("preview_WaterMarkImagePath", PublishmentSystemInfo.PublishmentSystemUrl));

                var showPopWinString = ModalSelectImage.GetOpenWindowString(PublishmentSystemInfo, WaterMarkImagePath.ClientID);
                ImageUrlSelect.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalUploadImageSingle.GetOpenWindowStringToTextBox(PublishmentSystemId, WaterMarkImagePath.ClientID);
                ImageUrlUpload.Attributes.Add("onclick", showPopWinString);
			}
		}

		private void LoadWaterMarkPosition (int selectPosition)
		{
			WaterMarkPosition.Text = "<table width=\"300\" height=\"243\" border=\"0\" background=\"../pic/flower.jpg\">";
			for (var i = 1;i < 10; i++)
			{
				if ((i % 3) == 1)
				{
					WaterMarkPosition.Text = WaterMarkPosition.Text + "<tr>";
				}
				if (selectPosition == i)
				{
					object obj1 = WaterMarkPosition.Text;
					WaterMarkPosition.Text = string.Concat(new object[]{obj1, "<td width=\"33%\" style=\"font-size:18px;\" align=\"center\"><input type=\"radio\" id=\"WaterMarkPosition\" name=\"WaterMarkPosition\" value=\"", i, "\" checked>#", i, "</td>"});
				}
				else
				{
					object obj2 = WaterMarkPosition.Text;
					WaterMarkPosition.Text = string.Concat(new object[]{obj2, "<td width=\"33%\" style=\"font-size:18px;\" align=\"center\"><input type=\"radio\" id=\"WaterMarkPosition\" name=\"WaterMarkPosition\" value=\"", i, "\" >#", i, "</td>"});
				}
				if ((i % 3) == 0)
				{
					WaterMarkPosition.Text = WaterMarkPosition.Text + "</tr>";
				}
			}
			WaterMarkPosition.Text = WaterMarkPosition.Text + "</table>";
		}

		private void LoadSystemFont()
		{
		    var familyArray = new InstalledFontCollection().Families;
		    foreach (var family in familyArray)
		    {
		        WaterMarkFontName.Items.Add(new ListItem(family.Name, family.Name));
		    }
		}

        public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                PublishmentSystemInfo.Additional.IsWaterMark = TranslateUtils.ToBool(IsWaterMark.SelectedValue);
                PublishmentSystemInfo.Additional.WaterMarkPosition = TranslateUtils.ToInt(Request.Form["WaterMarkPosition"]);
                PublishmentSystemInfo.Additional.WaterMarkTransparency = TranslateUtils.ToInt(WaterMarkTransparency.SelectedValue);
                PublishmentSystemInfo.Additional.WaterMarkMinWidth = TranslateUtils.ToInt(WaterMarkMinWidth.Text);
                PublishmentSystemInfo.Additional.WaterMarkMinHeight = TranslateUtils.ToInt(WaterMarkMinHeight.Text);
                PublishmentSystemInfo.Additional.IsImageWaterMark = TranslateUtils.ToBool(IsImageWaterMark.SelectedValue);
                PublishmentSystemInfo.Additional.WaterMarkFormatString = WaterMarkFormatString.Text;
                PublishmentSystemInfo.Additional.WaterMarkFontName = WaterMarkFontName.SelectedValue;
                PublishmentSystemInfo.Additional.WaterMarkFontSize = TranslateUtils.ToInt(WaterMarkFontSize.Text);
                PublishmentSystemInfo.Additional.WaterMarkImagePath = WaterMarkImagePath.Text;
				
				try
				{
                    DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);
                    Body.AddSiteLog(PublishmentSystemId, "修改图片水印设置");
                     SuccessMessage("图片水印设置修改成功！");
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "图片水印设置修改失败！");
				}
			}
		}

		public void IsWaterMark_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (EBooleanUtils.Equals(IsWaterMark.SelectedValue, EBoolean.True))
			{
				WaterMarkPositionRow.Visible = WaterMarkTransparencyRow.Visible = WaterMarkMinRow.Visible = IsImageWaterMarkRow.Visible = true;
				if (EBooleanUtils.Equals(IsImageWaterMark.SelectedValue, EBoolean.True))
				{
					WaterMarkFormatStringRow.Visible = WaterMarkFontNameRow.Visible = WaterMarkFontSizeRow.Visible = false;
					WaterMarkImagePathRow.Visible = true;
				}
				else
				{
					WaterMarkFormatStringRow.Visible = WaterMarkFontNameRow.Visible = WaterMarkFontSizeRow.Visible = true;
					WaterMarkImagePathRow.Visible = false;
				}
			}
			else
			{
				WaterMarkPositionRow.Visible = WaterMarkTransparencyRow.Visible = WaterMarkMinRow.Visible = IsImageWaterMarkRow.Visible = WaterMarkFormatStringRow.Visible = WaterMarkFontNameRow.Visible = WaterMarkFontSizeRow.Visible = WaterMarkImagePathRow.Visible = false;
			}
		}
	}
}
