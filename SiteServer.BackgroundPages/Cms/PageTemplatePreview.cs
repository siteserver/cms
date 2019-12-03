using System;
using System.Web.UI.WebControls;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;
using SiteServer.CMS.StlParser.Utility;


namespace SiteServer.BackgroundPages.Cms
{
    public class PageTemplatePreview : BasePageCms
    {
        public DropDownList DdlTemplateType;
        public PlaceHolder PhTemplateChannel;
        public DropDownList DdlChannelId;
        public TextBox TbCode;
        public Literal LtlPreview;
        public TextBox TbTemplate;
        public Button BtnReturn;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            if (IsPostBack) return;
            VerifySitePermissions(Constants.WebSitePermissions.Template);

            TemplateTypeUtils.AddListItems(DdlTemplateType);
            ChannelManager.AddListItemsAsync(DdlChannelId.Items, Site, false, true, AuthRequest.AdminPermissionsImpl).GetAwaiter().GetResult();
            if (AuthRequest.IsQueryExists("fromCache"))
            {
                TbTemplate.Text = WebConfigUtils.DecryptStringBySecretKey(CacheUtils.Get<string>("SiteServer.BackgroundPages.Cms.PageTemplatePreview"));
            }
            
            if (AuthRequest.IsQueryExists("returnUrl"))
            {
                BtnReturn.Visible = true;
            }
        }

        public void DdlTemplateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var templateType = TemplateTypeUtils.GetEnumType(DdlTemplateType.SelectedValue);
            if (templateType == TemplateType.IndexPageTemplate || templateType == TemplateType.IndexPageTemplate)
            {
                PhTemplateChannel.Visible = false;
            }
            else
            {
                PhTemplateChannel.Visible = true;
            }
        }

        public void BtnPreview_OnClick(object sender, EventArgs e)
        {           
            if (string.IsNullOrEmpty(TbTemplate.Text))
            {
                FailMessage("请输入STL标签");
                return;
            }

            var templateType = TemplateTypeUtils.GetEnumType(DdlTemplateType.SelectedValue);
            var channelId = SiteId;
            var contentId = 0;
            if (templateType == TemplateType.ChannelTemplate || templateType == TemplateType.ContentTemplate)
            {
                channelId = TranslateUtils.ToInt(DdlChannelId.SelectedValue);
                if (templateType == TemplateType.ContentTemplate)
                {
                    var channelInfo = ChannelManager.GetChannelAsync(SiteId, channelId).GetAwaiter().GetResult();
                    var count = DataProvider.ContentRepository.GetCountAsync(Site, channelInfo).GetAwaiter().GetResult();
                    if (count > 0)
                    {
                        var tableName = ChannelManager.GetTableNameAsync(Site, channelInfo).GetAwaiter().GetResult();
                        contentId = DataProvider.ContentRepository.GetFirstContentIdAsync(tableName, channelId).GetAwaiter().GetResult();
                    }

                    if (contentId == 0)
                    {
                        FailMessage("所选栏目下无内容，请选择有内容的栏目");
                        return;
                    }
                }
            }

            TbCode.Text = LtlPreview.Text = StlParserManager.ParseTemplatePreviewAsync(Site, templateType, channelId, contentId, TbTemplate.Text).GetAwaiter().GetResult();

            LtlPreview.Text += "<script>$('#linkCode').click();</script>";
        }

        public void BtnReturn_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(WebConfigUtils.DecryptStringBySecretKey(AuthRequest.GetQueryString("returnUrl")));
        }
    }
}