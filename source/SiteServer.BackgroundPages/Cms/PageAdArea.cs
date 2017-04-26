using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageAdArea : BasePageCms
    {
        public TextBox AdAreaName;
        public DataGrid dgContents;

        public Button AddAdArea;
        public Button Delete;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageAdArea), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }
         
        public string GetIsEnabled(string isEnabledStr)
        {
            return StringUtils.GetTrueOrFalseImageHtml(isEnabledStr);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (Body.IsQueryExists("Delete"))
            {
                var adAreaName = Body.GetQueryString("AdAreaName");
                try
                {
                    DataProvider.AdAreaDao.Delete(adAreaName, PublishmentSystemId);

                    Body.AddSiteLog(PublishmentSystemId, "删除广告位", $"广告名称：{adAreaName}");

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
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdAdvertisement, "固定广告位管理", AppManager.Cms.Permission.WebSite.Advertisement);
                
                Delete.Attributes.Add("onclick", "return confirm(\"此操作将删除所选广告位，确定吗？\");");
            }
        }

        public void BindGrid()
        {
            try
            {
                if (string.IsNullOrEmpty(AdAreaName.Text))
                {
                    dgContents.DataSource = DataProvider.AdAreaDao.GetDataSource(PublishmentSystemId);
                }
                else
                {
                    dgContents.DataSource = DataProvider.AdAreaDao.GetDataSourceByName(AdAreaName.Text, PublishmentSystemId);
                }
                dgContents.DataBind();
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage(ex.Message);
            }
        }

        public void ReFresh(object sender, EventArgs e)
        {
            BindGrid();
        }

        public void AddAdArea_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageAdAreaAdd.GetRedirectUrl(PublishmentSystemId));
        }

        public void Delete_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                if (Request.Form["AdAreaNameCollection"] != null)
                {
                    var arraylist = TranslateUtils.StringCollectionToStringList(Request.Form["AdAreaNameCollection"]);
                    try
                    {
                        foreach (string adAreaName in arraylist)
                        {
                            DataProvider.AdAreaDao.Delete(adAreaName, PublishmentSystemId);
                        }

                        Body.AddSiteLog(PublishmentSystemId, "删除广告位", $"广告名称：{Request.Form["AdAreaNameCollection"]}");

                        SuccessDeleteMessage();
                    }
                    catch (Exception ex)
                    {
                        FailDeleteMessage(ex);
                    }
                    BindGrid();
                }
                else
                {
                    FailMessage("请选择广告位后进行操作！");
                }
            }
        }
    }
}
