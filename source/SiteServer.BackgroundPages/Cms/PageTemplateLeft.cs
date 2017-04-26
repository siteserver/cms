using System;
using System.Collections.Generic;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageTemplateLeft : BasePageCms
    {
        private Dictionary<ETemplateType, int> _dictionary;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            _dictionary = DataProvider.TemplateDao.GetCountDictionary(PublishmentSystemId);
        }

        public string GetServiceUrl()
        {
            return PageServiceStl.GetRedirectUrl(PageServiceStl.TypeGetLoadingTemplates);
        }

        public int GetCount(string templateType)
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
