using System;
using System.Globalization;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Model
{
    [Serializable]
    public class PublishmentSystemInfoExtend : ExtendedAttributes
    {
        public const string DefaultApiUrl = "/api";
        public const string DefaultHomeUrl = "/home";

        public PublishmentSystemInfoExtend(string settingsXml)
        {
            var nameValueCollection = TranslateUtils.ToNameValueCollection(settingsXml);
            SetExtendedAttribute(nameValueCollection);
        }

        /****************站点设置********************/

        public string Charset
        {
            get { return GetString("Charset", ECharsetUtils.GetValue(ECharset.utf_8)); }
            set { SetExtendedAttribute("Charset", value); }
        }

        public int PageSize
        {
            get { return GetInt("PageSize", 30); }
            set { SetExtendedAttribute("PageSize", value.ToString()); }
        }

        //public EVisualType VisualType
        //{
        //    get
        //    {
        //        EVisualType visualType = EVisualTypeUtils.GetEnumType(base.GetString("VisualType", EVisualTypeUtils.GetValue(EVisualType.Static)));
        //        if (visualType != EVisualType.Static && visualType != EVisualType.Dynamic)
        //        {
        //            visualType = EVisualType.Static;
        //        }
        //        return visualType;
        //    }
        //    set { base.SetExtendedAttribute("VisualType", EVisualTypeUtils.GetValue(value)); }
        //}

        public bool IsCountDownload
        {
            get { return GetBool("IsCountDownload", true); }
            set { SetExtendedAttribute("IsCountDownload", value.ToString()); }
        }

        public bool IsCountHits
        {
            get { return GetBool("IsCountHits", false); }
            set { SetExtendedAttribute("IsCountHits", value.ToString()); }
        }

        public bool IsCountHitsByDay
        {
            get { return GetBool("IsCountHitsByDay", false); }
            set { SetExtendedAttribute("IsCountHitsByDay", value.ToString()); }
        }

        public bool IsGroupContent
        {
            get { return GetBool("IsGroupContent", true); }
            set { SetExtendedAttribute("IsGroupContent", value.ToString()); }
        }

        public bool IsRelatedByTags
        {
            get { return GetBool("IsRelatedByTags", true); }
            set { SetExtendedAttribute("IsRelatedByTags", value.ToString()); }
        }

        public bool IsTranslate
        {
            get { return GetBool("IsTranslate", true); }
            set { SetExtendedAttribute("IsTranslate", value.ToString()); }
        }

        public bool IsAutoSaveContent
        {
            get { return GetBool("IsAutoSaveContent", true); }
            set { SetExtendedAttribute("IsAutoSaveContent", value.ToString()); }
        }

        public int AutoSaveContentInterval
        {
            get { return GetInt("AutoSaveContentInterval", 180); }
            set { SetExtendedAttribute("AutoSaveContentInterval", value.ToString()); }
        }

        public bool IsSaveImageInTextEditor
        {
            get { return GetBool("IsSaveImageInTextEditor", true); }
            set { SetExtendedAttribute("IsSaveImageInTextEditor", value.ToString()); }
        }

        public bool IsAutoPageInTextEditor
        {
            get { return GetBool("IsAutoPageInTextEditor", false); }
            set { SetExtendedAttribute("IsAutoPageInTextEditor", value.ToString()); }
        }

        public int AutoPageWordNum
        {
            get { return GetInt("AutoPageWordNum", 1500); }
            set { SetExtendedAttribute("AutoPageWordNum", value.ToString()); }
        }

        public bool IsContentTitleBreakLine
        {
            get { return GetBool("IsContentTitleBreakLine", false); }
            set { SetExtendedAttribute("IsContentTitleBreakLine", value.ToString()); }
        }

        /// <summary>
        /// 敏感词自动检测
        /// </summary>
        public bool IsAutoCheckKeywords
        {
            get { return GetBool("lIsAutoCheckKeywords", false); }
            set { SetExtendedAttribute("lIsAutoCheckKeywords", value.ToString()); }
        }

        /// <summary>
        /// 编辑器上传文件URL前缀
        /// </summary>
        public string EditorUploadFilePre
        {
            get { return GetString("EditorUploadFilePre", string.Empty); }
            set { SetExtendedAttribute("EditorUploadFilePre", value); }
        }

        public int PhotoSmallWidth
        {
            get { return GetInt("PhotoSmallWidth", 70); }
            set { SetExtendedAttribute("PhotoSmallWidth", value.ToString()); }
        }

        public int PhotoSmallHeight
        {
            get { return GetInt("PhotoSmallHeight", 70); }
            set { SetExtendedAttribute("PhotoSmallHeight", value.ToString()); }
        }

        public int PhotoMiddleWidth
        {
            get { return GetInt("PhotoMiddleWidth", 400); }
            set { SetExtendedAttribute("PhotoMiddleWidth", value.ToString()); }
        }

        public int PhotoMiddleHeight
        {
            get { return GetInt("PhotoMiddleHeight", 400); }
            set { SetExtendedAttribute("PhotoMiddleHeight", value.ToString()); }
        }

        /****************图片水印设置********************/

        public bool IsWaterMark
        {
            get { return GetBool("IsWaterMark", false); }
            set { SetExtendedAttribute("IsWaterMark", value.ToString()); }
        }

        public bool IsImageWaterMark
        {
            get { return GetBool("IsImageWaterMark", false); }
            set { SetExtendedAttribute("IsImageWaterMark", value.ToString()); }
        }

        public int WaterMarkPosition
        {
            get { return GetInt("WaterMarkPosition", 9); }
            set { SetExtendedAttribute("WaterMarkPosition", value.ToString()); }
        }

        public int WaterMarkTransparency
        {
            get { return GetInt("WaterMarkTransparency", 5); }
            set { SetExtendedAttribute("WaterMarkTransparency", value.ToString()); }
        }

        public int WaterMarkMinWidth
        {
            get { return GetInt("WaterMarkMinWidth", 200); }
            set { SetExtendedAttribute("WaterMarkMinWidth", value.ToString()); }
        }

        public int WaterMarkMinHeight
        {
            get { return GetInt("WaterMarkMinHeight", 200); }
            set { SetExtendedAttribute("WaterMarkMinHeight", value.ToString()); }
        }

        public string WaterMarkFormatString
        {
            get { return GetString("WaterMarkFormatString", string.Empty); }
            set { SetExtendedAttribute("WaterMarkFormatString", value); }
        }

        public string WaterMarkFontName
        {
            get { return GetString("WaterMarkFontName", string.Empty); }
            set { SetExtendedAttribute("WaterMarkFontName", value); }
        }

        public int WaterMarkFontSize
        {
            get { return GetInt("WaterMarkFontSize", 12); }
            set { SetExtendedAttribute("WaterMarkFontSize", value.ToString()); }
        }

        public string WaterMarkImagePath
        {
            get { return GetString("WaterMarkImagePath", string.Empty); }
            set { SetExtendedAttribute("WaterMarkImagePath", value); }
        }

        /****************生成页面设置********************/

        public bool IsMultiDeployment
        {
            get { return GetBool("IsMultiDeployment", false); }
            set { SetExtendedAttribute("IsMultiDeployment", value.ToString()); }
        }

        public string OuterUrl
        {
            get { return GetString("OuterUrl", string.Empty); }
            set { SetExtendedAttribute("OuterUrl", value); }
        }

        public string InnerUrl
        {
            get { return GetString("InnerUrl", string.Empty); }
            set { SetExtendedAttribute("InnerUrl", value); }
        }

        public string ApiUrl
        {
            get { return GetString("ApiUrl", DefaultApiUrl); }
            set { SetExtendedAttribute("ApiUrl", value); }
        }

        public string HomeUrl
        {
            get { return GetString("HomeUrl", DefaultHomeUrl); }
            set { SetExtendedAttribute("HomeUrl", value); }
        }

        public string ChannelFilePathRule
        {
            get { return GetString("ChannelFilePathRule", "/channels/{@ChannelID}.html"); }
            set { SetExtendedAttribute("ChannelFilePathRule", value); }
        }

        public string ContentFilePathRule
        {
            get { return GetString("ContentFilePathRule", "/contents/{@ChannelID}/{@ContentID}.html"); }
            set { SetExtendedAttribute("ContentFilePathRule", value); }
        }

        public bool IsCreateContentIfContentChanged
        {
            get { return GetBool("IsCreateContentIfContentChanged", true); }
            set { SetExtendedAttribute("IsCreateContentIfContentChanged", value.ToString()); }
        }

        public bool IsCreateChannelIfChannelChanged
        {
            get { return GetBool("IsCreateChannelIfChannelChanged", true); }
            set { SetExtendedAttribute("IsCreateChannelIfChannelChanged", value.ToString()); }
        }

        public bool IsCreateShowPageInfo
        {
            get { return GetBool("IsCreateShowPageInfo", false); }
            set { SetExtendedAttribute("IsCreateShowPageInfo", value.ToString()); }
        }

        public bool IsCreateIe8Compatible
        {
            get { return GetBool("IsCreateIe8Compatible", false); }
            set { SetExtendedAttribute("IsCreateIe8Compatible", value.ToString()); }
        }

        public bool IsCreateBrowserNoCache
        {
            get { return GetBool("IsCreateBrowserNoCache", false); }
            set { SetExtendedAttribute("IsCreateBrowserNoCache", value.ToString()); }
        }

        public bool IsCreateJsIgnoreError
        {
            get { return GetBool("IsCreateJsIgnoreError", false); }
            set { SetExtendedAttribute("IsCreateJsIgnoreError", value.ToString()); }
        }

        public bool IsCreateSearchDuplicate
        {
            get { return GetBool("IsCreateSearchDuplicate", true); }
            set { SetExtendedAttribute("IsCreateSearchDuplicate", value.ToString()); }
        }

        public bool IsCreateWithJQuery
        {
            get { return GetBool("IsCreateWithJQuery", true); }
            set { SetExtendedAttribute("IsCreateWithJQuery", value.ToString()); }
        }

        public bool IsCreateIncludeToSsi
        {
            get { return GetBool("IsCreateIncludeToSsi", false); }
            set { SetExtendedAttribute("IsCreateIncludeToSsi", value.ToString()); }
        }

        public bool IsCreateDoubleClick
        {
            get { return GetBool("IsCreateDoubleClick", false); }
            set { SetExtendedAttribute("IsCreateDoubleClick", value.ToString()); }
        }

        public int CreateStaticMaxPage
        {
            get { return GetInt("CreateStaticMaxPage", 10); }
            set { SetExtendedAttribute("CreateStaticMaxPage", value.ToString()); }
        }

        public bool IsCreateStaticContentByAddDate
        {
            get { return GetBool("IsCreateStaticContentByAddDate", false); }
            set { SetExtendedAttribute("IsCreateStaticContentByAddDate", value.ToString()); }
        }

        public DateTime CreateStaticContentAddDate
        {
            get { return GetDateTime("CreateStaticContentAddDate", DateTime.MinValue); }
            set { SetExtendedAttribute("CreateStaticContentAddDate", DateUtils.GetDateString(value)); }
        }

        /****************站点地图设置********************/

        public string SiteMapGooglePath
        {
            get { return GetString("SiteMapGooglePath", "@/sitemap.xml"); }
            set { SetExtendedAttribute("SiteMapGooglePath", value); }
        }

        public string SiteMapGoogleChangeFrequency
        {
            get { return GetString("SiteMapGoogleChangeFrequency", "daily"); }
            set { SetExtendedAttribute("SiteMapGoogleChangeFrequency", value); }
        }

        public bool SiteMapGoogleIsShowLastModified
        {
            get { return GetBool("SiteMapGoogleIsShowLastModified", false); }
            set { SetExtendedAttribute("SiteMapGoogleIsShowLastModified", value.ToString()); }
        }

        public int SiteMapGooglePageCount
        {
            get { return GetInt("SiteMapGooglePageCount", 10000); }
            set { SetExtendedAttribute("SiteMapGooglePageCount", value.ToString()); }
        }

        public string SiteMapBaiduPath
        {
            get { return GetString("SiteMapBaiduPath", "@/baidunews.xml"); }
            set { SetExtendedAttribute("SiteMapBaiduPath", value); }
        }

        public string SiteMapBaiduWebMaster
        {
            get { return GetString("SiteMapBaiduWebMaster", string.Empty); }
            set { SetExtendedAttribute("SiteMapBaiduWebMaster", value); }
        }

        public string SiteMapBaiduUpdatePeri
        {
            get { return GetString("SiteMapBaiduUpdatePeri", "15"); }
            set { SetExtendedAttribute("SiteMapBaiduUpdatePeri", value); }
        }

        /****************流量统计设置********************/

        public bool IsTracker
        {
            get { return GetBool("IsTracker", false); }
            set { SetExtendedAttribute("IsTracker", value.ToString()); }
        }

        public int TrackerDays
        {
            get { return GetInt("TrackerDays", 0); }
            set { SetExtendedAttribute("TrackerDays", value.ToString()); }
        }

        public int TrackerPageView
        {
            get { return GetInt("TrackerPageView", 0); }
            set { SetExtendedAttribute("TrackerPageView", value.ToString()); }
        }

        public int TrackerUniqueVisitor
        {
            get { return GetInt("TrackerUniqueVisitor", 0); }
            set { SetExtendedAttribute("TrackerUniqueVisitor", value.ToString()); }
        }

        public int TrackerCurrentMinute
        {
            get { return GetInt("TrackerCurrentMinute", 30); }
            set { SetExtendedAttribute("TrackerCurrentMinute", value.ToString()); }
        }

        public ETrackerStyle TrackerStyle
        {
            get { return ETrackerStyleUtils.GetEnumType(GetString("TrackerStyle", ETrackerStyleUtils.GetValue(ETrackerStyle.Style1))); }
            set { SetExtendedAttribute("TrackerStyle", ETrackerStyleUtils.GetValue(value)); }
        }

        /****************显示项设置********************/

        public string ChannelDisplayAttributes
        {
            get { return GetString("ChannelDisplayAttributes", string.Empty); }
            set { SetExtendedAttribute("ChannelDisplayAttributes", value); }
        }

        public string ChannelEditAttributes
        {
            get { return GetString("ChannelEditAttributes", string.Empty); }
            set { SetExtendedAttribute("ChannelEditAttributes", value); }
        }

        /****************跨站转发设置********************/

        public bool IsCrossSiteTransChecked
        {
            get { return GetBool("IsCrossSiteTransChecked", false); }
            set { SetExtendedAttribute("IsCrossSiteTransChecked", value.ToString()); }
        }

        /****************站内链接设置********************/

        public bool IsInnerLink
        {
            get { return GetBool("IsInnerLink", false); }
            set { SetExtendedAttribute("IsInnerLink", value.ToString()); }
        }

        public bool IsInnerLinkByChannelName
        {
            get { return GetBool("IsInnerLinkByChannelName", false); }
            set { SetExtendedAttribute("IsInnerLinkByChannelName", value.ToString()); }
        }

        public string InnerLinkFormatString
        {
            get { return GetString("InnerLinkFormatString", @"<a href=""{0}"" target=""_blank"">{1}</a>"); }
            set { SetExtendedAttribute("InnerLinkFormatString", value); }
        }

        public int InnerLinkMaxNum
        {
            get { return GetInt("InnerLinkMaxNum", 10); }
            set { SetExtendedAttribute("InnerLinkMaxNum", value.ToString()); }
        }

        /****************记录系统操作设置********************/

        public bool ConfigTemplateIsCodeMirror
        {
            get { return GetBool("ConfigTemplateIsCodeMirror", true); }
            set { SetExtendedAttribute("ConfigTemplateIsCodeMirror", value.ToString()); }
        }

        public int ConfigVideoContentInsertWidth
        {
            get { return GetInt("ConfigVideoContentInsertWidth", 420); }
            set { SetExtendedAttribute("ConfigVideoContentInsertWidth", value.ToString()); }
        }

        public int ConfigVideoContentInsertHeight
        {
            get { return GetInt("ConfigVideoContentInsertHeight", 280); }
            set { SetExtendedAttribute("ConfigVideoContentInsertHeight", value.ToString()); }
        }

        public string ConfigExportType
        {
            get { return GetString("ConfigExportType", string.Empty); }
            set { SetExtendedAttribute("ConfigExportType", value); }
        }

        public string ConfigExportPeriods
        {
            get { return GetString("ConfigExportPeriods", string.Empty); }
            set { SetExtendedAttribute("ConfigExportPeriods", value); }
        }

        public string ConfigExportDisplayAttributes
        {
            get { return GetString("ConfigExportDisplayAttributes", string.Empty); }
            set { SetExtendedAttribute("ConfigExportDisplayAttributes", value); }
        }

        public string ConfigExportIsChecked
        {
            get { return GetString("ConfigExportIsChecked", string.Empty); }
            set { SetExtendedAttribute("ConfigExportIsChecked", value); }
        }

        public string ConfigSelectImageCurrentUrl
        {
            get { return GetString("ConfigSelectImageCurrentUrl", "@/" + ImageUploadDirectoryName); }
            set { SetExtendedAttribute("ConfigSelectImageCurrentUrl", value); }
        }

        public string ConfigSelectVideoCurrentUrl
        {
            get { return GetString("ConfigSelectVideoCurrentUrl", "@/" + VideoUploadDirectoryName); }
            set { SetExtendedAttribute("ConfigSelectVideoCurrentUrl", value); }
        }

        public string ConfigSelectFileCurrentUrl
        {
            get { return GetString("ConfigSelectFileCurrentUrl", "@/" + FileUploadDirectoryName); }
            set { SetExtendedAttribute("ConfigSelectFileCurrentUrl", value); }
        }

        public string ConfigUploadImageIsTitleImage
        {
            get { return GetString("ConfigUploadImageIsTitleImage", "True"); }
            set { SetExtendedAttribute("ConfigUploadImageIsTitleImage", value); }
        }

        public string ConfigUploadImageTitleImageWidth
        {
            get { return GetString("ConfigUploadImageTitleImageWidth", "300"); }
            set { SetExtendedAttribute("ConfigUploadImageTitleImageWidth", value); }
        }

        public string ConfigUploadImageTitleImageHeight
        {
            get { return GetString("ConfigUploadImageTitleImageHeight", string.Empty); }
            set { SetExtendedAttribute("ConfigUploadImageTitleImageHeight", value); }
        }

        public string ConfigUploadImageIsTitleImageLessSizeNotThumb
        {
            get { return GetString("ConfigUploadImageIsTitleImageLessSizeNotThumb", string.Empty); }
            set { SetExtendedAttribute("ConfigUploadImageIsTitleImageLessSizeNotThumb", value); }
        }

        public string ConfigUploadImageIsShowImageInTextEditor
        {
            get { return GetString("ConfigUploadImageIsShowImageInTextEditor", "True"); }
            set { SetExtendedAttribute("ConfigUploadImageIsShowImageInTextEditor", value); }
        }

        public string ConfigUploadImageIsLinkToOriginal
        {
            get { return GetString("ConfigUploadImageIsLinkToOriginal", string.Empty); }
            set { SetExtendedAttribute("ConfigUploadImageIsLinkToOriginal", value); }
        }

        public string ConfigUploadImageIsSmallImage
        {
            get { return GetString("ConfigUploadImageIsSmallImage", "True"); }
            set { SetExtendedAttribute("ConfigUploadImageIsSmallImage", value); }
        }

        public string ConfigUploadImageSmallImageWidth
        {
            get { return GetString("ConfigUploadImageSmallImageWidth", "500"); }
            set { SetExtendedAttribute("ConfigUploadImageSmallImageWidth", value); }
        }

        public string ConfigUploadImageSmallImageHeight
        {
            get { return GetString("ConfigUploadImageSmallImageHeight", string.Empty); }
            set { SetExtendedAttribute("ConfigUploadImageSmallImageHeight", value); }
        }

        public string ConfigUploadImageIsSmallImageLessSizeNotThumb
        {
            get { return GetString("ConfigUploadImageIsSmallImageLessSizeNotThumb", string.Empty); }
            set { SetExtendedAttribute("ConfigUploadImageIsSmallImageLessSizeNotThumb", value); }
        }

        /****************评论设置********************/

        public bool IsCommentable
        {
            get { return GetBool("IsCommentable", true); }
            set { SetExtendedAttribute("IsCommentable", value.ToString()); }
        }

        public bool IsCheckComments
        {
            get { return GetBool("IsCheckComments", false); }
            set { SetExtendedAttribute("IsCheckComments", value.ToString()); }
        }

        public bool IsAnonymousComments
        {
            get { return GetBool("IsAnonymousComments", true); }
            set { SetExtendedAttribute("IsAnonymousComments", value.ToString()); }
        }

        /****************站点基本设置********************/
        public string SiteSettingsCollection
        {
            get { return GetString("SiteSettingsCollection", string.Empty); }
            set { SetExtendedAttribute("SiteSettingsCollection", value); }
        }

        #region 上传设置

        public string ImageUploadDirectoryName
        {
            get { return GetString("ImageUploadDirectoryName", "upload/images"); }
            set { SetExtendedAttribute("ImageUploadDirectoryName", value); }
        }

        public string ImageUploadDateFormatString
        {
            get { return GetString("ImageUploadDateFormatString", EDateFormatTypeUtils.GetValue(EDateFormatType.Month)); }
            set { SetExtendedAttribute("ImageUploadDateFormatString", value); }
        }

        public bool IsImageUploadChangeFileName
        {
            get { return GetBool("IsImageUploadChangeFileName", true); }
            set { SetExtendedAttribute("IsImageUploadChangeFileName", value.ToString()); }
        }

        public string ImageUploadTypeCollection
        {
            get { return GetString("ImageUploadTypeCollection", "gif|jpg|jpeg|bmp|png|pneg|swf"); }
            set { SetExtendedAttribute("ImageUploadTypeCollection", value); }
        }

        public int ImageUploadTypeMaxSize
        {
            get { return GetInt("ImageUploadTypeMaxSize", 15360); }
            set { SetExtendedAttribute("ImageUploadTypeMaxSize", value.ToString()); }
        }

        public string VideoUploadDirectoryName
        {
            get { return GetString("VideoUploadDirectoryName", "upload/videos"); }
            set { SetExtendedAttribute("VideoUploadDirectoryName", value); }
        }

        public string VideoUploadDateFormatString
        {
            get { return GetString("VideoUploadDateFormatString", EDateFormatTypeUtils.GetValue(EDateFormatType.Month)); }
            set { SetExtendedAttribute("VideoUploadDateFormatString", value); }
        }

        public bool IsVideoUploadChangeFileName
        {
            get { return GetBool("IsVideoUploadChangeFileName", true); }
            set { SetExtendedAttribute("IsVideoUploadChangeFileName", value.ToString()); }
        }

        public string VideoUploadTypeCollection
        {
            get { return GetString("VideoUploadTypeCollection", "asf|asx|avi|flv|mid|midi|mov|mp3|mp4|mpg|mpeg|ogg|ra|rm|rmb|rmvb|rp|rt|smi|swf|wav|webm|wma|wmv|viv"); }
            set { SetExtendedAttribute("VideoUploadTypeCollection", value); }
        }

        public int VideoUploadTypeMaxSize
        {
            get { return GetInt("VideoUploadTypeMaxSize", 307200); }
            set { SetExtendedAttribute("VideoUploadTypeMaxSize", value.ToString()); }
        }

        public string FileUploadDirectoryName
        {
            get { return GetString("FileUploadDirectoryName", "upload/files"); }
            set { SetExtendedAttribute("FileUploadDirectoryName", value); }
        }

        public string FileUploadDateFormatString
        {
            get { return GetString("FileUploadDateFormatString", EDateFormatTypeUtils.GetValue(EDateFormatType.Month)); }
            set { SetExtendedAttribute("FileUploadDateFormatString", value); }
        }

        public bool IsFileUploadChangeFileName
        {
            get { return GetBool("IsFileUploadChangeFileName", true); }
            set { SetExtendedAttribute("IsFileUploadChangeFileName", value.ToString()); }
        }

        public string FileUploadTypeCollection
        {
            get { return GetString("FileUploadTypeCollection", "zip,rar,7z,js,css,txt,doc,docx,ppt,pptx,xls,xlsx,pdf"); }
            set { SetExtendedAttribute("FileUploadTypeCollection", value); }
        }

        public int FileUploadTypeMaxSize
        {
            get { return GetInt("FileUploadTypeMaxSize", 307200); }
            set { SetExtendedAttribute("FileUploadTypeMaxSize", value.ToString()); }
        }

        #endregion

        #region WCM

        /****************信息公开设置********************/

        public int GovPublicNodeId
        {
            get { return GetInt("GovPublicNodeId", 0); }
            set { SetExtendedAttribute("GovPublicNodeId", value.ToString()); }
        }

        public bool GovPublicIsPublisherRelatedDepartmentId
        {
            get { return GetBool("GovPublicIsPublisherRelatedDepartmentId", true); }
            set { SetExtendedAttribute("GovPublicIsPublisherRelatedDepartmentId", value.ToString()); }
        }

        public string GovPublicDepartmentIdCollection
        {
            get { return GetString("GovPublicDepartmentIdCollection", string.Empty); }
            set { SetExtendedAttribute("GovPublicDepartmentIdCollection", value); }
        }

        /****************依申请公开设置********************/

        public int GovPublicApplyDateLimit              //办理时限
        {
            get { return GetInt("GovPublicApplyDateLimit", 15); }
            set { SetExtendedAttribute("GovPublicApplyDateLimit", value.ToString()); }
        }

        public int GovPublicApplyAlertDate              //预警
        {
            get { return GetInt("GovPublicApplyAlertDate", -3); }
            set { SetExtendedAttribute("GovPublicApplyAlertDate", value.ToString()); }
        }

        public int GovPublicApplyYellowAlertDate      //黄牌
        {
            get { return GetInt("GovPublicApplyYellowAlertDate", 3); }
            set { SetExtendedAttribute("GovPublicApplyYellowAlertDate", value.ToString()); }
        }

        public int GovPublicApplyRedAlertDate       //红牌
        {
            get { return GetInt("GovPublicApplyRedAlertDate", 10); }
            set { SetExtendedAttribute("GovPublicApplyRedAlertDate", value.ToString()); }
        }

        public bool GovPublicApplyIsDeleteAllowed   //是否允许删除
        {
            get { return GetBool("GovPublicApplyIsDeleteAllowed", true); }
            set { SetExtendedAttribute("GovPublicApplyIsDeleteAllowed", value.ToString()); }
        }

        /****************互动交流设置********************/

        public int GovInteractNodeId
        {
            get { return GetInt("GovInteractNodeId", 0); }
            set { SetExtendedAttribute("GovInteractNodeId", value.ToString()); }
        }

        public int GovInteractApplyDateLimit              //办理时限
        {
            get { return GetInt("GovInteractApplyDateLimit", 15); }
            set { SetExtendedAttribute("GovInteractApplyDateLimit", value.ToString()); }
        }

        public int GovInteractApplyAlertDate              //预警
        {
            get { return GetInt("GovInteractApplyAlertDate", -3); }
            set { SetExtendedAttribute("GovInteractApplyAlertDate", value.ToString()); }
        }

        public int GovInteractApplyYellowAlertDate      //黄牌
        {
            get { return GetInt("GovInteractApplyYellowAlertDate", 3); }
            set { SetExtendedAttribute("GovInteractApplyYellowAlertDate", value.ToString()); }
        }

        public int GovInteractApplyRedAlertDate       //红牌
        {
            get { return GetInt("GovInteractApplyRedAlertDate", 10); }
            set { SetExtendedAttribute("GovInteractApplyRedAlertDate", value.ToString()); }
        }

        public bool GovInteractApplyIsDeleteAllowed   //是否允许删除
        {
            get { return GetBool("GovInteractApplyIsDeleteAllowed", true); }
            set { SetExtendedAttribute("GovInteractApplyIsDeleteAllowed", value.ToString()); }
        }

        public bool GovInteractApplyIsOpenWindow   //是否新窗口打开
        {
            get { return GetBool("GovInteractApplyIsOpenWindow", false); }
            set { SetExtendedAttribute("GovInteractApplyIsOpenWindow", value.ToString()); }
        }

        #endregion

        #region weixin

        public bool WxIsWebMenu
        {
            get { return GetBool("WxIsWebMenu", false); }
            set { SetExtendedAttribute("WxIsWebMenu", value.ToString()); }
        }

        public string WxWebMenuType
        {
            get { return GetString("WxWebMenuType", "Type1"); }
            set { SetExtendedAttribute("WxWebMenuType", value); }
        }

        public string WxWebMenuColor
        {
            get { return GetString("WxWebMenuColor", "41C281"); }
            set { SetExtendedAttribute("WxWebMenuColor", value); }
        }

        public bool WxIsWebMenuLeft
        {
            get { return GetBool("WxIsWebMenuLeft", true); }
            set { SetExtendedAttribute("WxIsWebMenuLeft", value.ToString()); }
        }

        public bool CardIsClaimCardCredits
        {
            get { return GetBool("CardIsClaimCardCredits", false); }
            set { SetExtendedAttribute("CardIsClaimCardCredits", value.ToString()); }
        }

        public int CardClaimCardCredits
        {
            get { return GetInt("CardClaimCardCredits", 20); }
            set { SetExtendedAttribute("CardClaimCardCredits", value.ToString()); }
        }

        public bool CardIsGiveConsumeCredits
        {
            get { return GetBool("CardIsGiveConsumeCredits", false); }
            set { SetExtendedAttribute("CardIsGiveConsumeCredits", value.ToString()); }
        }

        public decimal CardConsumeAmount
        {
            get { return GetDecimal("CardConsumeAmount", 100); }
            set { SetExtendedAttribute("CardConsumeAmount", value.ToString(CultureInfo.InvariantCulture)); }
        }

        public int CardGiveCredits
        {
            get { return GetInt("CardGiveCredits", 50); }
            set { SetExtendedAttribute("CardGiveCredits", value.ToString()); }
        }

        public bool CardIsBinding
        {
            get { return GetBool("CardIsBinding", true); }
            set { SetExtendedAttribute("CardIsBinding", value.ToString()); }
        }

        public bool CardIsExchange
        {
            get { return GetBool("CardIsExchange", true); }
            set { SetExtendedAttribute("CardIsExchange", value.ToString()); }
        }

        public decimal CardExchangeProportion
        {
            get { return GetDecimal("CardExchangeProportion", 10); }
            set { SetExtendedAttribute("CardExchangeProportion", value.ToString(CultureInfo.InvariantCulture)); }
        }

        public bool CardIsSign
        {
            get { return GetBool("CardIsSign", false); }
            set { SetExtendedAttribute("CardIsSign", value.ToString()); }
        }

        public string CardSignCreditsConfigure
        {
            get { return GetString("CardSignCreditsConfigure", string.Empty); }
            set { SetExtendedAttribute("CardSignCreditsConfigure", value); }
        }

        #endregion

        public override string ToString()
        {
            return TranslateUtils.NameValueCollectionToString(Attributes);
        }
    }
}
