using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.Plugin;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageTemplateLeft : BasePageCms
    {
        public Literal LtlTotalCount;
        public Literal LtlIndexPageCount;
        public Literal LtlChannelCount;
        public Literal LtlContentCount;
        public Literal LtlFileCount;

        private Dictionary<TemplateType, int> _dictionary;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            if (IsPostBack) return;

            _dictionary = DataProvider.TemplateDao.GetCountDictionary(SiteId);

            LtlTotalCount.Text = $"({GetCount(string.Empty)})";
            LtlIndexPageCount.Text = $"({GetCount("IndexPageTemplate")})";
            LtlChannelCount.Text = $"({GetCount("ChannelTemplate")})";
            LtlContentCount.Text = $"({GetCount("ContentTemplate")})";
            LtlFileCount.Text = $"({GetCount("FileTemplate")})";
        }

        public string GetServiceUrl()
        {
            return PageServiceStl.GetRedirectUrl(SiteId, PageServiceStl.TypeGetLoadingTemplates);
        }

        public string GetServiceParams()
        {
            return $"siteID={SiteId}&templateType=";
        }

        private int GetCount(string templateType)
        {
            var count = 0;
            if (string.IsNullOrEmpty(templateType))
            {
                foreach (var value in _dictionary)
                {
                    count += value.Value;
                }
            }
            else
            {
                var eTemplateType = TemplateTypeUtils.GetEnumType(templateType);
                if (_dictionary.ContainsKey(eTemplateType))
                {
                    count = _dictionary[eTemplateType];
                }
            }
            return count;
        }
    }
}
