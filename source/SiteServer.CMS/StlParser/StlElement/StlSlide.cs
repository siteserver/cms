using System;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
	public class StlSlide
	{
        private StlSlide() { }
		public const string ElementName = "stl:slide";                      //显示图片幻灯片
		
        public const string Attribute_IsDynamic = "isdynamic";              //是否动态显示

		public static ListDictionary AttributeList
		{
			get
			{
				var attributes = new ListDictionary();
                attributes.Add(Attribute_IsDynamic, "是否动态显示");
				return attributes;
			}
		}

        //对“图片幻灯片”（stl:slide）元素进行解析
        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
			var parsedContent = string.Empty;
            try
            {
                var ie = node.Attributes.GetEnumerator();

                var isDynamic = false;

                while (ie.MoveNext())
                {
                    var attr = (XmlAttribute)ie.Current;
                    var attributeName = attr.Name.ToLower();
                    if (attributeName.Equals(Attribute_IsDynamic))
                    {
                        isDynamic = TranslateUtils.ToBool(attr.Value);
                    }
                }

                if (isDynamic)
                {
                    parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                }
                else
                {
                    parsedContent = ParseImpl(pageInfo, contextInfo);
                }
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

			return parsedContent;
		}

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo)
        {
            pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JsAcSwfObject);

            var contentInfo = contextInfo.ContentInfo;
            if (contentInfo == null)
            {
                contentInfo = DataProvider.ContentDao.GetContentInfo(ETableStyle.BackgroundContent, pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, contextInfo.ContentID);
            }

            var photoInfoList = DataProvider.PhotoDao.GetPhotoInfoList(pageInfo.PublishmentSystemId, contextInfo.ContentID);

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

            var siblingContentID = BaiRongDataProvider.ContentDao.GetContentId(pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, contentInfo.NodeId, contentInfo.Taxis, true);

            if (siblingContentID > 0)
            {
                var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contentInfo.NodeId);
                var tableStyle = NodeManager.GetTableStyle(pageInfo.PublishmentSystemInfo, nodeInfo);
                var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, nodeInfo);
                var siblingContentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, siblingContentID);
                var title = siblingContentInfo.Title;
                var url = PageUtility.GetContentUrl(pageInfo.PublishmentSystemInfo, siblingContentInfo);
                var photoInfo = DataProvider.PhotoDao.GetFirstPhotoInfo(pageInfo.PublishmentSystemId, siblingContentID);
                var previewUrl = string.Empty;
                if (photoInfo != null)
                {
                    previewUrl = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, photoInfo.SmallUrl);
                }
                else
                {
                    previewUrl = SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.FileS);
                }
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

            siblingContentID = BaiRongDataProvider.ContentDao.GetContentId(pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, contentInfo.NodeId, contentInfo.Taxis, false);

            if (siblingContentID > 0)
            {
                var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contentInfo.NodeId);
                var tableStyle = NodeManager.GetTableStyle(pageInfo.PublishmentSystemInfo, nodeInfo);
                var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, nodeInfo);
                var siblingContentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, siblingContentID);
                var title = siblingContentInfo.Title;
                var url = PageUtility.GetContentUrl(pageInfo.PublishmentSystemInfo, siblingContentInfo);

                var photoInfo = DataProvider.PhotoDao.GetFirstPhotoInfo(pageInfo.PublishmentSystemId, siblingContentID);
                var previewUrl = string.Empty;
                if (photoInfo != null)
                {
                    previewUrl = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, photoInfo.SmallUrl);
                }
                else
                {
                    previewUrl = SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.FileS);
                }
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

            builder.Append(StlCacheManager.FileContent.GetContentByFilePath(SiteFilesAssets.GetPath(SiteFilesAssets.Slide.Template), ECharset.utf_8));

            return builder.ToString();
        }
	}
}
