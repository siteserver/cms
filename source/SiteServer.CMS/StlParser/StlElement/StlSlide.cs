using System.Collections.Generic;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "图片幻灯片", Description = "通过 stl:slide 标签在模板中显示图片幻灯片")]
    public class StlSlide
	{
        private StlSlide() { }
		public const string ElementName = "stl:slide";

        public static SortedList<string, string> AttributeList => null;

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JsAcSwfObject);

            //var contentInfo = contextInfo.ContentInfo ??
            //                  DataProvider.ContentDao.GetContentInfo(ETableStyle.BackgroundContent, pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, contextInfo.ContentId);
            var contentInfo = contextInfo.ContentInfo ??
                              Content.GetContentInfo(ETableStyle.BackgroundContent, pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, contextInfo.ContentId);

            //var photoInfoList = DataProvider.PhotoDao.GetPhotoInfoList(pageInfo.PublishmentSystemId, contextInfo.ContentId);
            var photoInfoList = Photo.GetPhotoInfoList(pageInfo.PublishmentSystemId, contextInfo.ContentId);

            var builder = new StringBuilder();

            builder.Append($@"
<script type=""text/javascript"">
var slideFullScreenUrl = ""{SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.Slide.FullScreenSwf)}"";
");

            builder.Append(@"
var slide_data = {
");

            builder.Append($@"
    ""slide"":{{""title"":""{StringUtils.ToJsString(contentInfo.Title)}""}},
    ""images"":[
");


            foreach (var photoInfo in photoInfoList)
            {
                builder.Append($@"
            {{""title"":""{StringUtils.ToJsString(contentInfo.Title)}"",""intro"":""{StringUtils.ToJsString(
                    photoInfo.Description)}"",""previewUrl"":""{StringUtils.ToJsString(
                    PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, photoInfo.SmallUrl))}"",""imageUrl"":""{StringUtils
                    .ToJsString(PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, photoInfo.LargeUrl))}"",""id"":""{photoInfo
                    .ID}""}},");
            }

            if (photoInfoList.Count > 0)
            {
                builder.Length -= 1;
            }

            builder.Append(@"
    ],
");

            //var siblingContentId = BaiRongDataProvider.ContentDao.GetContentId(pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, contentInfo.NodeId, contentInfo.Taxis, true);
            var siblingContentId = Content.GetContentId(pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, contentInfo.NodeId, contentInfo.Taxis, true);

            if (siblingContentId > 0)
            {
                var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contentInfo.NodeId);
                var tableStyle = NodeManager.GetTableStyle(pageInfo.PublishmentSystemInfo, nodeInfo);
                var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, nodeInfo);
                //var siblingContentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, siblingContentId);
                var siblingContentInfo = Content.GetContentInfo(tableStyle, tableName, siblingContentId);
                var title = siblingContentInfo.Title;
                var url = PageUtility.GetContentUrl(pageInfo.PublishmentSystemInfo, siblingContentInfo);
                //var photoInfo = DataProvider.PhotoDao.GetFirstPhotoInfo(pageInfo.PublishmentSystemId, siblingContentId);
                var photoInfo = Photo.GetFirstPhotoInfo(pageInfo.PublishmentSystemId, siblingContentId);
                var previewUrl = photoInfo != null ? PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, photoInfo.SmallUrl) : SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.FileS);
                builder.Append($@"
    ""next_album"":{{""title"":""{StringUtils.ToJsString(title)}"",""url"":""{StringUtils.ToJsString(url)}"",""previewUrl"":""{StringUtils.ToJsString(previewUrl)}""}},
");
            }
            else
            {
                builder.Append($@"
    ""next_album"":{{""title"":"""",""url"":""javascript:void(0);"",""previewUrl"":""{SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.FileS)}""}},
");
            }

            //siblingContentId = BaiRongDataProvider.ContentDao.GetContentId(pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, contentInfo.NodeId, contentInfo.Taxis, false);
            siblingContentId = Content.GetContentId(pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, contentInfo.NodeId, contentInfo.Taxis, false);

            if (siblingContentId > 0)
            {
                var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contentInfo.NodeId);
                var tableStyle = NodeManager.GetTableStyle(pageInfo.PublishmentSystemInfo, nodeInfo);
                var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, nodeInfo);
                //var siblingContentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, siblingContentId);
                var siblingContentInfo = Content.GetContentInfo(tableStyle, tableName, siblingContentId);
                var title = siblingContentInfo.Title;
                var url = PageUtility.GetContentUrl(pageInfo.PublishmentSystemInfo, siblingContentInfo);

                //var photoInfo = DataProvider.PhotoDao.GetFirstPhotoInfo(pageInfo.PublishmentSystemId, siblingContentId);
                var photoInfo = Photo.GetFirstPhotoInfo(pageInfo.PublishmentSystemId, siblingContentId);
                var previewUrl = photoInfo != null ? PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, photoInfo.SmallUrl) : SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.FileS);
                builder.Append($@"
    ""prev_album"":{{""title"":""{StringUtils.ToJsString(title)}"",""url"":""{StringUtils.ToJsString(url)}"",""previewUrl"":""{StringUtils
                    .ToJsString(previewUrl)}""}}
");
            }
            else
            {
                builder.Append($@"
    ""prev_album"":{{""title"":"""",""url"":""javascript:void(0);"",""previewUrl"":""{SiteFilesAssets.GetUrl(
                    pageInfo.ApiUrl, SiteFilesAssets.FileS)}""}}
");
            }

            builder.Append(@"
}
</script>
");

            builder.Append($@"
<link href=""{SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.Slide.Css)}"" rel=""stylesheet"" />
<script src=""{SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.Slide.Js)}"" type=""text/javascript"" charset=""gb2312""></script>
");

            builder.Append(TemplateManager.GetContentByFilePath(SiteFilesAssets.GetPath(SiteFilesAssets.Slide.Template)));

            return builder.ToString();
        }
	}
}
