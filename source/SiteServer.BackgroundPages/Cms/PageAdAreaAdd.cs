using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageAdAreaAdd : BasePageCms
    {
        public Literal ltlPageTitle;
        public TextBox AdAreaName;
        public RadioButtonList IsEnabled;
        public TextBox Width;
        public TextBox Height;
        public TextBox Summary;

        private bool _isEdit;
        private string _theAdAreaName;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageAdAreaAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (Body.IsQueryExists("AdAreaName"))
            {
                _isEdit = true;
                _theAdAreaName = Body.GetQueryString("AdAreaName");
            }

            if (!Page.IsPostBack)
            {
                var pageTitle = _isEdit ? "编辑广告位" : "添加广告位";
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdAdvertisement, pageTitle, AppManager.Cms.Permission.WebSite.Advertisement);

                ltlPageTitle.Text = pageTitle;

                EBooleanUtils.AddListItems(IsEnabled);
                ControlUtils.SelectListItems(IsEnabled, true.ToString());
                if (_isEdit)
                {
                    var adAreaInfo = DataProvider.AdAreaDao.GetAdAreaInfo(_theAdAreaName, PublishmentSystemId);
                    AdAreaName.Text = adAreaInfo.AdAreaName;
                    IsEnabled.SelectedValue = adAreaInfo.IsEnabled.ToString();
                     
                    Width.Text = adAreaInfo.Width.ToString();
                    Height.Text = adAreaInfo.Height.ToString();
                    Summary.Text = adAreaInfo.Summary;
                }
            }

            SuccessMessage(string.Empty);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                if (_isEdit == false)
                {
                    if (DataProvider.AdAreaDao.IsExists(AdAreaName.Text, PublishmentSystemId))
                    {
                        FailMessage($"名称为“{AdAreaName.Text}”的广告位已存在，请更改广告位名称！");
                        return;
                    }
                }
                try
                {
                    if (_isEdit)
                    { 
                        var adAreaInfo = DataProvider.AdAreaDao.GetAdAreaInfo(_theAdAreaName, PublishmentSystemId);
                        adAreaInfo.AdAreaName = AdAreaName.Text;
                        adAreaInfo.IsEnabled = TranslateUtils.ToBool(IsEnabled.SelectedValue);
                        adAreaInfo.Width = TranslateUtils.ToInt(Width.Text);
                        adAreaInfo.Height = TranslateUtils.ToInt(Height.Text);
                        adAreaInfo.Summary = Summary.Text;
                        
                        if (adAreaInfo.AdAreaName != AdAreaName.Text.Trim())
                        {
                            if (DataProvider.AdAreaDao.IsExists(AdAreaName.Text, PublishmentSystemId))
                            {
                                FailMessage($"名称为“{AdAreaName.Text}”的广告位已存在，请更改广告位名称！");
                                return;
                            }
                        }

                        DataProvider.AdAreaDao.Update(adAreaInfo);
                        Body.AddSiteLog(PublishmentSystemId, "修改固定广告", $"广告名称：{adAreaInfo.AdAreaName}");
                        SuccessMessage("修改广告成功！");
                    }
                    else
                    {
                         var adAreaInfo = new AdAreaInfo(0, PublishmentSystemId, AdAreaName.Text, TranslateUtils.ToInt(Width.Text), TranslateUtils.ToInt(Height.Text), Summary.Text, TranslateUtils.ToBool(IsEnabled.SelectedValue), DateTime.Now);
                         DataProvider.AdAreaDao.Insert(adAreaInfo);
                        Body.AddSiteLog(PublishmentSystemId, "新增固定广告位", $"广告位名称：{adAreaInfo.AdAreaName}");
                        SuccessMessage("新增广告位成功！");
                    }

                    AddWaitAndRedirectScript(PageAdArea.GetRedirectUrl(PublishmentSystemId));
                }
                catch (Exception ex)
                {
                    FailMessage(ex, $"操作失败：{ex.Message}");
                }
            }
        }
    }
}
