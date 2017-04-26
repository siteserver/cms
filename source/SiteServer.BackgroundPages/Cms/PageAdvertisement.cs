using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageAdvertisement : BasePageCms
    {
		public DropDownList AdvertisementType;
        public DataGrid dgContents;

		public Button AddAdvertisement;
		public Button Delete;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageAdvertisement), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public string GetAdvertisementType(string advertisementTypeStr)
		{
			var adType = EAdvertisementTypeUtils.GetEnumType(advertisementTypeStr);
			return EAdvertisementTypeUtils.GetText(adType);
		}

        public string GetAdvertisementInString(string advertisementName)
		{
            var builder = new StringBuilder();
            var adInfo = DataProvider.AdvertisementDao.GetAdvertisementInfo(advertisementName, PublishmentSystemId);
            if (!string.IsNullOrEmpty(adInfo.NodeIDCollectionToChannel))
            {
                builder.Append("栏目：");
                var nodeIDArrayList = TranslateUtils.StringCollectionToIntList(adInfo.NodeIDCollectionToChannel);
                foreach (int nodeID in nodeIDArrayList)
                {
                    builder.Append(NodeManager.GetNodeName(PublishmentSystemId, nodeID));
                    builder.Append(",");
                }
                builder.Length--;
            }
            if (!string.IsNullOrEmpty(adInfo.NodeIDCollectionToContent))
            {
                if (builder.Length > 0)
                {
                    builder.Append("<br />");
                }
                builder.Append("内容：");
                var nodeIDArrayList = TranslateUtils.StringCollectionToIntList(adInfo.NodeIDCollectionToContent);
                foreach (int nodeID in nodeIDArrayList)
                {
                    builder.Append(NodeManager.GetNodeName(PublishmentSystemId, nodeID));
                    builder.Append(",");
                }
                builder.Length--;
            }
            if (!string.IsNullOrEmpty(adInfo.FileTemplateIDCollection))
            {
                if (builder.Length > 0)
                {
                    builder.Append("<br />");
                }
                builder.Append("单页：");
                var fileTemplateIDArrayList = TranslateUtils.StringCollectionToIntList(adInfo.FileTemplateIDCollection);
                foreach (int fileTemplateID in fileTemplateIDArrayList)
                {
                    builder.Append(TemplateManager.GetCreatedFileFullName(PublishmentSystemId, fileTemplateID));
                    builder.Append(",");
                }
                builder.Length--;
            }
            return builder.ToString();
		}

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (Body.IsQueryExists("Delete"))
            {
                var advertisementName = Body.GetQueryString("AdvertisementName");
                try
                {
                    DataProvider.AdvertisementDao.Delete(advertisementName, PublishmentSystemId);

                    Body.AddSiteLog(PublishmentSystemId, "删除漂浮广告", $"广告名称：{advertisementName}");

                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }
            BindGrid();

			if (!Page.IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdAdvertisement, "漂浮广告管理", AppManager.Cms.Permission.WebSite.Advertisement);

				AdvertisementType.Items.Add(new ListItem("<所有类型>", string.Empty));
				EAdvertisementTypeUtils.AddListItems(AdvertisementType);
				ControlUtils.SelectListItems(AdvertisementType, string.Empty);

				Delete.Attributes.Add("onclick","return confirm(\"此操作将删除所选广告，确定吗？\");");
			}
		}

		public void BindGrid()
		{
			try
			{
                if (string.IsNullOrEmpty(AdvertisementType.SelectedValue))
                {
                    dgContents.DataSource = DataProvider.AdvertisementDao.GetDataSource(PublishmentSystemId);
                }
                else
                {
                    dgContents.DataSource = DataProvider.AdvertisementDao.GetDataSourceByType(EAdvertisementTypeUtils.GetEnumType(AdvertisementType.SelectedValue), PublishmentSystemId);
                }

                dgContents.DataBind();
			}
			catch(Exception ex)
			{
                PageUtils.RedirectToErrorPage(ex.Message);
			}
		}

		public void ReFresh(object sender, EventArgs e)
		{
			BindGrid();
		}


		public void AddAdvertisement_OnClick(object sender, EventArgs e)
		{
            PageUtils.Redirect(PageAdvertisementAdd.GetRedirectUrl(PublishmentSystemId));
		}

		public void Delete_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
				if (Request.Form["AdvertisementNameCollection"] != null)
				{
					var arraylist = TranslateUtils.StringCollectionToStringList(Request.Form["AdvertisementNameCollection"]);
					try
					{
						foreach (string advertisementName in arraylist)
						{
                            DataProvider.AdvertisementDao.Delete(advertisementName, PublishmentSystemId);
						}

                        Body.AddSiteLog(PublishmentSystemId, "删除漂浮广告", $"广告名称：{Request.Form["AdvertisementNameCollection"]}");

						SuccessDeleteMessage();
					}
					catch(Exception ex)
					{
                        FailDeleteMessage(ex);
					}
					BindGrid();
				}
				else
				{
                    FailMessage("请选择广告后进行操作！");
				}
			}
		}
	}
}
