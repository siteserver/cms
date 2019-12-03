using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using SiteServer.CMS.Api;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.Context;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.ImportExport;

namespace SiteServer.BackgroundPages.Settings
{
    public class ModalExportMessage : BasePage
    {
        private string _exportType;
        public const string ExportTypeSingleTableStyle = "SingleTableStyle";

        public static string GetOpenWindowStringToSingleTableStyle(string tableName)
        {
            return LayerUtils.GetOpenScript("导出数据",
                PageUtils.GetSettingsUrl(nameof(ModalExportMessage), new NameValueCollection
                {
                    {"TableName", tableName},
                    {"ExportType", ExportTypeSingleTableStyle}
                }), 380, 250);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _exportType = AuthRequest.GetQueryString("ExportType");

            if (!IsPostBack)
            {
                var fileName = string.Empty;
                try
                {
                    if (_exportType == ExportTypeSingleTableStyle)
                    {
                        var tableName = AuthRequest.GetQueryString("TableName");
                        fileName = ExportSingleTableStyleAsync(tableName).GetAwaiter().GetResult();
                    }

                    var link = new HyperLink();
                    var filePath = PathUtils.GetTemporaryFilesPath(fileName);
                    link.NavigateUrl = ApiRouteActionsDownload.GetUrl(ApiManager.InnerApiUrl, filePath);
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

        private static async Task<string> ExportSingleTableStyleAsync(string tableName)
        {
            return await ExportObject.ExportRootSingleTableStyleAsync(tableName);
        }
    }
}
