using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageTemplatePreview : BasePageCms
    {
        public Literal LtlMessage;
        public DropDownList DdlTemplateType;
        public PlaceHolder PhTemplateChannel;
        public DropDownList DdlNodeId;
        public TextBox TbTemplate;
        public Button BtnReturn;

        public PlaceHolder PhPreview;
        public Literal LtlPreview;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (IsPostBack) return;
            BreadCrumb(AppManager.Cms.LeftMenu.IdTemplate, "STL在线解析", AppManager.Permissions.WebSite.Template);

            ETemplateTypeUtils.AddListItems(DdlTemplateType);
            NodeManager.AddListItems(DdlNodeId.Items, PublishmentSystemInfo, false, true, Body.AdministratorName);
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
            PhPreview.Visible = false;
            LtlMessage.Text = string.Empty;

            if (string.IsNullOrEmpty(TbTemplate.Text))
            {
                LtlMessage.Text = @"<div class=""alert alert-danger"">
                                        请输入STL标签
                                    </div>";
                return;
            }

            var templateType = ETemplateTypeUtils.GetEnumType(DdlTemplateType.SelectedValue);
            var channelId = PublishmentSystemId;
            var contentId = 0;
            if (templateType == ETemplateType.ChannelTemplate || templateType == ETemplateType.ContentTemplate)
            {
                channelId = TranslateUtils.ToInt(DdlNodeId.SelectedValue);
                if (templateType == ETemplateType.ContentTemplate)
                {
                    var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, channelId);
                    if (nodeInfo.ContentNum > 0)
                    {
                        var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeInfo);
                        contentId = BaiRongDataProvider.ContentDao.GetFirstContentId(tableName, channelId);
                    }

                    if (contentId == 0)
                    {
                        LtlMessage.Text = @"<div class=""alert alert-danger"">
                                        所选栏目下无内容，请选择有内容的栏目
                                    </div>";
                        return;
                    }
                }
            }

            PhPreview.Visible = true;
            LtlPreview.Text = StlParserManager.ParseTemplateContent(TbTemplate.Text, PublishmentSystemId, channelId, contentId);
        }

        public void BtnReturn_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(TranslateUtils.DecryptStringBySecretKey(Body.GetQueryString("returnUrl")));
        }
    }
}