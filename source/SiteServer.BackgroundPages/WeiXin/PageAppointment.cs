using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageAppointment : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;

        public Button BtnAddSingle;
        public Button BtnAddMultiple;
        public Button BtnDelete;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageAppointment), new NameValueCollection
            {
                {"PublishmentSystemId", publishmentSystemId.ToString()}
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
                        DataProviderWx.AppointmentDao.Delete(PublishmentSystemId,list) ;
                        SuccessMessage("微预约删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "微预约删除失败！");
                    }
                }
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = 30;
            SpContents.SelectCommand = DataProviderWx.AppointmentDao.GetSelectString(PublishmentSystemId);
            SpContents.SortField = AppointmentAttribute.Id;
            SpContents.SortMode = SortMode.DESC;
            RptContents.ItemDataBound += rptContents_ItemDataBound;

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.WeiXin.LeftMenu.Function.IdAppointment, "微预约", AppManager.WeiXin.Permission.WebSite.Appointment);
                SpContents.DataBind();

                var urlAddSingle = PageAppointmentSingleAdd.GetRedirectUrl(PublishmentSystemId, 0,0);
                var urlAddMultiple = PageAppointmentMultipleAdd.GetRedirectUrl(PublishmentSystemId, 0,0);
                BtnAddSingle.Attributes.Add("onclick", $"location.href='{urlAddSingle}';return false");
                BtnAddMultiple.Attributes.Add("onclick", $"location.href='{urlAddMultiple}';return false");

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemId), "Delete", "True");
                BtnDelete.Attributes.Add("onclick", PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的微预约", "此操作将删除所选微预约，确认吗？"));
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var appointmentInfo = new AppointmentInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlTitle = e.Item.FindControl("ltlTitle") as Literal;
                var ltlKeywords = e.Item.FindControl("ltlKeywords") as Literal;
                var ltlStartDate = e.Item.FindControl("ltlStartDate") as Literal;
                var ltlEndDate = e.Item.FindControl("ltlEndDate") as Literal;
                var ltlContentIsSingle = e.Item.FindControl("ltlContentIsSingle") as Literal;
                var ltlPvCount = e.Item.FindControl("ltlPVCount") as Literal;
                var ltlUserCount = e.Item.FindControl("ltlUserCount") as Literal;
                var ltlIsEnabled = e.Item.FindControl("ltlIsEnabled") as Literal;
                var ltlAppointmentContentUrl = e.Item.FindControl("ltlAppointmentContentUrl") as Literal;
                var ltlPreviewUrl = e.Item.FindControl("ltlPreviewUrl") as Literal;
                var ltlEditUrl = e.Item.FindControl("ltlEditUrl") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();
                ltlTitle.Text = appointmentInfo.Title;
                ltlKeywords.Text = DataProviderWx.KeywordDao.GetKeywords(appointmentInfo.KeywordId);
                ltlStartDate.Text = DateUtils.GetDateAndTimeString(appointmentInfo.StartDate);
                ltlEndDate.Text = DateUtils.GetDateAndTimeString(appointmentInfo.EndDate);
                ltlContentIsSingle.Text = appointmentInfo.ContentIsSingle == true ? "单预约" : "多预约";
                ltlPvCount.Text = appointmentInfo.PvCount.ToString();
                ltlUserCount.Text = appointmentInfo.UserCount.ToString();
                ltlIsEnabled.Text = StringUtils.GetTrueOrFalseImageHtml(!appointmentInfo.IsDisabled);

                var urlAppointmentContent = PageAppointmentContent.GetRedirectUrl(PublishmentSystemId, appointmentInfo.Id);
                ltlAppointmentContentUrl.Text = $@"<a href=""{urlAppointmentContent}"">预约查看</a>";

                var itemId = 0;
                if (appointmentInfo.ContentIsSingle)
                {
                    itemId = DataProviderWx.AppointmentItemDao.GetItemId(PublishmentSystemId, appointmentInfo.Id);
                }

                var urlPreview = AppointmentManager.GetIndexUrl(PublishmentSystemInfo, appointmentInfo.Id, string.Empty);
                if (appointmentInfo.ContentIsSingle)
                {
                    urlPreview = AppointmentManager.GetItemUrl(PublishmentSystemInfo, appointmentInfo.Id, itemId, string.Empty);
                }
                //urlPreview = BackgroundPreview.GetRedirectUrlToMobile(urlPreview);
                //ltlPreviewUrl.Text = $@"<a href=""{urlPreview}"" target=""_blank"">预览</a>";

                var urlEdit = PageAppointmentMultipleAdd.GetRedirectUrl(PublishmentSystemId, appointmentInfo.Id, itemId);
                if (appointmentInfo.ContentIsSingle)
                {
                    urlEdit = PageAppointmentSingleAdd.GetRedirectUrl(PublishmentSystemId, appointmentInfo.Id, itemId);
                }
                ltlEditUrl.Text = $@"<a href=""{urlEdit}"">编辑</a>";
            }
        }
    }
}
