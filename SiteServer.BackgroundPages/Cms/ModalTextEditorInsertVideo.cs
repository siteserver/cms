using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.Utils.LitJson;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalTextEditorInsertVideo : BasePageCms
    {
        public TextBox TbPlayUrl;
        public CheckBox CbIsImageUrl;
        public CheckBox CbIsWidth;
        public CheckBox CbIsHeight;
        public CheckBox CbIsAutoPlay;
        public TextBox TbImageUrl;
        public DropDownList DdlPlayBy;
        public TextBox TbWidth;
        public TextBox TbHeight;

        private string _attributeName;

        public static string GetOpenWindowString(int siteId, string attributeName)
        {
            return LayerUtils.GetOpenScript("插入视频", PageUtils.GetCmsUrl(siteId, nameof(ModalTextEditorInsertVideo), new NameValueCollection
            {
                {"AttributeName", attributeName}
            }), 600, 520);
        }

        public string UploadUrl => PageUtils.GetCmsUrl(SiteId, nameof(ModalTextEditorInsertVideo), new NameValueCollection
        {
            {"upload", true.ToString()}
        });

        public string VideoTypeCollection => SiteInfo.Additional.VideoUploadTypeCollection;
        public string ImageTypeCollection => SiteInfo.Additional.ImageUploadTypeCollection;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (AuthRequest.IsQueryExists("upload"))
            {
                var json = JsonMapper.ToJson(Upload());
                Response.Write(json);
                Response.End();
                return;
            }

            _attributeName = AuthRequest.GetQueryString("AttributeName");

            if (IsPostBack) return;

            ControlUtils.AddListControlItems(DdlPlayBy, StlPlayer.PlayByList);

            CbIsImageUrl.Checked = SiteInfo.Additional.ConfigUEditorVideoIsImageUrl;
            CbIsAutoPlay.Checked = SiteInfo.Additional.ConfigUEditorVideoIsAutoPlay;
            CbIsWidth.Checked = SiteInfo.Additional.ConfigUEditorVideoIsWidth;
            CbIsHeight.Checked = SiteInfo.Additional.ConfigUEditorVideoIsHeight;
            ControlUtils.SelectSingleItem(DdlPlayBy, SiteInfo.Additional.ConfigUEditorVideoPlayBy);
            TbWidth.Text = SiteInfo.Additional.ConfigUEditorVideoWidth.ToString();
            TbHeight.Text = SiteInfo.Additional.ConfigUEditorVideoHeight.ToString();
        }

	    private Hashtable Upload()
        {
            var success = false;
            var url = string.Empty;
            var message = "上传失败";

            if (Request.Files["videodata"] != null)
            {
                var postedFile = Request.Files["videodata"];
                try
                {
                    if (!string.IsNullOrEmpty(postedFile?.FileName))
                    {
                        var filePath = postedFile.FileName;
                        var fileExtName = PathUtils.GetExtension(filePath);

                        var isAllow = true;
                        if (!PathUtility.IsVideoExtenstionAllowed(SiteInfo, fileExtName))
                        {
                            message = "此格式不允许上传，请选择有效的音频文件";
                            isAllow = false;
                        }
                        if (!PathUtility.IsVideoSizeAllowed(SiteInfo, postedFile.ContentLength))
                        {
                            message = "上传失败，上传文件超出规定文件大小";
                            isAllow = false;
                        }

                        if (isAllow)
                        {
                            var localDirectoryPath = PathUtility.GetUploadDirectoryPath(SiteInfo, fileExtName);
                            var localFileName = PathUtility.GetUploadFileName(SiteInfo, filePath);
                            var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);

                            postedFile.SaveAs(localFilePath);

                            url = PageUtility.GetSiteUrlByPhysicalPath(SiteInfo, localFilePath, true);
                            url = PageUtility.GetVirtualUrl(SiteInfo, url);

                            success = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            else if (Request.Files["imgdata"] != null)
            {
                var postedFile = Request.Files["imgdata"];
                try
                {
                    if (!string.IsNullOrEmpty(postedFile?.FileName))
                    {
                        var filePath = postedFile.FileName;
                        var fileExtName = PathUtils.GetExtension(filePath);

                        var isAllow = true;
                        if (!PathUtility.IsImageExtenstionAllowed(SiteInfo, fileExtName))
                        {
                            message = "此格式不允许上传，请选择有效的图片文件";
                            isAllow = false;
                        }
                        if (!PathUtility.IsImageSizeAllowed(SiteInfo, postedFile.ContentLength))
                        {
                            message = "上传失败，上传文件超出规定文件大小";
                            isAllow = false;
                        }

                        if (isAllow)
                        {
                            var localDirectoryPath = PathUtility.GetUploadDirectoryPath(SiteInfo, fileExtName);
                            var localFileName = PathUtility.GetUploadFileName(SiteInfo, filePath);
                            var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);

                            postedFile.SaveAs(localFilePath);

                            url = PageUtility.GetSiteUrlByPhysicalPath(SiteInfo, localFilePath, true);
                            url = PageUtility.GetVirtualUrl(SiteInfo, url);

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
                jsonAttributes.Add("url", url);
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
            var playUrl = TbPlayUrl.Text;
            var isImageUrl = CbIsImageUrl.Checked;
            var isAutoPlay = CbIsAutoPlay.Checked;
            var isWidth = CbIsWidth.Checked;
            var isHeight = CbIsHeight.Checked;
            var playBy = DdlPlayBy.SelectedValue;
            var imageUrl = TbImageUrl.Text;
            var width = TranslateUtils.ToInt(TbWidth.Text);
            var height = TranslateUtils.ToInt(TbHeight.Text);

            if (isImageUrl && string.IsNullOrEmpty(imageUrl))
            {
                FailMessage("请上传视频封面图片");
                return;
            }

            if (isImageUrl != SiteInfo.Additional.ConfigUEditorVideoIsImageUrl
                || isAutoPlay != SiteInfo.Additional.ConfigUEditorVideoIsAutoPlay
                || isWidth != SiteInfo.Additional.ConfigUEditorVideoIsWidth
                || isHeight != SiteInfo.Additional.ConfigUEditorVideoIsHeight
                || playBy != SiteInfo.Additional.ConfigUEditorVideoPlayBy
                || width != SiteInfo.Additional.ConfigUEditorVideoWidth
                || height != SiteInfo.Additional.ConfigUEditorVideoHeight)
            {
                SiteInfo.Additional.ConfigUEditorVideoIsImageUrl = isImageUrl;
                SiteInfo.Additional.ConfigUEditorVideoIsAutoPlay = isAutoPlay;
                SiteInfo.Additional.ConfigUEditorVideoIsWidth = isWidth;
                SiteInfo.Additional.ConfigUEditorVideoIsHeight = isHeight;
                SiteInfo.Additional.ConfigUEditorVideoPlayBy = playBy;
                SiteInfo.Additional.ConfigUEditorVideoWidth = width;
                SiteInfo.Additional.ConfigUEditorVideoHeight = height;
                DataProvider.SiteDao.Update(SiteInfo);
            }

            var script = "parent." + UEditorUtils.GetInsertVideoScript(_attributeName, playUrl, imageUrl, SiteInfo);
            LayerUtils.CloseWithoutRefresh(Page, script);
		}

	}
}
