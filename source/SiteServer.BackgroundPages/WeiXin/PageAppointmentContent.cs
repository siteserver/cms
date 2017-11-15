using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageAppointmentContent : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;

        public Button BtnHandle;
        public Button BtnDelete;
        public Button BtnExport;
        public Button BtnReturn;

        public int AppointmentId;

        public string SettingsXml;
        public string AppointmentTitle;
        public Literal LtlExtendTitle;
        public int TdCount = 0;

        public static string GetRedirectUrl(int publishmentSystemId, int appointmentId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageAppointmentContent), new NameValueCollection
            {
                {"PublishmentSystemId", publishmentSystemId.ToString()},
                {"appointmentID", appointmentId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            AppointmentId = TranslateUtils.ToInt(Request["appointmentID"]);

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]))
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWx.AppointmentContentDao.Delete(PublishmentSystemId, list);
                        SuccessMessage("预约删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "预约删除失败！");
                    }
                }
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = 30;
            SpContents.SelectCommand = DataProviderWx.AppointmentContentDao.GetSelectString(PublishmentSystemId, AppointmentId);
            SpContents.SortField = AppointmentContentAttribute.Id;
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdAppointment, "查看预约", AppManager.WeiXin.Permission.WebSite.Appointment);
                SpContents.DataBind();

                BtnHandle.Attributes.Add("onclick", ModalAppointmentHandle.GetOpenWindowStringToMultiple(PublishmentSystemId));

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemId, AppointmentId), "Delete", "True");
                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的微预约", "此操作将删除所选微预约，确认吗？"));

                BtnExport.Attributes.Add("onclick", ModalExportAppointmentContent.GetOpenWindowStringByAppointmentContent(PublishmentSystemId, AppointmentId, AppointmentTitle));

                var returnUrl = PageAppointment.GetRedirectUrl(PublishmentSystemId);
                BtnReturn.Attributes.Add("onclick", $"location.href='{returnUrl}';return false");
            }
        }

        private Dictionary<int, AppointmentItemInfo> _itemInfoDictionary = new Dictionary<int, AppointmentItemInfo>();

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var contentInfo = new AppointmentContentInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlRealName = e.Item.FindControl("ltlRealName") as Literal;
                var ltlAppointementTitle = e.Item.FindControl("ltlAppointementTitle") as Literal;
                var ltlMobile = e.Item.FindControl("ltlMobile") as Literal;
                var ltlExtendVal = e.Item.FindControl("ltlExtendVal") as Literal;
                var ltlEmail = e.Item.FindControl("ltlEmail") as Literal;
                var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;
                var ltlStatus = e.Item.FindControl("ltlStatus") as Literal;
                var ltlMessage = e.Item.FindControl("ltlMessage") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;
                var ltlSelectUrl = e.Item.FindControl("ltlSelectUrl") as Literal;



                AppointmentItemInfo itemInfo = null;
                if (_itemInfoDictionary.ContainsKey(contentInfo.AppointmentItemId))
                {
                    itemInfo = _itemInfoDictionary[contentInfo.AppointmentItemId];
                }
                else
                {
                    itemInfo = DataProviderWx.AppointmentItemDao.GetItemInfo(contentInfo.AppointmentItemId);
                    _itemInfoDictionary.Add(contentInfo.AppointmentItemId, itemInfo);
                }

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlRealName.Text = contentInfo.RealName;
                if (itemInfo != null)
                {
                    ltlAppointementTitle.Text = itemInfo.Title;
                    AppointmentTitle = itemInfo.Title;
                }
                ltlMobile.Text = contentInfo.Mobile;
                ltlEmail.Text = contentInfo.Email;
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(contentInfo.AddDate);
                ltlStatus.Text = EAppointmentStatusUtils.GetText(EAppointmentStatusUtils.GetEnumType(contentInfo.Status));
                ltlMessage.Text = contentInfo.Message;

                ltlEditUrl.Text =
                    $@"<a href=""javascrip:;"" onclick=""{ModalAppointmentHandle.GetOpenWindowStringToSingle(
                        PublishmentSystemId, contentInfo.Id)}"">预约处理</a>";

                ltlSelectUrl.Text =
                    $@"<a href=""javascrip:;"" onclick=""{ModalAppointmentContentDetail.GetOpenWindowStringToSingle(
                        PublishmentSystemId, contentInfo.Id)}"">预约详情</a>";

              
            }
        }

    }
}
