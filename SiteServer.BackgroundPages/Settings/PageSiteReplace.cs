using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageSiteReplace : BasePageCms
    {
        public Literal LtlPublishmentSystemName;
		public PlaceHolder PhChooseSiteTemplate;
		public Repeater RptContents;
		public HtmlInputHidden HihSiteTemplateDir;

        public PlaceHolder PhCreateSiteParameters;
        public Literal LtlSiteTemplateName;
        public RadioButtonList RblIsDeleteChannels;
        public RadioButtonList RblIsDeleteTemplates;
        public RadioButtonList RblIsDeleteFiles;
        public RadioButtonList RblIsOverride;

        public Button BtnNext;
		public Button BtnSubmit;

		private SortedList _sortedlist = new SortedList();

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetSettingsUrl(nameof(PageSiteReplace), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _sortedlist = SiteTemplateManager.Instance.GetSiteTemplateSortedList();

            if (IsPostBack) return;

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.Site);

            LtlPublishmentSystemName.Text = PublishmentSystemInfo.PublishmentSystemName;

            var directoryList = new List<DirectoryInfo>();
            foreach (string directoryName in _sortedlist.Keys)
            {
                var directoryPath = PathUtility.GetSiteTemplatesPath(directoryName);
                var dirInfo = new DirectoryInfo(directoryPath);
                directoryList.Add(dirInfo);
            }

            RptContents.DataSource = directoryList;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            if (SiteTemplateManager.Instance.GetSiteTemplateCount() == 0)
            {
                PageUtils.RedirectToErrorPage("无站点模板！");
            }
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item) return;

            var directoryInfo = (DirectoryInfo)e.Item.DataItem;
            var siteTemplateInfo = _sortedlist[directoryInfo.Name] as SiteTemplateInfo;

            if (siteTemplateInfo == null) return;

            var ltlChoose = (Literal)e.Item.FindControl("ltlChoose");
            var ltlTemplateName = (Literal)e.Item.FindControl("ltlTemplateName");
            var ltlName = (Literal)e.Item.FindControl("ltlName");
            var ltlDescription = (Literal)e.Item.FindControl("ltlDescription");
            var ltlSamplePic = (Literal)e.Item.FindControl("ltlSamplePic");

            ltlChoose.Text = $@"<input type=""radio"" name=""choose"" id=""choose"" onClick=""document.getElementById('HihSiteTemplateDir').value=this.value;"" value=""{directoryInfo.Name}"" />";
            
            if (!string.IsNullOrEmpty(siteTemplateInfo.SiteTemplateName))
            {
                ltlTemplateName.Text = !string.IsNullOrEmpty(siteTemplateInfo.WebSiteUrl) ? $"<a href=\"{PageUtils.ParseConfigRootUrl(siteTemplateInfo.WebSiteUrl)}\" target=_blank>{siteTemplateInfo.SiteTemplateName}</a>" : siteTemplateInfo.SiteTemplateName;
            }

            ltlName.Text = directoryInfo.Name;

            if (!string.IsNullOrEmpty(siteTemplateInfo.Description))
            {
                ltlDescription.Text = siteTemplateInfo.Description;
            }

            if (!string.IsNullOrEmpty(siteTemplateInfo.PicFileName))
            {
                var siteTemplateUrl = PageUtils.GetSiteTemplatesUrl(directoryInfo.Name);
                var picFileName = PageUtils.GetSiteTemplateMetadataUrl(siteTemplateUrl, siteTemplateInfo.PicFileName);
                ltlSamplePic.Text =
                    $"<a href=\"{picFileName}\" target=_blank><img height=120 width=100 border=0 src=\"{picFileName}\" /></a>";
            }
        }

        public void BtnNext_Click(object sender, EventArgs e)
        {
            var siteTemplateDir = HihSiteTemplateDir.Value;
            var siteTemplateInfo = _sortedlist[siteTemplateDir] as SiteTemplateInfo;

            if (siteTemplateInfo == null)
            {
                FailMessage("请选择需要替换的站点模板");
                return;
            }

            LtlSiteTemplateName.Text = !string.IsNullOrEmpty(siteTemplateInfo.WebSiteUrl) ? $"<a href=\"{PageUtils.ParseConfigRootUrl(siteTemplateInfo.WebSiteUrl)}\" target=_blank>{siteTemplateInfo.SiteTemplateName}</a>" : siteTemplateInfo.SiteTemplateName;

            PhChooseSiteTemplate.Visible = true;
            PhCreateSiteParameters.Visible = false;
            BtnNext.Visible = false;
            BtnSubmit.Visible = true;
        }

        public void BtnSubmit_Click(object sender, EventArgs e)
		{
		    var siteTemplateDir = HihSiteTemplateDir.Value;

            var userKeyPrefix = StringUtils.Guid();
            var siteTemplatePath = PathUtility.GetSiteTemplatesPath(siteTemplateDir);

            Body.AddAdminLog("整站替换", $"站点:{PublishmentSystemInfo.PublishmentSystemName}");

            PageUtils.Redirect(PageProgressBar.GetRecoveryUrl(PublishmentSystemId, RblIsDeleteChannels.SelectedValue, RblIsDeleteTemplates.SelectedValue, RblIsDeleteFiles.SelectedValue, false, siteTemplatePath, RblIsOverride.SelectedValue, RblIsOverride.SelectedValue, userKeyPrefix));
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageSite.GetRedirectUrl());
        }
    }
}
