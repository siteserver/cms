using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;
using MapManager = SiteServer.WeiXin.Core.MapManager;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundMapAdd : BackgroundBasePageWX
    {
        public Literal ltlPageTitle;

        public PlaceHolder phStep1;
        public TextBox tbKeywords;
        public TextBox tbTitle;
        public TextBox tbSummary;
        public CheckBox cbIsEnabled;
        public Literal ltlImageUrl;

        public PlaceHolder phStep2;
        public TextBox tbMapWD;
        public Literal ltlMap;

        public HtmlInputHidden imageUrl;

        public Button btnSubmit;
        public Button btnReturn;

        private int mapID;

        public static string GetRedirectUrl(int publishmentSystemID, int mapID)
        {
            return PageUtils.GetWXUrl($"background_mapAdd.aspx?publishmentSystemID={publishmentSystemID}&mapID={mapID}");
        }

        public string GetUploadUrl()
        {
            return BackgroundAjaxUpload.GetImageUrlUploadUrl(PublishmentSystemID);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            mapID = TranslateUtils.ToInt(GetQueryString("mapID"));

            if (!IsPostBack)
            {
                var pageTitle = mapID > 0 ? "编辑微导航" : "添加微导航";
                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Map, pageTitle, AppManager.WeiXin.Permission.WebSite.Map);
                ltlPageTitle.Text = pageTitle;

                ltlImageUrl.Text =
                    $@"<img id=""preview_imageUrl"" src=""{MapManager.GetImageUrl(PublishmentSystemInfo, string.Empty)}"" width=""370"" align=""middle"" />";

                if (mapID > 0)
                {
                    var mapInfo = DataProviderWX.MapDAO.GetMapInfo(mapID);

                    tbKeywords.Text = DataProviderWX.KeywordDAO.GetKeywords(mapInfo.KeywordID);
                    cbIsEnabled.Checked = !mapInfo.IsDisabled;
                    tbTitle.Text = mapInfo.Title;
                    if (!string.IsNullOrEmpty(mapInfo.ImageUrl))
                    {
                        ltlImageUrl.Text =
                            $@"<img id=""preview_imageUrl"" src=""{PageUtility.ParseNavigationUrl(
                                PublishmentSystemInfo, mapInfo.ImageUrl)}"" width=""370"" align=""middle"" />";
                    }
                    tbSummary.Text = mapInfo.Summary;

                    tbMapWD.Text = mapInfo.MapWD;

                    imageUrl.Value = mapInfo.ImageUrl;
                }

                btnReturn.Attributes.Add("onclick",
                    $@"location.href=""{BackgroundMap.GetRedirectUrl(PublishmentSystemID)}"";return false");
            }
        }

        public void Preview_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "K", "<script>window.open(\"http://map.baidu.com/mobile/webapp/place/list/qt=s&wd=" + tbMapWD.Text + "/vt=map\");</script>");
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                var selectedStep = 0;
                if (phStep1.Visible)
                {
                    selectedStep = 1;
                }
                else if (phStep2.Visible)
                {
                    selectedStep = 2;
                }

                phStep1.Visible = phStep2.Visible = false;

                if (selectedStep == 1)
                {
                    var isConflict = false;
                    var conflictKeywords = string.Empty;
                    if (!string.IsNullOrEmpty(tbKeywords.Text))
                    {
                        if (mapID > 0)
                        {
                            var mapInfo = DataProviderWX.MapDAO.GetMapInfo(mapID);
                            isConflict = KeywordManager.IsKeywordUpdateConflict(PublishmentSystemID, mapInfo.KeywordID, PageUtils.FilterXSS(tbKeywords.Text), out conflictKeywords);
                        }
                        else
                        {
                            isConflict = KeywordManager.IsKeywordInsertConflict(PublishmentSystemID, PageUtils.FilterXSS(tbKeywords.Text), out conflictKeywords);
                        }
                    }

                    if (isConflict)
                    {
                        FailMessage($"触发关键词“{conflictKeywords}”已存在，请设置其他关键词");
                        phStep1.Visible = true;
                    }
                    else
                    {
                        phStep2.Visible = true;
                        btnSubmit.Text = "确 认";
                    }
                }
                else if (selectedStep == 2)
                {
                    var mapInfo = new MapInfo();
                    if (mapID > 0)
                    {
                        mapInfo = DataProviderWX.MapDAO.GetMapInfo(mapID);
                    }
                    mapInfo.PublishmentSystemID = PublishmentSystemID;

                    mapInfo.KeywordID = DataProviderWX.KeywordDAO.GetKeywordID(PublishmentSystemID, mapID > 0, tbKeywords.Text, EKeywordType.Map, mapInfo.KeywordID);
                    mapInfo.IsDisabled = !cbIsEnabled.Checked;

                    mapInfo.Title = PageUtils.FilterXSS(tbTitle.Text);
                    mapInfo.ImageUrl = imageUrl.Value; ;
                    mapInfo.Summary = tbSummary.Text;

                    mapInfo.MapWD = tbMapWD.Text;

                    try
                    {
                        if (mapID > 0)
                        {
                            DataProviderWX.MapDAO.Update(mapInfo);

                            LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "修改微导航",
                                $"微导航:{tbTitle.Text}");
                            SuccessMessage("修改微导航成功！");
                        }
                        else
                        {
                            mapID = DataProviderWX.MapDAO.Insert(mapInfo);

                            LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "添加微导航",
                                $"微导航:{tbTitle.Text}");
                            SuccessMessage("添加微导航成功！");
                        }

                        var redirectUrl = PageUtils.GetWXUrl(
                            $"background_map.aspx?publishmentSystemID={PublishmentSystemID}");
                        AddWaitAndRedirectScript(redirectUrl);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "微导航设置失败！");
                    }

                    btnSubmit.Visible = false;
                    btnReturn.Visible = false;
                }
            }
        }
    }
}
