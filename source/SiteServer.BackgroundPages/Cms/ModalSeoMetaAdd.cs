using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalSeoMetaAdd : BasePageCms
    {
        protected TextBox SeoMetaName;
        protected TextBox PageTitle;
        protected TextBox Keywords;
        protected TextBox Description;
        protected TextBox Copyright;
        protected TextBox Author;
        protected TextBox Email;
        protected DropDownList Language;
        protected DropDownList Charset;
        protected DropDownList Distribution;
        protected DropDownList Rating;
        protected DropDownList Robots;
        protected DropDownList RevisitAfter;
        protected TextBox Expires;

        public static string GetOpenWindowStringToAdd(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowString("添加页面元数据", PageUtils.GetCmsUrl(nameof(ModalSeoMetaAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            }), 700, 560);
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, int seoMetaId)
        {
            return PageUtils.GetOpenWindowString("修改页面元数据", PageUtils.GetCmsUrl(nameof(ModalSeoMetaAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"SeoMetaID", seoMetaId.ToString()}
            }), 700, 560);
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, SeoMetaInfo seoMetaInfo)
        {
            return PageUtils.GetOpenWindowString("修改页面元数据", PageUtils.GetCmsUrl(nameof(ModalSeoMetaAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"Author", seoMetaInfo.Author},
                {"Charset", seoMetaInfo.Charset},
                {"Copyright", seoMetaInfo.Copyright},
                {"Description", seoMetaInfo.Description},
                {"Distribution", seoMetaInfo.Distribution},
                {"Email", seoMetaInfo.Email},
                {"Expires", seoMetaInfo.Expires},
                {"Keywords", seoMetaInfo.Keywords},
                {"Language", seoMetaInfo.Language},
                {"PageTitle", seoMetaInfo.PageTitle},
                {"Rating", seoMetaInfo.Rating},
                {"RevisitAfter", seoMetaInfo.RevisitAfter},
                {"Robots", seoMetaInfo.Robots}
            }), 700, 560);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (!IsPostBack)
            {
                if (Body.IsQueryExists("SeoMetaID"))
                {
                    var seoMetaId = Body.GetQueryInt("SeoMetaID");
                    var seoMetaInfo = DataProvider.SeoMetaDao.GetSeoMetaInfo(seoMetaId);
                    if (seoMetaInfo != null)
                    {
                        SeoMetaName.Text = seoMetaInfo.SeoMetaName;
                        SeoMetaName.Enabled = false;
                        PageTitle.Text = seoMetaInfo.PageTitle;
                        Keywords.Text = seoMetaInfo.Keywords;
                        Description.Text = seoMetaInfo.Description;
                        Copyright.Text = seoMetaInfo.Copyright;
                        Author.Text = seoMetaInfo.Author;
                        Email.Text = seoMetaInfo.Email;
                        ControlUtils.SelectListItems(Language, seoMetaInfo.Language);
                        ControlUtils.SelectListItems(Charset, seoMetaInfo.Charset);
                        ControlUtils.SelectListItems(Distribution, seoMetaInfo.Distribution);
                        ControlUtils.SelectListItems(Rating, seoMetaInfo.Rating);
                        ControlUtils.SelectListItems(Robots, seoMetaInfo.Robots);
                        ControlUtils.SelectListItems(RevisitAfter, seoMetaInfo.RevisitAfter);
                        Expires.Text = seoMetaInfo.Expires;
                    }
                }
                else
                {
                    PageTitle.Text = Body.GetQueryString("PageTitle");
                    Keywords.Text = Body.GetQueryString("Keywords");
                    Description.Text = Body.GetQueryString("Description");
                    Copyright.Text = Body.GetQueryString("Copyright");
                    Author.Text = Body.GetQueryString("Author");
                    Email.Text = Body.GetQueryString("Email");
                    ControlUtils.SelectListItems(Language, Body.GetQueryString("Language"));
                    ControlUtils.SelectListItems(Charset, Body.GetQueryString("Charset"));
                    ControlUtils.SelectListItems(Distribution, Body.GetQueryString("Distribution"));
                    ControlUtils.SelectListItems(Rating, Body.GetQueryString("Rating"));
                    ControlUtils.SelectListItems(Robots, Body.GetQueryString("Robots"));
                    ControlUtils.SelectListItems(RevisitAfter, Body.GetQueryString("RevisitAfter"));
                    Expires.Text = Body.GetQueryString("Expires");
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            SeoMetaInfo seoMetaInfo;

            if (Body.IsQueryExists("SeoMetaID"))
            {
                var seoMetaId = Body.GetQueryInt("SeoMetaID");
                seoMetaInfo = DataProvider.SeoMetaDao.GetSeoMetaInfo(seoMetaId);
            }
            else
            {
                seoMetaInfo = new SeoMetaInfo {IsDefault = false};
            }

            seoMetaInfo.PublishmentSystemId = PublishmentSystemId;
            seoMetaInfo.SeoMetaName = SeoMetaName.Text;
            seoMetaInfo.PageTitle = PageTitle.Text;
            seoMetaInfo.Keywords = Keywords.Text;
            seoMetaInfo.Description = Description.Text;
            seoMetaInfo.Copyright = Copyright.Text;
            seoMetaInfo.Author = Author.Text;
            seoMetaInfo.Email = Email.Text;
            seoMetaInfo.Language = Language.SelectedValue;
            seoMetaInfo.Charset = Charset.SelectedValue;
            seoMetaInfo.Distribution = Distribution.SelectedValue;
            seoMetaInfo.Rating = Rating.SelectedValue;
            seoMetaInfo.Robots = Robots.SelectedValue;
            seoMetaInfo.RevisitAfter = RevisitAfter.SelectedValue;
            seoMetaInfo.Expires = Expires.Text;

            if (Body.IsQueryExists("SeoMetaID"))
            {
                try
                {
                    DataProvider.SeoMetaDao.Update(seoMetaInfo);
                    Body.AddSiteLog(PublishmentSystemId, "修改页面元数据", $"元数据:{seoMetaInfo.SeoMetaName}");
                    isChanged = true;
                }
                catch (Exception ex)
                {
                    FailMessage(ex, ex.Message);
                }
            }
            else
            {
                var seoMetaNameArrayList = DataProvider.SeoMetaDao.GetSeoMetaNameArrayList(PublishmentSystemId);
                if (seoMetaNameArrayList.IndexOf(SeoMetaName.Text) != -1)
                {
                    FailMessage("页面元数据添加失败，名称已存在！");
                }
                else
                {
                    try
                    {
                        DataProvider.SeoMetaDao.Insert(seoMetaInfo);
                        Body.AddSiteLog(PublishmentSystemId, "添加页面元数据", $"元数据:{seoMetaInfo.SeoMetaName}");
                        isChanged = true;
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, ex.Message);
                    }
                }
            }

            if (isChanged)
            {
                if (Body.GetQueryString("PageTitle") == null)
                {
                    PageUtils.CloseModalPage(Page);
                }
                else
                {
                    PageUtils.CloseModalPageAndRedirect(Page, PageSeoMetaList.GetRedirectUrl(PublishmentSystemId));
                }
            }
        }
    }
}