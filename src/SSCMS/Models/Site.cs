using System;
using System.Collections.Generic;
using Datory;
using Datory.Annotations;
using SSCMS.Enums;

namespace SSCMS.Models
{
    [DataTable("siteserver_Site")]
    public class Site : Entity
    {
        [DataColumn]
        public string SiteDir { get; set; }

        [DataColumn]
        public string SiteName { get; set; }

        [DataColumn]
        public string SiteType { get; set; }

        [DataColumn]
        public string ImageUrl { get; set; }

        [DataColumn]
        public string Keywords { get; set; }

        [DataColumn]
        public string Description { get; set; }

        [DataColumn]
        public string TableName { get; set; }

        [DataColumn]
        public bool Root { get; set; }

        [DataColumn]
        public int ParentId { get; set; }

        [DataColumn]
        public int Taxis { get; set; }

        public IList<Site> Children { get; set; }

        public int PageSize { get; set; } = 30;

        public int CheckContentLevel { get; set; } = 1;

        public int CheckContentDefaultLevel { get; set; } = 1;

        public bool IsSaveImageInTextEditor { get; set; } = true;

        public bool IsAutoPageInTextEditor { get; set; }

        public int AutoPageWordNum { get; set; } = 1500;

        public bool IsContentTitleBreakLine { get; set; } = true;

        public bool IsContentSubTitleBreakLine { get; set; } = true;

        public bool IsAutoCheckKeywords { get; set; }

        public int PhotoSmallWidth { get; set; } = 70;

        public int PhotoMiddleWidth { get; set; } = 400;

        public bool IsWaterMark { get; set; }

        public bool IsImageWaterMark { get; set; }

        public int WaterMarkPosition { get; set; } = 9;

        public int WaterMarkTransparency { get; set; } = 5;

        public int WaterMarkMinWidth { get; set; } = 200;

        public int WaterMarkMinHeight { get; set; } = 200;

        public string WaterMarkFormatString { get; set; }

        public string WaterMarkFontName { get; set; }

        public int WaterMarkFontSize { get; set; } = 12;

        public string WaterMarkImagePath { get; set; }

        public bool IsSeparatedWeb { get; set; }

        public string SeparatedWebUrl { get; set; }

        public bool IsSeparatedAssets { get; set; }

        public string SeparatedAssetsUrl { get; set; }

        public bool IsSeparatedApi { get; set; }

        public string SeparatedApiUrl { get; set; }

        public string AssetsDir { get; set; } = "upload";

        public string ChannelFilePathRule { get; set; } = "/channels/{@ChannelID}.html";

        public string ContentFilePathRule { get; set; } = "/contents/{@ChannelID}/{@ContentID}.html";

        public bool IsCreateContentIfContentChanged { get; set; } = true;

        public bool IsCreateChannelIfChannelChanged { get; set; } = true;

        public bool IsCreateShowPageInfo { get; set; }

        public bool IsCreateIe8Compatible { get; set; }

        public bool IsCreateBrowserNoCache { get; set; }

        public bool IsCreateJsIgnoreError { get; set; }

        public bool IsCreateWithJQuery { get; set; } = true;

        public bool IsCreateDoubleClick { get; set; }

        public int CreateStaticMaxPage { get; set; } = 10;

        public bool IsCreateUseDefaultFileName { get; set; }

        public string CreateDefaultFileName { get; set; } = "index.html";

        public bool IsCreateStaticContentByAddDate { get; set; }

        public DateTime CreateStaticContentAddDate { get; set; } = DateTime.MinValue;

        public bool IsCrossSiteTransChecked { get; set; }

        public List<string> SearchListColumns { get; set; }

        public List<string> CheckListColumns { get; set; }

        public List<string> RecycleListColumns { get; set; }

        public List<string> ChannelListColumns { get; set; }

        public bool ConfigUEditorVideoIsImageUrl { get; set; }

        public bool ConfigUEditorVideoIsAutoPlay { get; set; }

        public bool ConfigUEditorVideoIsWidth { get; set; }

        public bool ConfigUEditorVideoIsHeight { get; set; }

        public string ConfigUEditorVideoPlayBy { get; set; }

