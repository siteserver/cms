using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Text.LitJson;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalTextEditorInsertVideo : BasePageCms
    {
        public TextBox TbPlayUrl;
        public TextBox TbWidth;
        public TextBox TbHeight;
        public CheckBox CbIsAutoPlay;

        private string _attributeName;

        public static string GetOpenWindowString(int publishmentSystemId, string attributeName)
        {
            return PageUtils.GetOpenWindowString("插入视频", PageUtils.GetCmsUrl(nameof(ModalTextEditorInsertVideo), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"AttributeName", attributeName}
            }), 550, 350);
        }

        public string UploadUrl => PageUtils.GetCmsUrl(nameof(ModalTextEditorInsertVideo), new NameValueCollection
        {
            {"PublishmentSystemID", PublishmentSystemId.ToString()},
            {"upload", true.ToString()}
        });

        public string TypeCollection => PublishmentSystemInfo.Additional.VideoUploadTypeCollection;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Body.IsQueryExists("upload"))
            {
                var json = JsonMapper.ToJson(Upload());
                Response.Write(json);
                Response.End();
                return;
            }

            _attributeName = Body.GetQueryString("AttributeName");

            if (IsPostBack) return;

            TbWidth.Text = PublishmentSystemInfo.Additional.ConfigVideoContentInsertWidth.ToString();
            TbHeight.Text = PublishmentSystemInfo.Additional.ConfigVideoContentInsertHeight.ToString();
        }

	    private Hashtable Upload()
        {
            var success = false;
            var playUrl = string.Empty;
            var message = "视频上传失败";

            if (Request.Files["filedata"] != null)
            {
                var postedFile = Request.Files["filedata"];
                try
                {
                    if (!string.IsNullOrEmpty(postedFile?.FileName))
                    {
                        var filePath = postedFile.FileName;
                        var fileExtName = PathUtils.GetExtension(filePath);

                        var isAllow = true;
                        if (!PathUtility.IsVideoExtenstionAllowed(PublishmentSystemInfo, fileExtName))
                        {
                            message = "此格式不允许上传，请选择有效的音频文件";
                            isAllow = false;
                        }
                        if (!PathUtility.IsVideoSizeAllowed(PublishmentSystemInfo, postedFile.ContentLength))
                        {
                            message = "上传失败，上传文件超出规定文件大小";
                            isAllow = false;
                        }

                        if (isAllow)
                        {
                            var localDirectoryPath = PathUtility.GetUploadDirectoryPath(PublishmentSystemInfo, fileExtName);
                            var localFileName = PathUtility.GetUploadFileName(PublishmentSystemInfo, filePath);
                            var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);

                            postedFile.SaveAs(localFilePath);

                            playUrl = PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, localFilePath);
                            playUrl = PageUtility.GetVirtualUrl(PublishmentSystemInfo, playUrl);

                            success = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }

            var jsonAttributes = new Hashtable();
            if (success)
            {
                jsonAttributes.Add("success", "true");
                jsonAttributes.Add("playUrl", playUrl);
            }
            else
            {
                jsonAttributes.Add("success", "false");
                jsonAttributes.Add("message", message);
            }

            return jsonAttributes;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var width = TranslateUtils.ToInt(TbWidth.Text);
            var height = TranslateUtils.ToInt(TbHeight.Text);
            if (width > 0 && height > 0 && (width != PublishmentSystemInfo.Additional.ConfigVideoContentInsertWidth || height != PublishmentSystemInfo.Additional.ConfigVideoContentInsertHeight))
            {
                PublishmentSystemInfo.Additional.ConfigVideoContentInsertWidth = width;
                PublishmentSystemInfo.Additional.ConfigVideoContentInsertHeight = height;
                DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);
            }

            var playUrl = TbPlayUrl.Text;

            var script = "parent." + ETextEditorTypeUtils.GetInsertVideoScript(_attributeName, playUrl, width, height, CbIsAutoPlay.Checked);
            PageUtils.CloseModalPageWithoutRefresh(Page, script);
		}

	}
}
