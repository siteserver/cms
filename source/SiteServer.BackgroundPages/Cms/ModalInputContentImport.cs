using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalInputContentImport : BasePageCms
    {
        public RadioButtonList ImportType;
		public HtmlInputFile myFile;
        public TextBox ImportStart;
        public TextBox ImportCount;
        public RadioButtonList IsChecked;

        private InputInfo _inputInfo;

        public static string GetOpenWindowString(int publishmentSystemId, int inputId)
        {
            return PageUtils.GetOpenWindowString("导入内容", PageUtils.GetCmsUrl(nameof(ModalInputContentImport), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"InputID", inputId.ToString()}
                }), 570, 320);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var inputID = Body.GetQueryInt("InputID");
            _inputInfo = DataProvider.InputDao.GetInputInfo(inputID);

			if (!IsPostBack)
			{
                EBooleanUtils.AddListItems(IsChecked, "已审核", "未审核");
                ControlUtils.SelectListItems(IsChecked, true.ToString());
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			if (myFile.PostedFile != null && "" != myFile.PostedFile.FileName)
			{
				try
				{
                    var filePath = myFile.PostedFile.FileName;
                    if (!StringUtils.EqualsIgnoreCase(PathUtils.GetExtension(filePath), ".csv"))
                    {
                        FailMessage("必须上传后缀为“.csv”的Excel文件");
                        return;
                    }

                    var localFilePath = PathUtils.GetTemporaryFilesPath(PathUtils.GetFileName(filePath));

                    myFile.PostedFile.SaveAs(localFilePath);

                    var importObject = new ImportObject(PublishmentSystemId);
                    importObject.ImportInputContentsByCsvFile(_inputInfo, localFilePath, TranslateUtils.ToInt(ImportStart.Text), TranslateUtils.ToInt(ImportCount.Text), TranslateUtils.ToBool(IsChecked.SelectedValue));

                    Body.AddSiteLog(PublishmentSystemId, "导入提交表单内容", $"提交表单：{_inputInfo.InputName}");

					PageUtils.CloseModalPage(Page);
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "导入提交表单失败！");
				}
			}
		}
	}
}
