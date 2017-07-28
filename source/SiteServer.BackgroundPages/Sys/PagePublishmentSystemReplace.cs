using System;
using System.Collections;
using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Text;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Sys
{
    public class PagePublishmentSystemReplace : BasePageCms
    {
        public Literal PublishmentSystemName;
		public PlaceHolder ChooseSiteTemplate;
		public DataGrid dgContents;
		public HtmlInputHidden SiteTemplateDir;

        public PlaceHolder CreateSiteParameters;
        public Literal ltlSiteTemplateName;
        public RadioButtonList IsDeleteChannels;
        public RadioButtonList IsDeleteTemplates;
        public RadioButtonList IsDeleteFiles;
        public RadioButtonList IsOverride;

		public PlaceHolder OperatingError;
        public Literal ltlErrorMessage;

		public Button Previous;
		public Button Next;

		SortedList sortedlist = new SortedList();

		public string GetSiteTemplateName(string siteTemplateDir)
		{
			var retval = string.Empty;
			var siteTemplateInfo = sortedlist[siteTemplateDir] as SiteTemplateInfo;
			if (siteTemplateInfo != null && !string.IsNullOrEmpty(siteTemplateInfo.SiteTemplateName))
			{
				if (!string.IsNullOrEmpty(siteTemplateInfo.WebSiteUrl))
				{

                    retval =
                        $"<a href=\"{PageUtils.ParseConfigRootUrl(siteTemplateInfo.WebSiteUrl)}\" target=_blank>{siteTemplateInfo.SiteTemplateName}</a>";
				}
				else
				{
					retval = siteTemplateInfo.SiteTemplateName;
				}
			}
			return retval;
		}

		public string GetDescription(string siteTemplateDir)
		{
			var retval = string.Empty;
			var siteTemplateInfo = sortedlist[siteTemplateDir] as SiteTemplateInfo;
			if (siteTemplateInfo != null && !string.IsNullOrEmpty(siteTemplateInfo.Description))
			{
				retval = siteTemplateInfo.Description;
			}
			return retval;
		}

		public string GetSamplePicHtml(string siteTemplateDir)
		{
			var retval = string.Empty;
			var siteTemplateInfo = sortedlist[siteTemplateDir] as SiteTemplateInfo;
			if (siteTemplateInfo != null && !string.IsNullOrEmpty(siteTemplateInfo.PicFileName))
			{
                var siteTemplateUrl = PageUtility.GetSiteTemplatesUrl(siteTemplateDir);
			    var picFileName = PageUtility.GetSiteTemplateMetadataUrl(siteTemplateUrl, siteTemplateInfo.PicFileName);
                retval =
                    $"<a href=\"{picFileName}\" target=_blank><img height=120 width=100 border=0 src=\"{picFileName}\" /></a>";
			}
			return retval;
		}

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            sortedlist = SiteTemplateManager.Instance.GetSiteTemplateSortedList();
			if (!IsPostBack)
            {
                BreadCrumbSys(AppManager.Sys.LeftMenu.Site, "整站替换", AppManager.Sys.Permission.SysSite);

                PublishmentSystemName.Text = PublishmentSystemInfo.PublishmentSystemName;

				BindGrid();

                if (SiteTemplateManager.Instance.GetSiteTemplateCount() > 0)
                {
                    SetActivePlaceHolder(WizardPlaceHolder.ChooseSiteTemplate, ChooseSiteTemplate);
                }
                else
                {
                    PageUtils.RedirectToErrorPage("无站点模板！");
                }
			}
		}

		public void BindGrid()
		{
			try
			{
				var directoryArrayList = new ArrayList();
				foreach (string directoryName in sortedlist.Keys)
				{
                    var directoryPath = PathUtility.GetSiteTemplatesPath(directoryName);
					var dirInfo = new DirectoryInfo(directoryPath);
					directoryArrayList.Add(dirInfo);
				}

                dgContents.DataSource = directoryArrayList;
                dgContents.DataBind();
			}
			catch (Exception ex)
			{
                PageUtils.RedirectToErrorPage(ex.Message);
			}
		}

		public WizardPlaceHolder CurrentWizardPlaceHolder
		{
			get
			{
				if (ViewState["WizardPlaceHolder"] != null)
					return (WizardPlaceHolder)ViewState["WizardPlaceHolder"];

                if (SiteTemplateManager.Instance.GetSiteTemplateCount() > 0)
                {
                    return WizardPlaceHolder.ChooseSiteTemplate;
                }
                else
                {
                    return WizardPlaceHolder.CreateSiteParameters;
                }
                
			}
			set
			{
				ViewState["WizardPlaceHolder"] = value;
			}
		}


		public enum WizardPlaceHolder
		{
            ChooseSiteTemplate,
            CreateSiteParameters,
			OperatingError,
		}

		void SetActivePlaceHolder(WizardPlaceHolder panel, Control controlToShow)
		{
			var currentPlaceHolder = FindControl(CurrentWizardPlaceHolder.ToString()) as PlaceHolder;
			if (currentPlaceHolder != null)
				currentPlaceHolder.Visible = false;

			switch (panel)
			{
                case WizardPlaceHolder.ChooseSiteTemplate:
					Previous.CssClass = "btn disabled";
                    Previous.Enabled = false;
                    Next.CssClass = "btn btn-primary";
                    Next.Enabled = true;
					break;
                case WizardPlaceHolder.CreateSiteParameters:
                    Previous.CssClass = "btn";
                    Previous.Enabled = true;
                    Next.CssClass = "btn btn-primary";
                    Next.Enabled = true;
                    break;
				case WizardPlaceHolder.OperatingError:
                    Previous.CssClass = "btn disabled";
                    Previous.Enabled = false;
                    Next.CssClass = "btn btn-primary disabled";
                    Next.Enabled = false;
					break;
				default:
                    Previous.CssClass = "btn";
                    Previous.Enabled = true;
                    Next.CssClass = "btn btn-primary";
                    Next.Enabled = true;
					break;
			}

			controlToShow.Visible = true;
			CurrentWizardPlaceHolder = panel;
		}

		public void NextPlaceHolder(object sender, EventArgs e)
		{
		    switch (CurrentWizardPlaceHolder)
			{
				case WizardPlaceHolder.ChooseSiteTemplate:
                    if (string.IsNullOrEmpty(SiteTemplateDir.Value))
                    {
                        FailMessage("必须选择一个站的模板进行操作");
                        return;
                    }
                    ltlSiteTemplateName.Text =
                        $"{GetSiteTemplateName(SiteTemplateDir.Value)}（{SiteTemplateDir.Value}）";
                    SetActivePlaceHolder(WizardPlaceHolder.CreateSiteParameters, CreateSiteParameters);
					break;

                case WizardPlaceHolder.CreateSiteParameters:
                    var userKeyPrefix = StringUtils.Guid();
			        var siteTemplatePath = PathUtility.GetSiteTemplatesPath(SiteTemplateDir.Value);

                    Body.AddAdminLog("整站替换",
                        $"站点:{PublishmentSystemInfo.PublishmentSystemName}");

                    PageUtils.Redirect(PageProgressBar.GetRecoveryUrl(PublishmentSystemId, IsDeleteChannels.SelectedValue, IsDeleteTemplates.SelectedValue, IsDeleteFiles.SelectedValue, false, siteTemplatePath, IsOverride.SelectedValue, IsOverride.SelectedValue, userKeyPrefix));
                    break;
			}
		}

		public void PreviousPlaceHolder(object sender, EventArgs e)
		{
			switch (CurrentWizardPlaceHolder)
			{
                case WizardPlaceHolder.ChooseSiteTemplate:
					break;

                case WizardPlaceHolder.CreateSiteParameters:
                    SetActivePlaceHolder(WizardPlaceHolder.ChooseSiteTemplate, ChooseSiteTemplate);
					break;
			}
		}

	}
}
