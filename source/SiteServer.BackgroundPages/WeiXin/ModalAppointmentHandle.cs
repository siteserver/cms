using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalAppointmentHandle : BasePageCms
    { 
        public DropDownList DdlStatus;
        public PlaceHolder PhMessage;
        public TextBox TbMessage;

        private int _contentId;
        private List<int> _contentIdList;

        public static string GetOpenWindowStringToSingle(int publishmentSystemId, int contentId)
        {
            return PageUtils.GetOpenWindowString("预约处理",
                PageUtils.GetWeiXinUrl(nameof(ModalAppointmentHandle), new NameValueCollection
                {
                    {"PublishmentSystemId", publishmentSystemId.ToString()},
                    {"contentID", contentId.ToString()}
                }), 360, 380);
        }

        public static string GetOpenWindowStringToMultiple(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowStringWithCheckBoxValue("预约处理",
                PageUtils.GetWeiXinUrl(nameof(ModalAppointmentHandle), new NameValueCollection
                {
                    {"PublishmentSystemId", publishmentSystemId.ToString()}
                }), "IDCollection", "请选择需要处理的预约申请", 360, 380);
        }
       
        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _contentId = Body.GetQueryInt("contentID");
            if (_contentId == 0)
            {
                _contentIdList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("IDCollection"));
            }
             
            if (!IsPostBack)
            {
                EAppointmentStatusUtils.AddListItems(DdlStatus);
                ControlUtils.SelectListItems(DdlStatus, EAppointmentStatusUtils.GetValue(EAppointmentStatus.Agree));

                if (_contentId > 0)
                {
                    var contentInfo = DataProviderWx.AppointmentContentDao.GetContentInfo(_contentId);
                    if (contentInfo != null)
                    {
                        DdlStatus.SelectedValue = contentInfo.Status;
                        TbMessage.Text = contentInfo.Message;
                    }
                }
            }
        }

        public void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            var status = EAppointmentStatusUtils.GetEnumType(DdlStatus.SelectedValue);
            if (status == EAppointmentStatus.Agree || status == EAppointmentStatus.Refuse)
            {
                PhMessage.Visible = true;
            }
            else
            {
                PhMessage.Visible = false;
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                if (_contentId > 0)
                {
                    var contentInfo = DataProviderWx.AppointmentContentDao.GetContentInfo(_contentId);
                    if (contentInfo != null)
                    {
                        contentInfo.Status = DdlStatus.SelectedValue;
                        contentInfo.Message = TbMessage.Text;
                        DataProviderWx.AppointmentContentDao.Update(contentInfo);
                    }
                }
                else if (_contentIdList != null && _contentIdList.Count > 0)
                {
                    foreach (var theContentId in _contentIdList)
                    {
                        var contentInfo = DataProviderWx.AppointmentContentDao.GetContentInfo(theContentId);
                        if (contentInfo != null)
                        {
                            contentInfo.Status = DdlStatus.SelectedValue;
                            contentInfo.Message = TbMessage.Text;
                            DataProviderWx.AppointmentContentDao.Update(contentInfo);
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
