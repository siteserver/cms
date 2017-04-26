using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalStlTagAdd : BasePageCms
    {
        protected TextBox TagName;
        protected TextBox Description;
        protected TextBox Content;

        public static string GetOpenWindowString(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowString("添加自定义模板标签", PageUtils.GetCmsUrl(nameof(ModalStlTagAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            }), 580, 520);
        }

        public static string GetOpenWindowString(int publishmentSystemId, string tagName)
        {
            return PageUtils.GetOpenWindowString("修改自定义模板标签", PageUtils.GetCmsUrl(nameof(ModalStlTagAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"TagName", tagName}
            }), 580, 520);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                if (Body.IsQueryExists("TagName"))
                {
                    var tagName = Body.GetQueryString("TagName");
                    var stlTagInfo = DataProvider.StlTagDao.GetStlTagInfo(PublishmentSystemId, tagName);
                    if (stlTagInfo != null)
                    {
                        TagName.Text = stlTagInfo.TagName;
                        TagName.Enabled = false;
                        Description.Text = stlTagInfo.TagDescription;
                        Content.Text = stlTagInfo.TagContent;
                    }
                }

            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            var stlTagInfo = new StlTagInfo();
            stlTagInfo.TagName = TagName.Text;
            stlTagInfo.PublishmentSystemID = PublishmentSystemId;
            stlTagInfo.TagDescription = Description.Text;
            stlTagInfo.TagContent = Content.Text;

            if (Body.IsQueryExists("TagName"))
            {
                try
                {
                    DataProvider.StlTagDao.Update(stlTagInfo);
                    Body.AddSiteLog(PublishmentSystemId, "修改自定义模板语言", $"模板标签名:{stlTagInfo.TagName}");
                    isChanged = true;
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "自定义模板标签修改失败！");
                }
            }
            else
            {
                var stlTagNameArrayList = DataProvider.StlTagDao.GetStlTagNameArrayList(PublishmentSystemId);
                if (stlTagNameArrayList.IndexOf(TagName.Text) != -1)
                {
                    FailMessage("自定义模板标签添加失败，自定义模板标签名已存在！");
                }
                else
                {
                    try
                    {
                        DataProvider.StlTagDao.Insert(stlTagInfo);
                        Body.AddSiteLog(PublishmentSystemId, "添加自定义模板语言", $"模板标签名:{stlTagInfo.TagName}");
                        isChanged = true;
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "自定义模板标签添加失败！");
                    }
                }
            }

            if (isChanged)
            {
                PageUtils.CloseModalPage(Page);
            }
        }
    }
}