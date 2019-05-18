using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.BackgroundPages.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageTemplate : BasePageCms
    {
        public DropDownList DdlTemplateType;
        public TextBox TbKeywords;
        public Repeater RptContents;
        public Literal LtlCommands;

        private string _templateType;
        private string _keywords;

        public static string GetRedirectUrl(int siteId)
        {
            return PageUtilsEx.GetCmsUrl(siteId, nameof(PageTemplate), null);
        }

        public static string GetRedirectUrl(int siteId, TemplateType templateType)
        {
            return PageUtilsEx.GetCmsUrl(siteId, nameof(PageTemplate), new NameValueCollection
            {
                {"templateType", templateType.Value}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            FxUtils.CheckRequestParameter("siteId");

            _templateType = AuthRequest.GetQueryString("templateType");
            _keywords = AuthRequest.GetQueryString("keywords");

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Template);

            DdlTemplateType.Items.Add(new ListItem("<所有类型>", string.Empty));
            ControlUtils.TemplateTypeUI.AddListItems(DdlTemplateType);
            ControlUtils.SelectSingleItem(DdlTemplateType, _templateType);

            TbKeywords.Text = _keywords;

            if (AuthRequest.IsQueryExists("Delete"))
            {
                var templateId = AuthRequest.GetQueryInt("TemplateID");

                try
                {
                    var templateInfo = TemplateManager.GetTemplateInfo(SiteId, templateId);
                    if (templateInfo != null)
                    {
                        DataProvider.TemplateDao.Delete(SiteId, templateId);
                        AuthRequest.AddSiteLog(SiteId,
                            $"删除{TemplateTypeUtils.GetText(templateInfo.Type)}",
                            $"模板名称:{templateInfo.TemplateName}");
                    }
                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }
            else if (AuthRequest.IsQueryExists("SetDefault"))
            {
                var templateId = AuthRequest.GetQueryInt("TemplateID");

                try
                {
                    var templateInfo = TemplateManager.GetTemplateInfo(SiteId, templateId);
                    if (templateInfo != null)
                    {
                        DataProvider.TemplateDao.SetDefault(SiteId, templateId);
                        AuthRequest.AddSiteLog(SiteId,
                            $"设置默认{TemplateTypeUtils.GetText(templateInfo.Type)}",
                            $"模板名称:{templateInfo.TemplateName}");
                    }
                    SuccessMessage();
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "操作失败");
                }
            }

            if (string.IsNullOrEmpty(_templateType))
            {
                LtlCommands.Text = $@"
<input type=""button"" class=""btn"" onclick=""location.href='{PageTemplateAdd.GetRedirectUrl(SiteId, 0, TemplateType.IndexPageTemplate)}';"" value=""添加首页模板"" />
<input type=""button"" class=""btn"" onclick=""location.href='{PageTemplateAdd.GetRedirectUrl(SiteId, 0, TemplateType.ChannelTemplate)}';"" value=""添加栏目模板"" />
<input type=""button"" class=""btn"" onclick=""location.href='{PageTemplateAdd.GetRedirectUrl(SiteId, 0, TemplateType.ContentTemplate)}';"" value=""添加内容模板"" />
<input type=""button"" class=""btn"" onclick=""location.href='{PageTemplateAdd.GetRedirectUrl(SiteId, 0, TemplateType.FileTemplate)}';"" value=""添加单页模板"" />
";
            }
            else
            {
                var templateType = TemplateTypeUtils.GetEnumType(_templateType);
                LtlCommands.Text = $@"
<input type=""button"" class=""btn btn-success"" onclick=""location.href='{PageTemplateAdd.GetRedirectUrl(SiteId, 0, templateType)}';"" value=""添加{TemplateTypeUtils.GetText(templateType)}"" />
";
            }

            RptContents.DataSource = TemplateManager.GetTemplateInfoList(SiteId, _keywords, _templateType);
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();
        }

        public void DdlTemplateType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            FxUtils.Page.Redirect(PageUtilsEx.GetCmsUrl(SiteId, nameof(PageTemplate), new NameValueCollection
            {
                {"templateType", DdlTemplateType.SelectedValue},
                {"keywords", TbKeywords.Text}
            }));
        }

        public void BtnSearch_Click(object sender, EventArgs e)
        {
            FxUtils.Page.Redirect(PageUtilsEx.GetCmsUrl(SiteId, nameof(PageTemplate), new NameValueCollection
            {
                {"templateType", DdlTemplateType.SelectedValue},
                {"keywords", TbKeywords.Text}
            }));
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var templateInfo = (TemplateInfo)e.Item.DataItem;
            var templateType = templateInfo.Type;

            var ltlTemplateName = (Literal)e.Item.FindControl("ltlTemplateName");
            var ltlRelatedFileName = (Literal)e.Item.FindControl("ltlRelatedFileName");
            var ltlFileName = (Literal)e.Item.FindControl("ltlFileName");
            var ltlUseCount = (Literal)e.Item.FindControl("ltlUseCount");
            var ltlTemplateType = (Literal)e.Item.FindControl("ltlTemplateType");
            var ltlDefaultUrl = (Literal)e.Item.FindControl("ltlDefaultUrl");
            var ltlCopyUrl = (Literal)e.Item.FindControl("ltlCopyUrl");
            var ltlLogUrl = (Literal)e.Item.FindControl("ltlLogUrl");
            var ltlCreateUrl = (Literal)e.Item.FindControl("ltlCreateUrl");
            var ltlDeleteUrl = (Literal)e.Item.FindControl("ltlDeleteUrl");

            var templateAddUrl = PageTemplateAdd.GetRedirectUrl(SiteId, templateInfo.Id, templateInfo.Type);
            ltlTemplateName.Text = $@"<a href=""{templateAddUrl}"">{templateInfo.TemplateName}</a>";
            ltlRelatedFileName.Text = templateInfo.RelatedFileName;

            if (templateType == TemplateType.IndexPageTemplate || templateType == TemplateType.FileTemplate)
            {
                var url = PageUtility.ParseNavigationUrl(SiteInfo, templateInfo.CreatedFileFullName, false);
                ltlFileName.Text = $"<a href='{url}' target='_blank'>{templateInfo.CreatedFileFullName}</a>";
            }

            ltlUseCount.Text = DataProvider.ChannelDao.GetTemplateUseCount(SiteId, templateInfo.Id, templateType, templateInfo.Default).ToString();

            ltlTemplateType.Text = TemplateTypeUtils.GetText(templateType);

            if (templateType != TemplateType.FileTemplate)
            {
                if (templateInfo.Default)
                {
                    ltlDefaultUrl.Text = @"<span class=""badge badge-primary"">默认模板</span>";
                }
                else
                {
                    var defaultUrl = PageUtilsEx.GetCmsUrl(SiteId, nameof(PageTemplate), new NameValueCollection
                    {
                        {"TemplateID", templateInfo.Id.ToString()},
                        {"SetDefault", true.ToString()},
                        { "TemplateType", templateType.Value }
                    });
                    ltlDefaultUrl.Text =
                        $@"<a href=""{defaultUrl}"" onClick=""javascript:return confirm('此操作将把此模板设为默认，确认吗？');"">设为默认</a>";
                }
            }

            var copyUrl = PageTemplateAdd.GetRedirectUrlToCopy(SiteId, templateInfo.Id);
            ltlCopyUrl.Text = $@"<a href=""{copyUrl}"">快速复制</a>";

            var logUrl = PageTemplateLog.GetRedirectUrl(SiteId, templateInfo.Id);
            ltlLogUrl.Text = $@"<a href=""{logUrl}"">修订历史</a>";

            ltlCreateUrl.Text =
                $@"<a href=""javascript:;"" onclick=""{ModalProgressBar.GetOpenWindowStringWithCreateByTemplate(
                    SiteId, templateInfo.Id)}"">生成页面</a>";

            if (!templateInfo.Default)
            {
                var deleteUrl = PageUtilsEx.GetCmsUrl(SiteId, nameof(PageTemplate), new NameValueCollection
                {
                    {"TemplateID", templateInfo.Id.ToString()},
                    {"Delete", true.ToString()},
                    { "TemplateType", templateType.Value }
                });

                ltlDeleteUrl.Text =
                $@"<a href=""javascript:;"" onclick=""{AlertUtils.ConfirmDelete("删除文件", $"此操作将删除模板“{templateInfo.TemplateName}”，确认吗？", deleteUrl)}"">删除</a>";
            }
        }
    }
}
