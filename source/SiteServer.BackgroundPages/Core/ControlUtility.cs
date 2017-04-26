using SiteServer.CMS.Model;
using SiteServer.BackgroundPages.Cms;

namespace SiteServer.BackgroundPages.Core
{
    public class ControlUtility
    {
        public static string GetImageUrlButtonGroupHtml(PublishmentSystemInfo publishmentSystemInfo, string textBoxID)
        {
            var selectImageClick = ModalSelectImage.GetOpenWindowString(publishmentSystemInfo, textBoxID);
            var uploadImageClick = ModalUploadImageSingle.GetOpenWindowStringToTextBox(publishmentSystemInfo.PublishmentSystemId, textBoxID);
            var cuttingImageClick = ModalCuttingImage.GetOpenWindowStringWithTextBox(publishmentSystemInfo.PublishmentSystemId, textBoxID);
            var previewImageClick = ModalMessage.GetOpenWindowStringToPreviewImage(publishmentSystemInfo.PublishmentSystemId, textBoxID);

            return $@"
<div class=""btn-group"">
    <a class=""btn"" href=""javascript:;"" onclick=""{selectImageClick};return false;"" title=""选择""><i class=""icon-th""></i></a>
    <a class=""btn"" href=""javascript:;"" onclick=""{uploadImageClick};return false;"" title=""上传""><i class=""icon-arrow-up""></i></a>
    <a class=""btn"" href=""javascript:;"" onclick=""{cuttingImageClick};return false;"" title=""裁切""><i class=""icon-crop""></i></a>
    <a class=""btn"" href=""javascript:;"" onclick=""{previewImageClick};return false;"" title=""预览""><i class=""icon-eye-open""></i></a>
</div>
";
        }
    }
}
