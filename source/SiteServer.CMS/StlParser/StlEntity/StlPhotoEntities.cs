using System.Collections.Generic;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlEntity
{
    [Stl(Usage = "图片实体", Description = "通过 {photo.} 实体在模板中显示相册图片")]
    public class StlPhotoEntities
	{
        private StlPhotoEntities()
		{
		}

        public const string EntityName = "photo";

        public static string PhotoId = "PhotoID";
        public static string SmallUrl = "SmallUrl";
        public static string MiddleUrl = "MiddleUrl";
        public static string LargeUrl = "LargeUrl";
        public static string Description = "Description";
        public static string ItemIndex = "ItemIndex";

	    public static SortedList<string, string> AttributeList => new SortedList<string, string>
	    {
	        {PhotoId, "图片ID"},
	        {SmallUrl, "小图地址"},
	        {MiddleUrl, "中图地址"},
	        {LargeUrl, "大图地址"},
	        {Description, "图片说明"},
	        {ItemIndex, "图片排序"}
	    };

        internal static string Parse(string stlEntity, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;

            if (contextInfo.ItemContainer?.PhotoItem == null) return string.Empty;

            try
            {
                var entityName = StlParserUtility.GetNameFromEntity(stlEntity);

                var type = entityName.Substring(7, entityName.Length - 8).ToLower();

                var photoInfo = new PhotoInfo(contextInfo.ItemContainer.PhotoItem.DataItem);

                if (StringUtils.EqualsIgnoreCase(type, PhotoId))
                {
                    parsedContent = photoInfo.ID.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(type, SmallUrl))
                {
                    parsedContent = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, photoInfo.SmallUrl);
                }
                else if (StringUtils.EqualsIgnoreCase(type, MiddleUrl))
                {
                    parsedContent = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, photoInfo.MiddleUrl);
                }
                else if (StringUtils.EqualsIgnoreCase(type, LargeUrl))
                {
                    parsedContent = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, photoInfo.LargeUrl);
                }
                else if (StringUtils.EqualsIgnoreCase(type, Description))
                {
                    parsedContent = photoInfo.Description;
                }
                else if (StringUtils.StartsWithIgnoreCase(type, StlParserUtility.ItemIndex))
                {
                    parsedContent = StlParserUtility.ParseItemIndex(contextInfo.ItemContainer.PhotoItem.ItemIndex, type, contextInfo).ToString();
                }
            }
            catch
            {
                // ignored
            }

            return parsedContent;
        }
	}
}
