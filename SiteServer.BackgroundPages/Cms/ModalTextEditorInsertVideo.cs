using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.Utils.Enumerations;
using SiteServer.Utils.LitJson;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalTextEditorInsertVideo : BasePageCms
    {
        public TextBox TbPlayUrl;
        public TextBox TbWidth;
        public TextBox TbHeight;
        public CheckBox CbIsAutoPlay;

        private string _attributeName;

        public static string GetOpenWindowString(int siteId, string attributeName)
        {
            return LayerUtils.GetOpenScript("插入视频", PageUtils.GetCmsUrl(siteId, nameof(ModalTextEditorInsertVideo), new NameValueCollection
            {
                {"AttributeName", attributeName}
            }), 600, 460);
        }

        public string UploadUrl => PageUtils.GetCmsUrl(SiteId, nameof(ModalTextEditorInsertVideo), new NameValueCollection
        {
            {"upload", true.ToString()}
        });

        public string TypeCollection => SiteInfo.Additional.VideoUploadTypeCollection;

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

            TbWidth.Text = SiteInfo.Additional.ConfigVideoContentInsertWidth.ToString();
            TbHeight.Text = SiteInfo.Additional.ConfigVideoContentInsertHeight.ToString();
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

                            playUrl = PageUtility.GetSiteUrlByPhysicalPath(SiteInfo, localFilePath, true);
                            playUrl = PageUtility.GetVirtualUrl(SiteInfo, playUrl);

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
            if (width > 0 && height > 0 && (width != SiteInfo.Additional.ConfigVideoContentInsertWidth || height != SiteInfo.Additional.ConfigVideoContentInsertHeight))
            {
                SiteInfo.Additional.ConfigVideoContentInsertWidth = width;
                SiteInfo.Additional.ConfigVideoContentInsertHeight = height;
                DataProvider.SiteDao.Update(SiteInfo);
            }

            var playUrl = TbPlayUrl.Text;

            var script = "parent." + ETextEditorTypeUtils.GetInsertVideoScript(_attributeName, playUrl, width, height, CbIsAutoPlay.Checked);
            LayerUtils.CloseWithoutRefresh(Page, script);
		}

	}
}
