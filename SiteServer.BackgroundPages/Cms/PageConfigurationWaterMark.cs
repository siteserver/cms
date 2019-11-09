using System;
using System.Drawing.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationWaterMark : BasePageCms
    {
		public DropDownList DdlIsWaterMark;
		public Literal LtlWaterMarkPosition;
		public PlaceHolder PhWaterMarkPosition;
		public DropDownList DdlWaterMarkTransparency;
		public PlaceHolder PhWaterMarkTransparency;
		public TextBox TbWaterMarkMinWidth;
		public TextBox TbWaterMarkMinHeight;
		public PlaceHolder PhWaterMarkMin;
		public DropDownList DdlIsImageWaterMark;
		public PlaceHolder PhIsImageWaterMark;
		public TextBox TbWaterMarkFormatString;
		public PlaceHolder PhWaterMarkFormatString;
		public DropDownList DdlWaterMarkFontName;
		public PlaceHolder PhWaterMarkFontName;
		public TextBox TbWaterMarkFontSize;
		public PlaceHolder PhWaterMarkFontSize;
		public TextBox TbWaterMarkImagePath;
		public PlaceHolder PhWaterMarkImagePath;
        public Button BtnImageUrlSelect;
        public Button BtnImageUrlUpload;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			PageUtils.CheckRequestParameter("siteId");

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Configration);

            EBooleanUtils.AddListItems(DdlIsWaterMark);
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsWaterMark, Site.Additional.IsWaterMark.ToString());

            LoadWaterMarkPosition(Site.Additional.WaterMarkPosition);

            for (var i = 1; i <= 10; i++)
            {
                DdlWaterMarkTransparency.Items.Add(new ListItem(i + "0%", i.ToString()));
            }
            ControlUtils.SelectSingleItemIgnoreCase(DdlWaterMarkTransparency, Site.Additional.WaterMarkTransparency.ToString());

            TbWaterMarkMinWidth.Text = Site.Additional.WaterMarkMinWidth.ToString();
            TbWaterMarkMinHeight.Text = Site.Additional.WaterMarkMinHeight.ToString();

            EBooleanUtils.AddListItems(DdlIsImageWaterMark, "图片型", "文字型");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsImageWaterMark, Site.Additional.IsImageWaterMark.ToString());

            TbWaterMarkFormatString.Text = Site.Additional.WaterMarkFormatString;

            LoadSystemFont();
            ControlUtils.SelectSingleItemIgnoreCase(DdlWaterMarkFontName, Site.Additional.WaterMarkFontName);

            TbWaterMarkFontSize.Text = Site.Additional.WaterMarkFontSize.ToString();

            TbWaterMarkImagePath.Text = Site.Additional.WaterMarkImagePath;
               
            DdlIsWaterMark_SelectedIndexChanged(null, null);
            TbWaterMarkImagePath.Attributes.Add("onchange", GetShowImageScript("preview_WaterMarkImagePath", Site.WebUrl));

            var showPopWinString = ModalSelectImage.GetOpenWindowString(Site, TbWaterMarkImagePath.ClientID);
            BtnImageUrlSelect.Attributes.Add("onclick", showPopWinString);

            showPopWinString = ModalUploadImageSingle.GetOpenWindowStringToTextBox(SiteId, TbWaterMarkImagePath.ClientID);
            BtnImageUrlUpload.Attributes.Add("onclick", showPopWinString);
        }

		private void LoadWaterMarkPosition (int selectPosition)
		{
            LtlWaterMarkPosition.Text = "<table width=\"300\" height=\"243\" border=\"0\" background=\"../pic/flower.jpg\">";
			for (var i = 1;i < 10; i++)
			{
				if (i % 3 == 1)
				{
                    LtlWaterMarkPosition.Text = LtlWaterMarkPosition.Text + "<tr>";
				}
				if (selectPosition == i)
				{
					object obj1 = LtlWaterMarkPosition.Text;
                    LtlWaterMarkPosition.Text = string.Concat(obj1, "<td width=\"33%\" style=\"font-size:18px;color: #fff\" align=\"center\"><input type=\"radio\" id=\"WaterMarkPosition\" name=\"WaterMarkPosition\" value=\"", i, "\" checked>#", i, "</td>");
				}
				else
				{
					object obj2 = LtlWaterMarkPosition.Text;
                    LtlWaterMarkPosition.Text = string.Concat(obj2, "<td width=\"33%\" style=\"font-size:18px;color: #fff\" align=\"center\"><input type=\"radio\" id=\"WaterMarkPosition\" name=\"WaterMarkPosition\" value=\"", i, "\" >#", i, "</td>");
				}
				if (i % 3 == 0)
				{
                    LtlWaterMarkPosition.Text = LtlWaterMarkPosition.Text + "</tr>";
				}
			}
            LtlWaterMarkPosition.Text = LtlWaterMarkPosition.Text + "</table>";
		}

		private void LoadSystemFont()
		{
		    var familyArray = new InstalledFontCollection().Families;
		    foreach (var family in familyArray)
		    {
                DdlWaterMarkFontName.Items.Add(new ListItem(family.Name, family.Name));
		    }
		}

        public override void Submit_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid) return;

		    Site.Additional.IsWaterMark = TranslateUtils.ToBool(DdlIsWaterMark.SelectedValue);
		    Site.Additional.WaterMarkPosition = TranslateUtils.ToInt(Request.Form["WaterMarkPosition"]);
		    Site.Additional.WaterMarkTransparency = TranslateUtils.ToInt(DdlWaterMarkTransparency.SelectedValue);
		    Site.Additional.WaterMarkMinWidth = TranslateUtils.ToInt(TbWaterMarkMinWidth.Text);
		    Site.Additional.WaterMarkMinHeight = TranslateUtils.ToInt(TbWaterMarkMinHeight.Text);
		    Site.Additional.IsImageWaterMark = TranslateUtils.ToBool(DdlIsImageWaterMark.SelectedValue);
		    Site.Additional.WaterMarkFormatString = TbWaterMarkFormatString.Text;
		    Site.Additional.WaterMarkFontName = DdlWaterMarkFontName.SelectedValue;
		    Site.Additional.WaterMarkFontSize = TranslateUtils.ToInt(TbWaterMarkFontSize.Text);
		    Site.Additional.WaterMarkImagePath = TbWaterMarkImagePath.Text;
				
		    try
		    {
		        DataProvider.SiteDao.UpdateAsync(Site).GetAwaiter().GetResult();
		        AuthRequest.AddSiteLogAsync(SiteId, "修改图片水印设置").GetAwaiter().GetResult();
		        SuccessMessage("图片水印设置修改成功！");
		    }
		    catch(Exception ex)
		    {
		        FailMessage(ex, "图片水印设置修改失败！");
		    }
		}

		public void DdlIsWaterMark_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (EBooleanUtils.Equals(DdlIsWaterMark.SelectedValue, EBoolean.True))
			{
                PhWaterMarkPosition.Visible = PhWaterMarkTransparency.Visible = PhWaterMarkMin.Visible = PhIsImageWaterMark.Visible = true;
				if (EBooleanUtils.Equals(DdlIsImageWaterMark.SelectedValue, EBoolean.True))
				{
                    PhWaterMarkFormatString.Visible = PhWaterMarkFontName.Visible = PhWaterMarkFontSize.Visible = false;
                    PhWaterMarkImagePath.Visible = true;
				}
				else
				{
                    PhWaterMarkFormatString.Visible = PhWaterMarkFontName.Visible = PhWaterMarkFontSize.Visible = true;
                    PhWaterMarkImagePath.Visible = false;
				}
			}
			else
			{
                PhWaterMarkPosition.Visible = PhWaterMarkTransparency.Visible = PhWaterMarkMin.Visible = PhIsImageWaterMark.Visible = PhWaterMarkFormatString.Visible = PhWaterMarkFontName.Visible = PhWaterMarkFontSize.Visible = PhWaterMarkImagePath.Visible = false;
			}
		}
	}
}
