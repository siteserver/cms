using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core;
using BaiRong.Core.IO;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalExportAppointmentContent : BasePageCms
    {
        public static string GetOpenWindowStringByAppointmentContent(int publishmentSystemId, int appointmentID, string appointmentTitle)
        {
            return PageUtils.GetOpenWindowString("导出CSV",
                PageUtils.GetWeiXinUrl(nameof(ModalExportAppointmentContent), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"appointmentID", appointmentID.ToString()},
                    {"appointmentTitle", appointmentTitle}
                }), 400, 240, true);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var appointmentID = Body.GetQueryInt("appointmentID");
                var appointmentTitle = Body.GetQueryString("appointmentTitle");

                var appointmentContentInfolList = DataProviderWX.AppointmentContentDAO.GetAppointmentContentInfoList(PublishmentSystemId, appointmentID);

                if (appointmentContentInfolList.Count == 0)
                {
                    FailMessage("暂无数据导出！");
                    return;
                }

                var docFileName = "预约名单" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                var filePath = PathUtils.GetTemporaryFilesPath(docFileName);

                ExportAppointmentContentCSV(filePath, appointmentContentInfolList, appointmentTitle, appointmentID);

                var fileUrl = PageUtils.GetTemporaryFilesUrl(docFileName);
                SuccessMessage($@"成功导出文件，请点击 <a href=""{fileUrl}"">这里</a> 进行下载！");
            }
        }

        public void ExportAppointmentContentCSV(string filePath, List<AppointmentContentInfo> appointmentContentInfolList, string appointmentTitle, int appointmentID)
        {
            var appointmentInfo = DataProviderWX.AppointmentDAO.GetAppointmentInfo(appointmentID);

            var head = new List<string>();
            head.Add("序号");
            head.Add("预约名称");
            if (appointmentInfo.IsFormRealName == "True")
            {
                head.Add(appointmentInfo.FormRealNameTitle);
            }
            if (appointmentInfo.IsFormMobile == "True")
            {
                head.Add(appointmentInfo.FormMobileTitle);
            }
            if (appointmentInfo.IsFormEmail == "True")
            {
                head.Add(appointmentInfo.FormEmailTitle);
            }
            head.Add("预约时间");
            head.Add("预约状态");
            head.Add("留言");
            var configExtendInfoList = DataProviderWX.ConfigExtendDAO.GetConfigExtendInfoList(PublishmentSystemId, appointmentID, EKeywordTypeUtils.GetValue(EKeywordType.Appointment));
            foreach (var cList in configExtendInfoList)
            {
                head.Add(cList.AttributeName);
            }

            var rows = new List<List<string>>();

            var index = 1;
            foreach (var applist in appointmentContentInfolList)
            {

                var row = new List<string>();

                row.Add((index++).ToString());
                row.Add(appointmentTitle);
                if (appointmentInfo.IsFormRealName == "True")
                {
                    row.Add(applist.RealName);
                }
                if (appointmentInfo.IsFormMobile == "True")
                {
                    row.Add(applist.Mobile);
                }
                if (appointmentInfo.IsFormEmail == "True")
                {
                    row.Add(applist.Email);
                }
                row.Add(DateUtils.GetDateAndTimeString(applist.AddDate));
                row.Add(EAppointmentStatusUtils.GetText(EAppointmentStatusUtils.GetEnumType(applist.Status)));
                row.Add(applist.Message);

                var SettingsXML = applist.SettingsXML.Replace("{", "").Replace("}", "");
                var arr = SettingsXML.Split(',');
                if (arr[0] != "")
                {
                    for (var i = 0; i < arr.Length; i++)
                    {
                        var arr1 = arr[i].Replace("\"", "").Split(':');
                        row.Add(arr1[1]);
                    }
                }
                rows.Add(row);
            }

            CSVUtils.Export(filePath, head, rows);
        }

    }
}
