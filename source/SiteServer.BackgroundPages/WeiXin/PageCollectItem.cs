using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;
using System.Collections.Specialized;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageCollectItem : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;

        public Button BtnDelete;
        public Button BtnReturn;
        private int _collectId;
        private string _returnUrl;
        private int _collectItemId;

        public static string GetRedirectUrl(int publishmentSystemId, int collectId, string returnUrl)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageCollectItem), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"collectId", collectId.ToString()},
                {"returnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public static string GetRedirectUrl(int publishmentSystemId, int collectItemId, int collectId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageCollectItem), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"collectItemId", collectItemId.ToString()},
                {"collectId", collectId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            _collectId = TranslateUtils.ToInt(Request.QueryString["collectID"]);
            _returnUrl = StringUtils.ValueFromUrl(Request.QueryString["returnUrl"]);
            _collectItemId = TranslateUtils.ToInt(Request.QueryString["collectItemID"]);
            if (IsForbidden) return;

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]))
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWx.CollectItemDao.Delete(PublishmentSystemId, list);
                        SuccessMessage("征集参赛选项删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "征集参赛选项删除失败！");
                    }
                }
            }

            if (_collectItemId > 0)
            {
                try
                {
                    DataProviderWx.CollectItemDao.Audit(PublishmentSystemId, _collectItemId);
                    SuccessMessage("征集参赛选项审核成功！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "征集参赛选项审核失败！");
                }
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = 30;
            SpContents.SelectCommand = DataProviderWx.CollectItemDao.GetSelectString(PublishmentSystemId, _collectId);
            SpContents.SortField = CollectItemAttribute.Id;
            SpContents.SortMode = SortMode.ASC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdCollect, "参赛记录", AppManager.WeiXin.Permission.WebSite.Collect);
                SpContents.DataBind();

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemId, _collectId, _returnUrl), "Delete", "True");
                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的征集活动参赛选项", "此操作将删除所选征集活动参赛选项，确认吗？"));
                BtnReturn.Attributes.Add("onclick", $"location.href='{_returnUrl}';return false");
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var collectItemInfo = new CollectItemInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlItemTitle = e.Item.FindControl("ltlItemTitle") as Literal;
                var ltlDescription = e.Item.FindControl("ltlDescription") as Literal;
                var ltlMobile = e.Item.FindControl("ltlMobile") as Literal;
                var ltlIsChecked = e.Item.FindControl("ltlIsChecked") as Literal;
                var ltlVoteNum = e.Item.FindControl("ltlVoteNum") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlItemTitle.Text = collectItemInfo.Title;
                ltlDescription.Text = collectItemInfo.Description;
                ltlMobile.Text = collectItemInfo.Mobile;
                ltlVoteNum.Text = collectItemInfo.VoteNum.ToString(); ;
                ltlIsChecked.Text = StringUtils.GetTrueOrFalseImageHtml(collectItemInfo.IsChecked);
                var urlEdit = GetRedirectUrl(PublishmentSystemId, collectItemInfo.Id, collectItemInfo.CollectId);
                ltlEditUrl.Text = $@"<a href=""{urlEdit}"">审核</a>";

            }
        }
    }
}
