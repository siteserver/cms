using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageMapAdd : BasePageCms
    {
        public Literal LtlPageTitle;

        public PlaceHolder PhStep1;
        public TextBox TbKeywords;
        public TextBox TbTitle;
        public TextBox TbSummary;
        public CheckBox CbIsEnabled;
        public Literal LtlImageUrl;

        public PlaceHolder PhStep2;
        public TextBox TbMapWd;
        public Literal LtlMap;

        public HtmlInputHidden ImageUrl;

        public Button BtnSubmit;
        public Button BtnReturn;

        private int _mapId;

        public static string GetRedirectUrl(int publishmentSystemId, int mapId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageMapAdd), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"mapId", mapId.ToString()}
            });
        }

        public string GetUploadUrl()
        {
            return AjaxUpload.GetImageUrlUploadUrl(PublishmentSystemId);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemId");
            _mapId = Body.GetQueryInt("mapID");

            if (!IsPostBack)
            {
                var pageTitle = _mapId > 0 ? "编辑微导航" : "添加微导航";
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdMap, pageTitle, AppManager.WeiXin.Permission.WebSite.Map);
                LtlPageTitle.Text = pageTitle;

                LtlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{MapManager.GetImageUrl(PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";

                if (_mapId > 0)
                {
                    var mapInfo = DataProviderWx.MapDao.GetMapInfo(_mapId);

                    TbKeywords.Text = DataProviderWx.KeywordDao.GetKeywords(mapInfo.KeywordId);
                    CbIsEnabled.Checked = !mapInfo.IsDisabled;
                    TbTitle.Text = mapInfo.Title;
                    if (!string.IsNullOrEmpty(mapInfo.ImageUrl))
                    {
                        LtlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, mapInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    TbSummary.Text = mapInfo.Summary;

                    TbMapWd.Text = mapInfo.MapWd;

                    ImageUrl.Value = mapInfo.ImageUrl;
                }

                BtnReturn.Attributes.Add("onclick",
                    $@"location.href=""{PageMap.GetRedirectUrl(PublishmentSystemId)}"";return false");
            }
        }

        public void Preview_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "K", "<script>window.open(\"http://map.baidu.com/mobile/webapp/place/list/qt=s&wd=" + TbMapWd.Text + "/vt=map\");</script>");
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                var selectedStep = 0;
                if (PhStep1.Visible)
                {
                    selectedStep = 1;
                }
                else if (PhStep2.Visible)
                {
                    selectedStep = 2;
                }

                PhStep1.Visible = PhStep2.Visible = false;

                if (selectedStep == 1)
                {
                    var isConflict = false;
                    var conflictKeywords = string.Empty;
                    if (!string.IsNullOrEmpty(TbKeywords.Text))
                    {
                        if (_mapId > 0)
                        {
                            var mapInfo = DataProviderWx.MapDao.GetMapInfo(_mapId);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemId, mapInfo.KeywordId, PageUtils.FilterXss(TbKeywords.Text), out conflictKeywords);
                        }
                        else
                        {
                            isConflict = KeywordManager.IsKeywordInsertConflict(PublishmentSystemId, PageUtils.FilterXss(TbKeywords.Text), out conflictKeywords);
                        }
                    }

                    if (isConflict)
                    {
                        FailMessage($"触发关键词“{conflictKeywords}”已存在，请设置其他关键词");
                        PhStep1.Visible = true;
                    }
                    else
                    {
                        PhStep2.Visible = true;
                        BtnSubmit.Text = "确 认";
                    }
                }
                else if (selectedStep == 2)
                {
                    var mapInfo = new MapInfo();
                    if (_mapId > 0)
                    {
                        mapInfo = DataProviderWx.MapDao.GetMapInfo(_mapId);
                    }
                    mapInfo.PublishmentSystemId = PublishmentSystemId;

                    mapInfo.KeywordId = DataProviderWx.KeywordDao.GetKeywordId(PublishmentSystemId, _mapId > 0, TbKeywords.Text, EKeywordType.Map, mapInfo.KeywordId);
                    mapInfo.IsDisabled = !CbIsEnabled.Checked;

                    mapInfo.Title = PageUtils.FilterXss(TbTitle.Text);
                    mapInfo.ImageUrl = ImageUrl.Value; ;
                    mapInfo.Summary = TbSummary.Text;

                    mapInfo.MapWd = TbMapWd.Text;

                    try
                    {
                        if (_mapId > 0)
                        {
                            DataProviderWx.MapDao.Update(mapInfo);

                            LogUtils.AddAdminLog(Body.AdministratorName, "修改微导航", $"微导航:{TbTitle.Text}");
                            SuccessMessage("修改微导航成功！");
                        }
                        else
                        {
                            _mapId = DataProviderWx.MapDao.Insert(mapInfo);

                            LogUtils.AddAdminLog(Body.AdministratorName, "添加微导航", $"微导航:{TbTitle.Text}");
                            SuccessMessage("添加微导航成功！");
                        }

                        var redirectUrl = PageMap.GetRedirectUrl(PublishmentSystemId);
                        AddWaitAndRedirectScript(redirectUrl);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "微导航设置失败！");
                    }

                    BtnSubmit.Visible = false;
                    BtnReturn.Visible = false;
                }
            }
        }
    }
}