        public int ConfigUEditorVideoWidth { get; set; } = 600;

        public int ConfigUEditorVideoHeight { get; set; } = 400;

        public bool ConfigUEditorAudioIsAutoPlay { get; set; }

        public string ConfigExportType { get; set; }

        public string ConfigExportPeriods { get; set; }

        public string ConfigExportDisplayAttributes { get; set; }

        public string ConfigExportIsChecked { get; set; }

        public string ConfigSelectImageCurrentUrl { get; set; }

        public string ConfigSelectVideoCurrentUrl { get; set; }

        public string ConfigSelectFileCurrentUrl { get; set; }

        public string ConfigUploadImageIsTitleImage { get; set; } = "True";

        public string ConfigUploadImageTitleImageWidth { get; set; } = "300";

        public string ConfigUploadImageTitleImageHeight { get; set; }

        public string ConfigUploadImageIsShowImageInTextEditor { get; set; } = "True";

        public string ConfigUploadImageIsLinkToOriginal { get; set; }

        public string ConfigUploadImageIsSmallImage { get; set; } = "True";

        public string ConfigUploadImageSmallImageWidth { get; set; } = "500";

        public string ConfigUploadImageSmallImageHeight { get; set; }

        public bool ConfigImageIsFix { get; set; } = true;

        public string ConfigImageFixWidth { get; set; } = "300";

        public string ConfigImageFixHeight { get; set; }

        public bool ConfigImageIsEditor { get; set; } = true;

        public bool ConfigImageEditorIsFix { get; set; } = true;

        public string ConfigImageEditorFixWidth { get; set; } = "500";

        public string ConfigImageEditorFixHeight { get; set; }

        public bool ConfigImageEditorIsLinkToOriginal { get; set; }

        public string ImageUploadDirectoryName { get; set; } = "upload/images";

        public DateFormatType ImageUploadDateFormatString { get; set; } = DateFormatType.Month;

        public bool IsImageUploadChangeFileName { get; set; } = true;

        public string ImageUploadExtensions { get; set; } = ".gif,.jpg,.jpeg,.bmp,.png,.pneg,.swf,.webp,.svg";

        public int ImageUploadTypeMaxSize { get; set; } = 15360;

        public string AudioUploadDirectoryName { get; set; } = "upload/audio";

        public DateFormatType AudioUploadDateFormatString { get; set; } = DateFormatType.Month;

        public bool IsAudioUploadChangeFileName { get; set; } = true;

        public string AudioUploadExtensions { get; set; } = ".mp3,.wma,.wav,.amr,.m4a";

        public int AudioUploadTypeMaxSize { get; set; } = 307200;

        public string VideoUploadDirectoryName { get; set; } = "upload/videos";

        public DateFormatType VideoUploadDateFormatString { get; set; } = DateFormatType.Month;

        public bool IsVideoUploadChangeFileName { get; set; } = true;

        public string VideoUploadExtensions { get; set; } =
            ".asf,.asx,.avi,.flv,.mid,.midi,.mov,.mp3,.mp4,.mpg,.mpeg,.ogg,.ra,.rm,.rmb,.rmvb,.rp,.rt,.smi,.swf,.wav,.webm,.wma,.wmv,.viv,.f4v,.m4v,.3gp,.3g2";

        public int VideoUploadTypeMaxSize { get; set; } = 307200;

        public string FileUploadDirectoryName { get; set; } = "upload/files";

        public DateFormatType FileUploadDateFormatString { get; set; } = DateFormatType.Month;

        public bool IsFileUploadChangeFileName { get; set; } = true;

        public string FileUploadExtensions { get; set; } = ".zip,.rar,.7z,.tar,.gz,.bz2,.cab,.iso,.doc,.docx,.xls,.xlsx,.ppt,.pptx,.pdf,.txt,.md,.xml,.js,.css,.html,.htm";

        public int FileUploadTypeMaxSize { get; set; } = 307200;

        public string TemplatesAssetsIncludeDir { get; set; } = "include";

        public string TemplatesAssetsJsDir { get; set; } = "js";

        public string TemplatesAssetsCssDir { get; set; } = "css";
    }
}
