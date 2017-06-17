using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using BaiRong.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.WeiXin.Core;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageService : BasePageCms
    {
        public const string TypeGetLoadingCategorys = "GetLoadingCategorys";

        public static string GetRedirectUrl(int publishmentSystemId, string type)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageService), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"type", type}
            });
        }

        public void Page_Load(object sender, System.EventArgs e)
        {
            var type = Request.QueryString["type"];

            var retval = new NameValueCollection();
            var retString = string.Empty;

            if (type == TypeGetLoadingCategorys)
            {
                var parentId = TranslateUtils.ToInt(Request["parentID"]);
                var loadingType = Request["loadingType"];
                var additional = Request["additional"];
                retString = GetLoadingCategorys(parentId, loadingType, additional);
            }

            if (!string.IsNullOrEmpty(retString))
            {
                Page.Response.Write(retString);
                Page.Response.End();
            }
            else
            {
                var jsonString = TranslateUtils.NameValueCollectionToJsonString(retval);
                Page.Response.Write(jsonString);
                Page.Response.End();
            }
        }

        public string GetLoadingCategorys(int parentId, string loadingType, string additional)
        {
            var arraylist = new ArrayList();

            var eLoadingType = ECategoryLoadingTypeUtils.GetEnumType(loadingType);

            var categoryIdList = DataProviderWx.StoreCategoryDao.GetCategoryIdListByParentId(PublishmentSystemId, parentId);
            var nameValueCollection = TranslateUtils.ToNameValueCollection(TranslateUtils.DecryptStringBySecretKey(additional));
            var allCategoryIdList = new List<int>();
            if (!string.IsNullOrEmpty(nameValueCollection["CategoryIDCollection"]))
            {
                allCategoryIdList = TranslateUtils.StringCollectionToIntList(nameValueCollection["CategoryIDCollection"]);
                nameValueCollection.Remove("CategoryIDCollection");
                foreach (var categotyId in categoryIdList)
                {
                    var categoryInfo = DataProviderWx.StoreCategoryDao.GetCategoryInfo(categotyId);
                    if (categoryInfo.ParentId != 0 || allCategoryIdList.Contains(categotyId))
                    {
                        arraylist.Add(PageStoreCategory.GetCategoryRowHtml(PublishmentSystemId, categoryInfo, eLoadingType, nameValueCollection));
                    }
                }
            }
            else
            {
                foreach (var categotyId in categoryIdList)
                {
                    var categoryInfo = DataProviderWx.StoreCategoryDao.GetCategoryInfo(categotyId);
                    arraylist.Add(PageStoreCategory.GetCategoryRowHtml(PublishmentSystemId, categoryInfo, eLoadingType, nameValueCollection));
                }
            }

            var builder = new StringBuilder();
            foreach (string html in arraylist)
            {
                builder.Append(html);
            }
            return builder.ToString();
        }
    }
}
