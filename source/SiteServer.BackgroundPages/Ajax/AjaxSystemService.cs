using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using BaiRong.Core;
using SiteServer.BackgroundPages.Admin;
using SiteServer.BackgroundPages.Core;

namespace SiteServer.BackgroundPages.Ajax
{
    public class AjaxSystemService : Page
    {
        private const string TypeGetLoadingDepartments = "GetLoadingDepartments";
        private const string TypeGetLoadingAreas = "GetLoadingAreas";

        public static string GetLoadingDepartmentsUrl()
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxSystemService), new NameValueCollection
            {
                {"type", TypeGetLoadingDepartments }
            });
        }

        public static string GetLoadingDepartmentsParameters(EDepartmentLoadingType loadingType, NameValueCollection additional)
        {
            return TranslateUtils.NameValueCollectionToString(new NameValueCollection
            {
                {"loadingType", EDepartmentLoadingTypeUtils.GetValue(loadingType)},
                {"additional", TranslateUtils.EncryptStringBySecretKey(TranslateUtils.NameValueCollectionToString(additional))}
            });
        }

        public static string GetLoadingAreasUrl()
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxSystemService), new NameValueCollection
            {
                {"type", TypeGetLoadingAreas }
            });
        }

        public static string GetLoadingAreasParameters(EAreaLoadingType loadingType, NameValueCollection additional)
        {
            return TranslateUtils.NameValueCollectionToString(new NameValueCollection
            {
                {"loadingType", EAreaLoadingTypeUtils.GetValue(loadingType)},
                {"additional", TranslateUtils.EncryptStringBySecretKey(TranslateUtils.NameValueCollectionToString(additional))}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            var type = Request.QueryString["type"];
            var retString = string.Empty;

            if (type == TypeGetLoadingDepartments)
            {
                var parentId = TranslateUtils.ToInt(Request["parentID"]);
                var loadingType = Request["loadingType"];
                var additional = Request["additional"];
                retString = GetLoadingDepartments(parentId, loadingType, additional);
            }
            else if (type == TypeGetLoadingAreas)
            {
                var parentId = TranslateUtils.ToInt(Request["parentID"]);
                var loadingType = Request["loadingType"];
                var additional = Request["additional"];
                retString = GetLoadingAreas(parentId, loadingType, additional);
            }

            Page.Response.Write(retString);
            Page.Response.End();
        }

        public string GetLoadingDepartments(int parentId, string loadingType, string additional)
        {
            var arraylist = new ArrayList();

            var eLoadingType = EDepartmentLoadingTypeUtils.GetEnumType(loadingType);

            var departmentIdList = BaiRongDataProvider.DepartmentDao.GetDepartmentIdListByParentId(parentId);
            var nameValueCollection = TranslateUtils.ToNameValueCollection(TranslateUtils.DecryptStringBySecretKey(additional));
            if (!string.IsNullOrEmpty(nameValueCollection["DepartmentIDCollection"]))
            {
                var allDepartmentIdArrayList = TranslateUtils.StringCollectionToIntList(nameValueCollection["DepartmentIDCollection"]);
                nameValueCollection.Remove("DepartmentIDCollection");
                foreach (var departmentId in departmentIdList)
                {
                    var departmentInfo = DepartmentManager.GetDepartmentInfo(departmentId);
                    if (departmentInfo.ParentId != 0 || allDepartmentIdArrayList.Contains(departmentId))
                    {
                        arraylist.Add(PageDepartment.GetDepartmentRowHtml(departmentInfo, eLoadingType, nameValueCollection));
                    }
                }
            }
            else
            {
                foreach (var departmentId in departmentIdList)
                {
                    var departmentInfo = DepartmentManager.GetDepartmentInfo(departmentId);
                    arraylist.Add(PageDepartment.GetDepartmentRowHtml(departmentInfo, eLoadingType, nameValueCollection));
                }
            }

            var builder = new StringBuilder();
            foreach (string html in arraylist)
            {
                builder.Append(html);
            }
            return builder.ToString();
        }

        public string GetLoadingAreas(int parentId, string loadingType, string additional)
        {
            var arraylist = new ArrayList();

            var eLoadingType = EAreaLoadingTypeUtils.GetEnumType(loadingType);

            var areaIdList = BaiRongDataProvider.AreaDao.GetAreaIdListByParentId(parentId);
            var nameValueCollection = TranslateUtils.ToNameValueCollection(TranslateUtils.DecryptStringBySecretKey(additional));
            if (!string.IsNullOrEmpty(nameValueCollection["AreaIDCollection"]))
            {
                var allAreaIdArrayList = TranslateUtils.StringCollectionToIntList(nameValueCollection["AreaIDCollection"]);
                nameValueCollection.Remove("AreaIDCollection");
                foreach (var areaId in areaIdList)
                {
                    var areaInfo = AreaManager.GetAreaInfo(areaId);
                    if (areaInfo.ParentId != 0 || allAreaIdArrayList.Contains(areaId))
                    {
                        arraylist.Add(PageArea.GetAreaRowHtml(areaInfo, eLoadingType, nameValueCollection));
                    }
                }
            }
            else
            {
                foreach (var areaId in areaIdList)
                {
                    var areaInfo = AreaManager.GetAreaInfo(areaId);
                    arraylist.Add(PageArea.GetAreaRowHtml(areaInfo, eLoadingType, nameValueCollection));
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
