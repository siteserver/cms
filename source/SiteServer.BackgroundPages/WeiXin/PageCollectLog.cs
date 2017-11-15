using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageCollectLog : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;

        public Button BtnDelete;
        public Button BtnReturn;

        private Dictionary<int, string> _idTitleMap;
        private int _collectId;
        private string _returnUrl;

        public static string GetRedirectUrl(int publishmentSystemId, int collectId, string returnUrl)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageCollectLog), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"collectId", collectId.ToString()},
                {"returnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            _collectId = TranslateUtils.ToInt(Request.QueryString["collectID"]);
            _returnUrl = StringUtils.ValueFromUrl(Request.QueryString["returnUrl"]);

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]))
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWx.CollectLogDao.Delete(list);
                        SuccessMessage("删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "删除失败！");
                    }
                }
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = 30;
            SpContents.SelectCommand = DataProviderWx.CollectLogDao.GetSelectString(_collectId);
            SpContents.SortField = CollectLogAttribute.Id;
            SpContents.SortMode = SortMode.ASC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdCollect, "投票记录", AppManager.WeiXin.Permission.WebSite.Collect);
                var itemInfoList = DataProviderWx.CollectItemDao.GetCollectItemInfoList(_collectId);
                _idTitleMap = new Dictionary<int, string>();
                foreach (var itemInfo in itemInfoList)
                {
                    _idTitleMap[itemInfo.Id] = itemInfo.Title;
                }

                SpContents.DataBind();

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemId, _collectId, _returnUrl), "Delete", "True");
                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的投票项", "此操作将删除所选投票项，确认吗？"));
                BtnReturn.Attributes.Add("onclick", $"location.href='{_returnUrl}';return false");
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var logInfo = new CollectLogInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlItemId = e.Item.FindControl("ltlItemID") as Literal;
                var ltlIpAddress = e.Item.FindControl("ltlIPAddress") as Literal;
                var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();

                if (_idTitleMap.ContainsKey(logInfo.ItemId))
                {
                    ltlItemId.Text = _idTitleMap[logInfo.ItemId];
                }

                ltlIpAddress.Text = logInfo.IpAddress;
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(logInfo.AddDate);
            }
        }
    }
}
