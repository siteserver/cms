using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using BaiRong.Core;
using SiteServer.BackgroundPages.Core;
using SiteServer.BackgroundPages.Wcm;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Ajax
{
    public class AjaxWcmService : Page
    {
        private const string TypeGetLoadingGovPublicCategories = "GetLoadingGovPublicCategories";

        public static string GetLoadingGovPublicCategoriesUrl()
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxWcmService), new NameValueCollection
            {
                {"type", TypeGetLoadingGovPublicCategories }
            });
        }

        public static string GetLoadingGovPublicCategoriesParameters(int publishmentSystemId, string classCode, EGovPublicCategoryLoadingType loadingType, NameValueCollection additional)
        {
            return TranslateUtils.NameValueCollectionToString(new NameValueCollection
            {
                {"publishmentSystemID", publishmentSystemId.ToString()},
                {"classCode", classCode},
                {"loadingType", EGovPublicCategoryLoadingTypeUtils.GetValue(loadingType)},
                {"additional", TranslateUtils.EncryptStringBySecretKey(TranslateUtils.NameValueCollectionToString(additional))}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            var type = Request["type"];
            var retString = string.Empty;

            if (type == TypeGetLoadingGovPublicCategories)
            {
                var classCode = Request["classCode"];
                var publishmentSystemId = TranslateUtils.ToInt(Request["publishmentSystemID"]);
                var parentId = TranslateUtils.ToInt(Request["parentID"]);
                var loadingType = Request["loadingType"];
                var additional = Request["additional"];
                retString = GetLoadingGovPublicCategories(classCode, publishmentSystemId, parentId, loadingType, additional);
            }

            Page.Response.Write(retString);
            Page.Response.End();
        }

        public string GetLoadingGovPublicCategories(string classCode, int publishmentSystemId, int parentId, string loadingType, string additional)
        {
            var arraylist = new ArrayList();

            var eLoadingType = EGovPublicCategoryLoadingTypeUtils.GetEnumType(loadingType);

            var categoryIdArrayList = DataProvider.GovPublicCategoryDao.GetCategoryIdArrayListByParentId(classCode, publishmentSystemId, parentId);
            var nameValueCollection = TranslateUtils.ToNameValueCollection(TranslateUtils.DecryptStringBySecretKey(additional));

            foreach (int categoryId in categoryIdArrayList)
            {
                var categoryInfo = DataProvider.GovPublicCategoryDao.GetCategoryInfo(categoryId);
                arraylist.Add(PageGovPublicCategory.GetCategoryRowHtml(categoryInfo, true, eLoadingType, nameValueCollection));
            }

            //arraylist.Reverse();

            var builder = new StringBuilder();
            foreach (string html in arraylist)
            {
                builder.Append(html);
            }
            return builder.ToString();
        }
    }
}
