using BaiRong.Core;
using SiteServer.WeiXin.Core;
using System.Collections;
using System.Collections.Specialized;
using System.Text;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundService : BackgroundBasePageWX
    {
        public const string TYPE_GetLoadingCategorys = "GetLoadingCategorys";

        public static string GetRedirectUrl(int publishmentSystemID, string type)
        {
            return PageUtils.GetWXUrl($"background_service.aspx?PublishmentSystemID={publishmentSystemID}&type={type}");
        }

        public void Page_Load(object sender, System.EventArgs e)
        {
            var type = Request.QueryString["type"];

            var retval = new NameValueCollection();
            var retString = string.Empty;

            if (type == TYPE_GetLoadingCategorys)
            {
                var parentID = TranslateUtils.ToInt(Request["parentID"]);
                var loadingType = Request["loadingType"];
                var additional = Request["additional"];
                retString = GetLoadingCategorys(parentID, loadingType, additional);
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

        public string GetLoadingCategorys(int parentID, string loadingType, string additional)
        {
            var arraylist = new ArrayList();

            var eLoadingType = ECategoryLoadingTypeUtils.GetEnumType(loadingType);

            var categoryIDList = DataProviderWX.StoreCategoryDAO.GetCategoryIDListByParentID(PublishmentSystemID, parentID);
            var nameValueCollection = TranslateUtils.ToNameValueCollection(RuntimeUtils.DecryptStringByTranslate(additional));
            var allCategoryIDArrayList = new ArrayList();
            if (!string.IsNullOrEmpty(nameValueCollection["CategoryIDCollection"]))
            {
                allCategoryIDArrayList = TranslateUtils.StringCollectionToIntArrayList(nameValueCollection["CategoryIDCollection"]);
                nameValueCollection.Remove("CategoryIDCollection");
                foreach (var categotyID in categoryIDList)
                {
                    var categoryInfo = DataProviderWX.StoreCategoryDAO.GetCategoryInfo(categotyID);
                    if (categoryInfo.ParentID != 0 || allCategoryIDArrayList.Contains(categotyID))
                    {
                        arraylist.Add(BackgroundStoreCategory.GetCategoryRowHtml(PublishmentSystemID, categoryInfo, eLoadingType, nameValueCollection));
                    }
                }
            }
            else
            {
                foreach (var categotyID in categoryIDList)
                {
                    var categoryInfo = DataProviderWX.StoreCategoryDAO.GetCategoryInfo(categotyID);
                    arraylist.Add(BackgroundStoreCategory.GetCategoryRowHtml(PublishmentSystemID, categoryInfo, eLoadingType, nameValueCollection));
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
