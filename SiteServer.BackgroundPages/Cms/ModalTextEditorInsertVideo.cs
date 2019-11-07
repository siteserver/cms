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

        public string VideoTypeCollection => Site.Additional.VideoUploadTypeCollection;
        public string ImageTypeCollection => Site.Additional.ImageUploadTypeCollection;

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

            CbIsImageUrl.Checked = Site.Additional.ConfigUEditorVideoIsImageUrl;
            CbIsAutoPlay.Checked = Site.Additional.ConfigUEditorVideoIsAutoPlay;
            CbIsWidth.Checked = Site.Additional.ConfigUEditorVideoIsWidth;
            CbIsHeight.Checked = Site.Additional.ConfigUEditorVideoIsHeight;
            ControlUtils.SelectSingleItem(DdlPlayBy, Site.Additional.ConfigUEditorVideoPlayBy);
            TbWidth.Text = Site.Additional.ConfigUEditorVideoWidth.ToString();
            TbHeight.Text = Site.Additional.ConfigUEditorVideoHeight.ToString();
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
                        if (!PathUtility.IsVideoExtenstionAllowed(Site, fileExtName))
                        {
                            message = "此格式不允许上传，请选择有效的音频文件";
                            isAllow = false;
                        }
                        if (!PathUtility.IsVideoSizeAllowed(Site, postedFile.ContentLength))
                        {
                            message = "上传失败，上传文件超出规定文件大小";
                            isAllow = false;
                        }

                        if (isAllow)
                        {
                            var localDirectoryPath = PathUtility.GetUploadDirectoryPath(Site, fileExtName);
                            var localFileName = PathUtility.GetUploadFileName(Site, filePath);
                            var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);

                            postedFile.SaveAs(localFilePath);

                            url = PageUtility.GetSiteUrlByPhysicalPathAsync(Site, localFilePath, true).GetAwaiter().GetResult();
                            url = PageUtility.GetVirtualUrl(Site, url);

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
                        if (!PathUtility.IsImageExtenstionAllowed(Site, fileExtName))
                        {
                            message = "此格式不允许上传，请选择有效的图片文件";
                            isAllow = false;
                        }
                        if (!PathUtility.IsImageSizeAllowed(Site, postedFile.ContentLength))
                        {
                            message = "上传失败，上传文件超出规定文件大小";
                            isAllow = false;
                        }

                        if (isAllow)
                        {
                            var localDirectoryPath = PathUtility.GetUploadDirectoryPath(Site, fileExtName);
                            var localFileName = PathUtility.GetUploadFileName(Site, filePath);
                            var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);

                            postedFile.SaveAs(localFilePath);

                            url = PageUtility.GetSiteUrlByPhysicalPathAsync(Site, localFilePath, true).GetAwaiter().GetResult();
                            url = PageUtility.GetVirtualUrl(Site, url);

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

            if (isImageUrl != Site.Additional.ConfigUEditorVideoIsImageUrl
                || isAutoPlay != Site.Additional.ConfigUEditorVideoIsAutoPlay
                || isWidth != Site.Additional.ConfigUEditorVideoIsWidth
                || isHeight != Site.Additional.ConfigUEditorVideoIsHeight
                || playBy != Site.Additional.ConfigUEditorVideoPlayBy
                || width != Site.Additional.ConfigUEditorVideoWidth
                || height != Site.Additional.ConfigUEditorVideoHeight)
            {
                Site.Additional.ConfigUEditorVideoIsImageUrl = isImageUrl;
                Site.Additional.ConfigUEditorVideoIsAutoPlay = isAutoPlay;
                Site.Additional.ConfigUEditorVideoIsWidth = isWidth;
                Site.Additional.ConfigUEditorVideoIsHeight = isHeight;
                Site.Additional.ConfigUEditorVideoPlayBy = playBy;
                Site.Additional.ConfigUEditorVideoWidth = width;
                Site.Additional.ConfigUEditorVideoHeight = height;
                DataProvider.SiteDao.UpdateAsync(Site).GetAwaiter().GetResult();
            }

            var script = "parent." + UEditorUtils.GetInsertVideoScript(_attributeName, playUrl, imageUrl, Site);
            LayerUtils.CloseWithoutRefresh(Page, script);
		}

	}
}
