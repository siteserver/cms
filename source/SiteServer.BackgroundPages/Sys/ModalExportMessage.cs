using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.ImportExport;

namespace SiteServer.BackgroundPages.Sys
{
    public class ModalExportMessage : BasePage
    {
        private string _exportType;

        public const int Width = 380;
        public const int Height = 250;
        public const string ExportTypeSingleTableStyle = "SingleTableStyle";

        public static string GetOpenWindowStringToSingleTableStyle(ETableStyle tableStyle, string tableName)
        {
            return PageUtils.GetOpenWindowString("导出数据",
                PageUtils.GetSysUrl(nameof(ModalExportMessage), new NameValueCollection
                {
                    {"TableStyle", ETableStyleUtils.GetValue(tableStyle)},
                    {"TableName", tableName},
                    {"ExportType", ExportTypeSingleTableStyle}
                }), Width, Height, true);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _exportType = Body.GetQueryString("ExportType");

            if (!IsPostBack)
            {
                var fileName = string.Empty;
                try
                {
                    if (_exportType == ExportTypeSingleTableStyle)
                    {
                        var tableStyle = ETableStyleUtils.GetEnumType(Body.GetQueryString("TableStyle"));
                        var tableName = Body.GetQueryString("TableName");
                        fileName = ExportSingleTableStyle(tableStyle, tableName);
                    }

                    var link = new HyperLink();
                    var filePath = PathUtils.GetTemporaryFilesPath(fileName);
                    link.NavigateUrl = ActionsDownload.GetUrl(PageUtils.GetApiUrl(), filePath);
                    link.Text = "下载";
                    var successMessage = "成功导出文件！&nbsp;&nbsp;" + ControlUtils.GetControlRenderHtml(link);
                    SuccessMessage(successMessage);
                }
                catch (Exception ex)
                {
                    var failedMessage = "文件导出失败！<br/><br/>原因为：" + ex.Message;
                    FailMessage(ex, failedMessage);
                }
            }
        }

        private string ExportSingleTableStyle(ETableStyle tableStyle, string tableName)
        {
            return ExportObject.ExportRootSingleTableStyle(tableStyle, tableName);
        }
    }
}
