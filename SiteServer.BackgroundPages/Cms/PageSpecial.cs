using System;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageSpecial : BasePageCms
    {
        protected TextBox TbKeyword;
        protected Repeater RptContents;
        protected Button BtnAdd;

        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageSpecial), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            var specialId = AuthRequest.GetQueryInt("specialId");
            var keyword = AuthRequest.GetQueryString("keyword");

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Template);

            TbKeyword.Text = keyword;

            if (specialId > 0)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["delete"]))
                {
                    var specialInfo = SpecialManager.DeleteSpecialInfo(SiteId, specialId);

                    AuthRequest.AddSiteLog(SiteId,
                        "删除专题",
                        $"专题名称:{specialInfo.Title}");

                    SuccessDeleteMessage();
                }
                else if (!string.IsNullOrEmpty(Request.QueryString["download"]))
                {
                    var specialInfo = SpecialManager.GetSpecialInfo(SiteId, specialId);
                    var directoryPath = SpecialManager.GetSpecialDirectoryPath(SiteInfo, specialInfo.Url);
                    var zipFilePath = SpecialManager.GetSpecialZipFilePath(directoryPath);
                    PageUtils.Download(Response, zipFilePath, $"{specialInfo.Title}.zip");
                    return;
                }
            }

            RptContents.DataSource = string.IsNullOrEmpty(keyword)
                ? DataProvider.SpecialDao.GetSpecialInfoList(SiteId)
                : DataProvider.SpecialDao.GetSpecialInfoList(SiteId, keyword);
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            BtnAdd.Attributes.Add("onclick", ModalSpecialAdd.GetOpenWindowString(SiteId));
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var specialInfo = (SpecialInfo)e.Item.DataItem;

            var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
            var ltlUrl = (Literal)e.Item.FindControl("ltlUrl");
            var ltlAddDate = (Literal)e.Item.FindControl("ltlAddDate");
            var ltlActions = (Literal)e.Item.FindControl("ltlActions");

            ltlTitle.Text = $@"<a href=""{SpecialManager.GetSpecialUrl(SiteInfo, specialInfo.Url)}"" target=""_blank"">{specialInfo.Title}</a>";
            ltlUrl.Text = specialInfo.Url;
            ltlAddDate.Text = specialInfo.AddDate.ToString("yyyy-MM-dd HH:mm");

            ltlActions.Text = $@"
<a class=""m-r-10"" href=""javascript:;"" onclick=""{ModalSpecialAdd.GetOpenWindowString(SiteId, specialInfo.Id)}"">编辑</a>
<a class=""m-r-10"" href=""javascript:;"" onclick=""{ModalSpecialUpload.GetOpenWindowString(SiteId, specialInfo.Id)}"">上传压缩包</a>
<a class=""m-r-10"" href=""{GetRedirectUrl(SiteId)}&specialId={specialInfo.Id}&download={true}"">下载压缩包</a>
<a class=""m-r-10"" onclick=""{AlertUtils.ConfirmDelete("删除专题", $"此操作将删除专题“{specialInfo.Title}”及相关文件，确认吗？", $"{GetRedirectUrl(SiteId)}&specialId={specialInfo.Id}&delete={true}")}"" href=""javascript:;"">删除</a>
";
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            Page.Response.Redirect(PageUrl);
        }

        private string _pageUrl;
        private string PageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_pageUrl))
                {
                    _pageUrl = $"{GetRedirectUrl(SiteId)}&keyword={TbKeyword.Text}";
                }
                return _pageUrl;
            }
        }
    }
}
