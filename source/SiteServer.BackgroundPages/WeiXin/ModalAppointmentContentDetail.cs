using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.WeiXin.Data;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalAppointmentContentDetail : BasePageCms
    {
        public Literal LtlAppointementTitle;
        public Literal LtlRealName;
        public Literal LtlMobile;
        public Literal LtlEmail;
        public Literal LtlAddDate;
        public Literal LtlExtendVal;

        private int _appointmentContentId;

        public static string GetOpenWindowStringToSingle(int publishmentSystemId, int appointmentContentId)
        {
            return PageUtils.GetOpenWindowString("预约详情查看",
                PageUtils.GetWeiXinUrl(nameof(ModalAppointmentContentDetail), new NameValueCollection
                {
                    {"PublishmentSystemId", publishmentSystemId.ToString()},
                    {"appointmentContentID", appointmentContentId.ToString()}
                }), 450, 550);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _appointmentContentId = Body.GetQueryInt("appointmentContentID");

            if (!IsPostBack)
            {
                var appointmentContentInfo = DataProviderWx.AppointmentContentDao.GetContentInfo(_appointmentContentId);
                var appointmentItemInfo = DataProviderWx.AppointmentItemDao.GetItemInfo(appointmentContentInfo.AppointmentItemId);
                LtlAppointementTitle.Text = appointmentItemInfo.Title;
                LtlMobile.Text = appointmentContentInfo.Mobile;
                LtlEmail.Text = appointmentContentInfo.Email;
                LtlAddDate.Text = DateUtils.GetDateAndTimeString(appointmentContentInfo.AddDate);
                LtlExtendVal.Text = "";
                if (!string.IsNullOrEmpty(appointmentContentInfo.SettingsXml) && appointmentContentInfo.SettingsXml.Trim() != "{}")
                {
                    var settingsXml = appointmentContentInfo.SettingsXml.Replace("{", "").Replace("}", "");
                    var stringBuilderHtml = new StringBuilder();

                    var arr = settingsXml.Split(',');
                    for (var i = 0; i < arr.Length; i++)
                    {
                        var arr1 = arr[i].Replace("\"", "").Split(':');
                        stringBuilderHtml.AppendFormat(@"<tr>");
                        stringBuilderHtml.AppendFormat(@"<td>{0}:</td>", arr1[0]);
                        stringBuilderHtml.AppendFormat(@"<td>{0}</td>", arr1[1]);
                        stringBuilderHtml.AppendFormat(@"</tr>");
                    }
                    LtlExtendVal.Text = stringBuilderHtml.ToString();
                }

            }
        }
    }
}
