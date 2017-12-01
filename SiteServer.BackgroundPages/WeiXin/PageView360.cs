using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageView360 : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;

        public Button BtnAdd;
        public Button BtnDelete;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageView360), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]))
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWx.View360Dao.Delete(list);
                        SuccessMessage("360全景删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "360全景删除失败！");
                    }
                }
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = 30;
            
            SpContents.SelectCommand = DataProviderWx.View360Dao.GetSelectString(PublishmentSystemId);
            SpContents.SortField = View360Attribute.Id;
            SpContents.SortMode = SortMode.ASC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            {

                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdView360, "360全景", AppManager.WeiXin.Permission.WebSite.View360);
                SpContents.DataBind();

                var urlAdd = PageView360Add.GetRedirectUrl(PublishmentSystemId, 0);
                BtnAdd.Attributes.Add("onclick", $"location.href='{urlAdd}';return false");

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemId), "Delete", "True");
                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的360全景活动", "此操作将删除所选360全景活动，确认吗？"));
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var view360Info = new View360Info(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlTitle = e.Item.FindControl("ltlTitle") as Literal;
                var ltlKeywords = e.Item.FindControl("ltlKeywords") as Literal;
                var ltlPvCount = e.Item.FindControl("ltlPVCount") as Literal;
                var ltlIsEnabled = e.Item.FindControl("ltlIsEnabled") as Literal;
                var ltlPreviewUrl = e.Item.FindControl("ltlPreviewUrl") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlTitle.Text = view360Info.Title;
                ltlKeywords.Text = DataProviderWx.KeywordDao.GetKeywords(view360Info.KeywordId);
                ltlPvCount.Text = view360Info.PvCount.ToString();

                ltlIsEnabled.Text = StringUtils.GetTrueOrFalseImageHtml(!view360Info.IsDisabled);

                //var urlPreview = View360Manager.GetView360Url(PublishmentSystemInfo, view360Info, string.Empty);
                //urlPreview = BackgroundPreview.GetRedirectUrlToMobile(urlPreview);
                //ltlPreviewUrl.Text = $@"<a href=""{urlPreview}"" target=""_blank"">预览</a>";

                var urlEdit = PageView360Add.GetRedirectUrl(PublishmentSystemId, view360Info.Id);
                ltlEditUrl.Text = $@"<a href=""{urlEdit}"">编辑</a>";
            }
        }
    }
}
