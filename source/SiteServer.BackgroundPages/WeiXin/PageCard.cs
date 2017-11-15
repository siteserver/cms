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
    public class PageCard : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;

        public Button BtnAdd;
        public Button BtnDelete;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageCard), new NameValueCollection
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
                        DataProviderWx.CardDao.Delete(PublishmentSystemId,list) ;
                        SuccessMessage("会员卡删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "会员卡删除失败！");
                    }
                }
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = 30;
            SpContents.SelectCommand = DataProviderWx.CardDao.GetSelectString(PublishmentSystemId);
            SpContents.SortField = CardAttribute.Id;
            SpContents.SortMode = SortMode.ASC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            {

                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdCard, "会员卡", AppManager.WeiXin.Permission.WebSite.Card);
                SpContents.DataBind();

                var urlAdd = PageCardAdd.GetRedirectUrl(PublishmentSystemId, 0);
                BtnAdd.Attributes.Add("onclick", $"location.href='{urlAdd}';return false");

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemId), "Delete", "True");
                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的会员卡", "此操作将删除所选会员卡，确认吗？"));
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var cardInfo = new CardInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlTitle = e.Item.FindControl("ltlTitle") as Literal;
                var ltlCardTitle = e.Item.FindControl("ltlCardTitle") as Literal;
                var ltlKeywords = e.Item.FindControl("ltlKeywords") as Literal;
                var ltlPvCount = e.Item.FindControl("ltlPVCount") as Literal;
                var ltlIsEnabled = e.Item.FindControl("ltlIsEnabled") as Literal;
                var ltlUserUrl = e.Item.FindControl("ltlUserUrl") as Literal;
                var ltlPreviewUrl = e.Item.FindControl("ltlPreviewUrl") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;
                var ltlOperator = e.Item.FindControl("ltlOperator") as Literal;
                 
                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlTitle.Text = cardInfo.Title;
                ltlCardTitle.Text = cardInfo.CardTitle;
                ltlKeywords.Text = DataProviderWx.KeywordDao.GetKeywords(cardInfo.KeywordId);
                ltlPvCount.Text = cardInfo.PvCount.ToString();

                ltlIsEnabled.Text = StringUtils.GetTrueOrFalseImageHtml(!cardInfo.IsDisabled);

                var urlCardSn = PageCardSn.GetRedirectUrl(PublishmentSystemId, cardInfo.Id,string.Empty,string.Empty,string.Empty,false);

                ltlUserUrl.Text = $@"<a href=""{urlCardSn}"">会员卡</a>";

                //var urlPreview = CardManager.GetCardUrl(cardInfo, string.Empty);
                //urlPreview = BackgroundPreview.GetRedirectUrlToMobile(urlPreview);
                //ltlPreviewUrl.Text = $@"<a href=""{urlPreview}"" target=""_blank"">预览</a>";

                var urlEdit = PageCardAdd.GetRedirectUrl(PublishmentSystemId, cardInfo.Id);
                ltlEditUrl.Text = $@"<a href=""{urlEdit}"">编辑</a>";

                ltlOperator.Text =
                    $@"<a href=""javascript:;"" onclick=""{ModalCardOperatorAdd.GetOpenWindowStringToAdd(
                        PublishmentSystemId, cardInfo.Id)}"">操作员</a>";
            }
        }
    }
}
