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
        public Literal ltlAppointementTitle;
        public Literal ltlRealName;
        public Literal ltlMobile;
        public Literal ltlEmail;
        public Literal ltlAddDate;
        public Literal ltlExtendVal;

        private int appointmentContentID;

        public static string GetOpenWindowStringToSingle(int publishmentSystemId, int appointmentContentID)
        {
            return PageUtils.GetOpenWindowString("预约详情查看",
                PageUtils.GetWeiXinUrl(nameof(ModalAppointmentContentDetail), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"appointmentContentID", appointmentContentID.ToString()}
                }), 450, 550);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            appointmentContentID = Body.GetQueryInt("appointmentContentID");

            if (!IsPostBack)
            {
                var appointmentContentInfo = DataProviderWX.AppointmentContentDAO.GetContentInfo(appointmentContentID);
                var appointmentItemInfo = DataProviderWX.AppointmentItemDAO.GetItemInfo(appointmentContentInfo.AppointmentItemID);
                ltlAppointementTitle.Text = appointmentItemInfo.Title;
                ltlMobile.Text = appointmentContentInfo.Mobile;
                ltlEmail.Text = appointmentContentInfo.Email;
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(appointmentContentInfo.AddDate);
                ltlExtendVal.Text = "";
                if (!string.IsNullOrEmpty(appointmentContentInfo.SettingsXML) && appointmentContentInfo.SettingsXML.ToString().Trim() != "{}")
                {
                    var SettingsXML = appointmentContentInfo.SettingsXML.Replace("{", "").Replace("}", "");
                    var stringBuilderHtml = new StringBuilder();

                    var arr = SettingsXML.Split(',');
                    for (var i = 0; i < arr.Length; i++)
                    {
                        var arr1 = arr[i].Replace("\"", "").Split(':');
                        stringBuilderHtml.AppendFormat(@"<tr>");
                        stringBuilderHtml.AppendFormat(@"<td>{0}:</td>", arr1[0]);
                        stringBuilderHtml.AppendFormat(@"<td>{0}</td>", arr1[1]);
                        stringBuilderHtml.AppendFormat(@"</tr>");
                    }
                    ltlExtendVal.Text = stringBuilderHtml.ToString();
                }

            }
        }
    }
}
