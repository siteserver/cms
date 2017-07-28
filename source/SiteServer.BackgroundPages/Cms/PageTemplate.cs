using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTemplate : BasePageCms
    {
		public DropDownList DdlTemplateType;
		public TextBox TbKeywords;
		public DataGrid DgContents;
        public Literal LtlCommands;

        private string _templateType;
        private string _keywords;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageTemplate), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            _templateType = Body.GetQueryString("templateType");
            _keywords = Body.GetQueryString("keywords");

			if (!IsPostBack)
			{
                BreadCrumb(AppManager.Cms.LeftMenu.IdTemplate, "模板管理", AppManager.Cms.Permission.WebSite.Template);

				DdlTemplateType.Items.Add(new ListItem("<所有类型>", string.Empty));
                ETemplateTypeUtils.AddListItems(DdlTemplateType);
                ControlUtils.SelectListItems(DdlTemplateType, _templateType);

                TbKeywords.Text = _keywords;

				if (Body.IsQueryExists("Delete"))
				{
					var templateId = Body.GetQueryInt("TemplateID");

					try
					{
                        var templateInfo = TemplateManager.GetTemplateInfo(PublishmentSystemId, templateId);
                        if (templateInfo != null)
                        {
                            DataProvider.TemplateDao.Delete(PublishmentSystemId, templateId);
                            Body.AddSiteLog(PublishmentSystemId,
                                $"删除{ETemplateTypeUtils.GetText(templateInfo.TemplateType)}",
                                $"模板名称:{templateInfo.TemplateName}");
                        }
						SuccessDeleteMessage();
					}
					catch(Exception ex)
					{
                        FailDeleteMessage(ex);
					}
				}
				else if (Body.IsQueryExists("SetDefault"))
				{
                    var templateId = Body.GetQueryInt("TemplateID");
			
					try
					{
                        var templateInfo = TemplateManager.GetTemplateInfo(PublishmentSystemId, templateId);
                        if (templateInfo != null)
                        {
                            DataProvider.TemplateDao.SetDefault(PublishmentSystemId, templateId);
                            Body.AddSiteLog(PublishmentSystemId,
                                $"设置默认{ETemplateTypeUtils.GetText(templateInfo.TemplateType)}",
                                $"模板名称:{templateInfo.TemplateName}");
                        }
						SuccessMessage();
					}
					catch(Exception ex)
					{
                        FailMessage(ex, "操作失败");
					}
				}

                if (string.IsNullOrEmpty(_templateType))
                {
                    LtlCommands.Text = $@"
<input type=""button"" class=""btn"" onclick=""location.href='{PageTemplateAdd.GetRedirectUrl(PublishmentSystemId, 0, ETemplateType.IndexPageTemplate)}';"" value=""添加首页模板"" />
<input type=""button"" class=""btn"" onclick=""location.href='{PageTemplateAdd.GetRedirectUrl(PublishmentSystemId, 0, ETemplateType.ChannelTemplate)}';"" value=""添加栏目模板"" />
<input type=""button"" class=""btn"" onclick=""location.href='{PageTemplateAdd.GetRedirectUrl(PublishmentSystemId, 0, ETemplateType.ContentTemplate)}';"" value=""添加内容模板"" />
<input type=""button"" class=""btn"" onclick=""location.href='{PageTemplateAdd.GetRedirectUrl(PublishmentSystemId, 0, ETemplateType.FileTemplate)}';"" value=""添加单页模板"" />
";
                }
                else
                {
                    var eTemplateType = ETemplateTypeUtils.GetEnumType(_templateType);
                    LtlCommands.Text = $@"
<input type=""button"" class=""btn btn-success"" onclick=""location.href='{PageTemplateAdd.GetRedirectUrl(PublishmentSystemId, 0, eTemplateType)}';"" value=""添加{ETemplateTypeUtils.GetText(eTemplateType)}"" />
";
                }

                DgContents.DataSource = DataProvider.TemplateDao.GetDataSource(PublishmentSystemId, _keywords, _templateType);
                DgContents.ItemDataBound += DgContents_ItemDataBound;
                DgContents.DataBind();
			}
		}

        public void DdlTemplateType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageUtils.GetCmsUrl(nameof(PageTemplate), new NameValueCollection
            {
                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                {"templateType", DdlTemplateType.SelectedValue},
                {"keywords", TbKeywords.Text}
            }));
        }

        public void btnSearch_Click(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageUtils.GetCmsUrl(nameof(PageTemplate), new NameValueCollection
            {
                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                {"templateType", DdlTemplateType.SelectedValue},
                {"keywords", TbKeywords.Text}
            }));
        }

        private void DgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var templateId = SqlUtils.EvalInt(e.Item.DataItem, "TemplateID");
                var templateType = ETemplateTypeUtils.GetEnumType(SqlUtils.EvalString(e.Item.DataItem, "TemplateType"));
                var templateName = SqlUtils.EvalString(e.Item.DataItem, "TemplateName");
                var createdFileFullName = SqlUtils.EvalString(e.Item.DataItem, "CreatedFileFullName");
                var isDefault = TranslateUtils.ToBool(SqlUtils.EvalString(e.Item.DataItem, "IsDefault"));

                var ltlTemplateName = (Literal)e.Item.FindControl("ltlTemplateName");
                var ltlFileName = (Literal)e.Item.FindControl("ltlFileName");
                var ltlUseCount = (Literal)e.Item.FindControl("ltlUseCount");
                var ltlTemplateType = (Literal)e.Item.FindControl("ltlTemplateType");
                var ltlDefaultUrl = (Literal)e.Item.FindControl("ltlDefaultUrl");
                var ltlCopyUrl = (Literal)e.Item.FindControl("ltlCopyUrl");
                var ltlLogUrl = (Literal)e.Item.FindControl("ltlLogUrl");
                var ltlCreateUrl = (Literal)e.Item.FindControl("ltlCreateUrl");
                var ltlDeleteUrl = (Literal)e.Item.FindControl("ltlDeleteUrl");

                var templateAddUrl = PageTemplateAdd.GetRedirectUrl(PublishmentSystemId, templateId, templateType);
                ltlTemplateName.Text = $@"<a href=""{templateAddUrl}"">{templateName}</a>";

                if (templateType == ETemplateType.IndexPageTemplate || templateType == ETemplateType.FileTemplate)
                {
                    var url = PageUtility.ParseNavigationUrl(PublishmentSystemInfo, createdFileFullName);
                    ltlFileName.Text = $"<a href='{url}' target='_blank'>{createdFileFullName}</a>";
                }

                ltlUseCount.Text = DataProvider.TemplateDao.GetTemplateUseCount(PublishmentSystemId, templateId, templateType, isDefault).ToString();

                ltlTemplateType.Text = ETemplateTypeUtils.GetText(templateType);

                if (templateType != ETemplateType.FileTemplate)
                {
                    if (isDefault)
                    {
                        ltlDefaultUrl.Text = "默认模板";
                    }
                    else
                    {
                        var defaultUrl = PageUtils.GetCmsUrl(nameof(PageTemplate), new NameValueCollection
                        {
                            {"PublishmentSystemID", PublishmentSystemId.ToString()},
                            {"TemplateID", templateId.ToString()},
                            {"SetDefault", true.ToString()}
                        });
                        ltlDefaultUrl.Text =
                            $@"<a href=""{defaultUrl}"" onClick=""javascript:return confirm('此操作将把此模板设为默认，确认吗？');"">设为默认</a>";
                    }
                }

                var copyUrl = PageTemplateAdd.GetRedirectUrlToCopy(PublishmentSystemId, templateId);
                ltlCopyUrl.Text = $@"<a href=""{copyUrl}"">快速复制</a>";

                var logUrl = PageTemplateLog.GetRedirectUrl(PublishmentSystemId, templateId);
                ltlLogUrl.Text = $@"<a href=""{logUrl}"">修订历史</a>";

                ltlCreateUrl.Text =
                    $@"<a href=""javascript:;"" onclick=""{ModalProgressBar.GetOpenWindowStringWithCreateByTemplate(
                        PublishmentSystemId, templateId)}"">生成页面</a>";

                if (!isDefault)
                {
                    var deleteUrl = PageUtils.GetCmsUrl(nameof(PageTemplate), new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"TemplateID", templateId.ToString()},
                        {"Delete", true.ToString()}
                    });
                    ltlDeleteUrl.Text =
                        $@"<a href=""{deleteUrl}"" onClick=""javascript:return confirm('此操作将删除模板“{templateName}”，确认吗？');"">删除</a>";
                }
            }
        }

	}
}
