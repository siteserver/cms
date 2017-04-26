using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageAdMaterial : BasePageCms
    {
        public DataGrid dgContents;

        public Button AddAdMaterial;
        public Button Delete;

        private int _advId;

        public static string GetRedirectUrl(int publishmentSystemId, int advId)
        {
            return PageUtils.GetCmsUrl(nameof(PageAdMaterial), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"AdvID", advId.ToString()}
            });
        }

        public string GetAdvName(int advId)
        {
            var advName = string.Empty;
            var advInfo = DataProvider.AdvDao.GetAdvInfo(advId,PublishmentSystemId);
            if (advInfo != null)
            {
                advName = advInfo.AdvName;
            }
            return advName;
        }

        public string GetAdMaterialType(string adTypeStr)
        {
            var adType = EAdvTypeUtils.GetEnumType(adTypeStr);
            return EAdvTypeUtils.GetText(adType);
        }
        
        public string GetIsEnabled(string isEnabledStr)
        {
            return StringUtils.GetTrueOrFalseImageHtml(isEnabledStr);
        }

        public string GetEditUrl(int adMaterialId)
        {
            return
                $@"<a href=""javascript:;"" onclick=""{ModalAdMaterialAdd.GetOpenWindowStringToEdit(adMaterialId,
                    PublishmentSystemId, _advId)}"">编辑</a>";
         }

        public void Page_Load(object sender, EventArgs e)
        {
            PageUtils.CheckRequestParameter("PublishmentSystemID");
            _advId = Body.GetQueryInt("AdvID");

            if (Body.IsQueryExists("Delete"))
            {
                var adMaterialId = Body.GetQueryInt("AdMaterialID");
                try
                {
                    DataProvider.AdMaterialDao.Delete(adMaterialId, PublishmentSystemId);

                    Body.AddSiteLog(PublishmentSystemId, "删除广告物料");

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
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdAdvertisement, "固定广告物料管理", AppManager.Cms.Permission.WebSite.Advertisement);
                
                AddAdMaterial.Attributes.Add("onclick", ModalAdMaterialAdd.GetOpenWindowStringToAdd(0, PublishmentSystemId, _advId));
                Delete.Attributes.Add("onclick", "return confirm(\"此操作将删除所选广告物料，确定吗？\");");
            }
        }

        public void BindGrid()
        {
            try
            {
                dgContents.DataSource = DataProvider.AdMaterialDao.GetDataSource(_advId,PublishmentSystemId);
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
         
        public void Delete_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                if (Request.Form["AdMaterialIDCollection"] != null)
                {
                    var arraylist = TranslateUtils.StringCollectionToStringList(Request.Form["AdMaterialIDCollection"]);
                    try
                    {
                        foreach (string adMaterialID in arraylist)
                        {
                            DataProvider.AdMaterialDao.Delete(TranslateUtils.ToInt(adMaterialID), PublishmentSystemId);
                        }

                        Body.AddSiteLog(PublishmentSystemId, "删除广告物料");

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
                    FailMessage("请选择广告后进行操作！");
                }
            }
        }
    }
}
