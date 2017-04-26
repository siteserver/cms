using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageSeoMetaList : BasePageCms
    {
        public DataGrid dgContents;
		public Button AddSeoMeta;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageSeoMetaList), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

		public bool IsSetDefaultable(string isDefault)
		{
            return !TranslateUtils.ToBool(isDefault);
		}

		public bool IsDefault(string isDefault)
		{
            return TranslateUtils.ToBool(isDefault);
		}

		public string GetPageTitle(string pageTitle)
		{
			return StringUtils.MaxLengthText(pageTitle, 30);
		}

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			PageUtils.CheckRequestParameter("PublishmentSystemID");

			if (Body.IsQueryExists("Delete"))
			{
				var seoMetaId = Body.GetQueryInt("SeoMetaID");

				try
				{
                    var metaInfo = DataProvider.SeoMetaDao.GetSeoMetaInfo(seoMetaId);
                    if (metaInfo != null)
                    {
                        DataProvider.SeoMetaDao.Delete(seoMetaId);
                        Body.AddSiteLog(PublishmentSystemId, "删除页面元数据", $"页面元数据:{metaInfo.SeoMetaName}");
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
				var seoMetaId = Body.GetQueryInt("SeoMetaID");
                var isSetDefault = Body.GetQueryBool("SetDefault");
			
				try
				{
					if (isSetDefault)
					{
						DataProvider.SeoMetaDao.SetDefault(PublishmentSystemId, seoMetaId, true);
					}
					else
					{
                        DataProvider.SeoMetaDao.SetDefault(PublishmentSystemId, seoMetaId, false);
					}
					SuccessMessage();
				}
				catch(Exception ex)
				{
					FailMessage(ex, "操作失败");
				}
			}

			if (!IsPostBack)
			{
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdSeo, "页面元数据(Meta) / 页面元数据设置", AppManager.Cms.Permission.WebSite.Seo);

                dgContents.DataSource = DataProvider.SeoMetaDao.GetDataSource(PublishmentSystemId);
                dgContents.ItemDataBound += dgContents_ItemDataBound;
                dgContents.DataBind();

				AddSeoMeta.Attributes.Add("onclick", ModalSeoMetaAdd.GetOpenWindowStringToAdd(PublishmentSystemId));
			}
		}

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var seoMetaID = SqlUtils.EvalInt(e.Item.DataItem, "SeoMetaID");
                var seoMetaName = SqlUtils.EvalString(e.Item.DataItem, "SeoMetaName");
                var pageTitle = SqlUtils.EvalString(e.Item.DataItem, "PageTitle");
                var isDefault = TranslateUtils.ToBool(SqlUtils.EvalString(e.Item.DataItem, "IsDefault"));

                var ltlSeoMetaName = e.Item.FindControl("ltlSeoMetaName") as Literal;
                var ltlPageTitle = e.Item.FindControl("ltlPageTitle") as Literal;
                var ltlIsDefault = e.Item.FindControl("ltlIsDefault") as Literal;
                var hlViewLink = e.Item.FindControl("hlViewLink") as HyperLink;
                var hlEditLink = e.Item.FindControl("hlEditLink") as HyperLink;
                var ltlDefaultUrl = e.Item.FindControl("ltlDefaultUrl") as Literal;
                var ltlDeleteUrl = e.Item.FindControl("ltlDeleteUrl") as Literal;

                ltlSeoMetaName.Text = seoMetaName;
                ltlPageTitle.Text = pageTitle;
                ltlIsDefault.Text = StringUtils.GetTrueImageHtml(isDefault);
                hlViewLink.Attributes.Add("onclick", ModalSeoMetaView.GetOpenWindowString(PublishmentSystemId, seoMetaID));
                hlEditLink.Attributes.Add("onclick", ModalSeoMetaAdd.GetOpenWindowStringToEdit(PublishmentSystemId, seoMetaID));
                if (!isDefault)
                {
                    var urlDefault = PageUtils.GetCmsUrl(nameof(PageSeoMetaList), new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString() },
                        {"SeoMetaID", seoMetaID.ToString() },
                        {"SetDefault", true.ToString() }
                    });
                    ltlDefaultUrl.Text =
                        $@"<a href=""{urlDefault}"" onClick=""javascript:return confirm('此操作将把此项设为默认，确认吗？');"">设为默认</a>";
                }
                else
                {
                    var urlDefault = PageUtils.GetCmsUrl(nameof(PageSeoMetaList), new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString() },
                        {"SeoMetaID", seoMetaID.ToString() },
                        {"SetDefault", false.ToString() }
                    });
                    ltlDefaultUrl.Text =
                        $@"<a href=""{urlDefault}"" onClick=""javascript:return confirm('此操作将取消默认，确认吗？');"">取消默认</a>";
                }

                var urlDelete = PageUtils.GetCmsUrl(nameof(PageSeoMetaList), new NameValueCollection
                {
                    {"PublishmentSystemID", PublishmentSystemId.ToString()},
                    {"SeoMetaID", seoMetaID.ToString()},
                    {"Delete", true.ToString()}
                });
                ltlDeleteUrl.Text =
                    $@"<a href=""{urlDelete}"" onClick=""javascript:return confirm('此操作将删除页面元数据“{seoMetaName}”，确认吗？');"">删除</a>";
            }
		}
	}
}
