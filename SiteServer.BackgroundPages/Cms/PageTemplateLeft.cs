using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageTemplateLeft : BasePageCms
    {
        public Literal LtlTotalCount;
        public Literal LtlIndexPageCount;
        public Literal LtlChannelCount;
        public Literal LtlContentCount;
        public Literal LtlFileCount;

        private Dictionary<ETemplateType, int> _dictionary;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (IsPostBack) return;

            _dictionary = DataProvider.TemplateDao.GetCountDictionary(PublishmentSystemId);

            LtlTotalCount.Text = $"({GetCount(string.Empty)})";
            LtlIndexPageCount.Text = $"({GetCount("IndexPageTemplate")})";
            LtlChannelCount.Text = $"({GetCount("ChannelTemplate")})";
            LtlContentCount.Text = $"({GetCount("ContentTemplate")})";
            LtlFileCount.Text = $"({GetCount("FileTemplate")})";
        }

        public string GetServiceUrl()
        {
            return PageServiceStl.GetRedirectUrl(PageServiceStl.TypeGetLoadingTemplates);
        }

        public string GetServiceParams()
        {
            return $"publishmentSystemID={PublishmentSystemId}&templateType=";
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
                var eTemplateType = ETemplateTypeUtils.GetEnumType(templateType);
                if (_dictionary.ContainsKey(eTemplateType))
                {
                    count = _dictionary[eTemplateType];
                }
            }
            return count;
        }
    }
}
