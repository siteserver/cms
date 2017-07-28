using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.IO;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalAppointmentHandle : BasePageCms
    { 
        public DropDownList ddlStatus;
        public PlaceHolder phMessage;
        public TextBox tbMessage;

        private int contentID;
        private List<int> contentIDList;

        public static string GetOpenWindowStringToSingle(int publishmentSystemId, int contentID)
        {
            return PageUtils.GetOpenWindowString("预约处理",
                PageUtils.GetWeiXinUrl(nameof(ModalAppointmentHandle), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"contentID", contentID.ToString()}
                }), 360, 380);
        }

        public static string GetOpenWindowStringToMultiple(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowStringWithCheckBoxValue("预约处理",
                PageUtils.GetWeiXinUrl(nameof(ModalAppointmentHandle), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()}
                }), "IDCollection", "请选择需要处理的预约申请", 360, 380);
        }
       
        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            contentID = Body.GetQueryInt("contentID");
            if (contentID == 0)
            {
                contentIDList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("IDCollection"));
            }
             
            if (!IsPostBack)
            {
                EAppointmentStatusUtils.AddListItems(ddlStatus);
                ControlUtils.SelectListItems(ddlStatus, EAppointmentStatusUtils.GetValue(EAppointmentStatus.Agree));

                if (contentID > 0)
                {
                    var contentInfo = DataProviderWX.AppointmentContentDAO.GetContentInfo(contentID);
                    if (contentInfo != null)
                    {
                        ddlStatus.SelectedValue = contentInfo.Status;
                        tbMessage.Text = contentInfo.Message;
                    }
                }
            }
        }

        public void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            var status = EAppointmentStatusUtils.GetEnumType(ddlStatus.SelectedValue);
            if (status == EAppointmentStatus.Agree || status == EAppointmentStatus.Refuse)
            {
                phMessage.Visible = true;
            }
            else
            {
                phMessage.Visible = false;
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                if (contentID > 0)
                {
                    var contentInfo = DataProviderWX.AppointmentContentDAO.GetContentInfo(contentID);
                    if (contentInfo != null)
                    {
                        contentInfo.Status = ddlStatus.SelectedValue;
                        contentInfo.Message = tbMessage.Text;
                        DataProviderWX.AppointmentContentDAO.Update(contentInfo);
                    }
                }
                else if (contentIDList != null && contentIDList.Count > 0)
                {
                    foreach (var theContentID in contentIDList)
                    {
                        var contentInfo = DataProviderWX.AppointmentContentDAO.GetContentInfo(theContentID);
                        if (contentInfo != null)
                        {
                            contentInfo.Status = ddlStatus.SelectedValue;
                            contentInfo.Message = tbMessage.Text;
                            DataProviderWX.AppointmentContentDAO.Update(contentInfo);
                        }
                    }
                }
                SuccessMessage("预约处理成功！");

                PageUtils.CloseModalPage(Page);
            }
            catch (Exception ex)
            {
                FailMessage(ex, "失败：" + ex.Message);
            }
        }
    }
}
