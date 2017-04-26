using System.Web;
using BaiRong.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Core
{
	public class VisualInfo
	{
		private int publishmentSystemID;
        private int channelID;
        private int contentID;
        private int fileTemplateID;
        private int pageIndex;
        private ETemplateType templateType;
        private bool isPreview;
        private string includeUrl;
        private string filePath;

		private VisualInfo()
		{
            publishmentSystemID = channelID = contentID = fileTemplateID = pageIndex = 0;
            templateType = ETemplateType.IndexPageTemplate;
            isPreview = false;
            includeUrl = string.Empty;
            filePath = string.Empty;
		}

        public static VisualInfo GetInstance()
        {
            var visualInfo = new VisualInfo();

            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["s"]))
            {
                visualInfo.publishmentSystemID = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["s"]);
            }
            if (visualInfo.publishmentSystemID == 0)
            {
                visualInfo.publishmentSystemID = PathUtility.GetCurrentPublishmentSystemId();
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["n"]))
            {
                visualInfo.channelID = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["n"]);
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["c"]))
            {
                visualInfo.contentID = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["c"]);
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["f"]))
            {
                visualInfo.fileTemplateID = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["f"]);
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["p"]))
            {
                visualInfo.pageIndex = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["p"]);
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["isPreview"]))
            {
                visualInfo.isPreview = TranslateUtils.ToBool(HttpContext.Current.Request.QueryString["isPreview"]);
                if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["previewID"]))
                {
                    visualInfo.contentID = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["previewID"]);//编辑界面预览
                }
            }

            if (visualInfo.channelID > 0)
            {
                visualInfo.templateType = ETemplateType.ChannelTemplate;
            }
            if (visualInfo.contentID > 0 || visualInfo.isPreview)
            {
                visualInfo.templateType = ETemplateType.ContentTemplate;
            }
            if (visualInfo.fileTemplateID > 0)
            {
                visualInfo.templateType = ETemplateType.FileTemplate;
            }

            if (visualInfo.channelID == 0)
            {
                visualInfo.channelID = visualInfo.publishmentSystemID;
            }

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(visualInfo.publishmentSystemID);
            if (visualInfo.templateType == ETemplateType.IndexPageTemplate)
            {
                var templateInfo = TemplateManager.GetTemplateInfo(visualInfo.publishmentSystemID, 0, ETemplateType.IndexPageTemplate);
                var isHeadquarters = publishmentSystemInfo.IsHeadquarters;
                visualInfo.filePath = PathUtility.GetIndexPageFilePath(publishmentSystemInfo, templateInfo.CreatedFileFullName, isHeadquarters, visualInfo.pageIndex);
            }
            else if (visualInfo.templateType == ETemplateType.ChannelTemplate)
            {
                visualInfo.filePath = PathUtility.GetChannelPageFilePath(publishmentSystemInfo, visualInfo.channelID, visualInfo.pageIndex);
            }
            else if (visualInfo.templateType == ETemplateType.ContentTemplate)
            {
                visualInfo.filePath = PathUtility.GetContentPageFilePath(publishmentSystemInfo, visualInfo.channelID, visualInfo.contentID, visualInfo.pageIndex);
            }
            else if (visualInfo.templateType == ETemplateType.FileTemplate)
            {
                var templateInfo = TemplateManager.GetTemplateInfo(visualInfo.publishmentSystemID, visualInfo.fileTemplateID);
                visualInfo.filePath = PathUtility.MapPath(publishmentSystemInfo, templateInfo.CreatedFileFullName);
            }

            return visualInfo;
        }

		public int PublishmentSystemID => publishmentSystemID;

	    public int ChannelID => channelID;

	    public int ContentID => contentID;

	    public int FileTemplateID => fileTemplateID;

	    public ETemplateType TemplateType => templateType;

	    public bool IsPreview => isPreview;

	    public string IncludeUrl => includeUrl;

	    public string FilePath => filePath;

	    public int PageIndex => pageIndex;
	}
}
