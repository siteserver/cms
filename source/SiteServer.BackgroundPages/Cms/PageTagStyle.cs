using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTagStyle : BasePageCms
    {
		public DataGrid dgContents;

        public Button AddButton;
        public Button Import;

        private string _elementName;

        public static string GetRedirectUrl(int publishmentSystemId, string elementName)
        {
            return PageUtils.GetCmsUrl(nameof(PageTagStyle), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"elementName", elementName}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            _elementName = Body.GetQueryString("elementName");

			if(!IsPostBack)
            {
                var tagTitle = TagStyleUtility.GetTagStyleTitle(_elementName);
                BreadCrumb(AppManager.Cms.LeftMenu.IdTemplate, AppManager.Cms.LeftMenu.Template.IdTagStyle, tagTitle + "样式", AppManager.Cms.Permission.WebSite.Template);

                if (Request.QueryString["Delete"] != null)
                {
                    var styleID = TranslateUtils.ToInt(Request.QueryString["StyleID"]);
                    try
                    {
                        DataProvider.TagStyleDao.Delete(styleID);
                        
                        SuccessMessage("样式删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "样式删除失败！");
                    }
                }

                AddButton.Attributes.Add("onclick", TextUtility.GetTagStyleOpenWindowStringToAdd(_elementName, PublishmentSystemId));

                InfoMessage(
                    $@"{tagTitle}标签为&lt;{_elementName} styleName=""样式名称""&gt;&lt;/{_elementName}&gt;");
                Import.Attributes.Add("onclick", ModalImport.GetOpenWindowString(PublishmentSystemId, ModalImport.TypeTagstyle));

                dgContents.DataSource = DataProvider.TagStyleDao.GetDataSource(PublishmentSystemId, _elementName);
                dgContents.ItemDataBound += dgContents_ItemDataBound;
                dgContents.DataBind();
			}
		}

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var styleID = SqlUtils.EvalInt(e.Item.DataItem, "StyleID");
                var styleName = SqlUtils.EvalString(e.Item.DataItem, "StyleName");
                var settingsXML = SqlUtils.EvalString(e.Item.DataItem, "SettingsXML");

                var ltlStyleName = (Literal)e.Item.FindControl("ltlStyleName");
                var ltlTemplateUrl = (Literal)e.Item.FindControl("ltlTemplateUrl");
                var ltlPreviewUrl = (Literal)e.Item.FindControl("ltlPreviewUrl");
                var ltlEditUrl = (Literal)e.Item.FindControl("ltlEditUrl");
                var ltlExportUrl = (Literal)e.Item.FindControl("ltlExportUrl");
                var ltlDeleteUrl = (Literal)e.Item.FindControl("ltlDeleteUrl");

                ltlStyleName.Text = styleName;

                var returnUrl = GetRedirectUrl(PublishmentSystemId, _elementName);

                ltlTemplateUrl.Text = $@"<a href=""{PageTagStyleTemplate.GetRedirectUrl(PublishmentSystemId, styleID, returnUrl)}"">自定义模板</a>";

                ltlPreviewUrl.Text = $@"<a href=""{PageTagStylePreview.GetRedirectUrl(PublishmentSystemId, styleID, returnUrl)}"">预览</a>";

                ltlEditUrl.Text =
                    $@"<a href=""javascript:;"" onclick=""{TextUtility.GetTagStyleOpenWindowStringToEdit(_elementName,
                        PublishmentSystemId, styleID)}"">编辑</a>";

                ltlExportUrl.Text =
                    $@"<a href=""javascript:;"" onclick=""{ModalExportMessage.GetOpenWindowStringToTagStyle(
                        PublishmentSystemId, styleID)}"">导出</a>";

                var deleteUrl = PageUtils.GetCmsUrl(nameof(PageTagStyle), new NameValueCollection
                {
                    {"PublishmentSystemID", PublishmentSystemId.ToString()},
                    {"elementName", _elementName},
                    {"Delete", true.ToString()},
                    {"StyleID", styleID.ToString()}
                });
                ltlDeleteUrl.Text =
                    $@"<a href=""{deleteUrl}"" onClick=""javascript:return confirm('此操作将删除样式“{styleName}”，确认吗？');"">删除</a>";
            }
        }
	}
}
