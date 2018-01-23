using System;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageTemplatePreview : BasePageCms
    {
        public DropDownList DdlTemplateType;
        public PlaceHolder PhTemplateChannel;
        public DropDownList DdlNodeId;
        public TextBox TbCode;
        public Literal LtlPreview;
        public TextBox TbTemplate;
        public Button BtnReturn;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            if (IsPostBack) return;
            VerifySitePermissions(AppManager.Permissions.WebSite.Template);

            ETemplateTypeUtils.AddListItems(DdlTemplateType);
            ChannelManager.AddListItems(DdlNodeId.Items, SiteInfo, false, true, Body.AdminName);
            if (Body.IsQueryExists("fromCache"))
            {
                TbTemplate.Text = TranslateUtils.DecryptStringBySecretKey(CacheUtils.Get<string>("SiteServer.BackgroundPages.Cms.PageTemplatePreview"));
            }
            
            if (Body.IsQueryExists("returnUrl"))
            {
                BtnReturn.Visible = true;
            }
        }

        public void DdlTemplateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var templateType = ETemplateTypeUtils.GetEnumType(DdlTemplateType.SelectedValue);
            if (templateType == ETemplateType.IndexPageTemplate || templateType == ETemplateType.IndexPageTemplate)
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

            var templateType = ETemplateTypeUtils.GetEnumType(DdlTemplateType.SelectedValue);
            var channelId = SiteId;
            var contentId = 0;
            if (templateType == ETemplateType.ChannelTemplate || templateType == ETemplateType.ContentTemplate)
            {
                channelId = TranslateUtils.ToInt(DdlNodeId.SelectedValue);
                if (templateType == ETemplateType.ContentTemplate)
                {
                    var nodeInfo = ChannelManager.GetChannelInfo(SiteId, channelId);
                    if (nodeInfo.ContentNum > 0)
                    {
                        var tableName = ChannelManager.GetTableName(SiteInfo, nodeInfo);
                        contentId = DataProvider.ContentDao.GetFirstContentId(tableName, channelId);
                    }

                    if (contentId == 0)
                    {
                        FailMessage("所选栏目下无内容，请选择有内容的栏目");
                        return;
                    }
                }
            }

            TbCode.Text = LtlPreview.Text = StlParserManager.ParseTemplateContent(TbTemplate.Text, SiteId, channelId, contentId);

            LtlPreview.Text += "<script>$('#linkCode').click();</script>";
        }

        public void BtnReturn_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(TranslateUtils.DecryptStringBySecretKey(Body.GetQueryString("returnUrl")));
        }
    }
}